using App.Models;
using App.Responses;

namespace App.Services.Interfaces
{
    public interface ICityService
    {
        Task<HttpResponseWrapper<List<string>>> GetPopularCitiesByCountryAsync(string countryCode);
    }
}
