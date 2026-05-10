using System.Collections.Generic;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Android;
using static TravelHub.E2E.Constants.AutomationIds;

namespace TravelHub.E2E.Pages;

public class AccountPage : BasePage
{
    public AccountPage(AndroidDriver driver, int explicitWaitSeconds = 20)
        : base(driver, explicitWaitSeconds) { }

    public void WaitForPageLoad()
    {
        try
        {
            Wait.Until(d =>
            {
                DismissAlertIfPresent();
                return IsDisplayed(Account.LoginButton)
                    || IsDisplayed(Account.UserName)
                    || IsDisplayed(Account.LogoutButton);
            });
        }
        catch (WebDriverTimeoutException)
        {
            // If the page didn't report loaded, try a gentle tap on center of screen to force focus/navigation and retry once.
            try
            {
                var size = Driver.Manage().Window.Size;
                var x = size.Width / 2;
                var y = size.Height / 2;
                Driver.ExecuteScript("mobile: clickGesture", new Dictionary<string, object>
                {
                    ["x"] = x,
                    ["y"] = y
                });

                Thread.Sleep(500);

                Wait.Until(d =>
                {
                    DismissAlertIfPresent();
                    return IsDisplayed(Account.LoginButton)
                        || IsDisplayed(Account.UserName)
                        || IsDisplayed(Account.LogoutButton);
                });
            }
            catch (WebDriverTimeoutException)
            {
                // propagate original timeout if retry also fails
                throw;
            }
        }
    }

    public void TapLogin()
        => Tap(Account.LoginButton);

    public void EnterEmail(string email)
        => EnterText(Account.EmailEntry, email);

    public void EnterPassword(string password)
        => EnterText(Account.PasswordEntry, password);

    public void TapRegisterLink()
        => Tap(Account.RegisterLink);

    public string GetUserName()
        => GetText(Account.UserName);

    public string GetUserEmail()
        => GetText(Account.UserEmail);

    public void TapLogout()
        => Tap(Account.LogoutButton);

    public bool IsLoggedIn()
        => IsDisplayed(Account.UserName);

    public bool IsLoginButtonDisplayed()
        => IsDisplayed(Account.LoginButton);
}
