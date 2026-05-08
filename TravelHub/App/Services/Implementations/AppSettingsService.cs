using App.Models;
using App.Services.Interfaces;

namespace App.Services.Implementations;

public class AppSettingsService : IAppSettingsService
{
    private const string CurrentVersionKey = "CurrentVersion";
    private const string CountryCodeKey = "SelectedCountryCode";
    private readonly IPreferencesService _preferences;
    private readonly ICountryService _countryService;
    private string _currentCountryCode;
    private Country? _cachedCountry;

    public AppSettingsService(IPreferencesService preferences, ICountryService countryService)
    {
        _preferences = preferences ?? throw new ArgumentNullException(nameof(preferences));
        _countryService = countryService ?? throw new ArgumentNullException(nameof(countryService));
        _currentCountryCode = _preferences.Get(CountryCodeKey, "CO"); // Colombia por defecto
    }

    public event EventHandler<string>? CountryChanged;

    public string CurrentCountryCode
    {
        get => _currentCountryCode;
        set
        {
            if (_currentCountryCode != value)
            {
                _currentCountryCode = value;
                _preferences.Set(CountryCodeKey, value);
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

    public void SetCountry(string countryCode)
    {
        CurrentCountryCode = countryCode;
        _cachedCountry = null;
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
