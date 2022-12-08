namespace powerFlexBackup.cipdevice.deviceParameterObjects
{
    public class InstanceAttribute
    {
        public int AttributeID { get; set; } = default!;
        public string Name { get; set; } = default!;

        public InstanceAttribute()
        {
        }

        public InstanceAttribute(int attributeID, string name)
        {
            AttributeID = attributeID;
            Name = name;
        }
    }

}