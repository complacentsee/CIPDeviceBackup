using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using powerFlexBackup.cipdevice.deviceParameterObjects;

namespace powerFlexBackup.cipdevice
{
    [SupportedDevice("PowerFlex 750 Series", 123, 1168)] 
    [SupportedDevice("PowerFlex 750 Series", 123, 2192)] 
    [SupportedDevice("PowerFlex 750 Series", 142, 1168)] 
    [SupportedDevice("PowerFlex 750 Series", 142, 2192)] 
    [SupportedDevice("PowerFlex 750 Series", 143, 2192)] 
    public class CIPDevice_PowerFlex750 : CIPDevice{

        List<DeviceParameter_PowerFlex750> parameterObjectList;
        List<PortParameterMap> portMap;
        public CIPDevice_PowerFlex750(String deviceAddress, Sres.Net.EEIP.EEIPClient eeipClient) :
            base(deviceAddress, eeipClient)
        {
            setInstanceAttributes();
            parameterObjectList = new List<DeviceParameter_PowerFlex750>();
            portMap = new List<PortParameterMap>();
            portMap.Add(new PortParameterMap(0, 0x0000));
            portMap.Add(new PortParameterMap(1, 0x4400));
            portMap.Add(new PortParameterMap(2, 0x4800));
            portMap.Add(new PortParameterMap(3, 0x4C00));
            portMap.Add(new PortParameterMap(4, 0x5000));
            portMap.Add(new PortParameterMap(5, 0x5400));
            portMap.Add(new PortParameterMap(6, 0x5800));
            portMap.Add(new PortParameterMap(7, 0x5C00));
            portMap.Add(new PortParameterMap(8, 0x6000));
            portMap.Add(new PortParameterMap(9, 0x6400));
            portMap.Add(new PortParameterMap(10, 0x6800));
            portMap.Add(new PortParameterMap(11, 0x6C00));
            portMap.Add(new PortParameterMap(12, 0x7000));
            portMap.Add(new PortParameterMap(13, 0x7400));
            portMap.Add(new PortParameterMap(14, 0x7800));

        }

        //NOTE: The powerflex 750 series does not support Parameter Objects
        // instead we must use DPI Online Read Full and Process the returned object. 
        public override void setInstanceAttributes(int instance = 0){
            parameterObject[instance].instanceAttributes.Clear();
            parameterObject[instance].instanceAttributes.Add(new InstanceAttribute(6, "DPI Offline Read Full"));
            parameterObject[instance].instanceAttributes.Add(new InstanceAttribute(7, "DPI Online Read Full"));
        }

        public override void getDeviceParameterValues(){
            //TODO: Implement
            getAllDeviceParameters();
        }

        public void getAllDevicePortIdentities(int ClassID = 0x0F, int instance = 0){
            //PowerFlex 750 does not support max count of components. 
            for (int i = 2; i < 16; i++)
            {
                try {
                    var IdentityObject = new IdentityObject();
                    var temp = getRawIdentiyObjectfromInstance(i); 
                    try {
                        IdentityObject = IdentityObject.getIdentityObjectfromResponse(temp);
                    } catch (Exception e) {
                        Console.WriteLine("Error getting Identity Object from instance {0}: {1}", i, e.Message);
                    }
                    bool deviceIsHIM = IdentityObject.ProductName.Contains("HIM") || IdentityObject.ProductCode == 767; 
                    bool portIsEmpty = IdentityObject.ProductName.Contains("Not") || IdentityObject.ProductCode == 65280;

                    if (deviceIsHIM || portIsEmpty){
                        continue;
                    }
                        parameterObject.Add(new DeviceParameterObject(ClassID, IdentityObject, new List<DeviceParameter>(), i-1));
                } catch (Exception e) {
                    Console.WriteLine("Error getting Identity Object from instance {0}: {1}", i, e.Message);
                }
            }
        }

        public override void getAllDeviceParameters(){
            getAllDevicePortIdentities();
            var length = parameterObject.Count;
            Console.WriteLine("Number of Device Parameter Objects: {0}", length);
    
            var index = 0;
            foreach(DeviceParameterObject PortGroup in parameterObject){
                var ClassID = 0x9F; 
                //FIXME: This is a hack to to handle the Safe Torque Off Modules not returning paramters.
                if(PortGroup.identityObject.ProductName.Contains("Safe Torque Off")){
                    continue;
                }
                //NOTE: The 20-COMM-E does not support the 0x9F sine it is a DPI device.
                //FIXME: We should find a more elegant way to handle this. Maybe we
                // should detect the "Embedded service error" and then try the 0x93 class 
                // instead of manually addressing the special case.
                if(PortGroup.identityObject.ProductName.Contains("20-COMM-E")){
                    ClassID = 0x93;
                }

                Console.WriteLine("Getting Parameters for {0}", PortGroup.identityObject.ProductName);
                getAllPortParameters(portMap[PortGroup.Port].Offset, index, ClassID);
                index++;
            }
        }

