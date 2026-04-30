using App.Models;
using App.Providers.Interfaces;
using App.Services.Interfaces;

namespace App.Services.Implementations;

public class AppConfigurationService : IAppConfigurationService
{
    private const string AppConfigEndpoint = "https://www.travelhub.mediomasivo.com/api/config";

    private readonly IBackEndService _backEndService;
    private readonly IBackendUrlProvider _backendUrlProvider;

    public AppConfigurationService(IBackEndService backEndService, IBackendUrlProvider backendUrlProvider)
    {
        _backEndService = backEndService ?? throw new ArgumentNullException(nameof(backEndService));
        _backendUrlProvider = backendUrlProvider ?? throw new ArgumentNullException(nameof(backendUrlProvider));
    }

    public async Task RefreshBackendUrlAsync()
    {
        var response = await _backEndService.GetAsync<AppConfigResponse>(AppConfigEndpoint);
        if (response.Error || response.Response == null || string.IsNullOrWhiteSpace(response.Response.AppBackEnd))
        {
            return;
        }

        _backendUrlProvider.TryUpdateBaseUrl(response.Response.AppBackEnd);
    }
}
