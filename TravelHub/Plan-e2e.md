# Plan de Ejecución de Pruebas E2E - TravelHub

## Estado actual

- Los `AutomationId` de MAUI en Android deben localizarse por `content-desc`/accessibility id, no solo por `resource-id`.
- `AuthFlowTests` y `SearchFlowTests` siguen siendo sensibles al estado del driver y al arranque de UiAutomator2.
- Las pruebas de auth deben crear su propio contexto y no asumir sesión previa compartida.

## 1. Pre-requisitos y Configuración del Entorno

- [ ] **1.1 Verificar variables de entorno**
  - [ ] `ANDROID_HOME` o `ANDROID_SDK_ROOT` configuradas
  - [ ] `JAVA_HOME` configurada (requerido por Appium)
- [ ] **1.2 Verificar e iniciar Appium Server**
  - [ ] Appium instalado (`npm list -g appium`)
  - [ ] `appium-uiautomator2-driver` instalado
  - [ ] Iniciar Appium Server (si no está corriendo):
    ```powershell
    Start-Process -WindowStyle Hidden -FilePath "cmd.exe" -ArgumentList "/c appium --log-level info --log appium.log"
    Start-Sleep -Seconds 3
    curl http://127.0.0.1:4723/status
    ```
  - [ ] Servidor Appium corriendo en `http://127.0.0.1:4723`
- [ ] **1.3 Verificar dispositivo Android**
  - [ ] Dispositivo físico conectado (`adb devices`) o emulador corriendo
  - [ ] Depuración USB habilitada en el dispositivo
- [ ] **1.4 Verificar configuración Appium**
  - [ ] Revisar `TravelHub.E2E/Configuration/appium.settings.json`
  - [ ] `DeviceName` coincide con el dispositivo conectado
  - [ ] Ruta del APK configurada correctamente

## 2. Compilar la Aplicación (APK)

- [ ] **2.1 Compilar APK y verificar que existe**
  ```powershell
  dotnet build App/App.csproj -f net10.0-android -c Release; if ($?) { Test-Path "App/bin/Release/net10.0-android/travelhubg4.app-Signed.apk" }
  ```

## 3. Ejecutar Pruebas Unitarias (Pre-validación)

- [ ] **3.1 Ejecutar todas las pruebas unitarias**
  ```powershell
  dotnet test TravelHub.Tests/TravelHub.Tests.csproj --blame-hang-timeout 300s
  ```
- [ ] **3.2 Verificar que todas las pruebas unitarias pasan**
  - Si alguna falla, detener y corregir antes de continuar con E2E

## 4. Ejecutar Pruebas E2E — Estrategia segura (una prueba activa a la vez)

Nota: para evitar desplazamientos infinitos y errores por elementos no encontrados, ejecutar UNA sola prueba E2E a la vez. Hasta que esa prueba pase completamente, NO marcarla como completada en este plan ni ejecutar otras.

Estrategia recomendada para cada suite:
- Habilitar únicamente la primera prueba de la suite (o la prueba más crítica) y comentar/ignorar las demás temporalmente.
- Verificar AutomationIds (content-desc / accessibility id en Android) y Page Objects antes de ejecutar.
- Añadir esperas explícitas y comprobaciones de existencia antes de interactuar (ExplicitWait + TryFind)

### 4.1 AuthFlowTests — enfoque por pasos (solo 1 prueba activa)

Objetivo: dejar habilitada solo `LoginPage_DisplaysLoginForm` hasta que pase sin fallos.

- Habilitar y ejecutar (comando):
  ```powershell
  dotnet test TravelHub.E2E/TravelHub.E2E.csproj --filter "FullyQualifiedName~LoginPage_DisplaysLoginForm" --blame-hang-timeout 60s
  ```
  - Revisar capturas en `TravelHub.E2E/Screenshots/` si falla.
  - Si el driver hace scroll continuo → el selector no encuentra el elemento: revisar AutomationId y uso de accessibility id (content-desc).

- Cómo comentar/ignorar las demás pruebas (alternativas):
  1) Añadir temporalmente `[Ignore("Paused until LoginPage passes")]` sobre las otras pruebas en el proyecto `TravelHub.E2E`.
  2) O comentar el método de prueba en el archivo de pruebas.
  3) O ejecutar con `--filter` (como arriba) — esta es la opción no invasiva y recomendada para no tocar código.

- Checklist de verificación antes de marcar la prueba:
  - [ ] Verificado que el AutomationId del control exista en `Constants/AutomationIds.cs` y en la UI compilada.
  - [ ] Page Object usa `FindByAccessibilityId` o `FindByContentDescription` según la plataforma.
  - [ ] Se agregaron esperas explícitas (p. ej. WaitUntil element exists 10s) antes de acciones en la prueba.
  - [ ] La prueba completa (todas las interacciones y aserciones) pasó localmente una vez.
  - [ ] Sólo entonces marcar la casilla en este documento.

