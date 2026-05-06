using App.MarkupExtensions;
using App.Services.Implementations;
using App.Services.Interfaces;
using Microsoft.Extensions.Logging;
using OneSignalSDK.DotNet;
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

            // Iniciar el servicio de OneSignal
            InitializeOneSignal();
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

        // Inicialización de OneSignal
        private void InitializeOneSignal()
        {
            // Enable verbose OneSignal logging to debug issues if needed.
            //OneSignal.Debug.LogLevel = LogLevel.VERBOSE;

            OneSignal.Initialize("4a46cf3a-a9ec-456b-b382-de8793e5f38b");

            // RequestPermissionAsync will show the notification permission prompt.
            // We recommend removing the following code and instead using an In-App Message to prompt for notification permission (See step 5)
            OneSignal.Notifications.RequestPermissionAsync(true);
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
