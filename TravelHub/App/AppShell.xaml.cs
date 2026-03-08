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
        Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
    }
}
