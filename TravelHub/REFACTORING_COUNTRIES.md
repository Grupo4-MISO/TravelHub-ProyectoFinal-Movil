# Refactorización de TravelHub: Migración a Backend Real

## 📋 Resumen

Se ha refactorizado la aplicación .NET MAUI para consumir datos reales desde el backend en lugar de datos mock. La implementación sigue patrones de diseño SOLID, utiliza inyección de dependencias y proporciona manejo robusto de errores y estados de carga.

## 🏗️ Arquitectura Implementada

### Componentes Principales

#### 1. **Modelo de Datos (App\Models\Country.cs)**
```csharp
public class Country
{
    public string Id { get; set; }           // UUID desde el backend
    public string Name { get; set; }         // Nombre del país
    public string Code { get; set; }         // Código ISO (AR, CL, CO, etc.)
    public string CurrencyCode { get; set; } // Código de moneda (ARS, CLP, etc.)
    public string CurrencySymbol { get; set; } // Símbolo ($, S/, etc.)
    public string FlagEmoji { get; set; }    // Emoji de bandera
    public string PhoneCode { get; set; }    // Código telefónico (+54, +56, etc.)
}
```

#### 2. **Interfaz del Servicio (App\Services\Interfaces\ICountryService.cs)**
Define el contrato para las operaciones con países:
- `GetCountriesAsync()` - Obtiene lista de países desde el backend
- `GetCountryByCode(code)` - Obtiene país por código
- `GetCountryById(id)` - Obtiene país por ID
- `GetPopularCitiesByCountry(code)` - Retorna ciudades populares

#### 3. **Implementación del Servicio (App\Services\Implementations\CountryService.cs)**

**Características principales:**
- ✅ Inyección de dependencias de `IBackEndService`
- ✅ Caché de países en memoria
- ✅ Manejo robusto de excepciones
- ✅ Llamadas asincrónicas al endpoint: `https://light-eggs-lie.loca.lt/api/v1/inventarios/countries`

```csharp
public async Task<HttpResponseWrapper<List<Country>>> GetCountriesAsync()
{
    // 1. Verifica caché
    if (_cachedCountries != null && _cachedCountries.Count > 0)
        return caché;

    // 2. Obtiene del backend
    var response = await _backEndService.GetAsync<List<Country>>(endpoint);

    // 3. Actualiza caché si es exitoso
    if (!response.Error && response.Response != null)
        _cachedCountries = response.Response;

    return response;
}
```

#### 4. **ViewModel Refactorizado (App\ViewModels\CountryViewModel.cs)**

**Cambios principales:**
- ✅ Constructor con inyección de `ICountryService`
- ✅ Carga de datos desde backend en inicialización
- ✅ Propiedades de estado:
  - `IsLoading` - Indicador de carga
  - `ErrorMessage` - Mensaje de error
  - `Countries` - ObservableCollection de items
- ✅ Comando `RetryLoadCommand` para reintentar en caso de error
- ✅ Manejo completo de ciclo de vida asincrónico

**Flujo de ejecución:**
```
Constructor
    ↓
MainThread.BeginInvokeOnMainThread(LoadCountries)
    ↓
IsLoading = true
    ↓
await countryService.GetCountriesAsync()
    ↓
Si error: ErrorMessage + IsLoading = false
Si éxito: Poblar Countries + IsLoading = false
```

#### 5. **Vista Actualizada (App\Views\CountryPage.xaml)**

**Estados visuales:**
1. **Cargando:** ActivityIndicator visible
2. **Error:** Mensaje de error + botón "Reintentar"
3. **Éxito:** CollectionView con lista de países

**Características:**
- Indicador de país seleccionado con icono
- Información visual: Bandera 🇨🇴, Nombre, Moneda
- Interactividad mejorada

## 🔄 Flujo de Inyección de Dependencias

### Configuración en MauiProgram.cs
```csharp
// Registros de servicios
builder.Services.AddSingleton<IBackEndService, BackEndService>();
builder.Services.AddSingleton<ICountryService, CountryService>();

// ViewModels con inyección
builder.Services.AddTransient<CountryViewModel>();
builder.Services.AddTransient<CountryPage>();
```

### Resolución en CountryPage.xaml.cs
```csharp
public CountryPage(CountryViewModel viewModel)
{
    InitializeComponent();
    BindingContext = viewModel;  // ViewModel inyectado automáticamente
}
```

## 🌐 Integración con Backend

### Endpoint Utilizado
```
GET https://light-eggs-lie.loca.lt/api/v1/inventarios/countries
Accept: application/json
```

### Respuesta Esperada
```json
[
  {
    "id": "2a1572bf-1ad7-4bf0-b8ae-000c067cbd45",
    "name": "Argentina",
    "code": "AR",
    "CurrencyCode": "ARS",
    "CurrencySymbol": "$",
    "FlagEmoji": "🇦🇷",
    "PhoneCode": "+54"
  },
  ...
]
```

### Configuración HttpClient
- Timeout: 60 segundos
- Certificado SSL: Ignorado en debug (solo pruebas)
- Case-insensitive deserialization
- Manejo de timeouts con reintentos

## 📊 Patrones de Diseño Implementados

### 1. **Dependency Injection (DI)**
- Interfaz `ICountryService` separada de la implementación
- Constructor injection en ViewModel
- Ciclo de vida controlado (Singleton para servicios)

### 2. **Repository Pattern**
- `CountryService` como repositorio de datos
- Abstracción de fuente de datos
- Caché transparente

