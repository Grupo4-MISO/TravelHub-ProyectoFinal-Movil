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
    protected AndroidDriver Driver { get; private set; } = null!;
    protected readonly AppiumFixture Fixture;
    protected HomePage Home { get; private set; } = null!;
    protected AccountLoginPage Login { get; private set; } = null!;
    protected AccountRegisterPage Register { get; private set; } = null!;
    protected AccountPage Account { get; private set; } = null!;
    protected SearchResultsPage SearchResults { get; private set; } = null!;
    protected PropertyDetailPage PropertyDetail { get; private set; } = null!;
    protected RoomSelectionPage RoomSelection { get; private set; } = null!;
    protected TravelerDataPage TravelerData { get; private set; } = null!;
    protected BookingSummaryPage BookingSummary { get; private set; } = null!;
    protected BookingConfirmedPage BookingConfirmed { get; private set; } = null!;
    protected PaymentPage Payment { get; private set; } = null!;
    protected SettingsPage Settings { get; private set; } = null!;
    protected ActiveBookingsPage ActiveBookings { get; private set; } = null!;

    private readonly string _testName;

    protected BaseTest(AppiumFixture fixture)
    {
        Fixture = fixture;
        _testName = GetType().Name;

        if (!fixture.IsEnvironmentReady)
            throw new InvalidOperationException(fixture.SkipReason ?? "Entorno de Appium/Android no disponible.");
 
        // Ensure a clean app state before each test to reduce inter-test flakiness
        var resetOk = false;
        for (int i = 0; i < 3; i++)
        {
            if (fixture.ResetAppState())
            {
                resetOk = true;
                break;
            }
            Thread.Sleep(3000);
        }

        if (!resetOk)
            throw new InvalidOperationException("No se pudo reiniciar el estado de la app antes de la prueba.");

        Driver = null!;
        for (int i = 0; i < 3; i++)
        {
            try
            {
                Driver = fixture.RestartDriver();
                break;
            }
            catch (WebDriverException) when (i < 2)
            {
                Thread.Sleep(5000);
            }
        }

        if (Driver == null)
            throw new InvalidOperationException("No se pudo inicializar el driver Appium después de varios intentos.");

        InitializePages();
    }

    private void InitializePages()
    {
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

    protected void RetryOnCrash(Action testBody, int maxRetries = 3)
    {
        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                testBody();
                return;
            }
            catch (WebDriverException ex) when (attempt < maxRetries && IsConnectionLost(ex))
            {
                Driver = Fixture.RestartDriver();
                InitializePages();
            }
        }
    }

    private static bool IsConnectionLost(WebDriverException ex)
    {
        var msg = ex.Message;
        return msg.Contains("socket hang up") ||
               msg.Contains("ECONNREFUSED") ||
               msg.Contains("code 255") ||
               msg.Contains("process exited");
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
        catch (WebDriverException)
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
