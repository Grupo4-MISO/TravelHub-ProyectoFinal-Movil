using App.Services.Interfaces;

namespace App
{
    public partial class App : Application
    {
        public App(IAppConfigurationService appConfigurationService)
        {
            if (appConfigurationService == null)
            {
                throw new ArgumentNullException(nameof(appConfigurationService));
            }

            InitializeComponent();
            _ = appConfigurationService.RefreshBackendUrlAsync();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}
