using App.DTOs;
using App.Models;
using App.Responses;

namespace App.Services.Interfaces;

public interface IAuthService
{
    Task<HttpResponseWrapper<AuthLoginResponse>> LoginAsync(AuthLoginRequest request);
    Task<HttpResponseWrapper<object>> RegisterAsync(TravelerCreateRequestDTO request);
}
