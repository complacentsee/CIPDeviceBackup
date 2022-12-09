using Newtonsoft.Json;
using powerFlexBackup.cipdevice.deviceParameterObjects;

namespace powerFlexBackup.cipdevice
{
    [SupportedDevice("Generic CIP Devices")] 
    public class CIPDevice_Generic : CIPDevice{
        public CIPDevice_Generic(String deviceAddress, Sres.Net.EEIP.EEIPClient eeipClient) :
            base(deviceAddress, eeipClient)
        {
            setDeviceIsGeneric();
            setParameterClassID(0x0F);
            setInstanceAttributes();
        }

        public override void setInstanceAttributes(int instance = 0){
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
    }
}