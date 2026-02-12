# CIPDeviceBackup

Application to read Ethernet/IP (CIP) device parameters and save them to a file.

## Description

CIPDeviceBackup connects to an Ethernet/IP (CIP) device, reads supported parameters, and writes the collected data to a file for backup or analysis purposes.

## Supported Devices

Optimized polling is implemented for the following devices:

- E1 / 193-ETN Ethernet Starters  
  - DeviceType: 3,  ProductCode: 300

- E300 Ethernet Starters  
  - DeviceType: 3, ProductCode: 651

- PowerFlex 40 Series  
  - DeviceType: 127, ProductCode: 40

- PowerFlex 520 Series  
  - DeviceType: 150, ProductCode: 9  
  - DeviceType: 151, ProductCode: 8  
  - DeviceType: 151, ProductCode: 9

- PowerFlex 70 Series  
  - DeviceType: 123, ProductCode: 50

Standard polling of each parameter: 

- PowerFlex 750 Series  
  - DeviceType: 123, ProductCode: 1168  
  - DeviceType: 123, ProductCode: 2192  
  - DeviceType: 142, ProductCode: 1168  
  - DeviceType: 142, ProductCode: 2192  
  - DeviceType: 143, ProductCode: 2192

- PowerFlex 750TS Series  
  - DeviceType: 143, ProductCode: 7568

Also supported:

- Generic CIP devices, unlikely to produce usable results but can be used to generate new specific device configurations. 

## Usage

CIPDeviceBackup [options]

### Single Device Backup

Back up a single device by specifying the host and optionally a CIP route.

### Batch Backup from L5X File

Back up all supported devices in a Logix Designer project by providing an L5X export file. The tool parses the module tree, identifies supported devices, builds the CIP route to each one, and backs them up sequentially.

The L5X parser:
- Reads all `<Module>` entries from the project file
- Skips inhibited modules
- Filters to only devices matching a supported device type/product code
- Builds CIP routes automatically from the module hierarchy (ParentModule, ParentModPortId, and upstream Ethernet port addresses)
- Names each backup file after the module's Name attribute

## Options

--host \<host\> (REQUIRED)
The host PLC to connect through.

--CIProute \<CIProute\>
The CIP route for a single device to read and record parameters from.
Cannot be used with --l5x.

--l5x \<filepath\>
Path to an L5X file (Logix Designer project export). Parses all modules, builds CIP routes, and backs up each supported device.
Cannot be used with --CIProute.
When used, --output is treated as a folder path.

-o, --output \<output\>
The file location to save the output.
Default: devicebackups\parameterbackup.txt
When used with --l5x, this is treated as a folder and each device is saved as {ModuleName}.txt within it.

-a, --all
Collect and save all parameter values.

-v, --verbose
Print parameters and values to console while collecting.

--noping
Skip pinging address prior to attempting connection.
Useful for devices behind firewalls with ICMP disabled.

--timeout \<timeout\>
Set ping and CIP connection timeout in seconds.

--version
Show version information.

-?, -h, --help
Show help and usage information.

## Examples

### Single Device

Read a device and save output to the default location:
```powershell
CIPDeviceBackup --host 192.168.1.10
```

Specify an explicit output file:
```powershell
CIPDeviceBackup --host 192.168.1.10 --output C:\temp\pf_backup.txt
```

Collect all parameters and print values to the console:
```powershell
CIPDeviceBackup --host 192.168.1.10 --all --verbose
```

Skip ICMP ping checks and increase timeout:
```powershell
CIPDeviceBackup --host 192.168.1.10 --noping --timeout 10
```

Use a CIP route to reach a device through the PLC:
```powershell
CIPDeviceBackup --host 192.168.1.10 --CIProute "3,10.39.30.94"
```

### Batch Backup from L5X

Back up all supported devices from a Logix Designer project export:
```powershell
CIPDeviceBackup --host 10.39.30.1 --l5x project.l5x --output C:\backups\myproject
```

This will:
1. Parse the L5X file and list all modules
2. Print which devices will be backed up and which are skipped (with reasons)
3. Connect to each supported device via its auto-built CIP route
4. Save each backup as `C:\backups\myproject\{ModuleName}.txt`
5. Print a summary of successes and failures

## Output

Collected parameter data is written as JSON to the file specified by --output.

Default output path:

devicebackups\parameterbackup.txt

When using --l5x, output is a folder containing one file per device named {ModuleName}.txt.

## Notes

- The --host option is always required. For --l5x mode, this is the PLC you connect through to reach the devices in the project.
- --l5x and --CIProute are mutually exclusive.
- Inhibited modules in the L5X file are automatically skipped.
- Only devices matching a supported device type/product code are included in L5X batch backups. Unsupported modules (controllers, adapters, I/O cards, etc.) are skipped with a reason displayed.
- If a device fails during batch backup, the error is logged and the tool continues to the next device.
- Timeout values apply to both ping checks and CIP connection attempts.
- Devices listed with optimized polling use device-specific polling logic; generic CIP devices use standard CIP reads.
