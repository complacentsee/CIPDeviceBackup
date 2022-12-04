using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using powerFlexBackup.cipdevice.deviceParameterObjects;

namespace powerFlexBackup.cipdevice
{

    public abstract class CIPDevice
    {
        private IdentityObject identityObject;
        private DeviceParameterObject parameterObject;
        private Sres.Net.EEIP.EEIPClient eeipClient;
        private String deviceAddress;

        public CIPDevice(String deviceAddress, IdentityObject identityObject, Sres.Net.EEIP.EEIPClient eeipClient)
        {
            this.deviceAddress = deviceAddress;
            this.identityObject = identityObject;
            this.eeipClient = eeipClient;
            this.eeipClient.RegisterSession(deviceAddress);
            var rawIdentityObject = getRawIdentiyObject();
            this.identityObject = identityObject.getIdentityObjectfromResponse(rawIdentityObject);
            this.parameterObject = new DeviceParameterObject();
        }
        
        private static void registerDeviceAddress(String deviceAddress, Sres.Net.EEIP.EEIPClient eeipClient)
        {
            eeipClient.RegisterSession(deviceAddress);
        }

        public void setDeviceIdentiyObject(IdentityObject identityObject)
        {
            this.identityObject = identityObject;
        }

        public void setEEIPClient (Sres.Net.EEIP.EEIPClient Client){
            eeipClient = Client;
        }

        public void setDeviceAddress (string deviceAddress){
            this.deviceAddress = deviceAddress;
        }

        public void setDeviceParameterList (List<DeviceParameter> DeviceParameterList){
            this.parameterObject.ParameterList = DeviceParameterList;
        }

        public void setInstanceAttribute(List<InstanceAttribute> InstanceAttribute){
            this.parameterObject.instanceAttributes = InstanceAttribute;
        }

        public void setDeviceParameterClassID(int ClassID){
            this.parameterObject.ClassID = ClassID;
        }

        public int getDeviceParameterClassID(){
            return this.parameterObject.ClassID;
        }

        public void initializeDeviceParameterObject (){
            this.parameterObject = new DeviceParameterObject();
        }

        public void removeNonRecordedDeviceParameters (){
            this.parameterObject.ParameterList.RemoveAll(x => x.record == false);
        }
        public void removeDefaultDeviceParameters (){
            this.parameterObject.ParameterList.RemoveAll(x => x.defaultValue.Equals(x.value));
        }

        public List<DeviceParameter> getDeviceParameterList (){
            return this.parameterObject.ParameterList;
        }

        public IdentityObject getIdentityObject()
        {
            return this.identityObject;
        }

        public int getAttributeIDfromString(String attributeName)
        {
            return parameterObject.instanceAttributes.Find(x => x.Name.Equals(attributeName)).AttributeID;
        }

        public void getDeviceParameterValues()
        {
            if(parameterObject.ParameterList != null){
                foreach(DeviceParameter Parameter in parameterObject.ParameterList)
                    {
                        if(Parameter.record | Globals.outputAllRecords){

                            if(Parameter.type is null)
                                Parameter.type = readDeviceParameterType(Parameter.number);

                            byte[] parameterValue = readDeviceParameterValue(Parameter.number);
                            Parameter.value = getParameterValuefromBytes(parameterValue,Parameter.type);
                            byte[] defaultParameterValue = readDeviceParameterDefaultValue(Parameter.number);
                            Parameter.defaultValue = getParameterValuefromBytes(defaultParameterValue,Parameter.type);

                            Parameter.valueHex = Convert.ToHexString(parameterValue);
                            Parameter.typeHex = Convert.ToHexString(Parameter.type);

                                            Console.WriteLine("Parameter #{0}, Value: {1}, ValueByte: {2}, Type: {3}, Size: {4}", Parameter.number, 
                                            Parameter.value, 
                                            Parameter.valueHex, 
                                            Parameter.typeHex,
                                            "");

                            Globals.logger.LogDebug(JsonConvert.SerializeObject(Parameter,Formatting.Indented));
                        }
                    }
            }
            return;
        }

        public void getAllDeviceParameters()
        {
            var maxParameterNumber = readDeviceParameterMaxNumber();
            for(int i = 1; i <= maxParameterNumber; i++)
            {
                var parameterNumber = i;
                var parameterName = readDeviceParameterName(parameterNumber);
                var parameterType = readDeviceParameterType(parameterNumber);
                var parameterValue = readDeviceParameterValue(parameterNumber);
                var parameterSize = readDeviceParameterSize(parameterNumber);
                var defaultParameterValue = readDeviceParameterDefaultValue(parameterNumber);
                var parameterValueString = getParameterValuefromBytes(parameterValue,parameterType);
                Console.WriteLine("Parameter #{0}, Value: {1}, ValueByte: {2}, Type: {3}, Size: {4}", parameterNumber, 
                                            parameterValueString, 
                                            Convert.ToHexString(parameterValue), 
                                            Convert.ToHexString(parameterType),
                                            Convert.ToHexString(parameterSize));
                var defaultParameterValueString = getParameterValuefromBytes(defaultParameterValue,parameterType);

                var parameter = new DeviceParameter(parameterNumber,parameterName,parameterValueString,Convert.ToHexString(parameterType),true,parameterType, parameterSize);
                parameter.valueHex = Convert.ToHexString(parameterValue);
                parameter.typeHex = Convert.ToHexString(parameterType);
                parameterObject.ParameterList.Add(parameter);
                //Globals.logger.LogInformation(JsonConvert.SerializeObject(parameter,Formatting.Indented));
            }
            return;
        }

        private int readDeviceParameterMaxNumber(){
            byte[] maxParameterNumberBytes = eeipClient.GetAttributeSingle(parameterObject.ClassID, 0, 2);
            int maxParameterNumber = Convert.ToUInt16(maxParameterNumberBytes[0]
                                                        | maxParameterNumberBytes[1] << 8);
            return maxParameterNumber;
        }

        private String readDeviceParameterName(int parameterNumber){
            byte[] parameterNameBytes = eeipClient.GetAttributeSingle(parameterObject.ClassID, parameterNumber, getAttributeIDfromString("Parameter Name String"));
            String parameterName = System.Text.Encoding.ASCII.GetString(parameterNameBytes, 1, Convert.ToInt32(parameterNameBytes[0])).TrimEnd();
            return parameterName;
        }

        private byte[] getRawIdentiyObject()
        {
            return this.eeipClient.GetAttributeAll(0x01, 1);
        }


        private byte[] readDeviceParameterValue(int parameterNumber)
        {
            return this.eeipClient.GetAttributeSingle(getDeviceParameterClassID(),parameterNumber, getAttributeIDfromString("Parameter Value"));
        }

        private byte[] readDeviceParameterDefaultValue(int parameterNumber)
        {
            return this.eeipClient.GetAttributeSingle(getDeviceParameterClassID(),parameterNumber, getAttributeIDfromString("Default Value"));
        }

        private byte[] readDeviceParameterType(int parameterNumber)
        {
            return this.eeipClient.GetAttributeSingle(getDeviceParameterClassID(),parameterNumber, getAttributeIDfromString("Data Type"));
        }

        private byte[] readDeviceParameterSize(int parameterNumber)
        {
            return this.eeipClient.GetAttributeSingle(getDeviceParameterClassID(),parameterNumber, getAttributeIDfromString("Data Size"));
        }

        //Each Device class should have a method to convert the returned bytes to the correct data type.
        //FIXME: Should this be a callback function?
        public abstract string getParameterValuefromBytes(byte[] parameterValueBytes, byte[] type);

    }
}
