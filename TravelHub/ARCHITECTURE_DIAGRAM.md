# 📊 Diagrama de Arquitectura - TravelHub Countries Module

## 🔄 Flujo de Datos Actual

```
┌─────────────────────────────────────────────────────────────────┐
│                         USER INTERFACE                          │
│                        CountryPage.xaml                         │
│  ┌───────────────────────────────────────────────────────────┐  │
│  │ [Loading Spinner]  [Error Message]  [Countries List]      │  │
│  └────────────────────────┬──────────────────────────────────┘  │
└─────────────────────────────┼──────────────────────────────────┘
                              │ Binding
                              ↓
┌─────────────────────────────────────────────────────────────────┐
│                      VIEW MODEL LAYER                           │
│                   CountryViewModel.cs                           │
│  ┌───────────────────────────────────────────────────────────┐  │
│  │ Properties:                                               │  │
│  │ - Countries: ObservableCollection<CountryItem>            │  │
│  │ - IsLoading: bool                                         │  │
│  │ - ErrorMessage: string                                    │  │
│  │                                                           │  │
│  │ Commands:                                                 │  │
│  │ - SelectCountryCommand                                    │  │
│  │ - RetryLoadCommand                                        │  │
│  └────────────────────────┬──────────────────────────────────┘  │
│                           │ Inyección de Dependencias            │
└─────────────────────────────┼──────────────────────────────────┘
                              │
                              ↓
┌─────────────────────────────────────────────────────────────────┐
│                   SERVICE LAYER (DI)                            │
│                  ICountryService Interface                      │
│  ┌───────────────────────────────────────────────────────────┐  │
│  │ + GetCountriesAsync(): Task<Response<List<Country>>>      │  │
│  │ + GetCountryByCode(code): Country?                        │  │
│  │ + GetCountryById(id): Country?                            │  │
│  │ + GetPopularCitiesByCountry(code): List<string>           │  │
│  └────────────────────────┬──────────────────────────────────┘  │
│                           │                                      │
│                    Implementación                                │
│                           │                                      │
│  ┌────────────────────────▼──────────────────────────────────┐  │
│  │            CountryService.cs                              │  │
│  │  ┌──────────────────────────────────────────────────────┐ │  │
│  │  │ [Caché en Memoria]                                   │ │  │
│  │  │ _cachedCountries: List<Country>?                     │ │  │
│  │  └──────────────────────────────────────────────────────┘ │  │
│  │  ┌──────────────────────────────────────────────────────┐ │  │
│  │  │ Lógica:                                              │ │  │
│  │  │ 1. Verifica caché                                    │ │  │
│  │  │ 2. Si está vacío → Llama a backend                  │ │  │
│  │  │ 3. Guarda en caché si es exitoso                    │ │  │
│  │  │ 4. Retorna wrapped response                         │ │  │
│  │  └──────────────────────────────────────────────────────┘ │  │
│  └────────────────────────┬──────────────────────────────────┘  │
└─────────────────────────────┼──────────────────────────────────┘
                              │ Inyección de Dependencias
                              ↓
┌─────────────────────────────────────────────────────────────────┐
│                 BACKEND SERVICE LAYER                           │
│                  IBackEndService Interface                      │
│  ┌───────────────────────────────────────────────────────────┐  │
│  │ + GetAsync<T>(url): Task<HttpResponseWrapper<T>>          │  │
│  │ + PostAsync<T>(url, model): Task<HttpResponseWrapper>     │  │
│  │ + PutAsync<T>(url, model): Task<HttpResponseWrapper>      │  │
│  │ + DeleteAsync(url): Task<HttpResponseWrapper>             │  │
│  └────────────────────────┬──────────────────────────────────┘  │
│                           │                                      │
│                    Implementación                                │
│                           │                                      │
│  ┌────────────────────────▼──────────────────────────────────┐  │
│  │            BackEndService.cs                              │  │
│  │  ┌──────────────────────────────────────────────────────┐ │  │
│  │  │ HttpClient: _httpClient (Timeout: 60s)              │ │  │
│  │  │ JsonSerializer Options: PropertyNameCaseInsensitive │ │  │
│  │  │ SSL: Ignora certificados en DEBUG                   │ │  │
│  │  └──────────────────────────────────────────────────────┘ │  │
│  └────────────────────────┬──────────────────────────────────┘  │
└─────────────────────────────┼──────────────────────────────────┘
                              │
                              ↓ HTTP GET
┌─────────────────────────────────────────────────────────────────┐
│                      BACKEND API                                │
│  GET https://light-eggs-lie.loca.lt/api/v1/inventarios/countries
│                                                                 │
│  Response 200 OK:                                              │
│  [                                                             │
│    {                                                           │
│      "id": "uuid",                                             │
│      "name": "Argentina",                                      │
│      "code": "AR",                                             │
│      "currencyCode": "ARS",                                    │
│      "currencySymbol": "$",                                    │
│      "flagEmoji": "🇦🇷",                                        │
│      "phoneCode": "+54"                                        │
│    },                                                          │
│    ... (6 países total)                                        │
│  ]                                                             │
└─────────────────────────────────────────────────────────────────┘
```

