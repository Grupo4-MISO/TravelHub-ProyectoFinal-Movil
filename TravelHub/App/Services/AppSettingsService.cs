using App.Models;

namespace App.Services;

public class AppSettingsService
{
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

    public Country CurrentCountry => MockDataService.GetCountryByCode(CurrentCountryCode);

    public void SetCountry(string countryCode)
    {
        CurrentCountryCode = countryCode;
    }
}