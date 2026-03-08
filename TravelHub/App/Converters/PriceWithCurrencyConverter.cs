using System.Globalization;
using App.Models;

namespace App.Converters;

public class PriceWithCurrencyConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is decimal price && parameter is Country country)
        {
            return $"{country.CurrencySymbol}{price:N0}";
        }
        
        if (value is decimal priceOnly)
        {
            return $"${priceOnly:N0}"; // Default
        }

        return value;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}