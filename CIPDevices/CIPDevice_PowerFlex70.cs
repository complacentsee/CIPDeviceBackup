using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using powerFlexBackup.cipdevice.deviceParameterObjects;

namespace powerFlexBackup.cipdevice
{
    [SupportedDevice("PowerFlex 70 Series", 123, 50, true)]
    public class CIPDevice_PowerFlex70 : CIPDevice{

        // Scattered read configuration for PowerFlex 70
        // Uses service 0x4B, max 22 parameters, 6 bytes per parameter (3 words)
        protected virtual int ScatteredReadBatchSize => 22;
        private const byte ScatteredReadServiceCode = 0x4B;
        private const int ScatteredReadClassID = 0x93;
        private const int ScatteredReadInstanceID = 0;

        // Comm adapter parameters start at 629 - need separate batch
        private const int CommAdapterParamStart = 629;

        public CIPDevice_PowerFlex70(String deviceAddress, Sres.Net.EEIP.EEIPClient eeipClient, byte[] CIPRoute, IOptions<AppConfiguration> options, ILogger logger) :
            base(deviceAddress, eeipClient, CIPRoute, options, logger)
        {
            setParameterClassID(0x0F);
            setInstanceAttributes();

            // Combine drive parameters with comm adapter parameters
            var allParams = JsonConvert.DeserializeObject<List<DeviceParameter>>(driveParameterListJSON)!;
            var commParams = JsonConvert.DeserializeObject<List<DeviceParameter>>(commAdapterParameterListJSON)!;
            allParams.AddRange(commParams);
            setDeviceParameterList(allParams);
        }

        /// <summary>
        /// PowerFlex 70 scattered read request format:
        /// For each parameter: UINT16 param_number + UINT16 pad + UINT16 pad (6 bytes per param)
        /// </summary>
        protected virtual byte[] BuildScatteredReadRequest(List<int> parameterNumbers)
        {
            byte[] requestData = new byte[parameterNumbers.Count * 6];
            int offset = 0;

            foreach (int paramNum in parameterNumbers)
            {
                requestData[offset++] = (byte)(paramNum & 0xFF);
                requestData[offset++] = (byte)((paramNum >> 8) & 0xFF);
                requestData[offset++] = 0;
                requestData[offset++] = 0;
                requestData[offset++] = 0;
                requestData[offset++] = 0;
            }

            return requestData;
        }

        /// <summary>
        /// PowerFlex 70 scattered read response format:
        /// For each parameter: INT16 param_number + UINT16 value_LSW + UINT16 value_MSW (6 bytes per param)
        /// High bit set on param_number indicates error (value field contains error code)
        /// </summary>
        /// <param name="errorParams">List to collect parameter numbers that returned errors</param>
        private Dictionary<int, byte[]> ParseScatteredReadResponse(byte[] response, List<int> parameterNumbers, List<int>? errorParams = null)
        {
            var results = new Dictionary<int, byte[]>();
            int offset = 0;

            foreach (int expectedParamNum in parameterNumbers)
            {
                if (offset + 6 > response.Length)
                {
                    logger.LogWarning("Response too short for parameter {0}", expectedParamNum);
                    break;
                }

                // Read parameter number (INT16 - high bit indicates error)
                short responseParamNum = (short)(response[offset] | (response[offset + 1] << 8));
                offset += 2;

                // Read 32-bit value (LSW first, then MSW)
                byte[] value = new byte[4];
                value[0] = response[offset++];
                value[1] = response[offset++];
                value[2] = response[offset++];
                value[3] = response[offset++];

                // Check for error (high bit set = negative param number)
                if (responseParamNum < 0)
                {
                    int actualParamNum = responseParamNum & 0x7FFF;
                    int errorCode = value[0] | (value[1] << 8);
                    logger.LogWarning("Parameter {0} returned error code: {1}", actualParamNum, errorCode);
                    errorParams?.Add(actualParamNum);
                    continue;
                }

                if (responseParamNum != expectedParamNum)
                {
                    logger.LogWarning("Parameter number mismatch: expected {0}, got {1}",
                        expectedParamNum, responseParamNum);
                }

                results[responseParamNum] = value;
            }

            return results;
        }

        public override void getDeviceParameterValues()
        {
            if(parameterObject[0].ParameterList == null)
                return;

            var driveParams = new List<int>();
            var commAdapterParams = new List<int>();
            var paramsByNumber = new Dictionary<int, DeviceParameter>();

            foreach(DeviceParameter parameter in parameterObject[0].ParameterList)
            {
                if(parameter.record || config.OutputAllRecords)
                {
                    if(parameter.type is null)
                        parameter.type = readDeviceParameterType(parameter.number);

                    paramsByNumber[parameter.number] = parameter;

                    // Split at comm adapter boundary
                    if(parameter.number < CommAdapterParamStart)
                        driveParams.Add(parameter.number);
                    else
                        commAdapterParams.Add(parameter.number);
                }
            }

            if(driveParams.Count == 0 && commAdapterParams.Count == 0)
                return;

            // Process drive parameters with scattered read
            if(driveParams.Count > 0)
            {
                logger.LogInformation("Reading {0} drive parameters using scattered read (batches of {1})",
                    driveParams.Count, ScatteredReadBatchSize);
                ProcessScatteredReadBatches(driveParams, paramsByNumber);
            }

            // Process comm adapter parameters individually (scattered read not supported)
            if(commAdapterParams.Count > 0)
            {
                logger.LogInformation("Reading {0} comm adapter parameters individually",
                    commAdapterParams.Count);
                ProcessIndividualReads(commAdapterParams, paramsByNumber);
            }
        }

        private void ProcessScatteredReadBatches(List<int> paramsToRead, Dictionary<int, DeviceParameter> paramsByNumber)
        {
            for (int i = 0; i < paramsToRead.Count; i += ScatteredReadBatchSize)
            {
                int count = Math.Min(ScatteredReadBatchSize, paramsToRead.Count - i);
                var batch = paramsToRead.GetRange(i, count);

                try
                {
                    byte[] requestData = BuildScatteredReadRequest(batch);
                    byte[] response = SendGenericCIPMessage(
                        ScatteredReadServiceCode,
                        ScatteredReadClassID,
                        ScatteredReadInstanceID,
                        requestData
                    );

                    var batchResults = ParseScatteredReadResponse(response, batch);
                    ApplyParameterResults(batchResults, paramsByNumber);
                }
                catch (Exception ex)
                {
                    logger.LogWarning("Scattered read failed for batch {0}-{1}: {2}. Falling back to individual reads.",
                        batch[0], batch[batch.Count-1], ex.Message);
                    ProcessIndividualReads(batch, paramsByNumber);
                }
            }
        }

