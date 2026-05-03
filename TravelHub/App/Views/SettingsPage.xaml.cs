using App.ViewModels;

namespace App.Views;

public partial class SettingsPage : ContentPage
{
    public SettingsPage()
    {
        InitializeComponent();
        Loaded += OnPageLoaded;
    }

    private void OnPageLoaded(object sender, EventArgs e)
    {
        Loaded -= OnPageLoaded;
        
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
