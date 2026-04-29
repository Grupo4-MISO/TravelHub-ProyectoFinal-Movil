using App.Models;

namespace App.Services.Interfaces;

public interface ICountryRepository
{
    Task<IReadOnlyList<Country>> GetCountriesAsync();
    Task SaveCountriesAsync(IEnumerable<Country> countries);
    Task ClearAsync();
}
