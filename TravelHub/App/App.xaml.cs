using App.Services.Interfaces;
using App.Services.Implementations;
using App.MarkupExtensions;
using System.Threading.Tasks;

namespace App
{
    public partial class App : Application
    {
        public App(
            IAppInitializationService appInitializationService, 
            IAppConfigurationService appConfigurationService, 
            ILocalizationService localizationService,
            IAccessibilityService accessibilityService)
        {
            if (appConfigurationService == null)
            {
                throw new ArgumentNullException(nameof(appConfigurationService));
            }
            if (appInitializationService == null)
            {
                throw new ArgumentNullException(nameof(appInitializationService));
            }

            InitializeComponent();
            TranslateExtension.Initialize(localizationService);
            
            _ = InitializeApplicationAsync(appInitializationService, appConfigurationService, accessibilityService);
        }

        private static async Task InitializeApplicationAsync(
            IAppInitializationService appInitializationService,
            IAppConfigurationService appConfigurationService,
            IAccessibilityService accessibilityService)
        {
            await appInitializationService.InitializeAsync();
            await appConfigurationService.RefreshBackendUrlAsync();
            await accessibilityService.LoadSettingsAsync();
            await accessibilityService.ApplySettingsAsync();
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
