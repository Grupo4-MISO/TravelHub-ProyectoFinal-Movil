using App.Models;
using App.Repositories.Interfaces;
using App.Services.Interfaces;

namespace App.Repositories.Implementations;

public class CityRepository : ICityRepository
{
    private readonly IDatabaseService _databaseService;

    public CityRepository(IDatabaseService databaseService)
    {
        _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
    }

    public async Task<IReadOnlyList<City>> GetCitiesAsync(string countryCode)
    {
        var db = await _databaseService.InitSQL();
        var cities = await db.Table<City>()
            .Where(city => city.CountryCode == countryCode)
            .OrderBy(city => city.Name)
            .ToListAsync();

        return cities;
    }

    public async Task SaveCitiesAsync(IEnumerable<City> cities)
    {
        if (cities == null)
        {
            throw new ArgumentNullException(nameof(cities));
        }

        var citiesToSave = cities   
            .Where(city => !string.IsNullOrWhiteSpace(city.Id))
            .GroupBy(city => city.CountryCode, StringComparer.Ordinal)
            .Select(group => group.Last())
            .ToList();

        var db = await _databaseService.InitSQL();

        await db.RunInTransactionAsync(connection =>
        {
            connection.DeleteAll<City>();
            foreach (var city in citiesToSave)
            {
                connection.InsertOrReplace(city);
            }
        });
    }

    public async Task ClearAsync()
    {
        var db = await _databaseService.InitSQL();
        await db.DeleteAllAsync<City>();
    }
}
