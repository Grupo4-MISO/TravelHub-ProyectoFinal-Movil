using App.Providers.Interfaces;
using App.Responses;
using App.Services.Implementations;
using App.Services.Interfaces;
using Moq;
using System.Net;
using Xunit;

namespace TravelHub.Tests.Services;

public class AppConfigurationServiceTests
{
    private readonly Mock<IBackEndService> _backEndServiceMock;
    private readonly Mock<IBackendUrlProvider> _backendUrlProviderMock;
    private readonly AppConfigurationService _service;

    public AppConfigurationServiceTests()
    {
        _backEndServiceMock = new Mock<IBackEndService>();
        _backendUrlProviderMock = new Mock<IBackendUrlProvider>();
        _service = new AppConfigurationService(_backEndServiceMock.Object, _backendUrlProviderMock.Object);
    }

    [Fact]
    public void Constructor_Throws_WhenBackEndServiceNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new AppConfigurationService(null!, _backendUrlProviderMock.Object));
    }

    [Fact]
    public void Constructor_Throws_WhenBackendUrlProviderNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new AppConfigurationService(_backEndServiceMock.Object, null!));
    }

    [Fact]
    public async Task GetPromotionalImagesAsync_ReturnsImages_WhenApiSucceeds()
    {
        var expectedImages = new List<string>
        {
            "https://images.test/banner1.webp",
            "https://images.test/banner2.webp"
        };

        _backendUrlProviderMock
            .Setup(x => x.Build("/api/v1/inventarios/promotional-images"))
            .Returns("https://api.test.com/api/v1/inventarios/promotional-images");

        _backEndServiceMock
            .Setup(x => x.GetAsync<List<string>>(It.IsAny<string>()))
            .ReturnsAsync(new HttpResponseWrapper<List<string>>(expectedImages, false, new HttpResponseMessage(HttpStatusCode.OK)));

        var result = await _service.GetPromotionalImagesAsync();

        Assert.Equal(2, result.Count);
        Assert.Contains("https://images.test/banner1.webp", result);
        Assert.Contains("https://images.test/banner2.webp", result);
    }

    [Fact]
    public async Task GetPromotionalImagesAsync_ReturnsFallbackImages_WhenApiFails()
    {
        _backendUrlProviderMock
            .Setup(x => x.Build("/api/v1/inventarios/promotional-images"))
            .Returns("https://api.test.com/api/v1/inventarios/promotional-images");

        _backEndServiceMock
            .Setup(x => x.GetAsync<List<string>>(It.IsAny<string>()))
            .ReturnsAsync(new HttpResponseWrapper<List<string>>(null!, true, new HttpResponseMessage(HttpStatusCode.InternalServerError)));

        var result = await _service.GetPromotionalImagesAsync();

        Assert.Equal(3, result.Count);
        Assert.Contains("https://www.centraldevacaciones.com/blog/wp-content/uploads/2015/07/EspecialWamos2FB.jpg", result);
    }

    [Fact]
    public async Task GetPromotionalImagesAsync_ReturnsFallbackImages_WhenApiReturnsEmptyList()
    {
        _backendUrlProviderMock
            .Setup(x => x.Build("/api/v1/inventarios/promotional-images"))
            .Returns("https://api.test.com/api/v1/inventarios/promotional-images");

        _backEndServiceMock
            .Setup(x => x.GetAsync<List<string>>(It.IsAny<string>()))
            .ReturnsAsync(new HttpResponseWrapper<List<string>>(new List<string>(), false, new HttpResponseMessage(HttpStatusCode.OK)));

        var result = await _service.GetPromotionalImagesAsync();

        Assert.Equal(3, result.Count);
    }

    [Fact]
    public async Task GetPromotionalImagesAsync_ReturnsFallbackImages_WhenApiReturnsNull()
    {
        _backendUrlProviderMock
            .Setup(x => x.Build("/api/v1/inventarios/promotional-images"))
            .Returns("https://api.test.com/api/v1/inventarios/promotional-images");

        _backEndServiceMock
            .Setup(x => x.GetAsync<List<string>>(It.IsAny<string>()))
            .ReturnsAsync(new HttpResponseWrapper<List<string>>(null!, false, new HttpResponseMessage(HttpStatusCode.OK)));

        var result = await _service.GetPromotionalImagesAsync();

        Assert.Equal(3, result.Count);
    }

    [Fact]
    public async Task GetPromotionalImagesAsync_UsesCorrectEndpoint()
    {
        string? capturedUrl = null;

        _backendUrlProviderMock
            .Setup(x => x.Build(It.IsAny<string>()))
            .Returns((string path) => $"https://api.test.com{path}");

        _backEndServiceMock
            .Setup(x => x.GetAsync<List<string>>(It.IsAny<string>()))
            .Callback<string>(url => capturedUrl = url)
            .ReturnsAsync(new HttpResponseWrapper<List<string>>(new List<string> { "img.jpg" }, false, new HttpResponseMessage(HttpStatusCode.OK)));

        await _service.GetPromotionalImagesAsync();

        Assert.Contains("/api/v1/inventarios/promotional-images", capturedUrl);
    }
}
