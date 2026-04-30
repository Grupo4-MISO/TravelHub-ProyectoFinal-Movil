using App.Services.Implementations;
using App.Views;

namespace App.Controls;

public partial class TopBar : ContentView
{
    public TopBar()
    {
        InitializeComponent();
        UpdateCountryDisplay();

        // Suscribirse a cambios de país
        AppSettingsService.Instance.CountryChanged += OnCountryChanged;
    }

    private void UpdateCountryDisplay()
    {
        var country = AppSettingsService.Instance.CurrentCountry;
        CountryFlag.Text = country.FlagEmoji;
        CountryButton.Text = country.Code;
        CurrencyCodeButton.Text = country.CurrencyCode;
    }
    private void UpdateCurrencyDisplay()
    {
        var country = AppSettingsService.Instance.CurrentCountry;
        CurrencyCodeButton.Text = country.CurrencyCode;
    }

    private void OnCountryChanged(object? sender, string e)
    {
        UpdateCountryDisplay();
    }

    private async void OnCountryButtonClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(CountryPage));
    }
    private void OnCurrencyChanged(object? sender, string e)
    {
        UpdateCurrencyDisplay();
    }
    private async void OnCurrencyButtonClicked(object sender, EventArgs e)
    {
        //await Shell.Current.GoToAsync(nameof(CountryPage));
    }
}
