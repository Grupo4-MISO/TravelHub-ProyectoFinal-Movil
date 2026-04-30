using System.Net;
using App.Models;
using App.Providers.Interfaces;
using App.Responses;
using App.Services.Interfaces;

namespace App.Services.Implementations;

public class AccommodationSearchService : IAccommodationSearchService
{
    private const string SearchEndpoint = "/api/v1/busquedas/search";

    private readonly IBackEndService _backEndService;
    private readonly IBackendUrlProvider _backendUrlProvider;

    public AccommodationSearchService(IBackEndService backEndService, IBackendUrlProvider backendUrlProvider)
    {
        _backEndService = backEndService ?? throw new ArgumentNullException(nameof(backEndService));
        _backendUrlProvider = backendUrlProvider ?? throw new ArgumentNullException(nameof(backendUrlProvider));
    }

    public async Task<HttpResponseWrapper<List<SearchAccommodationDto>>> SearchAccommodationsAsync(SearchCriteria criteria)
    {
        if (criteria == null)
        {
            return new HttpResponseWrapper<List<SearchAccommodationDto>>(
                default!,
                true,
                new HttpResponseMessage(HttpStatusCode.BadRequest));
        }

        var url = BuildSearchUrl(criteria);
        var response = await _backEndService.GetAsync<List<SearchAccommodationDto>>(url);
        if (response.Error || response.Response == null)
        {
            return new HttpResponseWrapper<List<SearchAccommodationDto>>(default!, true, response.HttpResponseMessage);
        }

        return new HttpResponseWrapper<List<SearchAccommodationDto>>(response.Response, false, response.HttpResponseMessage);
    }

    private string BuildSearchUrl(SearchCriteria criteria)
    {
        var normalizedCountryCode = string.IsNullOrWhiteSpace(criteria.CountryCode)
            ? "CO"
            : criteria.CountryCode.Trim().ToUpperInvariant();

        var normalizedCurrencyCode = string.IsNullOrWhiteSpace(criteria.CurrencyCode)
            ? "COP"
            : criteria.CurrencyCode.Trim().ToUpperInvariant();

        var queryParams = new Dictionary<string, string>
        {
            ["ciudad"] = (criteria.City ?? string.Empty).Trim(),
            ["capacidad"] = Math.Max(1, criteria.Adults + criteria.Children).ToString(),
            ["check_in"] = criteria.CheckIn.ToString("yyyy-MM-dd"),
            ["check_out"] = criteria.CheckOut.ToString("yyyy-MM-dd"),
            ["country_code"] = normalizedCountryCode,
            ["currency_code"] = normalizedCurrencyCode
        };

        var queryString = string.Join("&",
            queryParams.Select(param => $"{param.Key}={Uri.EscapeDataString(param.Value)}"));

        return $"{_backendUrlProvider.Build(SearchEndpoint)}?{queryString}";
    }

}
