using System.ComponentModel;
using App.Services.Interfaces;
using Microsoft.Maui.Controls;

namespace App.MarkupExtensions
{
    [ContentProperty(nameof(Key))]
    public class TranslateExtension : IMarkupExtension
    {
        private static ILocalizationService? _localizationService;
        private static readonly LocalizedBindingSource LocalizedSource = new();
        
        public string? Key { get; set; }
        
        public TranslateExtension()
        {
        }
        
        public TranslateExtension(string key)
        {
            Key = key;
        }
        
        public static void Initialize(ILocalizationService? localizationService)
        {
            _localizationService = localizationService;
            LocalizedSource.SetLocalizationService(localizationService);
        }
        
        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (string.IsNullOrEmpty(Key))
            {
                return string.Empty;
            }

            if (_localizationService == null)
            {
                return Key;
            }

            if (serviceProvider == null)
            {
                return _localizationService.GetString(Key);
            }

            return new Binding($"[{Key}]", source: LocalizedSource);
        }

        private sealed class LocalizedBindingSource : INotifyPropertyChanged
        {
            private ILocalizationService? _source;

            public string this[string key] => _source?.GetString(key) ?? key;

            public event PropertyChangedEventHandler? PropertyChanged;

            public void SetLocalizationService(ILocalizationService? source)
            {
                if (_source != null)
                {
                    _source.LanguageChanged -= OnLanguageChanged;
                }

                _source = source;

                if (_source != null)
                {
                    _source.LanguageChanged += OnLanguageChanged;
                }

                NotifyTranslationsChanged();
            }

            private void OnLanguageChanged(object? sender, EventArgs e)
            {
                NotifyTranslationsChanged();
            }

            private void NotifyTranslationsChanged()
            {
                var eventArgs = new PropertyChangedEventArgs("Item[]");

                var dispatcher = Application.Current?.Dispatcher;
                if (dispatcher == null)
                {
                    PropertyChanged?.Invoke(this, eventArgs);
                    return;
                }

                if (dispatcher.IsDispatchRequired)
                {
                    dispatcher.Dispatch(() => PropertyChanged?.Invoke(this, eventArgs));
                    return;
                }

                PropertyChanged?.Invoke(this, eventArgs);
            }
        }
    }
}
