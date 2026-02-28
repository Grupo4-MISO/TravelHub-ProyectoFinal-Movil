using App.Views;

namespace App;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Register routes for navigation
        Routing.RegisterRoute("SearchResultsPage", typeof(SearchResultsPage));
        Routing.RegisterRoute("PropertyDetailPage", typeof(PropertyDetailPage));
        Routing.RegisterRoute("RoomSelectionPage", typeof(RoomSelectionPage));
        Routing.RegisterRoute("TravelerDataPage", typeof(TravelerDataPage));
        Routing.RegisterRoute("BookingSummaryPage", typeof(BookingSummaryPage));
        Routing.RegisterRoute("BookingConfirmedPage", typeof(BookingConfirmedPage));
        Routing.RegisterRoute("LoginPage", typeof(LoginPage));
    }
}
