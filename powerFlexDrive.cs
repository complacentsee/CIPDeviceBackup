using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using powerFlexBackup.powerFlexDrive.DriveParameterObjects;

namespace powerFlexBackup.powerFlexDrive
{

    public abstract class powerFlexDrive
    {
        private IdentityObject? identityObject;
        private driveParameterObject? parameterObject;
        private Sres.Net.EEIP.EEIPClient? eeipClient;
        private String? driveAddress;

        public static powerFlexDrive getDrivefromAddress(String driveAddress, Sres.Net.EEIP.EEIPClient eeipClient){
             
            eeipClient.RegisterSession(driveAddress);
            IdentityObject identityObject = getDriveIdentiyObjectfromAddress(driveAddress, eeipClient);
            Globals.logger.LogInformation ("Connected to device {0}, device reporting as {1}", driveAddress, identityObject.ProductName);

            //use case statement to determine which device type is being used         
            switch (identityObject.DeviceType)
            {
                case 150:
                    // PowerFlex 525
                    Globals.logger.LogInformation ("Identified drive as PowerFlex525 or compatable.");
                    return new PowerFlex525(driveAddress, 
                                                identityObject, 
                                                eeipClient);

                default:
                //FIXME: Application should close here with information about the device type
                    return new PowerFlex525(driveAddress, 
                                                identityObject, 
                                                eeipClient);     
            }

        }

        private static IdentityObject getDriveIdentiyObjectfromAddress(String driveAddress, Sres.Net.EEIP.EEIPClient eeipClient)
        {
            eeipClient.RegisterSession(driveAddress);
            byte[] response = eeipClient.GetAttributeAll(0x01, 1);
            return IdentityObject.getIdentityObjectfromResponse(response);
        }

        public void setDriveIdentiyObject(IdentityObject identityObject)
        {
            this.identityObject = identityObject;
        }

        public void setEEIPClient (Sres.Net.EEIP.EEIPClient Client){
            this.eeipClient = Client;
        }

        public void setDriveAddress (string driveAddress){
            this.driveAddress = driveAddress;
        }

        public void setDriveParameterList (List<DriveParameter> DriveParameterList){
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
            this.parameterObject = new driveParameterObject();
        }

        public void removeNonRecordedDriveParameters (){
            this.parameterObject.ParameterList.RemoveAll(x => x.recordValue == false);
        }
        public void removeDefaultDriveParameters (){
            this.parameterObject.ParameterList.RemoveAll(x => x.defaultParameterValue.Equals(x.parameterValue));
        }

        public List<DriveParameter> getDriveDriveParameterList (){
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
                foreach(DriveParameter Parameter in parameterObject.ParameterList)
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
