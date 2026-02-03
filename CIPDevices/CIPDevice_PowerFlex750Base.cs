using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using powerFlexBackup.cipdevice.deviceParameterObjects;

namespace powerFlexBackup.cipdevice
{
    public abstract class CIPDevice_PowerFlex750Base : CIPDevice
    {
        protected List<DeviceParameter_PowerFlex750> parameterObjectList;
        protected List<PortParameterMap> portMap;

        public CIPDevice_PowerFlex750Base(String deviceAddress, Sres.Net.EEIP.EEIPClient eeipClient, byte[] CIPRoute, IOptions<AppConfiguration> options, ILogger logger) :
            base(deviceAddress, eeipClient, CIPRoute, options, logger)
        {
            setInstanceAttributes();
            parameterObjectList = new List<DeviceParameter_PowerFlex750>();
            portMap = new List<PortParameterMap>();
            InitializePortMap();
        }

        // Template method - subclasses implement specific port map offsets
        protected abstract void InitializePortMap();

        //NOTE: The powerflex 750 series does not support Parameter Objects
        // instead we must use DPI Online Read Full and Process the returned object.
        public override void setInstanceAttributes(int instance = 0){
            parameterObject[instance].instanceAttributes.Clear();
            parameterObject[instance].instanceAttributes.Add(new InstanceAttribute(6, "DPI Offline Read Full"));
            parameterObject[instance].instanceAttributes.Add(new InstanceAttribute(7, "DPI Online Read Full"));
        }

        public override void getDeviceParameterValues(){
            //TODO: Implement actual function isntead of handing off to all device parameters.
            // Ipement a method to load the parameter object list with the correct parameters for a given device.
            // this can be a reduced list of only record records for each parameter type.
            // Known device types so far, 27 module types are available.
            // PowerFlex 753
            // PowerFlex 755
            // 20-COMM-E
            // DeviceLogix
            // I/O Module 24V (multiple types are available)
            // I/O Module 125V (multiple types are available)
            // EtherNet/IP
            getAllDeviceParameters();
        }

        public void getAllDevicePortIdentities(int ClassID = 0x0F, int instance = 0){
            //PowerFlex 750 does not support max count of components.
            //Instead check each port for a valid device. Port will report it if it empty.
            for (int i = 2; i < 16; i++)
            {
                try {
                    var IdentityObject = new IdentityObject();
                    var temp = getRawIdentiyObjectfromInstance(i);
                    try {
                        IdentityObject = IdentityObject.getIdentityObjectfromResponse(temp, logger);
                    } catch (Exception e) {
                        Console.WriteLine("Error getting Identity Object from instance {0}: {1}", i, e.Message);
                    }

                    // SKIP empty port data collection and HIM modules. Maybe HIM modules should be collected?
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
                // Need to test if they are parameter objects or not.
                if(PortGroup.identityObject.ProductName.Contains("Safe Torque Off")){
                    continue;
                }
                //NOTE: The 20-COMM-E does not support the 0x9F sine it is a DPI device.
                //TODO: Should find a more elegant way to handle this. Maybe we
                // should catch the "Embedded service error" and then try the 0x93 class
                // instead of manually addressing the special case.
                if(PortGroup.identityObject.ProductName.Contains("20-COMM-E")){
                    ClassID = 0x93;
                }

                Console.WriteLine("Getting Parameters for {0}", PortGroup.identityObject.ProductName);
                getAllPortParameters(portMap[PortGroup.Port].Offset, index, ClassID);
                index++;
            }
        }

        protected virtual void getAllPortParameters(int offset, int instance = 0, int ClassID = 0x9F){
            var parametersRemaning = true;
            int parameterNumber = 1;
            while(parametersRemaning){
                var nextParameter = DPIOnlineReadFull.ByteArrayToDeviceParameter(
                    readPF750DPIOnlineReadFull(parameterNumber + offset, ClassID),
                    logger,
                    this.getParameterValuefromBytes);
                if (nextParameter.NextParameter < parameterNumber){
                    parametersRemaning = false;
                }
                var param = new DeviceParameter
                {
                    number = parameterNumber,
                    name = nextParameter.ParameterName,
                    value = nextParameter.ParameterValue.value,
                    valueHex = Convert.ToHexString(nextParameter.ParameterValue.toBytes()),
                    defaultValue = nextParameter.DefaultValue.value,
                    type = nextParameter.Descriptor.dataType,
                    typeHex = Convert.ToHexString(nextParameter.Descriptor.dataType),
                    isWritable = nextParameter.Descriptor.Writable,
                    record = nextParameter.Descriptor.Writable
                };

                parameterObject[instance].ParameterList.Add(param);
                parameterNumber = nextParameter.NextParameter;

                if(config.OutputVerbose){
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

        public byte[] readPF750DPIOfflineReadFull(int instanceNumber, int ClassID = 0x9F){
            byte[] responseBytes = GetAttributeSingle(ClassID, instanceNumber, 6);
            return responseBytes;
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
