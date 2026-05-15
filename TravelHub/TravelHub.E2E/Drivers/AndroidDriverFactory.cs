using System.Net.Http;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;
using TravelHub.E2E.Configuration;

namespace TravelHub.E2E.Drivers;

public static class AndroidDriverFactory
{
    public static AndroidDriver Create(AppiumSettings settings)
    {
        // Quick connectivity check to Appium server to fail fast if unavailable
        try
        {
            using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };
            var statusUrl = new Uri(new Uri(settings.ServerUrl), "/status").ToString();
            var response = client.GetAsync(statusUrl).GetAwaiter().GetResult();
            if (!response.IsSuccessStatusCode)
                throw new WebDriverException($"Appium server returned {response.StatusCode}");
        }
        catch (Exception ex)
        {
            throw new WebDriverException($"No connection to Appium server at {settings.ServerUrl}: {ex.Message}");
        }

        var options = new AppiumOptions();

        options.PlatformName = "Android";
        options.AutomationName = "UiAutomator2";
        options.DeviceName = "Android";

        if (!string.IsNullOrEmpty(settings.AppPath))
            options.App = settings.AppPath;

        options.AddAdditionalAppiumOption(
            "appPackage",
            "travelhubg4.app");

        options.AddAdditionalAppiumOption(
            "appWaitActivity",
            settings.AppWaitActivity);

        options.AddAdditionalAppiumOption(
            "appWaitForLaunch",
            true);

        options.AddAdditionalAppiumOption(
            "appWaitDuration",
            90000);

        options.AddAdditionalAppiumOption(
            "adbExecTimeout",
            120000);

        options.AddAdditionalAppiumOption(
            "androidInstallTimeout",
            120000);

        options.AddAdditionalAppiumOption(
            "autoGrantPermissions",
            true);

        options.AddAdditionalAppiumOption(
            "noReset",
            false);

        options.AddAdditionalAppiumOption(
            "skipDeviceInitialization",
            false);

        options.AddAdditionalAppiumOption(
            "uiautomator2ServerInstallTimeout",
            60000);

        options.AddAdditionalAppiumOption(
            "uiautomator2ServerLaunchTimeout",
            60000);

        options.AddAdditionalAppiumOption(
            "disableWindowAnimation",
            true);

        options.AddAdditionalAppiumOption(
            "shouldTerminateApp",
            true);

        var driver = new AndroidDriver(
            new Uri(settings.ServerUrl),
            options);

        driver.Manage().Timeouts().ImplicitWait =
            TimeSpan.FromSeconds(settings.ImplicitWaitSeconds);

        // If fixture requests resetting app between tests, set desired capability to not reuse app state and leave driver running
        if (settings.ResetAppBetweenTests)
        {
            // 'shouldTerminateApp' capability already set; ensure 'noReset' is false to force reinstall/clean start when needed
            options.AddAdditionalAppiumOption("noReset", false);
        }
        return driver;
    }
}