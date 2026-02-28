using System.Windows.Input;
using App.Models;

namespace App.ViewModels;

public class BookingConfirmedViewModel : BaseViewModel, IQueryAttributable
{
    private Reservation _reservation = new();
    public Reservation Reservation
    {
        get => _reservation;
        set => SetProperty(ref _reservation, value);
    }

    public ICommand GoHomeCommand { get; }
    public ICommand ViewBookingsCommand { get; }

    public BookingConfirmedViewModel()
    {
        Title = "Reserva Confirmada";
        GoHomeCommand = new Command(OnGoHome);
        ViewBookingsCommand = new Command(OnViewBookings);
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("reservation", out var obj) && obj is Reservation reservation)
            Reservation = reservation;
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
