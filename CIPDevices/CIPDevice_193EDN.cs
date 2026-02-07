using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using powerFlexBackup.cipdevice.deviceParameterObjects;

namespace powerFlexBackup.cipdevice
{
    /// <summary>
    /// E1/193-EDN DeviceNet Starters
    /// </summary>
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
            { 'number': 1, 'name': null, 'defaultValue': null, 'record': false, 'type': null },
            { 'number': 2, 'name': null, 'defaultValue': null, 'record': false, 'type': null },
            { 'number': 3, 'name': null, 'defaultValue': null, 'record': false, 'type': null },
            { 'number': 4, 'name': null, 'defaultValue': null, 'record': false, 'type': null },
            { 'number': 5, 'name': null, 'defaultValue': null, 'record': false, 'type': null },
            { 'number': 6, 'name': null, 'defaultValue': null, 'record': false, 'type': null },
            { 'number': 7, 'name': null, 'defaultValue': null, 'record': false, 'type': null },
            { 'number': 8, 'name': null, 'defaultValue': null, 'record': false, 'type': null },
            { 'number': 9, 'name': null, 'defaultValue': null, 'record': false, 'type': null },
            { 'number': 10, 'name': null, 'defaultValue': null, 'record': false, 'type': null },
            { 'number': 11, 'name': null, 'defaultValue': null, 'record': false, 'type': null },
            { 'number': 12, 'name': null, 'defaultValue': null, 'record': false, 'type': null },
            { 'number': 13, 'name': null, 'defaultValue': null, 'record': false, 'type': null },
            { 'number': 14, 'name': null, 'defaultValue': null, 'record': false, 'type': null },
            { 'number': 15, 'name': null, 'defaultValue': null, 'record': false, 'type': null },
            { 'number': 16, 'name': null, 'defaultValue': null, 'record': false, 'type': null },
            { 'number': 17, 'name': null, 'defaultValue': null, 'record': false, 'type': null },
            { 'number': 18, 'name': null, 'defaultValue': null, 'record': false, 'type': null },
            { 'number': 19, 'name': null, 'defaultValue': null, 'record': false, 'type': null },
            { 'number': 20, 'name': null, 'defaultValue': null, 'record': false, 'type': null },
            { 'number': 21, 'name': null, 'defaultValue': null, 'record': false, 'type': null },
            { 'number': 22, 'name': null, 'defaultValue': null, 'record': false, 'type': null },
            { 'number': 23, 'name': null, 'defaultValue': null, 'record': false, 'type': null },
            { 'number': 24, 'name': null, 'defaultValue': null, 'record': false, 'type': null },
            { 'number': 25, 'name': null, 'defaultValue': null, 'record': false, 'type': null },
            { 'number': 26, 'name': null, 'defaultValue': null, 'record': false, 'type': null },
            { 'number': 27, 'name': null, 'defaultValue': null, 'record': false, 'type': null },
            { 'number': 28, 'name': null, 'defaultValue': null, 'record': false, 'type': null },
            { 'number': 29, 'name': null, 'defaultValue': null, 'record': false, 'type': null },
            { 'number': 30, 'name': null, 'defaultValue': null, 'record': false, 'type': null },
            { 'number': 31, 'name': null, 'defaultValue': null, 'record': false, 'type': null },
            { 'number': 32, 'name': null, 'defaultValue': null, 'record': false, 'type': null },
            { 'number': 33, 'name': null, 'defaultValue': null, 'record': false, 'type': null },
            { 'number': 34, 'name': null, 'defaultValue': null, 'record': false, 'type': null },
            { 'number': 35, 'name': null, 'defaultValue': null, 'record': false, 'type': null },
            { 'number': 36, 'name': null, 'defaultValue': null, 'record': false, 'type': null },
            { 'number': 37, 'name': null, 'defaultValue': null, 'record': false, 'type': null },
            { 'number': 38, 'name': null, 'defaultValue': null, 'record': false, 'type': null }
        ]";
    }
}
