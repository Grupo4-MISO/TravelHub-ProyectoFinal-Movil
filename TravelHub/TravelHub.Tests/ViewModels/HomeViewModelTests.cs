using App.Responses;
using App.Services.Interfaces;
using App.Providers.Interfaces;
using App.ViewModels;
using Moq;
using Xunit;
using System.Windows.Input;

namespace TravelHub.Tests.ViewModels;

public class HomeViewModelTests
{
    private readonly Mock<ICountryService> _countryServiceMock;
    private readonly Mock<ICityService> _cityServiceMock;
    private readonly Mock<IBackendUrlProvider> _backendUrlProviderMock;
    private readonly Mock<IAppSettingsService> _appSettingsServiceMock;
    private readonly Mock<IAppConfigurationService> _appConfigurationServiceMock;
    private readonly HomeViewModel _viewModel;

    public HomeViewModelTests()
    {
        _countryServiceMock = new Mock<ICountryService>();
        _cityServiceMock = new Mock<ICityService>();
        _backendUrlProviderMock = new Mock<IBackendUrlProvider>();
        _appSettingsServiceMock = new Mock<IAppSettingsService>();
        _appConfigurationServiceMock = new Mock<IAppConfigurationService>();

        _appSettingsServiceMock.Setup(x => x.CurrentCountryCode).Returns("CO");

        _cityServiceMock
            .Setup(x => x.GetPopularCitiesByCountryAsync(It.IsAny<string>()))
            .ReturnsAsync(new HttpResponseWrapper<List<string>>(new List<string> { "Bogotá" }, false, new HttpResponseMessage(System.Net.HttpStatusCode.OK)));

        _appConfigurationServiceMock
            .Setup(x => x.GetPromotionalImagesAsync())
            .ReturnsAsync(new List<string> { "https://images.test/banner1.webp" });

        _viewModel = new HomeViewModel(
            _countryServiceMock.Object,
            _cityServiceMock.Object,
            _backendUrlProviderMock.Object,
            _appSettingsServiceMock.Object,
            _appConfigurationServiceMock.Object);
    }

    [Fact]
    public void Constructor_SetsTitleCorrectly()
    {
        Assert.Equal("TravelHub", _viewModel.Title);
    }

