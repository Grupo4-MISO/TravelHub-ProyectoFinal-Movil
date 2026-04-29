using App.Services.Implementations;
using App.Services.Interfaces;
using App.ViewModels;
using App.Views;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;

namespace App;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseMauiMaps()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("MaterialIcons-Regular.ttf", "MaterialSymbols");
            });

        // Register pages
        builder.Services.AddTransient<HomePage>();
        builder.Services.AddTransient<SearchResultsPage>();
        builder.Services.AddTransient<PropertyDetailPage>();
        builder.Services.AddTransient<RoomSelectionPage>();
        builder.Services.AddTransient<TravelerDataPage>();
        builder.Services.AddTransient<BookingSummaryPage>();
        builder.Services.AddTransient<BookingConfirmedPage>();
        builder.Services.AddTransient<BookingDetailsPage>();
        builder.Services.AddTransient<AccountLoginPage>();
        builder.Services.AddTransient<AccountRegisterPage>();
        builder.Services.AddTransient<ActiveBookingsPage>();
        builder.Services.AddTransient<AccountPage>();
        builder.Services.AddTransient<CountryPage>();

        // Register ViewModels
        builder.Services.AddTransient<HomeViewModel>();
        builder.Services.AddTransient<SearchResultsViewModel>();
        builder.Services.AddTransient<PropertyDetailViewModel>();
        builder.Services.AddTransient<RoomSelectionViewModel>();
        builder.Services.AddTransient<TravelerDataViewModel>();
        builder.Services.AddTransient<BookingSummaryViewModel>();
        builder.Services.AddTransient<BookingConfirmedViewModel>();
        builder.Services.AddTransient<BookingDetailsViewModel>();
        builder.Services.AddTransient<AccountLoginViewModel>();
        builder.Services.AddTransient<AccountRegisterViewModel>();
        builder.Services.AddTransient<ActiveBookingsViewModel>();
        builder.Services.AddTransient<AccountViewModel>();
        builder.Services.AddTransient<CountryViewModel>();

        // Register Services
        builder.Services.AddSingleton<IBackendUrlProvider, BackendUrlProvider>();
        builder.Services.AddSingleton<IBackEndService, BackEndService>();
        builder.Services.AddSingleton<IAppConfigurationService, AppConfigurationService>();
        builder.Services.AddSingleton<ICountryService, CountryService>();
        builder.Services.AddSingleton<IAccommodationSearchService, AccommodationSearchService>();
        builder.Services.AddSingleton<IPropertyDetailService, PropertyDetailService>();
        builder.Services.AddSingleton<IAuthService, AuthService>();
        builder.Services.AddSingleton<IUserSessionService, UserSessionService>();
        builder.Services.AddSingleton<ITravelerProfileService, TravelerProfileService>();
        builder.Services.AddSingleton<INavigationService, ShellNavigationService>();
        builder.Services.AddSingleton<IAppSettingsService>(sp => global::App.Services.AppSettingsService.Instance);

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
