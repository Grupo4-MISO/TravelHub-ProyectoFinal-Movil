using App.Services.Implementations;
using App.Services.Interfaces;
using App.ViewModels;
using App.Views;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using App.Repositories.Interfaces;
using App.Repositories.Implementations;
using App.Providers.Interfaces;
using App.Providers.Implementations;

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



        // Register Services
        builder.Services.AddSingleton<IPreferencesService, PreferencesService>();
        builder.Services.AddSingleton<IMainThreadService, MainThreadService>();
        builder.Services.AddSingleton<IBackendUrlProvider, BackendUrlProvider>();

        // AppSettingsService with dependencies
        builder.Services.AddSingleton<IAppSettingsService, AppSettingsService>(sp =>
            new AppSettingsService(sp.GetRequiredService<IPreferencesService>()));

        builder.Services.AddSingleton<IAppInitializationService, AppInitializationService>();
        builder.Services.AddSingleton<IAppConfigurationService, AppConfigurationService>();
        builder.Services.AddSingleton<ICountryService, CountryService>();
        builder.Services.AddSingleton<ICityService, CityService>();
        builder.Services.AddSingleton<IAccommodationSearchService, AccommodationSearchService>();
        builder.Services.AddSingleton<IPropertyDetailService, PropertyDetailService>();
        builder.Services.AddSingleton<IBackEndService, BackEndService>();
        builder.Services.AddSingleton<IAuthService, AuthService>();
        builder.Services.AddSingleton<IUserSessionService, UserSessionService>();
        builder.Services.AddSingleton<ITravelerProfileService, TravelerProfileService>();
        builder.Services.AddSingleton<INavigationService, ShellNavigationService>();

        // Register Repositories
        builder.Services.AddSingleton<IDatabaseService, DatabaseService>();
        builder.Services.AddSingleton<ICountryRepository, CountryRepository>();
        builder.Services.AddSingleton<ICityRepository, CityRepository>();

        // Register ViewModels
        builder.Services.AddScoped<HomeViewModel>();
        builder.Services.AddScoped<SearchResultsViewModel>();
        builder.Services.AddTransient<PropertyDetailViewModel>();
        builder.Services.AddTransient<RoomSelectionViewModel>();
        builder.Services.AddTransient<TravelerDataViewModel>();
        builder.Services.AddTransient<BookingSummaryViewModel>();
        builder.Services.AddTransient<BookingConfirmedViewModel>();
        builder.Services.AddTransient<BookingDetailsViewModel>();
        builder.Services.AddTransient<AccountLoginViewModel>();
        builder.Services.AddTransient<AccountRegisterViewModel>();
        builder.Services.AddScoped<ActiveBookingsViewModel>();
        builder.Services.AddScoped<AccountViewModel>();
        builder.Services.AddScoped<CountryViewModel>();

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

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
