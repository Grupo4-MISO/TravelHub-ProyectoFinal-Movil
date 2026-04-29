using App.Services;

namespace App.Helpers;

public static class SettingHelper
{
    public static string Version
    {
        get => AppSettingsService.Instance.CurrentVersion;
        set => AppSettingsService.Instance.CurrentVersion = value;
    }
}
