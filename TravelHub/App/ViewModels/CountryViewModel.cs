using System.Collections.ObjectModel;
using System.Windows.Input;
using App.Models;
using App.Providers.Interfaces;
using App.Services.Implementations;
using App.Services.Interfaces;

namespace App.ViewModels;

public partial class CountryViewModel : BaseViewModel
{
    private ObservableCollection<CountryItem> _countries = [];
    private readonly ICountryService _countryService;
    private readonly IBackendUrlProvider _backendUrlProvider;
    private bool _isLoading;
    private string _errorMessage = string.Empty;

    public ObservableCollection<CountryItem> Countries
    {
        get => _countries;
        set => SetProperty(ref _countries, value);
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

    public ICommand SelectCountryCommand { get; }
    public ICommand RetryLoadCommand { get; }

    public CountryViewModel(ICountryService countryService, IBackendUrlProvider backendUrlProvider)
    {
        _countryService = countryService ?? throw new ArgumentNullException(nameof(countryService));
        _backendUrlProvider = backendUrlProvider ?? throw new ArgumentNullException(nameof(backendUrlProvider));
        SelectCountryCommand = new Command<CountryItem?>(async (country) => await SelectCountry(country));
        RetryLoadCommand = new Command(async () => await LoadCountries());
        _backendUrlProvider.BaseUrlChanged += OnBackendUrlChanged;
        
        // Cargar pa�ses al inicializar
        MainThread.BeginInvokeOnMainThread(async () => await LoadCountries());
    }

    private async Task LoadCountries()
    {
        IsLoading = true;
        ErrorMessage = string.Empty;

        try
        {
            var response = await _countryService.GetCountriesAsync();

            if (response.Error)
            {
                ErrorMessage = "No se pudieron cargar los países. Intenta más tarde.";
                return;
            }

            if (response.Response == null || response.Response.Count == 0)
            {
                ErrorMessage = "No hay países disponibles.";
                return;
            }

            var currentCode = AppSettingsService.Instance.CurrentCountryCode;
            Countries.Clear();

            foreach (var country in response.Response)
            {
                Countries.Add(new CountryItem(country, country.Code == currentCode));
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error en LoadCountries: {ex.Message}");
            ErrorMessage = "Error inesperado al cargar los países.";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task SelectCountry(CountryItem? selectedCountry)
    {
        if (selectedCountry == null) return;

        // Actualizar selecci�n
        foreach (var country in Countries)
        {
            country.IsSelected = country.Code == selectedCountry.Code;
        }

        // Guardar configuraci�n
        AppSettingsService.Instance.SetCountry(selectedCountry.Code);

        // Mostrar confirmaci�n
        //await Shell.Current.DisplayAlert(
        //    "Pa�s seleccionado",
        //    $"Ahora est�s navegando en {selectedCountry.Name}",
        //    "OK");

        // Volver atr�s
        await Shell.Current.GoToAsync("..");
    }

    private void OnBackendUrlChanged(object? sender, string newBaseUrl)
    {
        MainThread.BeginInvokeOnMainThread(async () => await LoadCountries());
    }
}

public class CountryItem : BaseViewModel
{
    private string _id;
    public string Id
    {
        get => _id;
        init => SetProperty(ref _id, value);
    }

    private string _name;
    public string Name
    {
        get => _name;
        init => SetProperty(ref _name, value);
    }

    private string _code;
    public string Code
    {
        get => _code;
        init => SetProperty(ref _code, value);
    }

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

    private string _flagEmoji;
    public string FlagEmoji
    {
        get => _flagEmoji;
        init => SetProperty(ref _flagEmoji, value);
    }

    private string _phoneCode;
    public string PhoneCode
    {
        get => _phoneCode;
        init => SetProperty(ref _phoneCode, value);
    }

    private bool _isSelected;
    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }

    public CountryItem(Country value, bool isSelected)
    {
        Id = value.Id;
        Name = value.Name;
        Code = value.Code;
        CurrencyCode = value.CurrencyCode;
        CurrencySymbol = value.CurrencySymbol;
        FlagEmoji = value.FlagEmoji;
        PhoneCode = value.PhoneCode;
        IsSelected = isSelected;
    }
}
