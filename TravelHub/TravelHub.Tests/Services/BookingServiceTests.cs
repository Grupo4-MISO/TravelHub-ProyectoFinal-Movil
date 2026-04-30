using App.DTOs;
using App.Providers.Interfaces;
using App.Responses;
using App.Services.Implementations;
using App.Services.Interfaces;
using Moq;
using Moq.Protected;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using Xunit;

namespace TravelHub.Tests.Services;

public class BookingServiceTests
{
    private readonly Mock<IBackendUrlProvider> _backendUrlProviderMock;
    private readonly Mock<IBackEndService> _backEndServiceMock;
    private readonly BookingService _bookingService;

    public BookingServiceTests()
    {
        _backendUrlProviderMock = new Mock<IBackendUrlProvider>();
        _backEndServiceMock = new Mock<IBackEndService>();
        _bookingService = new BookingService(_backEndServiceMock.Object, _backendUrlProviderMock.Object);
    }

    [Fact]
    public async Task GetUserBookingsAsync_Should_Return_Bookings_When_Api_Returns_Success()
    {
        // Arrange
        var userId = "user-123";
        var expectedBookings = new List<BookingResponseDto>
        {
            new BookingResponseDto
            {
                Id = "booking-1",
                PublicId = "RSV-12345",
                HabitacionId = "room-1",
                CheckIn = "2026-05-01",
                CheckOut = "2026-05-03",
                Estado = "confirmada",
                CreatedAt = "2026-04-30T03:24:36.124491",
                UpdatedAt = "2026-04-30T03:03:50.916582"
            }
        };

        var url = "https://api.test.com/api/v1/reservas/usuario/user-123";
        _backendUrlProviderMock.Setup(x => x.Build(It.IsAny<string>())).Returns(url);

        _backEndServiceMock
            .Setup(x => x.GetAsync<List<BookingResponseDto>>(url))
            .ReturnsAsync(new HttpResponseWrapper<List<BookingResponseDto>>(expectedBookings, false, new HttpResponseMessage(HttpStatusCode.OK)));

        // Act
        var result = await _bookingService.GetUserBookingsAsync(userId);

        // Assert
        Assert.False(result.Error);
        Assert.NotNull(result.Response);
        Assert.Single(result.Response);
        Assert.Equal("booking-1", result.Response[0].Id);
        Assert.Equal("RSV-12345", result.Response[0].PublicId);
        Assert.Equal("room-1", result.Response[0].HabitacionId);
    }

    [Fact]
    public async Task GetUserBookingsAsync_Should_Return_Error_When_Api_Returns_Failure()
    {
        // Arrange
        var userId = "user-123";
        var url = "https://api.test.com/api/v1/reservas/usuario/user-123";
        _backendUrlProviderMock.Setup(x => x.Build(It.IsAny<string>())).Returns(url);

        _backEndServiceMock
            .Setup(x => x.GetAsync<List<BookingResponseDto>>(url))
            .ReturnsAsync(new HttpResponseWrapper<List<BookingResponseDto>>(null, true, new HttpResponseMessage(HttpStatusCode.InternalServerError)));

        // Act
        var result = await _bookingService.GetUserBookingsAsync(userId);

        // Assert
        Assert.True(result.Error);
        Assert.Null(result.Response);
    }

    [Fact]
    public async Task GetUserBookingsAsync_Should_Use_Correct_Endpoint_With_UserId()
    {
        // Arrange
        var userId = "test-user-id";
        string? capturedUrl = null;

        _backendUrlProviderMock
            .Setup(x => x.Build(It.IsAny<string>()))
            .Returns((string path) => $"https://api.test.com{path}");

        _backEndServiceMock
            .Setup(x => x.GetAsync<List<BookingResponseDto>>(It.IsAny<string>()))
            .Callback<string>(url => capturedUrl = url)
            .ReturnsAsync(new HttpResponseWrapper<List<BookingResponseDto>>(new List<BookingResponseDto>(), false, new HttpResponseMessage(HttpStatusCode.OK)));

        // Act
        await _bookingService.GetUserBookingsAsync(userId);

        // Assert
        Assert.Contains("/api/v1/reservas/usuario/test-user-id", capturedUrl);
    }

