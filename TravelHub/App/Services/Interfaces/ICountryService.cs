using App.Models;
using App.Responses;

namespace App.Services.Interfaces
{
    public interface ICountryService
    {
        Task<HttpResponseWrapper<List<Country>>> GetCountriesAsync();
        Country? GetCountryByCode(string code);
        Country? GetCountryById(string id);
        List<string> GetPopularCitiesByCountry(string countryCode);
    }
}
