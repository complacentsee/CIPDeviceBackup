using powerFlexBackup.cipdevice.deviceParameterObjects;

namespace powerFlexBackup.cipdevice
{
    [SupportedDevice("Generic CIP Devices", new int(), new int())] 
    public class CIPDevice_Generic : CIPDevice{
        public CIPDevice_Generic(String deviceAddress, Sres.Net.EEIP.EEIPClient eeipClient, byte[] CIPRoute) :
            base(deviceAddress, eeipClient, CIPRoute)
        {
            setDeviceIsGeneric();
            setParameterClassID(0x0F);
            setInstanceAttributes();
            Console.WriteLine("Generic CIP Device Created");
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