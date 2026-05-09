using App.DTOs;
using App.Models;
using App.Services.Interfaces;
using System.Windows.Input;

namespace App.ViewModels;

public class BookingConfirmedViewModel : BaseViewModel, IQueryAttributable
{
    private readonly IAppSettingsService _appSettingsService;
    private ReservationTemporalDTO _reservation = new();
    public ReservationTemporalDTO Reservation
    {
        get => _reservation;
        set => SetProperty(ref _reservation, value);
    }

    public ICommand PayCommand { get; }
    public ICommand GoHomeCommand { get; }
    public ICommand ViewBookingsCommand { get; }

    public BookingConfirmedViewModel(IAppSettingsService appSettingsService)
    {
        _appSettingsService = appSettingsService ?? throw new ArgumentNullException(nameof(appSettingsService));
        Title = "Reserva Confirmada";
        PayCommand = new Command(OnPay);
        GoHomeCommand = new Command(OnGoHome);
        ViewBookingsCommand = new Command(OnViewBookings);
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("reservation", out var obj) && obj is ReservationTemporalDTO reservation)
            Reservation = reservation;
    }

    private async void OnPay()
    {
        var navParams = new Dictionary<string, object>
        {
            { "amount", Reservation.TotalPrice },
            { "referenceId", Reservation.Booking.Id },
            { "description", $"Pago reserva {Reservation.Booking.BookingCode}" },
            { "currency", _appSettingsService.CurrentCurrencyCode },
            { "returnRoute", "//home" }
        };
        await Shell.Current.GoToAsync("PaymentPage", navParams);
    }

    private async void OnGoHome()
    {
        await Shell.Current.GoToAsync("//home");
    }

    private async void OnViewBookings()
    {
        await Shell.Current.GoToAsync("//bookings");
    }
}
