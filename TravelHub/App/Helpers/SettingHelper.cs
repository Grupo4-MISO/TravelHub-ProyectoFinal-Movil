using App.Services.Implementations;

namespace App.Helpers;

public class SettingHelper
{
    private readonly AppSettingsService _appSettingsService;

    public SettingHelper(AppSettingsService appSettingsService)
    {
        _appSettingsService = appSettingsService ?? throw new ArgumentNullException(nameof(appSettingsService));
    }

    public string Version
    {
        get => _appSettingsService.CurrentVersion;
        set => _appSettingsService.CurrentVersion = value;
    }
}
