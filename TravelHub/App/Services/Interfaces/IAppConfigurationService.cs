namespace App.Services.Interfaces;

public interface IAppConfigurationService
{
    Task RefreshBackendUrlAsync();
}
