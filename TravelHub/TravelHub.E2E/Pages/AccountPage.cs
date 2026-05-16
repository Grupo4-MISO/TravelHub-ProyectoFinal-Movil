using OpenQA.Selenium.Appium.Android;
using static TravelHub.E2E.Constants.AutomationIds;

namespace TravelHub.E2E.Pages;

public class AccountPage : BasePage
{
    private const int AccountPageWaitSeconds = 120;

    public AccountPage(AndroidDriver driver, int explicitWaitSeconds = AccountPageWaitSeconds)
        : base(driver, explicitWaitSeconds) { }

    public void WaitForPageLoad()
        => WaitForPageLoad(Account.Page);

    public void TapLogin()
        => Tap(Account.LoginButton);

    public void EnterEmail(string email)
        => EnterText(Account.EmailEntry, email);

    public void EnterPassword(string password)
        => EnterText(Account.PasswordEntry, password);

    public void TapRegisterLink()
        => WaitForElement(Account.RegisterLink).Click();

    public string GetUserName()
        => GetText(Account.UserName);

    public string GetUserEmail()
        => GetText(Account.UserEmail);

    public void TapLogout()
        => Tap(Account.LogoutButton);

    public bool IsLoggedIn()
        => IsDisplayed(Account.LogoutButton);

    public bool IsLoginButtonDisplayed()
        => IsDisplayed(Account.LoginButton);
}
