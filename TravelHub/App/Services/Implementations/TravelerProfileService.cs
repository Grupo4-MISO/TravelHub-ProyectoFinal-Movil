using System.Net;
using App.Models;
using App.Providers.Interfaces;
using App.Responses;
using App.Services.Interfaces;

namespace App.Services.Implementations;

public class TravelerProfileService : ITravelerProfileService
{
    private readonly IBackEndService _backEndService;
    private readonly IBackendUrlProvider _backendUrlProvider;
    private readonly IUserSessionService _userSessionService;

    public TravelerProfileService(
        IBackEndService backEndService,
        IBackendUrlProvider backendUrlProvider,
        IUserSessionService userSessionService)
    {
        _backEndService = backEndService ?? throw new ArgumentNullException(nameof(backEndService));
        _backendUrlProvider = backendUrlProvider ?? throw new ArgumentNullException(nameof(backendUrlProvider));
        _userSessionService = userSessionService ?? throw new ArgumentNullException(nameof(userSessionService));
    }

    public Task<HttpResponseWrapper<TravelerProfileResponse>> GetTravelerByUserIdAsync(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Task.FromResult(
                new HttpResponseWrapper<TravelerProfileResponse>(
                    default!,
                    true,
                    new HttpResponseMessage(HttpStatusCode.BadRequest)));
        }

        _backEndService.SetAuthorization(_userSessionService.Token);

        var encodedId = Uri.EscapeDataString(userId.Trim());
        var url = _backendUrlProvider.Build($"/api/v1/Travelers/users/{encodedId}");
        return _backEndService.GetAsync<TravelerProfileResponse>(url);
    }

    public Task<HttpResponseWrapper<object>> UpdateTravelerAsync(string travelerId, TravelerUpdateRequest request)
    {
        if (string.IsNullOrWhiteSpace(travelerId) || request == null)
        {
            return Task.FromResult(
                new HttpResponseWrapper<object>(
                    default!,
                    true,
                    new HttpResponseMessage(HttpStatusCode.BadRequest)));
        }

        _backEndService.SetAuthorization(_userSessionService.Token);

        var encodedId = Uri.EscapeDataString(travelerId.Trim());
        var url = _backendUrlProvider.Build($"/api/v1/Travelers/{encodedId}");
        return _backEndService.PutAsync(url, request);
    }

    public Task<HttpResponseWrapper<object>> CreateTravelerAsync(TravelerCreateDTO request)
    {
        _backEndService.SetAuthorization(_userSessionService.Token);

        var url = _backendUrlProvider.Build($"/api/v1/Travelers");
        return _backEndService.PostAsync(url, request);
    }
}
