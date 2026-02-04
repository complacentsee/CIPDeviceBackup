using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using powerFlexBackup.cipdevice.deviceParameterObjects;
using Sres.Net.EEIP;

namespace powerFlexBackup.cipdevice
{

    public abstract class CIPDevice
    {
        private IdentityObject parentIdentityObject;
        public List<DeviceParameterObject> parameterObject;
        private Sres.Net.EEIP.EEIPClient eeipClient;
        private String deviceAddress;
        private byte[] CIPRoute;
        private bool deviceIsGeneric = false;
        protected readonly AppConfiguration config;
        protected readonly ILogger logger;

        public CIPDevice(
            String deviceAddress,
            Sres.Net.EEIP.EEIPClient eeipClient,
            byte[] CIPRoute,
            IOptions<AppConfiguration> options,
            ILogger logger)
        {
            this.deviceAddress = deviceAddress;
            this.eeipClient = eeipClient;
            this.CIPRoute = CIPRoute;
            this.config = options.Value;
            this.logger = logger;
            this.eeipClient.RegisterSession(deviceAddress);

            this.parentIdentityObject = new IdentityObject();
            var rawIdentityObject = getRawIdentiyObject();
            this.parentIdentityObject = parentIdentityObject.getIdentityObjectfromResponse(rawIdentityObject, logger);

            if(config.OutputVerbose){
                Console.WriteLine("Device Identity Object:");
                Console.WriteLine(JsonConvert.SerializeObject(this.parentIdentityObject, Formatting.Indented));
            }

            this.parameterObject = new List<DeviceParameterObject>();
            this.parameterObject.Add(new DeviceParameterObject(0x0F,this.parentIdentityObject, new List<DeviceParameter>()));

        }
        
        private static void registerDeviceAddress(String deviceAddress, Sres.Net.EEIP.EEIPClient eeipClient)
        {
            eeipClient.RegisterSession(deviceAddress);
        }

        public void setDeviceIdentiyObject(IdentityObject identityObject)
        {
            this.parentIdentityObject = identityObject;
        }

        public void setEEIPClient (Sres.Net.EEIP.EEIPClient Client){
            eeipClient = Client;
        }

        public void setDeviceAddress (string deviceAddress){
            this.deviceAddress = deviceAddress;
        }

        public void setDeviceParameterList (List<DeviceParameter> DeviceParameterList, int instance = 0){
            this.parameterObject[instance].ParameterList = DeviceParameterList;
        }

        public void setInstanceAttribute(List<InstanceAttribute> InstanceAttribute, int instance = 0){
            this.parameterObject[instance].instanceAttributes = InstanceAttribute;
        }

        public int getDeviceParameterClassID(int instance = 0){
            return this.parameterObject[instance].ClassID;
        }
        public void removeNonRecordedDeviceParameters(){
            foreach(var instance in this.parameterObject){
                instance.ParameterList.RemoveAll(x => x.record == false);
            }
        }
        public void removeReadOnlyDeviceParameters(){
            foreach(var instance in this.parameterObject){
                instance.ParameterList.RemoveAll(x => x.isWritable == false);
            }
        }
        public void removeDefaultDeviceParameters(){
            foreach(var instance in this.parameterObject){
                    instance.ParameterList.RemoveAll(x => x.defaultValue.Equals(x.value));
            }
        }

        public List<DeviceParameter> getDeviceParameterList(int instance = 0){
            return this.parameterObject[instance].ParameterList;
        }

        public IdentityObject getParentIdentityObject()
        {
            return this.parentIdentityObject;
        }
        public IdentityObject getIdentityObject(int instance = 0 )
        {
            return this.parameterObject[instance].identityObject;
        }

        public List<DeviceParameterObject> getParameterObject()
        {
            return this.parameterObject;
        }

        public void setDeviceIsGeneric(){
            this.deviceIsGeneric = true;
        }

        public void setParameterClassID(int ClassID, int instance = 0){
            this.parameterObject[instance].ClassID = ClassID;
        }
        public bool getDeviceIsGeneric(){
            return this.deviceIsGeneric;
        }

        public int getAttributeIDfromString(String attributeName, int instance = 0)
        {   try{
                return this.parameterObject[instance].instanceAttributes.Find(x => x.Name.Equals(attributeName))!.AttributeID;
            }
            catch(NullReferenceException){
                logger.LogWarning("Attribute {0} not found in instance {1}", attributeName, instance);
                return 0;
            }
        }

        public abstract void getDeviceParameterValues();

