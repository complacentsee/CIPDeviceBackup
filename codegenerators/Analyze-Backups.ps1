<#
.SYNOPSIS
    Analyzes CIP device backup files to detect unknown PowerFlex 750 port card types.

.DESCRIPTION
    Scans all .json backup files in the devicebackups directory, extracts port card
    identity information (ProductCode, ProductName), and compares against known port
    card definitions in the codebase. Reports any unknown port card types that need
    new PF750_PortCard_*.cs class definitions.

.PARAMETER BackupDir
    Directory containing backup .json files. Defaults to the script's own directory.

.PARAMETER PortCardDir
    Directory containing PF750_PortCard_*.cs source files. Auto-detected from script location.

.EXAMPLE
    .\devicebackups\Analyze-Backups.ps1
    .\devicebackups\Analyze-Backups.ps1 -BackupDir C:\backups
#>
param(
    [string]$BackupDir = $null,
    [string]$PortCardDir = $null
)

$ErrorActionPreference = "Stop"

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$projectRoot = Split-Path -Parent $scriptDir

if (-not $BackupDir) {
    $BackupDir = $scriptDir
}

if (-not $PortCardDir) {
    $PortCardDir = Join-Path $projectRoot "CIPDevices\PortCards"
}

# --- Step 1: Auto-discover known ProductCodes from source ---

Write-Host "Discovering known port card ProductCodes from source..." -ForegroundColor Cyan

$knownProductCodes = @{}
$portCardFiles = Get-ChildItem -Path $PortCardDir -Filter "PF750_PortCard_*.cs" -ErrorAction SilentlyContinue

if ($portCardFiles.Count -eq 0) {
    Write-Error "No PF750_PortCard_*.cs files found in $PortCardDir"
    exit 1
}

foreach ($file in $portCardFiles) {
    $content = Get-Content $file.FullName -Raw
    # Match: ProductCodes => [57760] or ProductCodes => [57760, 12345]
    if ($content -match 'ProductCodes\s*=>\s*\[([^\]]+)\]') {
        $codes = $Matches[1] -split ',' | ForEach-Object { $_.Trim() } | Where-Object { $_ -ne "" }
        foreach ($code in $codes) {
            if ($code -match '^\d+$') {
                $knownProductCodes[[int]$code] = $file.BaseName -replace 'PF750_PortCard_', ''
            }
        }
    }
}

# Known-skip ProductCodes (not port cards, or intentionally skipped)
$skipProductCodes = @{
    767   = "HIM Module"
    65280 = "Empty Port"
}

$allKnown = @{}
foreach ($k in $knownProductCodes.Keys) { $allKnown[$k] = $knownProductCodes[$k] }
foreach ($k in $skipProductCodes.Keys) { $allKnown[$k] = $skipProductCodes[$k] }

Write-Host "  Found $($knownProductCodes.Count) known port card types:" -ForegroundColor Gray
foreach ($entry in $knownProductCodes.GetEnumerator() | Sort-Object Value) {
    Write-Host "    $($entry.Value) (ProductCode $($entry.Key))" -ForegroundColor Gray
}
Write-Host "  Plus $($skipProductCodes.Count) skipped types (HIM, Empty)" -ForegroundColor Gray
Write-Host ""

# --- Step 2: Scan backup files ---

$backupFiles = Get-ChildItem -Path $BackupDir -Filter "*.json" -ErrorAction SilentlyContinue
if ($backupFiles.Count -eq 0) {
    Write-Error "No .json backup files found in $BackupDir"
    exit 1
}

Write-Host "Scanning $($backupFiles.Count) backup files..." -ForegroundColor Cyan

# Track unknowns: ProductCode -> { Name, Count, FirstFile, FirstPort, Files }
$unknownCards = @{}
# Track per-file findings: FilePath -> [ { Port, ProductCode, ProductName } ]
$fileFindings = @{}

