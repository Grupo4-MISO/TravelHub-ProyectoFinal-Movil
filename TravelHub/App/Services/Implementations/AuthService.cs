using System.Net;
using App.Models;
using App.Providers.Interfaces;
using App.Responses;
using App.Services.Interfaces;

namespace App.Services.Implementations;

public class AuthService : IAuthService
{
    private const string LoginEndpoint = "/api/v1/auth/login";

    private readonly IBackEndService _backEndService;
    private readonly IBackendUrlProvider _backendUrlProvider;

    public AuthService(IBackEndService backEndService, IBackendUrlProvider backendUrlProvider)
    {
        _backEndService = backEndService ?? throw new ArgumentNullException(nameof(backEndService));
        _backendUrlProvider = backendUrlProvider ?? throw new ArgumentNullException(nameof(backendUrlProvider));
    }

    public Task<HttpResponseWrapper<AuthLoginResponse>> LoginAsync(AuthLoginRequest request)
    {
        if (request == null ||
            string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Password))
        {
            return Task.FromResult(
                new HttpResponseWrapper<AuthLoginResponse>(
                    default!,
                    true,
                    new HttpResponseMessage(HttpStatusCode.BadRequest)));
        }

        var payload = new AuthLoginRequest
        {
            Email = request.Email.Trim(),
            Password = request.Password
        };

        var url = _backendUrlProvider.Build(LoginEndpoint);
        return _backEndService.PostAsync<AuthLoginRequest, AuthLoginResponse>(url, payload);
    }
}
