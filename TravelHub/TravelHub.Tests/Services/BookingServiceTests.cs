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
        var expectedBookings = new List<BookingHoldResponseDto>
        {
            new BookingHoldResponseDto
            {
                Id = "booking-1",
                PublicId = "RSV-12345",
                HabitacionId = "room-1",
                CheckIn = "2026-05-01",
                CheckOut = "2026-05-03",
                Estado = "confirmada",
                CreatedAt = DateTime.Parse("2026-04-30T03:24:36.124491"),
                UpdatedAt = DateTime.Parse("2026-04-30T03:03:50.916582")
            }
        };

        var url = "https://api.test.com/api/v1/reservas/usuario/user-123";
        _backendUrlProviderMock.Setup(x => x.Build(It.IsAny<string>())).Returns(url);

        _backEndServiceMock
            .Setup(x => x.GetAsync<List<BookingHoldResponseDto>>(url))
            .ReturnsAsync(new HttpResponseWrapper<List<BookingHoldResponseDto>>(expectedBookings, false, new HttpResponseMessage(HttpStatusCode.OK)));

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
            .Setup(x => x.GetAsync<List<BookingHoldResponseDto>>(url))
            .ReturnsAsync(new HttpResponseWrapper<List<BookingHoldResponseDto>>(null, true, new HttpResponseMessage(HttpStatusCode.InternalServerError)));

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
            .Setup(x => x.GetAsync<List<BookingHoldResponseDto>>(It.IsAny<string>()))
            .Callback<string>(url => capturedUrl = url)
            .ReturnsAsync(new HttpResponseWrapper<List<BookingHoldResponseDto>>(new List<BookingHoldResponseDto>(), false, new HttpResponseMessage(HttpStatusCode.OK)));

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

    [Fact]
    public async Task CreateReservationHoldAsync_Should_Call_Hold_Endpoint_With_Correct_Payload()
    {
        // Arrange
        var holdPayload = new ReservationHoldRequestDto
        {
            UserId = "user-123",
            HabitacionId = "room-456",
            CheckIn = "2026-06-01",
            CheckOut = "2026-06-03"
        };
        var url = "https://dpyrs6tuvj15e.cloudfront.net/api/v1/reservas/hold";

        _backendUrlProviderMock
            .Setup(x => x.Build("/api/v1/reservas/hold"))
            .Returns(url);

        _backEndServiceMock
            .Setup(x => x.PostAsync(url, It.IsAny<ReservationHoldRequestDto>()))
            .ReturnsAsync(new HttpResponseWrapper<object>(null, false, new HttpResponseMessage(HttpStatusCode.Created)));

        // Act
        var result = await _bookingService.CreateReservationHoldAsync(holdPayload);

        // Assert
        Assert.False(result.Error);
        _backEndServiceMock.Verify(x => x.PostAsync(url, It.Is<ReservationHoldRequestDto>(payload =>
            payload.UserId == holdPayload.UserId &&
            payload.HabitacionId == holdPayload.HabitacionId &&
            payload.CheckIn == holdPayload.CheckIn &&
            payload.CheckOut == holdPayload.CheckOut
        )), Times.Once);
    }

    [Fact]
    public async Task CreateReservationHoldAsync_Should_Return_Error_When_Api_Fails()
    {
        // Arrange
        var holdPayload = new ReservationHoldRequestDto
        {
            UserId = "user-123",
            HabitacionId = "room-456",
            CheckIn = "2026-06-01",
            CheckOut = "2026-06-03"
        };
        var url = "https://dpyrs6tuvj15e.cloudfront.net/api/v1/reservas/hold";

        _backendUrlProviderMock
            .Setup(x => x.Build("/api/v1/reservas/hold"))
            .Returns(url);

        _backEndServiceMock
            .Setup(x => x.PostAsync(url, It.IsAny<ReservationHoldRequestDto>()))
            .ReturnsAsync(new HttpResponseWrapper<object>(null, true, new HttpResponseMessage(HttpStatusCode.BadRequest)));

        // Act
        var result = await _bookingService.CreateReservationHoldAsync(holdPayload);

        // Assert
        Assert.True(result.Error);
    }

    [Fact]
    public async Task GetPaymentProvidersAsync_Should_Call_Transactions_Providers_Endpoint()
    {
        // Arrange
        var url = "https://dpyrs6tuvj15e.cloudfront.net/api/v1/Transactions/providers";
        var expectedProviders = new List<PaymentProviderDto>
        {
            new PaymentProviderDto
            {
                Id = "9a4a97e8-7f0e-46fb-a451-d7de8ef19ba5",
                Name = "Stripe",
                IsActive = true,
                Logo = "https://logos-world.net/wp-content/uploads/2021/03/Stripe-Logo.png"
            }
        };

        _backendUrlProviderMock
            .Setup(x => x.Build("/api/v1/Transactions/providers"))
            .Returns(url);

        _backEndServiceMock
            .Setup(x => x.GetAsync<List<PaymentProviderDto>>(url))
            .ReturnsAsync(new HttpResponseWrapper<List<PaymentProviderDto>>(expectedProviders, false, new HttpResponseMessage(HttpStatusCode.OK)));

        // Act
        var result = await _bookingService.GetPaymentProvidersAsync();

        // Assert
        Assert.False(result.Error);
        Assert.NotNull(result.Response);
        Assert.Single(result.Response);
        Assert.Equal("Stripe", result.Response[0].Name);
    }

    [Fact]
    public async Task GetPaymentProvidersAsync_Should_Return_Error_When_Api_Returns_Failure()
    {
        // Arrange
        var url = "https://dpyrs6tuvj15e.cloudfront.net/api/v1/Transactions/providers";

        _backendUrlProviderMock
            .Setup(x => x.Build("/api/v1/Transactions/providers"))
            .Returns(url);

        _backEndServiceMock
            .Setup(x => x.GetAsync<List<PaymentProviderDto>>(url))
            .ReturnsAsync(new HttpResponseWrapper<List<PaymentProviderDto>>(null, true, new HttpResponseMessage(HttpStatusCode.BadRequest)));

        // Act
        var result = await _bookingService.GetPaymentProvidersAsync();

        // Assert
        Assert.True(result.Error);
    }

    [Fact]
    public async Task GetPaymentsByReservationAsync_Should_Return_Payments_When_Api_Succeeds()
    {
        // Arrange
        var reservaId = "d4915cc4-bb74-4a15-b5a1-bb3c174f94ff";
        var expectedPayments = new List<PaymentReservationDTO>
        {
            new PaymentReservationDTO
            {
                Id = "73933606-ccd0-4f1d-894d-6c3be12d99b2",
                ReservaId = reservaId,
                ProviderId = "7f93f279-130f-4231-9458-b976925d40eb",
                Amount = 1035639.0m,
                Currency = "ARS",
                Status = "authorized",
                Description = "Pago por reserva del hotel"
            }
        };

        var url = $"https://dpyrs6tuvj15e.cloudfront.net/api/v1/api/v1/Transactions/payments/reserva/{reservaId}";
        _backendUrlProviderMock
            .Setup(x => x.Build(It.IsAny<string>()))
            .Returns(url);

        _backEndServiceMock
            .Setup(x => x.GetAsync<List<PaymentReservationDTO>>(url))
            .ReturnsAsync(new HttpResponseWrapper<List<PaymentReservationDTO>>(expectedPayments, false, new HttpResponseMessage(HttpStatusCode.OK)));

        // Act
        var result = await _bookingService.GetPaymentsByReservationAsync(reservaId);

        // Assert
        Assert.False(result.Error);
        Assert.NotNull(result.Response);
        Assert.Single(result.Response);
        Assert.Equal("73933606-ccd0-4f1d-894d-6c3be12d99b2", result.Response[0].Id);
        Assert.Equal(1035639.0m, result.Response[0].Amount);
        Assert.Equal("ARS", result.Response[0].Currency);
        Assert.Equal("authorized", result.Response[0].Status);
    }

    [Fact]
    public async Task GetPaymentsByReservationAsync_Should_Return_Error_When_Api_Fails()
    {
        // Arrange
        var reservaId = "d4915cc4-bb74-4a15-b5a1-bb3c174f94ff";
        var url = $"https://dpyrs6tuvj15e.cloudfront.net/api/v1/api/v1/Transactions/payments/reserva/{reservaId}";
        _backendUrlProviderMock
            .Setup(x => x.Build(It.IsAny<string>()))
            .Returns(url);

        _backEndServiceMock
            .Setup(x => x.GetAsync<List<PaymentReservationDTO>>(url))
            .ReturnsAsync(new HttpResponseWrapper<List<PaymentReservationDTO>>(null, true, new HttpResponseMessage(HttpStatusCode.InternalServerError)));

        // Act
        var result = await _bookingService.GetPaymentsByReservationAsync(reservaId);

        // Assert
        Assert.True(result.Error);
        Assert.Null(result.Response);
    }

    [Fact]
    public async Task GetPaymentsByReservationAsync_Should_Use_Correct_Endpoint_With_ReservaId()
    {
        // Arrange
        var reservaId = "d4915cc4-bb74-4a15-b5a1-bb3c174f94ff";
        string? capturedUrl = null;

        _backendUrlProviderMock
            .Setup(x => x.Build(It.IsAny<string>()))
            .Returns((string path) => $"https://api.test.com{path}");

        _backEndServiceMock
            .Setup(x => x.GetAsync<List<PaymentReservationDTO>>(It.IsAny<string>()))
            .Callback<string>(url => capturedUrl = url)
            .ReturnsAsync(new HttpResponseWrapper<List<PaymentReservationDTO>>(new List<PaymentReservationDTO>(), false, new HttpResponseMessage(HttpStatusCode.OK)));

        // Act
        await _bookingService.GetPaymentsByReservationAsync(reservaId);

        // Assert
        Assert.Contains("/api/v1/Transactions/payments/reserva/d4915cc4-bb74-4a15-b5a1-bb3c174f94ff", capturedUrl);
    }
}
