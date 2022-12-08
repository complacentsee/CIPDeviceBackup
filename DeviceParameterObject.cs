using Newtonsoft.Json;
namespace powerFlexBackup.cipdevice.deviceParameterObjects
{
    public class DeviceParameterObject
    {
        public int ClassID { get; set; } = 0x0F;
        public IdentityObject identityObject = new IdentityObject();
        public List<DeviceParameter> ParameterList { get; set; } = default!;
        
        [JsonIgnore]
        public List<InstanceAttribute> instanceAttributes{ get; set; } = default!;

        public DeviceParameterObject(int ClassID, IdentityObject identityObject, List<DeviceParameter> ParameterList)
        {
            this.ClassID = ClassID;
            this.identityObject = identityObject;
            this.ParameterList = ParameterList;
            instanceAttributes = new List<InstanceAttribute>();
            instanceAttributes.Add(new InstanceAttribute(1, "Parameter Value"));
            instanceAttributes.Add(new InstanceAttribute(2, "Link Path Size"));
            instanceAttributes.Add(new InstanceAttribute(3, "Link Path"));
            instanceAttributes.Add(new InstanceAttribute(4, "Descriptor"));
            instanceAttributes.Add(new InstanceAttribute(5, "Data Type"));
            instanceAttributes.Add(new InstanceAttribute(6, "Data Size"));
        }
    }
}