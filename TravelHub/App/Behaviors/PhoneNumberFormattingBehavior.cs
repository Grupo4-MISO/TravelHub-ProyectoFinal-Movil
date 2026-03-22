using Microsoft.Maui.Controls;

namespace App.Behaviors;

public class PhoneNumberFormattingBehavior : Behavior<Entry>
{
    protected override void OnAttachedTo(Entry entry)
    {
        base.OnAttachedTo(entry);
        entry.TextChanged += OnTextChanged;
    }

    protected override void OnDetachingFrom(Entry entry)
    {
        base.OnDetachingFrom(entry);
        entry.TextChanged -= OnTextChanged;
    }

    private void OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (sender is not Entry entry)
            return;

        var text = e.NewTextValue;
        if (string.IsNullOrEmpty(text))
            return;

        // Remover todos los caracteres que no sean dígitos
        var digitsOnly = new string(text.Where(char.IsDigit).ToArray());

        // Limitar a 10 dígitos (ajusta según el país)
        if (digitsOnly.Length > 10)
            digitsOnly = digitsOnly.Substring(0, 10);

        // Formatear: 321 123 4567
        var formatted = FormatPhoneNumber(digitsOnly);

        if (formatted != text)
        {
            entry.TextChanged -= OnTextChanged;
            entry.Text = formatted;
            entry.CursorPosition = formatted.Length;
            entry.TextChanged += OnTextChanged;
        }
    }

    private string FormatPhoneNumber(string digits)
    {
        if (digits.Length <= 3)
            return digits;
        
        if (digits.Length <= 6)
            return $"{digits.Substring(0, 3)} {digits.Substring(3)}";
        
        return $"{digits.Substring(0, 3)} {digits.Substring(3, 3)} {digits.Substring(6)}";
    }
}