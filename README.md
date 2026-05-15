# TravelHub - App Móvil de Alojamiento

## Pitch
App multiplataforma para reserva de hoteles en 6 países de Latinoamérica. Busca alojamiento por ciudad, gestiona reservas y perfiles de viajero.

## Arquitectura
- **Patrón**: MVVM (.NET MAUI 10)
- **Capas**: View (XAML) → ViewModel (Lógica) → Model (Dominio) → Service (Negocio)
- **Plataformas**: Android, iOS

## Patrones Implementados
- MVVM con `INotifyPropertyChanged` (BaseViewModel reutilizable)
- Inyección de dependencias vía interfaces de servicios (`IAuthService`, `IBookingService`)
- DTOs para comunicación con API backend
- Caché en memoria (`MockDataService`) y SQLite local

## Organización del Proyecto
```
TravelHub-ProyectoFinal-Movil/
├── TravelHub/App/       # Código principal
│   ├── Views/           # 14 páginas XAML
│   ├── ViewModels/       # 14 ViewModels + BaseViewModel
│   ├── Models/           # 12 entidades de dominio
│   ├── DTOs/             # 10 DTOs para API
│   ├── Services/         # Interfaces + Implementaciones
│   └── Platforms/        # Código específico por SO
├── TravelHub.Tests/      # Pruebas unitarias (xUnit + Moq)
└── README.md
```

## Buenas Prácticas
- `BaseViewModel` con método genérico `SetProperty` para notificación de cambios
- Cobertura de pruebas a ViewModels y servicios críticos
- SQLite para almacenamiento local y soporte offline
- Preferencias persistentes de usuario (`Microsoft.Maui.Storage`)
- Caché de imágenes con `UriImageSource` (1 día de validez)
- Separación de DTOs y Models para evitar acoplamiento
- Código `nullable-enabled` e `implicit usings`

## Funcionalidades Principales
1. Búsqueda de alojamiento por ciudad, fechas, adultos, niños y habitaciones
2. Detalle de propiedad con imágenes, amenidades y reseñas
3. Flujo completo de reserva: Selección → Viajero → Resumen → Confirmación
4. Gestión de cuenta (registro/inicio de sesión) y reservas activas
5. Soporte multi-moneda (COP, PEN, USD, MXN, CLP, ARS) y países
6. **Accesibilidad**:
   - Modo Daltonismo para usuarios con protanopia, deuteranopia y tritanopia
   - Ajuste de tamaño de texto (1.0x, 1.25x, 1.5x, 2.0x)
   - Modo Oscuro para reducir fatiga visual

## Documentación de Pruebas

### Introducción

Este documento describe la estrategia de pruebas implementada en la aplicación móvil TravelHub, así como su integración dentro del ciclo de desarrollo. El objetivo de estas pruebas es garantizar la calidad del código, validar el comportamiento de los componentes y servicios, y prevenir errores antes del despliegue a producción.

Las pruebas se dividen en dos categorías principales:
- **Pruebas Unitarias**: Validan la lógica de negocio en ViewModels y servicios
- **Pruebas End-to-End (E2E)**: Validan flujos completos de usuario en Android

### Herramientas y Tecnologías

#### Pruebas Unitarias
- **Framework**: xUnit (.NET)
- **Mocking**: Moq para aislar dependencias
- **Ejecución**: Dotnet CLI

Las pruebas unitarias se ejecutan de manera automatizada en el entorno de desarrollo y pueden integrarse fácilmente en pipelines de CI/CD.

#### Pruebas E2E
- **Framework**: Appium + xUnit
- **Patrón de Diseño**: Page Object Model (POM)
- **Plataforma**: Android
- **Servidor Appium**: Requerido en `http://localhost:4723`

El patrón Page Object Model permite abstraer la lógica de interacción con la interfaz en objetos de página independientes, mejorando la reutilización de código y mantenimiento.

---

### Ejecución de Pruebas Unitarias

#### Comando de Ejecución

Para ejecutar todas las pruebas unitarias desde la raíz del proyecto:

```powershell
# Desde TravelHub/ (raíz del proyecto)
dotnet test TravelHub.Tests\TravelHub.Tests.csproj
```

#### Cobertura

Las pruebas unitarias validan los siguientes componentes críticos:

- **ViewModels**: SearchViewModel, BookingViewModel, AuthViewModel, SettingsViewModel
- **Servicios de Negocio**: BookingService, AuthService, LocalizationService, MockDataService
- **Validaciones**: Transformación de datos, mapeo de DTOs, conversión de monedas

#### Estado Actual

Las pruebas unitarias cuentan con una cobertura integral que valida la lógica de negocio principal, permitiendo detectar errores en la capa de aplicación de manera temprana.

---

### Ejecución de Pruebas End-to-End (E2E)

