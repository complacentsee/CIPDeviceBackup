using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace powerFlexBackup.cipdevice.deviceParameterObjects
{
    public class DeviceParameterObject
    {
        [JsonIgnore]
        public int ClassID { get; set; } = 0x0F;
        public int Port { get; set; } = 0;
        public IdentityObject identityObject = new IdentityObject();
        [JsonConverter(typeof(CompactListJsonConverter))]
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

        public DeviceParameterObject(int ClassID, IdentityObject identityObject, List<DeviceParameter> ParameterList, int port)
        {
            this.ClassID = ClassID;
            this.identityObject = identityObject;
            this.ParameterList = ParameterList;
            this.Port = port;
            this.instanceAttributes = new List<InstanceAttribute>();
            this.instanceAttributes.Add(new InstanceAttribute(1, "Parameter Value"));
            this.instanceAttributes.Add(new InstanceAttribute(2, "Link Path Size"));
            this.instanceAttributes.Add(new InstanceAttribute(3, "Link Path"));
            this.instanceAttributes.Add(new InstanceAttribute(4, "Descriptor"));
            this.instanceAttributes.Add(new InstanceAttribute(5, "Data Type"));
            this.instanceAttributes.Add(new InstanceAttribute(6, "Data Size"));
        }

        //public bool ShouldSerializePort() {return Port > 1;}
    }

    /// <summary>
    /// Serializes each list element as a compact single-line JSON object
    /// while the parent object remains indented.
    /// </summary>
    public class CompactListJsonConverter : JsonConverter<List<DeviceParameter>>
    {
        public override void WriteJson(JsonWriter writer, List<DeviceParameter>? value, JsonSerializer serializer)
        {
            writer.WriteStartArray();
            if (value != null)
            {
                foreach (var item in value)
                {
                    var token = JToken.FromObject(item, serializer);
                    writer.WriteRawValue(token.ToString(Formatting.None));
                }
            }
            writer.WriteEndArray();
        }

        public override List<DeviceParameter>? ReadJson(JsonReader reader, Type objectType, List<DeviceParameter>? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var array = JArray.Load(reader);
            return array.ToObject<List<DeviceParameter>>();
        }
    }
}