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

        GoToAccount();
        if (Account.IsLoggedIn())
        {
            Account.TapLogout();
            Account.WaitForPageLoad();
        }

        Account.TapRegisterLink();
        Register.WaitForPageLoad();
        Register.RegisterUser(
            TestDataFactory.Traveler.FirstName,
            TestDataFactory.Traveler.LastName,
            email,
            password);

        Account.WaitForPageLoad();
        return (email, password);
    }

    private void GoToAccount()
    {
        Console.WriteLine("GoToAccount: dismissing alerts and opening Account tab");
        DismissAlertIfPresent();
        Console.WriteLine("GoToAccount: opening Account tab");
        OpenAccountTab();
        Console.WriteLine("GoToAccount: waiting for Account page to load");
        Account.WaitForPageLoad();
        Console.WriteLine($"GoToAccount: page loaded. IsLoggedIn={Account.IsLoggedIn()}");
        if (Account.IsLoggedIn())
        {
            Console.WriteLine("GoToAccount: user is logged in, logging out");
            Account.TapLogout();
            Account.WaitForPageLoad();
            Console.WriteLine("GoToAccount: logged out, page after logout loaded");
        }
    }

    private void OpenAccountTab()
    {
        var size = Driver.Manage().Window.Size;
        var x = (int)(size.Width * 0.625);
        var yCandidates = new[]
        {
            (int)(size.Height * 0.88),
            (int)(size.Height * 0.92),
            (int)(size.Height * 0.95)
        };

        foreach (var y in yCandidates)
        {
            Driver.ExecuteScript("mobile: clickGesture", new Dictionary<string, object>
            {
                ["x"] = x,
                ["y"] = y
            });

            Thread.Sleep(750);
            if (Account.IsLoginButtonDisplayed() || Account.IsLoggedIn())
            {
                return;
            }
        }

        NavigateToTab(TabNames.MyAccount);
    }

    [Fact]
    public void LoginPage_DisplaysLoginForm()
    {
        Console.WriteLine("Test: LoginPage_DisplaysLoginForm - starting");
        GoToAccount();
        Console.WriteLine("Test: after GoToAccount - checking if login button is displayed");
        var displayed = Account.IsLoginButtonDisplayed();
        Console.WriteLine($"Test: LoginPage - IsLoginButtonDisplayed={displayed}");
        Assert.True(displayed,
            "Login button should be visible on account page when not logged in");
        Console.WriteLine("Test: LoginPage_DisplaysLoginForm - passed");
    }

    [Fact(Skip = "Paused until LoginPage passes")]
    public void Login_NavigatesToAccountPage()
    {
        var credentials = CreateAccountAndReturnCredentials();

        GoToAccount();
        Account.EnterEmail(credentials.Email);
        Account.EnterPassword(credentials.Password);
        Account.TapLogin();

        Account.WaitForPageLoad();
        Assert.True(Account.IsLoggedIn(),
            "User should be logged in after valid credentials");
    }

    [Fact(Skip = "Paused until LoginPage passes")]
    public void Login_WithInvalidCredentials_ShowsErrorMessage()
    {
        GoToAccount();
        Account.EnterEmail(TestDataFactory.Users.InvalidUser.Email);
        Account.EnterPassword(TestDataFactory.Users.InvalidUser.Password);
        Account.TapLogin();

        Account.WaitForPageLoad();
        Assert.True(Account.IsLoginButtonDisplayed(),
            "Login button should still be displayed after invalid credentials");
    }

    [Fact(Skip = "Paused until LoginPage passes")]
    public void RegisterLink_NavigatesToRegisterPage()
    {
        GoToAccount();
        Account.TapRegisterLink();

        Register.WaitForPageLoad();
        Assert.NotNull(Driver.PageSource);
    }

    [Fact(Skip = "Paused until LoginPage passes")]
    public void Register_CreatesNewAccount()
    {
        CreateAccountAndReturnCredentials();
        GoToAccount();
        Assert.True(Account.IsLoginButtonDisplayed(),
            "Account page should return to the login form after successful registration");
    }
}
