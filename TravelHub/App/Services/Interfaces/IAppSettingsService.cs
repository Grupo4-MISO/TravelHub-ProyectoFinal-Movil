using App.Models;

namespace App.Services.Interfaces;

public interface IAppSettingsService
{
    Country? CurrentCountry { get; }
}
