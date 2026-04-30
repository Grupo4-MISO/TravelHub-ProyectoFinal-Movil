using App.Models;

namespace App.Repositories.Interfaces;

public interface ICityRepository
{
    Task<IReadOnlyList<City>> GetCitiesAsync(string countryCode);
    Task SaveCitiesAsync(IEnumerable<City> cities);
    Task ClearAsync();
}
