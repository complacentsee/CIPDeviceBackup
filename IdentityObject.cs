using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace powerFlexBackup.cipdevice
{
    public abstract class IdentityObject
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
        public UInt32 SerialNumber;
        public String ProductName = "";

        [JsonIgnore]
        public UInt16 Status;

        public abstract IdentityObject getIdentityObjectfromResponse(byte[] IdentityObjectBytes);
    }
}
