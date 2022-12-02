using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace powerFlexBackup.cipdevice
{
    public class IdentityObject_PowerFlex525 : IdentityObject
    {
        public override IdentityObject_PowerFlex525 getIdentityObjectfromResponse(byte[] IdentityObjectBytes)
        {
            IdentityObject_PowerFlex525 IdentityObject = new IdentityObject_PowerFlex525();
            
            IdentityObject.VendorID = Convert.ToUInt16(IdentityObjectBytes[0]
                                                        | IdentityObjectBytes[1] << 8);
            Globals.logger.LogDebug ("VendorID: " + IdentityObject.VendorID);

            IdentityObject.DeviceType = Convert.ToUInt16(IdentityObjectBytes[2]
                                                        | IdentityObjectBytes[3] << 8);
            Globals.logger.LogDebug ("DeviceType: " + IdentityObject.DeviceType);

            IdentityObject.ProductCode = Convert.ToUInt16(IdentityObjectBytes[4]
                                                        | IdentityObjectBytes[5] << 8);
            Globals.logger.LogDebug ("ProductCode: " + IdentityObject.ProductCode);

            IdentityObject.revision.Major = IdentityObjectBytes[6];
            IdentityObject.revision.Minor = IdentityObjectBytes[7];
            Globals.logger.LogDebug ("Revision: " + IdentityObject.revision.Major + "." + IdentityObject.revision.Minor);

            IdentityObject.Status = Convert.ToUInt16(IdentityObjectBytes[8]
                                                        | IdentityObjectBytes[9] << 8);
            Globals.logger.LogDebug ("Status: " + IdentityObject.Status);

            IdentityObject.SerialNumber = (UInt32)(IdentityObjectBytes[10]
                                                            | IdentityObjectBytes[11] << 8
                                                            | IdentityObjectBytes[12] << 16
                                                            | IdentityObjectBytes[13] << 24);
            Globals.logger.LogDebug ("SerialNumber: {0:X}", IdentityObject.SerialNumber);

            IdentityObject.ProductName = System.Text.Encoding.ASCII.GetString(IdentityObjectBytes, 15, Convert.ToInt32(IdentityObjectBytes[14]));
            Globals.logger.LogDebug ("ProductName: " + IdentityObject.ProductName);
                                             
            return IdentityObject;
        }
    }
}