- Pruebas AuthFlowTests (documentadas; ejecutar UNA a la vez si se prefiere):
  - `LoginPage_DisplaysLoginForm`  <-- PRIMERA prueba, dejar activa hasta que pase.
  - `Login_NavigatesToAccountPage`  <-- comentar/ignorar hasta que la anterior pase.
  - `Login_WithInvalidCredentials_ShowsErrorMessage`  <-- comentar/ignorar.
  - `RegisterLink_NavigatesToRegisterPage`  <-- comentar/ignorar.
  - `Register_CreatesNewAccount`  <-- comentar/ignorar.

### 4.2 SearchFlowTests — aplicar la misma política (solo una activa a la vez)

- Recomendación inicial: dejar inhabilitadas todas y, una vez pase `LoginPage_DisplaysLoginForm`, habilitar `SearchHotels_WithDefaultValues_DisplaysResults` y repetir el proceso.
- Ejecutar con filtro por nombre completo cuando toque:
  ```powershell
  dotnet test TravelHub.E2E/TravelHub.E2E.csproj --filter "FullyQualifiedName~SearchHotels_WithDefaultValues_DisplaysResults" --blame-hang-timeout 60s
  ```

### 4.3 BookingFlowTests y 4.4 SettingsTests

- Misma regla: no ejecutar en paralelo con otras pruebas. Habilitar la primera prueba de la suite solo cuando las suites anteriores estén validadas.
- Prerrequisito para `BookingSummary_RequiresTermsAcceptance`: asegurar `AutomationId` para el checkbox de Términos.

### Notas técnicas para evitar desplazamiento continuo (symptomático de "element not found")
- Preferir localizadores por AccessibilityId (content-desc) para MAUI/Android; resource-id es menos fiable en algunos builds.
- En Page Objects usar patrón: WaitForElement(AutomationId, timeout) → ScrollIfNeededUntil(AutomationId, maxTries) → ThenInteract.
- Evitar búsquedas por texto visible que dependen de idioma/localización. Usar constantes.
- Aumentar temporalmente ExplicitWait en `appium.settings.json` cuando la app carga lentamente.

### Condición para marcar check en este documento
- No marcar ninguna casilla de ejecución de pruebas E2E hasta que la prueba indicada haya completado todas sus aserciones sin fallos en N ejecuciones consecutivas (recomendar 1-3 según estabilidad).
- Una vez la primera prueba de la suite pasa estable, quitar los `[Ignore]` / comentarios de la siguiente prueba y repetir el proceso.

(Al terminar con AuthFlowTests: descomentar o quitar `[Ignore]` de las pruebas siguientes y ejecutar la siguiente prueba activa)


## 5. Análisis de Fallos y Ajuste de Pruebas Unitarias

- [ ] **5.1 Si alguna prueba E2E falla por timeout (WebDriverTimeoutException):**
  - [ ] Revisar captura de pantalla en `TravelHub.E2E/Screenshots/`
  - [ ] Verificar que el `AutomationId` existe en la app (revisar `Constants/AutomationIds.cs` vs la UI real)
  - [ ] Verificar que la página se cargó correctamente (el driver encontró la actividad)
  - [ ] Aumentar `ExplicitWaitSeconds` en `appium.settings.json` si es necesario
  - [ ] Verificar que el Page Object usa el `AutomationId` correcto

- [ ] **5.2 Si alguna prueba E2E falla por lógica de negocio:**
  - [ ] Identificar qué prueba unitaria cubre esa misma lógica
  - [ ] Ejecutar la prueba unitaria específica:
    ```powershell
    dotnet test TravelHub.Tests/TravelHub.Tests.csproj --filter "FullyQualifiedName~<TestName>"
    ```
  - [ ] Ajustar la prueba unitaria y/o el código de producción
  - [ ] Re-ejecutar la prueba E2E

- [ ] **5.3 Si todas las pruebas E2E fallan con el mismo error:**
  - [ ] Verificar que el APK se instaló correctamente en el dispositivo
  - [ ] Verificar que `appWaitActivity` coincide con la actividad principal
  - [ ] Probar instalación manual: `adb install -r App/bin/Release/net10.0-android/travelhubg4.app-Signed.apk`
  - [ ] Verificar conectividad con Appium: `curl http://127.0.0.1:4723/status`

## 6. Reporte Final

- [ ] **6.1 Generar resumen de resultados con TRX**
  ```powershell
  dotnet test TravelHub.E2E/ --logger "trx;LogFileName=e2e-results.trx" --blame-hang-timeout 60s
  ```
- [ ] **6.2 Documentar pruebas fallidas y acciones correctivas tomadas**
- [ ] **6.3 Verificar que todas las pruebas unitarias siguen pasando después de ajustes**

## 7. Cleanup

- [ ] **7.1 Desinstalar la app del dispositivo**
  ```powershell
  adb uninstall com.travelhub.g4
  ```
- [ ] **7.2 Detener Appium Server**
  ```powershell
  Stop-Process -Name "node" -Force
  ```

---

**Resumen:** 4 suites | 17 pruebas E2E | Timeout individual: 60s | Timeout hang: 60s
