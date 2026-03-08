using System.Collections.ObjectModel;
using System.Windows.Input;
using App.Models;
using App.Services;
using App.Views;

namespace App.ViewModels;

public class ActiveBookingsViewModel : BaseViewModel
{
    public ObservableCollection<Reservation> Reservations { get; } = [];

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

    public ActiveBookingsViewModel()
    {
        Title = "Mis Reservas";
        RefreshCommand = new Command(OnRefresh);
        CheckInCommand = new Command<Reservation>(OnCheckIn);
        BookingSelectedCommand = new Command<Reservation>(OnBookingSelected);
        LoadReservations();
    }

    private void LoadReservations()
    {
        Reservations.Clear();
        foreach (var r in MockDataService.GetActiveReservations())
            Reservations.Add(r);
        OnPropertyChanged(nameof(HasReservations));
    }

    private void OnRefresh()
    {
        LoadReservations();
        IsRefreshing = false;
    }

    private async void OnCheckIn(Reservation? reservation)
    {
        if (reservation == null) return;
        await Shell.Current.DisplayAlert("Check-in", $"Aquí va abrir la camara y leer el QR generado para reserva {reservation.BookingCode}", "OK");
    }

    private async void OnBookingSelected(Reservation? reservation)
    {
        if (reservation == null) return;

        // Navegar a BookingDetailsPage pasando el ID de la reserva
        var navParams = new Dictionary<string, object> { { "ReservationId", reservation.Id } };
        await Shell.Current.GoToAsync("BookingDetailsPage", navParams);
    }
}
