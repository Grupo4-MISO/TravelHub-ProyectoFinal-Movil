using System.Net;
using App.Models;
using App.Responses;
using App.Services.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace App.Services.Implementations;

public class PropertyDetailService : IPropertyDetailService
{
    private readonly IBackEndService _backEndService;
    private readonly IBackendUrlProvider _backendUrlProvider;

    public PropertyDetailService(IBackEndService backEndService, IBackendUrlProvider backendUrlProvider)
    {
        _backEndService = backEndService ?? throw new ArgumentNullException(nameof(backEndService));
        _backendUrlProvider = backendUrlProvider ?? throw new ArgumentNullException(nameof(backendUrlProvider));
    }

    public async Task<HttpResponseWrapper<Property>> GetPropertyDetailAsync(string propertyId, string currencyCode)
    {
        if (string.IsNullOrWhiteSpace(propertyId))
        {
            return new HttpResponseWrapper<Property>(default!, true, new HttpResponseMessage(HttpStatusCode.BadRequest));
        }

        var normalizedCurrency = string.IsNullOrWhiteSpace(currencyCode) ? "COP" : currencyCode.Trim().ToUpperInvariant();
        var detailUrl = BuildPropertyDetailUrl(propertyId, normalizedCurrency);

        var response = await _backEndService.GetAsync<AccommodationDetailDto>(detailUrl);
        if (response.Error || response.Response == null)
        {
            return new HttpResponseWrapper<Property>(default!, true, response.HttpResponseMessage);
        }

        var mappedProperty = MapToProperty(response.Response, normalizedCurrency);
        return new HttpResponseWrapper<Property>(mappedProperty, false, response.HttpResponseMessage);
    }

    public async Task<HttpResponseWrapper<List<Review>>> GetPropertyReviewsAsync(string propertyId)
    {
        if (string.IsNullOrWhiteSpace(propertyId))
        {
            return new HttpResponseWrapper<List<Review>>(default!, true, new HttpResponseMessage(HttpStatusCode.BadRequest));
        }

        var reviewsUrl = BuildPropertyReviewsUrl(propertyId);
        var response = await _backEndService.GetAsync<object>(reviewsUrl);
        if (response.Error || response.Response == null)
        {
            return new HttpResponseWrapper<List<Review>>(default!, true, response.HttpResponseMessage);
        }

        List<AccommodationReviewDto> reviewDtos;
        try
        {
            reviewDtos = ParseReviews(response.Response);
        }
        catch (JsonSerializationException)
        {
            reviewDtos = [];
        }

        var mappedReviews = reviewDtos.Select(MapToReview).ToList();
        return new HttpResponseWrapper<List<Review>>(mappedReviews, false, response.HttpResponseMessage);
    }

    private string BuildPropertyDetailUrl(string propertyId, string currencyCode)
    {
        var normalizedPropertyId = Uri.EscapeDataString(propertyId.Trim());
        var normalizedCurrency = Uri.EscapeDataString(currencyCode.Trim().ToUpperInvariant());
        return _backendUrlProvider.Build($"/api/v1/inventarios/hospedajes/{normalizedPropertyId}/{normalizedCurrency}");
    }

    private string BuildPropertyReviewsUrl(string propertyId)
    {
        var normalizedPropertyId = Uri.EscapeDataString(propertyId.Trim());
        return _backendUrlProvider.Build($"/api/v1/reviews/hospedajes/{normalizedPropertyId}");
    }

    private static Property MapToProperty(AccommodationDetailDto dto, string currencyCode)
    {
        var rooms = dto.Rooms.Select(MapToRoom).ToList();
        var imageUrls = dto.Images
            .Select(image => image.Url?.Trim() ?? string.Empty)
            .Where(url => !string.IsNullOrWhiteSpace(url))
            .ToList();

        var pricePerNight = rooms.Count > 0
            ? rooms.Min(room => room.PricePerNight)
            : 0m;

        return new Property
        {
            Id = ComputeStablePositiveId(string.IsNullOrWhiteSpace(dto.Id) ? dto.ProviderId : dto.Id),
            ProviderId = string.IsNullOrWhiteSpace(dto.ProviderId) ? dto.Id : dto.ProviderId,
            Name = dto.Name ?? string.Empty,
            City = dto.City ?? string.Empty,
            Address = dto.Address ?? string.Empty,
            Description = dto.Description ?? string.Empty,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude,
            Rating = dto.Rating,
            ReviewCount = dto.Reviews,
            PricePerNight = pricePerNight,
            Rooms = rooms,
            Amenities = dto.Amenities.Select(MapToAmenity).ToList(),
            ImageUrls = imageUrls,
            ImageUrl = imageUrls.FirstOrDefault() ?? string.Empty,
            IsAvailable = rooms.Count > 0,
            Country = new Country
            {
                Code = dto.CountryCode ?? string.Empty,
                Name = dto.Country ?? string.Empty,
                CurrencyCode = currencyCode
            }
        };
    }

    private static Room MapToRoom(AccommodationDetailRoomDto roomDto)
    {
        var roomIdSource = roomDto.Id ?? string.Empty;
        var roomCode = roomDto.Code?.Trim() ?? string.Empty;
        var description = roomDto.Description?.Trim() ?? string.Empty;

        return new Room
        {
            Id = ComputeStablePositiveId(roomIdSource),
            ProviderId = roomIdSource,
            Name = string.IsNullOrWhiteSpace(roomCode) ? "Habitacion" : $"Habitacion {roomCode}",
            Description = description,
            MaxGuests = Math.Max(1, roomDto.Capacity),
            PricePerNight = roomDto.Price,
            ImageUrl = roomDto.ImageUrl ?? string.Empty,
            IsAvailable = true
        };
    }

    private static Amenity MapToAmenity(AccommodationDetailAmenityDto amenityDto)
    {
        return new Amenity
        {
            Id = ComputeStablePositiveId(amenityDto.Id ?? string.Empty),
            Name = amenityDto.Name ?? string.Empty,
            Icon = amenityDto.Icon ?? string.Empty
        };
    }

    private static Review MapToReview(AccommodationReviewDto reviewDto)
    {
        return new Review
        {
            AuthorName = reviewDto.UserName ?? string.Empty,
            Comment = reviewDto.Comment ?? string.Empty,
            Rating = reviewDto.Rating,
            Date = DateTime.Now
        };
    }

    private static List<AccommodationReviewDto> ParseReviews(object responsePayload)
    {
        var token = responsePayload switch
        {
            JToken jToken => jToken,
            _ => JToken.FromObject(responsePayload)
        };

        if (token.Type == JTokenType.Array)
        {
            return token.ToObject<List<AccommodationReviewDto>>() ?? [];
        }

        if (token.Type == JTokenType.Object)
        {
            var objectToken = (JObject)token;
            var candidate = objectToken["data"] ?? objectToken["reviews"] ?? objectToken["items"];
            if (candidate is JArray arrayToken)
            {
                return arrayToken.ToObject<List<AccommodationReviewDto>>() ?? [];
            }
        }

        throw new JsonSerializationException("Formato de respuesta de comentarios no soportado.");
    }

    private static int ComputeStablePositiveId(string source)
    {
        if (string.IsNullOrWhiteSpace(source))
        {
            return 0;
        }

        return source.GetHashCode() & int.MaxValue;
    }
}
