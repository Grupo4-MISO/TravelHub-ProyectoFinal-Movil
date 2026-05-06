using App.Models;
using App.Services.Interfaces;
using Microsoft.Maui.Storage;
using OneSignalSDK.DotNet;

namespace App.Services.Implementations;

public class UserSessionService : IUserSessionService
{
    private const string TokenKey = "AuthToken";
    private const string UserIdKey = "AuthUserId";
    private const string UserNameKey = "AuthUserName";
    private const string UserRoleKey = "AuthUserRole";

    private readonly IBackEndService _backEndService;

    public string Token { get; private set; } = string.Empty;
    public AuthUserDto User { get; private set; } = new();

    public bool IsAuthenticated => !string.IsNullOrWhiteSpace(Token) && !string.IsNullOrWhiteSpace(User.Id);

    public UserSessionService(IBackEndService backEndService)
    {
        _backEndService = backEndService ?? throw new ArgumentNullException(nameof(backEndService));
        LoadFromPreferences();
    }

    public async Task SetSession(AuthLoginResponse authResponse)
    {
        if (authResponse == null || string.IsNullOrWhiteSpace(authResponse.Token) || authResponse.User == null)
        {
            return;
        }

        Token = authResponse.Token.Trim();
        User = new AuthUserDto
        {
            Id = authResponse.User.Id?.Trim() ?? string.Empty,
            Username = authResponse.User.Username?.Trim() ?? string.Empty,
            Role = authResponse.User.Role?.Trim() ?? string.Empty
        };

        Preferences.Default.Set(TokenKey, Token);
        Preferences.Default.Set(UserIdKey, User.Id);
        Preferences.Default.Set(UserNameKey, User.Username);
        Preferences.Default.Set(UserRoleKey, User.Role);
        await _backEndService.SetAuthorization(Token);
        RegisterUserForNotifications(User.Id);
    }

    public async Task ClearSession()
    {
        UnregisterUserFromNotifications();
        Token = string.Empty;
        User = new AuthUserDto();

        Preferences.Default.Remove(TokenKey);
        Preferences.Default.Remove(UserIdKey);
        Preferences.Default.Remove(UserNameKey);
        Preferences.Default.Remove(UserRoleKey);
        await _backEndService.SetAuthorization(null);
    }

    private void LoadFromPreferences()
    {
        Token = Preferences.Default.Get(TokenKey, string.Empty);
        User = new AuthUserDto
        {
            Id = Preferences.Default.Get(UserIdKey, string.Empty),
            Username = Preferences.Default.Get(UserNameKey, string.Empty),
            Role = Preferences.Default.Get(UserRoleKey, string.Empty)
        };

        if (!string.IsNullOrWhiteSpace(Token))
        {
            _backEndService.SetAuthorization(Token);
        }
    }

    private static void RegisterUserForNotifications(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return;
        }

        OneSignal.Logout();
        OneSignal.Login(userId);
    }

    private static void UnregisterUserFromNotifications()
    {
        OneSignal.Logout();
    }
}
