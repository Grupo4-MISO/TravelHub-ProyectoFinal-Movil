# Ejemplos de Uso - Integración de Countries

## 1. Usando ICountryService en Otros ViewModels

### Ejemplo: SearchResultsViewModel

```csharp
using App.Services.Interfaces;
using App.Models;

namespace App.ViewModels;

public class SearchResultsViewModel : BaseViewModel
{
    private readonly ICountryService _countryService;
    private ObservableCollection<Property> _properties = [];
    private Country? _selectedCountry;

    public Country? SelectedCountry
    {
        get => _selectedCountry;
        set => SetProperty(ref _selectedCountry, value);
    }

    public ObservableCollection<Property> Properties
    {
        get => _properties;
        set => SetProperty(ref _properties, value);
    }

    public SearchResultsViewModel(ICountryService countryService)
    {
        _countryService = countryService ?? throw new ArgumentNullException(nameof(countryService));
        LoadSelectedCountry();
    }

    private void LoadSelectedCountry()
    {
        var code = AppSettingsService.Instance.CurrentCountryCode;
        SelectedCountry = _countryService.GetCountryByCode(code);

        if (SelectedCountry != null)
        {
            LoadPropertiesForCountry(SelectedCountry.Code);
        }
    }

    private void LoadPropertiesForCountry(string countryCode)
    {
        // Cargar propiedades para el país seleccionado
        // Usar countryCode para filtrar resultados del backend
    }
}
```

## 2. Obtener Moneda del País Actual

```csharp
// En cualquier parte del código:
var currentCountry = AppSettingsService.Instance.CurrentCountry;
if (currentCountry != null)
{
    string currencySymbol = currentCountry.CurrencySymbol; // "$"
    string currencyCode = currentCountry.CurrencyCode;     // "ARS"

    decimal price = 1000;
    string formatted = $"{currentCountry.CurrencySymbol} {price}"; // "$ 1000"
}
```

## 3. Obtener Ciudades Populares del País

```csharp
// Inyectar ICountryService
private readonly ICountryService _countryService;

public async Task LoadPopularCities()
{
    var code = AppSettingsService.Instance.CurrentCountryCode;
    var cities = _countryService.GetPopularCitiesByCountry(code);

    // Usar ciudades para búsqueda
    foreach (var city in cities)
    {
        Console.WriteLine(city); // Cartagena, Bogotá, Medellín, etc.
    }
}
```

## 4. Formato de Precio con Moneda

```csharp
public class PriceFormatter
{
    private readonly ICountryService _countryService;

    public PriceFormatter(ICountryService countryService)
    {
        _countryService = countryService;
    }

    public string FormatPrice(decimal amount, string countryCode)
    {
        var country = _countryService.GetCountryByCode(countryCode);
        if (country == null) 
            return $"${amount}";

        return $"{country.CurrencySymbol} {amount}";
    }

    // Uso:
    // var formatter = new PriceFormatter(countryService);
    // string price = formatter.FormatPrice(380000, "CO"); // "$ 380000"
}
```

## 5. Selector de País en XAML

### Picker Binding Example

```xaml
<Picker Title="Selecciona País"
        ItemsSource="{Binding Countries}"
        ItemDisplayBinding="{Binding Name}"
        SelectedItem="{Binding SelectedCountry}"
        SelectedItemChangedCommand="{Binding CountrySelectedCommand}">
</Picker>
```

### ViewModel

```csharp
public partial class SettingsViewModel : BaseViewModel
{
    private readonly ICountryService _countryService;
    private ObservableCollection<CountryItem> _countries = [];
    private CountryItem? _selectedCountry;

    public ObservableCollection<CountryItem> Countries
    {
        get => _countries;
        set => SetProperty(ref _countries, value);
    }

    public CountryItem? SelectedCountry
    {
        get => _selectedCountry;
        set
        {
            if (SetProperty(ref _selectedCountry, value))
            {
                if (value != null)
                    OnCountryChanged(value);
            }
        }
    }

    public SettingsViewModel(ICountryService countryService)
    {
        _countryService = countryService;
        InitializeCountries();
    }

    private async void InitializeCountries()
    {
        var response = await _countryService.GetCountriesAsync();
        if (!response.Error && response.Response != null)
        {
            foreach (var country in response.Response)
            {
                Countries.Add(new CountryItem(country));
            }
        }
    }

    private void OnCountryChanged(CountryItem country)
    {
        AppSettingsService.Instance.SetCountry(country.Code);
    }
}
```

## 6. Manejo de Errores Completo

