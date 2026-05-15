using App.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace App.ViewModels;

public class SettingsViewModel : BaseViewModel
{
    private readonly IAccessibilityService _accessibilityService;
    private readonly ILocalizationService _localizationService;

    public ICommand RestoreDefaultsCommand { get; }

    public SettingsViewModel(IAccessibilityService accessibilityService, ILocalizationService localizationService)
    {
        _accessibilityService = accessibilityService ?? throw new System.ArgumentNullException(nameof(accessibilityService));
        _localizationService = localizationService ?? throw new System.ArgumentNullException(nameof(localizationService));
        RestoreDefaultsCommand = new Command(async () => await RestoreDefaults());
        LoadCurrentSettings();
    }

    private bool _darkModeEnabled;
    public bool DarkModeEnabled
    {
        get => _darkModeEnabled;
        set
        {
            if (SetProperty(ref _darkModeEnabled, value))
            {
                _accessibilityService.DarkModeEnabled = value;
            }
        }
    }

    private string _colorBlindnessMode = "none";
    public string ColorBlindnessMode
    {
        get => _colorBlindnessMode;
        private set
        {
            if (SetProperty(ref _colorBlindnessMode, value))
            {
                _accessibilityService.ColorBlindnessMode = value;
                _selectedColorBlindnessOption = GetColorBlindnessLabel(value);
                OnPropertyChanged(nameof(SelectedColorBlindnessOption));
            }
        }
    }

    private string _selectedColorBlindnessOption = string.Empty;
    public string SelectedColorBlindnessOption
    {
        get => _selectedColorBlindnessOption;
        set
        {
            if (!SetProperty(ref _selectedColorBlindnessOption, value))
            {
                return;
            }

            if (_colorBlindnessByLabel.TryGetValue(value, out var mode))
            {
                ColorBlindnessMode = mode;
            }
        }
    }

    private double _textSizeScale = 1.0;
    public double TextSizeScale
    {
        get => _textSizeScale;
        set
        {
            if (SetProperty(ref _textSizeScale, value))
            {
                _accessibilityService.TextSizeScale = value;
            }
        }
    }

    public List<string> ColorBlindnessOptions { get; private set; } = new List<string>();

    public List<double> TextSizeOptions { get; } = new List<double> { 1.0, 1.25, 1.5, 2.0 };

    private readonly Dictionary<string, string> _colorBlindnessByLabel = new();

    public void RefreshLocalization()
    {
        BuildColorBlindnessOptions();
    }

    private void LoadCurrentSettings()
    {
        _darkModeEnabled = _accessibilityService.DarkModeEnabled;
        _colorBlindnessMode = _accessibilityService.ColorBlindnessMode;
        _textSizeScale = _accessibilityService.TextSizeScale;
        BuildColorBlindnessOptions();
        OnPropertyChanged(nameof(DarkModeEnabled));
        OnPropertyChanged(nameof(ColorBlindnessMode));
        OnPropertyChanged(nameof(TextSizeScale));
    }

    private void BuildColorBlindnessOptions()
    {
        var options = new[]
        {
            ("none", _localizationService.GetString("Settings_ColorBlindness_None")),
            ("protanopia", _localizationService.GetString("Settings_ColorBlindness_Protanopia")),
            ("deuteranopia", _localizationService.GetString("Settings_ColorBlindness_Deuteranopia")),
            ("tritanopia", _localizationService.GetString("Settings_ColorBlindness_Tritanopia"))
        };

        ColorBlindnessOptions = options.Select(option => option.Item2).ToList();

        _colorBlindnessByLabel.Clear();
        foreach (var (mode, label) in options)
        {
            _colorBlindnessByLabel[label] = mode;
        }

        var selectedLabel = GetColorBlindnessLabel(_colorBlindnessMode);
        if (string.IsNullOrWhiteSpace(selectedLabel))
        {
            selectedLabel = options[0].Item2;
            _colorBlindnessMode = options[0].Item1;
            _accessibilityService.ColorBlindnessMode = _colorBlindnessMode;
            OnPropertyChanged(nameof(ColorBlindnessMode));
        }

        _selectedColorBlindnessOption = selectedLabel;

        OnPropertyChanged(nameof(ColorBlindnessOptions));
        OnPropertyChanged(nameof(SelectedColorBlindnessOption));
    }

    private string GetColorBlindnessLabel(string mode)
    {
        return mode switch
        {
            "none" => _localizationService.GetString("Settings_ColorBlindness_None"),
            "protanopia" => _localizationService.GetString("Settings_ColorBlindness_Protanopia"),
            "deuteranopia" => _localizationService.GetString("Settings_ColorBlindness_Deuteranopia"),
            "tritanopia" => _localizationService.GetString("Settings_ColorBlindness_Tritanopia"),
            _ => string.Empty
        };
    }

    public async Task RestoreDefaults()
    {
        DarkModeEnabled = false;
        ColorBlindnessMode = "none";
        TextSizeScale = 1.0;
        BuildColorBlindnessOptions();
        await _accessibilityService.ApplySettingsAsync();
    }
}

