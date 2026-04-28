using System.Net;
using App.Models;
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

    public async Task<HttpResponseWrapper<List<Property>>> SearchAccommodationsAsync(SearchCriteria criteria)
    {
        if (criteria == null)
        {
            return new HttpResponseWrapper<List<Property>>(
                default!,
                true,
                new HttpResponseMessage(HttpStatusCode.BadRequest));
        }

        var url = BuildSearchUrl(criteria);
        var response = await _backEndService.GetAsync<List<SearchAccommodationDto>>(url);
        if (response.Error || response.Response == null)
        {
            return new HttpResponseWrapper<List<Property>>(default!, true, response.HttpResponseMessage);
        }

        var properties = response.Response.Select(item => MapToProperty(item, criteria)).ToList();
        return new HttpResponseWrapper<List<Property>>(properties, false, response.HttpResponseMessage);
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

    private static Property MapToProperty(SearchAccommodationDto dto, SearchCriteria criteria)
    {
        var hotelOrRoomId = string.IsNullOrWhiteSpace(dto.PropertyId) ? dto.RoomId : dto.PropertyId;
        var computedId = ComputeStablePositiveId(hotelOrRoomId);
        var imageUrl = dto.ImageUrl ?? string.Empty;

        return new Property
        {
            Id = computedId,
            ProviderId = dto.PropertyId ?? string.Empty,
            Name = dto.Name ?? string.Empty,
            City = dto.City ?? string.Empty,
            Address = dto.Address ?? string.Empty,
            Description = dto.Description ?? string.Empty,
            Rating = dto.Rating,
            ReviewCount = dto.Reviews,
            PricePerNight = dto.Price,
            ImageUrl = imageUrl,
            ImageUrls = string.IsNullOrWhiteSpace(imageUrl) ? [] : [imageUrl],
            IsAvailable = true,
            Country = new Country
            {
                Name = dto.Country ?? string.Empty,
                Code = string.IsNullOrWhiteSpace(criteria.CountryCode) ? "CO" : criteria.CountryCode.Trim().ToUpperInvariant(),
                CurrencyCode = string.IsNullOrWhiteSpace(dto.CurrencyCode) ? criteria.CurrencyCode : dto.CurrencyCode
            },
            Rooms =
            [
                new Room
                {
                    Id = ComputeStablePositiveId(dto.RoomId ?? hotelOrRoomId),
                    ProviderId = dto.RoomId ?? string.Empty,
                    Name = string.IsNullOrWhiteSpace(dto.RoomCode) ? "Habitación" : $"Habitación {dto.RoomCode}",
                    Description = dto.Description ?? string.Empty,
                    MaxGuests = Math.Max(1, dto.Capacity),
                    PricePerNight = dto.Price,
                    ImageUrl = imageUrl,
                    IsAvailable = true
                }
            ]
        };
    }

    private static int ComputeStablePositiveId(string source)
    {
        return source.GetHashCode() & int.MaxValue;
    }
}
