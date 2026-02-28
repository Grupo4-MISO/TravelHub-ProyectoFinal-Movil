using App.ViewModels;
using App.Views;
using Microsoft.Extensions.Logging;

namespace App;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Register pages
        builder.Services.AddTransient<HomePage>();
        builder.Services.AddTransient<SearchResultsPage>();
        builder.Services.AddTransient<PropertyDetailPage>();
        builder.Services.AddTransient<RoomSelectionPage>();
        builder.Services.AddTransient<TravelerDataPage>();
        builder.Services.AddTransient<BookingSummaryPage>();
        builder.Services.AddTransient<BookingConfirmedPage>();
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<ActiveBookingsPage>();
        builder.Services.AddTransient<AccountPage>();

        // Register ViewModels
        builder.Services.AddTransient<HomeViewModel>();
        builder.Services.AddTransient<SearchResultsViewModel>();
        builder.Services.AddTransient<PropertyDetailViewModel>();
        builder.Services.AddTransient<RoomSelectionViewModel>();
        builder.Services.AddTransient<TravelerDataViewModel>();
        builder.Services.AddTransient<BookingSummaryViewModel>();
        builder.Services.AddTransient<BookingConfirmedViewModel>();
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<ActiveBookingsViewModel>();
        builder.Services.AddTransient<AccountViewModel>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
