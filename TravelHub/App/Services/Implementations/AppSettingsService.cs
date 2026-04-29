using App.Models;
using App.Services.Interfaces;

namespace App.Services.Implementations;

public class AppSettingsService : IAppSettingsService
{
    private const string CurrentVersionKey = "CurrentVersion";
    private const string CountryCodeKey = "SelectedCountryCode";
    private static AppSettingsService? _instance;
    private string _currentCountryCode;

    public static AppSettingsService Instance => _instance ??= new AppSettingsService();

    public event EventHandler<string>? CountryChanged;

    private AppSettingsService()
    {
        _currentCountryCode = Preferences.Default.Get(CountryCodeKey, "CO"); // Colombia por defecto
    }

    public string CurrentCountryCode
    {
        get => _currentCountryCode;
        set
        {
            if (_currentCountryCode != value)
            {
                _currentCountryCode = value;
                Preferences.Default.Set(CountryCodeKey, value);
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
        get => Preferences.Default.Get(CurrentVersionKey, VersionTracking.CurrentVersion);
        set => Preferences.Default.Set(CurrentVersionKey, value);
    }
    public void SetCurrentVersion(string version)
    {
        CurrentVersion = version;
    }
}