        public virtual void setInstanceAttributes(int instance = 0)
        {
            parameterObject[instance].instanceAttributes.Add(new InstanceAttribute(7, "Parameter Name String"));
            parameterObject[instance].instanceAttributes.Add(new InstanceAttribute(8, "Units String"));
            parameterObject[instance].instanceAttributes.Add(new InstanceAttribute(9, "Help String"));
            parameterObject[instance].instanceAttributes.Add(new InstanceAttribute(10, "Minimum Value"));
            parameterObject[instance].instanceAttributes.Add(new InstanceAttribute(11, "Maximum Value"));
            parameterObject[instance].instanceAttributes.Add(new InstanceAttribute(12, "Default Value"));
            parameterObject[instance].instanceAttributes.Add(new InstanceAttribute(13, "Scaling Multiplier"));
            parameterObject[instance].instanceAttributes.Add(new InstanceAttribute(14, "Scaling Divisor"));
            parameterObject[instance].instanceAttributes.Add(new InstanceAttribute(15, "Scaling Base"));
            parameterObject[instance].instanceAttributes.Add(new InstanceAttribute(16, "Scaling Offset"));
            parameterObject[instance].instanceAttributes.Add(new InstanceAttribute(17, "Multiplier Link"));
            parameterObject[instance].instanceAttributes.Add(new InstanceAttribute(18, "Divisor Link"));
            parameterObject[instance].instanceAttributes.Add(new InstanceAttribute(19, "Base Link"));
            parameterObject[instance].instanceAttributes.Add(new InstanceAttribute(20, "Offset Link"));
            parameterObject[instance].instanceAttributes.Add(new InstanceAttribute(21, "Decimal Precision"));
        }
        public void getDeviceParameterValuesCIPStandardCompliant(int instance = 0)
        {
            if(getDeviceIsGeneric()){
                getAllDeviceParameters();
            }

            if(parameterObject[instance].ParameterList != null){
                foreach(DeviceParameter Parameter in parameterObject[instance].ParameterList)
                    {
                        if(Parameter.record || config.OutputAllRecords){

                            if(Parameter.type is null)
                                Parameter.type = readDeviceParameterType(Parameter.number);

                            byte[] parameterValue = readDeviceParameterValue(Parameter.number);
                            Parameter.value = getParameterValuefromBytes(parameterValue,Parameter.type);

                            if(Parameter.defaultValue is null){
                                byte[] defaultParameterValue = readDeviceParameterDefaultValue(Parameter.number);
                                Parameter.defaultValue = getParameterValuefromBytes(defaultParameterValue,Parameter.type);
                            }

                            Parameter.valueHex = Convert.ToHexString(parameterValue);
                            Parameter.typeHex = Convert.ToHexString(Parameter.type);

                            if(config.OutputVerbose){
                                Console.WriteLine("Parameter #{0}, Value: {1}, ValueByte: {2}, Type: {3}", 
                                    Parameter.number, 
                                    Parameter.value, 
                                    Parameter.valueHex, 
                                    Parameter.typeHex);
                            }
                        }
                    }
            }
            return;
        }

        public abstract void getAllDeviceParameters();

        public void getAllDeviceParametersCIPStandardCompliant(int instance = 0){
            //clear existing parameter list incase one was loaded from class
            parameterObject[instance].ParameterList.Clear();

            var maxParameterNumber = readDeviceParameterMaxNumber();
            for(int i = 1; i <= maxParameterNumber; i++)
            {
                var parameterNumber = i;
                var parameterName = readDeviceParameterName(parameterNumber);
                var parameterType = readDeviceParameterType(parameterNumber);
                var parameterValue = readDeviceParameterValue(parameterNumber);
                var defaultParameterValue = readDeviceParameterDefaultValue(parameterNumber);
                var defaultParameterValueString = getParameterValuefromBytes(defaultParameterValue,parameterType);
                var parameterValueString = getParameterValuefromBytes(parameterValue,parameterType);
                var parameterDescriptor = readDeviceParameterDescriptor(parameterNumber);

                if(config.OutputVerbose){
                    Console.WriteLine("Parameter #{0}, Name: {1}, Value(Bytes): {2}, Type: {3}", 
                        parameterNumber, 
                        parameterName, 
                        Convert.ToHexString(parameterValue), 
                        Convert.ToHexString(parameterType));
                }

                var parameter = new DeviceParameter
                {
                    number = parameterNumber,
                    name = parameterName,
                    value = parameterValueString,
                    defaultValue = defaultParameterValueString,
                    record = true,
                    type = parameterType,
                    valueHex = Convert.ToHexString(parameterValue),
                    typeHex = Convert.ToHexString(parameterType),
                    Descriptor = Convert.ToHexString(parameterDescriptor),
                    isWritable = !parameterReadOnly(parameterDescriptor)
                };
                parameterObject[instance].ParameterList.Add(parameter);
            }
            return; 
        }

