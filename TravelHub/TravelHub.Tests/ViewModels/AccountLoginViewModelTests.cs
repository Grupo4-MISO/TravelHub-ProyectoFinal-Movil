using App.Models;
using App.Responses;
using App.Services.Interfaces;
using App.ViewModels;
using TravelHub.Tests.Mocks;
using Xunit;

namespace TravelHub.Tests.ViewModels;

public class AccountLoginViewModelTests
{
    private readonly MockAuthService _mockAuthService;
    private readonly MockUserSessionService _mockUserSessionService;
    private readonly MockNavigationService _mockNavigationService;
    private readonly AccountLoginViewModel _viewModel;

    public AccountLoginViewModelTests()
    {
        _mockAuthService = new MockAuthService();
        _mockUserSessionService = new MockUserSessionService();
        _mockNavigationService = new MockNavigationService();
        _viewModel = new AccountLoginViewModel(_mockAuthService, _mockUserSessionService, _mockNavigationService);
    }

    [Fact]
    public void Constructor_SetsTitleCorrectly()
    {
        Assert.Equal("Iniciar Sesion", _viewModel.Title);
    }

    [Fact]
    public void Constructor_Throws_WhenAuthServiceNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new AccountLoginViewModel(null!, _mockUserSessionService, _mockNavigationService));
    }

    [Fact]
    public void Constructor_Throws_WhenUserSessionServiceNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new AccountLoginViewModel(_mockAuthService, null!, _mockNavigationService));
    }

    [Fact]
    public void Constructor_Throws_WhenNavigationServiceNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new AccountLoginViewModel(_mockAuthService, _mockUserSessionService, null!));
    }

    [Fact]
    public async Task LoginCommand_EmptyEmail_ShowsErrorAlert()
    {
        _viewModel.Email = "";
        _viewModel.Password = "password123";

        _viewModel.LoginCommand.Execute(null);

        await Task.Delay(100);

        Assert.Equal("Error", _mockNavigationService.LastAlertTitle);
        Assert.Contains("email", _mockNavigationService.LastAlertMessage?.ToLower());
        Assert.Equal(0, _mockAuthService.LoginCallCount);
    }

    [Fact]
    public async Task LoginCommand_EmptyPassword_ShowsErrorAlert()
    {
        _viewModel.Email = "test@email.com";
        _viewModel.Password = "";

        _viewModel.LoginCommand.Execute(null);

        await Task.Delay(100);

        Assert.Equal("Error", _mockNavigationService.LastAlertTitle);
        Assert.Contains("contrasena", _mockNavigationService.LastAlertMessage?.ToLower());
        Assert.Equal(0, _mockAuthService.LoginCallCount);
    }

    [Fact]
    public async Task LoginCommand_ValidCredentials_CallsLoginAsync()
    {
        _viewModel.Email = "test@email.com";
        _viewModel.Password = "password123";

        _mockAuthService.LoginResponse = new AuthLoginResponse
        {
            Token = "fake-jwt-token",
            User = new AuthUserDto { Id = "1", Username = "test", Role = "User" }
        };

        _viewModel.LoginCommand.Execute(null);

        await Task.Delay(200);

        Assert.Equal(1, _mockAuthService.LoginCallCount);
        Assert.Equal("test@email.com", _mockAuthService.LastLoginRequest?.Email);
        Assert.Equal("password123", _mockAuthService.LastLoginRequest?.Password);
    }

    [Fact]
    public async Task LoginCommand_ApiReturnsToken_CallsSetSession()
    {
        _viewModel.Email = "test@email.com";
        _viewModel.Password = "password123";

        _mockAuthService.LoginResponse = new AuthLoginResponse
        {
            Token = "fake-jwt-token",
            User = new AuthUserDto { Id = "1", Username = "test", Role = "User" }
        };

        _viewModel.LoginCommand.Execute(null);

        await Task.Delay(200);

        Assert.True(_mockUserSessionService.SetSessionCalled);
        Assert.NotNull(_mockUserSessionService.LastSessionData);
        Assert.Equal("fake-jwt-token", _mockUserSessionService.LastSessionData?.Token);
    }

    [Fact]
    public async Task LoginCommand_ApiReturnsToken_NavigatesToAccount()
    {
        _viewModel.Email = "test@email.com";
        _viewModel.Password = "password123";

        _mockAuthService.LoginResponse = new AuthLoginResponse
        {
            Token = "fake-jwt-token",
            User = new AuthUserDto { Id = "1", Username = "test", Role = "User" }
        };

        _viewModel.LoginCommand.Execute(null);

        await Task.Delay(200);

        Assert.Equal("//account", _mockNavigationService.LastNavigationUri);
    }

    [Fact]
    public async Task LoginCommand_ApiReturnsToken_ClearsPassword()
    {
        _viewModel.Email = "test@email.com";
        _viewModel.Password = "password123";

        _mockAuthService.LoginResponse = new AuthLoginResponse
        {
            Token = "fake-jwt-token",
            User = new AuthUserDto { Id = "1", Username = "test", Role = "User" }
        };

        _viewModel.LoginCommand.Execute(null);

        await Task.Delay(200);

        Assert.Equal(string.Empty, _viewModel.Password);
    }

    [Fact]
    public async Task LoginCommand_ApiReturnsError_ShowsErrorAlert()
    {
        _viewModel.Email = "test@email.com";
        _viewModel.Password = "wrongpassword";

        _mockAuthService.ShouldReturnError = true;
        _mockAuthService.ErrorMessage = "Invalid credentials";

        _viewModel.LoginCommand.Execute(null);

        await Task.Delay(200);

        Assert.Equal("Error", _mockNavigationService.LastAlertTitle);
        Assert.NotNull(_mockNavigationService.LastAlertMessage);
    }

    [Fact]
    public async Task LoginCommand_WithReturnTo_NavigatesBack()
    {
        _viewModel.Email = "test@email.com";
        _viewModel.Password = "password123";

        _mockAuthService.LoginResponse = new AuthLoginResponse
        {
            Token = "fake-jwt-token",
            User = new AuthUserDto { Id = "1", Username = "test", Role = "User" }
        };

        var query = new Dictionary<string, object> { { "returnTo", "TravelerDataPage" } };
        _viewModel.ApplyQueryAttributes(query);

        _viewModel.LoginCommand.Execute(null);

        await Task.Delay(200);

        Assert.Equal("..", _mockNavigationService.LastNavigationUri);
    }

    [Fact]
    public void ApplyQueryAttributes_SetsReturnTo()
    {
        var query = new Dictionary<string, object> { { "returnTo", "TravelerDataPage" } };

        _viewModel.ApplyQueryAttributes(query);

        Assert.Equal("TravelerDataPage", GetReturnTo());
    }

    private string? GetReturnTo()
    {
        var field = typeof(AccountLoginViewModel).GetField("_returnTo",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        return field?.GetValue(_viewModel) as string;
    }
}
