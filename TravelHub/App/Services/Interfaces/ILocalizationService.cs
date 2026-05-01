using System.Globalization;

namespace App.Services.Interfaces
{
    public interface ILocalizationService
    {
        CultureInfo CurrentCulture { get; }
        string GetString(string key);
        void SetLanguage(string cultureCode);
        event EventHandler LanguageChanged;
    }
}
