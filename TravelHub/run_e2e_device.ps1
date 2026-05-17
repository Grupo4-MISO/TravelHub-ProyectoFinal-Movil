<#
PowerShell script to install APK on a connected Android device and run Maestro flows against it.
Usage:
  .\run_e2e_device.ps1 -ApkPath ".\artifacts\android\app.apk" -FlowsRoot "TravelHub/TravelHub.E2E" 

Requirements:
  - adb on PATH
  - maestro CLI on PATH
  - Windows PowerShell (tested)
#>
param(
    [string]$ApkPath = ".\artifacts\android\app.apk",
    [string]$FlowsRoot = "TravelHub/TravelHub.E2E",
    [string[]]$Flows = @("maestro_auth_login_flow.yaml", "maestro_booking_summary_flow.yaml"),
    [switch]$SkipInstall
)

function Fail([string]$msg, [int]$code = 1) {
    Write-Host $msg -ForegroundColor Red
    exit $code
}

Write-Host "Running E2E on connected Android device"

# Check maestro
if (-not (Get-Command maestro -ErrorAction SilentlyContinue)) {
    Write-Host "Maestro CLI not found in PATH." -ForegroundColor Yellow
    Write-Host "Install it and re-run: curl -sSfL https://get.maestro.dev | bash" -ForegroundColor Yellow
    Read-Host "Press Enter to continue if you installed Maestro, or Ctrl+C to abort"
}

# Check adb
if (-not (Get-Command adb -ErrorAction SilentlyContinue)) {
    Fail "adb not found in PATH. Install Android platform-tools and ensure adb is on PATH." 2
}

# List devices
$raw = & adb devices
$lines = $raw -split "`n" | ForEach-Object { $_.Trim() } | Where-Object { $_ -ne "" }
# skip header if present
if ($lines.Count -gt 0 -and $lines[0] -like "List of devices*") { $lines = $lines | Select-Object -Skip 1 }
$deviceIds = @()
foreach ($ln in $lines) {
    if ($ln -match "^(\S+)\s+(device)\b") {
        $deviceIds += $matches[1]
    }
}

if ($deviceIds.Count -eq 0) {
    Fail "No connected devices found. Ensure USB debugging is enabled and device is authorized (adb devices)." 3
}

$selectedDevice = $null
if ($deviceIds.Count -eq 1) {
    $selectedDevice = $deviceIds[0]
    Write-Host "Using device: $selectedDevice"
} else {
    Write-Host "Multiple devices detected:" -ForegroundColor Cyan
    for ($i=0; $i -lt $deviceIds.Count; $i++) { Write-Host "[$i] $($deviceIds[$i])" }
    $idx = Read-Host "Select device index to use (0..$($deviceIds.Count-1))"
    if ($idx -notmatch '^[0-9]+$' -or [int]$idx -lt 0 -or [int]$idx -ge $deviceIds.Count) { Fail "Invalid selection" 4 }
    $selectedDevice = $deviceIds[[int]$idx]
}

# Install APK (unless skipped)
if (-not $SkipInstall) {
    if (-not (Test-Path $ApkPath)) {
        Fail "APK not found at: $ApkPath. Build your app and place the APK at this path or pass -ApkPath." 5
    }
    Write-Host "Installing APK $ApkPath to device $selectedDevice..."
    $installOutput = & adb -s $selectedDevice install -r $ApkPath 2>&1
    Write-Host $installOutput
    if ($installOutput -match "Failure") {
        Write-Host "APK install reported failure, continuing but tests may fail." -ForegroundColor Yellow
    }
}

# Run flows
foreach ($flow in $Flows) {
    $flowPath = Join-Path $FlowsRoot $flow
    if (-not (Test-Path $flowPath)) {
        Write-Host "Flow not found: $flowPath" -ForegroundColor Yellow
        continue
    }
    $timestamp = (Get-Date).ToString('yyyyMMdd-HHmmss')
    $logFile = "maestro_$($flow)_$timestamp.log"
    Write-Host "Running flow: $flowPath -> logging to $logFile"
    # Run maestro and tee output to log
    & maestro test $flowPath 2>&1 | Tee-Object -FilePath $logFile
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Flow $flow returned non-zero exit code ($LASTEXITCODE)" -ForegroundColor Red
    }
}

Write-Host "Finished. Collected logs and screenshots (if any) in the current folder." -ForegroundColor Green
Write-Host "Screenshots expected: auth_poc_after_login.png, booking_summary_before_terms.png" -ForegroundColor Green

exit 0
