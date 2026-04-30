using App.Helpers;
using App.Services.Interfaces;

namespace App.Services.Implementations;

public class AppInitializationService : IAppInitializationService
{
    private readonly IDatabaseService _databaseService;
    private readonly AppSettingsService _appSettingsService;
    private readonly SettingHelper _settingHelper;
    private readonly SemaphoreSlim _sync = new(1, 1);
    private bool _isInitialized;

    public AppInitializationService(IDatabaseService databaseService, AppSettingsService appSettingsService)
    {
        _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
        _appSettingsService = appSettingsService ?? throw new ArgumentNullException(nameof(appSettingsService));
        _settingHelper = new SettingHelper(appSettingsService);
    }

    public async Task InitializeAsync()
    {
        if (_isInitialized)
        {
            return;
        }

        await _sync.WaitAsync();
        try
        {
            if (_isInitialized)
            {
                return;
            }

            VersionTracking.Track();
            var currentVersion = VersionTracking.CurrentVersion;

            if (!currentVersion.Equals(_appSettingsService.CurrentVersion, StringComparison.Ordinal))
            {
                await ClearDataAsync();
            }

            await _databaseService.InitSQL();
            _isInitialized = true;
        }
        finally
        {
            _sync.Release();
        }
    }

    public async Task ClearDataAsync()
    {
        VersionTracking.Track();
        var currentVersion = VersionTracking.CurrentVersion;

        CacheHelper.ClearAllPreferences();
        await _databaseService.DeleteAllDatabaseFiles();
        _settingHelper.Version = currentVersion;
    }
}
