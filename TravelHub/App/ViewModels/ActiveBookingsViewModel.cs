using System.Collections.ObjectModel;
using System.Windows.Input;
using App.DTOs;
using App.Models;
using App.Services.Interfaces;
using App.Views;

namespace App.ViewModels;

public class ActiveBookingsViewModel : BaseViewModel
{
    private readonly IBookingService _bookingService;
    private readonly IUserSessionService _userSessionService;

    public ObservableCollection<Reservation> Reservations { get; } = new ObservableCollection<Reservation>();

    public ICommand RefreshCommand { get; }
    public ICommand CheckInCommand { get; }
    public ICommand BookingSelectedCommand { get; }

    private bool _isRefreshing;
    public bool IsRefreshing
    {
        get => _isRefreshing;
        set => SetProperty(ref _isRefreshing, value);
    }

    public bool HasReservations => Reservations.Count > 0;

    private string _errorMessage = string.Empty;
    public string ErrorMessage
    {
        get => _errorMessage;
        set
        {
            if (SetProperty(ref _errorMessage, value))
                OnPropertyChanged(nameof(HasError));
        }
    }

    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

    public ActiveBookingsViewModel(IBookingService bookingService, IUserSessionService userSessionService)
    {
        Title = "Mis Reservas";
        _bookingService = bookingService;
        _userSessionService = userSessionService;

        RefreshCommand = new Command(async () => await LoadReservationsAsync());
        CheckInCommand = new Command<Reservation>(OnCheckIn);
        BookingSelectedCommand = new Command<Reservation>(OnBookingSelected);

        _ = LoadReservationsAsync();
    }

    private async Task LoadReservationsAsync()
    {
        if (IsBusy) return;
        IsBusy = true;
        IsRefreshing = true;
        ErrorMessage = string.Empty;

        try
        {
            Reservations.Clear();

            var userId = _userSessionService.User?.Id;
            if (string.IsNullOrEmpty(userId))
            {
                ErrorMessage = "Usuario no autenticado";
                OnPropertyChanged(nameof(HasReservations));
                return;
            }

            var bookingsResponse = await _bookingService.GetUserBookingsAsync(userId);
            if (bookingsResponse.Error || bookingsResponse.Response == null)
            {
                ErrorMessage = await bookingsResponse.GetErrorMessageAsync();
                OnPropertyChanged(nameof(HasReservations));
                return;
            }

            var bookings = bookingsResponse.Response;
            if (bookings.Count == 0)
            {
                OnPropertyChanged(nameof(HasReservations));
                return;
            }

            var roomIds = bookings.Select(b => b.HabitacionId).Distinct().ToList();
            var hotelsResponse = await _bookingService.GetHotelsByRoomIdsAsync(roomIds);

            Dictionary<string, HotelInventoryDto>? hotels = null;
            if (!hotelsResponse.Error && hotelsResponse.Response != null)
            {
                hotels = hotelsResponse.Response;
            }

            foreach (var booking in bookings)
            {
                var reservation = MapToReservation(booking, hotels);
                Reservations.Add(reservation);
            }

            OnPropertyChanged(nameof(HasReservations));
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
            IsRefreshing = false;
        }
    }

    private Reservation MapToReservation(BookingResponseDto dto, Dictionary<string, HotelInventoryDto>? hotels)
    {
        var reservation = new Reservation
        {
            Id = dto.Id,
            BookingCode = dto.PublicId,
            RoomId = dto.HabitacionId,
            CheckIn = DateTime.TryParse(dto.CheckIn, out var checkIn) ? checkIn : DateTime.MinValue,
            CheckOut = DateTime.TryParse(dto.CheckOut, out var checkOut) ? checkOut : DateTime.MinValue,
            Status = dto.Estado,
            CreatedAt = DateTime.TryParse(dto.CreatedAt, out var createdAt) ? createdAt : DateTime.MinValue,
            UpdatedAt = DateTime.TryParse(dto.UpdatedAt, out var updatedAt) ? updatedAt : DateTime.MinValue
        };

        if (hotels != null && hotels.TryGetValue(dto.HabitacionId, out var hotel))
        {
            reservation.HotelName = hotel.Nombre;
            reservation.HotelCity = hotel.Ciudad;
            reservation.HotelCountry = hotel.Pais;
            reservation.HotelAddress = hotel.Direccion;
            reservation.HotelImage = hotel.Imagen;
        }

        return reservation;
    }

    private async void OnCheckIn(Reservation? reservation)
    {
        if (reservation == null) return;
        await Shell.Current.DisplayAlert("Check-in", $"Aquí va abrir la cámara y leer el QR generado para reserva {reservation.BookingCode}", "OK");
    }

    private async void OnBookingSelected(Reservation? reservation)
    {
        if (reservation == null) return;

        var navParams = new Dictionary<string, object> { { "ReservationId", reservation.Id } };
        await Shell.Current.GoToAsync("BookingDetailsPage", navParams);
    }
}
