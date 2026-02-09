# Code Generators

Scripts to batch-backup PowerFlex 750 series devices, detect unknown port card types, and generate C# class definitions for them.

## Prerequisites

- PowerShell 5.1+ (included with Windows)
- A published build of CIPDeviceBackup:
  ```
  dotnet publish --configuration Release --runtime win-x64 --self-contained true -p:PublishSingleFile=true --output .\publish
  ```

## Workflow

### 1. Populate the device list

Edit `codegenerators/devices.txt` with one IP address per line. Lines starting with `#` are comments.

```
# Line 1 compressors
10.23.38.33
10.23.38.34
# Line 2 pumps
10.38.42.71
```

### 2. Run backups

```powershell
.\codegenerators\Run-Backups.ps1 -DeviceList codegenerators\devices.txt -ExePath .\publish\CIPDeviceBackup.exe
```

This connects to each device sequentially with `--all` (full parameter metadata) and saves a backup to `devicebackups\{ip}.json`. A progress bar tracks completion, and results are logged to `devicebackups\run_log.txt`.

| Parameter | Default | Description |
|-----------|---------|-------------|
| `-DeviceList` | *(required)* | Path to the IP address list |
| `-ExePath` | `CIPDeviceBackup.exe` in project root | Path to the published executable |
| `-NoPing` | `$true` | Skip ICMP ping before connecting |

Failed devices are logged but don't stop the batch. A summary is printed at the end.

### 3. Analyze backups for unknown port cards

```powershell
.\codegenerators\Analyze-Backups.ps1
```

Scans all `.json` files in `devicebackups\`, extracts port card identities (ProductCode, ProductName), and compares against known definitions in `CIPDevices\PortCards\PF750_PortCard_*.cs`. Unknown types are reported with ready-to-run generation commands.

| Parameter | Default | Description |
|-----------|---------|-------------|
| `-BackupDir` | `devicebackups\` | Directory containing backup files |
| `-PortCardDir` | `CIPDevices\PortCards\` | Directory containing port card source files |

Output is saved to `devicebackups\unknown_cards_report.txt`.

### 4. Generate port card classes

For each unknown type from the report:

```powershell
.\codegenerators\Generate-PortCard.ps1 -BackupFile devicebackups\10.23.38.33.json -Port 3
```

This parses the backup, extracts the parameter list with full metadata (type, record/isWritable, name, defaultValue), and generates a C# class at `CIPDevices\PortCards\PF750_PortCard_{Name}.cs`.

| Parameter | Default | Description |
|-----------|---------|-------------|
| `-BackupFile` | *(required)* | Path to the backup `.json` file |
| `-Port` | *(required)* | Port number (1-14) of the expansion card |
| `-ClassName` | Derived from ProductName | Class name suffix (e.g. `CommDNet`) |
| `-OutputDir` | `CIPDevices\PortCards\` | Output directory for generated `.cs` file |

### 5. Build and verify

```powershell
dotnet build
```

Review the generated class and verify:
- `ClassID` is correct (0x9F for HOST, 0x93 for DPI devices)
- Parameter names and types look reasonable
- Dual-class cards (like EtherNet/IP) need manual addition of `DeviceClassID` and `deviceParameterListJSON`

## Notes

- Backups **must** use `--all` to capture type and record metadata. Without it, the generator cannot determine parameter types.
- The analyzer auto-discovers known ProductCodes from source, so it stays in sync as new port cards are added.
- Generated classes default to `ClassID = 0x9F` (HOST memory). Verify this is correct for each card type.
- HIM modules (ProductCode 767) and empty ports (65280) are automatically excluded from the analysis.
