using App.Models;

namespace App.Services.Interfaces;

public interface IAppSettingsService
{
    Country? CurrentCountry { get; }
    Task<Country?> GetCurrentCountryAsync();
    string CurrentCountryCode { get; set; }
    event EventHandler<string>? CountryChanged;
    void SetCountry(string countryCode);

    string CurrentCurrencyCode { get; set; }
    string CurrentCurrencySymbol { get; }
    event EventHandler<string>? CurrencyChanged;
    void SetCurrency(string currencyCode);
}
