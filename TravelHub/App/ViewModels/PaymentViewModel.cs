using App.DTOs;
using App.Services.Interfaces;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace App.ViewModels;

public class PaymentViewModel : BaseViewModel, IQueryAttributable
{
    private readonly IUserSessionService _userSessionService;
    private readonly IBookingService _bookingService;
    private bool _paymentProvidersLoaded;
    private bool _isLoadingProviders;

    private decimal _amount;
    public decimal Amount
    {
        get => _amount;
        set => SetProperty(ref _amount, value);
    }

    private string _referenceId = string.Empty;
    public string ReferenceId
    {
        get => _referenceId;
        set => SetProperty(ref _referenceId, value);
    }

    private string _description = string.Empty;
    public string Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }

    private string _currency = "COP";
    public string Currency
    {
        get => _currency;
        set => SetProperty(ref _currency, value);
    }

    private string _returnRoute = "//home";
    public string ReturnRoute
    {
        get => _returnRoute;
        set => SetProperty(ref _returnRoute, value);
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

    public ICommand SelectPaymentProviderCommand { get; }
    public ICommand PayCommand { get; }
    public ICommand GoBackCommand { get; }

    public PaymentViewModel(IBookingService bookingService, IUserSessionService userSessionService)
    {
        _bookingService = bookingService ?? throw new ArgumentNullException(nameof(bookingService));
        _userSessionService = userSessionService ?? throw new ArgumentNullException(nameof(userSessionService));
        Title = "Pago";
        SelectPaymentProviderCommand = new Command<PaymentProviderDto>(OnSelectPaymentProvider);
        PayCommand = new Command(OnPay);
        GoBackCommand = new Command(OnGoBack);
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("amount", out var amount) && amount is decimal d)
            Amount = d;

        if (query.TryGetValue("referenceId", out var refId))
            ReferenceId = refId?.ToString() ?? string.Empty;

        if (query.TryGetValue("description", out var desc))
            Description = desc?.ToString() ?? string.Empty;

        if (query.TryGetValue("currency", out var curr))
            Currency = curr?.ToString() ?? "COP";

        if (query.TryGetValue("returnRoute", out var route))
            ReturnRoute = route?.ToString() ?? "//home";
    }

    public async Task EnsurePaymentProvidersLoadedAsync()
    {
        if (_paymentProvidersLoaded || _isLoadingProviders)
            return;

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
            return;

        var paymentRequest = new PaymentRequestDTO
        {
            ReservaId = ReferenceId,
            Amount = Amount,
            Currency = Currency,
            ProviderId = SelectedPaymentProvider.Id,
            Description = Description,
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
        await Shell.Current.GoToAsync(ReturnRoute);
    }

    private async void OnGoBack()
    {
        await Shell.Current.GoToAsync("..");
    }
}
