using App.DTOs;
using App.Models;
using App.Responses;

namespace App.Services.Interfaces;

public interface IPropertyDetailService
{
    Task<HttpResponseWrapper<AccommodationDetailDto>> GetPropertyDetailAsync(string propertyId, string currencyCode);
    Task<HttpResponseWrapper<AccommodationInfoDto>> GetPropertyDetailByRoomIdAsync(string roomId, string currencyCode);
    Task<HttpResponseWrapper<List<AccommodationReviewDto>>> GetPropertyReviewsAsync(string propertyId);
}
