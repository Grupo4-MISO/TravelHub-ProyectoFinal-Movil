using App.Models;
using App.Responses;

namespace App.Services.Interfaces;

public interface ITravelerProfileService
{
    Task<HttpResponseWrapper<TravelerProfileResponse>> GetTravelerByUserIdAsync(string userId);
    Task<HttpResponseWrapper<object>> UpdateTravelerAsync(string travelerId, TravelerUpdateRequest request);
}