### 3. **Observable Pattern**
- `ObservableCollection` para UI binding
- `INotifyPropertyChanged` en ViewModel
- Actualizaciones automáticas de UI

### 4. **Async/Await Pattern**
- Operaciones no-bloqueantes
- Manejo de Task<T> correctamente
- Evita deadlocks

### 5. **Error Handling**
- Wrapper `HttpResponseWrapper<T>` para respuestas
- Propiedades `Error` y `Response` diferenciadas
- Try-catch con logging

## 🔐 Buenas Prácticas Implementadas

### ✅ Validación de Entrada
```csharp
_backEndService = backEndService ?? throw new ArgumentNullException(nameof(backEndService));
```

### ✅ Caché Inteligente
```csharp
if (_cachedCountries != null && _cachedCountries.Count > 0)
    return cached;  // Evita llamadas innecesarias
```

### ✅ Logging
```csharp
System.Diagnostics.Debug.WriteLine($"Error obteniendo países: {ex.Message}");
```

### ✅ Thread Safety (UI Updates)
```csharp
MainThread.BeginInvokeOnMainThread(async () => await LoadCountries());
```

### ✅ Null Safety
```csharp
public Country? GetCountryByCode(string code)  // Retorna nullable
if (response.Response != null && response.Response.Count > 0)  // Validación
```

## 📁 Estructura de Archivos

```
App/
├── Models/
│   └── Country.cs (actualizado con string Id)
├── Services/
│   ├── Interfaces/
│   │   ├── IBackEndService.cs (existente)
│   │   └── ICountryService.cs (nuevo)
│   ├── Implementations/
│   │   ├── BackEndService.cs (existente)
│   │   └── CountryService.cs (nuevo)
│   ├── AppSettingsService.cs (actualizado)
│   └── MockDataService.cs (actualizado)
├── ViewModels/
│   ├── CountryViewModel.cs (refactorizado)
│   └── BaseViewModel.cs (existente)
├── Views/
│   ├── CountryPage.xaml (actualizado)
│   └── CountryPage.xaml.cs (refactorizado)
└── MauiProgram.cs (actualizado)
```

## 🚀 Cómo Usar

### 1. **Abrir CountryPage**
```csharp
// La página se abre automáticamente
// El ViewModel carga países en el constructor
```

### 2. **Estados Visuales**
- **Cargando:** Se muestra spinner circular
- **Error:** Se muestra mensaje + botón reintentar
- **Éxito:** Se lista todos los países

### 3. **Seleccionar País**
```csharp
// Click en país → SelectCountryCommand
// Actualiza AppSettingsService.CurrentCountryCode
// Vuelve atrás automáticamente
```

## 🔧 Configuración del Backend (Requisitos)

### Variables de Entorno (si aplica)
```
BACKEND_URL=https://light-eggs-lie.loca.lt
API_VERSION=v1
COUNTRIES_ENDPOINT=/api/v1/inventarios/countries
```

### Certificado SSL en Debug
El BackEndService actualmente ignora validación de certificado en DEBUG:
```csharp
ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
```

⚠️ **IMPORTANTE:** Cambiar en RELEASE:
```csharp
ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => 
    errors == System.Net.Security.SslPolicyErrors.None;
```

## 📈 Mejoras Futuras

1. **Persistencia Local**
   - Guardar países en base de datos local
   - Sincronización en background

2. **Rate Limiting**
   - Limitar llamadas al API
   - Exponential backoff

3. **Offline Support**
   - Datos cached funcionan sin internet
   - Sincronización cuando hay conexión

4. **Múltiples Endpoints**
   - Configuración dinámica de URLs
   - Fallback a servidores alternativos

5. **Analytics**
   - Tracking de tiempos de carga
   - Errores reportados a servidor

## 🧪 Testing

### Unit Tests (Recomendado)
```csharp
[TestMethod]
public async Task GetCountriesAsync_ReturnsData()
{
    var service = new CountryService(mockBackEndService);
    var result = await service.GetCountriesAsync();
    Assert.IsTrue(!result.Error);
    Assert.IsTrue(result.Response?.Count > 0);
}
```

### Integration Tests
```csharp
[TestMethod]
public async Task CountryViewModel_LoadsCountries()
{
    var vm = new CountryViewModel(countryService);
    await Task.Delay(1000); // Esperar carga
    Assert.IsTrue(vm.Countries.Count > 0);
}
```

## 📝 Notas Importantes

1. **IDs cambieron de int a string** - Ahora coinciden con UUIDs del backend
2. **Métodos GetCountryByCode/ById ahora retornan nullable** - Manejar nulos correctamente
3. **MockDataService se mantiene** - Como fallback para datos de propiedades
4. **BackEndService existente se reutiliza** - No hay duplicación de código
5. **Caché en memoria** - Se limpia si la app cierra

## ✨ Resumen de Cambios

| Archivo | Cambio | Razón |
|---------|--------|-------|
| Country.cs | Id: int → string | Coincidir con UUIDs backend |
| CountryViewModel.cs | Inyección ICountryService | Desacoplamiento |
| CountryPage.xaml | Estados de carga/error | UX mejorada |
| MauiProgram.cs | Registrar ICountryService | DI setup |
| MockDataService.cs | Métodos nullable | Type safety |
| AppSettingsService.cs | CurrentCountry nullable | Manejo de errores |

---

**Versión:** 1.0  
**Fecha:** 2024  
**Estado:** Implementado y compilado exitosamente ✅
