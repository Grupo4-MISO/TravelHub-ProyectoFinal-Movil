namespace App.Services.Interfaces;

public interface IAppConfigurationService
{
    Task RefreshBackendUrlAsync();
    Task<List<string>> GetPromotionalImagesAsync();
}
