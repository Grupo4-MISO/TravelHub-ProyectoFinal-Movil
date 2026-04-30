namespace App.Services.Interfaces;

public interface IAppInitializationService
{
    Task InitializeAsync();
    Task ClearDataAsync();
}