        public abstract int readDeviceParameterMaxNumber();
        public int readDeviceParameterMaxNumberCIPStandardCompliant(int  instance = 0){
            byte[] maxParameterNumberBytes = GetAttributeSingle(parameterObject[instance].ClassID, 0, 2);
            int maxParameterNumber = Convert.ToUInt16(maxParameterNumberBytes[0]
                                                        | maxParameterNumberBytes[1] << 8);
            return maxParameterNumber;
        }

        public byte[] GetAttributeSingle(int classID, int instanceID, int attributeID)
        {
            if (CIPRoute.Length > 0) {
                try{return eeipClient.GetAttributeSingle(CIPRoute, classID, instanceID, attributeID);}
                catch(Exception e){
                    logger.LogError("Failed to get attribute single (ClassID: {0}, InstanceID: {1}, AttributeID: {2}): {3}", classID, instanceID, attributeID, e.Message);
                    return new byte[0];}
            } else {
                try{return eeipClient.GetAttributeSingle(classID, instanceID, attributeID);}
                catch(Exception e){
                    logger.LogError("Failed to get attribute single (ClassID: {0}, InstanceID: {1}, AttributeID: {2}): {3}", classID, instanceID, attributeID, e.Message);
                    return new byte[0];}
            }
        }

        /// <summary>
        /// Send a generic CIP message - allows full control over service, class, instance, and data
        /// </summary>
        /// <param name="serviceCode">CIP Service Code (e.g., 0x0E for Get Attribute Single, 0x32 for Scattered Read)</param>
        /// <param name="classID">Target Class ID</param>
        /// <param name="instanceID">Target Instance ID</param>
        /// <param name="requestData">Request data bytes</param>
        /// <returns>Response data bytes</returns>
        protected byte[] SendGenericCIPMessage(byte serviceCode, int classID, int instanceID ,byte[] requestData)
        {
            if (CIPRoute.Length > 0) {
                try {
                    return eeipClient.GenericCIPMessage(CIPRoute, serviceCode, classID, instanceID, requestData);
                }
                catch(Exception e) {
                    logger.LogError("Failed to send generic CIP message (Service: 0x{0:X2}, Class: 0x{1:X2}, Instance: {2}): {3}",
                        serviceCode, classID, instanceID, e.Message);
                    throw;
                }
            } else {
                try {
                    return eeipClient.GenericCIPMessage(serviceCode, classID, instanceID, requestData);
                }
                catch(Exception e) {
                    logger.LogError("Failed to send generic CIP message (Service: 0x{0:X2}, Class: 0x{1:X2}, Instance: {2}): {3}",
                        serviceCode, classID, instanceID, e.Message);
                    throw;
                }
            }
        }

        private byte[] GetAttributeAll(int classID, int instanceID)
        {
            if (CIPRoute.Length > 0) {
                return eeipClient.GetAttributeAll(CIPRoute, classID, instanceID);
            } else {
                return eeipClient.GetAttributeAll(classID, instanceID);
            }
        }

        private String readDeviceParameterName(int parameterNumber, int instance = 0){
            try{
                byte[] parameterNameBytes = GetAttributeSingle(parameterObject[instance].ClassID, parameterNumber, getAttributeIDfromString("Parameter Name String"));
                if (parameterNameBytes.Length == 0) return "";
                String parameterName = System.Text.Encoding.ASCII.GetString(parameterNameBytes, 1, Convert.ToInt32(parameterNameBytes[0])).TrimEnd();
                return parameterName;
            }
            catch(Exception e){
                logger.LogError("Failed to read parameter name for parameter {0}, instance {1}: {2}", parameterNumber, instance, e.Message);
                return "";
            }
        }

        private byte[] getRawIdentiyObject()
        {
            try {
                return GetAttributeAll(0x01, 1);
            }
            catch (Sres.Net.EEIP.CIPException ex) when (ex.Message.Contains("Service not supported") && CIPRoute.Length > 0) {
                // DeviceNet routing doesn't support GetAttributeAll - fall back to individual reads
                return getIdentityObjectViaIndividualReads();
            }
        }

