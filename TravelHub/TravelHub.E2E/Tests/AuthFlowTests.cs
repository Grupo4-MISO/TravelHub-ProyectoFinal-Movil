using TravelHub.E2E.Constants;
using TravelHub.E2E.Utilities;
using OpenQA.Selenium.Appium.Android;
using Xunit;

namespace TravelHub.E2E.Tests;

public class AuthFlowTests : BaseTest
{
    public AuthFlowTests(AppiumFixture fixture) : base(fixture) { }

    private (string Email, string Password) CreateAccountAndReturnCredentials()
    {
        var email = TestDataFactory.GenerateEmail();
        var password = TestDataFactory.GeneratePassword();
        return (email, password);
    }

    private (string Email, string Password) RegisterAccountAndReturnCredentials()
    {
        var (email, password) = CreateAccountAndReturnCredentials();

        GoToAccount();
        Account.TapRegisterLink();
        Register.WaitForPageLoad();

        Register.RegisterUser(
            TestDataFactory.Traveler.FirstName,
            TestDataFactory.Traveler.LastName,
            TestDataFactory.GenerateDocument(),
            TestDataFactory.GeneratePhone(),
            email,
            password);

        DismissAlertIfPresent();

        return (email, password);
    }

    private void GoToAccount()
    {
        DismissAlertIfPresent();
        NavigateToTab(TabNames.MyAccount);
        Account.WaitForPageLoad();
    }

    [Fact]
    public void LoginPage_DisplaysLoginForm()
    {
        GoToAccount();
        var displayed = Account.IsLoginButtonDisplayed();
        Assert.True(displayed,
            "Login button should be visible on account page when not logged in");
    }

    [Fact]
    public void Login_NavigatesToAccountPage()
    {
        var credentials = RegisterAccountAndReturnCredentials();

        Account.EnterEmail(credentials.Email);
        Account.EnterPassword(credentials.Password);
        Account.TapLogin();

        Account.WaitForPageLoad();
        Assert.True(Account.IsLoggedIn(),
            "User should be logged in after valid credentials");
    }

    [Fact]
    public void Login_WithInvalidCredentials_ShowsErrorMessage()
    {
        GoToAccount();
        Account.EnterEmail(TestDataFactory.Users.InvalidUser.Email);
        Account.EnterPassword(TestDataFactory.Users.InvalidUser.Password);
        Account.TapLogin();

        var alertDismissed = DismissAlertIfPresent();
        Assert.True(alertDismissed, "Invalid credentials should trigger an error alert.");
        Account.WaitForPageLoad();
        Assert.False(Account.IsLoggedIn(),
            "User should remain logged out after invalid credentials");
    }

    [Fact]
    public void RegisterLink_NavigatesToRegisterPage()
    {
        GoToAccount();
        Account.TapRegisterLink();

        Register.WaitForPageLoad();
        Assert.NotNull(Driver.PageSource);
    }

    [Fact]
    public void Register_CreatesNewAccount()
    {
        RegisterAccountAndReturnCredentials();
        Account.WaitForPageLoad();
        Assert.True(Account.IsLoginButtonDisplayed(),
            "Account page should return to the login form after successful registration");
    }
}
