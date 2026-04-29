namespace App.Providers.Interfaces;

public interface IBackendUrlProvider
{
    string BaseUrl { get; }
    event EventHandler<string>? BaseUrlChanged;
    string Build(string relativePath);
    bool TryUpdateBaseUrl(string newBaseUrl);
}
