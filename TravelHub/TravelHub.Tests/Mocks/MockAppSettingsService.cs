using App.Models;
using App.Services.Interfaces;

namespace TravelHub.Tests.Mocks;

public class MockAppSettingsService : IAppSettingsService
{
    public Country? CurrentCountry { get; set; } = new Country
    {
        Code = "CO",
        Name = "Colombia",
        PhoneCode = "+57",
        FlagEmoji = "🇨🇴"
    };
}