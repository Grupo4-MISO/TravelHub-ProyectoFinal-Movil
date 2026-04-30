using App.Models;
using App.Responses;
using App.Services.Interfaces;
using App.Providers.Interfaces;
using App.ViewModels;
using Moq;
using Xunit;
using System.Windows.Input;

namespace TravelHub.Tests.ViewModels;

public class CountryViewModelTests
{
    private readonly Mock<ICountryService> _countryServiceMock;
    private readonly Mock<IBackendUrlProvider> _backendUrlProviderMock;
    private readonly Mock<IMainThreadService> _mainThreadServiceMock;
    private readonly Mock<IAppSettingsService> _appSettingsServiceMock;
    private readonly CountryViewModel _viewModel;

    public CountryViewModelTests()
    {
        _countryServiceMock = new Mock<ICountryService>();
        _backendUrlProviderMock = new Mock<IBackendUrlProvider>();
        _mainThreadServiceMock = new Mock<IMainThreadService>();
        _appSettingsServiceMock = new Mock<IAppSettingsService>();

        _mainThreadServiceMock.Setup(x => x.IsMainThread).Returns(true);
        _mainThreadServiceMock.Setup(x => x.BeginInvokeOnMainThread(It.IsAny<Action>()))
            .Callback<Action>(action => action());
        _appSettingsServiceMock.Setup(x => x.CurrentCountryCode).Returns("CO");

        // Mock countries response
        var countries = new List<Country>
        {
            new Country { Id = "1", Name = "Colombia", Code = "CO", CurrencyCode = "COP", CurrencySymbol = "$", FlagEmoji = "🇨🇴", PhoneCode = "+57" }
        };
        _countryServiceMock.Setup(x => x.GetCountriesAsync())
            .ReturnsAsync(new HttpResponseWrapper<List<Country>>(countries, false, new HttpResponseMessage(System.Net.HttpStatusCode.OK)));

        _viewModel = new CountryViewModel(
            _countryServiceMock.Object,
            _backendUrlProviderMock.Object,
            _mainThreadServiceMock.Object,
            _appSettingsServiceMock.Object);
    }

    [Fact]
    public void Constructor_Throws_WhenCountryServiceNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new CountryViewModel(null!, _backendUrlProviderMock.Object, _mainThreadServiceMock.Object, _appSettingsServiceMock.Object));
    }

    [Fact]
    public void Constructor_Throws_WhenBackendUrlProviderNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new CountryViewModel(_countryServiceMock.Object, null!, _mainThreadServiceMock.Object, _appSettingsServiceMock.Object));
    }

    [Fact]
    public void Constructor_Throws_WhenMainThreadServiceNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new CountryViewModel(_countryServiceMock.Object, _backendUrlProviderMock.Object, null!, _appSettingsServiceMock.Object));
    }

    [Fact]
    public void Constructor_Throws_WhenAppSettingsServiceNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new CountryViewModel(_countryServiceMock.Object, _backendUrlProviderMock.Object, _mainThreadServiceMock.Object, null!));
    }

    [Fact]
    public void DefaultValues_AreSetCorrectly()
    {
        Assert.NotNull(_viewModel.Countries);
        Assert.Single(_viewModel.Countries);
        Assert.False(_viewModel.IsLoading);
        Assert.Empty(_viewModel.ErrorMessage);
    }

    [Fact]
    public void IsLoading_PropertyChanged_RaisesNotification()
    {
        var propertyChangedRaised = false;
        _viewModel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(CountryViewModel.IsLoading))
                propertyChangedRaised = true;
        };

        _viewModel.IsLoading = true;

        Assert.True(propertyChangedRaised);
    }

    [Fact]
    public void ErrorMessage_PropertyChanged_RaisesNotification()
    {
        var propertyChangedRaised = false;
        _viewModel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(CountryViewModel.ErrorMessage))
                propertyChangedRaised = true;
        };

        _viewModel.ErrorMessage = "Test error";

        Assert.True(propertyChangedRaised);
    }

    [Fact]
    public void CountryItem_Constructor_SetsPropertiesCorrectly()
    {
        var country = new Country
        {
            Id = "1",
            Name = "Colombia",
            Code = "CO",
            CurrencyCode = "COP",
            CurrencySymbol = "$",
            FlagEmoji = "🇨🇴",
            PhoneCode = "+57"
        };

        var item = new CountryItem(country, true);

        Assert.Equal("1", item.Id);
        Assert.Equal("Colombia", item.Name);
        Assert.Equal("CO", item.Code);
        Assert.Equal("COP", item.CurrencyCode);
        Assert.Equal("$", item.CurrencySymbol);
        Assert.Equal("🇨🇴", item.FlagEmoji);
        Assert.Equal("+57", item.PhoneCode);
        Assert.True(item.IsSelected);
    }

    [Fact]
    public void CountryItem_IsSelected_PropertyChanged_RaisesNotification()
    {
        var country = new Country
        {
            Id = "1",
            Name = "Colombia",
            Code = "CO",
            CurrencyCode = "COP",
            CurrencySymbol = "$",
            FlagEmoji = "🇨🇴",
            PhoneCode = "+57"
        };

        var item = new CountryItem(country, false);
        var propertyChangedRaised = false;
        item.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(CountryItem.IsSelected))
                propertyChangedRaised = true;
        };

        item.IsSelected = true;

        Assert.True(propertyChangedRaised);
    }

    [Fact]
    public void CountryItem_NotSelected_ByDefault_WhenFalse()
    {
        var country = new Country
        {
            Id = "2",
            Name = "México",
            Code = "MX",
            CurrencyCode = "MXN",
            CurrencySymbol = "$",
            FlagEmoji = "🇲🇽",
            PhoneCode = "+52"
        };

        var item = new CountryItem(country, false);

        Assert.False(item.IsSelected);
    }
}
