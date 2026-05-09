using App.Models;
using App.Services.Interfaces;

namespace App.Services.Implementations;

public class AppSettingsService : IAppSettingsService
{
    private const string CurrentVersionKey = "CurrentVersion";
    private const string CountryCodeKey = "SelectedCountryCode";
    private const string CurrencyCodeKey = "SelectedCurrencyCode";
    private readonly IPreferencesService _preferences;
    private readonly ICountryService _countryService;
    private string _currentCountryCode;
    private string _currentCurrencyCode;
    private Country? _cachedCountry;

    public AppSettingsService(IPreferencesService preferences, ICountryService countryService)
    {
        _preferences = preferences ?? throw new ArgumentNullException(nameof(preferences));
        _countryService = countryService ?? throw new ArgumentNullException(nameof(countryService));
        _currentCountryCode = _preferences.Get(CountryCodeKey, "CO"); // Colombia por defecto
        _currentCurrencyCode = _preferences.Get(CurrencyCodeKey, "COP"); // COP por defecto
    }

    public event EventHandler<string>? CountryChanged;
    public event EventHandler<string>? CurrencyChanged;

    public string CurrentCountryCode
    {
        get => _currentCountryCode;
        set
        {
            if (_currentCountryCode != value)
            {
                _currentCountryCode = value;
                _preferences.Set(CountryCodeKey, value);
                _cachedCountry = null;
                CountryChanged?.Invoke(this, value);
            }
        }
    }

    public Country? CurrentCountry
    {
        get
        {
            if (_cachedCountry != null && string.Equals(_cachedCountry.Code, _currentCountryCode, StringComparison.OrdinalIgnoreCase))
                return _cachedCountry;

            _cachedCountry = _countryService.GetCountryByCode(_currentCountryCode);
            return _cachedCountry;
        }
    }

    public string CurrentCurrencyCode
    {
        get => _currentCurrencyCode;
        set
        {
            if (_currentCurrencyCode != value)
            {
                _currentCurrencyCode = value;
                _preferences.Set(CurrencyCodeKey, value);
                CurrencyChanged?.Invoke(this, value);
            }
        }
    }

    public string CurrentCurrencySymbol
    {
        get
        {
            var country = CurrentCountry;
            if (country != null && string.Equals(country.CurrencyCode, _currentCurrencyCode, StringComparison.OrdinalIgnoreCase))
                return country.CurrencySymbol;

            return _currentCurrencyCode switch
            {
                "COP" or "CLP" or "ARS" or "MXN" => "$",
                "PEN" => "S/",
                "USD" => "$",
                _ => "$"
            };
        }
    }

    public void SetCountry(string countryCode)
    {
        if (_currentCountryCode != countryCode)
        {
            _currentCountryCode = countryCode;
            _preferences.Set(CountryCodeKey, countryCode);
            _cachedCountry = null;
            CountryChanged?.Invoke(this, countryCode);
        }

        var country = _countryService.GetCountryByCode(countryCode);
        if (country != null && !string.Equals(_currentCurrencyCode, country.CurrencyCode, StringComparison.OrdinalIgnoreCase))
        {
            _currentCurrencyCode = country.CurrencyCode;
            _preferences.Set(CurrencyCodeKey, _currentCurrencyCode);
            CurrencyChanged?.Invoke(this, _currentCurrencyCode);
        }
    }

    public void SetCurrency(string currencyCode)
    {
        CurrentCurrencyCode = currencyCode;
    }

    public string CurrentVersion
    {
        get => _preferences.Get(CurrentVersionKey, "1.0");
        set => _preferences.Set(CurrentVersionKey, value);
    }
    public void SetCurrentVersion(string version)
    {
        CurrentVersion = version;
    }
}
