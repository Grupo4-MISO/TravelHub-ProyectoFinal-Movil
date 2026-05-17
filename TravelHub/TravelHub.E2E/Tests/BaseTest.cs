using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Android;
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
 
        // Ensure a clean app state before each test to reduce inter-test flakiness
        if (!fixture.ResetAppState())
            throw new InvalidOperationException("No se pudo reiniciar el estado de la app antes de la prueba.");
        Driver = fixture.RestartDriver();

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
    }
}
