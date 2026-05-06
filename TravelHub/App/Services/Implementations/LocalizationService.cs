using System.Globalization;
using System.Resources;
using System.Threading.Tasks;
using App.Services.Interfaces;

namespace App.Services.Implementations
{
    public class LocalizationService : ILocalizationService
    {
        private const string PreferenceKey = "App_Language";
        private const string DefaultCulture = "es";
        
        public CultureInfo CurrentCulture { get; private set; }
        
        public event EventHandler LanguageChanged;
        
        private readonly ResourceManager _resourceManager;
        private readonly IPreferencesService _preferencesService;
        
        public LocalizationService(IPreferencesService preferencesService)
        {
            _resourceManager = new ResourceManager("App.Resources.Strings.AppResources", typeof(LocalizationService).Assembly);
            _preferencesService = preferencesService ?? throw new ArgumentNullException(nameof(preferencesService));
            
            var savedCulture = _preferencesService.Get<string>(PreferenceKey, DefaultCulture);
            SetLanguage(savedCulture);
        }
        
        public string GetString(string key)
        {
            try
            {
                var value = _resourceManager.GetString(key, CurrentCulture);
                return value ?? key;
            }
            catch
            {
                return key;
            }
        }
        
        public void SetLanguage(string cultureCode)
        {
            var culture = new CultureInfo(cultureCode);
            if (CurrentCulture?.Name == culture.Name)
            {
                return;
            }

            CurrentCulture = culture;
            
            _preferencesService.Set<string>(PreferenceKey, cultureCode);
            
            var handlers = LanguageChanged;
            if (handlers != null)
            {
                _ = Task.Run(() => handlers(this, EventArgs.Empty));
            }
        }
    }
}
