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

        public CIPDevice_PowerFlex750Base(String deviceAddress, Sres.Net.EEIP.EEIPClient eeipClient, byte[] CIPRoute, IOptions<AppConfiguration> options, ILogger logger, IdentityObject identityObject) :
            base(deviceAddress, eeipClient, CIPRoute, options, logger, identityObject)
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
                    var temp = getRawIdentiyObjectfromInstance(i);
                    IdentityObject portIdentity;
                    try {
                        portIdentity = IdentityObject.getIdentityObjectfromResponse(temp, logger);
                    } catch (Exception e) {
                        Console.WriteLine("Error getting Identity Object from instance {0}: {1}", i, e.Message);
                        continue;
                    }

                    // SKIP empty port data collection and HIM modules. Maybe HIM modules should be collected?
                    bool deviceIsHIM = portIdentity.ProductName.Contains("HIM") || portIdentity.ProductCode == 767;
                    bool portIsEmpty = portIdentity.ProductName.Contains("Not") || portIdentity.ProductCode == 65280;

                    if (deviceIsHIM || portIsEmpty){
                        continue;
                    }
                        parameterObject.Add(new DeviceParameterObject(ClassID, portIdentity, new List<DeviceParameter>(), i-1));
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
                // Try to get predefined parameter list
                var predefinedParams = tryGetPredefinedParameterList(
                    PortGroup.identityObject,
                    out int classID,
                    out bool useScatteredRead);

                if (predefinedParams != null)
                {
                    // Use predefined list - faster!
                    Console.WriteLine("Loading predefined parameters for {0}",
                        PortGroup.identityObject.ProductName);

                    // Add parameters to the port group
                    PortGroup.ParameterList.AddRange(predefinedParams);

                    // Read actual values for recordable params
                    readParameterValues(predefinedParams, portMap[PortGroup.Port].Offset, classID, useScatteredRead);
                }
                else
                {
                    // Fall back to dynamic reading for unknown cards
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

                    Console.WriteLine("Getting Parameters for {0} (dynamic read)",
                        PortGroup.identityObject.ProductName);
                    getAllPortParameters(portMap[PortGroup.Port].Offset, index, ClassID);
                }

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

        /// <summary>
        /// Try to get predefined parameter list for a PowerFlex 750 port card.
        /// Returns null if no predefined list exists (triggers fallback to dynamic DPI reading).
        /// </summary>
        /// <param name="identity">Identity Object from the port containing ProductCode and ProductName</param>
        /// <param name="classID">Output: CIP Class ID for parameter access (0x9F or 0x93)</param>
        /// <returns>List of DeviceParameter definitions if card is known, null if unknown</returns>
        protected virtual List<DeviceParameter>? tryGetPredefinedParameterList(
            IdentityObject identity,
            out int classID,
            out bool useScatteredRead)
        {
            classID = 0x9F;  // Default to HOST memory class
            useScatteredRead = true;

            var cardDef = PortCards.PowerFlex750PortCard.getCardDefinitionForPort(
                identity.ProductCode,
                identity.ProductName);

            if (cardDef != null)
            {
                classID = cardDef.ClassID;
                useScatteredRead = cardDef.UseScatteredRead;
                logger.LogInformation("Using predefined parameter list for {0} (ProductCode {1})",
                    identity.ProductName, identity.ProductCode);
                return cardDef.getParameterList();
            }

            logger.LogInformation("No predefined list for {0} (ProductCode {1}), using dynamic DPI read",
                identity.ProductName, identity.ProductCode);
            return null;
        }

        // Scattered read configuration for PowerFlex 750 port cards (per 750-PM001)
        // Service 0x4D, Class 0x93, Instance 0, Attribute 0
        // Request:  DINT param_number + DINT pad   (8 bytes per param)
        // Response: DINT param_number + DINT value  (8 bytes per param)
        private const byte PF750ScatteredReadServiceCode = 0x4D;
        private const int PF750ScatteredReadClassID = 0x93;
        private const int PF750ScatteredReadInstanceID = 0;
        private const int PF750ScatteredReadAttributeID = 0;
        private const int PF750ScatteredReadBatchSize = 32;

        /// <summary>
        /// Read actual parameter values for a predefined port card parameter list using scattered read.
        /// Falls back to individual DPI Online Read Full if scattered read fails.
        /// </summary>
        /// <param name="parameters">Predefined parameter list from port card definition</param>
        /// <param name="offset">Port offset from portMap (e.g., 0x4400 for port 1)</param>
        /// <param name="classID">CIP Class ID for this card type (0x9F or 0x93)</param>
        protected virtual void readParameterValues(
            List<DeviceParameter> parameters,
            int offset,
            int classID,
            bool useScatteredRead = true)
        {
            var paramsToRead = parameters
                .Where(p => p.record || config.OutputAllRecords)
                .ToList();

            if (paramsToRead.Count == 0)
                return;

            // Build lookup by offset-adjusted parameter number -> DeviceParameter
            var paramsByOffsetNumber = new Dictionary<int, DeviceParameter>();
            foreach (var p in paramsToRead)
                paramsByOffsetNumber[p.number + offset] = p;

            var offsetParamNumbers = paramsToRead.Select(p => p.number + offset).ToList();

            if (!useScatteredRead)
            {
                logger.LogInformation("Reading {0} parameters from port offset 0x{1:X4} using individual DPI reads",
                    paramsToRead.Count, offset);
                ReadParameterValuesIndividual(offsetParamNumbers, paramsByOffsetNumber, classID);
                return;
            }

            logger.LogInformation("Reading {0} parameters from port offset 0x{1:X4} using scattered read (batches of {2})",
                paramsToRead.Count, offset, PF750ScatteredReadBatchSize);

            for (int i = 0; i < offsetParamNumbers.Count; i += PF750ScatteredReadBatchSize)
            {
                int count = Math.Min(PF750ScatteredReadBatchSize, offsetParamNumbers.Count - i);
                var batch = offsetParamNumbers.GetRange(i, count);

                try
                {
                    byte[] requestData = BuildPF750ScatteredReadRequest(batch);
                    byte[] response = SendGenericCIPMessage(
                        PF750ScatteredReadServiceCode,
                        PF750ScatteredReadClassID,
                        PF750ScatteredReadInstanceID,
                        PF750ScatteredReadAttributeID,
                        requestData);

                    var batchResults = ParsePF750ScatteredReadResponse(response, batch, paramsByOffsetNumber);
                    ApplyPF750ScatteredReadResults(batchResults, paramsByOffsetNumber);
                }
                catch (Exception ex)
                {
                    logger.LogWarning("Scattered read failed for port offset 0x{0:X4}, batch {1}-{2}: {3}. Falling back to individual DPI reads.",
                        offset, batch[0], batch[batch.Count - 1], ex.Message);
                    ReadParameterValuesIndividual(batch, paramsByOffsetNumber, classID);
                }
            }
        }

        /// <summary>
        /// PowerFlex 750 scattered read request format (per 750-PM001):
        /// DINT pairs: param_number + pad, param_number + pad, ...
        /// 8 bytes per parameter, max 32 parameters per request
        /// </summary>
        private byte[] BuildPF750ScatteredReadRequest(List<int> parameterNumbers)
        {
            byte[] requestData = new byte[parameterNumbers.Count * 8];
            int pos = 0;

            foreach (int paramNum in parameterNumbers)
            {
                // DINT parameter number (little-endian)
                requestData[pos++] = (byte)(paramNum & 0xFF);
                requestData[pos++] = (byte)((paramNum >> 8) & 0xFF);
                requestData[pos++] = (byte)((paramNum >> 16) & 0xFF);
                requestData[pos++] = (byte)((paramNum >> 24) & 0xFF);
                // DINT pad
                requestData[pos++] = 0;
                requestData[pos++] = 0;
                requestData[pos++] = 0;
                requestData[pos++] = 0;
            }

            return requestData;
        }

        /// <summary>
        /// PowerFlex 750 scattered read response format (per 750-PM001):
        /// DINT pairs: param_number + value, param_number + value, ...
        /// 8 bytes per parameter. Bit 15 set on param_number indicates error
        /// (value field contains error code).
        /// </summary>
        private Dictionary<int, byte[]> ParsePF750ScatteredReadResponse(byte[] response, List<int> parameterNumbers, Dictionary<int, DeviceParameter> paramsByOffsetNumber)
        {
            var results = new Dictionary<int, byte[]>();
            int pos = 0;

            foreach (int expectedParamNum in parameterNumbers)
            {
                if (pos + 8 > response.Length)
                {
                    logger.LogWarning("Scattered read response too short at parameter {0}", expectedParamNum);
                    break;
                }

                // DINT parameter number (little-endian), bit 15 = error flag
                int responseParamNum = response[pos] | (response[pos + 1] << 8)
                    | (response[pos + 2] << 16) | (response[pos + 3] << 24);
                pos += 4;

                // DINT value (4 bytes)
                byte[] value = new byte[4];
                value[0] = response[pos++];
                value[1] = response[pos++];
                value[2] = response[pos++];
                value[3] = response[pos++];

                // Check for error: device toggles bit 15 on the parameter number.
                // Compare against expected to detect error, since high-offset ports
                // (e.g., 755TS Port 10 at 0xA000) already have bit 15 set in normal param numbers.
                if (responseParamNum == expectedParamNum)
                {
                    // Success: response matches what we sent
                    results[responseParamNum] = value;
                }
                else if (responseParamNum == (expectedParamNum ^ 0x8000))
                {
                    // Error: bit 15 was toggled by the device
                    int errorCode = value[0] | (value[1] << 8) | (value[2] << 16) | (value[3] << 24);
                    string paramInfo = paramsByOffsetNumber.TryGetValue(expectedParamNum, out var errParam)
                        ? $"{errParam.number} [{errParam.name}]"
                        : $"0x{expectedParamNum:X}";
                    logger.LogWarning("Scattered read: parameter {0} returned error code 0x{1:X}", paramInfo, errorCode);
                }
                else
                {
                    // Unexpected parameter number in response
                    logger.LogWarning("Scattered read: unexpected response param 0x{0:X}, expected 0x{1:X}",
                        responseParamNum, expectedParamNum);
                }
            }

            return results;
        }

        /// <summary>
        /// Apply scattered read results to DeviceParameter objects.
        /// </summary>
        private void ApplyPF750ScatteredReadResults(Dictionary<int, byte[]> results, Dictionary<int, DeviceParameter> paramsByOffsetNumber)
        {
            foreach (var kvp in results)
            {
                if (paramsByOffsetNumber.TryGetValue(kvp.Key, out var param) && param.type != null)
                {
                    param.value = getParameterValuefromBytes(kvp.Value, param.type);
                    param.valueHex = Convert.ToHexString(kvp.Value);

                    if (config.OutputVerbose)
                        Console.WriteLine($"Parameter {param.number} [{param.name}] = {param.value}");
                }
            }
        }

        /// <summary>
        /// Fallback: read parameters individually via DPI Online Read Full.
        /// Used when scattered read fails for a batch.
        /// </summary>
        private void ReadParameterValuesIndividual(List<int> offsetParamNumbers, Dictionary<int, DeviceParameter> paramsByOffsetNumber, int classID)
        {
            foreach (int offsetParamNum in offsetParamNumbers)
            {
                if (!paramsByOffsetNumber.TryGetValue(offsetParamNum, out var param))
                    continue;

                try
                {
                    var paramData = DPIOnlineReadFull.ByteArrayToDeviceParameter(
                        readPF750DPIOnlineReadFull(offsetParamNum, classID),
                        logger,
                        this.getParameterValuefromBytes);

                    param.value = paramData.ParameterValue.value;
                    param.valueHex = Convert.ToHexString(paramData.ParameterValue.toBytes());

                    if (param.defaultValue == null)
                        param.defaultValue = paramData.DefaultValue.value;

                    if (config.OutputVerbose)
                        Console.WriteLine($"Parameter {param.number} [{param.name}] = {param.value}");
                }
                catch (Exception ex)
                {
                    logger.LogWarning("Failed to read parameter {0} (offset 0x{1:X}): {2}",
                        param.number, offsetParamNum, ex.Message);
                }
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
