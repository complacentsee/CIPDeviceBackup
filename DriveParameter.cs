using Newtonsoft.Json;
namespace powerFlexBackup.powerFlexDrive.DriveParameterObjects
{
    public class DriveParameter
    {
        public int parameterNumber { get; set; }
        public string parameterName { get; set; }
        public string parameterValue { get; set; }
        public string defaultParameterValue { get; set; }
        public bool recordValue { get; set; }
        [JsonIgnore]
        public byte[]? parameterType { get; set; }

        public DriveParameter()
        {
        }
        
        public bool ShouldSerializeparameterNumber() {return recordValue;}
        public bool ShouldSerializeparameterName() {return recordValue;}
        public bool ShouldSerializeparameterValue() {return recordValue;}
        public bool ShouldSerializerecordValue() {return false;}
    }
}
