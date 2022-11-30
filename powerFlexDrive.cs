using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
namespace powerFlexBackup.powerFlexDrive
{

    public abstract class powerFlexDrive
    {
        private IdentityObject? identityObject;
        private DriveParameterList? DriveParameterList;
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

        public void setDriveDriveParameterList (DriveParameterList DriveParameterList){
            this.DriveParameterList = DriveParameterList;
        }

        public void removeNonRecordedDriveParameterList (){
            this.DriveParameterList.driveParameters.RemoveAll(x => x.recordValue == false);
        }

        public DriveParameterList getDriveDriveParameterList (){
            return this.DriveParameterList;
        }

        public IdentityObject getIdentityObject()
        {
            return identityObject;
        }

        public void getDriveParameterValues()
        {
            if(DriveParameterList.driveParameters != null){
                foreach(DriveParameter driveParameter in DriveParameterList.driveParameters)
                    {
                        if(driveParameter.recordValue){
                            if(driveParameter.parameterType is null)
                                driveParameter.parameterType = readDriveParameterType(driveParameter.parameterNumber);

                            byte[] parameterValue = readDriveParameterValue(driveParameter.parameterNumber);
                            driveParameter.parameterValue = getParameterValuefromBytes(parameterValue,driveParameter.parameterType);
                            Globals.logger.LogDebug(JsonConvert.SerializeObject(driveParameter,Formatting.Indented));
                        }
                    }
            }
            return;
        }

        public void getDriveParameterValuesParallel()
        {
            if(DriveParameterList.driveParameters != null){
                Parallel.ForEach(DriveParameterList.driveParameters, new ParallelOptions { MaxDegreeOfParallelism = 5}, driveParameter =>
                    {
                        if(driveParameter.parameterType is null)
                            driveParameter.parameterType = readDriveParameterType(driveParameter.parameterNumber);

                        byte[] parameterValue = readDriveParameterValue(driveParameter.parameterNumber);
                        driveParameter.parameterValue = getParameterValuefromBytes(parameterValue,driveParameter.parameterType);
                        Console.WriteLine(driveParameter.parameterNumber + ": " + driveParameter.parameterName + " : " + driveParameter.parameterValue + " : " + BitConverter.ToString(driveParameter.parameterType));
                    });
            }
            return;
        }

        private byte[] readDriveParameterValue(int parameterNumber)
        {
            return eeipClient.GetAttributeSingle(0x0F,parameterNumber, 1);
        }
        private byte[] readDriveParameterType(int parameterNumber)
        {
            return eeipClient.GetAttributeSingle(0x0F,parameterNumber, 5);;
        }

        public abstract string getParameterValuefromBytes(byte[] parameterValueBytes, byte[] parameterType);
    }
}
