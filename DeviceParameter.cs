using Newtonsoft.Json;
namespace powerFlexBackup.cipdevice.deviceParameterObjects
{
    public class DeviceParameter
    {
        public int? port { get; set; } = null;
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
        public DeviceParameter(int number, string name, string value, string defaultValue, bool record, byte[] type)
        {
            this.number = number;
            this.name = name;
            this.value = value;
            this.defaultValue = defaultValue;
            this.record = record;
            this.type = type;
        }
        
        public DeviceParameter(int number, string name, string value, string defaultValue, bool record, byte[] type, byte[] size)
        {
            this.number = number;
            this.name = name;
            this.value = value;
            this.defaultValue = defaultValue;
            this.record = record;
            this.type = type;
            this.size = size;
        }
        
        public bool ShouldSerializenumber() {return record | Globals.outputAllRecords;}
        public bool ShouldSerializename() {return record | Globals.outputAllRecords;}
        public bool ShouldSerializevalue() {return record | Globals.outputAllRecords;}
        public bool ShouldSerializedefaultValue() {return record | Globals.outputAllRecords;}
        public bool ShouldSerializerecord() {return false | Globals.outputAllRecords;}
        public bool ShouldSerializetype() {return false | Globals.outputAllRecords;}
        public bool ShouldSerializesize() {return false | Globals.outputAllRecords;}
        public bool ShouldSerializevalueHex() {return false | Globals.outputAllRecords;}
        public bool ShouldSerializetypeHex() {return false | Globals.outputAllRecords;}
        public bool ShouldSerializeisWritable() {return false | Globals.outputAllRecords;}
        public bool ShouldSerializeisDescriptor() {return false | Globals.outputAllRecords;}
        public bool ShouldSerializedisplayValue() {return false | Globals.outputAllRecords;}

        public bool ShouldSerializeport() {return !(port is null) | Globals.outputAllRecords;}
    }
}
