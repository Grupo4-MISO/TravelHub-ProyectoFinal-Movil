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
    private readonly IQrScanResultService _qrScanResultService;

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

    public ActiveBookingsViewModel(IBookingService bookingService, IUserSessionService userSessionService, IQrScanResultService qrScanResultService)
    {
        Title = "Mis Reservas";
        _bookingService = bookingService;
        _userSessionService = userSessionService;
        _qrScanResultService = qrScanResultService;

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

    private Reservation MapToReservation(BookingHoldResponseDto dto, Dictionary<string, HotelInventoryDto>? hotels)
    {
        var reservation = new Reservation
        {
            Id = dto.Id,
            BookingCode = dto.PublicId,
            RoomId = dto.HabitacionId,
            CheckIn = DateTime.TryParse(dto.CheckIn, out var checkIn) ? checkIn : DateTime.MinValue,
            CheckOut = DateTime.TryParse(dto.CheckOut, out var checkOut) ? checkOut : DateTime.MinValue,
            Status = dto.Estado,
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt
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

    private TaskCompletionSource<string?>? _tcs;

    private async void OnCheckIn(Reservation? reservation)
    {
        if (reservation == null) return;

        var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
        if (status != PermissionStatus.Granted)
        {
            status = await Permissions.RequestAsync<Permissions.Camera>();
            if (status != PermissionStatus.Granted)
            {
                await Shell.Current.DisplayAlertAsync("Permiso requerido", "Se requiere acceso a la cámara para escanear el código QR", "OK");
                return;
            }
        }
        try
        {

            _qrScanResultService.Clear();
            _tcs = new TaskCompletionSource<string?>();

            Shell.Current.Navigated += OnShellNavigated;

            await Shell.Current.GoToAsync("QrScannerPage");

            var scannedUrl = await _tcs.Task;
            var bookingId = scannedUrl?.Split('/').LastOrDefault();

            Shell.Current.Navigated -= OnShellNavigated;

            if (!string.IsNullOrEmpty(bookingId))
            {
                await ProcessCheckInAsync(reservation, bookingId);
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync("Error", $"Ocurrió un error: {ex.Message}", "OK");
        }
    }

    private void OnShellNavigated(object? sender, ShellNavigatedEventArgs e)
    {
        var location = e.Current?.Location?.OriginalString ?? "";
        if (location.Contains("QrScannerPage", StringComparison.OrdinalIgnoreCase))
            return;

        if (_tcs != null && !_tcs.Task.IsCompleted)
        {
            _tcs.TrySetResult(_qrScanResultService.ScannedUrl);
        }
    }

    private async Task ProcessCheckInAsync(Reservation reservation, string bookingId)
    {
        try
        {
            IsBusy = true;

            var bookingsResponse = await _bookingService.CheckInBookingByReservationIdAsync(bookingId);
            if (bookingsResponse.Error )
            {
                var errorMsg = await bookingsResponse.GetErrorMessageAsync();
                await Shell.Current.DisplayAlertAsync("Error", $"No se pudo completar el check-in: {errorMsg}", "OK");
                return;
            }

            await Shell.Current.DisplayAlertAsync("Check-in exitoso", "La reserva ha sido completada", "OK");
            await LoadReservationsAsync();
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync("Error", $"Ocurrió un error: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async void OnBookingSelected(Reservation? reservation)
    {
        if (reservation == null) return;

        var navParams = new Dictionary<string, object> { { "ReservationId", reservation.Id } };
        await Shell.Current.GoToAsync("BookingDetailsPage", navParams);
    }
}
