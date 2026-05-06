using App.DTOs;
using App.Models;
using App.Services.Implementations;
using App.Services.Interfaces;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace App.ViewModels;

public class BookingConfirmedViewModel : BaseViewModel, IQueryAttributable
{
    private readonly IUserSessionService _userSessionService;
    private readonly IBookingService _bookingService;
    private bool _paymentProvidersLoaded;
    private bool _isLoadingProviders;

    private ReservationTemporalDTO _reservation = new();
    public ReservationTemporalDTO Reservation
    {
        get => _reservation;
        set => SetProperty(ref _reservation, value);
    }

    public ObservableCollection<PaymentProviderDto> PaymentProviders { get; } = [];

    private PaymentProviderDto? _selectedPaymentProvider;
    public PaymentProviderDto? SelectedPaymentProvider
    {
        get => _selectedPaymentProvider;
        set
        {
            if (SetProperty(ref _selectedPaymentProvider, value))
            {
                OnPropertyChanged(nameof(IsPaymentProviderSelected));
                OnPropertyChanged(nameof(PayButtonText));
            }
        }
    }

    public bool IsPaymentProviderSelected => SelectedPaymentProvider != null;
    public string PayButtonText => SelectedPaymentProvider == null
        ? "Pagar"
        : $"Pagar con {SelectedPaymentProvider.Name}";

    private string _currency = "COP";
    public string Currency
    {
        get => _currency;
        set => SetProperty(ref _currency, value);
    }

    public ICommand SelectPaymentProviderCommand { get; }
    public ICommand PayCommand { get; }
    public ICommand GoHomeCommand { get; }
    public ICommand ViewBookingsCommand { get; }

    public BookingConfirmedViewModel(IBookingService bookingService, IUserSessionService userSessionService)
    {
        _bookingService = bookingService ?? throw new ArgumentNullException(nameof(bookingService));
        _userSessionService = userSessionService ?? throw new ArgumentNullException(nameof(userSessionService));
        Title = "Reserva Confirmada";
        SelectPaymentProviderCommand = new Command<PaymentProviderDto>(OnSelectPaymentProvider);
        PayCommand = new Command(OnPay);
        GoHomeCommand = new Command(OnGoHome);
        ViewBookingsCommand = new Command(OnViewBookings);
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("reservation", out var obj) && obj is ReservationTemporalDTO reservation)
            Reservation = reservation;
    }

    public async Task EnsurePaymentProvidersLoadedAsync()
    {
        if (_paymentProvidersLoaded || _isLoadingProviders)
        {
            return;
        }

        _isLoadingProviders = true;
        try
        {
            var response = await _bookingService.GetPaymentProvidersAsync();
            if (response.Error || response.Response == null)
            {
                var message = await response.GetErrorMessageAsync();
                await Shell.Current.DisplayAlertAsync(
                    "Error",
                    string.IsNullOrWhiteSpace(message)
                        ? "No fue posible cargar los medios de pago."
                        : message,
                    "OK");
                return;
            }

            PaymentProviders.Clear();
            foreach (var provider in response.Response.Where(p => p.IsActive))
            {
                PaymentProviders.Add(provider);
            }

            _paymentProvidersLoaded = true;
        }
        finally
        {
            _isLoadingProviders = false;
        }
    }

    private void OnSelectPaymentProvider(PaymentProviderDto? provider)
    {
        SelectedPaymentProvider = provider;
    }

    private async void OnPay()
    {
        if (SelectedPaymentProvider == null)
        {
            return;
        }

        //await Shell.Current.DisplayAlertAsync(
        //    "Pago",
        //    $"Continuaremos con {SelectedPaymentProvider.Name}.",
        //    "OK");

        var paymentRequest = new PaymentRequestDTO
        {
            ReservaId = Reservation.Booking.Id,
            Amount = Reservation.TotalPrice,
            Currency = Currency, // Cambiar según sea necesario
            ProviderId = SelectedPaymentProvider.Id,
            Description = $"Pago reserva {Reservation.Booking.BookingCode}",
            Metadata = new Dictionary<string, string>
            {
                { "user_id", _userSessionService.User.Id },
                { "name", _userSessionService.User.Username }
            }
        };

        var response = await _bookingService.CreatePaymentAsync(paymentRequest);
        if (response.Error || response.Response == null)
        {
            var message = await response.GetErrorMessageAsync();
            await Shell.Current.DisplayAlertAsync(
                "Error",
                string.IsNullOrWhiteSpace(message)
                    ? "No fue posible crear un pago."
                    : message,
                "OK");
            return;
        }

        var payment = response.Response;
        if (payment.Url == null)
        {
            await Shell.Current.DisplayAlertAsync(
                "Error",
                "No fue posible crear un pago.",
                "OK");
            return;
        }

        await Launcher.OpenAsync(payment.Url);
        GoHomeCommand.Execute(null);
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
