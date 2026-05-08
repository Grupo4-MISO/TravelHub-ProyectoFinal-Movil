using App.DTOs;
using App.Responses;
using App.Services.Interfaces;
using App.ViewModels;
using Moq;
using System.Net;
using Xunit;

namespace TravelHub.Tests.ViewModels;

public class BookingDetailsViewModelTests
{
    private readonly Mock<IBookingService> _bookingServiceMock;
    private readonly Mock<IPropertyDetailService> _propertyDetailServiceMock;
    private readonly BookingDetailsViewModel _viewModel;

    public BookingDetailsViewModelTests()
    {
        _bookingServiceMock = new Mock<IBookingService>();
        _propertyDetailServiceMock = new Mock<IPropertyDetailService>();

        _bookingServiceMock
            .Setup(x => x.GetBookingByReservationIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new HttpResponseWrapper<BookingResponseDto>(null!, false, new HttpResponseMessage(HttpStatusCode.OK)));

        _viewModel = new BookingDetailsViewModel(
            _bookingServiceMock.Object,
            _propertyDetailServiceMock.Object);
    }

    [Fact]
    public void Constructor_SetsTitleCorrectly()
    {
        Assert.Equal("Detalle de Reserva", _viewModel.Title);
    }

    [Fact]
    public void Constructor_Throws_WhenBookingServiceNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new BookingDetailsViewModel(null!, _propertyDetailServiceMock.Object));
    }

    [Fact]
    public void Constructor_Throws_WhenPropertyDetailServiceNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new BookingDetailsViewModel(_bookingServiceMock.Object, null!));
    }

    [Fact]
    public void DefaultValues_AreSetCorrectly()
    {
        Assert.Null(_viewModel.ReservationId);
        Assert.Equal(string.Empty, _viewModel.ImageUrl);
        Assert.NotNull(_viewModel.Booking);
        Assert.NotNull(_viewModel.Property);
        Assert.NotNull(_viewModel.Room);
        Assert.Equal(string.Empty, _viewModel.PaymentInfo);
        Assert.Equal("COP - Peso Colombiano", _viewModel.Currency);
        Assert.NotNull(_viewModel.DownloadVoucherCommand);
        Assert.NotNull(_viewModel.DownloadConfirmationCommand);
        Assert.NotNull(_viewModel.DownloadInvoiceCommand);
        Assert.NotNull(_viewModel.CallHotelCommand);
        Assert.NotNull(_viewModel.EmailHotelCommand);
        Assert.NotNull(_viewModel.ModifyBookingCommand);
        Assert.NotNull(_viewModel.CancelBookingCommand);
    }

    [Fact]
    public void ApplyQueryAttributes_SetsReservationId()
    {
        var query = new Dictionary<string, object> { { "ReservationId", "rsv-123" } };

        _viewModel.ApplyQueryAttributes(query);

        Assert.Equal("rsv-123", _viewModel.ReservationId);
    }

    [Fact]
    public void ApplyQueryAttributes_DoesNotSetReservationId_WhenKeyMissing()
    {
        var query = new Dictionary<string, object>();

        _viewModel.ApplyQueryAttributes(query);

        Assert.Null(_viewModel.ReservationId);
    }

    [Fact]
    public void ApplyQueryAttributes_DoesNotSetReservationId_WhenValueIsNotString()
    {
        var query = new Dictionary<string, object> { { "ReservationId", 123 } };

        _viewModel.ApplyQueryAttributes(query);

        Assert.Null(_viewModel.ReservationId);
    }

    [Fact]
    public void Nights_ComputesCorrectly()
    {
        _viewModel.CheckIn = new DateTime(2026, 5, 10);
        _viewModel.CheckOut = new DateTime(2026, 5, 14);

        Assert.Equal(4, _viewModel.Nights);
    }

    [Fact]
    public void Nights_ReturnsZero_WhenCheckOutEqualsCheckIn()
    {
        _viewModel.CheckIn = new DateTime(2026, 5, 10);
        _viewModel.CheckOut = new DateTime(2026, 5, 10);

        Assert.Equal(0, _viewModel.Nights);
    }

    [Fact]
    public void SubTotal_ComputesCorrectly()
    {
        _viewModel.CheckIn = new DateTime(2026, 5, 10);
        _viewModel.CheckOut = new DateTime(2026, 5, 13);
        _viewModel.Room = new AccommodationDetailRoomDto { Price = 119m };

        Assert.Equal(300m, _viewModel.SubTotal);
    }

    [Fact]
    public void SubTotal_ThrowsNullReference_WhenRoomIsNull()
    {
        _viewModel.Room = null;

        Assert.Throws<NullReferenceException>(() => _ = _viewModel.SubTotal);
    }

    [Fact]
    public void Taxes_ComputesCorrectly()
    {
        _viewModel.CheckIn = new DateTime(2026, 5, 10);
        _viewModel.CheckOut = new DateTime(2026, 5, 13);
        _viewModel.Room = new AccommodationDetailRoomDto { Price = 119m };

        Assert.Equal(57m, _viewModel.Taxes);
    }

    [Fact]
    public void TotalPrice_ComputesCorrectly()
    {
        _viewModel.CheckIn = new DateTime(2026, 5, 10);
        _viewModel.CheckOut = new DateTime(2026, 5, 13);
        _viewModel.Room = new AccommodationDetailRoomDto { Price = 119m };

        Assert.Equal(357m, _viewModel.TotalPrice);
    }

    [Fact]
    public void CheckInSetter_TriggersPropertyChanged_ForNightsSubTotalTaxesTotalPrice()
    {
        var propertiesChanged = new List<string>();
        _viewModel.PropertyChanged += (_, e) => propertiesChanged.Add(e.PropertyName);

        _viewModel.CheckIn = new DateTime(2026, 6, 1);

        Assert.Contains(nameof(BookingDetailsViewModel.Nights), propertiesChanged);
        Assert.Contains(nameof(BookingDetailsViewModel.SubTotal), propertiesChanged);
        Assert.Contains(nameof(BookingDetailsViewModel.Taxes), propertiesChanged);
        Assert.Contains(nameof(BookingDetailsViewModel.TotalPrice), propertiesChanged);
    }

    [Fact]
    public void CheckOutSetter_TriggersPropertyChanged_ForNightsSubTotalTaxesTotalPrice()
    {
        var propertiesChanged = new List<string>();
        _viewModel.PropertyChanged += (_, e) => propertiesChanged.Add(e.PropertyName);

        _viewModel.CheckOut = new DateTime(2026, 6, 5);

        Assert.Contains(nameof(BookingDetailsViewModel.Nights), propertiesChanged);
        Assert.Contains(nameof(BookingDetailsViewModel.SubTotal), propertiesChanged);
        Assert.Contains(nameof(BookingDetailsViewModel.Taxes), propertiesChanged);
        Assert.Contains(nameof(BookingDetailsViewModel.TotalPrice), propertiesChanged);
    }

    [Fact]
    public async Task LoadReservationDetails_LoadsBookingAndProperty()
    {
        var bookingId = "rsv-123";
        var bookingResponse = new BookingResponseDto
        {
            Id = bookingId,
            PublicId = "RSV-12345",
            HabitacionId = "room-1",
            CheckIn = "2026-05-10",
            CheckOut = "2026-05-13",
            Estado = "confirmada"
        };

        _bookingServiceMock
            .Setup(x => x.GetBookingByReservationIdAsync(bookingId))
            .ReturnsAsync(new HttpResponseWrapper<BookingResponseDto>(bookingResponse, false, new HttpResponseMessage(HttpStatusCode.OK)));

        var propertyRoom = new AccommodationDetailRoomDto
        {
            Id = "room-1",
            Description = "Suite",
            Price = 200m,
            Capacity = 2
        };

        var propertyResponse = new AccommodationInfoDto
        {
            Name = "Hotel Test",
            Address = "Calle 123",
            City = "Bogotá",
            Room = propertyRoom,
            Images = new List<AccommodationDetailImageDto>
            {
                new AccommodationDetailImageDto { Id = "img-1", Url = "https://image.test/hotel.jpg" }
            }
        };

        _propertyDetailServiceMock
            .Setup(x => x.GetPropertyDetailByRoomIdAsync("room-1", "COP"))
            .ReturnsAsync(new HttpResponseWrapper<AccommodationInfoDto>(propertyResponse, false, new HttpResponseMessage(HttpStatusCode.OK)));

        _bookingServiceMock
            .Setup(x => x.GetPaymentsByReservationAsync(bookingId))
            .ReturnsAsync(new HttpResponseWrapper<List<PaymentReservationDTO>>(new List<PaymentReservationDTO>
            {
                new PaymentReservationDTO
                {
                    Id = "pay-1",
                    Amount = 600m,
                    Currency = "COP",
                    Status = "authorized"
                }
            }, false, new HttpResponseMessage(HttpStatusCode.OK)));

        _viewModel.ApplyQueryAttributes(new Dictionary<string, object> { { "ReservationId", bookingId } });

        await Task.Delay(100);

        Assert.Equal(bookingId, _viewModel.ReservationId);
        Assert.Equal("Hotel Test", _viewModel.Property.Name);
        Assert.Equal("https://image.test/hotel.jpg", _viewModel.ImageUrl);
        Assert.Equal("Suite", _viewModel.Room.Description);
    }

    [Fact]
    public async Task LoadPaymentInfoAsync_ShowsPagoPendiente_WhenEstadoIsNotConfirmada()
    {
        var bookingId = "rsv-123";
        var bookingResponse = new BookingResponseDto
        {
            Id = bookingId,
            PublicId = "RSV-12345",
            HabitacionId = "room-1",
            CheckIn = "2026-05-10",
            CheckOut = "2026-05-13",
            Estado = "pendiente"
        };

        _bookingServiceMock
            .Setup(x => x.GetBookingByReservationIdAsync(bookingId))
            .ReturnsAsync(new HttpResponseWrapper<BookingResponseDto>(bookingResponse, false, new HttpResponseMessage(HttpStatusCode.OK)));

        var propertyRoom = new AccommodationDetailRoomDto { Id = "room-1", Description = "Suite", Price = 200m };
        var propertyResponse = new AccommodationInfoDto
        {
            Name = "Hotel Test",
            Room = propertyRoom,
            Images = new List<AccommodationDetailImageDto>
            {
                new AccommodationDetailImageDto { Id = "img-1", Url = "https://image.test/hotel.jpg" }
            }
        };

        _propertyDetailServiceMock
            .Setup(x => x.GetPropertyDetailByRoomIdAsync("room-1", "COP"))
            .ReturnsAsync(new HttpResponseWrapper<AccommodationInfoDto>(propertyResponse, false, new HttpResponseMessage(HttpStatusCode.OK)));

        _viewModel.ApplyQueryAttributes(new Dictionary<string, object> { { "ReservationId", bookingId } });

        await Task.Delay(100);

        Assert.Equal("Pago pendiente", _viewModel.PaymentInfo);
    }

    [Fact]
    public async Task LoadPaymentInfoAsync_ShowsSinInformacion_WhenPaymentApiFails()
    {
        var bookingId = "rsv-123";
        var bookingResponse = new BookingResponseDto
        {
            Id = bookingId,
            PublicId = "RSV-12345",
            HabitacionId = "room-1",
            Estado = "confirmada"
        };

        _bookingServiceMock
            .Setup(x => x.GetBookingByReservationIdAsync(bookingId))
            .ReturnsAsync(new HttpResponseWrapper<BookingResponseDto>(bookingResponse, false, new HttpResponseMessage(HttpStatusCode.OK)));

        var propertyResponse = new AccommodationInfoDto
        {
            Name = "Hotel Test",
            Room = new AccommodationDetailRoomDto { Id = "room-1", Price = 200m },
            Images = new List<AccommodationDetailImageDto>()
        };

        _propertyDetailServiceMock
            .Setup(x => x.GetPropertyDetailByRoomIdAsync("room-1", "COP"))
            .ReturnsAsync(new HttpResponseWrapper<AccommodationInfoDto>(propertyResponse, false, new HttpResponseMessage(HttpStatusCode.OK)));

        _bookingServiceMock
            .Setup(x => x.GetPaymentsByReservationAsync(bookingId))
            .ReturnsAsync(new HttpResponseWrapper<List<PaymentReservationDTO>>(null!, true, new HttpResponseMessage(HttpStatusCode.InternalServerError)));

        _viewModel.ApplyQueryAttributes(new Dictionary<string, object> { { "ReservationId", bookingId } });

        await Task.Delay(100);

        Assert.Equal("Sin informaci�n de pago", _viewModel.PaymentInfo);
    }

    [Fact]
    public async Task LoadPaymentInfoAsync_ShowsSinInformacion_WhenNoPaymentsReturned()
    {
        var bookingId = "rsv-123";
        var bookingResponse = new BookingResponseDto
        {
            Id = bookingId,
            PublicId = "RSV-12345",
            HabitacionId = "room-1",
            Estado = "confirmada"
        };

        _bookingServiceMock
            .Setup(x => x.GetBookingByReservationIdAsync(bookingId))
            .ReturnsAsync(new HttpResponseWrapper<BookingResponseDto>(bookingResponse, false, new HttpResponseMessage(HttpStatusCode.OK)));

        var propertyResponse = new AccommodationInfoDto
        {
            Name = "Hotel Test",
            Room = new AccommodationDetailRoomDto { Id = "room-1", Price = 200m },
            Images = new List<AccommodationDetailImageDto>()
        };

        _propertyDetailServiceMock
            .Setup(x => x.GetPropertyDetailByRoomIdAsync("room-1", "COP"))
            .ReturnsAsync(new HttpResponseWrapper<AccommodationInfoDto>(propertyResponse, false, new HttpResponseMessage(HttpStatusCode.OK)));

        _bookingServiceMock
            .Setup(x => x.GetPaymentsByReservationAsync(bookingId))
            .ReturnsAsync(new HttpResponseWrapper<List<PaymentReservationDTO>>(new List<PaymentReservationDTO>(), false, new HttpResponseMessage(HttpStatusCode.OK)));

        _viewModel.ApplyQueryAttributes(new Dictionary<string, object> { { "ReservationId", bookingId } });

        await Task.Delay(100);

        Assert.Equal("Sin informaci�n de pago", _viewModel.PaymentInfo);
    }

    [Fact]
    public async Task LoadPaymentInfoAsync_UpdatesPaymentInfo_WhenConfirmedAndHasPayments()
    {
        var bookingId = "rsv-123";
        var bookingResponse = new BookingResponseDto
        {
            Id = bookingId,
            PublicId = "RSV-12345",
            HabitacionId = "room-1",
            Estado = "confirmada"
        };

        _bookingServiceMock
            .Setup(x => x.GetBookingByReservationIdAsync(bookingId))
            .ReturnsAsync(new HttpResponseWrapper<BookingResponseDto>(bookingResponse, false, new HttpResponseMessage(HttpStatusCode.OK)));

        var propertyResponse = new AccommodationInfoDto
        {
            Name = "Hotel Test",
            Room = new AccommodationDetailRoomDto { Id = "room-1", Price = 200m },
            Images = new List<AccommodationDetailImageDto>()
        };

        _propertyDetailServiceMock
            .Setup(x => x.GetPropertyDetailByRoomIdAsync("room-1", "COP"))
            .ReturnsAsync(new HttpResponseWrapper<AccommodationInfoDto>(propertyResponse, false, new HttpResponseMessage(HttpStatusCode.OK)));

        var payments = new List<PaymentReservationDTO>
        {
            new PaymentReservationDTO
            {
                Id = "pay-1",
                ReservaId = bookingId,
                Amount = 1035639.0m,
                Currency = "ARS",
                Status = "authorized",
                Description = "Pago por reserva"
            }
        };

        _bookingServiceMock
            .Setup(x => x.GetPaymentsByReservationAsync(bookingId))
            .ReturnsAsync(new HttpResponseWrapper<List<PaymentReservationDTO>>(payments, false, new HttpResponseMessage(HttpStatusCode.OK)));

        _viewModel.ApplyQueryAttributes(new Dictionary<string, object> { { "ReservationId", bookingId } });

        await Task.Delay(100);

        Assert.Contains("Autorizado", _viewModel.PaymentInfo);
        Assert.Contains("ARS", _viewModel.PaymentInfo);
        Assert.Contains("1035639", _viewModel.PaymentInfo.Replace(",", "").Replace(".", ""));
        Assert.Equal("ARS", _viewModel.Currency);
    }

    [Fact]
    public async Task LoadPaymentInfoAsync_HandlesAllPaymentStatusTranslations()
    {
        var statusMappings = new Dictionary<string, string>
        {
            ["authorized"] = "Autorizado",
            ["completed"] = "Completado",
            ["pending"] = "Pendiente",
            ["failed"] = "Fallido",
            ["refunded"] = "Reembolsado",
            ["unknown_status"] = "unknown_status"
        };

        foreach (var (status, expectedDisplay) in statusMappings)
        {
            var bookingId = $"rsv-{status}";
            var bookingResponse = new BookingResponseDto
            {
                Id = bookingId,
                HabitacionId = "room-1",
                Estado = "confirmada"
            };

            _bookingServiceMock
                .Setup(x => x.GetBookingByReservationIdAsync(bookingId))
                .ReturnsAsync(new HttpResponseWrapper<BookingResponseDto>(bookingResponse, false, new HttpResponseMessage(HttpStatusCode.OK)));

            var propertyResponse = new AccommodationInfoDto
            {
                Name = "Hotel Test",
                Room = new AccommodationDetailRoomDto { Id = "room-1", Price = 200m },
                Images = new List<AccommodationDetailImageDto>()
            };

            _propertyDetailServiceMock
                .Setup(x => x.GetPropertyDetailByRoomIdAsync("room-1", "COP"))
                .ReturnsAsync(new HttpResponseWrapper<AccommodationInfoDto>(propertyResponse, false, new HttpResponseMessage(HttpStatusCode.OK)));

            var payments = new List<PaymentReservationDTO>
            {
                new PaymentReservationDTO
                {
                    Id = $"pay-{status}",
                    Amount = 100m,
                    Currency = "USD",
                    Status = status
                }
            };

            _bookingServiceMock
                .Setup(x => x.GetPaymentsByReservationAsync(bookingId))
                .ReturnsAsync(new HttpResponseWrapper<List<PaymentReservationDTO>>(payments, false, new HttpResponseMessage(HttpStatusCode.OK)));

            _viewModel.ApplyQueryAttributes(new Dictionary<string, object> { { "ReservationId", bookingId } });

            await Task.Delay(50);

            Assert.StartsWith(expectedDisplay, _viewModel.PaymentInfo);
        }
    }

    [Fact]
    public async Task LoadReservationDetails_ReturnsEarly_WhenBookingResultIsNull()
    {
        _bookingServiceMock
            .Setup(x => x.GetBookingByReservationIdAsync(It.IsAny<string>()))
            .ReturnsAsync((HttpResponseWrapper<BookingResponseDto>)null!);

        _viewModel.ApplyQueryAttributes(new Dictionary<string, object> { { "ReservationId", "rsv-123" } });

        await Task.Delay(100);

        Assert.Equal(string.Empty, _viewModel.Property.Name);
    }

    [Fact]
    public async Task LoadReservationDetails_ReturnsEarly_WhenBookingResponseIsNull()
    {
        _bookingServiceMock
            .Setup(x => x.GetBookingByReservationIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new HttpResponseWrapper<BookingResponseDto>(null!, false, new HttpResponseMessage(HttpStatusCode.OK)));

        _viewModel.ApplyQueryAttributes(new Dictionary<string, object> { { "ReservationId", "rsv-123" } });

        await Task.Delay(100);

        Assert.Equal(string.Empty, _viewModel.Property.Name);
    }

    [Fact]
    public async Task LoadReservationDetails_ReturnsEarly_WhenPropertyResultIsNull()
    {
        var bookingResponse = new BookingResponseDto
        {
            Id = "rsv-123",
            HabitacionId = "room-1",
            Estado = "pendiente"
        };

        _bookingServiceMock
            .Setup(x => x.GetBookingByReservationIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new HttpResponseWrapper<BookingResponseDto>(bookingResponse, false, new HttpResponseMessage(HttpStatusCode.OK)));

        _propertyDetailServiceMock
            .Setup(x => x.GetPropertyDetailByRoomIdAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync((HttpResponseWrapper<AccommodationInfoDto>)null!);

        _viewModel.ApplyQueryAttributes(new Dictionary<string, object> { { "ReservationId", "rsv-123" } });

        await Task.Delay(100);

        Assert.Equal(string.Empty, _viewModel.Property.Name);
    }

    [Fact]
    public void GuestsCount_CanBeSetAndRead()
    {
        _viewModel.GuestsCount = 4;
        Assert.Equal(4, _viewModel.GuestsCount);

        _viewModel.GuestsCount = 0;
        Assert.Equal(0, _viewModel.GuestsCount);
    }

    [Fact]
    public void ImageUrl_CanBeSetAndRead()
    {
        var url = "https://image.test/hotel.jpg";

        _viewModel.ImageUrl = url;

        Assert.Equal(url, _viewModel.ImageUrl);
    }
}
