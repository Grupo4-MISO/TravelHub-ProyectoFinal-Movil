using System.Text.Json;

namespace App.Utils;

public static class JwtDecoder
{
    public static bool IsTokenExpired(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return true;

        try
        {
            var expiration = GetExpirationFromToken(token);
            if (expiration == null)
                return true;

            return expiration.Value <= DateTimeOffset.UtcNow;
        }
        catch
        {
            return true;
        }
    }

    public static DateTimeOffset? GetExpirationFromToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return null;

        var parts = token.Split('.');
        if (parts.Length < 2)
            return null;

        var payload = parts[1];
        var json = DecodeBase64Url(payload);

        using var doc = JsonDocument.Parse(json);
        if (doc.RootElement.TryGetProperty("exp", out var expElement))
        {
            var expSeconds = expElement.GetInt64();
            return DateTimeOffset.FromUnixTimeSeconds(expSeconds);
        }

        return null;
    }

    private static string DecodeBase64Url(string input)
    {
        var padded = input.Replace('-', '+').Replace('_', '/');
        switch (padded.Length % 4)
        {
            case 2: padded += "=="; break;
            case 3: padded += "="; break;
        }
        var bytes = Convert.FromBase64String(padded);
        return System.Text.Encoding.UTF8.GetString(bytes);
    }
}