## 🏗️ Estructura de Capas

```
┌────────────────────────────────────────────┐
│          PRESENTATION LAYER (UI)           │
│  CountryPage.xaml & CountryPage.xaml.cs   │
│  Responsabilidad: Mostrar UI + Binding      │
└────────────────────┬───────────────────────┘
                     │
┌────────────────────▼───────────────────────┐
│       VIEWMODEL LAYER (Logic & State)      │
│         CountryViewModel.cs                │
│  Responsabilidad: Lógica, Estado, Binding  │
└────────────────────┬───────────────────────┘
                     │
┌────────────────────▼───────────────────────┐
│     APPLICATION SERVICE LAYER (DI)         │
│  ICountryService + CountryService          │
│  Responsabilidad: Orquesta fuentes datos   │
└────────────────────┬───────────────────────┘
                     │
┌────────────────────▼───────────────────────┐
│      INFRASTRUCTURE SERVICE LAYER          │
│  IBackEndService + BackEndService          │
│  Responsabilidad: Comunicación HTTP        │
└────────────────────┬───────────────────────┘
                     │
┌────────────────────▼───────────────────────┐
│           EXTERNAL API (Backend)           │
│   REST API - Countries Endpoint            │
│   Responsabilidad: Proveer datos           │
└────────────────────────────────────────────┘
```

## 📦 Inyección de Dependencias (DI Container)

```
MauiProgram.cs
│
└─ builder.Services.AddSingleton<IBackEndService, BackEndService>()
│
└─ builder.Services.AddSingleton<ICountryService, CountryService>()
   │
   └─ Resolve: CountryService(IBackEndService backEndService)
      │
      ├─ Singleton de BackEndService
      └─ Instancia única en toda la app
│
└─ builder.Services.AddTransient<CountryViewModel>()
   │
   └─ Resolve: CountryViewModel(ICountryService countryService)
      │
      ├─ Nueva instancia por cada resolución
      └─ Inyecta singleton de ICountryService
│
└─ builder.Services.AddTransient<CountryPage>()
   │
   └─ Resolve: CountryPage(CountryViewModel viewModel)
      │
      ├─ Nueva instancia por cada navegación
      └─ Inyecta transient de CountryViewModel
```

## 🔀 Ciclo de Vida - Secuencia Temporal

