using System.Globalization;
using App.Resources.Styles;

namespace App.Converters;

public class IconKeyToGlyphConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string iconKey)
        {
            // Intenta obtener desde la clase estática primero
            var glyph = Icons.GetGlyphByKey(iconKey);
            if (!string.IsNullOrEmpty(glyph) && glyph != Icons.Info)
                return glyph;

            // Fallback: intenta obtener desde los recursos XAML
            if (Application.Current?.Resources.TryGetValue(iconKey, out var resourceGlyph) == true)
            {
                return resourceGlyph;
            }
        }
        return Icons.Info; // Icono por defecto
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}