using App.Models;
using App.Responses;

namespace App.Services.Interfaces;

public interface IPropertyDetailService
{
    Task<HttpResponseWrapper<Property>> GetPropertyDetailAsync(string propertyId, string currencyCode);
    Task<HttpResponseWrapper<List<Review>>> GetPropertyReviewsAsync(string propertyId);
}
