using App.Services.Interfaces;
using Microsoft.Maui.Controls;

namespace App.Services.Implementations;

public class AccessibilityService : IAccessibilityService
{
    private readonly IPreferencesService _preferencesService;
    
    private const string ColorBlindnessKey = "accessibility_color_blindness_mode";
    private const string TextSizeScaleKey = "accessibility_text_size_scale";
    private const string DarkModeKey = "accessibility_dark_mode_enabled";
    
    private string _colorBlindnessMode = "none";
    private double _textSizeScale = 1.0;
    private bool _darkModeEnabled = false;
    
    public event EventHandler? AccessibilitySettingsChanged;
    
    public string ColorBlindnessMode
    {
        get => _colorBlindnessMode;
        set
        {
            if (_colorBlindnessMode != value)
            {
                _colorBlindnessMode = value;
                _preferencesService.Set(ColorBlindnessKey, value);
                OnAccessibilitySettingsChanged();
            }
        }
    }
    
    public double TextSizeScale
    {
        get => _textSizeScale;
        set
        {
            if (Math.Abs(_textSizeScale - value) > 0.001)
            {
                _textSizeScale = value;
                _preferencesService.Set(TextSizeScaleKey, value);
                OnAccessibilitySettingsChanged();
            }
        }
    }
    
    public bool DarkModeEnabled
    {
        get => _darkModeEnabled;
        set
        {
            if (_darkModeEnabled != value)
            {
                _darkModeEnabled = value;
                _preferencesService.Set(DarkModeKey, value);
                OnAccessibilitySettingsChanged();
            }
        }
    }
    
    public AccessibilityService(IPreferencesService preferencesService)
    {
        _preferencesService = preferencesService ?? throw new ArgumentNullException(nameof(preferencesService));
    }
    
    public Task LoadSettingsAsync()
    {
        _colorBlindnessMode = _preferencesService.Get(ColorBlindnessKey, "none");
        _textSizeScale = _preferencesService.Get(TextSizeScaleKey, 1.0);
        _darkModeEnabled = _preferencesService.Get(DarkModeKey, false);
        return Task.CompletedTask;
    }
    
    public Task ApplySettingsAsync()
    {
        ApplyColorBlindnessMode();
        ApplyTextSizeScale();
        ApplyDarkMode();
        return Task.CompletedTask;
    }
    
    private void ApplyColorBlindnessMode()
    {
        var resources = Application.Current?.Resources;
        if (resources == null) return;
        
        if (_colorBlindnessMode == "none")
        {
            resources["Primary"] = Color.FromArgb("#015C77");
            resources["Accent"] = Color.FromArgb("#F06D51");
            resources["Success"] = Color.FromArgb("#4CAF50");
            resources["Error"] = Color.FromArgb("#F44336");
            resources["Star"] = Color.FromArgb("#FFC107");
        }
        else if (_colorBlindnessMode == "protanopia")
        {
            resources["Primary"] = Color.FromArgb("#01698C");
            resources["Accent"] = Color.FromArgb("#E87C3A");
            resources["Success"] = Color.FromArgb("#4CAF50");
            resources["Error"] = Color.FromArgb("#D32F2F");
            resources["Star"] = Color.FromArgb("#FFC107");
        }
        else if (_colorBlindnessMode == "deuteranopia")
        {
            resources["Primary"] = Color.FromArgb("#017A7A");
            resources["Accent"] = Color.FromArgb("#E87C3A");
            resources["Success"] = Color.FromArgb("#88B04B");
            resources["Error"] = Color.FromArgb("#D32F2F");
            resources["Star"] = Color.FromArgb("#FFC107");
        }
        else if (_colorBlindnessMode == "tritanopia")
        {
            resources["Primary"] = Color.FromArgb("#015C77");
            resources["Accent"] = Color.FromArgb("#D4845E");
            resources["Success"] = Color.FromArgb("#4CAF50");
            resources["Error"] = Color.FromArgb("#E91E63");
            resources["Star"] = Color.FromArgb("#FFC107");
        }
    }
    
    private void ApplyTextSizeScale()
    {
        var resources = Application.Current?.Resources;
        if (resources == null) return;
        
        double baseFontSize = 14.0;
        resources["BaseFontSize"] = baseFontSize * _textSizeScale;
        resources["HeadlineFontSize"] = 28.0 * _textSizeScale;
        resources["SubHeadlineFontSize"] = 20.0 * _textSizeScale;
        resources["TitleFontSize"] = 18.0 * _textSizeScale;
        resources["SubtitleFontSize"] = 14.0 * _textSizeScale;
        resources["CaptionFontSize"] = 14.0 * _textSizeScale;
        resources["PriceFontSize"] = 20.0 * _textSizeScale;
    }
    
    private void ApplyDarkMode()
    {
        var resources = Application.Current?.Resources;
        if (resources == null) return;
        
        if (_darkModeEnabled)
        {
            resources["Background"] = Color.FromArgb("#121212");
            resources["Surface"] = Color.FromArgb("#1E1E1E");
            resources["TextPrimary"] = Colors.White;
            resources["TextSecondary"] = Color.FromArgb("#B3B3B3");
            resources["Primary"] = Color.FromArgb("#4FC3F7");
            resources["Gray100"] = Color.FromArgb("#2D2D2D");
            resources["Gray200"] = Color.FromArgb("#3D3D3D");
            resources["Gray300"] = Color.FromArgb("#4D4D4D");
        }
        else
        {
            resources["Background"] = Color.FromArgb("#F5F5F5");
            resources["Surface"] = Colors.White;
            resources["TextPrimary"] = Color.FromArgb("#212121");
            resources["TextSecondary"] = Color.FromArgb("#757575");
            resources["Primary"] = Color.FromArgb("#015C77");
            resources["Gray100"] = Color.FromArgb("#E1E1E1");
            resources["Gray200"] = Color.FromArgb("#C8C8C8");
            resources["Gray300"] = Color.FromArgb("#ACACAC");
        }
    }
    
    protected virtual void OnAccessibilitySettingsChanged()
    {
        AccessibilitySettingsChanged?.Invoke(this, EventArgs.Empty);
        _ = ApplySettingsAsync();
    }
}
