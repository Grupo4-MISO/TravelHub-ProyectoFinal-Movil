using App.Services.Interfaces;

namespace App.MarkupExtensions
{
    [ContentProperty(nameof(Key))]
    public class TranslateExtension : IMarkupExtension
    {
        private static ILocalizationService? _localizationService;
        
        public string? Key { get; set; }
        
        public TranslateExtension()
        {
        }
        
        public TranslateExtension(string key)
        {
            Key = key;
        }
        
        public static void Initialize(ILocalizationService localizationService)
        {
            _localizationService = localizationService;
        }
        
        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (string.IsNullOrEmpty(Key) || _localizationService == null)
                return Key ?? string.Empty;
                
            return _localizationService.GetString(Key);
        }
    }
}
