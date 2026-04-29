using App.Models;
using App.Responses;
using App.Services.Interfaces;

namespace TravelHub.Tests.Mocks;

public class MockAuthService : IAuthService
{
    public bool ShouldReturnError { get; set; }
    public string? ErrorMessage { get; set; }
    public AuthLoginResponse? LoginResponse { get; set; }
    public int LoginCallCount { get; private set; }
    public AuthLoginRequest? LastLoginRequest { get; private set; }

    public Task<HttpResponseWrapper<AuthLoginResponse>> LoginAsync(AuthLoginRequest request)
    {
        LoginCallCount++;
        LastLoginRequest = request;

        if (ShouldReturnError)
        {
            var message = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.BadRequest)
            {
                Content = new System.Net.Http.StringContent(ErrorMessage ?? "Error")
            };
            return Task.FromResult(new HttpResponseWrapper<AuthLoginResponse>(default!, true, message));
        }

        var responseMessage = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.OK);
        return Task.FromResult(new HttpResponseWrapper<AuthLoginResponse>(LoginResponse!, false, responseMessage));
    }
}