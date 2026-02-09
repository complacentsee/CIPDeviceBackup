<#
.SYNOPSIS
    Batch backup runner for CIP Device Backup CLI against PowerFlex 750 series devices.

.DESCRIPTION
    Reads a list of IP addresses from a text file and runs the CIP Device Backup CLI
    against each device sequentially. Uses --all to capture full parameter metadata
    (type, record, isWritable) needed for generating missing port card definitions.

.PARAMETER DeviceList
    Path to a text file containing one IP address per line. Lines starting with #
    are treated as comments. Blank lines are skipped.

.PARAMETER ExePath
    Path to the CIPDeviceBackup executable. Defaults to CIPDeviceBackup.exe in the
    project root (parent of this script's directory).

.PARAMETER NoPing
    Skip pinging devices before connecting. Enabled by default since industrial
    networks often block ICMP.

.EXAMPLE
    .\devicebackups\Run-Backups.ps1 -DeviceList devices.txt
    .\devicebackups\Run-Backups.ps1 -DeviceList devices.txt -ExePath .\publish\CIPDeviceBackup.exe
#>
param(
    [Parameter(Mandatory = $true)]
    [string]$DeviceList,

    [string]$ExePath = $null,

    [switch]$NoPing = $true
)

$ErrorActionPreference = "Continue"

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$projectRoot = Split-Path -Parent $scriptDir
$backupDir = $scriptDir

# Default exe path: project root
if (-not $ExePath) {
    $ExePath = Join-Path $projectRoot "CIPDeviceBackup.exe"
}

if (-not (Test-Path $ExePath)) {
    Write-Error "CIPDeviceBackup executable not found at: $ExePath"
    Write-Error "Build the project first with: dotnet publish --configuration Release --runtime win-x64 --self-contained true -p:PublishSingleFile=true --output .\publish"
    exit 1
}

if (-not (Test-Path $DeviceList)) {
    Write-Error "Device list file not found: $DeviceList"
    exit 1
}

# Read and filter device list
$devices = Get-Content $DeviceList |
    ForEach-Object { $_.Trim() } |
    Where-Object { $_ -ne "" -and -not $_.StartsWith("#") }

if ($devices.Count -eq 0) {
    Write-Error "No devices found in $DeviceList"
    exit 1
}

Write-Host "Starting batch backup of $($devices.Count) devices" -ForegroundColor Cyan
Write-Host "Backup directory: $backupDir" -ForegroundColor Cyan
Write-Host ""

$logFile = Join-Path $backupDir "run_log.txt"
$succeeded = 0
$failed = 0
$failedDevices = @()
$startTime = Get-Date

# Write log header
"Batch Backup Run - $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')" | Out-File $logFile
"Devices: $($devices.Count)" | Out-File $logFile -Append
"Executable: $ExePath" | Out-File $logFile -Append
"---" | Out-File $logFile -Append

for ($i = 0; $i -lt $devices.Count; $i++) {
    $ip = $devices[$i]
    $pct = [math]::Floor(($i / $devices.Count) * 100)

    Write-Progress -Activity "Running CIP Device Backups" `
        -Status "$ip ($($i + 1) of $($devices.Count))" `
        -PercentComplete $pct `
        -CurrentOperation "$succeeded OK, $failed failed"

    $outputFile = Join-Path $backupDir "$ip.json"
    $deviceStart = Get-Date

    $arguments = @("--host", $ip, "--all", "-o", $outputFile)
#    if ($NoPing) {
#        $arguments += "--noping"
#    }
    Write-Host $arguments

    try {
        $process = Start-Process -FilePath $ExePath -ArgumentList $arguments `
            -NoNewWindow -Wait -PassThru -RedirectStandardOutput (Join-Path $backupDir "stdout_temp.txt") `
            -RedirectStandardError (Join-Path $backupDir "stderr_temp.txt")

        $elapsed = (Get-Date) - $deviceStart
        $elapsedStr = "{0:F1}s" -f $elapsed.TotalSeconds

        if ($process.ExitCode -eq 0) {
            $succeeded++
            $status = "OK ($elapsedStr)"
            Write-Host "  [$($i + 1)/$($devices.Count)] $ip ... " -NoNewline
            Write-Host $status -ForegroundColor Green
        } else {
            $failed++
            $stderr = if (Test-Path (Join-Path $backupDir "stderr_temp.txt")) {
                Get-Content (Join-Path $backupDir "stderr_temp.txt") -Raw
            } else { "unknown error" }
            $status = "FAILED (exit $($process.ExitCode), $elapsedStr): $($stderr.Trim())"
            $failedDevices += $ip
            Write-Host "  [$($i + 1)/$($devices.Count)] $ip ... " -NoNewline
            Write-Host $status -ForegroundColor Red
        }
    } catch {
        $failed++
        $elapsed = (Get-Date) - $deviceStart
        $elapsedStr = "{0:F1}s" -f $elapsed.TotalSeconds
        $status = "ERROR ($elapsedStr): $($_.Exception.Message)"
        $failedDevices += $ip
        Write-Host "  [$($i + 1)/$($devices.Count)] $ip ... " -NoNewline
        Write-Host $status -ForegroundColor Red
    }

    "$ip - $status" | Out-File $logFile -Append
}

# Clean up temp files
Remove-Item (Join-Path $backupDir "stdout_temp.txt") -ErrorAction SilentlyContinue
Remove-Item (Join-Path $backupDir "stderr_temp.txt") -ErrorAction SilentlyContinue

# Final progress
Write-Progress -Activity "Running CIP Device Backups" -Completed

$totalElapsed = (Get-Date) - $startTime

# Summary
Write-Host ""
Write-Host "=== Batch Backup Complete ===" -ForegroundColor Cyan
Write-Host "  Total:     $($devices.Count) devices"
Write-Host "  Succeeded: $succeeded" -ForegroundColor Green
Write-Host "  Failed:    $failed" -ForegroundColor $(if ($failed -gt 0) { "Red" } else { "Green" })
Write-Host "  Duration:  $("{0:F0}" -f $totalElapsed.TotalMinutes) minutes"
Write-Host "  Log:       $logFile"

if ($failedDevices.Count -gt 0) {
    Write-Host ""
    Write-Host "Failed devices:" -ForegroundColor Yellow
    $failedDevices | ForEach-Object { Write-Host "  $_" -ForegroundColor Red }
}

# Append summary to log
"---" | Out-File $logFile -Append
"Completed: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')" | Out-File $logFile -Append
"Succeeded: $succeeded / $($devices.Count)" | Out-File $logFile -Append
"Failed: $failed" | Out-File $logFile -Append
"Duration: $("{0:F1}" -f $totalElapsed.TotalMinutes) minutes" | Out-File $logFile -Append
