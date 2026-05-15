using App.ViewModels;
using App.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.ApplicationModel;

namespace App.Views;

public partial class SettingsPage : ContentPage
{
    private readonly ILocalizationService? _localizationService;
    private bool _isUpdatingLanguage;

    public SettingsPage()
    {
        InitializeComponent();
        _localizationService = IPlatformApplication.Current?.Services.GetRequiredService<ILocalizationService>();
        if (_localizationService != null)
        {
            _localizationService.LanguageChanged += OnServiceLanguageChanged;
        }

        UpdateLocalizedTexts();
        UpdateLanguagePickerFromService();
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        if (_localizationService != null)
        {
            _localizationService.LanguageChanged -= OnServiceLanguageChanged;
            _localizationService.LanguageChanged += OnServiceLanguageChanged;
        }

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

        if (BindingContext is SettingsViewModel viewModel)
        {
            viewModel.RefreshLocalization();
        }

        UpdateLocalizedTexts();
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

    private void OnLanguageChanged(object? sender, EventArgs e)
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

    protected override void OnDisappearing()
    {
        if (_localizationService != null)
        {
            _localizationService.LanguageChanged -= OnServiceLanguageChanged;
        }

        base.OnDisappearing();
    }

    private void OnServiceLanguageChanged(object? sender, EventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            if (BindingContext is SettingsViewModel viewModel)
            {
                viewModel.RefreshLocalization();
            }

            UpdateLocalizedTexts();
            UpdateLanguagePickerFromService();
        });
    }

    private void UpdateLocalizedTexts()
    {
        if (_localizationService == null)
        {
            return;
        }

        Title = _localizationService.GetString("Settings_Title");
        SettingsHeaderLabel.Text = _localizationService.GetString("Settings_Header");
        LanguageLabel.Text = _localizationService.GetString("Language_Seleccionar");
        DarkModeLabel.Text = _localizationService.GetString("Settings_DarkMode");
        ColorBlindnessLabel.Text = _localizationService.GetString("Settings_ColorBlindnessMode");
        ColorBlindnessPicker.Title = _localizationService.GetString("Settings_ColorBlindness_SelectMode");
        TextSizeLabel.Text = _localizationService.GetString("Settings_TextSize");
        TextSizePicker.Title = _localizationService.GetString("Settings_TextSize_SelectSize");
        RestoreDefaultsButton.Text = _localizationService.GetString("Settings_RestoreDefaults");
    }
}