        private void ProcessIndividualReads(List<int> paramsToRead, Dictionary<int, DeviceParameter> paramsByNumber)
        {
            foreach (int paramNum in paramsToRead)
            {
                try
                {
                    if(paramsByNumber.TryGetValue(paramNum, out var parameter) && parameter.type != null)
                    {
                        byte[] value = readDeviceParameterValue(paramNum);
                        parameter.value = getParameterValuefromBytes(value, parameter.type);

                        if(parameter.defaultValue == null)
                        {
                            try
                            {
                                byte[] defaultValue = readDeviceParameterDefaultValue(paramNum);
                                parameter.defaultValue = getParameterValuefromBytes(defaultValue, parameter.type);
                            }
                            catch { }
                        }

                        if(config.OutputVerbose)
                        {
                            Console.WriteLine($"Parameter {parameter.number} [{parameter.name}] = {parameter.value}");
                        }
                    }
                }
                catch
                {
                    logger.LogWarning("Failed to read parameter {0}", paramNum);
                }
            }
        }

        private void ApplyParameterResults(Dictionary<int, byte[]> results, Dictionary<int, DeviceParameter> paramsByNumber)
        {
            foreach(var kvp in results)
            {
                if(paramsByNumber.TryGetValue(kvp.Key, out var parameter) && parameter.type != null)
                {
                    parameter.value = getParameterValuefromBytes(kvp.Value, parameter.type);

                    if(parameter.defaultValue == null)
                    {
                        try
                        {
                            byte[] defaultValue = readDeviceParameterDefaultValue(parameter.number);
                            parameter.defaultValue = getParameterValuefromBytes(defaultValue, parameter.type);
                        }
                        catch { }
                    }

                    if(config.OutputVerbose)
                    {
                        Console.WriteLine($"Parameter {parameter.number} [{parameter.name}] = {parameter.value}");
                    }
                }
            }
        }

        public override void getAllDeviceParameters(){
            getAllDeviceParametersCIPStandardCompliant();
        }

        public override int readDeviceParameterMaxNumber(){
            return readDeviceParameterMaxNumberCIPStandardCompliant();
        }

