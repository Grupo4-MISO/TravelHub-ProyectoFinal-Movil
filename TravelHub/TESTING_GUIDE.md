# 🧪 TESTING & VALIDACIÓN

## ✅ Checklist de Compilación

```bash
# 1. Limpiar solución
dotnet clean

# 2. Restaurar paquetes
dotnet restore

# 3. Compilar solución
dotnet build

# Resultado esperado: ✅ Build succeeded
```

## 🔍 Verificación Manual

### 1. Abrir la Aplicación

```
1. Compilar proyecto
2. Ejecutar en emulador o dispositivo
3. Navegar a la página de selección de país
```

### 2. Verificar Carga de Países

**Estado Esperado:**
```
┌────────────────────────────────┐
│ Selecciona el país donde       │
│ deseas buscar hospedajes       │
│                                │
│ 🇦🇷 Argentina        Moneda: ARS
│ 🇨🇱 Chile            Moneda: CLP
│ 🇨🇴 Colombia         Moneda: COP
│ 🇪🇨 Ecuador          Moneda: USD
│ 🇲🇽 México           Moneda: MXN
│ 🇵🇪 Perú             Moneda: PEN
│                                │
└────────────────────────────────┘
```

### 3. Verificar Selección de País

```
1. Hacer clic en "Argentina"
   ✓ Debe mostrar checkmark en Argentina
   ✓ Debe mostrar alerta: "Ahora estás navegando en Argentina"
   ✓ Debe volver a la página anterior

2. Hacer clic en otro país (ej: Colombia)
   ✓ Debe actualizar selección
   ✓ Debe guardar en AppSettings
```

### 4. Verificar Indicador de Carga

```
Primera ejecución:
  ✓ Debe mostrar spinner mientras carga
  ✓ Durará ~1-2 segundos (primer request)

Segunda ejecución:
  ✓ Debe cargar casi instantáneamente
  ✓ Spinner no debe aparecer (caché)
```

### 5. Verificar Manejo de Errores

**Escenario 1: Sin Internet**
```
1. Desconectar internet
2. Forzar cierre y apertura de CountryPage
3. Resultado esperado:
   ✓ Spinner mientras intenta conectar
   ✓ Mensaje: "No hay conexión a internet..."
   ✓ Botón "Reintentar" visible
```

**Escenario 2: Backend no disponible**
```
1. Detener servidor backend (apagar servidor)
2. Forzar cierre de caché (reiniciar app)
3. Abrir CountryPage
4. Resultado esperado:
   ✓ Spinner mientras intenta conectar
   ✓ Timeout después de 60 segundos
   ✓ Mensaje: "Tiempo de espera agotado..."
   ✓ Botón "Reintentar" funcional
```

**Escenario 3: Reintentar en Error**
```
1. Provocar error (sin internet)
2. Hacer clic en "Reintentar"
3. Resultado esperado:
   ✓ Spinner vuelve a aparecer
   ✓ Intenta nuevamente
```

## 📱 Testing en diferentes dispositivos

### Android
```
1. Emulador Android 12+
2. Dispositivo físico Android
3. Verificar:
   - ✓ Spinner animado
   - ✓ Binding de datos
   - ✓ Gestos táctiles
```

### iOS (si aplica)
```
1. Emulador iOS
2. Dispositivo físico iPhone
3. Verificar:
   - ✓ Misma funcionalidad
   - ✓ Performance similar
   - ✓ Binding XAML
```

### Windows (si aplica)
```
1. Ejecutar en Windows
2. Verificar:
   - ✓ UI responsiva
   - ✓ Manejo de eventos
```

## 🔎 Debug Logging

### Habilitar Debug Output

```csharp
// En MauiProgram.cs
#if DEBUG
    builder.Logging.AddDebug();
#endif
```

### Verificar Logs

En Visual Studio Output Window:
```
[CountryService] Getting countries...
[CountryService] Cache hit - returning cached countries
[BackEndService] HTTP 200 - Successfully retrieved countries
[CountryViewModel] Loaded 6 countries
```

## 🧩 Unit Tests (Recomendado)

### Crear proyecto de tests

```bash
dotnet new xunit -n App.Tests
```

### Test 1: GetCountriesAsync

```csharp
[Fact]
public async Task GetCountriesAsync_WithValidResponse_ReturnsCountries()
{
    // Arrange
    var mockBackEnd = new Mock<IBackEndService>();
    var testCountries = new List<Country>
    {
        new Country { Id = "1", Name = "Test Country", Code = "TC" }
    };

    mockBackEnd
        .Setup(x => x.GetAsync<List<Country>>(It.IsAny<string>()))
        .ReturnsAsync(new HttpResponseWrapper<List<Country>>(testCountries, false, null!));

    var service = new CountryService(mockBackEnd.Object);

    // Act
    var result = await service.GetCountriesAsync();

    // Assert
    Assert.False(result.Error);
    Assert.NotEmpty(result.Response);
    Assert.Equal("Test Country", result.Response[0].Name);
}
```

