using App.Models;

namespace App.Services.Interfaces;

public interface IUserSessionService
{
    bool IsAuthenticated { get; }
    string Token { get; }
    AuthUserDto User { get; }
    Task SetSession(AuthLoginResponse authResponse);
    Task ClearSession();
}
