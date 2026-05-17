#!/usr/bin/env bash
# Run E2E PoC locally. Usage: ./run_e2e_local.sh android /path/to/app.apk
PLATFORM=$1
APP_PATH=$2

if ! command -v maestro &> /dev/null; then
  echo "Maestro CLI not found. Install: curl -sSfL https://get.maestro.dev | bash"
  exit 1
fi

case "$PLATFORM" in
  android)
    if [ -z "$APP_PATH" ]; then echo "Provide APK path: ./run_e2e_local.sh android /path/to/app.apk"; exit 1; fi
    adb install -r "$APP_PATH"
    maestro test TravelHub.E2E\maestro_auth_login_flow.yaml
    maestro test TravelHub.E2E\maestro_booking_summary_flow.yaml
    ;;
  ios)
    if [ -z "$APP_PATH" ]; then echo "Provide .app path: ./run_e2e_local.sh ios /path/to/YourApp.app"; exit 1; fi
    xcrun simctl install booted "$APP_PATH"
    maestro test TravelHub.E2E/maestro_auth_login_flow.yaml
    maestro test TravelHub.E2E/maestro_booking_summary_flow.yaml
    ;;
  *)
    echo "Usage: $0 {android|ios} /path/to/app"
    exit 2
    ;;
esac
