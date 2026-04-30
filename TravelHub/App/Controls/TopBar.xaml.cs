using App.Services.Interfaces;
using App.Views;

namespace App.Controls;

public partial class TopBar : ContentView
{
    private IAppSettingsService _appSettingsService;

    public TopBar()
    {
        InitializeComponent();

        // ✅ Esperar a que el Handler esté disponible
        HandlerChanged += OnHandlerChanged;
    }

    private void OnHandlerChanged(object? sender, EventArgs e)
    {
        if (Handler?.MauiContext?.Services == null)
            return;

        _appSettingsService = Handler.MauiContext.Services.GetRequiredService<IAppSettingsService>()
            ?? throw new InvalidOperationException("No se pudo resolver IAppSettingsService");

        UpdateCountryDisplay();
        _appSettingsService.CountryChanged += OnCountryChanged;

        // Desuscribirse para no duplicar
        HandlerChanged -= OnHandlerChanged;
    }

    private void UpdateCountryDisplay()
    {
        if (_appSettingsService == null) return;

        var country = _appSettingsService.CurrentCountry;
        CountryFlag.Text = country?.FlagEmoji ?? string.Empty;
        CountryButton.Text = country?.Code ?? string.Empty;
        CurrencyCodeButton.Text = country?.CurrencyCode ?? string.Empty;
    }

    private void OnCountryChanged(object? sender, string e)
    {
        UpdateCountryDisplay();
    }

    private async void OnCountryButtonClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(CountryPage));
    }

    private void OnCurrencyButtonClicked(object sender, EventArgs e)
    {

    }
}
