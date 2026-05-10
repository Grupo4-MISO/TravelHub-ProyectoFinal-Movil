using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Support.UI;
using TravelHub.E2E.Constants;
using TravelHub.E2E.Pages;
using TravelHub.E2E.Utilities;
using Xunit;
namespace TravelHub.E2E.Tests;

[Collection("Appium")]
public abstract class BaseTest : IDisposable
{
    protected readonly AndroidDriver Driver;
    protected readonly AppiumFixture Fixture;
    protected readonly HomePage Home;
    protected readonly AccountLoginPage Login;
    protected readonly AccountRegisterPage Register;
    protected readonly AccountPage Account;
    protected readonly SearchResultsPage SearchResults;
    protected readonly PropertyDetailPage PropertyDetail;
    protected readonly RoomSelectionPage RoomSelection;
    protected readonly TravelerDataPage TravelerData;
    protected readonly BookingSummaryPage BookingSummary;
    protected readonly BookingConfirmedPage BookingConfirmed;
    protected readonly PaymentPage Payment;
    protected readonly SettingsPage Settings;
    protected readonly ActiveBookingsPage ActiveBookings;

    private readonly string _testName;

    protected BaseTest(AppiumFixture fixture)
    {
        Fixture = fixture;
        _testName = GetType().Name;

        if (!fixture.IsEnvironmentReady)
            throw new InvalidOperationException(fixture.SkipReason ?? "Entorno de Appium/Android no disponible.");

        Driver = fixture.Driver ?? throw new InvalidOperationException("Driver de Appium no inicializado.");

        Home = new HomePage(Driver);
        Login = new AccountLoginPage(Driver);
        Register = new AccountRegisterPage(Driver);
        Account = new AccountPage(Driver);
        SearchResults = new SearchResultsPage(Driver);
        PropertyDetail = new PropertyDetailPage(Driver);
        RoomSelection = new RoomSelectionPage(Driver);
        TravelerData = new TravelerDataPage(Driver);
        BookingSummary = new BookingSummaryPage(Driver);
        BookingConfirmed = new BookingConfirmedPage(Driver);
        Payment = new PaymentPage(Driver);
        Settings = new SettingsPage(Driver);
        ActiveBookings = new ActiveBookingsPage(Driver);
    }

    protected void NavigateToTab(string tabName)
    {
        var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(15));
        var tabLocator = By.XPath($"//*[@text='{tabName}']");
        wait.Until(d =>
        {
            try
            {
                var el = d.FindElement(tabLocator);
                el.Click();
                return true;
            }
            catch (StaleElementReferenceException)
            {
                return false;
            }
        });
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

    public void Dispose()
    {
        try
        {
            var _ = Driver?.PageSource;
        }
        catch
        {
            ScreenshotCapture.Capture(Driver, _testName);
        }

        try
        {
            if (Driver == null) return;
            var tabLocator = By.XPath($"//*[@text='{TabNames.Settings}']");
            try
            {
                Driver.FindElement(tabLocator).Click();
            }
            catch
            {
                Driver.PressKeyCode(4);
                Thread.Sleep(500);
                Driver.FindElement(tabLocator).Click();
            }
        }
        catch { }
    }
}