for ($i = 0; $i -lt $backupFiles.Count; $i++) {
    $file = $backupFiles[$i]
    $pct = [math]::Floor(($i / $backupFiles.Count) * 100)

    Write-Progress -Activity "Analyzing Backups" `
        -Status "$($file.Name) ($($i + 1) of $($backupFiles.Count))" `
        -PercentComplete $pct

    try {
        $raw = Get-Content $file.FullName -Raw

        # The backup format is multiple JSON objects separated by newlines.
        # Split on }{ boundaries (with possible whitespace/newlines between).
        # Wrap in array brackets for valid JSON array parsing.
        $jsonArray = "[$($raw -replace '}\s*\{', '},{')]"
        $objects = $jsonArray | ConvertFrom-Json

        foreach ($obj in $objects) {
            # Only check expansion ports (Port > 0)
            if ($obj.Port -gt 0 -and $obj.identityObject) {
                $pc = [int]$obj.identityObject.ProductCode
                $pn = $obj.identityObject.ProductName

                if (-not $allKnown.ContainsKey($pc)) {
                    # Unknown card found
                    if (-not $unknownCards.ContainsKey($pc)) {
                        $unknownCards[$pc] = @{
                            Name      = $pn
                            Count     = 0
                            FirstFile = $file.Name
                            FirstPort = $obj.Port
                            Files     = @()
                        }
                    }
                    $unknownCards[$pc].Count++
                    $unknownCards[$pc].Files += $file.Name

                    if (-not $fileFindings.ContainsKey($file.Name)) {
                        $fileFindings[$file.Name] = @()
                    }
                    $fileFindings[$file.Name] += @{
                        Port        = $obj.Port
                        ProductCode = $pc
                        ProductName = $pn
                    }
                }
            }
        }
    } catch {
        Write-Warning "Failed to parse $($file.Name): $($_.Exception.Message)"
    }
}

Write-Progress -Activity "Analyzing Backups" -Completed

# --- Step 3: Generate report ---

$reportLines = @()
$reportLines += "CIP Device Backup - Unknown Port Card Report"
$reportLines += "Generated: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')"
$reportLines += "Backups scanned: $($backupFiles.Count)"
$reportLines += ""

if ($unknownCards.Count -eq 0) {
    $msg = "No unknown port card types found. All port cards have matching definitions."
    $reportLines += $msg
    Write-Host ""
    Write-Host $msg -ForegroundColor Green
} else {
    Write-Host ""
    Write-Host "=== Unknown Port Card Types ===" -ForegroundColor Yellow
    $reportLines += "=== Unknown Port Card Types ==="
    $reportLines += ""

    foreach ($entry in $unknownCards.GetEnumerator() | Sort-Object { $_.Value.Count } -Descending) {
        $pc = $entry.Key
        $info = $entry.Value
        $uniqueFiles = ($info.Files | Sort-Object -Unique).Count
        $line = "ProductCode $pc `"$($info.Name)`" - found in $uniqueFiles backup(s)"
        $detail = "  First seen: $($info.FirstFile), Port $($info.FirstPort)"
        $cmd = "  Generate:   .\devicebackups\Generate-PortCard.ps1 -BackupFile devicebackups\$($info.FirstFile) -Port $($info.FirstPort)"

        $reportLines += $line
        $reportLines += $detail
        $reportLines += $cmd
        $reportLines += ""

        Write-Host "  $line" -ForegroundColor Yellow
        Write-Host "  $detail" -ForegroundColor Gray
        Write-Host "  $cmd" -ForegroundColor Cyan
        Write-Host ""
    }

    if ($fileFindings.Count -gt 0) {
        $reportLines += "=== Backups Containing Unknown Cards ==="
        $reportLines += ""

        foreach ($entry in $fileFindings.GetEnumerator() | Sort-Object Name) {
            $reportLines += $entry.Key
            foreach ($finding in $entry.Value) {
                $reportLines += "  Port $($finding.Port): ProductCode $($finding.ProductCode) `"$($finding.ProductName)`""
            }
            $reportLines += ""
        }
    }
}

$reportFile = Join-Path $BackupDir "unknown_cards_report.txt"
$reportLines | Out-File $reportFile -Encoding UTF8
Write-Host "Report saved to: $reportFile" -ForegroundColor Cyan
