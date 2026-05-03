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

## Equipo de Desarrollo
- Neider Fajardo
- Juan Camilo Mora
- Daniel Andrade
- Daniel Oicata

**Proyecto Final MISO - Grupo 4**
