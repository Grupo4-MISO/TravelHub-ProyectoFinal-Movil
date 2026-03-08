using System.Globalization;

namespace App.Converters;

public class TruncateTextConverter : IValueConverter
{
    public int MaxLength { get; set; } = 15;
    public string Suffix { get; set; } = "...";

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string text || string.IsNullOrEmpty(text))
            return value;

        // Si se pasa un parámetro, usarlo como MaxLength
        if (parameter is string paramStr && int.TryParse(paramStr, out int maxLen))
        {
            MaxLength = maxLen;
        }

        if (text.Length <= MaxLength)
            return text;

        return text.Substring(0, MaxLength) + Suffix;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}