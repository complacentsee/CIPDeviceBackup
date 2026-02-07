using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using powerFlexBackup.cipdevice.deviceParameterObjects;

namespace powerFlexBackup.cipdevice
{
    /// <summary>
    /// E1/193-EDN DeviceNet Starters
    /// </summary>
    [SupportedDevice("E1/193-EDN DeviceNET Starter", 3, true)]
    public class CIPDevice_193EDN : CIPDevice{
        public CIPDevice_193EDN(String deviceAddress, Sres.Net.EEIP.EEIPClient eeipClient, byte[] CIPRoute, IOptions<AppConfiguration> options, ILogger logger, IdentityObject identityObject) :
            base(deviceAddress, eeipClient, CIPRoute, options, logger, identityObject)
        {
            setParameterClassID(0x0F);
            setInstanceAttributes();
            setDeviceParameterList(JsonConvert.DeserializeObject<List<DeviceParameter>>(parameterListJSON)!);
        }

        public override void getDeviceParameterValues(){
            getDeviceParameterValuesCIPStandardCompliant();
        }
        public override void getAllDeviceParameters(){
            getAllDeviceParametersCIPStandardCompliant();
        }

        public override int readDeviceParameterMaxNumber(){
            return readDeviceParameterMaxNumberCIPStandardCompliant();
        }
        public string parameterListJSON = @"[
            { 'number': '1', 'name': 'Average %FLA', 'defaultValue': '0', 'record': 'false', 'type': 'xw==' },
            { 'number': '2', 'name': '% Therm Utilized', 'defaultValue': '0', 'record': 'false', 'type': 'xg==' },
            { 'number': '3', 'name': 'Trip Status', 'defaultValue': '0000000000000000', 'record': 'false', 'type': '0g==' },
            { 'number': '4', 'name': 'Warning Status', 'defaultValue': '0000000000000000', 'record': 'false', 'type': '0g==' },
            { 'number': '5', 'name': 'Trip Log 0', 'defaultValue': '0000000000000000', 'record': 'false', 'type': '0g==' },
            { 'number': '6', 'name': 'Trip Log 1', 'defaultValue': '0000000000000000', 'record': 'false', 'type': '0g==' },
            { 'number': '7', 'name': 'Trip Log 2', 'defaultValue': '0000000000000000', 'record': 'false', 'type': '0g==' },
            { 'number': '8', 'name': 'Trip Log 3', 'defaultValue': '0000000000000000', 'record': 'false', 'type': '0g==' },
            { 'number': '9', 'name': 'Trip Log 4', 'defaultValue': '0000000000000000', 'record': 'false', 'type': '0g==' },
            { 'number': '10', 'name': 'Device Status', 'defaultValue': '0000000000000000', 'record': 'false', 'type': '0g==' },
            { 'number': '11', 'name': 'Firmware', 'defaultValue': '0', 'record': 'false', 'type': 'xw==' },
            { 'number': '12', 'name': 'Trip Enable', 'defaultValue': '0000000000000000', 'record': 'true', 'type': '0g==' },
            { 'number': '13', 'name': 'Warning Enable', 'defaultValue': '0000000000000000', 'record': 'true', 'type': '0g==' },
            { 'number': '14', 'name': 'Trip Reset', 'defaultValue': '0', 'record': 'true', 'type': 'xg==' },
            { 'number': '15', 'name': 'Single/Three Ph', 'defaultValue': '1', 'record': 'true', 'type': 'xg==' },
            { 'number': '16', 'name': 'OL Reset Mode', 'defaultValue': '0', 'record': 'true', 'type': 'xg==' },
            { 'number': '17', 'name': 'OL Warning Level', 'defaultValue': '90', 'record': 'true', 'type': 'xg==' },
            { 'number': '18', 'name': 'Jam Inhibit Time', 'defaultValue': '10', 'record': 'true', 'type': 'xg==' },
            { 'number': '19', 'name': 'Jam Trip Delay', 'defaultValue': '50', 'record': 'true', 'type': 'xg==' },
            { 'number': '20', 'name': 'Jam Trip Level', 'defaultValue': '250', 'record': 'true', 'type': 'xw==' },
            { 'number': '21', 'name': 'Jam Warn Level', 'defaultValue': '150', 'record': 'true', 'type': 'xw==' },
            { 'number': '22', 'name': 'UL Inhibit Time', 'defaultValue': '10', 'record': 'true', 'type': 'xg==' },
            { 'number': '23', 'name': 'UL Warn Level', 'defaultValue': '70', 'record': 'true', 'type': 'xg==' },
            { 'number': '24', 'name': 'Program Lock', 'defaultValue': 'False', 'record': 'true', 'type': 'wQ==' },
            { 'number': '25', 'name': 'Set To Defaults', 'defaultValue': 'False', 'record': 'true', 'type': 'wQ==' },
            { 'number': '26', 'name': 'AutoBaudEnable', 'defaultValue': 'True', 'record': 'true', 'type': 'wQ==' },
            { 'number': '27', 'name': 'NonVol Baud Rate', 'defaultValue': '0', 'record': 'false', 'type': 'xg==' },
            { 'number': '28', 'name': 'Output Assembly', 'defaultValue': '103', 'record': 'true', 'type': 'xg==' },
            { 'number': '29', 'name': 'Input Assembly', 'defaultValue': '110', 'record': 'true', 'type': 'xg==' },
            { 'number': '30', 'name': 'Assy Word0 Param', 'defaultValue': '1', 'record': 'true', 'type': 'xg==' },
            { 'number': '31', 'name': 'Assy Word1 Param', 'defaultValue': '2', 'record': 'true', 'type': 'xg==' },
            { 'number': '32', 'name': 'Assy Word2 Param', 'defaultValue': '3', 'record': 'true', 'type': 'xg==' },
            { 'number': '33', 'name': 'Assy Word3 Param', 'defaultValue': '4', 'record': 'true', 'type': 'xg==' },
            { 'number': '34', 'name': 'OutA Pr FltState', 'defaultValue': 'False', 'record': 'true', 'type': 'wQ==' },
            { 'number': '35', 'name': 'OutA Pr FltValue', 'defaultValue': 'False', 'record': 'true', 'type': 'wQ==' },
            { 'number': '36', 'name': 'OutA Dn FltState', 'defaultValue': 'False', 'record': 'true', 'type': 'wQ==' },
            { 'number': '37', 'name': 'OutA Dn FltValue', 'defaultValue': 'False', 'record': 'true', 'type': 'wQ==' },
            { 'number': '38', 'name': 'OutA Dn IdlState', 'defaultValue': 'False', 'record': 'true', 'type': 'wQ==' },
            { 'number': '39', 'name': 'OutA Dn IdlValue', 'defaultValue': 'False', 'record': 'true', 'type': 'wQ==' },
            { 'number': '40', 'name': 'IN1 Assignment', 'defaultValue': '0', 'record': 'true', 'type': 'xg==' },
            { 'number': '41', 'name': 'IN2 Assignment', 'defaultValue': '0', 'record': 'true', 'type': 'xg==' }
        ]";
    }
}
