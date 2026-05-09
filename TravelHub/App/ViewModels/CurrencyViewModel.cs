using System.Collections.ObjectModel;
using System.Windows.Input;
using App.Models;
using App.Services.Interfaces;

namespace App.ViewModels;

public partial class CurrencyViewModel : BaseViewModel
{
    private ObservableCollection<CurrencyItem> _currencies = [];
    private readonly ICountryService _countryService;
    private readonly IMainThreadService _mainThreadService;
    private readonly IAppSettingsService _appSettingsService;
    private bool _isLoading;
    private string _errorMessage = string.Empty;

    public ObservableCollection<CurrencyItem> Currencies
    {
        get => _currencies;
        set => SetProperty(ref _currencies, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public ICommand SelectCurrencyCommand { get; }
    public ICommand RetryLoadCommand { get; }

    public CurrencyViewModel(ICountryService countryService, IMainThreadService mainThreadService, IAppSettingsService appSettingsService)
    {
        _countryService = countryService ?? throw new ArgumentNullException(nameof(countryService));
        _mainThreadService = mainThreadService ?? throw new ArgumentNullException(nameof(mainThreadService));
        _appSettingsService = appSettingsService ?? throw new ArgumentNullException(nameof(appSettingsService));
        SelectCurrencyCommand = new Command<CurrencyItem?>(async (currency) => await SelectCurrency(currency));
        RetryLoadCommand = new Command(async () => await LoadCurrencies());

        _mainThreadService.BeginInvokeOnMainThread(async () => await LoadCurrencies());
    }

    private async Task LoadCurrencies()
    {
        IsLoading = true;
        ErrorMessage = string.Empty;

        try
        {
            var response = await _countryService.GetCountriesAsync();

            if (response.Error)
            {
                ErrorMessage = "No se pudieron cargar las monedas. Intenta más tarde.";
                return;
            }

            if (response.Response == null || response.Response.Count == 0)
            {
                ErrorMessage = "No hay monedas disponibles.";
                return;
            }

            var currentCode = _appSettingsService.CurrentCurrencyCode;
            var uniqueCurrencies = new Dictionary<string, (string code, string symbol, Country first)>();

            foreach (var country in response.Response)
            {
                if (!uniqueCurrencies.ContainsKey(country.CurrencyCode))
                {
                    uniqueCurrencies[country.CurrencyCode] = (country.CurrencyCode, country.CurrencySymbol, country);
                }
            }

            Currencies.Clear();

            foreach (var kvp in uniqueCurrencies)
            {
                Currencies.Add(new CurrencyItem(kvp.Value.code, kvp.Value.symbol, kvp.Value.code == currentCode));
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error en LoadCurrencies: {ex.Message}");
            ErrorMessage = "Error inesperado al cargar las monedas.";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task SelectCurrency(CurrencyItem? selectedCurrency)
    {
        if (selectedCurrency == null) return;

        foreach (var currency in Currencies)
        {
            currency.IsSelected = currency.CurrencyCode == selectedCurrency.CurrencyCode;
        }

        _appSettingsService.SetCurrency(selectedCurrency.CurrencyCode);

        await Shell.Current.GoToAsync("..");
    }
}

public class CurrencyItem : BaseViewModel
{
    private string _currencyCode;
    public string CurrencyCode
    {
        get => _currencyCode;
        init => SetProperty(ref _currencyCode, value);
    }

    private string _currencySymbol;
    public string CurrencySymbol
    {
        get => _currencySymbol;
        init => SetProperty(ref _currencySymbol, value);
    }

    public string DisplayName => $"{CurrencyCode} ({CurrencySymbol})";

    private bool _isSelected;
    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }

    public CurrencyItem(string currencyCode, string currencySymbol, bool isSelected)
    {
        _currencyCode = currencyCode;
        _currencySymbol = currencySymbol;
        _isSelected = isSelected;
    }
}
