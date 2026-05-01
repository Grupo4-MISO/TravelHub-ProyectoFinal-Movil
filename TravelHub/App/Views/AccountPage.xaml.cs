using App.ViewModels;
using App.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace App.Views;

public partial class AccountPage : ContentPage
{
    private readonly ILocalizationService _localizationService;
    
    public AccountPage()
    {
        InitializeComponent();
        BindingContext = IPlatformApplication.Current?.Services.GetRequiredService<AccountViewModel>();
        _localizationService = IPlatformApplication.Current?.Services.GetRequiredService<ILocalizationService>();
        
        // Set picker items with localized strings
        if (_localizationService != null)
        {
            LanguagePicker.Items.Clear();
            LanguagePicker.Items.Add(_localizationService.GetString("Language_Espanol"));
            LanguagePicker.Items.Add(_localizationService.GetString("Language_English"));
            LanguagePicker.SelectedIndex = _localizationService.CurrentCulture.Name == "en" ? 1 : 0;
            
            // Update picker items when language changes
            _localizationService.LanguageChanged += (s, e) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    var currentIndex = LanguagePicker.SelectedIndex;
                    LanguagePicker.Items.Clear();
                    LanguagePicker.Items.Add(_localizationService.GetString("Language_Espanol"));
                    LanguagePicker.Items.Add(_localizationService.GetString("Language_English"));
                    LanguagePicker.SelectedIndex = currentIndex;
                });
            };
        }
    }
    
    private void OnLanguageChanged(object sender, EventArgs e)
    {
        if (LanguagePicker.SelectedIndex == -1 || _localizationService == null)
            return;
            
        var cultureCode = LanguagePicker.SelectedIndex == 0 ? "es" : "en";
        _localizationService.SetLanguage(cultureCode);
    }
}
