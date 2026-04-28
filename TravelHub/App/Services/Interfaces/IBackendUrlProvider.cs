namespace App.Services.Interfaces;

public interface IBackendUrlProvider
{
    string BaseUrl { get; }
    string Build(string relativePath);
    bool TryUpdateBaseUrl(string newBaseUrl);
}
