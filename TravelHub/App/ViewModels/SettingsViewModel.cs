using App.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

namespace App.ViewModels;

public class SettingsViewModel : BaseViewModel
{
    private readonly IAccessibilityService _accessibilityService;

    public ICommand RestoreDefaultsCommand { get; }

    public SettingsViewModel(IAccessibilityService accessibilityService)
    {
        _accessibilityService = accessibilityService ?? throw new System.ArgumentNullException(nameof(accessibilityService));
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
        set
        {
            if (SetProperty(ref _colorBlindnessMode, value))
            {
                _accessibilityService.ColorBlindnessMode = value;
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

    public List<string> ColorBlindnessOptions { get; } = new List<string>
    {
        "none",
        "protanopia",
        "deuteranopia",
        "tritanopia"
    };

    public List<double> TextSizeOptions { get; } = new List<double> { 1.0, 1.25, 1.5, 2.0 };

    private void LoadCurrentSettings()
    {
        _darkModeEnabled = _accessibilityService.DarkModeEnabled;
        _colorBlindnessMode = _accessibilityService.ColorBlindnessMode;
        _textSizeScale = _accessibilityService.TextSizeScale;
        OnPropertyChanged(nameof(DarkModeEnabled));
        OnPropertyChanged(nameof(ColorBlindnessMode));
        OnPropertyChanged(nameof(TextSizeScale));
    }

    public async Task RestoreDefaults()
    {
        DarkModeEnabled = false;
        ColorBlindnessMode = "none";
        TextSizeScale = 1.0;
        await _accessibilityService.ApplySettingsAsync();
    }
}

