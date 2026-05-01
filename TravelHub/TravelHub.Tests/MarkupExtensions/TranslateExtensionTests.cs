using App.MarkupExtensions;
using App.Services.Interfaces;
using Moq;
using Xunit;

namespace TravelHub.Tests.MarkupExtensions
{
    public class TranslateExtensionTests
    {
        public TranslateExtensionTests()
        {
            // Initialize with mock for most tests
            var mockService = new Mock<ILocalizationService>();
            TranslateExtension.Initialize(mockService.Object);
        }

        [Fact]
        public void ProvideValue_ReturnsCorrectSpanishString()
        {
            // Arrange
            var mockService = new Mock<ILocalizationService>();
            mockService.Setup(x => x.GetString("Home_Buscar"))
                .Returns("Buscar");
            TranslateExtension.Initialize(mockService.Object);
            
            var translate = new TranslateExtension("Home_Buscar");

            // Act
            var result = translate.ProvideValue(null);

            // Assert
            Assert.Equal("Buscar", result);
        }

        [Fact]
        public void ProvideValue_ReturnsCorrectEnglishString()
        {
            // Arrange
            var mockService = new Mock<ILocalizationService>();
            mockService.Setup(x => x.GetString("Home_Buscar"))
                .Returns("Search");
            TranslateExtension.Initialize(mockService.Object);
            
            var translate = new TranslateExtension("Home_Buscar");

            // Act
            var result = translate.ProvideValue(null);

            // Assert
            Assert.Equal("Search", result);
        }

        [Fact]
        public void ProvideValue_HandlesNullKey()
        {
            // Arrange
            var translate = new TranslateExtension(null!);

            // Act
            var result = translate.ProvideValue(null);

            // Assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void ProvideValue_HandlesEmptyKey()
        {
            // Arrange
            var translate = new TranslateExtension(string.Empty);

            // Act
            var result = translate.ProvideValue(null);

            // Assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void ProvideValue_ReturnsKey_WhenLocalizationServiceIsNull()
        {
            // Arrange
            TranslateExtension.Initialize(null!);
            var translate = new TranslateExtension("Home_Buscar");

            // Act
            var result = translate.ProvideValue(null);

            // Assert - Should return the key when service is null
            Assert.Equal("Home_Buscar", result);
        }

        [Fact]
        public void Constructor_WithKey_SetsKeyProperty()
        {
            // Arrange & Act
            var translate = new TranslateExtension("Test_Key");

            // Assert
            Assert.Equal("Test_Key", translate.Key);
        }
    }
}
