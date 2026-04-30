using App.Services.Interfaces;
using Microsoft.Maui.Storage;

namespace App.Services.Implementations;

public class PreferencesService : IPreferencesService
{
    public T Get<T>(string key, T defaultValue, string sharedName = default!)
    {
        return Preferences.Default.Get(key, defaultValue, sharedName);
    }

    public void Set<T>(string key, T value, string sharedName = default!)
    {
        Preferences.Default.Set(key, value, sharedName);
    }
}
