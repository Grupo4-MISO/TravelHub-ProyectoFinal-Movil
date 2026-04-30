using App.DTOs;
using App.Models;
using System.Windows.Input;

namespace App.ViewModels;

public class BookingSummaryViewModel : BaseViewModel, IQueryAttributable
{
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

    public BookingSummaryViewModel()
    {
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

    private async void OnConfirmBooking()
    {
        if (!AcceptTerms)
        {
            await Shell.Current.DisplayAlert("Aviso", "Debes aceptar los terminos y condiciones.", "OK");
            return;
        }

        //var reservation = new Reservation
        //{
        //    Id = new Random().Next(1000, 9999),
        //    BookingCode = $"TH-{DateTime.Now:yyyy}-{new Random().Next(1000, 9999):D4}",
        //    Property = Property,
        //    Room = Room,
        //    Traveler = Traveler,
        //    CheckIn = CheckIn,
        //    CheckOut = CheckOut,
        //    Adults = 2,
        //    TotalPrice = TotalPrice,
        //    Status = "Confirmada"
        //};

        //var navParams = new Dictionary<string, object> { { "reservation", reservation } };
        //await Shell.Current.GoToAsync("BookingConfirmedPage", navParams);
    }
}
