using App.Models;
using App.Services.Interfaces;

namespace TravelHub.Tests.Mocks;

public class MockUserSessionService : IUserSessionService
{
    public bool SetSessionCalled { get; private set; }
    public AuthLoginResponse? LastSessionData { get; private set; }

    public Task SetSession(AuthLoginResponse sessionData)
    {
        SetSessionCalled = true;
        LastSessionData = sessionData;
        return Task.CompletedTask;
    }
}
