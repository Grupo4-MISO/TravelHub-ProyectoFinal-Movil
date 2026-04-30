using App.Models;
using App.Services.Interfaces;

namespace App.Services.Implementations;

public class AppSettingsService : IAppSettingsService
{
    private const string CurrentVersionKey = "CurrentVersion";
    private const string CountryCodeKey = "SelectedCountryCode";
    private readonly IPreferencesService _preferences;
    private string _currentCountryCode;

    public AppSettingsService(IPreferencesService preferences)
    {
        _preferences = preferences ?? throw new ArgumentNullException(nameof(preferences));
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

    public Country? CurrentCountry => MockDataService.GetCountryByCode(CurrentCountryCode);

    public void SetCountry(string countryCode)
    {
        CurrentCountryCode = countryCode;
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
