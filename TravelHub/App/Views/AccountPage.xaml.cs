using App.ViewModels;
using App.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace App.Views;

public partial class AccountPage : ContentPage
{
    private readonly ILocalizationService _localizationService;
    private bool _isUpdatingLanguage;
    
    public AccountPage()
    {
        InitializeComponent();
        BindingContext = IPlatformApplication.Current?.Services.GetRequiredService<AccountViewModel>();
        _localizationService = IPlatformApplication.Current?.Services.GetRequiredService<ILocalizationService>();
        UpdateLanguagePickerFromService();
    }
    
    protected override void OnAppearing()
    {
        base.OnAppearing();
        UpdateLanguagePickerFromService();
    }
    
    private void UpdateLanguagePickerFromService()
    {
        if (_localizationService == null)
        {
            return;
        }
        
        _isUpdatingLanguage = true;
        try
        {
            LanguagePicker.SelectedIndexChanged -= OnLanguageChanged;
            LanguagePicker.Items.Clear();
            LanguagePicker.Items.Add(_localizationService.GetString("Language_Espanol"));
            LanguagePicker.Items.Add(_localizationService.GetString("Language_English"));
            LanguagePicker.SelectedIndex = _localizationService.CurrentCulture.Name == "en" ? 1 : 0;
        }
        finally
        {
            LanguagePicker.SelectedIndexChanged += OnLanguageChanged;
            _isUpdatingLanguage = false;
        }
    }
    
    private void OnLanguageChanged(object sender, EventArgs e)
    {
        if (_isUpdatingLanguage || LanguagePicker.SelectedIndex == -1 || _localizationService == null)
            return;
            
        var cultureCode = LanguagePicker.SelectedIndex == 0 ? "es" : "en";
        if (_localizationService.CurrentCulture.Name == cultureCode)
        {
            return;
        }
        _localizationService.SetLanguage(cultureCode);
    }
}
