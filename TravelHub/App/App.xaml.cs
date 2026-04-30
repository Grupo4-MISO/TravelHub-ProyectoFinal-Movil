using App.Services.Interfaces;

namespace App
{
    public partial class App : Application
    {
        public App(IAppConfigurationService appConfigurationService, IAppInitializationService appInitializationService)
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
}
