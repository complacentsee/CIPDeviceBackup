<#
.SYNOPSIS
    Generates a PF750_PortCard C# class from a CIP device backup file.

.DESCRIPTION
    Parses a backup JSON file (produced with --all flag), extracts port card identity
    and parameter metadata, and generates a C# class file following the existing
    PowerFlex750PortCard pattern.

    The backup MUST have been created with --all to include type, record, and
    isWritable fields in the parameter data.

.PARAMETER BackupFile
    Path to the backup .json file containing the device data.

.PARAMETER Port
    The port number (1-14) of the expansion card to generate code for.

.PARAMETER ClassName
    Optional class name suffix. If not provided, derived from the ProductName
    by removing spaces and special characters.

.PARAMETER OutputDir
    Output directory for the generated .cs file. Defaults to CIPDevices/PortCards/
    relative to the project root.

.EXAMPLE
    .\devicebackups\Generate-PortCard.ps1 -BackupFile devicebackups\192.168.1.50.json -Port 3
    .\devicebackups\Generate-PortCard.ps1 -BackupFile devicebackups\192.168.1.50.json -Port 5 -ClassName "CommDNet"
#>
param(
    [Parameter(Mandatory = $true)]
    [string]$BackupFile,

    [Parameter(Mandatory = $true)]
    [int]$Port,

    [string]$ClassName = $null,

    [string]$OutputDir = $null
)

$ErrorActionPreference = "Stop"

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$projectRoot = Split-Path -Parent $scriptDir

if (-not $OutputDir) {
    $OutputDir = Join-Path $projectRoot "CIPDevices\PortCards"
}

if (-not (Test-Path $BackupFile)) {
    Write-Error "Backup file not found: $BackupFile"
    exit 1
}

# Known CIP type code mappings (base64 -> hex -> name)
$typeNames = @{
    "xg==" = "0xC6 INT"
    "xw==" = "0xC7 UINT"
    "yA==" = "0xC8 DINT"
    "yg==" = "0xCA REAL"
    "0Q==" = "0xD1 WORD"
    "0g==" = "0xD2 DWORD"
    "0w==" = "0xD3 LWORD"
}

# --- Parse backup file ---

Write-Host "Parsing backup file: $BackupFile" -ForegroundColor Cyan

$raw = Get-Content $BackupFile -Raw
$jsonArray = "[$($raw -replace '}\s*\{', '},{')]"
$objects = $jsonArray | ConvertFrom-Json

# Find the object for the requested port
$portObj = $objects | Where-Object { $_.Port -eq $Port } | Select-Object -First 1

if (-not $portObj) {
    Write-Error "No DeviceParameterObject found for Port $Port in $BackupFile"
    Write-Host "Available ports:" -ForegroundColor Yellow
    $objects | ForEach-Object { Write-Host "  Port $($_.Port): $($_.identityObject.ProductName) (ProductCode $($_.identityObject.ProductCode))" }
    exit 1
}

$identity = $portObj.identityObject
$productCode = [int]$identity.ProductCode
$productName = $identity.ProductName
$params = $portObj.ParameterList

Write-Host "  ProductCode: $productCode" -ForegroundColor Gray
Write-Host "  ProductName: $productName" -ForegroundColor Gray
Write-Host "  Parameters:  $($params.Count)" -ForegroundColor Gray

if ($params.Count -eq 0) {
    Write-Error "No parameters found for Port $Port. Was the backup created with --all flag?"
    exit 1
}

# Check if type info is present (requires --all flag)
$firstParam = $params[0]
$hasTypeInfo = $null -ne $firstParam.type -or $null -ne $firstParam.typeHex
if (-not $hasTypeInfo) {
    Write-Error "Parameter data missing type information. The backup must be created with the --all flag."
    exit 1
}

# --- Derive class name ---

if (-not $ClassName) {
    # Sanitize ProductName: remove special chars, convert spaces to nothing
    $ClassName = $productName -replace '[^a-zA-Z0-9 ]', '' -replace '\s+', ''
    # Handle edge cases
    if ($ClassName -eq "" -or $ClassName -match '^\d') {
        $ClassName = "ProductCode$productCode"
    }
}