        // Drive parameters (1-628) - common to all comm adapters
        public string driveParameterListJSON = @"[ {
                        'number': '1',
                        'name': 'Output Freq',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'ww=='
                        },
                        {
                        'number': '2',
                        'name': 'Commanded Freq',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'ww=='
                        },
                        {
                        'number': '3',
                        'name': 'Output Current',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'yA=='
                        },
                        {
                        'number': '4',
                        'name': 'Torque Current',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xA=='
                        },
                        {
                        'number': '5',
                        'name': 'Flux Current',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xA=='
                        },
                        {
                        'number': '6',
                        'name': 'Output Voltage',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '7',
                        'name': 'Output Power',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'yA=='
                        },
                        {
                        'number': '8',
                        'name': 'Output Powr Fctr',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '9',
                        'name': 'Elapsed MWh',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'yA=='
                        },
                        {
                        'number': '10',
                        'name': 'Elapsed Run Time',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'yA=='
                        },
                        {
                        'number': '11',
                        'name': 'MOP Frequency',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'ww=='
                        },
                        {
                        'number': '12',
                        'name': 'DC Bus Voltage',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '13',
                        'name': 'DC Bus Memory',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '14',
                        'name': 'Elapsed kWh',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'yA=='
                        },
                        {
                        'number': '15',
                        'name': 'Torque Estimate',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'ww=='
                        },
                        {
                        'number': '16',
                        'name': 'Analog In1 Value',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'ww=='
                        },
                        {
                        'number': '17',
                        'name': 'Analog In2 Value',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'ww=='
                        },
                        {
                        'number': '18',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '19',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '20',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '21',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '22',
                        'name': 'Ramped Speed',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'ww=='
                        },
                        {
                        'number': '23',
                        'name': 'Speed Reference',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'ww=='
                        },
                        {
                        'number': '24',
                        'name': 'Commanded Torque',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'ww=='
                        },
                        {
                        'number': '25',
                        'name': 'Speed Feedback',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'ww=='
                        },
                        {
                        'number': '26',
                        'name': 'Rated kW',
                        'defaultValue': '400',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '27',
                        'name': 'Rated Volts',
                        'defaultValue': '4800',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '28',
                        'name': 'Rated Amps',
                        'defaultValue': '80',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '29',
                        'name': 'Control SW Ver',
                        'defaultValue': '5001',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '30',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '31',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '32',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '33',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '34',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '35',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '36',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '37',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '38',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '39',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '40',
                        'name': 'Motor Type',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '41',
                        'name': 'Motor NP Volts',
                        'defaultValue': '4600',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '42',
                        'name': 'Motor NP FLA',
                        'defaultValue': '64',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '43',
                        'name': 'Motor NP Hertz',
                        'defaultValue': '600',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '44',
                        'name': 'Motor NP RPM',
                        'defaultValue': '1750',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '45',
                        'name': 'Motor NP Power',
                        'defaultValue': '500',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '46',
                        'name': 'Mtr NP Pwr Units',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '47',
                        'name': 'Motor OL Hertz',
                        'defaultValue': '200',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '48',
                        'name': 'Motor OL Factor',
                        'defaultValue': '100',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '49',
                        'name': 'Motor Poles',
                        'defaultValue': '4',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '50',
                        'name': 'Motor OL Mode',
                        'defaultValue': '0000000000000000',
                        'record': 'true',
                        'type': '0g=='
                        },
                        {
                        'number': '51',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '52',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '53',
                        'name': 'Motor Cntl Sel',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '54',
                        'name': 'Maximum Voltage',
                        'defaultValue': '4600',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '55',
                        'name': 'Maximum Freq',
                        'defaultValue': '1300',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '56',
                        'name': 'Compensation',
                        'defaultValue': '0000000000011011',
                        'record': 'true',
                        'type': '0g=='
                        },
                        {
                        'number': '57',
                        'name': 'Flux Up Mode',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '58',
                        'name': 'Flux Up Time',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '59',
                        'name': 'SV Boost Filter',
                        'defaultValue': '500',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '60',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '61',
                        'name': 'Autotune',
                        'defaultValue': '3',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '62',
                        'name': 'IR Voltage Drop',
                        'defaultValue': '128',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '63',
                        'name': 'Flux Current Ref',
                        'defaultValue': '256',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '64',
                        'name': 'IXo Voltage Drop',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '65',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '66',
                        'name': 'Autotune Torque',
                        'defaultValue': '500',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '67',
                        'name': 'Inertia Autotune',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '68',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '69',
                        'name': 'Start\/Acc Boost',
                        'defaultValue': '51',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '70',
                        'name': 'Run Boost',
                        'defaultValue': '51',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '71',
                        'name': 'Break Voltage',
                        'defaultValue': '1150',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '72',
                        'name': 'Break Frequency',
                        'defaultValue': '150',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '73',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '74',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '75',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '76',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '77',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '78',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '79',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '80',
                        'name': 'Feedback Select',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '81',
                        'name': 'Minimum Speed',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '82',
                        'name': 'Maximum Speed',
                        'defaultValue': '600',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '83',
                        'name': 'Overspeed Limit',
                        'defaultValue': '100',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '84',
                        'name': 'Skip Frequency 1',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'ww=='
                        },
                        {
                        'number': '85',
                        'name': 'Skip Frequency 2',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'ww=='
                        },
                        {
                        'number': '86',
                        'name': 'Skip Frequency 3',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'ww=='
                        },
                        {
                        'number': '87',
                        'name': 'Skip Freq Band',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '88',
                        'name': 'Speed\/Torque Mod',
                        'defaultValue': '1',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '89',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '90',
                        'name': 'Speed Ref A Sel',
                        'defaultValue': '2',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '91',
                        'name': 'Speed Ref A Hi',
                        'defaultValue': '600',
                        'record': 'true',
                        'type': 'ww=='
                        },
                        {
                        'number': '92',
                        'name': 'Speed Ref A Lo',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'ww=='
                        },
                        {
                        'number': '93',
                        'name': 'Speed Ref B Sel',
                        'defaultValue': '11',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '94',
                        'name': 'Speed Ref B Hi',
                        'defaultValue': '600',
                        'record': 'true',
                        'type': 'ww=='
                        },
                        {
                        'number': '95',
                        'name': 'Speed Ref B Lo',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'ww=='
                        },
                        {
                        'number': '96',
                        'name': 'TB Man Ref Sel',
                        'defaultValue': '1',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '97',
                        'name': 'TB Man Ref Hi',
                        'defaultValue': '600',
                        'record': 'true',
                        'type': 'ww=='
                        },
                        {
                        'number': '98',
                        'name': 'TB Man Ref Lo',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'ww=='
                        },
                        {
                        'number': '99',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '100',
                        'name': 'Jog Speed 1',
                        'defaultValue': '100',
                        'record': 'true',
                        'type': 'ww=='
                        },
                        {
                        'number': '101',
                        'name': 'Preset Speed 1',
                        'defaultValue': '50',
                        'record': 'true',
                        'type': 'ww=='
                        },
                        {
                        'number': '102',
                        'name': 'Preset Speed 2',
                        'defaultValue': '100',
                        'record': 'true',
                        'type': 'ww=='
                        },
                        {
                        'number': '103',
                        'name': 'Preset Speed 3',
                        'defaultValue': '200',
                        'record': 'true',
                        'type': 'ww=='
                        },
                        {
                        'number': '104',
                        'name': 'Preset Speed 4',
                        'defaultValue': '300',
                        'record': 'true',
                        'type': 'ww=='
                        },
                        {
                        'number': '105',
                        'name': 'Preset Speed 5',
                        'defaultValue': '400',
                        'record': 'true',
                        'type': 'ww=='
                        },
                        {
                        'number': '106',
                        'name': 'Preset Speed 6',
                        'defaultValue': '500',
                        'record': 'true',
                        'type': 'ww=='
                        },
                        {
                        'number': '107',
                        'name': 'Preset Speed 7',
                        'defaultValue': '600',
                        'record': 'true',
                        'type': 'ww=='
                        },
                        {
                        'number': '108',
                        'name': 'Jog Speed 2',
                        'defaultValue': '100',
                        'record': 'true',
                        'type': 'ww=='
                        },
                        {
                        'number': '109',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '110',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '111',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '112',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '113',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '114',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '115',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '116',
                        'name': 'Trim % Setpoint',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'ww=='
                        },
                        {
                        'number': '117',
                        'name': 'Trim In Select',
                        'defaultValue': '2',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '118',
                        'name': 'Trim Out Select',
                        'defaultValue': '0000000000000000',
                        'record': 'true',
                        'type': '0g=='
                        },
                        {
                        'number': '119',
                        'name': 'Trim Hi',
                        'defaultValue': '600',
                        'record': 'true',
                        'type': 'ww=='
                        },
                        {
                        'number': '120',
                        'name': 'Trim Lo',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'ww=='
                        },
                        {
                        'number': '121',
                        'name': 'Slip RPM @ FLA',
                        'defaultValue': '500',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '122',
                        'name': 'Slip Comp Gain',
                        'defaultValue': '400',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '123',
                        'name': 'Slip RPM Meter',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'ww=='
                        },
                        {
                        'number': '124',
                        'name': 'PI Configuration',
                        'defaultValue': '0000000000000000',
                        'record': 'true',
                        'type': '0g=='
                        },
                        {
                        'number': '125',
                        'name': 'PI Control',
                        'defaultValue': '0000000000000000',
                        'record': 'true',
                        'type': '0g=='
                        },
                        {
                        'number': '126',
                        'name': 'PI Reference Sel',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '127',
                        'name': 'PI Setpoint',
                        'defaultValue': '5000',
                        'record': 'true',
                        'type': 'ww=='
                        },
                        {
                        'number': '128',
                        'name': 'PI Feedback Sel',
                        'defaultValue': '2',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '129',
                        'name': 'PI Integral Time',
                        'defaultValue': '200',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '130',
                        'name': 'PI Prop Gain',
                        'defaultValue': '100',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '131',
                        'name': 'PI Lower Limit',
                        'defaultValue': '-1000',
                        'record': 'true',
                        'type': 'ww=='
                        },
                        {
                        'number': '132',
                        'name': 'PI Upper Limit',
                        'defaultValue': '1000',
                        'record': 'true',
                        'type': 'ww=='
                        },
                        {
                        'number': '133',
                        'name': 'PI Preload',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'ww=='
                        },
                        {
                        'number': '134',
                        'name': 'PI Status',
                        'defaultValue': '0000000000000000',
                        'record': 'false',
                        'type': '0g=='
                        },
                        {
                        'number': '135',
                        'name': 'PI Ref Meter',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'ww=='
                        },
                        {
                        'number': '136',
                        'name': 'PI Fdback Meter',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'ww=='
                        },
                        {
                        'number': '137',
                        'name': 'PI Error Meter',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'ww=='
                        },
                        {
                        'number': '138',
                        'name': 'PI Output Meter',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'ww=='
                        },
                        {
                        'number': '139',
                        'name': 'PI BW Filter',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '140',
                        'name': 'Accel Time 1',
                        'defaultValue': '100',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '141',
                        'name': 'Accel Time 2',
                        'defaultValue': '100',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '142',
                        'name': 'Decel Time 1',
                        'defaultValue': '100',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '143',
                        'name': 'Decel Time 2',
                        'defaultValue': '100',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '144',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '145',
                        'name': 'DB While Stopped',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '146',
                        'name': 'S Curve %',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '147',
                        'name': 'Current Lmt Sel',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '148',
                        'name': 'Current Lmt Val',
                        'defaultValue': '120',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '149',
                        'name': 'Current Lmt Gain',
                        'defaultValue': '250',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '150',
                        'name': 'Drive OL Mode',
                        'defaultValue': '3',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '151',
                        'name': 'PWM Frequency',
                        'defaultValue': '4',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '152',
                        'name': 'Droop RPM @ FLA',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '153',
                        'name': 'Regen Power Lim',
                        'defaultValue': '-500',
                        'record': 'true',
                        'type': 'ww=='
                        },
                        {
                        'number': '154',
                        'name': 'Current Rate Lim',
                        'defaultValue': '4000',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '155',
                        'name': 'Stop\/Brk Mode A',
                        'defaultValue': '1',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '156',
                        'name': 'Stop\/Brk Mode B',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '157',
                        'name': 'DC Brake Lvl Sel',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '158',
                        'name': 'DC Brake Level',
                        'defaultValue': '80',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '159',
                        'name': 'DC Brake Time',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '160',
                        'name': 'Bus Reg Ki',
                        'defaultValue': '450',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '161',
                        'name': 'Bus Reg Mode A',
                        'defaultValue': '1',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '162',
                        'name': 'Bus Reg Mode B',
                        'defaultValue': '4',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '163',
                        'name': 'DB Resistor Type',
                        'defaultValue': '2',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '164',
                        'name': 'Bus Reg Kp',
                        'defaultValue': '1500',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '165',
                        'name': 'Bus Reg Kd',
                        'defaultValue': '1000',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '166',
                        'name': 'Flux Braking',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '167',
                        'name': 'Powerup Delay',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '168',
                        'name': 'Start At PowerUp',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '169',
                        'name': 'Flying Start En',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '170',
                        'name': 'Flying StartGain',
                        'defaultValue': '4000',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '171',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '172',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '173',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '174',
                        'name': 'Auto Rstrt Tries',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '175',
                        'name': 'Auto Rstrt Delay',
                        'defaultValue': '10',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '176',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '177',
                        'name': 'Gnd Warn Level',
                        'defaultValue': '30',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '178',
                        'name': 'Sleep Wake Mode',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '179',
                        'name': 'Sleep Wake Ref',
                        'defaultValue': '2',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '180',
                        'name': 'Wake Level',
                        'defaultValue': '6000',
                        'record': 'true',
                        'type': 'ww=='
                        },
                        {
                        'number': '181',
                        'name': 'Wake Time',
                        'defaultValue': '10',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '182',
                        'name': 'Sleep Level',
                        'defaultValue': '5000',
                        'record': 'true',
                        'type': 'ww=='
                        },
                        {
                        'number': '183',
                        'name': 'Sleep Time',
                        'defaultValue': '10',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '184',
                        'name': 'Power Loss Mode',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '185',
                        'name': 'Power Loss Time',
                        'defaultValue': '5',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '186',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '187',
                        'name': 'Load Loss Level',
                        'defaultValue': '2000',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '188',
                        'name': 'Load Loss Time',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '189',
                        'name': 'Shear Pin Time',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '190',
                        'name': 'Direction Mode',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '191',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '192',
                        'name': 'AutoMan Cnfg',
                        'defaultValue': '0000000000000001',
                        'record': 'true',
                        'type': '0g=='
                        },
                        {
                        'number': '193',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '194',
                        'name': 'Save MOP Ref',
                        'defaultValue': '0000000000000000',
                        'record': 'true',
                        'type': '0g=='
                        },
                        {
                        'number': '195',
                        'name': 'MOP Rate',
                        'defaultValue': '10',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '196',
                        'name': 'Param Access Lvl',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '197',
                        'name': 'Reset To Defalts',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '198',
                        'name': 'Load Frm Usr Set',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '199',
                        'name': 'Save To User Set',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '200',
                        'name': 'Reset Meters',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '201',
                        'name': 'Language',
                        'defaultValue': '1',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '202',
                        'name': 'Voltage Class',
                        'defaultValue': '3',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '203',
                        'name': 'Drive Checksum',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xA=='
                        },
                        {
                        'number': '204',
                        'name': 'Dyn UserSet Cnfg',
                        'defaultValue': '0000000000000000',
                        'record': 'true',
                        'type': '0g=='
                        },
                        {
                        'number': '205',
                        'name': 'Dyn UserSet Sel',
                        'defaultValue': '0000000000000000',
                        'record': 'true',
                        'type': '0g=='
                        },
                        {
                        'number': '206',
                        'name': 'Dyn UserSet Actv',
                        'defaultValue': '0000000000000000',
                        'record': 'false',
                        'type': '0g=='
                        },
                        {
                        'number': '207',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '208',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '209',
                        'name': 'Drive Status 1',
                        'defaultValue': '0000000000000000',
                        'record': 'false',
                        'type': '0g=='
                        },
                        {
                        'number': '210',
                        'name': 'Drive Status 2',
                        'defaultValue': '0000000000000000',
                        'record': 'false',
                        'type': '0g=='
                        },
                        {
                        'number': '211',
                        'name': 'Drive Alarm 1',
                        'defaultValue': '0000000000000000',
                        'record': 'false',
                        'type': '0g=='
                        },
                        {
                        'number': '212',
                        'name': 'Drive Alarm 2',
                        'defaultValue': '00000000000000000000000000000000',
                        'record': 'false',
                        'type': '0w=='
                        },
                        {
                        'number': '213',
                        'name': 'Speed Ref Source',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '214',
                        'name': 'Start Inhibits',
                        'defaultValue': '0000000000000000',
                        'record': 'false',
                        'type': '0g=='
                        },
                        {
                        'number': '215',
                        'name': 'Last Stop Source',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '216',
                        'name': 'Dig In Status',
                        'defaultValue': '0000000000000000',
                        'record': 'false',
                        'type': '0g=='
                        },
                        {
                        'number': '217',
                        'name': 'Dig Out Status',
                        'defaultValue': '0000000000000000',
                        'record': 'false',
                        'type': '0g=='
                        },
                        {
                        'number': '218',
                        'name': 'Drive Temp',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'ww=='
                        },
                        {
                        'number': '219',
                        'name': 'Drive OL Count',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '220',
                        'name': 'Motor OL Count',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '221',
                        'name': 'Mtr OL Trip Time',
                        'defaultValue': '99999',
                        'record': 'false',
                        'type': 'yA=='
                        },
                        {
                        'number': '222',
                        'name': 'Drive Status 3',
                        'defaultValue': '0000000000000000',
                        'record': 'false',
                        'type': '0g=='
                        },
                        {
                        'number': '223',
                        'name': 'Status 3 @ Fault',
                        'defaultValue': '0000000000000000',
                        'record': 'false',
                        'type': '0g=='
                        },
                        {
                        'number': '224',
                        'name': 'Fault Frequency',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'ww=='
                        },
                        {
                        'number': '225',
                        'name': 'Fault Amps',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '226',
                        'name': 'Fault Bus Volts',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '227',
                        'name': 'Status 1 @ Fault',
                        'defaultValue': '0000000000000000',
                        'record': 'false',
                        'type': '0g=='
                        },
                        {
                        'number': '228',
                        'name': 'Status 2 @ Fault',
                        'defaultValue': '0000000000000000',
                        'record': 'false',
                        'type': '0g=='
                        },
                        {
                        'number': '229',
                        'name': 'Alarm 1 @ Fault',
                        'defaultValue': '0000000000000000',
                        'record': 'false',
                        'type': '0g=='
                        },
                        {
                        'number': '230',
                        'name': 'Alarm 2 @ Fault',
                        'defaultValue': '00000000000000000000000000000000',
                        'record': 'false',
                        'type': '0w=='
                        },
                        {
                        'number': '231',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '232',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '233',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '234',
                        'name': 'Testpoint 1 Sel',
                        'defaultValue': '499',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '235',
                        'name': 'Testpoint 1 Data',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xA=='
                        },
                        {
                        'number': '236',
                        'name': 'Testpoint 2 Sel',
                        'defaultValue': '499',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '237',
                        'name': 'Testpoint 2 Data',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xA=='
                        },
                        {
                        'number': '238',
                        'name': 'Fault Config 1',
                        'defaultValue': '0000000001001110',
                        'record': 'true',
                        'type': '0g=='
                        },
                        {
                        'number': '239',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '240',
                        'name': 'Fault Clear',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '241',
                        'name': 'Fault Clear Mode',
                        'defaultValue': '1',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '242',
                        'name': 'Power Up Marker',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'yA=='
                        },
                        {
                        'number': '243',
                        'name': 'Fault 1 Code',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '244',
                        'name': 'Fault 1 Time',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'yA=='
                        },
                        {
                        'number': '245',
                        'name': 'Fault 2 Code',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '246',
                        'name': 'Fault 2 Time',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'yA=='
                        },
                        {
                        'number': '247',
                        'name': 'Fault 3 Code',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '248',
                        'name': 'Fault 3 Time',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'yA=='
                        },
                        {
                        'number': '249',
                        'name': 'Fault 4 Code',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '250',
                        'name': 'Fault 4 Time',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'yA=='
                        },
                        {
                        'number': '251',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '252',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '253',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '254',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '255',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '256',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '257',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '258',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '259',
                        'name': 'Alarm Config 1',
                        'defaultValue': '0000011110111111',
                        'record': 'true',
                        'type': '0g=='
                        },
                        {
                        'number': '260',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '261',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '262',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '263',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '264',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '265',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '266',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '267',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '268',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '269',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '270',
                        'name': 'DPI Data Rate',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '271',
                        'name': 'Drive Logic Rslt',
                        'defaultValue': '1111111111111111',
                        'record': 'false',
                        'type': '0g=='
                        },
                        {
                        'number': '272',
                        'name': 'Drive Ref Rslt',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'ww=='
                        },
                        {
                        'number': '273',
                        'name': 'Drive Ramp Rslt',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'ww=='
                        },
                        {
                        'number': '274',
                        'name': 'DPI Port Select',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '275',
                        'name': 'DPI Port Value',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'ww=='
                        },
                        {
                        'number': '276',
                        'name': 'Logic Mask',
                        'defaultValue': '0000000000101111',
                        'record': 'true',
                        'type': '0g=='
                        },
                        {
                        'number': '277',
                        'name': 'Start Mask',
                        'defaultValue': '0000000000101111',
                        'record': 'true',
                        'type': '0g=='
                        },
                        {
                        'number': '278',
                        'name': 'Jog Mask',
                        'defaultValue': '0000000000101111',
                        'record': 'true',
                        'type': '0g=='
                        },
                        {
                        'number': '279',
                        'name': 'Direction Mask',
                        'defaultValue': '0000000000101111',
                        'record': 'true',
                        'type': '0g=='
                        },
                        {
                        'number': '280',
                        'name': 'Reference Mask',
                        'defaultValue': '0000000000101111',
                        'record': 'true',
                        'type': '0g=='
                        },
                        {
                        'number': '281',
                        'name': 'Accel Mask',
                        'defaultValue': '0000000000101111',
                        'record': 'true',
                        'type': '0g=='
                        },
                        {
                        'number': '282',
                        'name': 'Decel Mask',
                        'defaultValue': '0000000000101111',
                        'record': 'true',
                        'type': '0g=='
                        },
                        {
                        'number': '283',
                        'name': 'Fault Clr Mask',
                        'defaultValue': '0000000000101111',
                        'record': 'true',
                        'type': '0g=='
                        },
                        {
                        'number': '284',
                        'name': 'MOP Mask',
                        'defaultValue': '0000000000101111',
                        'record': 'true',
                        'type': '0g=='
                        },
                        {
                        'number': '285',
                        'name': 'Local Mask',
                        'defaultValue': '0000000000101111',
                        'record': 'true',
                        'type': '0g=='
                        },
                        {
                        'number': '286',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '287',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '288',
                        'name': 'Stop Owner',
                        'defaultValue': '0000000000000000',
                        'record': 'false',
                        'type': '0g=='
                        },
                        {
                        'number': '289',
                        'name': 'Start Owner',
                        'defaultValue': '0000000000000000',
                        'record': 'false',
                        'type': '0g=='
                        },
                        {
                        'number': '290',
                        'name': 'Jog Owner',
                        'defaultValue': '0000000000000000',
                        'record': 'false',
                        'type': '0g=='
                        },
                        {
                        'number': '291',
                        'name': 'Direction Owner',
                        'defaultValue': '0000000000000000',
                        'record': 'false',
                        'type': '0g=='
                        },
                        {
                        'number': '292',
                        'name': 'Reference Owner',
                        'defaultValue': '0000000000000000',
                        'record': 'false',
                        'type': '0g=='
                        },
                        {
                        'number': '293',
                        'name': 'Accel Owner',
                        'defaultValue': '0000000000000000',
                        'record': 'false',
                        'type': '0g=='
                        },
                        {
                        'number': '294',
                        'name': 'Decel Owner',
                        'defaultValue': '0000000000000000',
                        'record': 'false',
                        'type': '0g=='
                        },
                        {
                        'number': '295',
                        'name': 'Fault Clr Owner',
                        'defaultValue': '0000000000000000',
                        'record': 'false',
                        'type': '0g=='
                        },
                        {
                        'number': '296',
                        'name': 'MOP Owner',
                        'defaultValue': '0000000000000000',
                        'record': 'false',
                        'type': '0g=='
                        },
                        {
                        'number': '297',
                        'name': 'Local Owner',
                        'defaultValue': '0000000000000000',
                        'record': 'false',
                        'type': '0g=='
                        },
                        {
                        'number': '298',
                        'name': 'DPI Ref Select',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '299',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '300',
                        'name': 'Data In A1',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '301',
                        'name': 'Data In A2',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '302',
                        'name': 'Data In B1',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '303',
                        'name': 'Data In B2',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '304',
                        'name': 'Data In C1',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '305',
                        'name': 'Data In C2',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '306',
                        'name': 'Data In D1',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '307',
                        'name': 'Data In D2',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '308',
                        'name': 'HighRes Ref',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xA=='
                        },
                        {
                        'number': '309',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '310',
                        'name': 'Data Out A1',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '311',
                        'name': 'Data Out A2',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '312',
                        'name': 'Data Out B1',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '313',
                        'name': 'Data Out B2',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '314',
                        'name': 'Data Out C1',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '315',
                        'name': 'Data Out C2',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '316',
                        'name': 'Data Out D1',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '317',
                        'name': 'Data Out D2',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '318',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '319',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '320',
                        'name': 'Anlg In Config',
                        'defaultValue': '0000000000000000',
                        'record': 'true',
                        'type': '0g=='
                        },
                        {
                        'number': '321',
                        'name': 'Anlg In Sqr Root',
                        'defaultValue': '0000000000000000',
                        'record': 'true',
                        'type': '0g=='
                        },
                        {
                        'number': '322',
                        'name': 'Analog In 1 Hi',
                        'defaultValue': '10000',
                        'record': 'true',
                        'type': 'ww=='
                        },
                        {
                        'number': '323',
                        'name': 'Analog In 1 Lo',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'ww=='
                        },
                        {
                        'number': '324',
                        'name': 'Analog In 1 Loss',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '325',
                        'name': 'Analog In 2 Hi',
                        'defaultValue': '10000',
                        'record': 'true',
                        'type': 'ww=='
                        },
                        {
                        'number': '326',
                        'name': 'Analog In 2 Lo',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'ww=='
                        },
                        {
                        'number': '327',
                        'name': 'Analog In 2 Loss',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '328',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '329',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '330',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '331',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '332',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '333',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '334',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '335',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '336',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '337',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '338',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '339',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '340',
                        'name': 'Anlg Out Config',
                        'defaultValue': '0000000000000000',
                        'record': 'true',
                        'type': '0g=='
                        },
                        {
                        'number': '341',
                        'name': 'Anlg Out Absolut',
                        'defaultValue': '0000000000000001',
                        'record': 'true',
                        'type': '0g=='
                        },
                        {
                        'number': '342',
                        'name': 'Analog Out1 Sel',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '343',
                        'name': 'Analog Out1 Hi',
                        'defaultValue': '1000',
                        'record': 'true',
                        'type': 'ww=='
                        },
                        {
                        'number': '344',
                        'name': 'Analog Out1 Lo',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'ww=='
                        },
                        {
                        'number': '345',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '346',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '347',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '348',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '349',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '350',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '351',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '352',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '353',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '354',
                        'name': 'Anlg Out1 Scale',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '355',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '356',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '357',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '358',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '359',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '360',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '361',
                        'name': 'Digital In1 Sel',
                        'defaultValue': '4',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '362',
                        'name': 'Digital In2 Sel',
                        'defaultValue': '5',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '363',
                        'name': 'Digital In3 Sel',
                        'defaultValue': '18',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '364',
                        'name': 'Digital In4 Sel',
                        'defaultValue': '15',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '365',
                        'name': 'Digital In5 Sel',
                        'defaultValue': '16',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '366',
                        'name': 'Digital In6 Sel',
                        'defaultValue': '17',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '367',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '368',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '369',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '370',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '371',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '372',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '373',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '374',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '375',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '376',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '377',
                        'name': 'Anlg Out1 Setpt',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'ww=='
                        },
                        {
                        'number': '378',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '379',
                        'name': 'Dig Out Setpt',
                        'defaultValue': '0000000000000000',
                        'record': 'true',
                        'type': '0g=='
                        },
                        {
                        'number': '380',
                        'name': 'Digital Out1 Sel',
                        'defaultValue': '1',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '381',
                        'name': 'Dig Out1 Level',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '382',
                        'name': 'Dig Out1 OnTime',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '383',
                        'name': 'Dig Out1 OffTime',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '384',
                        'name': 'Digital Out2 Sel',
                        'defaultValue': '4',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '385',
                        'name': 'Dig Out2 Level',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '386',
                        'name': 'Dig Out2 OnTime',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '387',
                        'name': 'Dig Out2 OffTime',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '388',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '389',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '390',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '391',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '392',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '393',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '394',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '395',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '396',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '397',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '398',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '399',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '400',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '401',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '402',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '403',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '404',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '405',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '406',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '407',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '408',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '409',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '410',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '411',
                        'name': 'DigIn DataLogic',
                        'defaultValue': '0000000000111111',
                        'record': 'true',
                        'type': '0g=='
                        },
                        {
                        'number': '412',
                        'name': 'Motor Fdbk Type',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '413',
                        'name': 'Encoder PPR',
                        'defaultValue': '1024',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '414',
                        'name': 'Enc Pos Feedback',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xA=='
                        },
                        {
                        'number': '415',
                        'name': 'Encoder Speed',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'ww=='
                        },
                        {
                        'number': '416',
                        'name': 'Fdbk Filter Sel',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '417',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '418',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '419',
                        'name': 'Notch FilterFreq',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '420',
                        'name': 'Notch Filter K',
                        'defaultValue': '3',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '421',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '422',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '423',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '424',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '425',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '426',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '427',
                        'name': 'Torque Ref A Sel',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '428',
                        'name': 'Torque Ref A Hi',
                        'defaultValue': '1000',
                        'record': 'true',
                        'type': 'ww=='
                        },
                        {
                        'number': '429',
                        'name': 'Torque Ref A Lo',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'ww=='
                        },
                        {
                        'number': '430',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '431',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '432',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '433',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '434',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '435',
                        'name': 'Torque Setpoint1',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'ww=='
                        },
                        {
                        'number': '436',
                        'name': 'Pos Torque Limit',
                        'defaultValue': '2000',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '437',
                        'name': 'Neg Torque Limit',
                        'defaultValue': '-2000',
                        'record': 'true',
                        'type': 'ww=='
                        },
                        {
                        'number': '438',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '439',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '440',
                        'name': 'Control Status',
                        'defaultValue': '00000000000000000000000000000000',
                        'record': 'false',
                        'type': '0w=='
                        },
                        {
                        'number': '441',
                        'name': 'Torq Current Ref',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'ww=='
                        },
                        {
                        'number': '442',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '443',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '444',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '445',
                        'name': 'Ki Speed Loop',
                        'defaultValue': '78',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '446',
                        'name': 'Kp Speed Loop',
                        'defaultValue': '63',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '447',
                        'name': 'Kf Speed Loop',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '448',
                        'name': 'Spd Err Filt BW',
                        'defaultValue': '2000',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '449',
                        'name': 'Speed Desired BW',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '450',
                        'name': 'Total Inertia',
                        'defaultValue': '10',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '451',
                        'name': 'Speed Loop Meter',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'ww=='
                        },
                        {
                        'number': '452',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '453',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '454',
                        'name': 'Rev Speed Limit',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'ww=='
                        },
                        {
                        'number': '455',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '456',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '457',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '458',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '459',
                        'name': 'PI Deriv Time',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '460',
                        'name': 'PI Reference Hi',
                        'defaultValue': '1000',
                        'record': 'true',
                        'type': 'ww=='
                        },
                        {
                        'number': '461',
                        'name': 'PI Reference Lo',
                        'defaultValue': '-1000',
                        'record': 'true',
                        'type': 'ww=='
                        },
                        {
                        'number': '462',
                        'name': 'PI Feedback Hi',
                        'defaultValue': '1000',
                        'record': 'true',
                        'type': 'ww=='
                        },
                        {
                        'number': '463',
                        'name': 'PI Feedback Lo',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'ww=='
                        },
                        {
                        'number': '464',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '465',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '466',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '467',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '468',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '469',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '470',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '471',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '472',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '473',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '474',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '475',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '476',
                        'name': 'Scale1 In Value',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'ww=='
                        },
                        {
                        'number': '477',
                        'name': 'Scale1 In Hi',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'ww=='
                        },
                        {
                        'number': '478',
                        'name': 'Scale1 In Lo',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'ww=='
                        },
                        {
                        'number': '479',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '480',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '481',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '482',
                        'name': 'Scale2 In Value',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'ww=='
                        },
                        {
                        'number': '483',
                        'name': 'Scale2 In Hi',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'ww=='
                        },
                        {
                        'number': '484',
                        'name': 'Scale2 In Lo',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'ww=='
                        },
                        {
                        'number': '485',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '486',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '487',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '488',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '489',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '490',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '491',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '492',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '493',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '494',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '495',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '496',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '497',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '498',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '499',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '500',
                        'name': 'Ki Current Limit',
                        'defaultValue': '1500',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '501',
                        'name': 'Kd Current Limit',
                        'defaultValue': '500',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '502',
                        'name': 'Bus Reg ACR Kp',
                        'defaultValue': '450',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '503',
                        'name': 'Jerk',
                        'defaultValue': '900',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '504',
                        'name': 'Kp LL Bus Reg',
                        'defaultValue': '500',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '505',
                        'name': 'Kd LL Bus Reg',
                        'defaultValue': '500',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '506',
                        'name': 'Angl Stblty Gain',
                        'defaultValue': '51',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '507',
                        'name': 'Volt Stblty Gain',
                        'defaultValue': '93',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '508',
                        'name': 'Stability Filter',
                        'defaultValue': '3250',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '509',
                        'name': 'Lo Freq Reg KpId',
                        'defaultValue': '64',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '510',
                        'name': 'Lo Freq Reg KpIq',
                        'defaultValue': '64',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '511',
                        'name': 'Ki Cur Reg',
                        'defaultValue': '44',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '512',
                        'name': 'Kp Cur Reg',
                        'defaultValue': '1600',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '513',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '514',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '515',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '516',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '517',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '518',
                        'name': 'Host DAC Enable',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '519',
                        'name': 'DAC47-A',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '520',
                        'name': 'DAC47-B',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '521',
                        'name': 'DAC47-C',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '522',
                        'name': 'DAC47-D',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '523',
                        'name': 'Bus Utilization',
                        'defaultValue': '950',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '524',
                        'name': 'PWM Type Select',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '525',
                        'name': 'Torque Adapt Spd',
                        'defaultValue': '100',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '526',
                        'name': 'Torq Reg Enable',
                        'defaultValue': '1',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '527',
                        'name': 'Kp Torque Reg',
                        'defaultValue': '32',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '528',
                        'name': 'Ki Torque Reg',
                        'defaultValue': '128',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '529',
                        'name': 'Torque Reg Trim',
                        'defaultValue': '100',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '530',
                        'name': 'Slip Reg Enable',
                        'defaultValue': '1',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '531',
                        'name': 'Kp Slip Reg',
                        'defaultValue': '256',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '532',
                        'name': 'Ki Slip Reg',
                        'defaultValue': '64',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '533',
                        'name': 'Flux Reg Enable',
                        'defaultValue': '1',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '534',
                        'name': 'Kp Flux Reg',
                        'defaultValue': '64',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '535',
                        'name': 'Ki Flux Reg',
                        'defaultValue': '32',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '536',
                        'name': 'Ki Flux Braking',
                        'defaultValue': '100',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '537',
                        'name': 'Kp Flux Braking',
                        'defaultValue': '500',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '538',
                        'name': 'Rec Delay Time',
                        'defaultValue': '1000',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '539',
                        'name': 'Freq Reg Kp',
                        'defaultValue': '450',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '540',
                        'name': 'Freq Reg Ki',
                        'defaultValue': '2000',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '541',
                        'name': 'Encdlss Ang Comp',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'ww=='
                        },
                        {
                        'number': '542',
                        'name': 'Encdlss Vlt Comp',
                        'defaultValue': '128',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '543',
                        'name': 'Excitation KI',
                        'defaultValue': '44',
                        'record': 'true',
                        'type': 'ww=='
                        },
                        {
                        'number': '544',
                        'name': 'Excitation KP',
                        'defaultValue': '1800',
                        'record': 'true',
                        'type': 'ww=='
                        },
                        {
                        'number': '545',
                        'name': 'In Phase LossLvl',
                        'defaultValue': '325',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '546',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '547',
                        'name': 'Ki Fast Braking',
                        'defaultValue': '1000',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '548',
                        'name': 'Kp Fast Braking',
                        'defaultValue': '2000',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '549',
                        'name': 'Flux Braking %',
                        'defaultValue': '125',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '550',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '551',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '552',
                        'name': 'Dead Time Comp',
                        'defaultValue': '75',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '553',
                        'name': 'Flux Down Rate',
                        'defaultValue': '100',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '554',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '555',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '556',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '557',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '558',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '559',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '560',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '561',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '562',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '563',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '564',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '565',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '566',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '567',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '568',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '569',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '570',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '571',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '572',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '573',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '574',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '575',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '576',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '577',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '578',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '579',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '580',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '581',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '582',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '583',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '584',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '585',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '586',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '587',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '588',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '589',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '590',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '591',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '592',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '593',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '594',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '595',
                        'name': 'Port Mask Act',
                        'defaultValue': '0000000000000000',
                        'record': 'false',
                        'type': '0g=='
                        },
                        {
                        'number': '596',
                        'name': 'Write Mask Cfg',
                        'defaultValue': '0000000000101110',
                        'record': 'true',
                        'type': '0g=='
                        },
                        {
                        'number': '597',
                        'name': 'Write Mask Act',
                        'defaultValue': '0000000000000000',
                        'record': 'false',
                        'type': '0g=='
                        },
                        {
                        'number': '598',
                        'name': 'Logic Mask Act',
                        'defaultValue': '0000000000000000',
                        'record': 'false',
                        'type': '0g=='
                        },
                        {
                        'number': '599',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '600',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '601',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '602',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '603',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '604',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '605',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '606',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '607',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '608',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '609',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '610',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '611',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '612',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '613',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '614',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '615',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '616',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '617',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '618',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '619',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xw=='
                        },
                        {
                        'number': '620',
                        'name': 'Fiber Control',
                        'defaultValue': '0000000000000000',
                        'record': 'true',
                        'type': '0g=='
                        },
                        {
                        'number': '621',
                        'name': 'Fiber Status',
                        'defaultValue': '0000000000000000',
                        'record': 'false',
                        'type': '0g=='
                        },
                        {
                        'number': '622',
                        'name': 'Sync Time',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '623',
                        'name': 'Traverse Inc',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '624',
                        'name': 'Traverse Dec',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '625',
                        'name': 'Max Traverse',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '626',
                        'name': 'P Jump',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '627',
                        'name': 'DPI Port',
                        'defaultValue': '5',
                        'record': 'false',
                        'type': 'xg=='
                        },
                        {
                        'number': '628',
                        'name': 'DPI Data Rate',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xg=='
                        }
                        ]";

        // 20-COMM-E (Ethernet) comm adapter parameters (629+)
        // Override this in subclasses for different comm adapters (e.g., 20-COMM-D for DeviceNet)
        protected virtual string commAdapterParameterListJSON => @"[
                        {
                        'number': '629',
                        'name': 'BOOTP',
                        'defaultValue': '1',
                        'record': 'true',
                        'type': 'xg=='
                        },
                        {
                        'number': '630',
                        'name': 'IP Addr Cfg 1',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xg=='
                        },
                        {
                        'number': '631',
                        'name': 'IP Addr Cfg 2',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xg=='
                        },
                        {
                        'number': '632',
                        'name': 'IP Addr Cfg 3',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xg=='
                        },
                        {
                        'number': '633',
                        'name': 'IP Addr Cfg 4',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xg=='
                        },
                        {
                        'number': '634',
                        'name': 'Subnet Cfg 1',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xg=='
                        },
                        {
                        'number': '635',
                        'name': 'Subnet Cfg 2',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xg=='
                        },
                        {
                        'number': '636',
                        'name': 'Subnet Cfg 3',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xg=='
                        },
                        {
                        'number': '637',
                        'name': 'Subnet Cfg 4',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xg=='
                        },
                        {
                        'number': '638',
                        'name': 'Gateway Cfg 1',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xg=='
                        },
                        {
                        'number': '639',
                        'name': 'Gateway Cfg 2',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xg=='
                        },
                        {
                        'number': '640',
                        'name': 'Gateway Cfg 3',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xg=='
                        },
                        {
                        'number': '641',
                        'name': 'Gateway Cfg 4',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xg=='
                        },
                        {
                        'number': '642',
                        'name': 'EN Rate Cfg',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xg=='
                        },
                        {
                        'number': '643',
                        'name': 'EN Rate Act',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xg=='
                        },
                        {
                        'number': '644',
                        'name': 'Ref \/ Fdbk Size',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xg=='
                        },
                        {
                        'number': '645',
                        'name': 'Datalink Size',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xg=='
                        },
                        {
                        'number': '646',
                        'name': 'Reset Module',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xg=='
                        },
                        {
                        'number': '647',
                        'name': 'Comm Flt Action',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xg=='
                        },
                        {
                        'number': '648',
                        'name': 'Idle Flt Action',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xg=='
                        },
                        {
                        'number': '649',
                        'name': 'DPI I\/O Cfg',
                        'defaultValue': '00000001',
                        'record': 'true',
                        'type': '0Q=='
                        },
                        {
                        'number': '650',
                        'name': 'DPI I\/O Act',
                        'defaultValue': '00000000',
                        'record': 'false',
                        'type': '0Q=='
                        },
                        {
                        'number': '651',
                        'name': 'Flt Cfg Logic',
                        'defaultValue': '0000000000000000',
                        'record': 'true',
                        'type': '0g=='
                        },
                        {
                        'number': '652',
                        'name': 'Flt Cfg Ref',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'yA=='
                        },
                        {
                        'number': '653',
                        'name': 'Flt Cfg A1 In',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'yA=='
                        },
                        {
                        'number': '654',
                        'name': 'Flt Cfg A2 In',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'yA=='
                        },
                        {
                        'number': '655',
                        'name': 'Flt Cfg B1 In',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'yA=='
                        },
                        {
                        'number': '656',
                        'name': 'Flt Cfg B2 In',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'yA=='
                        },
                        {
                        'number': '657',
                        'name': 'Flt Cfg C1 In',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'yA=='
                        },
                        {
                        'number': '658',
                        'name': 'Flt Cfg C2 In',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'yA=='
                        },
                        {
                        'number': '659',
                        'name': 'Flt Cfg D1 In',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'yA=='
                        },
                        {
                        'number': '660',
                        'name': 'Flt Cfg D2 In',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'yA=='
                        },
                        {
                        'number': '661',
                        'name': 'M-S Input',
                        'defaultValue': '00000001',
                        'record': 'true',
                        'type': '0Q=='
                        },
                        {
                        'number': '662',
                        'name': 'M-S Output',
                        'defaultValue': '00000001',
                        'record': 'true',
                        'type': '0Q=='
                        },
                        {
                        'number': '663',
                        'name': 'Ref Adjust',
                        'defaultValue': '10000',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '664',
                        'name': 'Peer A Input',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xg=='
                        },
                        {
                        'number': '665',
                        'name': 'Peer B Input',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xg=='
                        },
                        {
                        'number': '666',
                        'name': 'Peer Cmd Mask',
                        'defaultValue': '0000000000000000',
                        'record': 'true',
                        'type': '0g=='
                        },
                        {
                        'number': '667',
                        'name': 'Peer Flt Action',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xg=='
                        },
                        {
                        'number': '668',
                        'name': 'Peer Inp Addr 1',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xg=='
                        },
                        {
                        'number': '669',
                        'name': 'Peer Inp Addr 2',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xg=='
                        },
                        {
                        'number': '670',
                        'name': 'Peer Inp Addr 3',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xg=='
                        },
                        {
                        'number': '671',
                        'name': 'Peer Inp Addr 4',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xg=='
                        },
                        {
                        'number': '672',
                        'name': 'Peer Inp Timeout',
                        'defaultValue': '1000',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '673',
                        'name': 'Peer Inp Enable',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xg=='
                        },
                        {
                        'number': '674',
                        'name': 'Peer Inp Status',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xg=='
                        },
                        {
                        'number': '675',
                        'name': 'Peer A Output',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xg=='
                        },
                        {
                        'number': '676',
                        'name': 'Peer B Output',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xg=='
                        },
                        {
                        'number': '677',
                        'name': 'Peer Out Enable',
                        'defaultValue': '0',
                        'record': 'true',
                        'type': 'xg=='
                        },
                        {
                        'number': '678',
                        'name': 'Peer Out Time',
                        'defaultValue': '1000',
                        'record': 'true',
                        'type': 'xw=='
                        },
                        {
                        'number': '679',
                        'name': 'Peer Out Skip',
                        'defaultValue': '1',
                        'record': 'true',
                        'type': 'xg=='
                        },
                        {
                        'number': '680',
                        'name': 'Reserved',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xg=='
                        },
                        {
                        'number': '681',
                        'name': 'Web Enable',
                        'defaultValue': '0',
                        'record': 'false',
                        'type': 'xg=='
                        },
                        {
                        'number': '682',
                        'name': 'Web Features',
                        'defaultValue': '00000001',
                        'record': 'true',
                        'type': '0Q=='
                        }
                        ]";
    }
}