```
┌──────────────────────────────────────────────────────────────────┐
│ TIEMPO 1: Usuario navega a CountryPage                           │
└──────────────────────────────────────────────────────────────────┘
   │
   ├─ [DI Container] Resuelve CountryPage(CountryViewModel)
   │
   ├─ [DI Container] Resuelve CountryViewModel(ICountryService)
   │
   ├─ [DI Container] Resuelve ICountryService → CountryService(IBackEndService)
   │
   └─ [Constructor] CountryViewModel
      │
      ├─ SelectCountryCommand = new Command()
      ├─ RetryLoadCommand = new Command()
      │
      └─ MainThread.BeginInvokeOnMainThread(LoadCountries)

┌──────────────────────────────────────────────────────────────────┐
│ TIEMPO 2: CountryViewModel.LoadCountries() inicia                │
└──────────────────────────────────────────────────────────────────┘
   │
   ├─ IsLoading = true  ──→ [UI Updates] Muestra spinner
   ├─ ErrorMessage = ""  ──→ [UI Updates] Limpia errores
   │
   └─ await countryService.GetCountriesAsync()
      │
      ├─ [CountryService] Verifica caché (_cachedCountries)
      │  │
      │  └─ Si existe → Retorna caché (rápido)
      │  
      ├─ [CountryService] Si no existe caché
      │  │
      │  └─ await backEndService.GetAsync<List<Country>>(endpoint)
      │     │
      │     └─ [BackEndService] HttpClient.GetAsync()
      │        │
      │        ├─ Send: GET /api/v1/inventarios/countries
      │        ├─ Timeout: 60 segundos
      │        ├─ Retry: Automático en timeout
      │        │
      │        └─ Response: 200 OK + JSON
      │           │
      │           └─ Deserialize JSON → List<Country>
      │              │
      │              └─ Retorna HttpResponseWrapper<List<Country>>
      │
      └─ [CountryService] Guarda en caché si es exitoso

┌──────────────────────────────────────────────────────────────────┐
│ TIEMPO 3: Procesa resultado                                      │
└──────────────────────────────────────────────────────────────────┘
   │
   ├─ if response.Error
   │  │
   │  └─ ErrorMessage = "No se pudieron cargar los países..."
   │     └─ IsLoading = false  ──→ [UI Updates] Muestra error
   │
   └─ else si response.Response es válido
      │
      ├─ foreach country in response.Response
      │  │
      │  └─ Countries.Add(new CountryItem(country))
      │     └─ [UI Updates] CollectionView se actualiza automáticamente
      │
      └─ IsLoading = false  ──→ [UI Updates] Oculta spinner

┌──────────────────────────────────────────────────────────────────┐
│ TIEMPO 4: Usuario selecciona país (ej: Argentina)                │
└──────────────────────────────────────────────────────────────────┘
   │
   ├─ [UI] TapGestureRecognizer dispara SelectCountryCommand
   │
   ├─ await SelectCountry(CountryItem selectedCountry)
   │  │
   │  ├─ Actualiza IsSelected en todos los items
   │  │
   │  ├─ AppSettingsService.SetCountry("AR")
   │  │  │
   │  │  └─ Preferences.Default.Set("SelectedCountryCode", "AR")
   │  │
   │  ├─ Shell.Current.DisplayAlert("País seleccionado", "...", "OK")
   │  │
   │  └─ Shell.Current.GoToAsync("..")  ──→ Vuelve atrás
   │
   └─ [Eventos] CountryChanged?.Invoke(this, "AR")
      │
      └─ Notifica a otras partes de la app (HomeViewModel, etc.)
```

## 🎯 Estados de la UI