$fullClassName = "PF750_PortCard_$ClassName"
$outputFile = Join-Path $OutputDir "$fullClassName.cs"

Write-Host "  Class name:  $fullClassName" -ForegroundColor Gray
Write-Host "  Output file: $outputFile" -ForegroundColor Gray
Write-Host ""

# --- Build parameter list ---

# Collect type info for summary comment
$usedTypes = @{}

$paramLines = @()
foreach ($p in $params) {
    $num = $p.number

    # Get type as base64 string
    $typeBase64 = $null
    if ($p.type) {
        # type field is a byte array that gets serialized as base64
        if ($p.type -is [string]) {
            $typeBase64 = $p.type
        } else {
            # It's a byte array from JSON deserialization - convert to base64
            $typeBase64 = [Convert]::ToBase64String([byte[]]$p.type)
        }
    } elseif ($p.typeHex) {
        # Fall back to typeHex -> bytes -> base64
        $hexStr = $p.typeHex
        $bytes = [byte[]]::new($hexStr.Length / 2)
        for ($j = 0; $j -lt $bytes.Length; $j++) {
            $bytes[$j] = [Convert]::ToByte($hexStr.Substring($j * 2, 2), 16)
        }
        $typeBase64 = [Convert]::ToBase64String($bytes)
    }

    if ($typeBase64 -and -not $usedTypes.ContainsKey($typeBase64)) {
        if ($typeNames.ContainsKey($typeBase64)) {
            $usedTypes[$typeBase64] = $typeNames[$typeBase64]
        } else {
            # Decode hex for unknown types
            $decoded = [Convert]::FromBase64String($typeBase64)
            $hexVal = "0x" + [BitConverter]::ToString($decoded) -replace '-', ''
            $usedTypes[$typeBase64] = "$hexVal UNKNOWN"
        }
    }

    # Determine record flag
    $record = "false"
    if ($null -ne $p.isWritable) {
        $record = if ($p.isWritable) { "true" } else { "false" }
    } elseif ($null -ne $p.record) {
        $record = if ($p.record) { "true" } else { "false" }
    }

    $name = $p.name -replace "'", "\'"
    $defaultVal = if ($p.defaultValue) { $p.defaultValue } else { "0" }

    $paramLine = "            { 'number': '$num', 'name': '$name', 'defaultValue': '$defaultVal', 'record': '$record', 'type': '$typeBase64' }"
    $paramLines += $paramLine
}

# Build type summary for comment
$typeSummary = ($usedTypes.GetEnumerator() | Sort-Object Value | ForEach-Object { "$($_.Key) ($($_.Value))" }) -join ", "

# Count writable/read-only
$writableCount = ($params | Where-Object { $_.isWritable -eq $true -or $_.record -eq $true }).Count
$readOnlyCount = $params.Count - $writableCount

# --- Generate C# file ---

$paramListStr = $paramLines -join ",`n"

$csContent = @"
namespace powerFlexBackup.cipdevice.PortCards
{
    /// <summary>
    /// $productName Port Card
    ///
    /// $($params.Count) parameters ($writableCount writable, $readOnlyCount read-only).
    /// Types: $typeSummary
    /// </summary>
    public class $fullClassName : PowerFlex750PortCard
    {
        public override ushort[] ProductCodes => [$productCode];
        public override string ProductName => "$productName";
        public override int HostClassID => 0x9F;

        protected override string hostParameterListJSON => @"[
$paramListStr
        ]";
    }
}
"@

$csContent | Out-File $outputFile -Encoding UTF8
Write-Host "Generated: $outputFile" -ForegroundColor Green
Write-Host "  $($params.Count) parameters ($writableCount writable, $readOnlyCount read-only)" -ForegroundColor Gray
Write-Host "  Types: $typeSummary" -ForegroundColor Gray
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Cyan
Write-Host "  1. Review the generated file for correctness" -ForegroundColor Gray
Write-Host "  2. Verify ClassID is 0x9F (HOST) - change to 0x93 if this is a DPI-class card" -ForegroundColor Gray
Write-Host "  3. Run 'dotnet build' to compile" -ForegroundColor Gray
