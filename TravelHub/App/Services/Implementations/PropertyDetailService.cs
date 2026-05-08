using App.DTOs;
using App.Models;
using App.Providers.Interfaces;
using App.Responses;
using App.Services.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;

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

    public async Task<HttpResponseWrapper<AccommodationDetailDto>> GetPropertyDetailAsync(string propertyId, string currencyCode)
    {
        if (string.IsNullOrWhiteSpace(propertyId))
        {
            return new HttpResponseWrapper<AccommodationDetailDto>(default!, true, new HttpResponseMessage(HttpStatusCode.BadRequest));
        }

        var normalizedCurrency = string.IsNullOrWhiteSpace(currencyCode) ? "COP" : currencyCode.Trim().ToUpperInvariant();
        var detailUrl = BuildPropertyDetailUrl(propertyId, normalizedCurrency);

        var response = await _backEndService.GetAsync<AccommodationDetailDto>(detailUrl);
        if (response.Error || response.Response == null)
        {
            return new HttpResponseWrapper<AccommodationDetailDto>(default!, true, response.HttpResponseMessage);
        }

        return new HttpResponseWrapper<AccommodationDetailDto>(response.Response, false, response.HttpResponseMessage);
    }
    public async Task<HttpResponseWrapper<AccommodationInfoDto>> GetPropertyDetailByRoomIdAsync(string roomId, string currencyCode)
    {
        if (string.IsNullOrWhiteSpace(roomId))
        {
            return new HttpResponseWrapper<AccommodationInfoDto>(default!, true, new HttpResponseMessage(HttpStatusCode.BadRequest));
        }

        var normalizedCurrency = string.IsNullOrWhiteSpace(currencyCode) ? "COP" : currencyCode.Trim().ToUpperInvariant();
        var detailUrl = BuildPropertyDetailByRoomIdUrl(roomId, normalizedCurrency);

        var response = await _backEndService.GetAsync<AccommodationInfoDto>(detailUrl);
        if (response.Error || response.Response == null)
        {
            return new HttpResponseWrapper<AccommodationInfoDto>(default!, true, response.HttpResponseMessage);
        }

        return new HttpResponseWrapper<AccommodationInfoDto>(response.Response, false, response.HttpResponseMessage);
    }
    public async Task<HttpResponseWrapper<List<AccommodationReviewDto>>> GetPropertyReviewsAsync(string propertyId)
    {
        if (string.IsNullOrWhiteSpace(propertyId))
        {
            return new HttpResponseWrapper<List<AccommodationReviewDto>>(default!, true, new HttpResponseMessage(HttpStatusCode.BadRequest));
        }

        var reviewsUrl = BuildPropertyReviewsUrl(propertyId);
        var response = await _backEndService.GetAsync<object>(reviewsUrl);
        if (response.Error || response.Response == null)
        {
            return new HttpResponseWrapper<List<AccommodationReviewDto>>(default!, true, response.HttpResponseMessage);
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
        return new HttpResponseWrapper<List<AccommodationReviewDto>>(reviewDtos, false, response.HttpResponseMessage);
    }

    private string BuildPropertyDetailUrl(string propertyId, string currencyCode)
    {
        var normalizedPropertyId = Uri.EscapeDataString(propertyId.Trim());
        var normalizedCurrency = Uri.EscapeDataString(currencyCode.Trim().ToUpperInvariant());
        return _backendUrlProvider.Build($"/api/v1/inventarios/hospedajes/{normalizedPropertyId}/{normalizedCurrency}");
    }

    private string BuildPropertyDetailByRoomIdUrl(string roomId, string currencyCode)
    {
        var normalizedRoomId = Uri.EscapeDataString(roomId.Trim());
        var normalizedCurrency = Uri.EscapeDataString(currencyCode.Trim().ToUpperInvariant());
        return _backendUrlProvider.Build($"/api/v1/inventarios/habitacion/{normalizedRoomId}/{normalizedCurrency}");
    }
    private string BuildPropertyReviewsUrl(string propertyId)
    {
        var normalizedPropertyId = Uri.EscapeDataString(propertyId.Trim());
        return _backendUrlProvider.Build($"/api/v1/reviews/hospedajes/{normalizedPropertyId}");
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
