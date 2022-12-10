using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace powerFlexBackup.cipdevice
{
    public class IdentityObject
    {
        public struct Revision
            {
                public byte Major;
                public byte Minor;
            }
        public UInt16 VendorID;
        public UInt16 DeviceType;
        public UInt16 ProductCode;
        public Revision revision;
        public String SerialNumber = "";
        public String ProductName = "";
        public String Host { get; set; } = "";

        [JsonIgnore]
        public UInt16 Status;

        public IdentityObject getIdentityObjectfromResponse(byte[] IdentityObjectBytes)
        {
            if(IdentityObjectBytes.Length < 15){
                Globals.logger.LogError("IdentityObjectBytes is too short");
                throw new Exception("IdentityObjectBytes is too short");
            }
            IdentityObject IdentityObject = new IdentityObject();
            
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

            var SerialNumber = CIPDeviceHelper.ReverseBytes((new ArraySegment<byte>(IdentityObjectBytes, 10, 4)).ToArray());
            IdentityObject.SerialNumber = Convert.ToHexString(SerialNumber);  
            Globals.logger.LogDebug ("SerialNumber: {0:X}", IdentityObject.SerialNumber);

            IdentityObject.ProductName = System.Text.Encoding.ASCII.GetString(IdentityObjectBytes, 15, Convert.ToInt32(IdentityObjectBytes[14]));
            IdentityObject.ProductName = IdentityObject.ProductName.TrimEnd();
            Globals.logger.LogDebug ("ProductName: " + IdentityObject.ProductName);
                                             
            return IdentityObject;
        }

    }
}
