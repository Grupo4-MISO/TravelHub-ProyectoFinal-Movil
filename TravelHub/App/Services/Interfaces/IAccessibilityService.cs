using System;
using System.Threading.Tasks;

namespace App.Services.Interfaces;

public interface IAccessibilityService
{
    string ColorBlindnessMode { get; set; }
    double TextSizeScale { get; set; }
    bool DarkModeEnabled { get; set; }
    
    event EventHandler? AccessibilitySettingsChanged;
    
    Task ApplySettingsAsync();
    Task LoadSettingsAsync();
}
