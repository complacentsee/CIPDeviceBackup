using Newtonsoft.Json;
namespace powerFlexBackup.cipdevice.deviceParameterObjects
{
    public class DeviceParameter
    {
        public int number { get; set; }
        public string name { get; set; }
        public string value { get; set; }
        public string defaultValue { get; set; }
        public bool record { get; set; }
        public byte[]? type { get; set; } = null;
        public byte[]? size { get; set; } = null;
        public string? valueHex { get; set; } = null;
        public string? typeHex { get; set; } = null;
        public bool? isWritable { get; set; } = null;
        public string? Descriptor { get; set; } = null;
        public string? displayValue { get; set; } = null;

        public DeviceParameter()
        {
            this.number = 0;
            this.name = "";
            this.value = "";
            this.defaultValue = "";
            this.record = false;
        }
        
        public bool ShouldSerializenumber() {return record || AppConfiguration.OutputAllRecordsStatic;}
        public bool ShouldSerializename() {return record || AppConfiguration.OutputAllRecordsStatic;}
        public bool ShouldSerializevalue() {return record || AppConfiguration.OutputAllRecordsStatic;}
        public bool ShouldSerializedefaultValue() {return record || AppConfiguration.OutputAllRecordsStatic;}
        public bool ShouldSerializerecord() {return false || AppConfiguration.OutputAllRecordsStatic;}
        public bool ShouldSerializetype() {return false || AppConfiguration.OutputAllRecordsStatic;}
        public bool ShouldSerializesize() {return false || AppConfiguration.OutputAllRecordsStatic;}
        public bool ShouldSerializevalueHex() {return false || AppConfiguration.OutputAllRecordsStatic;}
        public bool ShouldSerializetypeHex() {return false || AppConfiguration.OutputAllRecordsStatic;}
        public bool ShouldSerializeisWritable() {return false || AppConfiguration.OutputAllRecordsStatic;}
        public bool ShouldSerializeisDescriptor() {return false || AppConfiguration.OutputAllRecordsStatic;}
        public bool ShouldSerializedisplayValue() {return false || AppConfiguration.OutputAllRecordsStatic;}

    }
}
