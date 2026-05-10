using App.Views;

namespace App;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Register routes for navigation
        Routing.RegisterRoute(nameof(BookingDetailsPage), typeof(BookingDetailsPage));
        Routing.RegisterRoute(nameof(BookingSummaryPage), typeof(BookingSummaryPage));
        Routing.RegisterRoute(nameof(SearchResultsPage), typeof(SearchResultsPage));
        Routing.RegisterRoute(nameof(PropertyDetailPage), typeof(PropertyDetailPage));
        Routing.RegisterRoute(nameof(RoomSelectionPage), typeof(RoomSelectionPage));
        Routing.RegisterRoute(nameof(TravelerDataPage), typeof(TravelerDataPage));
        Routing.RegisterRoute(nameof(BookingSummaryPage), typeof(BookingSummaryPage));
        Routing.RegisterRoute(nameof(BookingConfirmedPage), typeof(BookingConfirmedPage));
        Routing.RegisterRoute(nameof(AccountLoginPage), typeof(AccountLoginPage));
        Routing.RegisterRoute(nameof(CountryPage), typeof(CountryPage));
        Routing.RegisterRoute(nameof(AccountRegisterPage), typeof(AccountRegisterPage));
        Routing.RegisterRoute(nameof(PaymentPage), typeof(PaymentPage));
        Routing.RegisterRoute(nameof(QrScannerPage), typeof(QrScannerPage));
        Routing.RegisterRoute(nameof(CurrencyPage), typeof(CurrencyPage));

        Navigated += OnNavigated;
    }

    private static void OnNavigated(object? sender, ShellNavigatedEventArgs e)
    {
        if (Current?.CurrentPage is not { } page)
            return;

        SetContentDescriptions(page);
    }

    private static void SetContentDescriptions(IVisualTreeElement element)
    {
        foreach (var child in element.GetVisualChildren())
        {
            if (child is IView view)
                ApplyContentDescription(view);

            SetContentDescriptions(child);
        }
    }

    private static void ApplyContentDescription(IView view)
    {
        var id = view.AutomationId;
        if (string.IsNullOrEmpty(id))
            return;

#if ANDROID
        if (view.Handler?.PlatformView is Android.Views.View platformView)
            platformView.ContentDescription = id;
#endif
    }
}
