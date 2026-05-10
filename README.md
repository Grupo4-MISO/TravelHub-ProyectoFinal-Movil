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

## Pruebas

### Pruebas Unitarias
Las pruebas unitarias validan la lógica de negocio en ViewModels y servicios.

**Ejecución:**
```powershell
# Desde la carpeta TravelHub (raíz del proyecto)
dotnet test TravelHub.Tests\TravelHub.Tests.csproj
```

**Cobertura:**
- ViewModels críticos (SearchViewModel, BookingViewModel, AuthViewModel)
- Servicios de negocio (BookingService, LocalizationService)
- Validaciones de datos y transformaciones

---

### Pruebas E2E (End-to-End)
Las pruebas E2E validan flujos completos en Android usando Appium.

**Estado:**
- **Diseñadas**: 17 pruebas
- **Funcionales**: 5 (SettingsTests) ✓
- **En desarrollo**: 12 (AuthFlowTests, SearchFlowTests, BookingFlowTests)

**Requisitos:**
- Android SDK configurado
- Emulador de Android corriendo o dispositivo conectado
- Appium server ejecutándose en `http://localhost:4723`

**Ejecución:**
```powershell
# Desde la carpeta TravelHub
dotnet test TravelHub.E2E

# Ejecutar solo pruebas específicas
dotnet test TravelHub.E2E --filter "FullyQualifiedName~SettingsTests"
```

**Estructura:**
- **SettingsTests** (5 pruebas): Modo oscuro, tamaño de texto, modo daltonismo, restaurar predeterminados
- **SearchFlowTests** (4 pruebas - en desarrollo): Búsqueda con parámetros, resultados, selección
- **BookingFlowTests** (3 pruebas - en desarrollo): Flujo de reserva, resumen, confirmación
- **AuthFlowTests** (5 pruebas - en desarrollo): Login, registro, validación de credenciales

**Estrategia de ejecución:**
Se utiliza una estrategia incremental: una suite se ejecuta completamente, valida que pase sin errores, y solo entonces se habilita la siguiente. Esto se controla mediante atributos `[Fact(Skip="...")]` en xUnit.

---

## Equipo de Desarrollo
- Neider Fajardo
- Juan Camilo Mora
- Daniel Andrade
- Daniel Oicata

**Proyecto Final MISO - Grupo 4**
