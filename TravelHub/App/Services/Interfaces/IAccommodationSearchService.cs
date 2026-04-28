using App.Models;
using App.Responses;

namespace App.Services.Interfaces;

public interface IAccommodationSearchService
{
    Task<HttpResponseWrapper<List<Property>>> SearchAccommodationsAsync(SearchCriteria criteria);
}
