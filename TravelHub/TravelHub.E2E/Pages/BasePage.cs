using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Support.UI;
using TravelHub.E2E.Constants;

namespace TravelHub.E2E.Pages;

public abstract class BasePage
{
    protected readonly AndroidDriver Driver;
    protected readonly WebDriverWait Wait;

    protected BasePage(AndroidDriver driver, int explicitWaitSeconds = 60)
    {
        Driver = driver;
        Wait = new WebDriverWait(driver, TimeSpan.FromSeconds(explicitWaitSeconds));
    }

    private By AutomationIdLocator(string id)
        => MobileBy.AccessibilityId(id);

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
            return Wait.Until(d => d.FindElement(AutomationIdLocator(automationId)));
        }
        catch (WebDriverTimeoutException)
        {
            var contentDescLocator = By.XPath($"//*[@content-desc='{automationId}']");
            try
            {
                return Wait.Until(d => d.FindElement(contentDescLocator));
            }
            catch (WebDriverTimeoutException)
            {
            }

            var resourceIdLocator = MobileBy.AndroidUIAutomator(
                $"new UiSelector().resourceIdMatches(\".*{automationId}.*\")");
            try
            {
                return Wait.Until(d => d.FindElement(resourceIdLocator));
            }
            catch (WebDriverTimeoutException)
            {
            }

            var scrolledLocator = MobileBy.AndroidUIAutomator(
                $"new UiScrollable(new UiSelector().scrollable(true)).setMaxSearchSwipes(3).scrollIntoView(new UiSelector().description(\"{automationId}\"))");
            try
            {
                return Wait.Until(d => d.FindElement(scrolledLocator));
            }
            catch (WebDriverTimeoutException)
            {
                // Fallback: search by visible text (useful when AutomationId maps to text)
                var textLocator = By.XPath($"//*[@text='{automationId}']");
                try
                {
                    return Wait.Until(d => d.FindElement(textLocator));
                }
                catch (WebDriverTimeoutException)
                {
                    // Save diagnostics for investigation: PageSource + screenshot
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
                    catch { }

                    throw;
                }
            }
        }
    }

    protected IReadOnlyCollection<IWebElement> WaitForElements(string automationId)
        => Wait.Until(d => d.FindElements(AutomationIdLocator(automationId)));

    protected void Tap(string automationId)
        => WaitForElement(automationId).Click();

    protected void EnterText(string automationId, string text)
    {
        var element = WaitForElement(automationId);
        try
        {
            // Ensure field is focused before clearing / typing to avoid suggestions overlay interfering
            element.Click();
        }
        catch { }
n        element.Clear();
        element.SendKeys(text);
n        // Hide keyboard if present to ensure subsequent taps land on the app UI, not the keyboard suggestions.
        try
        {
            // Appium AndroidDriver exposes HideKeyboard()
            Driver.HideKeyboard();
        }
        catch { }
n        // Small pause to allow UI to settle after hiding keyboard
        try { System.Threading.Thread.Sleep(200); } catch { }
    }

    protected string GetText(string automationId)
        => WaitForElement(automationId).Text;

    protected bool IsDisplayed(string automationId)
        => Driver.FindElements(AutomationIdLocator(automationId)).Count > 0;

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

        var shortWait = new WebDriverWait(Driver, TimeSpan.FromSeconds(8));
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
}
