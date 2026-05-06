using App.DTOs;
using App.Models;
using App.Services.Interfaces;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using System.Windows.Input;

namespace App.ViewModels;

public class BookingSummaryViewModel : BaseViewModel, IQueryAttributable
{
    private readonly IBookingService _bookingService;
    private readonly IUserSessionService _userSessionService;
    private bool _holdCreationAttempted;
    private bool _isCreatingHold;

    private string _imageUrl = string.Empty;
    private string ImageUrl
    {
        get => _imageUrl;
        set => SetProperty(ref _imageUrl, value);
    }

    private AccommodationDetailDto _property = new();
    public AccommodationDetailDto Property
    {
        get => _property;
        set => SetProperty(ref _property, value);
    }

    private AccommodationDetailRoomDto _room = new();
    public AccommodationDetailRoomDto Room
    {
        get => _room;
        set => SetProperty(ref _room, value);
    }

    private Traveler _traveler = new();
    public Traveler Traveler
    {
        get => _traveler;
        set => SetProperty(ref _traveler, value);
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
                OnPropertyChanged(nameof(TotalPrice));
            }
        }
    }

    private bool _acceptTerms;
    public bool AcceptTerms
    {
        get => _acceptTerms;
        set => SetProperty(ref _acceptTerms, value);
    }

    public int Nights => (CheckOut - CheckIn).Days;
    public decimal TotalPrice => Room.Price * Nights;

    public ICommand ConfirmBookingCommand { get; }

    public BookingSummaryViewModel(IBookingService bookingService, IUserSessionService userSessionService)
    {
        _bookingService = bookingService ?? throw new ArgumentNullException(nameof(bookingService));
        _userSessionService = userSessionService ?? throw new ArgumentNullException(nameof(userSessionService));
        Title = "Resumen de Reserva";
        ConfirmBookingCommand = new Command(OnConfirmBooking);
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("property", out var pObj) && pObj is AccommodationDetailDto property)
        {
            Property = property;
            ImageUrl = property.Images?.FirstOrDefault()?.Url ?? string.Empty;
        }
        if (query.TryGetValue("room", out var rObj) && rObj is AccommodationDetailRoomDto room)
            Room = room;
        if (query.TryGetValue("traveler", out var tObj) && tObj is Traveler traveler)
            Traveler = traveler;

        OnPropertyChanged(nameof(Nights));
        OnPropertyChanged(nameof(TotalPrice));
    }

    public async Task EnsureReservationHoldAsync()
    {
        if (_holdCreationAttempted || _isCreatingHold)
        {
            return;
        }

        _holdCreationAttempted = true;
        _isCreatingHold = true;
        try
        {
            var userId = _userSessionService.User?.Id?.Trim();
            if (string.IsNullOrWhiteSpace(userId))
            {
                await Shell.Current.DisplayAlertAsync("Error", "No se encontró un usuario autenticado.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(Room.Id))
            {
                await Shell.Current.DisplayAlertAsync("Error", "No se encontró la habitación seleccionada.", "OK");
                return;
            }

            if (CheckOut <= CheckIn)
            {
                await Shell.Current.DisplayAlertAsync("Error", "Las fechas de la reserva no son válidas.", "OK");
                return;
            }

            var holdPayload = new ReservationHoldRequestDto
            {
                UserId = userId,
                HabitacionId = Room.Id,
                CheckIn = CheckIn.ToString("yyyy-MM-dd"),
                CheckOut = CheckOut.ToString("yyyy-MM-dd")
            };

            var response = await _bookingService.CreateReservationHoldAsync(holdPayload);
            if (response.Error)
            {
                var message = await response.GetErrorMessageAsync();
                await Shell.Current.DisplayAlertAsync(
                    "Error",
                    string.IsNullOrWhiteSpace(message)
                        ? "No fue posible guardar temporalmente tu reserva."
                        : message,
                    "OK");
                return;
            }

            var toast = Toast.Make("Hemos guardado tu selección mientras te decides.", ToastDuration.Short, 14);
            await toast.Show();
        }
        finally
        {
            _isCreatingHold = false;
        }
    }

    private async void OnConfirmBooking()
    {
        if (!AcceptTerms)
        {
            await Shell.Current.DisplayAlertAsync("Aviso", "Debes aceptar los terminos y condiciones.", "OK");
            return;
        }
        var userId = _userSessionService.User?.Id?.Trim();
        if (string.IsNullOrWhiteSpace(userId))
        {
            await Shell.Current.DisplayAlertAsync("Error", "No se encontró un usuario autenticado.", "OK");
            return;
        }
        var holdPayload = new ReservationHoldRequestDto
        {
            UserId = userId,
            HabitacionId = Room.Id,
            CheckIn = CheckIn.ToString("yyyy-MM-dd"),
            CheckOut = CheckOut.ToString("yyyy-MM-dd")
        };

        var response = await _bookingService.CreateReservationAsync(holdPayload);
        if (response.Error || response.Response.Reserva == null)
        {
            var message = await response.GetErrorMessageAsync();
            await Shell.Current.DisplayAlertAsync(
                "Error",
                string.IsNullOrWhiteSpace(message)
                    ? "No fue posible confirmar tu reserva."
                    : message,
                "OK");
            return;
        }

        var toast = Toast.Make("Reserva confirmada.", ToastDuration.Short, 14);
        await toast.Show();

        var reservationTemporal = new ReservationTemporalDTO
        {
            Booking = response.Response.Reserva,
            Property = Property,
            Room = Room,
            Traveler = Traveler,
            CheckIn = CheckIn,
            CheckOut = CheckOut,
            TotalPrice = TotalPrice
        };

        var navParams = new Dictionary<string, object> { { "reservation", reservationTemporal } };
        await Shell.Current.GoToAsync("BookingConfirmedPage", navParams);
    }
}
