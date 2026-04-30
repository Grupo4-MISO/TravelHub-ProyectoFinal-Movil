using App.Models;

namespace App.Services.Interfaces;

public interface IAppSettingsService
{
    Country? CurrentCountry { get; }
    string CurrentCountryCode { get; set; }
    event EventHandler<string>? CountryChanged;
    void SetCountry(string countryCode);
}
