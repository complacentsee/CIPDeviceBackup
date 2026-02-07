using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using powerFlexBackup.cipdevice.deviceParameterObjects;

namespace powerFlexBackup.cipdevice
{
    /// <summary>
    /// E3 Plus Motor Protection Relay (DeviceNet)
    /// Product Code 5 indicates E3 (3-15A) variant
    /// </summary>
    [SupportedDevice("E3 (3-15A)", 3, 5, true)]
    public class CIPDevice_E3Plus : CIPDevice{
        public CIPDevice_E3Plus(String deviceAddress, Sres.Net.EEIP.EEIPClient eeipClient, byte[] CIPRoute, IOptions<AppConfiguration> options, ILogger logger, IdentityObject identityObject) :
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
            { 'number': '1', 'name': 'L1 Current', 'defaultValue': '0', 'record': 'false', 'type': 'Aw==' },
            { 'number': '2', 'name': 'L2 Current', 'defaultValue': '0', 'record': 'false', 'type': 'Aw==' },
            { 'number': '3', 'name': 'L3 Current', 'defaultValue': '0', 'record': 'false', 'type': 'Aw==' },
            { 'number': '4', 'name': 'Average Current', 'defaultValue': '0', 'record': 'false', 'type': 'Aw==' },
            { 'number': '5', 'name': 'L1 %FLA', 'defaultValue': '0', 'record': 'false', 'type': 'Ag==' },
            { 'number': '6', 'name': 'L2 %FLA', 'defaultValue': '0', 'record': 'false', 'type': 'Ag==' },
            { 'number': '7', 'name': 'L3 %FLA', 'defaultValue': '0', 'record': 'false', 'type': 'Ag==' },
            { 'number': '8', 'name': 'Average %FLA', 'defaultValue': '0', 'record': 'false', 'type': 'Ag==' },
            { 'number': '9', 'name': '% Therm Utilized', 'defaultValue': '0', 'record': 'false', 'type': 'CA==' },
            { 'number': '10', 'name': 'Reserved', 'defaultValue': '0', 'record': 'false', 'type': 'Ag==' },
            { 'number': '11', 'name': 'Current Imbal', 'defaultValue': '0', 'record': 'false', 'type': 'CA==' },
            { 'number': '12', 'name': 'OL Time To Trip', 'defaultValue': '0', 'record': 'false', 'type': 'Ag==' },
            { 'number': '13', 'name': 'OL Time To Reset', 'defaultValue': '0', 'record': 'false', 'type': 'Ag==' },
            { 'number': '14', 'name': 'Trip Status', 'defaultValue': '00000000', 'record': 'false', 'type': 'AQ==' },
            { 'number': '15', 'name': 'Warning Status', 'defaultValue': '00000000', 'record': 'false', 'type': 'AQ==' },
            { 'number': '16', 'name': 'Trip Log 0', 'defaultValue': '00000000', 'record': 'false', 'type': 'AQ==' },
            { 'number': '17', 'name': 'Trip Log 1', 'defaultValue': '00000000', 'record': 'false', 'type': 'AQ==' },
            { 'number': '18', 'name': 'Trip Log 2', 'defaultValue': '00000000', 'record': 'false', 'type': 'AQ==' },
            { 'number': '19', 'name': 'Trip Log 3', 'defaultValue': '00000000', 'record': 'false', 'type': 'AQ==' },
            { 'number': '20', 'name': 'Trip Log 4', 'defaultValue': '00000000', 'record': 'false', 'type': 'AQ==' },
            { 'number': '21', 'name': 'Device Status', 'defaultValue': '00000000', 'record': 'false', 'type': 'AQ==' },
            { 'number': '22', 'name': 'Firmware', 'defaultValue': '0', 'record': 'false', 'type': 'Ag==' },
            { 'number': '23', 'name': 'Dev Config', 'defaultValue': '00000000', 'record': 'false', 'type': 'AQ==' },
            { 'number': '24', 'name': 'Trip Enable', 'defaultValue': '00000110', 'record': 'true', 'type': 'AQ==' },
            { 'number': '25', 'name': 'Warning Enable', 'defaultValue': '00000000', 'record': 'true', 'type': 'AQ==' },
            { 'number': '26', 'name': 'Trip Reset', 'defaultValue': '0', 'record': 'true', 'type': 'CA==' },
            { 'number': '27', 'name': 'Single/Three Ph', 'defaultValue': '1', 'record': 'true', 'type': 'CA==' },
            { 'number': '28', 'name': 'FLA Setting', 'defaultValue': '300', 'record': 'true', 'type': 'Aw==' },
            { 'number': '29', 'name': 'Trip Class', 'defaultValue': '10', 'record': 'true', 'type': 'CA==' },
            { 'number': '30', 'name': 'OL/PTC ResetMode', 'defaultValue': '0', 'record': 'true', 'type': 'CA==' },
            { 'number': '31', 'name': 'OL Reset Level', 'defaultValue': '75', 'record': 'true', 'type': 'CA==' },
            { 'number': '32', 'name': 'OL Warning Level', 'defaultValue': '85', 'record': 'true', 'type': 'CA==' },
            { 'number': '33', 'name': 'PL Inhibit Time', 'defaultValue': '0', 'record': 'true', 'type': 'CA==' },
            { 'number': '34', 'name': 'PL Trip Delay', 'defaultValue': '10', 'record': 'true', 'type': 'CA==' },
            { 'number': '35', 'name': 'Reserved', 'defaultValue': '0', 'record': 'false', 'type': 'Ag==' },
            { 'number': '36', 'name': 'Reserved', 'defaultValue': '0', 'record': 'false', 'type': 'Ag==' },
            { 'number': '37', 'name': 'Reserved', 'defaultValue': '0', 'record': 'false', 'type': 'Ag==' },
            { 'number': '38', 'name': 'Reserved', 'defaultValue': '0', 'record': 'false', 'type': 'Ag==' },
            { 'number': '39', 'name': 'Stall Enbld Time', 'defaultValue': '10', 'record': 'true', 'type': 'CA==' },
            { 'number': '40', 'name': 'Stall Trip Level', 'defaultValue': '600', 'record': 'true', 'type': 'Ag==' },
            { 'number': '41', 'name': 'Jam Inhibit Time', 'defaultValue': '10', 'record': 'true', 'type': 'CA==' },
            { 'number': '42', 'name': 'Jam Trip Delay', 'defaultValue': '50', 'record': 'true', 'type': 'CA==' },
            { 'number': '43', 'name': 'Jam Trip Level', 'defaultValue': '250', 'record': 'true', 'type': 'Ag==' },
            { 'number': '44', 'name': 'Jam Warn Level', 'defaultValue': '150', 'record': 'true', 'type': 'Ag==' },
            { 'number': '45', 'name': 'UL Inhibit Time', 'defaultValue': '10', 'record': 'true', 'type': 'CA==' },
            { 'number': '46', 'name': 'UL Trip Delay', 'defaultValue': '50', 'record': 'true', 'type': 'CA==' },
            { 'number': '47', 'name': 'UL Trip Level', 'defaultValue': '50', 'record': 'true', 'type': 'CA==' },
            { 'number': '48', 'name': 'UL Warn Level', 'defaultValue': '70', 'record': 'true', 'type': 'CA==' },
            { 'number': '49', 'name': 'CI Inhibit Time', 'defaultValue': '10', 'record': 'true', 'type': 'CA==' },
            { 'number': '50', 'name': 'CI Trip Delay', 'defaultValue': '50', 'record': 'true', 'type': 'CA==' },
            { 'number': '51', 'name': 'CI Trip Level', 'defaultValue': '35', 'record': 'true', 'type': 'CA==' },
            { 'number': '52', 'name': 'CI Warn Level', 'defaultValue': '20', 'record': 'true', 'type': 'CA==' },
            { 'number': '53', 'name': 'Program Lock', 'defaultValue': 'False', 'record': 'true', 'type': 'BA==' },
            { 'number': '54', 'name': 'Set To Defaults', 'defaultValue': 'False', 'record': 'true', 'type': 'BA==' },
            { 'number': '55', 'name': 'AutoBaudEnable', 'defaultValue': 'True', 'record': 'true', 'type': 'BA==' },
            { 'number': '56', 'name': 'NonVol Baud Rate', 'defaultValue': '0', 'record': 'true', 'type': 'CA==' },
            { 'number': '57', 'name': 'Reserved', 'defaultValue': '0', 'record': 'false', 'type': 'Ag==' },
            { 'number': '58', 'name': 'COS Mask', 'defaultValue': '00000000', 'record': 'true', 'type': 'AQ==' },
            { 'number': '59', 'name': 'Output Assembly', 'defaultValue': '103', 'record': 'true', 'type': 'CA==' },
            { 'number': '60', 'name': 'Input Assembly', 'defaultValue': '100', 'record': 'true', 'type': 'CA==' },
            { 'number': '61', 'name': 'Assy Word0 Param', 'defaultValue': '21', 'record': 'true', 'type': 'CA==' },
            { 'number': '62', 'name': 'Assy Word1 Param', 'defaultValue': '1', 'record': 'true', 'type': 'CA==' },
            { 'number': '63', 'name': 'Assy Word2 Param', 'defaultValue': '2', 'record': 'true', 'type': 'CA==' },
            { 'number': '64', 'name': 'Assy Word3 Param', 'defaultValue': '3', 'record': 'true', 'type': 'CA==' },
            { 'number': '65', 'name': 'OutA Pr FltState', 'defaultValue': 'False', 'record': 'true', 'type': 'BA==' },
            { 'number': '66', 'name': 'OutA Pr FltValue', 'defaultValue': 'False', 'record': 'true', 'type': 'BA==' },
            { 'number': '67', 'name': 'OutA Dn FltState', 'defaultValue': 'False', 'record': 'true', 'type': 'BA==' },
            { 'number': '68', 'name': 'OutA Dn FltValue', 'defaultValue': 'False', 'record': 'true', 'type': 'BA==' },
            { 'number': '69', 'name': 'OutA Dn IdlState', 'defaultValue': 'False', 'record': 'true', 'type': 'BA==' },
            { 'number': '70', 'name': 'OutA Dn IdlValue', 'defaultValue': 'False', 'record': 'true', 'type': 'BA==' },
            { 'number': '71', 'name': 'Reserved', 'defaultValue': '0', 'record': 'false', 'type': 'Ag==' },
            { 'number': '72', 'name': 'Reserved', 'defaultValue': '0', 'record': 'false', 'type': 'Ag==' },
            { 'number': '73', 'name': 'Reserved', 'defaultValue': '0', 'record': 'false', 'type': 'Ag==' },
            { 'number': '74', 'name': 'Reserved', 'defaultValue': '0', 'record': 'false', 'type': 'Ag==' },
            { 'number': '75', 'name': 'Reserved', 'defaultValue': '0', 'record': 'false', 'type': 'Ag==' },
            { 'number': '76', 'name': 'Reserved', 'defaultValue': '0', 'record': 'false', 'type': 'Ag==' },
            { 'number': '77', 'name': 'Reserved', 'defaultValue': '0', 'record': 'false', 'type': 'Ag==' },
            { 'number': '78', 'name': 'Reserved', 'defaultValue': '0', 'record': 'false', 'type': 'Ag==' },
            { 'number': '79', 'name': 'Reserved', 'defaultValue': '0', 'record': 'false', 'type': 'Ag==' },
            { 'number': '80', 'name': 'Reserved', 'defaultValue': '0', 'record': 'false', 'type': 'Ag==' },
            { 'number': '81', 'name': 'Reserved', 'defaultValue': '0', 'record': 'false', 'type': 'Ag==' },
            { 'number': '82', 'name': 'Reserved', 'defaultValue': '0', 'record': 'false', 'type': 'Ag==' },
            { 'number': '83', 'name': 'IN1 Assignment', 'defaultValue': '0', 'record': 'true', 'type': 'CA==' },
            { 'number': '84', 'name': 'IN2 Assignment', 'defaultValue': '0', 'record': 'true', 'type': 'CA==' },
            { 'number': '85', 'name': 'Reserved', 'defaultValue': '0', 'record': 'false', 'type': 'Ag==' },
            { 'number': '86', 'name': 'Reserved', 'defaultValue': '0', 'record': 'false', 'type': 'Ag==' },
            { 'number': '87', 'name': 'Reserved', 'defaultValue': '0', 'record': 'false', 'type': 'Ag==' },
            { 'number': '88', 'name': 'Reserved', 'defaultValue': '0', 'record': 'false', 'type': 'Ag==' },
            { 'number': '89', 'name': 'Reserved', 'defaultValue': '0', 'record': 'false', 'type': 'Ag==' },
            { 'number': '90', 'name': 'Warn Log 0', 'defaultValue': '00000000', 'record': 'false', 'type': 'AQ==' },
            { 'number': '91', 'name': 'Warn Log 1', 'defaultValue': '00000000', 'record': 'false', 'type': 'AQ==' },
            { 'number': '92', 'name': 'Warn Log 2', 'defaultValue': '00000000', 'record': 'false', 'type': 'AQ==' },
            { 'number': '93', 'name': 'Warn Log 3', 'defaultValue': '00000000', 'record': 'false', 'type': 'AQ==' },
            { 'number': '94', 'name': 'Warn Log 4', 'defaultValue': '00000000', 'record': 'false', 'type': 'AQ==' },
            { 'number': '95', 'name': 'Elapsed Time', 'defaultValue': '0', 'record': 'false', 'type': 'Ag==' },
            { 'number': '96', 'name': 'Starts Counter', 'defaultValue': '0', 'record': 'false', 'type': 'Ag==' },
            { 'number': '97', 'name': 'Starts Available', 'defaultValue': '0', 'record': 'false', 'type': 'CA==' },
            { 'number': '98', 'name': 'Time to Start', 'defaultValue': '0', 'record': 'false', 'type': 'Ag==' },
            { 'number': '99', 'name': 'Starts/Hour', 'defaultValue': '2', 'record': 'true', 'type': 'CA==' },
            { 'number': '100', 'name': 'Starts Interval', 'defaultValue': '600', 'record': 'true', 'type': 'Ag==' },
            { 'number': '101', 'name': 'PM - # Starts', 'defaultValue': '0', 'record': 'true', 'type': 'Ag==' },
            { 'number': '102', 'name': 'PM - Oper. Hours', 'defaultValue': '0', 'record': 'true', 'type': 'Ag==' },
            { 'number': '103', 'name': 'Test Enable', 'defaultValue': '1', 'record': 'true', 'type': 'CA==' },
            { 'number': '104', 'name': 'Clear Queue', 'defaultValue': 'False', 'record': 'true', 'type': 'BA==' }
        ]";
    }
}
