using App.Services.Interfaces;
using App.Views;

namespace App.Controls;

public partial class TopBar : ContentView
{
    private IAppSettingsService _appSettingsService = null!;

    public TopBar()
    {
        InitializeComponent();

        HandlerChanged += OnHandlerChanged;
    }

    private void OnHandlerChanged(object? sender, EventArgs e)
    {
        if (Handler?.MauiContext?.Services == null)
            return;

        _appSettingsService = Handler.MauiContext.Services.GetRequiredService<IAppSettingsService>()
            ?? throw new InvalidOperationException("No se pudo resolver IAppSettingsService");

        _ = UpdateCountryDisplayAsync();
        _appSettingsService.CountryChanged += OnCountryChanged;
        _appSettingsService.CurrencyChanged += OnCurrencyChanged;

        HandlerChanged -= OnHandlerChanged;
    }

    private async Task UpdateCountryDisplayAsync()
    {
        if (_appSettingsService == null) return;

        var country = await _appSettingsService.GetCurrentCountryAsync();
        MainThread.BeginInvokeOnMainThread(() =>
        {
            CountryFlag.Text = country?.FlagEmoji ?? string.Empty;
            CountryButton.Text = country?.Code ?? string.Empty;
            CurrencyCodeButton.Text = _appSettingsService.CurrentCurrencyCode;
        });
    }

    private void OnCountryChanged(object? sender, string e)
    {
        _ = UpdateCountryDisplayAsync();
    }

    private void OnCurrencyChanged(object? sender, string e)
    {
        _ = UpdateCountryDisplayAsync();
    }

    private async void OnCountryButtonClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(CountryPage));
    }

    private async void OnCurrencyButtonClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(CurrencyPage));
    }
}
