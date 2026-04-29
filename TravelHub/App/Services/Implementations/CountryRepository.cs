using App.Models;
using App.Services.Interfaces;

namespace App.Services.Implementations;

public class CountryRepository : ICountryRepository
{
    private readonly IDatabaseService _databaseService;

    public CountryRepository(IDatabaseService databaseService)
    {
        _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
    }

    public async Task<IReadOnlyList<Country>> GetCountriesAsync()
    {
        var db = await _databaseService.InitSQL();
        var countries = await db.Table<Country>()
            .OrderBy(country => country.Name)
            .ToListAsync();

        return countries;
    }

    public async Task SaveCountriesAsync(IEnumerable<Country> countries)
    {
        if (countries == null)
        {
            throw new ArgumentNullException(nameof(countries));
        }

        var countriesToSave = countries
            .Where(country => !string.IsNullOrWhiteSpace(country.Id))
            .GroupBy(country => country.Id, StringComparer.Ordinal)
            .Select(group => group.Last())
            .ToList();

        var db = await _databaseService.InitSQL();

        await db.RunInTransactionAsync(connection =>
        {
            connection.DeleteAll<Country>();
            foreach (var country in countriesToSave)
            {
                connection.InsertOrReplace(country);
            }
        });
    }

    public async Task ClearAsync()
    {
        var db = await _databaseService.InitSQL();
        await db.DeleteAllAsync<Country>();
    }
}
