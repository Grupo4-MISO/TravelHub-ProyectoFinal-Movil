using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Support.UI;
using static TravelHub.E2E.Constants.AutomationIds;

namespace TravelHub.E2E.Pages;

public class AccountPage : BasePage
{
    private const int AccountPageWaitSeconds = 30;

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
    {
        try
        {
            var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(15));
            return wait.Until(d =>
                FindByAutomationId(d, Account.LogoutButton, requireEnabled: false)) != null;
        }
        catch
        {
            return false;
        }
    }

    public bool IsLoginButtonDisplayed()
    {
        return IsDisplayed(Account.LoginButton);
    }
}
