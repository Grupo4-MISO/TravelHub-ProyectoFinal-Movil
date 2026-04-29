using App.Models;

namespace App.Repositories.Interfaces;

public interface ICountryRepository
{
    Task<IReadOnlyList<Country>> GetCountriesAsync();
    Task SaveCountriesAsync(IEnumerable<Country> countries);
    Task ClearAsync();
}
