namespace powerFlexBackup.cipdevice.deviceParameterObjects
{
    public class DeviceParameterObject
    {
        public int ClassID { get; set; } = default!;
        public List<DeviceParameter> ParameterList { get; set; } = default!;
        public List<InstanceAttribute> instanceAttributes{ get; set; } = default!;
        public DeviceParameterObject()
        {
        }
    }
}