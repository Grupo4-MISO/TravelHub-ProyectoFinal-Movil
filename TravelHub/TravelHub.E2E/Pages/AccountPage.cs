using System;
using System.Collections.Generic;
using System.IO;
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
        DismissAlertIfPresent();
        // Fast path: if the page root exists, click it to ensure the view has focus without tapping the tab bar.
        try
        {
            var roots = Driver.FindElements(MobileBy.AccessibilityId(Account.Page));
            if (roots != null && roots.Count > 0)
            {
                try { roots[0].Click(); } catch { }
                try { Thread.Sleep(300); } catch { }
            }
        }
        catch { }

        try
        {
            Wait.Until(d =>
            {
                DismissAlertIfPresent();
                return IsDisplayed(Account.LoginButton)
                    || IsDisplayed(Account.UserName)
                    || IsDisplayed(Account.LogoutButton);
            });
            return;
        }
        catch (WebDriverTimeoutException)
        {
            // Capture diagnostics to help troubleshoot visibility issues
            try
            {
                var ts = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
                var dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "TestResults");
                Directory.CreateDirectory(dir);
                var pageSourcePath = Path.Combine(dir, $"PageSource_AccountPage_{ts}.xml");
                File.WriteAllText(pageSourcePath, Driver.PageSource);
                var screenshot = ((ITakesScreenshot)Driver).GetScreenshot();
                var screenshotPath = Path.Combine(dir, $"Screenshot_AccountPage_{ts}.png");
                File.WriteAllBytes(screenshotPath, screenshot.AsByteArray);
            }
            catch { }
            // Retry by tapping a safe area above the tab bar to avoid re-selecting tabs (approx. 30% height)
            try
            {
                var size = Driver.Manage().Window.Size;
                var x = size.Width / 2;
                var y = (int)(size.Height * 0.35);
                Driver.ExecuteScript("mobile: clickGesture", new Dictionary<string, object>
                {
                    ["x"] = x,
                    ["y"] = y
                });
                try { Thread.Sleep(400); } catch { }
                Wait.Until(d =>
                {
                    DismissAlertIfPresent();
                    return IsDisplayed(Account.LoginButton)
                        || IsDisplayed(Account.UserName)
                        || IsDisplayed(Account.LogoutButton);
                });
                return;
            }
            catch (WebDriverTimeoutException)
            {
                // give up and propagate timeout for investigation
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
