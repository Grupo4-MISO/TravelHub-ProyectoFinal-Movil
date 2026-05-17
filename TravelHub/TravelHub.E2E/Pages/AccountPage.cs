using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Appium;
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
        => Driver.FindElements(MobileBy.AccessibilityId(Account.LogoutButton))
            .Any(e => e.Displayed);

    public bool IsLoginButtonDisplayed()
        => Driver.FindElements(MobileBy.AccessibilityId(Account.LoginButton))
            .Any(e => e.Displayed);
}
