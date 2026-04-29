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

    public Task<HttpResponseWrapper<TravelerProfileResponse>> GetTravelerByIdAsync(string travelerId)
    {
        if (string.IsNullOrWhiteSpace(travelerId))
        {
            return Task.FromResult(
                new HttpResponseWrapper<TravelerProfileResponse>(
                    default!,
                    true,
                    new HttpResponseMessage(HttpStatusCode.BadRequest)));
        }

        _backEndService.SetAuthorization(_userSessionService.Token);

        var encodedId = Uri.EscapeDataString(travelerId.Trim());
        var url = _backendUrlProvider.Build($"/api/v1/Travelers/{encodedId}");
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
}