        private byte[] getIdentityObjectViaIndividualReads()
        {
            // Identity Object (Class 0x01) attributes
            var vendorId = GetAttributeSingle(0x01, 1, 1);      // 2 bytes
            var deviceType = GetAttributeSingle(0x01, 1, 2);    // 2 bytes
            var productCode = GetAttributeSingle(0x01, 1, 3);   // 2 bytes
            var revision = GetAttributeSingle(0x01, 1, 4);      // 2 bytes
            var status = GetAttributeSingle(0x01, 1, 5);        // 2 bytes
            var serialNumber = GetAttributeSingle(0x01, 1, 6);  // 4 bytes
            var productName = GetAttributeSingle(0x01, 1, 7);   // SHORT_STRING

            // Assemble same format as GetAttributeAll
            using (var ms = new System.IO.MemoryStream())
            {
                ms.Write(vendorId, 0, vendorId.Length);
                ms.Write(deviceType, 0, deviceType.Length);
                ms.Write(productCode, 0, productCode.Length);
                ms.Write(revision, 0, revision.Length);
                ms.Write(status, 0, status.Length);
                ms.Write(serialNumber, 0, serialNumber.Length);
                ms.Write(productName, 0, productName.Length);
                return ms.ToArray();
            }
        }


        protected byte[] readDeviceParameterValue(int parameterNumber)
        {
            return GetAttributeSingle(getDeviceParameterClassID(), parameterNumber, getAttributeIDfromString("Parameter Value"));
        }

        private byte[] readDeviceParameterDescriptor(int parameterNumber)
        {
            return GetAttributeSingle(getDeviceParameterClassID(), parameterNumber, getAttributeIDfromString("Descriptor"));
        }

        protected byte[] readDeviceParameterDefaultValue(int parameterNumber)
        {
            return GetAttributeSingle(getDeviceParameterClassID(), parameterNumber, getAttributeIDfromString("Default Value"));
        }

        protected byte[] readDeviceParameterType(int parameterNumber)
        {
            return GetAttributeSingle(getDeviceParameterClassID(), parameterNumber, getAttributeIDfromString("Data Type"));
        }

        private byte[] readDeviceParameterSize(int parameterNumber)
        {
            return GetAttributeSingle(getDeviceParameterClassID(), parameterNumber, getAttributeIDfromString("Data Size"));
        }

        public byte[] getRawIdentiyObjectfromInstance(int instance)
        {
            return GetAttributeAll(0x01, instance);
        }

        public string getParameterValuefromBytes(byte[] parameterValueBytes, byte[] type)
        {
            switch (type[0])
            {
                case 0xC1:
                    return CIPDeviceHelper.convertBytestoBOOL(parameterValueBytes);

                case 0xC2:
                    return CIPDeviceHelper.convertBytestoINT8(parameterValueBytes);

                case 0xC3:
                    return CIPDeviceHelper.convertBytesToINT16LittleEndian(parameterValueBytes);

                case 0xC4:
                    return CIPDeviceHelper.convertBytestoINT32LittleEndian(parameterValueBytes);

                case 0xC6:
                    return CIPDeviceHelper.convertBytestoUSINT8(parameterValueBytes);

                case 0xC7:
                    return CIPDeviceHelper.convertBytestoUINT16LittleEndian(parameterValueBytes);

                case 0xC8:
                    return CIPDeviceHelper.convertBytestoUINT32LittleEndian(parameterValueBytes);

                case 0xCA:
                    return CIPDeviceHelper.convertBytestoFloat32LittleEndian(parameterValueBytes);
                
                case 0xD1:
                    return CIPDeviceHelper.convertBytestoWORD(parameterValueBytes);
                
                case 0xD2:
                    return CIPDeviceHelper.convertBytestoDWORD(parameterValueBytes);
                
                case 0xD3:
                    return CIPDeviceHelper.convertBytestoQWORD(parameterValueBytes);

                // Non-standard type codes (used by 20-COMM-D DeviceNet adapter)
                // Determined empirically from parameter value byte sizes
                case 0x01:  // 2-byte value - treating as UINT16
                    return CIPDeviceHelper.convertBytestoUINT16LittleEndian(parameterValueBytes);

                case 0x02:  // 2-byte value - treating as INT16
                    return CIPDeviceHelper.convertBytesToINT16LittleEndian(parameterValueBytes);

                case 0x08:  // 1-byte value - treating as USINT
                case 0x18:  // 1-byte value - treating as USINT
                    return CIPDeviceHelper.convertBytestoUSINT8(parameterValueBytes);

                case 0x09:  // 4-byte value - treating as UDINT
                    return CIPDeviceHelper.convertBytestoUINT32LittleEndian(parameterValueBytes);

                default:
                    return "Unknown Parameter Type";    
            }
        }

        public bool parameterSupportsEnumberatedStrings(byte[] CIPStandardDescriptor){
            return (CIPStandardDescriptor[0] & (1 << 1)) != 0;
        }

        public bool parameterReadOnly(byte[] CIPStandardDescriptor){
            return (CIPStandardDescriptor[0] & (1 << 4)) != 0;
        }

        public bool parameterMonitorOnly(byte[] CIPStandardDescriptor){
            return (CIPStandardDescriptor[0] & (1 << 5)) != 0;
        }
    }
}