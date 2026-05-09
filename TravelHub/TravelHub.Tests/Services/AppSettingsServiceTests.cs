using App.Models;
using App.Services.Implementations;
using App.Services.Interfaces;
using Moq;
using Xunit;

namespace TravelHub.Tests.Services;

public class AppSettingsServiceTests
{
    private readonly Mock<IPreferencesService> _preferencesMock;
    private readonly Mock<ICountryService> _countryServiceMock;
    private readonly AppSettingsService _service;

    public AppSettingsServiceTests()
    {
        _preferencesMock = new Mock<IPreferencesService>();
        _countryServiceMock = new Mock<ICountryService>();

        _preferencesMock
            .Setup(x => x.Get(It.IsAny<string>(), It.IsAny<string>()))
            .Returns("CO");

        _service = new AppSettingsService(_preferencesMock.Object, _countryServiceMock.Object);
    }

    [Fact]
    public void Constructor_Throws_WhenPreferencesNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new AppSettingsService(null!, _countryServiceMock.Object));
    }

    [Fact]
    public void Constructor_Throws_WhenCountryServiceNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new AppSettingsService(_preferencesMock.Object, null!));
    }

    [Fact]
    public void Constructor_LoadsDefaultCountryCode_FromPreferences()
    {
        _preferencesMock
            .Setup(x => x.Get("SelectedCountryCode", "CO"))
            .Returns("AR");

        var service = new AppSettingsService(_preferencesMock.Object, _countryServiceMock.Object);

        Assert.Equal("AR", service.CurrentCountryCode);
    }

    [Fact]
    public void Constructor_UsesDefaultCountryCode_WhenPreferencesEmpty()
    {
        _preferencesMock
            .Setup(x => x.Get("SelectedCountryCode", "CO"))
            .Returns("CO");

        var service = new AppSettingsService(_preferencesMock.Object, _countryServiceMock.Object);

        Assert.Equal("CO", service.CurrentCountryCode);
    }

    [Fact]
    public void CurrentCountry_ReturnsNull_WhenCountryServiceReturnsNull()
    {
        _countryServiceMock
            .Setup(x => x.GetCountryByCode("CO"))
            .Returns((Country?)null);

        var result = _service.CurrentCountry;

        Assert.Null(result);
    }

    [Fact]
    public void CurrentCountry_ReturnsCountry_FromCountryService()
    {
        var expectedCountry = new Country
        {
            Id = "co-1",
            Code = "CO",
            Name = "Colombia",
            CurrencyCode = "COP",
            CurrencySymbol = "$",
            PhoneCode = "+57"
        };

        _countryServiceMock
            .Setup(x => x.GetCountryByCode("CO"))
            .Returns(expectedCountry);

        var result = _service.CurrentCountry;

        Assert.NotNull(result);
        Assert.Equal("Colombia", result.Name);
        Assert.Equal("CO", result.Code);
        Assert.Equal("COP", result.CurrencyCode);
    }

    [Fact]
    public void CurrentCountry_CachesCountry_AfterFirstAccess()
    {
        var country = new Country
        {
            Id = "co-1",
            Code = "CO",
            Name = "Colombia",
            CurrencyCode = "COP",
            CurrencySymbol = "$",
            PhoneCode = "+57"
        };

        _countryServiceMock
            .Setup(x => x.GetCountryByCode("CO"))
            .Returns(country);

        var firstResult = _service.CurrentCountry;
        var secondResult = _service.CurrentCountry;

        Assert.Same(firstResult, secondResult);
        _countryServiceMock.Verify(x => x.GetCountryByCode("CO"), Times.Once);
    }

    [Fact]
    public void CurrentCountry_ReFetches_WhenCountryCodeChanges()
    {
        var colombia = new Country { Id = "co-1", Code = "CO", Name = "Colombia", CurrencyCode = "COP" };
        var argentina = new Country { Id = "ar-1", Code = "AR", Name = "Argentina", CurrencyCode = "ARS" };

        _countryServiceMock
            .Setup(x => x.GetCountryByCode("CO"))
            .Returns(colombia);

        _countryServiceMock
            .Setup(x => x.GetCountryByCode("AR"))
            .Returns(argentina);

        var firstResult = _service.CurrentCountry;
        Assert.Equal("Colombia", firstResult.Name);

        _service.CurrentCountryCode = "AR";

        var secondResult = _service.CurrentCountry;
        Assert.Equal("Argentina", secondResult.Name);
        _countryServiceMock.Verify(x => x.GetCountryByCode("CO"), Times.Once);
        _countryServiceMock.Verify(x => x.GetCountryByCode("AR"), Times.Once);
    }

    [Fact]
    public void SetCountry_UpdatesCountryCode_AndClearsCache()
    {
        var colombia = new Country { Id = "co-1", Code = "CO", Name = "Colombia", CurrencyCode = "COP" };
        var argentina = new Country { Id = "ar-1", Code = "AR", Name = "Argentina", CurrencyCode = "ARS" };

        _countryServiceMock
            .Setup(x => x.GetCountryByCode("CO"))
            .Returns(colombia);

        _countryServiceMock
            .Setup(x => x.GetCountryByCode("AR"))
            .Returns(argentina);

        var before = _service.CurrentCountry;
        Assert.Equal("Colombia", before.Name);

        _service.SetCountry("AR");

        Assert.Equal("AR", _service.CurrentCountryCode);

        var after = _service.CurrentCountry;
        Assert.Equal("Argentina", after.Name);
    }

    [Fact]
    public void CountryChanged_Fires_WhenCountryCodeChanges()
    {
        var eventFired = false;
        string? newCode = null;

        _service.CountryChanged += (_, code) =>
        {
            eventFired = true;
            newCode = code;
        };

        _service.CurrentCountryCode = "PE";

        Assert.True(eventFired);
        Assert.Equal("PE", newCode);
    }

    [Fact]
    public void CountryChanged_DoesNotFire_WhenCountryCodeUnchanged()
    {
        var eventFired = false;

        _service.CountryChanged += (_, _) => eventFired = true;

        _service.CurrentCountryCode = "CO";

        Assert.False(eventFired);
    }

    [Fact]
    public void SetCountry_FiresCountryChanged()
    {
        var eventFired = false;
        string? newCode = null;

        _service.CountryChanged += (_, code) =>
        {
            eventFired = true;
            newCode = code;
        };

        _service.SetCountry("MX");

        Assert.True(eventFired);
        Assert.Equal("MX", newCode);
    }

    [Fact]
    public void CurrentVersion_GetSet_WorksCorrectly()
    {
        _preferencesMock
            .Setup(x => x.Get("CurrentVersion", "1.0"))
            .Returns("2.5");

        var version = _service.CurrentVersion;

        Assert.Equal("2.5", version);

        _service.CurrentVersion = "3.0";
        _preferencesMock.Verify(x => x.Set("CurrentVersion", "3.0"), Times.Once);
    }

    [Fact]
    public void SetCurrentVersion_SavesToPreferences()
    {
        _service.SetCurrentVersion("4.0");

        _preferencesMock.Verify(x => x.Set("CurrentVersion", "4.0"), Times.Once);
    }

    [Fact]
    public void CurrencyChanged_Fires_WhenCurrencyCodeChanges()
    {
        var eventFired = false;
        string? newCode = null;

        _service.CurrencyChanged += (_, code) =>
        {
            eventFired = true;
            newCode = code;
        };

        _service.CurrentCurrencyCode = "USD";

        Assert.True(eventFired);
        Assert.Equal("USD", newCode);
    }

    [Fact]
    public void CurrencyChanged_DoesNotFire_WhenCurrencyCodeUnchanged()
    {
        var eventFired = false;

        _service.CurrencyChanged += (_, _) => eventFired = true;

        var current = _service.CurrentCurrencyCode;
        _service.CurrentCurrencyCode = current;

        Assert.False(eventFired);
    }

    [Fact]
    public void SetCurrency_UpdatesCurrencyCode_AndFiresEvent()
    {
        var eventFired = false;
        string? newCode = null;

        _service.CurrencyChanged += (_, code) =>
        {
            eventFired = true;
            newCode = code;
        };

        _service.SetCurrency("MXN");

        Assert.True(eventFired);
        Assert.Equal("MXN", newCode);
    }

    [Fact]
    public void SetCurrency_DoesNotChangeCountryCode()
    {
        _service.SetCurrency("PEN");

        Assert.Equal("CO", _service.CurrentCountryCode);
    }

    [Fact]
    public void SetCountry_AlsoUpdatesCurrency_ToCountryCurrency()
    {
        var argentina = new Country
        {
            Id = "ar-1",
            Code = "AR",
            Name = "Argentina",
            CurrencyCode = "ARS",
            CurrencySymbol = "$",
            PhoneCode = "+54"
        };

        _countryServiceMock
            .Setup(x => x.GetCountryByCode("AR"))
            .Returns(argentina);

        _service.SetCountry("AR");

        Assert.Equal("AR", _service.CurrentCountryCode);
        Assert.Equal("ARS", _service.CurrentCurrencyCode);
    }

    [Fact]
    public void CurrentCurrencySymbol_ReturnsSymbol_FromCurrentCountry()
    {
        var colombia = new Country
        {
            Id = "co-1",
            Code = "CO",
            Name = "Colombia",
            CurrencyCode = "COP",
            CurrencySymbol = "$",
            PhoneCode = "+57"
        };

        _countryServiceMock
            .Setup(x => x.GetCountryByCode("CO"))
            .Returns(colombia);

        var symbol = _service.CurrentCurrencySymbol;

        Assert.Equal("$", symbol);
    }
}
