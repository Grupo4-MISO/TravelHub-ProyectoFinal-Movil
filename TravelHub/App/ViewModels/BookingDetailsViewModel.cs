using App.DTOs;
using App.Models;
using App.Services;
using App.Services.Implementations;
using App.Services.Interfaces;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace App.ViewModels;

[QueryProperty(nameof(ReservationId), "ReservationId")]
public partial class BookingDetailsViewModel : BaseViewModel, IQueryAttributable
{
    private readonly IBookingService _bookingService;
    private readonly IPropertyDetailService _propertyDetailService;
    private readonly IAppSettingsService _appSettingsService;

    private string _reservationId = string.Empty;
    public string ReservationId
    {
        get => _reservationId;
        set => SetProperty(ref _reservationId, value);
    }

    private string _imageUrl = string.Empty;
    public string ImageUrl
    {
        get => _imageUrl;
        set => SetProperty(ref _imageUrl, value);
    }

    private BookingResponseDto _booking = new();
    public BookingResponseDto Booking
    {
        get => _booking;
        set
        {
            if (SetProperty(ref _booking, value))
            {
                OnPropertyChanged(nameof(CanCancel));
                OnPropertyChanged(nameof(CanPay));
            }
        }
    }

    private AccommodationInfoDto _property = new();
    public AccommodationInfoDto Property
    {
        get => _property;
        set => SetProperty(ref _property, value);
    }

    private AccommodationDetailRoomDto? _room = new();
    public AccommodationDetailRoomDto? Room
    {
        get => _room;
        set => SetProperty(ref _room, value);
    }

    private DateTime _checkIn = DateTime.Today.AddDays(7);
    public DateTime CheckIn
    {
        get => _checkIn;
        set
        {
            if (SetProperty(ref _checkIn, value))
            {
                OnPropertyChanged(nameof(Nights));
                OnPropertyChanged(nameof(SubTotal));
                OnPropertyChanged(nameof(Taxes));
                OnPropertyChanged(nameof(TotalPrice));
            }
        }
    }

    private DateTime _checkOut = DateTime.Today.AddDays(9);
    public DateTime CheckOut
    {
        get => _checkOut;
        set
        {
            if (SetProperty(ref _checkOut, value))
            {
                OnPropertyChanged(nameof(Nights));
                OnPropertyChanged(nameof(SubTotal));
                OnPropertyChanged(nameof(Taxes));
                OnPropertyChanged(nameof(TotalPrice));
            }
        }
    }

    private int _guestsCount = 0;
    public int GuestsCount
    {
        get => _guestsCount;
        set => SetProperty(ref _guestsCount, value);
    }

    public int Nights => (CheckOut - CheckIn).Days;
    public decimal SubTotal => Room != null ? (Room.Price * Nights) / 1.19m : 0;
    public decimal Taxes => SubTotal * 0.19m;

    public decimal TotalPrice => Room?.Price * Nights ?? 0;


    private string _currency = "COP - Peso Colombiano";
    public string Currency
    {
        get => _currency;
        set => SetProperty(ref _currency, value);
    }


    private string _paymentInfo = string.Empty;
    public string PaymentInfo
    {
        get => _paymentInfo;
        set => SetProperty(ref _paymentInfo, value);
    }

    public bool CanCancel => Booking != null &&
        (string.Equals(Booking.Estado, "pendiente", StringComparison.OrdinalIgnoreCase) ||
         string.Equals(Booking.Estado, "confirmada", StringComparison.OrdinalIgnoreCase));

    public bool CanPay => Booking != null &&
        string.Equals(Booking.Estado, "pendiente", StringComparison.OrdinalIgnoreCase);

    public ICommand DownloadVoucherCommand { get; }
    public ICommand DownloadConfirmationCommand { get; }
    public ICommand DownloadInvoiceCommand { get; }
    public ICommand CallHotelCommand { get; }
    public ICommand EmailHotelCommand { get; }
    public ICommand ModifyBookingCommand { get; }
    public ICommand CancelBookingCommand { get; }
    public ICommand PayCommand { get; }

