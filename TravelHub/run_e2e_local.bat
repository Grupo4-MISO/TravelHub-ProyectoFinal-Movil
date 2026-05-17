@echo off
REM Usage: run_e2e_local.bat android C:\path\to\app.apk
set PLATFORM=%1
set APP_PATH=%2
where maestro >nul 2>nul || (
  echo Maestro CLI not found. Install with: curl -sSfL https://get.maestro.dev | bash
  exit /b 1
)
if "%PLATFORM%"=="android" (
  if "%APP_PATH%"=="" (
    echo Provide APK path: run_e2e_local.bat android C:\path\to\app.apk
    exit /b 1
  )
  adb install -r "%APP_PATH%"
  maestro test TravelHub.E2E\maestro_auth_login_flow.yaml
  maestro test TravelHub.E2E\maestro_booking_summary_flow.yaml
) else if "%PLATFORM%"=="ios" (
  echo Use macOS to run iOS simulator commands.
) else (
  echo Usage: %0 {android|ios} /path/to/app
  exit /b 2
)
