using App.Models;
using App.Responses;
using App.Services.Interfaces;
using App.ViewModels;
using Moq;
using Xunit;

namespace TravelHub.Tests.ViewModels;

public class AccountLoginViewModelTests
{
    private readonly Mock<IAuthService> _authServiceMock;
    private readonly Mock<IUserSessionService> _userSessionServiceMock;
    private readonly Mock<INavigationService> _navigationServiceMock;
    private readonly AccountLoginViewModel _viewModel;

    public AccountLoginViewModelTests()
    {
        _authServiceMock = new Mock<IAuthService>();
        _userSessionServiceMock = new Mock<IUserSessionService>();
        _navigationServiceMock = new Mock<INavigationService>();
        _viewModel = new AccountLoginViewModel(_authServiceMock.Object, _userSessionServiceMock.Object, _navigationServiceMock.Object);
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
            new AccountLoginViewModel(null!, _userSessionServiceMock.Object, _navigationServiceMock.Object));
    }

    [Fact]
    public void Constructor_Throws_WhenUserSessionServiceNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new AccountLoginViewModel(_authServiceMock.Object, null!, _navigationServiceMock.Object));
    }

    [Fact]
    public void Constructor_Throws_WhenNavigationServiceNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new AccountLoginViewModel(_authServiceMock.Object, _userSessionServiceMock.Object, null!));
    }

    [Fact]
    public async Task LoginCommand_EmptyEmail_ShowsErrorAlert()
    {
        _viewModel.Email = "";
        _viewModel.Password = "password123";

        _viewModel.LoginCommand.Execute(null);

        await Task.Delay(200);

        _navigationServiceMock.Verify(n => n.DisplayAlertAsync("Error",
            It.Is<string>(msg => msg.ToLower().Contains("email")),
            "OK"), Times.Once);
        _authServiceMock.Verify(a => a.LoginAsync(It.IsAny<AuthLoginRequest>()), Times.Never);
    }

    [Fact]
    public async Task LoginCommand_EmptyPassword_ShowsErrorAlert()
    {
        _viewModel.Email = "test@email.com";
        _viewModel.Password = "";

        _viewModel.LoginCommand.Execute(null);

        await Task.Delay(200);

        _navigationServiceMock.Verify(n => n.DisplayAlertAsync("Error",
            It.Is<string>(msg => msg.ToLower().Contains("contrasena")),
            "OK"), Times.Once);
        _authServiceMock.Verify(a => a.LoginAsync(It.IsAny<AuthLoginRequest>()), Times.Never);
    }

    [Fact]
    public async Task LoginCommand_ValidCredentials_CallsLoginAsync()
    {
        var expectedRequest = new AuthLoginRequest
        {
            Email = "test@email.com",
            Password = "password123"
        };

        _viewModel.Email = expectedRequest.Email;
        _viewModel.Password = expectedRequest.Password;

        var response = new AuthLoginResponse
        {
            Token = "fake-jwt-token",
            User = new AuthUserDto { Id = "1", Username = "test", Role = "User" }
        };

        _authServiceMock.Setup(a => a.LoginAsync(It.Is<AuthLoginRequest>(r =>
                r.Email == expectedRequest.Email && r.Password == expectedRequest.Password)))
            .ReturnsAsync(new HttpResponseWrapper<AuthLoginResponse>(response, false, new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.OK)));

        _viewModel.LoginCommand.Execute(null);

        await Task.Delay(500);

        _authServiceMock.Verify(a => a.LoginAsync(It.Is<AuthLoginRequest>(r =>
            r.Email == expectedRequest.Email && r.Password == expectedRequest.Password)), Times.Once);
    }

    [Fact]
    public async Task LoginCommand_ApiReturnsToken_CallsSetSession()
    {
        _viewModel.Email = "test@email.com";
        _viewModel.Password = "password123";

        var response = new AuthLoginResponse
        {
            Token = "fake-jwt-token",
            User = new AuthUserDto { Id = "1", Username = "test", Role = "User" }
        };

        _authServiceMock.Setup(a => a.LoginAsync(It.IsAny<AuthLoginRequest>()))
            .ReturnsAsync(new HttpResponseWrapper<AuthLoginResponse>(response, false, new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.OK)));

        _viewModel.LoginCommand.Execute(null);

        await Task.Delay(500);

        _userSessionServiceMock.Verify(s => s.SetSession(It.Is<AuthLoginResponse>(r =>
            r.Token == response.Token)), Times.Once);
    }

    [Fact]
    public async Task LoginCommand_ApiReturnsToken_NavigatesToAccount()
    {
        _viewModel.Email = "test@email.com";
        _viewModel.Password = "password123";

        var response = new AuthLoginResponse
        {
            Token = "fake-jwt-token",
            User = new AuthUserDto { Id = "1", Username = "test", Role = "User" }
        };

        _authServiceMock.Setup(a => a.LoginAsync(It.IsAny<AuthLoginRequest>()))
            .ReturnsAsync(new HttpResponseWrapper<AuthLoginResponse>(response, false, new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.OK)));

        _viewModel.LoginCommand.Execute(null);

        await Task.Delay(500);

        _navigationServiceMock.Verify(n => n.GoToAsync("//account"), Times.Once);
    }

    [Fact]
    public async Task LoginCommand_ApiReturnsToken_ClearsPassword()
    {
        _viewModel.Email = "test@email.com";
        _viewModel.Password = "password123";

        var response = new AuthLoginResponse
        {
            Token = "fake-jwt-token",
            User = new AuthUserDto { Id = "1", Username = "test", Role = "User" }
        };

        _authServiceMock.Setup(a => a.LoginAsync(It.IsAny<AuthLoginRequest>()))
            .ReturnsAsync(new HttpResponseWrapper<AuthLoginResponse>(response, false, new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.OK)));

        _viewModel.LoginCommand.Execute(null);

        await Task.Delay(500);

        Assert.Equal(string.Empty, _viewModel.Password);
    }

    [Fact]
    public async Task LoginCommand_ApiReturnsError_ShowsErrorAlert()
    {
        _viewModel.Email = "test@email.com";
        _viewModel.Password = "wrongpassword";

        var errorResponse = new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest)
        {
            Content = new System.Net.Http.StringContent("Invalid credentials")
        };

        _authServiceMock.Setup(a => a.LoginAsync(It.IsAny<AuthLoginRequest>()))
            .ReturnsAsync(new HttpResponseWrapper<AuthLoginResponse>(default!, true, errorResponse));

        _viewModel.LoginCommand.Execute(null);

        await Task.Delay(500);

        _navigationServiceMock.Verify(n => n.DisplayAlertAsync("Error",
            It.IsAny<string>(), "OK"), Times.Once);
    }

    [Fact]
    public async Task LoginCommand_WithReturnTo_NavigatesBack()
    {
        _viewModel.Email = "test@email.com";
        _viewModel.Password = "password123";

        var response = new AuthLoginResponse
        {
            Token = "fake-jwt-token",
            User = new AuthUserDto { Id = "1", Username = "test", Role = "User" }
        };

        _authServiceMock.Setup(a => a.LoginAsync(It.IsAny<AuthLoginRequest>()))
            .ReturnsAsync(new HttpResponseWrapper<AuthLoginResponse>(response, false, new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.OK)));

        var query = new Dictionary<string, object> { { "returnTo", "TravelerDataPage" } };
        _viewModel.ApplyQueryAttributes(query);

        _viewModel.LoginCommand.Execute(null);

        await Task.Delay(500);

        _navigationServiceMock.Verify(n => n.GoToAsync(".."), Times.Once);
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