```csharp
public async Task LoadCountriesWithErrorHandling()
{
    try
    {
        IsLoading = true;
        ErrorMessage = string.Empty;

        // Verificar conexión primero
        var connection = _backEndService.CheckConnection();
        if (!connection.IsSuccess)
        {
            ErrorMessage = "No hay conexión a internet. Verifica tu conexión.";
            return;
        }

        // Obtener países
        var response = await _countryService.GetCountriesAsync();

        if (response.Error)
        {
            if (response.HttpResponseMessage?.StatusCode == System.Net.HttpStatusCode.RequestTimeout)
            {
                ErrorMessage = "Tiempo de espera agotado. Intenta de nuevo.";
            }
            else if (response.HttpResponseMessage?.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable)
            {
                ErrorMessage = "Servidor no disponible. Intenta más tarde.";
            }
            else
            {
                ErrorMessage = "Error al cargar los países. Intenta de nuevo.";
            }
            return;
        }

        if (response.Response == null || response.Response.Count == 0)
        {
            ErrorMessage = "No hay países disponibles.";
            return;
        }

        // Cargar datos exitosamente
        PopulateCountries(response.Response);
    }
    catch (HttpRequestException ex)
    {
        ErrorMessage = $"Error de red: {ex.Message}";
        Debug.WriteLine($"NetworkError: {ex}");
    }
    catch (TaskCanceledException ex)
    {
        ErrorMessage = "La solicitud fue cancelada. Intenta de nuevo.";
        Debug.WriteLine($"Timeout: {ex}");
    }
    catch (Exception ex)
    {
        ErrorMessage = "Error inesperado. Intenta de nuevo.";
        Debug.WriteLine($"UnexpectedError: {ex}");
    }
    finally
    {
        IsLoading = false;
    }
}

private void PopulateCountries(List<Country> countries)
{
    Countries.Clear();
    var currentCode = AppSettingsService.Instance.CurrentCountryCode;

    foreach (var country in countries.OrderBy(c => c.Name))
    {
        Countries.Add(new CountryItem(country, country.Code == currentCode));
    }
}
```

## 7. Caché Personalizado

```csharp
public class CachedCountryService : ICountryService
{
    private readonly ICountryService _innerService;
    private DateTime _lastFetch = DateTime.MinValue;
    private const int CacheMinutesExpiry = 60;

    public CachedCountryService(ICountryService innerService)
    {
        _innerService = innerService;
    }

    public async Task<HttpResponseWrapper<List<Country>>> GetCountriesAsync()
    {
        // Limpiar caché si expiró
        if ((DateTime.Now - _lastFetch).TotalMinutes > CacheMinutesExpiry)
        {
            var response = await _innerService.GetCountriesAsync();
            _lastFetch = DateTime.Now;
            return response;
        }

        // Devolver caché si está fresco
        return await _innerService.GetCountriesAsync();
    }

    public Country? GetCountryByCode(string code)
        => _innerService.GetCountryByCode(code);

    public Country? GetCountryById(string id)
        => _innerService.GetCountryById(id);

    public List<string> GetPopularCitiesByCountry(string countryCode)
        => _innerService.GetPopularCitiesByCountry(countryCode);
}

// Uso en MauiProgram.cs:
// var countryService = new CountryService(backEndService);
// builder.Services.AddSingleton<ICountryService>(
//     new CachedCountryService(countryService)
// );
```

## 8. Tests Unitarios

```csharp
using Xunit;
using Moq;

public class CountryServiceTests
{
    [Fact]
    public async Task GetCountriesAsync_WithValidResponse_ReturnsCountries()
    {
        // Arrange
        var mockBackEnd = new Mock<IBackEndService>();
        var countries = new List<Country>
        {
            new Country { Id = "1", Name = "Colombia", Code = "CO" }
        };
        var response = new HttpResponseWrapper<List<Country>>(countries, false, null!);

        mockBackEnd
            .Setup(x => x.GetAsync<List<Country>>(It.IsAny<string>()))
            .ReturnsAsync(response);

        var service = new CountryService(mockBackEnd.Object);

        // Act
        var result = await service.GetCountriesAsync();

        // Assert
        Assert.False(result.Error);
        Assert.Single(result.Response);
        Assert.Equal("Colombia", result.Response[0].Name);
    }

    [Fact]
    public async Task GetCountriesAsync_WithError_ReturnsError()
    {
        // Arrange
        var mockBackEnd = new Mock<IBackEndService>();
        var response = new HttpResponseWrapper<List<Country>>(null, true, null!);

        mockBackEnd
            .Setup(x => x.GetAsync<List<Country>>(It.IsAny<string>()))
            .ReturnsAsync(response);

        var service = new CountryService(mockBackEnd.Object);

        // Act
        var result = await service.GetCountriesAsync();

        // Assert
        Assert.True(result.Error);
        Assert.Null(result.Response);
    }

    [Fact]
    public void GetCountryByCode_WithValidCode_ReturnsCountry()
    {
        // Arrange
        var mockBackEnd = new Mock<IBackEndService>();
        var service = new CountryService(mockBackEnd.Object);

        // Act
        var result = service.GetCountryByCode("CO");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Colombia", result.Name);
    }
}
```

## 9. Integración con SearchCriteria

```csharp
public class SearchCriteria
{
    public string CountryCode { get; set; } = "CO";

    public Country? GetSelectedCountry(ICountryService countryService)
    {
        return countryService.GetCountryByCode(CountryCode);
    }

    public string GetFormattedLocation(ICountryService countryService)
    {
        var country = GetSelectedCountry(countryService);
        return country != null 
            ? $"{country.FlagEmoji} {country.Name}"
            : "No seleccionado";
    }
}
```

## 10. Flujo Completo: Búsqueda por País

```
1. Usuario abre CountryPage
   ↓
2. CountryViewModel carga países desde ICountryService
   ↓
3. Backend devuelve lista de 6 países
   ↓
4. Usuario selecciona país (ej: Argentina)
   ↓
5. AppSettingsService.SetCountry("AR")
   ↓
6. Vuelve a HomePage / SearchResultsPage
   ↓
7. SearchViewModel obtiene país actual
   ↓
8. Carga propiedades filtradas para Argentina
   ↓
9. Muestra precios en ARS ($)
   ↓
10. Muestra ciudades populares: Buenos Aires, Mendoza, etc.
```

---

**Tip:** Siempre inyecta `ICountryService` en lugar de `CountryService` para facilitar testing con mocks.