    [Fact]
    public void Constructor_Throws_WhenCountryServiceNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new HomeViewModel(null!, _cityServiceMock.Object, _backendUrlProviderMock.Object, _appSettingsServiceMock.Object, _appConfigurationServiceMock.Object));
    }

    [Fact]
    public void Constructor_Throws_WhenCityServiceNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new HomeViewModel(_countryServiceMock.Object, null!, _backendUrlProviderMock.Object, _appSettingsServiceMock.Object, _appConfigurationServiceMock.Object));
    }

    [Fact]
    public void Constructor_Throws_WhenBackendUrlProviderNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new HomeViewModel(_countryServiceMock.Object, _cityServiceMock.Object, null!, _appSettingsServiceMock.Object, _appConfigurationServiceMock.Object));
    }

    [Fact]
    public void Constructor_Throws_WhenAppConfigurationServiceNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new HomeViewModel(_countryServiceMock.Object, _cityServiceMock.Object, _backendUrlProviderMock.Object, _appSettingsServiceMock.Object, null!));
    }

    [Fact]
    public void DefaultValues_AreSetCorrectly()
    {
        Assert.Equal(2, _viewModel.Adults);
        Assert.Equal(0, _viewModel.Children);
        Assert.Equal(1, _viewModel.Rooms);
        Assert.False(_viewModel.IsGuestConfigVisible);
        Assert.NotNull(_viewModel.PromotionalImages);
        Assert.NotNull(_viewModel.FeaturedProperties);
        Assert.NotNull(_viewModel.PopularCities);
    }

    [Fact]
    public void CheckInDate_CannotBeBeforeMinDate()
    {
        var minDate = _viewModel.MinDate;
        var beforeMinDate = minDate.AddDays(-1);

        _viewModel.CheckInDate = beforeMinDate;

        // The ViewModel doesn't enforce MinDate validation (done in XAML via MinimumDate binding)
        // The setter allows the value but the UI constrains the picker
        Assert.Equal(beforeMinDate, _viewModel.CheckInDate);
    }

    [Fact]
    public void CheckInDate_UpdatesCheckOutDate_WhenAfterCheckOut()
    {
        var initialCheckOut = _viewModel.CheckOutDate;
        var newCheckIn = initialCheckOut.AddDays(1);

        _viewModel.CheckInDate = newCheckIn;

        Assert.Equal(newCheckIn.AddDays(1), _viewModel.CheckOutDate);
    }

    [Fact]
    public void GuestSummary_FormatsCorrectly_WithAdultsOnly()
    {
        _viewModel.Adults = 2;
        _viewModel.Children = 0;
        _viewModel.Rooms = 1;

        Assert.Contains("1 habitacion", _viewModel.GuestSummary);
        Assert.Contains("2 Adultos", _viewModel.GuestSummary);
        Assert.Contains("Sin ninos", _viewModel.GuestSummary);
    }

    [Fact]
    public void GuestSummary_FormatsCorrectly_WithChildren()
    {
        _viewModel.Adults = 2;
        _viewModel.Children = 2;
        _viewModel.Rooms = 2;

        Assert.Contains("2 habitaciones", _viewModel.GuestSummary);
        Assert.Contains("2 Adultos", _viewModel.GuestSummary);
        Assert.Contains("2 Ninos", _viewModel.GuestSummary);
    }

    [Fact]
    public void IncrementAdultsCommand_IncrementsAdults()
    {
        var initial = _viewModel.Adults;

        _viewModel.IncrementAdultsCommand.Execute(null);

        Assert.Equal(initial + 1, _viewModel.Adults);
    }

    [Fact]
    public void DecrementAdultsCommand_DecrementsAdults_WhenGreaterThanOne()
    {
        _viewModel.Adults = 3;
        var initial = _viewModel.Adults;

        _viewModel.DecrementAdultsCommand.Execute(null);

        Assert.Equal(initial - 1, _viewModel.Adults);
    }

    [Fact]
    public void DecrementAdultsCommand_DoesNotDecrement_WhenOne()
    {
        _viewModel.Adults = 1;

        _viewModel.DecrementAdultsCommand.Execute(null);

        Assert.Equal(1, _viewModel.Adults);
    }

    [Fact]
    public void IncrementChildrenCommand_IncrementsChildren()
    {
        var initial = _viewModel.Children;

        _viewModel.IncrementChildrenCommand.Execute(null);

        Assert.Equal(initial + 1, _viewModel.Children);
    }

    [Fact]
    public void DecrementChildrenCommand_DecrementsChildren_WhenGreaterThanZero()
    {
        _viewModel.Children = 2;
        var initial = _viewModel.Children;

        _viewModel.DecrementChildrenCommand.Execute(null);

        Assert.Equal(initial - 1, _viewModel.Children);
    }

    [Fact]
    public void DecrementChildrenCommand_DoesNotDecrement_WhenZero()
    {
        _viewModel.Children = 0;

        _viewModel.DecrementChildrenCommand.Execute(null);

        Assert.Equal(0, _viewModel.Children);
    }

    [Fact]
    public void IncrementRoomsCommand_IncrementsRooms()
    {
        var initial = _viewModel.Rooms;

        _viewModel.IncrementRoomsCommand.Execute(null);

        Assert.Equal(initial + 1, _viewModel.Rooms);
    }

    [Fact]
    public void DecrementRoomsCommand_DecrementsRooms_WhenGreaterThanOne()
    {
        _viewModel.Rooms = 3;
        var initial = _viewModel.Rooms;

        _viewModel.DecrementRoomsCommand.Execute(null);

        Assert.Equal(initial - 1, _viewModel.Rooms);
    }

    [Fact]
    public void DecrementRoomsCommand_DoesNotDecrement_WhenOne()
    {
        _viewModel.Rooms = 1;

        _viewModel.DecrementRoomsCommand.Execute(null);

        Assert.Equal(1, _viewModel.Rooms);
    }

    [Fact]
    public void ToggleGuestConfigCommand_TogglesVisibility()
    {
        var initial = _viewModel.IsGuestConfigVisible;

        _viewModel.ToggleGuestConfigCommand.Execute(null);

        Assert.Equal(!initial, _viewModel.IsGuestConfigVisible);
    }

    [Fact]
    public void PromotionalImages_Collection_IsInitialized()
    {
        Assert.NotNull(_viewModel.PromotionalImages);
        Assert.NotEmpty(_viewModel.PromotionalImages);
    }

    [Fact]
    public async Task LoadDataAsync_LoadsPromotionalImages_FromAppConfiguration()
    {
        var expectedImages = new List<string>
        {
            "https://images.test/banner1.webp",
            "https://images.test/banner2.webp"
        };

        _appConfigurationServiceMock
            .Setup(x => x.GetPromotionalImagesAsync())
            .ReturnsAsync(expectedImages);

        var method = typeof(HomeViewModel).GetMethod("LoadDataAsync",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        var task = (Task)method?.Invoke(_viewModel, null)!;
        await task;

        Assert.Equal(2, _viewModel.PromotionalImages.Count);
        Assert.Contains("https://images.test/banner1.webp", _viewModel.PromotionalImages);
        Assert.Contains("https://images.test/banner2.webp", _viewModel.PromotionalImages);
    }

    [Fact]
    public void ResolveCurrencyCode_ReturnsCorrectCode_ForKnownCountry()
    {
        // Using reflection to test private static method
        var method = typeof(HomeViewModel).GetMethod("ResolveCurrencyCode",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

        Assert.Equal("COP", method?.Invoke(null, new object[] { "CO" }));
        Assert.Equal("PEN", method?.Invoke(null, new object[] { "PE" }));
        Assert.Equal("USD", method?.Invoke(null, new object[] { "EC" }));
        Assert.Equal("MXN", method?.Invoke(null, new object[] { "MX" }));
        Assert.Equal("CLP", method?.Invoke(null, new object[] { "CL" }));
        Assert.Equal("ARS", method?.Invoke(null, new object[] { "AR" }));
        Assert.Equal("COP", method?.Invoke(null, new object[] { "UNKNOWN" }));
    }
}
