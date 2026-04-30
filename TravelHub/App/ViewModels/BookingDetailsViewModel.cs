using App.DTOs;
using App.Models;
using App.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace App.ViewModels;

[QueryProperty(nameof(ReservationId), "ReservationId")]
public partial class BookingDetailsViewModel : BaseViewModel, IQueryAttributable
{
    private int _reservationId;
    public int ReservationId
    {
        get => _reservationId;
        set => SetProperty(ref _reservationId, value);
    }

    // CA2: Información General
    
    private string _bookingNumber = string.Empty;
    public string BookingNumber
    {
        get => _bookingNumber;
        set => SetProperty(ref _bookingNumber, value);
    }

    private string _bookingStatus = string.Empty;
    public string BookingStatus
        {
        get => _bookingStatus;
        set => SetProperty(ref _bookingStatus, value);
    }


    private Color _statusColor = Colors.Transparent;

    public Color StatusColor
    {
        get => _statusColor;
        set => SetProperty(ref _statusColor, value);
    }

    private DateTime _checkIn;
    public DateTime CheckIn
    {
        get => _checkIn;
        set => SetProperty(ref _checkIn, value);
    }


    private DateTime _checkOut;
    public DateTime CheckOut
    {
        get => _checkOut;
        set => SetProperty(ref _checkOut, value);
    }


    private int _guestsCount;
    public int GuestsCount
    {
        get => _guestsCount;
        set => SetProperty(ref _guestsCount, value);
    }


    private string _roomType = string.Empty;
    public string RoomType
    {
        get => _roomType;
        set => SetProperty(ref _roomType, value);
    }


    private int _nights;
    public int Nights
    {
        get => _nights;
        set => SetProperty(ref _nights, value);
    }

    // CA3: Información del Hotel

    private HotelInfo _hotel = new();
    public HotelInfo Hotel
    {
        get => _hotel;
        set => SetProperty(ref _hotel, value);
    }

    // CA4: Servicios Incluidos

    private ObservableCollection<AccommodationDetailAmenityDto> _includedServices = new();
    public ObservableCollection<AccommodationDetailAmenityDto> IncludedServices
    {
        get => _includedServices;
        set => SetProperty(ref _includedServices, value);
    }

    // CA5: Información Económica

    private decimal _pricePerNight;
    public decimal PricePerNight
    {
        get => _pricePerNight;
        set => SetProperty(ref _pricePerNight, value);
    }


    private decimal _subTotal;
    public decimal SubTotal
    {
        get => _subTotal;
        set => SetProperty(ref _subTotal, value);
    }


    private decimal _taxes;
    public decimal Taxes
    {
        get => _taxes;
        set => SetProperty(ref _taxes, value);
    }


    private decimal _totalPrice;
    public decimal TotalPrice
    {
        get => _totalPrice;
        set => SetProperty(ref _totalPrice, value);
    }


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

    // CA6: Documentos

    private bool _hasInvoice = true;
    public bool HasInvoice
    {
        get => _hasInvoice;
        set => SetProperty(ref _hasInvoice, value);
    }

    // CA7: Gestión de Reserva

    private bool _canModify = true;
    public bool CanModify
    {
        get => _canModify;
        set => SetProperty(ref _canModify, value);
    }


    private bool _canCancel = true;
    public bool CanCancel
    {
        get => _canCancel;
        set => SetProperty(ref _canCancel, value);
    }

    public ICommand DownloadVoucherCommand { get; }
    public ICommand DownloadConfirmationCommand { get; }
    public ICommand DownloadInvoiceCommand { get; }
    public ICommand CallHotelCommand { get; }
    public ICommand EmailHotelCommand { get; }
    public ICommand ModifyBookingCommand { get; }
    public ICommand CancelBookingCommand { get; }