    public BookingDetailsViewModel(IBookingService bookingService, IPropertyDetailService propertyDetailService, IAppSettingsService appSettingsService)
    {
        Title = "Detalle de Reserva";
        _bookingService = bookingService ?? throw new ArgumentNullException(nameof(bookingService));
        _propertyDetailService = propertyDetailService ?? throw new ArgumentNullException(nameof(propertyDetailService));
        _appSettingsService = appSettingsService ?? throw new ArgumentNullException(nameof(appSettingsService));
        DownloadVoucherCommand = new Command(async () => await DownloadVoucher());
        DownloadConfirmationCommand = new Command(async () => await DownloadConfirmation());
        DownloadInvoiceCommand = new Command(async () => await DownloadInvoice());
        CallHotelCommand = new Command(async () => await CallHotel());
        EmailHotelCommand = new Command(async () => await EmailHotel());
        ModifyBookingCommand = new Command(async () => await ModifyBooking());
        CancelBookingCommand = new Command(async () => await CancelBooking());
        PayCommand = new Command(OnPay);
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("ReservationId", out var obj) && obj is string value)
            ReservationId = value;
        _ = LoadReservationDetails(ReservationId);
    }

    private async Task LoadReservationDetails(string reservationId)
    {
        // Cargar datos de la reserva desde el servicio
        var result = await _bookingService.GetBookingByReservationIdAsync(reservationId);

        if (result == null || result.Response == null) return;
        Booking = result.Response;
        // Cargar datos de la propiedad desde el servicio
        var currencyCode = _appSettingsService.CurrentCurrencyCode;
        var hospedajeResult = await _propertyDetailService.GetPropertyDetailByRoomIdAsync(Booking.HabitacionId, currencyCode);
        if (hospedajeResult == null || hospedajeResult.Response == null) return;

        Property = hospedajeResult.Response;
        ImageUrl = Property.Images != null && Property.Images.Count > 0 ? Property.Images[0].Url : string.Empty;
        Room = Property.Room;

        CheckIn = DateTime.TryParse(Booking.CheckIn, out var checkIn) ? checkIn : DateTime.MinValue;
        CheckOut = DateTime.TryParse(Booking.CheckOut, out var checkOut) ? checkOut : DateTime.MinValue;
        

        // Informaci�n econ�mica

        await LoadPaymentInfoAsync(reservationId);

    }

    private async Task LoadPaymentInfoAsync(string reservationId)
    {
        if (!string.Equals(Booking.Estado, "confirmada", StringComparison.OrdinalIgnoreCase))
        {
            PaymentInfo = "Pago pendiente";
            return;
        }

        var paymentResult = await _bookingService.GetPaymentsByReservationAsync(reservationId);

        if (paymentResult.Error || paymentResult.Response == null || paymentResult.Response.Count == 0)
        {
            PaymentInfo = "Sin información de pago";
            return;
        }

        var payment = paymentResult.Response[0];
        var statusDisplay = payment.Status switch
        {
            "authorized" => "Autorizado",
            "completed" => "Completado",
            "pending" => "Pendiente",
            "failed" => "Fallido",
            "refunded" => "Reembolsado",
            _ => payment.Status
        };

        PaymentInfo = $"{statusDisplay} - {payment.Amount:N2} {payment.Currency}";
        Currency = $"{_appSettingsService.CurrentCurrencyCode}";

        if (Room != null)
        {
            OnPropertyChanged(nameof(SubTotal));
            OnPropertyChanged(nameof(Taxes));
            OnPropertyChanged(nameof(TotalPrice));
        }
    }

    // CA6: Descarga de Documentos

    private async Task DownloadVoucher()
    {
        await Shell.Current.DisplayAlertAsync("Descarga", "Descargando voucher...", "OK");
        // Implementar l�gica de descarga
    }
    private async Task DownloadConfirmation()
    {
        await Shell.Current.DisplayAlertAsync("Descarga", "Descargando confirmación...", "OK");
        // Implementar l�gica de descarga
    }
    private async Task DownloadInvoice()
    {
        await Shell.Current.DisplayAlertAsync("Descarga", "Descargando factura...", "OK");
        // Implementar l�gica de descarga
    }

    // CA8: Contacto

    private async Task CallHotel()
    {
        try
        {
            //if (PhoneDialer.Default.IsSupported)
            //    PhoneDialer.Default.Open(Hotel.Phone);
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync("Error", $"No se pudo realizar la llamada: {ex.Message}", "OK");
        }
    }


    private async Task EmailHotel()
    {
        try
        {
            //var message = new EmailMessage
            //{
            //    Subject = $"Consulta - Reserva {BookingNumber}",
            //    To = new List<string> { Hotel.Email },
            //    Body = $"Hola, tengo una consulta sobre mi reserva {BookingNumber}..."
            //};
            //await Email.Default.ComposeAsync(message);
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync("Error", $"No se pudo abrir el email: {ex.Message}", "OK");
        }
    }

    // CA7: Gesti�n

    private async Task ModifyBooking()
    {
        await Shell.Current.DisplayAlertAsync("Modificar", "Funcionalidad de modificación en desarrollo", "OK");
        // Navegar a página de modificación
    }


    private async Task CancelBooking()
    {
        bool confirm = await Shell.Current.DisplayAlertAsync(
            "Cancelar Reserva",
            "¿Está seguro que desea cancelar esta reserva? Esta acción puede tener penalizaciones según las políticas del hotel.",
            "Sí, cancelar",
            "No");

        if (!confirm) return;

        try
        {
            IsBusy = true;

            var result = await _bookingService.RevokeBookingByReservationIdAsync(ReservationId);

            if (!result.Error)
            {
                await Shell.Current.DisplayAlertAsync("Reserva Cancelada",
                    "La reserva ha sido cancelada exitosamente.", "OK");
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                var errorMsg = await result.GetErrorMessageAsync();
                await Shell.Current.DisplayAlertAsync("Error",
                    $"No se pudo cancelar la reserva: {errorMsg}", "OK");
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync("Error",
                $"Ocurrió un error inesperado: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async void OnPay()
    {
        var navParams = new Dictionary<string, object>
        {
            { "amount", TotalPrice },
            { "referenceId", Booking.Id },
            { "description", $"Pago reserva {Booking.PublicId}" },
            { "currency", _appSettingsService.CurrentCurrencyCode },
            { "returnRoute", "//bookings" }
        };
        await Shell.Current.GoToAsync("PaymentPage", navParams);
    }
}


//// Clases auxiliares
//public class HotelInfo
//{
//    public string Name { get; set; } = string.Empty;
//    public string Address { get; set; } = string.Empty;
//    public string City { get; set; } = string.Empty;
//    public string Phone { get; set; } = string.Empty;
//    public string Email { get; set; } = string.Empty;
//    public string FullAddress { get; set; } = string.Empty;
//    public string ImageUrl { get; set; } = string.Empty;
//}