        private void getAllPortParameters(int offset, int instance = 0, int ClassID = 0x9F){
            var parametersRemaning = true;
            int parameterNumber = 1;
            while(parametersRemaning){
                var nextParameter = ByteArrayToDeviceParameter(readPF750DPIOnlineReadFull(parameterNumber + offset, ClassID));
                if (nextParameter.NextParameter < parameterNumber){
                    parametersRemaning = false;
                }
                var param = new deviceParameterObjects.DeviceParameter();

                param.number = parameterNumber;
                param.name = nextParameter.ParameterName;
                param.value = nextParameter.ParameterValue.value;
                param.valueHex = Convert.ToHexString(nextParameter.ParameterValue.toBytes());
                param.defaultValue = nextParameter.DefaultValue.value;
                param.type = nextParameter.Descriptor.dataType;
                param.typeHex = Convert.ToHexString(nextParameter.Descriptor.dataType);
                param.isWritable = nextParameter.Descriptor.Writable;
                param.record = nextParameter.Descriptor.Writable;

                parameterObject[instance].ParameterList.Add(param);
                parameterNumber = nextParameter.NextParameter;

                if(Globals.outputVerbose){
                    Console.WriteLine(JsonConvert.SerializeObject(nextParameter));
                    Console.WriteLine();    
                }
            }
        }


        // Powerflex 750 does not support this command
        public override int readDeviceParameterMaxNumber(){
            return 0;
        }

        //Defaults to 0x9F (HOST) memory but can be changed to 0x9E (DEVICE) memory if needed.
        public byte[] readPF750DPIOnlineReadFull(int instanceNumber, int ClassID = 0x9F){
            byte[] responseBytes = GetAttributeSingle(ClassID, instanceNumber, 7);
            return responseBytes;
        }



        private DeviceParameter_PowerFlex750 ByteArrayToDeviceParameter(byte[] byteArray){
            var deviceParameter = new DeviceParameter_PowerFlex750();
            deviceParameter.Descriptor = new Descriptor(byteArray.Take(4).ToArray());
            deviceParameter.ParameterValue = new Parameter_ContainerPowerFlex750(byteArray.Skip(4).Take(4).ToArray());
            deviceParameter.ParameterValue.value = this.getParameterValuefromBytes(deviceParameter.ParameterValue.toBytes(), deviceParameter.dataType);
            deviceParameter.MinimumValue = new Parameter_ContainerPowerFlex750(byteArray.Skip(8).Take(4).ToArray());
            deviceParameter.MinimumValue.value = this.getParameterValuefromBytes(deviceParameter.MinimumValue.toBytes(), deviceParameter.dataType);
            deviceParameter.MaxumumValue = new Parameter_ContainerPowerFlex750(byteArray.Skip(12).Take(4).ToArray());
            deviceParameter.MaxumumValue.value = this.getParameterValuefromBytes(deviceParameter.MaxumumValue.toBytes(), deviceParameter.dataType);
            deviceParameter.DefaultValue = new Parameter_ContainerPowerFlex750(byteArray.Skip(16).Take(4).ToArray());
            deviceParameter.DefaultValue.value = this.getParameterValuefromBytes(deviceParameter.DefaultValue.toBytes(), deviceParameter.dataType);
            deviceParameter.NextParameter = CIPDeviceHelper.convertBytestoUINT16LittleEndianV(byteArray.Skip(20).Take(2).ToArray());
            deviceParameter.PreviousParameter = CIPDeviceHelper.convertBytestoUINT16LittleEndianV(byteArray.Skip(22).Take(2).ToArray());
            deviceParameter.UnitsString = System.Text.Encoding.ASCII.GetString(byteArray.Skip(24).Take(4).ToArray()).Trim();
            deviceParameter.Multiplier = CIPDeviceHelper.convertBytestoUINT16LittleEndianV(byteArray.Skip(28).Take(2).ToArray());
            deviceParameter.Divisor = CIPDeviceHelper.convertBytestoUINT16LittleEndianV(byteArray.Skip(30).Take(2).ToArray());
            deviceParameter.Base = CIPDeviceHelper.convertBytestoUINT16LittleEndianV(byteArray.Skip(32).Take(2).ToArray());
            deviceParameter.Offset = CIPDeviceHelper.convertBytesToINT16LittleEndianV(byteArray.Skip(34).Take(2).ToArray());
            deviceParameter.Link = byteArray.Skip(36).Take(3).ToArray();
            deviceParameter.AlwaysZero = byteArray.Skip(39).Take(1).ToArray()[0];
            deviceParameter.ParameterName = System.Text.Encoding.ASCII.GetString(byteArray.Skip(40).Take(32).ToArray()).Trim();
            return deviceParameter;
        }

