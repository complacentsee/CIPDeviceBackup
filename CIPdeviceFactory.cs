using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using powerFlexBackup.cipdevice;
using powerFlexBackup.cipdevice.deviceParameterObjects;

namespace powerFlexBackup
{

    public class CIPDeviceFactory
    {
        CIPDeviceDictionary DeviceDictionary;
        public CIPDeviceFactory()
        {
            DeviceDictionary = new CIPDeviceDictionary();
        }

        public CIPDevice getDevicefromAddress(String driveAddress, Sres.Net.EEIP.EEIPClient eeipClient){
            
            eeipClient.RegisterSession(driveAddress);
            var rawIdentityObject = getRawIdentiyObjectfromSession(eeipClient);
            var deviceType = getIdentiyObjectDeviceTypefromRaw(rawIdentityObject);
            var productCode = getIdentiyObjectProductCodefromRaw(rawIdentityObject);
            var identityObjectClassString = DeviceDictionary.getIdentityObjectClass(deviceType, productCode);
            Type DeviceClass = Type.GetType(DeviceDictionary.getCIPDeviceClass(deviceType, productCode));
            Type IdentityObjectClass = Type.GetType(identityObjectClassString);

            var identityObject = (IdentityObject)Activator.CreateInstance(IdentityObjectClass);

            eeipClient.UnRegisterSession();

            return (CIPDevice)Activator.CreateInstance(DeviceClass, new object[] {driveAddress, identityObject, eeipClient});
        }

        private static byte[] getRawIdentiyObjectfromSession(Sres.Net.EEIP.EEIPClient eeipClient)
        {
            return eeipClient.GetAttributeAll(0x01, 1);
        }

        private static int getIdentiyObjectDeviceTypefromRaw(byte[] rawIdentityObject)
        {
            return Convert.ToUInt16(rawIdentityObject[2]
                                        | rawIdentityObject[3] << 8);
        }

        private static int getIdentiyObjectProductCodefromRaw(byte[] rawIdentityObject)
        {
            return Convert.ToUInt16(rawIdentityObject[4]
                                        | rawIdentityObject[5] << 8);
        }
    }
}