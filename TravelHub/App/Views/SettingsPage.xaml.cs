using App.ViewModels;

namespace App.Views;

public partial class SettingsPage : ContentPage
{
    public SettingsPage()
    {
        InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        
        if (BindingContext == null)
        {
            try
            {
                if (IPlatformApplication.Current?.Services != null)
                {
                    BindingContext = IPlatformApplication.Current.Services.GetRequiredService<SettingsViewModel>();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error initializing SettingsPage: {ex}");
            }
        }
    }
}
