using App.Providers.Interfaces;
using Microsoft.Maui.Storage;

namespace App.Providers.Implementations;

public class BackendUrlProvider : IBackendUrlProvider
{
    private const string BackendUrlPreferenceKey = "BackendUrl";
    private const string DefaultBackendUrl = "https://dpyrs6tuvj15e.cloudfront.net";
    private readonly object _sync = new();
    private string _baseUrl;
    public event EventHandler<string>? BaseUrlChanged;

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
        var storedUrl = Preferences.Default.Get(BackendUrlPreferenceKey, DefaultBackendUrl);
        var normalizedStoredUrl = NormalizeBaseUrl(storedUrl);
        _baseUrl = normalizedStoredUrl;
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
        var hasChanged = false;
        lock (_sync)
        {
            if (string.Equals(_baseUrl, normalizedBaseUrl, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            _baseUrl = normalizedBaseUrl;
            hasChanged = true;
        }

        Preferences.Default.Set(BackendUrlPreferenceKey, normalizedBaseUrl);
        if (hasChanged)
        {
            BaseUrlChanged?.Invoke(this, normalizedBaseUrl);
        }
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