```
┌─────────────────────────────────┐
│     ESTADO: CARGANDO            │
├─────────────────────────────────┤
│ IsLoading = true                │
│ ErrorMessage = ""               │
│ Countries = []                  │
│                                 │
│ Visible:                        │
│ ┌──────────────────────────────┐│
│ │      ↻ Cargando países...    ││
│ └──────────────────────────────┘│
│ Oculto: CollectionView, Error   │
└─────────────────────────────────┘

┌─────────────────────────────────┐
│     ESTADO: ERROR               │
├─────────────────────────────────┤
│ IsLoading = false               │
│ ErrorMessage = "No se pudo..."  │
│ Countries = []                  │
│                                 │
│ Visible:                        │
│ ┌──────────────────────────────┐│
│ │ ⚠️  No se pudieron cargar     ││
│ │    los países. Intenta...    ││
│ │                              ││
│ │    [Reintentar]              ││
│ └──────────────────────────────┘│
│ Oculto: CollectionView, Spinner │
└─────────────────────────────────┘

┌─────────────────────────────────┐
│     ESTADO: ÉXITO               │
├─────────────────────────────────┤
│ IsLoading = false               │
│ ErrorMessage = ""               │
│ Countries = [6 items]           │
│                                 │
│ Visible:                        │
│ ┌──────────────────────────────┐│
│ │ 🇦🇷 Argentina      [✓]        ││
│ │ 🇨🇱 Chile                    ││
│ │ 🇨🇴 Colombia                 ││
│ │ 🇪🇨 Ecuador                  ││
│ │ 🇲🇽 México                   ││
│ │ 🇵🇪 Perú                     ││
│ └──────────────────────────────┘│
│ Oculto: Error, Spinner          │
└─────────────────────────────────┘
```

## 🔐 Manejo de Errores

```
┌─────────────────────────────────────────────┐
│         ESCENARIOS DE ERROR                 │
└─────────────────────────────────────────────┘

1. Sin conexión a internet
   │
   └─ CheckConnection() retorna IsSuccess = false
      │
      └─ ErrorMessage = "No hay conexión a internet..."

2. Servidor no disponible (503)
   │
   └─ HttpResponseMessage.StatusCode = ServiceUnavailable
      │
      └─ ErrorMessage = "Servidor no disponible..."

3. Timeout (>60 segundos)
   │
   └─ TaskCanceledException
      │
      └─ HttpResponseMessage.StatusCode = RequestTimeout
         │
         └─ ErrorMessage = "Tiempo de espera agotado..."

4. JSON inválido
   │
   └─ JsonSerializationException
      │
      └─ ErrorMessage = "Error inesperado..."

5. Servidor retorna error 4xx/5xx
   │
   └─ HttpResponseMessage.IsSuccessStatusCode = false
      │
      └─ ErrorMessage = "Error al cargar los países..."

   ↓ Usuario hace clic en "Reintentar"
   └─ RetryLoadCommand → LoadCountries() nuevamente
```

## 💾 Caché Strategy

```
┌─────────────────────────────────────────────┐
│        CACHÉ EN MEMORIA                     │
├─────────────────────────────────────────────┤
│                                             │
│  _cachedCountries: List<Country>?           │
│                                             │
│  ┌─────────────────────────────────────┐  │
│  │ Primera llamada:                    │  │
│  │ _cachedCountries = null             │  │
│  │         │                           │  │
│  │         └─→ [HTTP Request]          │  │
│  │             [Espera ~1-2s]          │  │
│  │         ↓                           │  │
│  │ _cachedCountries = [6 países]       │  │
│  │         │                           │  │
│  │         └─→ Retorna datos           │  │
│  └─────────────────────────────────────┘  │
│                                             │
│  ┌─────────────────────────────────────┐  │
│  │ Segunda llamada (mismo sesión):     │  │
│  │ _cachedCountries != null            │  │
│  │         │                           │  │
│  │         └─→ Retorna caché           │  │
│  │             [Instantáneo <5ms]      │  │
│  └─────────────────────────────────────┘  │
│                                             │
│  ⏰ Duración: Hasta que la app cierre      │
│  💾 Ubicación: Memoria RAM                │
│  🔄 Sincronización: N/A (solo lectura)    │
│                                             │
└─────────────────────────────────────────────┘
```

---

**Nota:** Esta arquitectura sigue los principios SOLID:
- **S**ingle Responsibility: Cada clase tiene una responsabilidad
- **O**pen/Closed: Abierto para extensión (interfaces)
- **L**iskov Substitution: Implementaciones intercambiables
- **I**nterface Segregation: Interfaces específicas
- **D**ependency Inversion: Depende de abstracciones
