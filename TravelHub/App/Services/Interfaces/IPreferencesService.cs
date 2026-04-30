namespace App.Services.Interfaces;

public interface IPreferencesService
{
    T Get<T>(string key, T defaultValue, string sharedName = default!);
    void Set<T>(string key, T value, string sharedName = default!);
}