    public BookingDetailsViewModel()
    {
        Title = "Detalle de Reserva";
        DownloadVoucherCommand = new Command(async () => await DownloadVoucher());
        DownloadConfirmationCommand = new Command(async () => await DownloadConfirmation());
        DownloadInvoiceCommand = new Command(async () => await DownloadInvoice());
        CallHotelCommand = new Command(async () => await CallHotel());
        EmailHotelCommand = new Command(async () => await EmailHotel());
        ModifyBookingCommand = new Command(async () => await ModifyBooking());
        CancelBookingCommand = new Command(async () => await CancelBooking());
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("ReservationId", out var obj) && obj is int value)
            ReservationId = value;
        LoadReservationDetails(ReservationId);
    }

    private void LoadReservationDetails(int reservationId)
    {
        //// Cargar datos de la reserva desde el servicio
        //var reservation = MockDataService.GetActiveReservations()
        //    .FirstOrDefault(r => r.Id == reservationId);

        //if (reservation == null) return;

        //// Mapear datos de la reserva al ViewModel
        //BookingNumber = reservation.BookingCode;
        //BookingStatus = reservation.Status;
        //StatusColor = reservation.Status.ToLower() switch
        //{
        //    "confirmada" => Color.FromArgb("#4CAF50"),
        //    "pendiente" => Color.FromArgb("#FFC107"),
        //    "cancelada" => Color.FromArgb("#F44336"),
        //    _ => Color.FromArgb("#757575")
        //};

        //CheckIn = reservation.CheckIn;
        //CheckOut = reservation.CheckOut;
        //GuestsCount = reservation.Adults + reservation.Children;
        //RoomType = reservation.Room.Name;
        //Nights = reservation.Nights;

        //// Información del hotel
        //Hotel = new HotelInfo
        //{
        //    Name = reservation.Property.Name,
        //    Address = reservation.Property.Address,
        //    City = reservation.Property.City,
        //    Phone = "+57 300 123 4567", // Mock - debería venir del modelo
        //    Email = "reservas@" + reservation.Property.Name.ToLower().Replace(" ", "") + ".com",
        //    FullAddress = $"{reservation.Property.Address}, {reservation.Property.City}",
        //    ImageUrl = reservation.Property.ImageUrl
        //};

        //// Servicios incluidos
        //IncludedServices = new ObservableCollection<Amenity>(reservation.Property.Amenities);

        //// Información económica
        //PricePerNight = reservation.Room.PricePerNight;
        //SubTotal = reservation.Room.PricePerNight * reservation.Nights;
        //Taxes = SubTotal * 0.19m; // IVA 19%
        //TotalPrice = reservation.TotalPrice;
        //PaymentInfo = "Pagado - Tarjeta •••• 4567";

        //// Gestión de reserva
        //CanModify = reservation.Status.ToLower() == "confirmada" && 
        //            reservation.CheckIn > DateTime.Now.AddDays(2);
        //CanCancel = reservation.Status.ToLower() == "confirmada" && 
        //            reservation.CheckIn > DateTime.Now.AddDays(1);
    }

    // CA6: Descarga de Documentos

    private async Task DownloadVoucher()
    {
        await Shell.Current.DisplayAlert("Descarga", "Descargando voucher...", "OK");
        // Implementar lógica de descarga
    }
    private async Task DownloadConfirmation()
    {
        await Shell.Current.DisplayAlert("Descarga", "Descargando confirmación...", "OK");
        // Implementar lógica de descarga
    }
    private async Task DownloadInvoice()
    {
        await Shell.Current.DisplayAlert("Descarga", "Descargando factura...", "OK");
        // Implementar lógica de descarga
    }

    // CA8: Contacto

    private async Task CallHotel()
    {
        try
        {
            if (PhoneDialer.Default.IsSupported)
                PhoneDialer.Default.Open(Hotel.Phone);
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"No se pudo realizar la llamada: {ex.Message}", "OK");
        }
    }


    private async Task EmailHotel()
    {
        try
        {
            var message = new EmailMessage
            {
                Subject = $"Consulta - Reserva {BookingNumber}",
                To = new List<string> { Hotel.Email },
                Body = $"Hola, tengo una consulta sobre mi reserva {BookingNumber}..."
            };
            await Email.Default.ComposeAsync(message);
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"No se pudo abrir el email: {ex.Message}", "OK");
        }
    }

    // CA7: Gestión

    private async Task ModifyBooking()
    {
        await Shell.Current.DisplayAlert("Modificar", "Funcionalidad de modificación en desarrollo", "OK");
        // Navegar a página de modificación
    }


    private async Task CancelBooking()
    {
        bool confirm = await Shell.Current.DisplayAlert(
            "Cancelar Reserva",
            "¿Está seguro que desea cancelar esta reserva? Esta acción puede tener penalizaciones según las políticas del hotel.",
            "Sí, cancelar",
            "No");

        if (confirm)
        {
            // Implementar lógica de cancelación
            await Shell.Current.DisplayAlert("Cancelación", "Procesando cancelación...", "OK");
            await Shell.Current.GoToAsync("..");
        }
    }
}


// Clases auxiliares
public class HotelInfo
{
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FullAddress { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
}

