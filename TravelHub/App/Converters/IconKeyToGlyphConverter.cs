using System.Globalization;

namespace App.Converters;

public class IconKeyToGlyphConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string iconKey && Application.Current?.Resources.TryGetValue(iconKey, out var glyph) == true)
        {
            return glyph;
        }
        return "&#xe88e;"; // Icono por defecto (info)
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}