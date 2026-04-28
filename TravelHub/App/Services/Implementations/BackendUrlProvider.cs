using App.Services.Interfaces;
using Microsoft.Maui.Storage;

namespace App.Services.Implementations;

public class BackendUrlProvider : IBackendUrlProvider
{
    private const string BackendUrlEnvironmentVariable = "TRAVELHUB_BACKEND_URL";
    private const string BackendUrlPreferenceKey = "BackendUrl";
    private const string DefaultBackendUrl = "https://light-eggs-lie.loca.lt";
    private readonly object _sync = new();
    private string _baseUrl;

    public string BaseUrl
    {
        get
        {
            lock (_sync)
            {
                return _baseUrl;
            }
        }
    }

    public BackendUrlProvider()
    {
        var configuredDefaultUrl = NormalizeBaseUrl(Environment.GetEnvironmentVariable(BackendUrlEnvironmentVariable));
        var storedUrl = Preferences.Default.Get(BackendUrlPreferenceKey, configuredDefaultUrl);
        var normalizedStoredUrl = NormalizeBaseUrl(storedUrl);

        _baseUrl = normalizedStoredUrl;
        Preferences.Default.Set(BackendUrlPreferenceKey, normalizedStoredUrl);
    }

    public string Build(string relativePath)
    {
        var currentBaseUrl = BaseUrl;
        if (string.IsNullOrWhiteSpace(relativePath))
        {
            return currentBaseUrl;
        }

        if (Uri.TryCreate(relativePath, UriKind.Absolute, out var absoluteUri) &&
            IsHttpOrHttps(absoluteUri))
        {
            return absoluteUri.ToString();
        }

        var normalizedRelativePath = relativePath.StartsWith('/') ? relativePath : $"/{relativePath}";
        return $"{currentBaseUrl}{normalizedRelativePath}";
    }

    public bool TryUpdateBaseUrl(string newBaseUrl)
    {
        var normalizedBaseUrl = NormalizeBaseUrl(newBaseUrl);
        lock (_sync)
        {
            if (string.Equals(_baseUrl, normalizedBaseUrl, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            _baseUrl = normalizedBaseUrl;
        }

        Preferences.Default.Set(BackendUrlPreferenceKey, normalizedBaseUrl);
        return true;
    }

    private static string NormalizeBaseUrl(string? baseUrl)
    {
        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            return DefaultBackendUrl;
        }

        var trimmed = baseUrl.Trim().TrimEnd('/');
        if (Uri.TryCreate(trimmed, UriKind.Absolute, out var parsedUri))
        {
            if (IsHttpOrHttps(parsedUri))
            {
                return parsedUri.ToString().TrimEnd('/');
            }
        }

        return DefaultBackendUrl;
    }

    private static bool IsHttpOrHttps(Uri uri)
    {
        return uri.Scheme.Equals(Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase) ||
               uri.Scheme.Equals(Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase);
    }
}
