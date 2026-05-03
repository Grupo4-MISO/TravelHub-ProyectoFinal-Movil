using App.Services.Implementations;
using App.Services.Interfaces;
using App.ViewModels;
using Moq;
using Xunit;

namespace TravelHub.Tests.ViewModels;

public class SettingsViewModelTests
{
    private readonly Mock<IAccessibilityService> _accessibilityServiceMock;
    private readonly SettingsViewModel _viewModel;

    public SettingsViewModelTests()
    {
        _accessibilityServiceMock = new Mock<IAccessibilityService>();
        _accessibilityServiceMock.Setup(x => x.DarkModeEnabled).Returns(false);
        _accessibilityServiceMock.Setup(x => x.ColorBlindnessMode).Returns("none");
        _accessibilityServiceMock.Setup(x => x.TextSizeScale).Returns(1.0);

        _viewModel = new SettingsViewModel(_accessibilityServiceMock.Object);
    }

    [Fact]
    public void Constructor_Throws_WhenAccessibilityServiceNull()
    {
        Assert.Throws<ArgumentNullException>(() => new SettingsViewModel(null!));
    }

    [Fact]
    public void Constructor_SetsPropertiesFromService()
    {
        // Arrange
        _accessibilityServiceMock.Setup(x => x.DarkModeEnabled).Returns(true);
        _accessibilityServiceMock.Setup(x => x.ColorBlindnessMode).Returns("protanopia");
        _accessibilityServiceMock.Setup(x => x.TextSizeScale).Returns(1.5);

        // Act
        var viewModel = new SettingsViewModel(_accessibilityServiceMock.Object);

        // Assert
        Assert.True(viewModel.DarkModeEnabled);
        Assert.Equal("protanopia", viewModel.ColorBlindnessMode);
        Assert.Equal(1.5, viewModel.TextSizeScale);
    }

    [Fact]
    public void DarkModeEnabled_Set_CallsService()
    {
        // Act
        _viewModel.DarkModeEnabled = true;

        // Assert
        _accessibilityServiceMock.VerifySet(x => x.DarkModeEnabled = true, Times.Once());
    }

    [Fact]
    public void ColorBlindnessMode_Set_CallsService()
    {
        // Act
        _viewModel.ColorBlindnessMode = "deuteranopia";

        // Assert
        _accessibilityServiceMock.VerifySet(x => x.ColorBlindnessMode = "deuteranopia", Times.Once());
    }

    [Fact]
    public void TextSizeScale_Set_CallsService()
    {
        // Act
        _viewModel.TextSizeScale = 2.0;

        // Assert
        _accessibilityServiceMock.VerifySet(x => x.TextSizeScale = 2.0, Times.Once());
    }

    [Fact]
    public void ColorBlindnessOptions_ReturnsCorrectOptions()
    {
        // Act
        var options = _viewModel.ColorBlindnessOptions;

        // Assert
        Assert.Contains("none", options);
        Assert.Contains("protanopia", options);
        Assert.Contains("deuteranopia", options);
        Assert.Contains("tritanopia", options);
        Assert.Equal(4, options.Count);
    }

    [Fact]
    public void TextSizeOptions_ReturnsCorrectOptions()
    {
        // Act
        var options = _viewModel.TextSizeOptions;

        // Assert
        Assert.Contains(1.0, options);
        Assert.Contains(1.25, options);
        Assert.Contains(1.5, options);
        Assert.Contains(2.0, options);
        Assert.Equal(4, options.Count);
    }

    [Fact]
    public void RestoreDefaultsCommand_CallsService()
    {
        // Act
        _viewModel.RestoreDefaultsCommand?.Execute(null);

        // Assert - verify that the properties were set to defaults
        _accessibilityServiceMock.VerifySet(x => x.DarkModeEnabled = false, Times.Once());
        _accessibilityServiceMock.VerifySet(x => x.ColorBlindnessMode = "none", Times.Once());
        _accessibilityServiceMock.VerifySet(x => x.TextSizeScale = 1.0, Times.Once());
    }
}