### Test 2: GetCountryByCode

```csharp
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
    Assert.Equal("COP", result.CurrencyCode);
}
```

### Test 3: CountryViewModel Load

```csharp
[Fact]
public async Task CountryViewModel_LoadsCountries_OnInitialization()
{
    // Arrange
    var mockCountryService = new Mock<ICountryService>();
    var testCountries = new List<Country>
    {
        new Country { Id = "1", Name = "Argentina", Code = "AR" }
    };

    mockCountryService
        .Setup(x => x.GetCountriesAsync())
        .ReturnsAsync(new HttpResponseWrapper<List<Country>>(testCountries, false, null!));

    // Act
    var vm = new CountryViewModel(mockCountryService.Object);
    await Task.Delay(500); // Esperar async load

    // Assert
    Assert.False(vm.IsLoading);
    Assert.Empty(vm.ErrorMessage);
    Assert.Single(vm.Countries);
}
```

## 📊 Performance Testing

### Medir Tiempo de Carga

```csharp
var sw = System.Diagnostics.Stopwatch.StartNew();

var result = await countryService.GetCountriesAsync();

sw.Stop();
System.Diagnostics.Debug.WriteLine($"Load time: {sw.ElapsedMilliseconds}ms");

// Esperado:
// Primera llamada: 1000-2000ms
// Segunda llamada (caché): <100ms
```

### Monitorear Memoria

```csharp
// En Device Memory Monitor
// Expected: ~10-15MB para la app
// Countries caché: ~10KB
```

## 🌐 Test de Conectividad

### Test 1: Verificar Endpoint

```bash
curl -X GET "https://light-eggs-lie.loca.lt/api/v1/inventarios/countries" \
     -H "accept: application/json"

# Resultado esperado: 200 OK
# Body: [...6 países...]
```

### Test 2: Simular Timeout

```csharp
// En BackEndService durante debug
_httpClient.Timeout = TimeSpan.FromMilliseconds(1); // 1ms timeout
// Fuerza timeout simulado

// Resultado esperado:
// TaskCanceledException
// UI muestra: "Tiempo de espera agotado"
```

### Test 3: Simular Error 5xx

```
Backend devuelve: 500 Internal Server Error

Resultado esperado:
✓ HttpResponseMessage.IsSuccessStatusCode = false
✓ UI muestra: "Error al cargar los países"
✓ Botón Reintentar disponible
```

## 🔧 Verificación de Inyección de Dependencias

### Test: DI Container Resolution

```csharp
[Fact]
public void MauiProgram_RegistersServices_Correctly()
{
    // Arrange & Act
    var mauiApp = MauiProgram.CreateMauiApp();
    var services = mauiApp.Services;

    // Assert
    var countryService = services.GetRequiredService<ICountryService>();
    Assert.NotNull(countryService);

    var backEndService = services.GetRequiredService<IBackEndService>();
    Assert.NotNull(backEndService);

    var countryViewModel = services.GetRequiredService<CountryViewModel>();
    Assert.NotNull(countryViewModel);
}
```

## 📋 Checklist Final de Validación

### Funcionalidad
- [ ] Página carga sin errores
- [ ] Países se cargan correctamente
- [ ] Selección de país funciona
- [ ] Guardar en AppSettings funciona
- [ ] Volver atrás funciona

### Performance
- [ ] Primera carga: ~1-2 segundos
- [ ] Segunda carga: <100ms
- [ ] Caché funciona correctamente
- [ ] Sin memory leaks

### Error Handling
- [ ] Sin internet → Error + Reintentar
- [ ] Timeout → Error + Reintentar
- [ ] Servidor error → Error + Reintentar
- [ ] JSON inválido → Error + Reintentar

### UI/UX
- [ ] Spinner visible mientras carga
- [ ] Error visible y legible
- [ ] Botón Reintentar funcional
- [ ] Selección de país intuitiva

### Código
- [ ] Compila sin errores
- [ ] Compila sin warnings
- [ ] Null-safe
- [ ] Async-safe
- [ ] Code style consistente

### Documentación
- [ ] README actualizado
- [ ] Arquitectura documentada
- [ ] Ejemplos incluidos
- [ ] Tests escritos

## 🎯 Resultado Esperado

```
✅ Compilación: Éxito
✅ Carga de datos: Éxito (6 países)
✅ Selección: Funciona
✅ Persistencia: Funciona
✅ Error handling: Robusto
✅ Performance: Optimizado
✅ Documentación: Completa
```

---

**¡La refactorización está lista para producción!** 🚀
