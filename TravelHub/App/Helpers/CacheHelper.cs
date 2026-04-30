namespace App.Helpers;

public static class CacheHelper
{
    public static void ClearAllPreferences()
    {
        Preferences.Default.Clear();
    }
}
