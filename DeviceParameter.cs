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
        public byte[] type { get; set; }
        public byte[]? size { get; set; }
        public string? valueHex { get; set; }
        public string? typeHex { get; set; }

        public DeviceParameter()
        {
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
        
        // public bool ShouldSerializenumber() {return record | Globals.outputAllRecords;}
        // public bool ShouldSerializename() {return record | Globals.outputAllRecords;}
        // public bool ShouldSerializevalue() {return record | Globals.outputAllRecords;}
        // public bool ShouldSerializedefaultValue() {return record | Globals.outputAllRecords;}
        // public bool ShouldSerializerecord() {return false | Globals.outputAllRecords;}
        // public bool ShouldSerializetype() {return false | Globals.outputAllRecords;}
        // public bool ShouldSerializesize() {return false | Globals.outputAllRecords;}
    }
}
