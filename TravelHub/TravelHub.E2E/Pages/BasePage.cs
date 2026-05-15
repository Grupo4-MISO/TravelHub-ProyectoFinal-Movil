using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Support.UI;
using TravelHub.E2E.Constants;
using System.Linq;

namespace TravelHub.E2E.Pages;

public abstract class BasePage
{
    private const string AppPackage = "travelhubg4.app";
    protected readonly AndroidDriver Driver;
    protected readonly WebDriverWait Wait;

    protected BasePage(AndroidDriver driver, int explicitWaitSeconds = 15)
    {
        Driver = driver;
        Wait = new WebDriverWait(driver, TimeSpan.FromSeconds(explicitWaitSeconds));
    }

    private static IWebElement? FindByAutomationId(ISearchContext context, string automationId, bool requireEnabled)
    {
        var byAccessibilityId = context.FindElements(MobileBy.AccessibilityId(automationId))
            .FirstOrDefault(e => e.Displayed && (!requireEnabled || e.Enabled));
        if (byAccessibilityId != null)
        {
            return byAccessibilityId;
        }

        var byResourceId = context.FindElements(MobileBy.Id($"{AppPackage}:id/{automationId}"))
            .FirstOrDefault(e => e.Displayed && (!requireEnabled || e.Enabled));
        if (byResourceId != null)
        {
            return byResourceId;
        }

        return context.FindElements(MobileBy.Id(automationId))
            .FirstOrDefault(e => e.Displayed && (!requireEnabled || e.Enabled));
    }

    protected bool DismissAlertIfPresent()
    {
        try
        {
            Driver.SwitchTo().Alert().Accept();
            return true;
        }
        catch (NoAlertPresentException)
        {
            return false;
        }
    }

    protected IWebElement WaitForElement(string automationId)
    {
        DismissAlertIfPresent();

        try
        {
            // Wait for presence (not necessarily enabled) to reduce false negatives when element exists but is temporarily disabled
            var element = Wait.Until(d =>
            {
                return FindByAutomationId(d, automationId, requireEnabled: false);
            });
            if (element == null)
            {
                throw new WebDriverTimeoutException($"No se encontró elemento con AutomationId '{automationId}'.");
            }
            return element;
        }
        catch (WebDriverTimeoutException)
        {
            CaptureDiagnostics(automationId);
            throw;
        }
    }

    protected IReadOnlyCollection<IWebElement> WaitForElements(string automationId)
        => Wait.Until(d =>
        {
            var byAccessibilityId = d.FindElements(MobileBy.AccessibilityId(automationId));
            if (byAccessibilityId.Count > 0)
            {
                return byAccessibilityId;
            }

            var byResourceId = d.FindElements(MobileBy.Id($"{AppPackage}:id/{automationId}"));
            if (byResourceId.Count > 0)
            {
                return byResourceId;
            }

            return d.FindElements(MobileBy.Id(automationId));
        });

    protected void Tap(string automationId)
    {
        // Ensure element is enabled before attempting to click to avoid stale/disabled interaction failures
        Wait.Until(d => FindByAutomationId(d, automationId, requireEnabled: true) != null);
        WaitForElement(automationId).Click();
    }

    protected void EnterText(string automationId, string text)
    {
            // Wait until the element is present and enabled before interacting
            Wait.Until(d => FindByAutomationId(d, automationId, requireEnabled: true) != null);
            var element = WaitForElement(automationId);
            try
            {
                // Ensure field is focused before clearing / typing to avoid suggestions overlay interfering
                element.Click();
            }
            catch { }
            element.Clear();
            element.Clear();
            element.SendKeys(text);
            // Hide keyboard if present to ensure subsequent taps land on the app UI, not the keyboard suggestions.
            try
            {
                // Appium AndroidDriver exposes HideKeyboard()
                Driver.HideKeyboard();
            }
            catch { }
            // Small pause to allow UI to settle after hiding keyboard
        }

    protected string GetText(string automationId)
        => WaitForElement(automationId).Text;

    protected bool IsDisplayed(string automationId)
        => FindByAutomationId(Driver, automationId, requireEnabled: false) != null;

    protected bool IsEnabled(string automationId)
    {
        try
        {
            return WaitForElement(automationId).Enabled;
        }
        catch (WebDriverTimeoutException)
        {
            return false;
        }
    }

    protected void ScrollTo(string automationId)
    {
        var element = WaitForElement(automationId);
        ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].scrollIntoView(true);", element);
    }

    protected void NavigateToTab(string tabName)
    {
        DismissAlertIfPresent();
        var tabId = tabName switch
        {
            TabNames.Search => "Shell_SearchTab",
            TabNames.Bookings => "Shell_BookingsTab",
            TabNames.MyAccount => "Shell_AccountTab",
            TabNames.Settings => "Shell_SettingsTab",
            _ => tabName
        };

        var shortWait = new WebDriverWait(Driver, TimeSpan.FromSeconds(6));
        try
        {
            var tabByAccessibilityId = MobileBy.AccessibilityId(tabId);

            shortWait.Until(d =>
            {
                var tab = d.FindElements(tabByAccessibilityId)
                    .FirstOrDefault(e => e.Displayed && e.Enabled);
                if (tab == null)
                {
                    return false;
                }

                tab.Click();
                return true;
            });
            return;
        }
        catch (WebDriverTimeoutException)
        {
            var size = Driver.Manage().Window.Size;
            var tabIndex = tabName switch
            {
                TabNames.Search => 0,
                TabNames.Bookings => 1,
                TabNames.MyAccount => 2,
                TabNames.Settings => 3,
                _ => 0
            };

            var x = (int)(size.Width * (0.125 + (tabIndex * 0.25)));
            var y = (int)(size.Height * 0.95);
            Driver.ExecuteScript("mobile: clickGesture", new Dictionary<string, object>
            {
                ["x"] = x,
                ["y"] = y
            });
        }
    }

    protected void WaitForPageLoad(string pageAutomationId)
        => WaitForElement(pageAutomationId);

    private void CaptureDiagnostics(string automationId)
    {
        try
        {
            var ts = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "TestResults");
            Directory.CreateDirectory(dir);

            var pageSourcePath = Path.Combine(dir, $"PageSource_{automationId}_{ts}.xml");
            File.WriteAllText(pageSourcePath, Driver.PageSource);

            var screenshot = ((ITakesScreenshot)Driver).GetScreenshot();
            var screenshotPath = Path.Combine(dir, $"Screenshot_{automationId}_{ts}.png");
            File.WriteAllBytes(screenshotPath, screenshot.AsByteArray);
        }
        catch
        {
            // Diagnostics should never mask the original failure.
        }
    }
}
