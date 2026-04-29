using App.Models;
using App.Services.Interfaces;

namespace TravelHub.Tests.Mocks;

public class MockUserSessionService : IUserSessionService
{
    public bool SetSessionCalled { get; private set; }
    public AuthLoginResponse? LastSessionData { get; private set; }
    public bool ClearSessionCalled { get; private set; }

    public bool IsAuthenticated => !string.IsNullOrEmpty(Token);
    public string Token { get; private set; } = string.Empty;
    public AuthUserDto? User { get; private set; }

    public Task SetSession(AuthLoginResponse sessionData)
    {
        SetSessionCalled = true;
        LastSessionData = sessionData;
        Token = sessionData.Token;
        User = sessionData.User;
        return Task.CompletedTask;
    }

    public Task ClearSession()
    {
        ClearSessionCalled = true;
        Token = string.Empty;
        User = null;
        return Task.CompletedTask;
    }
}
