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

    public async Task<List<string>> GetPromotionalImagesAsync()
    {
        var url = _backendUrlProvider.Build("/api/v1/inventarios/promotional-images");
        var response = await _backEndService.GetAsync<List<string>>(url);

        if (response.Error || response.Response == null || response.Response.Count == 0)
        {
            return
            [
                "https://www.centraldevacaciones.com/blog/wp-content/uploads/2015/07/EspecialWamos2FB.jpg",
                "https://www.viajesexito.com/wp-content/uploads/2025/07/banner-principal-mobile-2.webp",
                "https://www.latamairlines.com/content/dam/latamxp/sites/sh-ofertas/paquetes-promov3.png"
            ];
        }

        return response.Response;
    }
}
