using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using powerFlexBackup.cipdevice.deviceParameterObjects;

namespace powerFlexBackup.cipdevice
{
    /// <summary>
    /// E3 Plus Motor Protection Relay
    /// </summary>
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
        public string parameterListJSON = @"[]";
    }
}
