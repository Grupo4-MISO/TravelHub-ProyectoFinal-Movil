E2E Maestro migration PoC

Files added to repo:
- TravelHub.E2E/maestro_auth_login_flow.yaml
- TravelHub.E2E/maestro_booking_summary_flow.yaml
- TravelHub.E2E/maestro_login_subflow.yaml
- TravelHub.E2E/maestro_register_subflow.yaml
- TravelHub.E2E/maestro_reset_app.yaml
- TravelHub.E2E/generate_credentials.js
- maestro-e2e-poc.yml (sample workflow; move to .github/workflows/)
- run_e2e_local.sh and run_e2e_local.bat (local runner scripts)

How to run locally
1. Install Maestro CLI
2. Build your MAUI app for the target platform and produce an APK or .app
3. Start an emulator or boot a simulator
4. Install the app: adb install -r ./path/to/app.apk (Android) or xcrun simctl install booted ./path/to/YourApp.app (iOS)
5. Run flows:
   maestro test TravelHub.E2E\maestro_auth_login_flow.yaml

Notes on CI
- Copy maestro-e2e-poc.yml into .github/workflows/
- Ensure APK/.app is placed in ./artifacts/android/ or ./artifacts/ios/ as referenced
- macOS runners are required for iOS tests

If you want, I can:
- Move the sample workflow into .github/workflows/ (requires creating dot-directories — please confirm if allowed)
- Adjust the workflow to build the MAUI app in CI (requires adding Android SDK and macOS build steps)
