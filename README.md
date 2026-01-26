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

## Options

--host <host> (REQUIRED)  
The host to read and record parameters from.

--CIProute <CIProute>  
The CIP route for the device to read and record parameters from.

-o, --output <output>  
The file location to save the output.  
Default: devicebackups\parameterbackup.txt

-a, --all  
Collect and save all parameter values.

-to, -v, --verbose  
Print parameters and values to console while collecting.

--noping  
Skip pinging address prior to attempting connection.  
Useful for devices behind firewalls with ICMP disabled.

--timeout <timeout>  
Set ping and CIP connection timeout in seconds.

--version  
Show version information.

-?, -h, --help  
Show help and usage information.

## Examples

Read a device and save output to the default location:
```powershell
CIPDeviceBackup --host 192.168.1.10
```

Specify an explicit output file:
```powershell
CIPDeviceBackup --host 192.168.1.10 --output C:\temp\pf_backup.txt```
```
Collect all parameters and print values to the console:
```powershell
CIPDeviceBackup --host 192.168.1.10 --all --verbose
```
Skip ICMP ping checks and increase timeout:
```powershell
CIPDeviceBackup --host 192.168.1.10 --noping --timeout 10
```
Use a CIP route (format depends on device and network topology):
```powershell
CIPDeviceBackup --host 192.168.1.10 --CIProute "<route>"
```
## Output

Collected parameter data is written to the file specified by --output.

Default output path:

devicebackups\parameterbackup.txt

## Notes

- The --host option is required.
- Timeout values apply to both ping checks and CIP connection attempts.
- Devices listed with optimized polling use device-specific polling logic; generic CIP devices use standard CIP reads.
