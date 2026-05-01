using App.Services.Implementations;
using App.Services.Interfaces;
using App.MarkupExtensions;

namespace App
{
    public partial class App : Application
    {
        public App(IAppInitializationService appInitializationService, IAppConfigurationService appConfigurationService, ILocalizationService localizationService)
        {
            if (appConfigurationService == null)
            {
                throw new ArgumentNullException(nameof(appConfigurationService));
            }
            if (appInitializationService == null)
            {
                throw new ArgumentNullException(nameof(appInitializationService));
            }
            //if (appSettingsService == null)
            //{
            //    throw new ArgumentNullException(nameof(appSettingsService));
            //}

            InitializeComponent();
            TranslateExtension.Initialize(localizationService);
            _ = InitializeApplicationAsync(appInitializationService, appConfigurationService);
        }

        private static async Task InitializeApplicationAsync(
            IAppInitializationService appInitializationService,
            IAppConfigurationService appConfigurationService)
        {
            await appInitializationService.InitializeAsync();
            await appConfigurationService.RefreshBackendUrlAsync();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }

    public class AppInitializationService : IAppInitializationService
    {
        private readonly IAppSettingsService _settings;

        public AppInitializationService(IAppSettingsService settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public Task ClearDataAsync()
        {
            throw new NotImplementedException();
        }

        public Task InitializeAsync()
        {
            // Implementation of initialization logic
            throw new NotImplementedException();
        }
    }
}