        public class DeviceParameter_PowerFlex750{
            public Descriptor Descriptor {get; set;} = new Descriptor();
            public Parameter_ContainerPowerFlex750 ParameterValue {get; set;} = new Parameter_ContainerPowerFlex750();
            public Parameter_ContainerPowerFlex750 MinimumValue {get; set;} = new Parameter_ContainerPowerFlex750();
            public Parameter_ContainerPowerFlex750 MaxumumValue {get; set;} = new Parameter_ContainerPowerFlex750();
            public Parameter_ContainerPowerFlex750 DefaultValue {get; set;} = new Parameter_ContainerPowerFlex750();
            public UInt16 NextParameter {get; set;} = 0;
            public UInt16 PreviousParameter {get; set;} = 0;
            public String UnitsString {get; set;} = "";
            public UInt16 Multiplier {get; set;} = 0;
            public UInt16 Divisor {get; set;} = 0;
            public UInt16 Base {get; set;} = 0;
            public Int16 Offset {get; set;} = 0;
            public byte[] Link {get; set;} = new byte[3];
            public byte AlwaysZero {get; set;} = new byte();
            public String ParameterName {get; set;} = "";

            public byte[] dataType {        
                get
                {
                return this.Descriptor.dataType;
                }        
            }
            public bool Writable {        
                get
                {
                return this.Descriptor.Writable;
                }        
            }

        }

        public class Parameter_ContainerPowerFlex750{
            public byte[] container {get; set;}
            public string value {get; set;}

            public Parameter_ContainerPowerFlex750(){
                container = new byte[4];
                value = "";
            }
            public Parameter_ContainerPowerFlex750(byte[] byteArray){
                this.container = byteArray;
                value = "";
            }

            public byte[] toBytes(){
                return container;
            }
        }

    public class Descriptor{
        public bool[] descriptor {get; set;} = new bool[32];

        public byte[] dataType 
        {
            get
                {
                    return this.getDataType();
                }
        }
        public bool Writable 
        {
            get
                {
                    return this.getIsWritable();
                }
        }

        public Descriptor(){
            descriptor = new bool[32];
        }
        public Descriptor(byte[] byteArray){
            this.descriptor = CIPDeviceHelper.unpackageBytesToBoolArray(byteArray);
        }

        // MAPPING PER POWERFLEX 750 MANUAL
        // Descriptor to decode the type of the parameter
        // Right bit is least significant bit (0).
        // 000 = USINT used as an array of Boolean
        // 001 = UINT used as an array of Boolean
        // 010 = USINT (8 bit integer)
        // 011 = UINT (16 bit integer)
        // 100 = UDINT (32 bit integer)
        // 101 = TCHAR ((8 bit (not Unicode) or 16 bits (Unicode))
        // 110 = REAL (32 bit floating point value)
        // 111 = Use bits 16, 17, 18
        //
        // Bit 16 is the least significant bit.
        // 000 = Reserved
        // 001 = UDINT used as an array of Boolean.
        // 010 = Reserved
        // 011 = Reserved
        // 100 = Reserved
        // 101 = Reserved
        // 110 = Reserved
        // 111 = Reserved
        //
        private byte[] getDataType(){
            var dataType = new byte[1];
            if(descriptor[2] == false && descriptor[1] == false && descriptor[0] == false){
                dataType[0] = 0xD1;
            }
            else if(descriptor[2] == false && descriptor[1] == false && descriptor[0] == true){
                dataType[0] = 0xD2;
            }
            else if(descriptor[2] == false && descriptor[1] == true && descriptor[0] == false){
                dataType[0] = 0xC6;
            }
            else if(descriptor[2] == false && descriptor[1] == true && descriptor[0] == true){
                dataType[0] = 0xC7;
            }
            else if(descriptor[2] == true && descriptor[1] == false && descriptor[0] == false){
                dataType[0] = 0xC8;
            }
            else if(descriptor[2] == true && descriptor[1] == false && descriptor[0] == true){
                Globals.logger.LogError("TCHAR not implemented");
                dataType[0] = 0xC2;
            }
            else if(descriptor[2] == true && descriptor[1] == true && descriptor[0] == false){
                dataType[0] = 0xCA;
            }
            else if(descriptor[2] == true && descriptor[1] == true && descriptor[0] == true){
                if(descriptor[2] == true)
                    dataType[0] = 0xD3;
            }
            return dataType;
        }

        private bool getIsWritable(){
            return descriptor[8];
        }
    } 

    public class PortParameterMap{
        public int Port {get; set;}
        public int Offset {get; set;}

        public PortParameterMap(int port, int offset){
            Port = port;
            Offset = offset;
        }
    }    
    }
}