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
        private String driveAddress;

        public CIPDevice(String driveAddress, IdentityObject identityObject, Sres.Net.EEIP.EEIPClient eeipClient)
        {
            this.driveAddress = driveAddress;
            this.identityObject = identityObject;
            this.eeipClient = eeipClient;
            this.eeipClient.RegisterSession(driveAddress);
            var rawIdentityObject = getRawIdentiyObject();
            this.identityObject = identityObject.getIdentityObjectfromResponse(rawIdentityObject);
            this.parameterObject = new DeviceParameterObject();
        }
        
        private static void registerDeviceAddress(String driveAddress, Sres.Net.EEIP.EEIPClient eeipClient)
        {
            eeipClient.RegisterSession(driveAddress);
        }

        public void setDriveIdentiyObject(IdentityObject identityObject)
        {
            this.identityObject = identityObject;
        }

        public void setEEIPClient (Sres.Net.EEIP.EEIPClient Client){
            eeipClient = Client;
        }

        public void setDriveAddress (string driveAddress){
            this.driveAddress = driveAddress;
        }

        public void setDriveParameterList (List<DeviceParameter> DriveParameterList){
            this.parameterObject.ParameterList = DriveParameterList;
        }

        public void setInstanceAttribute(List<InstanceAttribute> InstanceAttribute){
            this.parameterObject.instanceAttributes = InstanceAttribute;
        }

        public void setDriveParameterClassID(int ClassID){
            this.parameterObject.ClassID = ClassID;
        }

        public int getDriveParameterClassID(){
            return this.parameterObject.ClassID;
        }

        public void initializeDriveParameterObject (){
            this.parameterObject = new DeviceParameterObject();
        }

        public void removeNonRecordedDriveParameters (){
            this.parameterObject.ParameterList.RemoveAll(x => x.recordValue == false);
        }
        public void removeDefaultDriveParameters (){
            this.parameterObject.ParameterList.RemoveAll(x => x.defaultParameterValue.Equals(x.parameterValue));
        }

        public List<DeviceParameter> getDriveDriveParameterList (){
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

        public void getDriveParameterValues()
        {
            if(parameterObject.ParameterList != null){
                foreach(DeviceParameter Parameter in parameterObject.ParameterList)
                    {
                        if(Parameter.recordValue){
                            if(Parameter.parameterType is null)
                                Parameter.parameterType = readDriveParameterType(Parameter.parameterNumber);

                            byte[] parameterValue = readDriveParameterValue(Parameter.parameterNumber);
                            Parameter.parameterValue = getParameterValuefromBytes(parameterValue,Parameter.parameterType);

                            // byte[] defaultParameterValue = readDriveParameterDefaultValue(Parameter.parameterNumber);
                            // Parameter.defaultParameterValue = getParameterValuefromBytes(defaultParameterValue,Parameter.parameterType);

                            Globals.logger.LogDebug(JsonConvert.SerializeObject(Parameter,Formatting.Indented));
                        }
                    }
            }
            return;
        }

        private byte[] getRawIdentiyObject()
        {
            return this.eeipClient.GetAttributeAll(0x01, 1);
        }


        private byte[] readDriveParameterValue(int parameterNumber)
        {
            return this.eeipClient.GetAttributeSingle(getDriveParameterClassID(),parameterNumber, getAttributeIDfromString("Parameter Value"));
        }

        private byte[] readDriveParameterDefaultValue(int parameterNumber)
        {
            return this.eeipClient.GetAttributeSingle(getDriveParameterClassID(),parameterNumber, getAttributeIDfromString("Default Value"));
        }

        private byte[] readDriveParameterType(int parameterNumber)
        {
            return this.eeipClient.GetAttributeSingle(getDriveParameterClassID(),parameterNumber, getAttributeIDfromString("Data Type"));;
        }
        //Each drive class should have a method to convert the returned bytes to the correct data type.
        //FIXME: Should this be a callback function?
        public abstract string getParameterValuefromBytes(byte[] parameterValueBytes, byte[] parameterType);
    }
}