#### Introducción a Pruebas E2E

Las pruebas End-to-End validan el comportamiento completo de la aplicación desde la perspectiva del usuario final. Simulan interacciones reales con la interfaz gráfica, verificando que todos los componentes funcionen correctamente de manera integrada.

El objetivo de estas pruebas es:
- Validar flujos completos de usuario (búsqueda, reserva, configuración)
- Detectar errores de integración entre componentes UI
- Garantizar la correcta interacción con controles y navegación
- Asegurar la calidad desde la perspectiva del usuario final

#### Requisitos Previos

Para ejecutar las pruebas E2E, asegúrese de contar con:

- **Android SDK** configurado y disponible en el PATH
- **Emulador de Android** ejecutándose (API 30 o superior) o dispositivo Android conectado
- **Appium Server** ejecutándose en `http://localhost:4723`
- **.NET 10 SDK** instalado

#### Comando de Ejecución

```powershell
# Desde TravelHub/ (raíz del proyecto)
# Ejecutar todas las pruebas E2E
dotnet test TravelHub.E2E

# Ejecutar una suite de pruebas específica
dotnet test TravelHub.E2E --filter "FullyQualifiedName~SettingsTests"

# Ejecutar una prueba individual
dotnet test TravelHub.E2E --filter "FullyQualifiedName~SettingsTests.SettingsPage_DisplaysAllControls"
```

#### Cobertura de Pruebas E2E

Las pruebas E2E se organizan en cuatro suites funcionales:

| Suite | Cantidad | Estado | Funcionalidades |
|-------|----------|--------|-----------------|
| **SettingsTests** | 5 |  Funcional | Modo oscuro, tamaño de texto, daltonismo, restaurar predeterminados |
| **SearchFlowTests** | 4 |  En desarrollo | Búsqueda con parámetros, resultados, selección de propiedad |
| **BookingFlowTests** | 3 |  En desarrollo | Flujo de reserva, resumen, confirmación |
| **AuthFlowTests** | 5 |  En desarrollo | Login, registro, validación de credenciales, navegación |
| **TOTAL** | **17** | - | - |

#### Estado Actual de Ejecución

- **Pruebas Diseñadas**: 17
- **Pruebas Funcionales**: 5 (SettingsTests)
- **Pruebas en Desarrollo**: 12 (AuthFlow, SearchFlow, BookingFlow) 🔨

Las pruebas de SettingsTests están completamente operativas y se ejecutan exitosamente, validando la accesibilidad y configuración de la aplicación.

#### Estrategia Incremental de Ejecución

Se implementa una estrategia incremental para garantizar la estabilidad y calidad:

1. **Una Suite Activa**: Solo una suite de pruebas se ejecuta en cada momento
2. **Validación Completa**: Se requiere que todas las pruebas de la suite pasen antes de activar la siguiente
3. **Control mediante Skip**: Utiliza atributos `[Fact(Skip="...")]` de xUnit para desactivar suites no preparadas
4. **Progresión Ordenada**: SettingsTests → SearchFlowTests → BookingFlowTests → AuthFlowTests


#### Estructura del Proyecto E2E

```
TravelHub.E2E/
├── Tests/
│   ├── AuthFlowTests.cs
│   ├── SearchFlowTests.cs
│   ├── BookingFlowTests.cs
│   └── SettingsTests.cs
├── Pages/
│   ├── BasePage.cs          # Clase base con métodos comunes
│   ├── LoginPage.cs
│   ├── AccountPage.cs
│   ├── SettingsPage.cs
│   ├── SearchResultsPage.cs
│   └── PropertyDetailPage.cs
├── Drivers/
│   └── AndroidDriverFactory.cs  # Gestión de driver Appium
├── Utilities/
│   ├── AppiumFixture.cs
│   └── TestDataFactory.cs
└── appsettings.json
```

#### Patrones Implementados

- **Page Object Model**: Separación de lógica de interacción en objetos de página
- **Base Page**: Métodos comunes reutilizables (WaitForPageLoad, EnterText, TapElement)
- **Test Fixture**: Inicialización y limpieza del driver Appium por prueba
- **Aislamiento de Datos**: TestDataFactory proporciona datos consistentes

#### Buenas Prácticas Implementadas

- Separación de pruebas por flujo funcional
- Uso de AutomationIds para identificación confiable de controles
- Manejo explícito de esperas y asincronía
- Captura de pantalla y PageSource en caso de fallos
- Ejecución secuencial e independiente de suites
- Logs en consola para trazabilidad de ejecución
- Aislamiento y reproducibilidad de cada prueba

---

## Equipo de Desarrollo
- Neider Fajardo
- Juan Camilo Mora
- Daniel Andrade
- Daniel Oicata

**Proyecto Final MISO - Grupo 4**