    [Fact]
    public async Task GetHotelsByRoomIdsAsync_Should_Return_Hotels_When_Api_Returns_Success()
    {
        // Arrange
        var roomIds = new List<string> { "room-1", "room-2" };
        var expectedHotels = new Dictionary<string, HotelInventoryDto>
        {
            { "room-1", new HotelInventoryDto { Nombre = "Hotel A", Ciudad = "Buenos Aires", Pais = "Argentina", Direccion = "Av. Principal 109", Imagen = "https://image.com/a.jpg" } },
            { "room-2", new HotelInventoryDto { Nombre = "Hotel B", Ciudad = "Córdoba", Pais = "Argentina", Direccion = "Calle 10", Imagen = "https://image.com/b.jpg" } }
        };

        var url = "https://api.test.com/api/v1/inventarios/hoteles";
        _backendUrlProviderMock.Setup(x => x.Build(It.IsAny<string>())).Returns(url);

        _backEndServiceMock
            .Setup(x => x.PostAsync<object, Dictionary<string, HotelInventoryDto>>(url, It.IsAny<object>()))
            .ReturnsAsync(new HttpResponseWrapper<Dictionary<string, HotelInventoryDto>>(expectedHotels, false, new HttpResponseMessage(HttpStatusCode.OK)));

        // Act
        var result = await _bookingService.GetHotelsByRoomIdsAsync(roomIds);

        // Assert
        Assert.False(result.Error);
        Assert.NotNull(result.Response);
        Assert.Equal(2, result.Response.Count);
        Assert.True(result.Response.ContainsKey("room-1"));
        Assert.True(result.Response.ContainsKey("room-2"));
    }

    [Fact]
    public async Task GetHotelsByRoomIdsAsync_Should_Send_Correct_Payload_With_HabitacionesIds()
    {
        // Arrange
        var roomIds = new List<string> { "room-1", "room-2", "room-3" };
        object? capturedPayload = null;

        var url = "https://api.test.com/api/v1/inventarios/hoteles";
        _backendUrlProviderMock.Setup(x => x.Build(It.IsAny<string>())).Returns(url);

        _backEndServiceMock
            .Setup(x => x.PostAsync<object, Dictionary<string, HotelInventoryDto>>(url, It.IsAny<object>()))
            .Callback<string, object>((url, payload) => capturedPayload = payload)
            .ReturnsAsync(new HttpResponseWrapper<Dictionary<string, HotelInventoryDto>>(new Dictionary<string, HotelInventoryDto>(), false, new HttpResponseMessage(HttpStatusCode.OK)));

        // Act
        await _bookingService.GetHotelsByRoomIdsAsync(roomIds);

        // Assert
        Assert.NotNull(capturedPayload);
        
        // Verificar que el payload tiene la propiedad habitaciones_ids
        var json = JsonSerializer.Serialize(capturedPayload);
        using var document = JsonDocument.Parse(json);
        var root = document.RootElement;
        
        Assert.True(root.TryGetProperty("habitaciones_ids", out var habitacionesIds));
        Assert.Equal(3, habitacionesIds.GetArrayLength());
        Assert.Contains("room-1", habitacionesIds.ToString());
    }

    [Fact]
    public async Task GetHotelsByRoomIdsAsync_Should_Return_Error_When_Api_Returns_Failure()
    {
        // Arrange
        var roomIds = new List<string> { "room-1" };
        var url = "https://api.test.com/api/v1/inventarios/hoteles";
        _backendUrlProviderMock.Setup(x => x.Build(It.IsAny<string>())).Returns(url);

        _backEndServiceMock
            .Setup(x => x.PostAsync<object, Dictionary<string, HotelInventoryDto>>(url, It.IsAny<object>()))
            .ReturnsAsync(new HttpResponseWrapper<Dictionary<string, HotelInventoryDto>>(null, true, new HttpResponseMessage(HttpStatusCode.BadRequest)));

        // Act
        var result = await _bookingService.GetHotelsByRoomIdsAsync(roomIds);

        // Assert
        Assert.True(result.Error);
        Assert.Null(result.Response);
    }

    [Fact]
    public async Task GetHotelsByRoomIdsAsync_Should_Handle_Empty_RoomIds_List()
    {
        // Arrange
        var roomIds = new List<string>();
        var url = "https://api.test.com/api/v1/inventarios/hoteles";
        _backendUrlProviderMock.Setup(x => x.Build(It.IsAny<string>())).Returns(url);

        _backEndServiceMock
            .Setup(x => x.PostAsync<object, Dictionary<string, HotelInventoryDto>>(url, It.IsAny<object>()))
            .ReturnsAsync(new HttpResponseWrapper<Dictionary<string, HotelInventoryDto>>(new Dictionary<string, HotelInventoryDto>(), false, new HttpResponseMessage(HttpStatusCode.OK)));

        // Act
        var result = await _bookingService.GetHotelsByRoomIdsAsync(roomIds);

        // Assert
        Assert.False(result.Error);
        Assert.NotNull(result.Response);
        Assert.Empty(result.Response);
    }
}
