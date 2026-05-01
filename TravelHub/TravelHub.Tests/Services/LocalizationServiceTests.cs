using App.Services.Interfaces;
using App.Services.Implementations;
using Moq;
using Xunit;

namespace TravelHub.Tests.Services
{
    public class LocalizationServiceTests
    {
        private const string PreferenceKey = "App_Language";
        private readonly Mock<IPreferencesService> _mockPreferences;
        private readonly LocalizationService _localizationService;

        public LocalizationServiceTests()
        {
            _mockPreferences = new Mock<IPreferencesService>();
            _mockPreferences.Setup(x => x.Get<string>(PreferenceKey, "es")).Returns("es");
            _localizationService = new LocalizationService(_mockPreferences.Object);
        }

        [Fact]
        public void DefaultCulture_IsSpanish()
        {
            // Arrange & Act
            var culture = _localizationService.CurrentCulture;

            // Assert
            Assert.Equal("es", culture.Name);
        }

        [Fact]
        public void GetString_ReturnsSpanishString_WhenSpanishIsDefault()
        {
            // Arrange & Act
            var result = _localizationService.GetString("Home_Buscar");

            // Assert
            Assert.Equal("Buscar", result);
        }

        [Fact]
        public void GetString_ReturnsEnglishString_WhenLanguageIsEnglish()
        {
            // Arrange
            _localizationService.SetLanguage("en");

            // Act
            var result = _localizationService.GetString("Home_Buscar");

            // Assert
            Assert.Equal("Search", result);
        }

        [Fact]
        public void SetLanguage_ChangesCurrentCulture()
        {
            // Arrange & Act
            _localizationService.SetLanguage("en");

            // Assert
            Assert.Equal("en", _localizationService.CurrentCulture.Name);
        }

        [Fact]
        public void SetLanguage_RaisesLanguageChangedEvent()
        {
            // Arrange
            var eventRaised = false;
            _localizationService.LanguageChanged += (s, e) => eventRaised = true;

            // Act
            _localizationService.SetLanguage("en");

            // Assert
            Assert.True(eventRaised);
        }

        [Fact]
        public void GetString_ReturnsKey_WhenKeyNotFound()
        {
            // Arrange
            var nonExistentKey = "NonExistentKey";

            // Act
            var result = _localizationService.GetString(nonExistentKey);

            // Assert
            Assert.Equal(nonExistentKey, result);
        }

        [Fact]
        public void SetLanguage_PersistsPreference()
        {
            // Arrange
            _localizationService.SetLanguage("en");

            // Assert
            _mockPreferences.Verify(x => x.Set<string>(PreferenceKey, "en"), Times.Once);
        }

         [Fact]
         public void Constructor_LoadsSavedLanguage()
         {
             // Arrange
             _mockPreferences.Setup(x => x.Get<string>(PreferenceKey, "es")).Returns("en");

             // Act
             var newService = new LocalizationService(_mockPreferences.Object);

             // Assert
             Assert.Equal("en", newService.CurrentCulture.Name);
         }
     }
}
