using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using powerFlexBackup.cipdevice.deviceParameterObjects;

namespace powerFlexBackup.cipdevice
{
    [SupportedDevice("PowerFlex 40 Series", 127,  40, true)]
    public class CIPDevice_PowerFlex40 : CIPDevice{

        // Scattered read configuration
        private const int ScatteredReadBatchSize = 60;
        private const byte ScatteredReadServiceCode = 0x32;
        private const int ScatteredReadClassID = 0x93;
        private const int ScatteredReadInstanceID = 0;

        public CIPDevice_PowerFlex40(String deviceAddress, Sres.Net.EEIP.EEIPClient eeipClient, byte[] CIPRoute, IOptions<AppConfiguration> options, ILogger logger) :
            base(deviceAddress, eeipClient, CIPRoute, options, logger)
        {
            setParameterClassID(0x0F);
            setInstanceAttributes();
            setDeviceParameterList(JsonConvert.DeserializeObject<List<DeviceParameter>>(parameterListJSON)!);
        }

        /// <summary>
        /// Builds scattered read request for PowerFlex 40.
        /// Format: 4 bytes per param (UINT16 param_number + UINT16 pad)
        /// </summary>
        private byte[] BuildScatteredReadRequest(List<int> parameterNumbers)
        {
            byte[] requestData = new byte[parameterNumbers.Count * 4];
            int offset = 0;

            foreach (int paramNum in parameterNumbers)
            {
                requestData[offset++] = (byte)(paramNum & 0xFF);
                requestData[offset++] = (byte)((paramNum >> 8) & 0xFF);
                requestData[offset++] = 0;
                requestData[offset++] = 0;
            }

            return requestData;
        }

        /// <summary>
        /// Parses scattered read response for PowerFlex 40.
        /// Format: 4 bytes per param (UINT16 param_number + UINT16 value)
        /// High bit on param_number indicates error (still records value)
        /// </summary>
        private Dictionary<int, byte[]> ParseScatteredReadResponse(byte[] response, List<int> parameterNumbers)
        {
            var results = new Dictionary<int, byte[]>();
            int offset = 0;

            foreach (int expectedParamNum in parameterNumbers)
            {
                if (offset + 4 > response.Length)
                {
                    logger.LogWarning("Response too short for parameter {0}", expectedParamNum);
                    break;
                }

                // Use short to detect high bit (error flag)
                short responseParamNum = (short)(response[offset] | (response[offset + 1] << 8));
                offset += 2;

                int actualParamNum = responseParamNum;

                // Check for error (high bit set = negative param number)
                if (responseParamNum < 0)
                {
                    actualParamNum = responseParamNum & 0x7FFF;
                    logger.LogWarning("Parameter {0} returned error in scattered read", actualParamNum);
                }
                else if (responseParamNum != expectedParamNum)
                {
                    logger.LogWarning("Parameter number mismatch: expected {0}, got {1}",
                        expectedParamNum, responseParamNum);
                }

                byte[] value = new byte[2];
                value[0] = response[offset++];
                value[1] = response[offset++];

                results[actualParamNum] = value;
            }

            return results;
        }

        // Comm adapter parameters start at 169 - need separate batch
        private const int CommAdapterParamStart = 169;

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

            // Process drive parameters
            if(driveParams.Count > 0)
            {
                logger.LogInformation("Reading {0} drive parameters using scattered read (batches of {1})",
                    driveParams.Count, ScatteredReadBatchSize);
                ProcessScatteredReadBatches(driveParams, paramsByNumber);
            }

            // Process comm adapter parameters individually (scattered read not supported)
            // TODO: Make configurable for different adapter types (Ethernet, DeviceNet, ControlNet)
            // we need to understand how to parse the comm adaptor from the route to do this.
            if(commAdapterParams.Count > 0)
            {
                logger.LogInformation("Reading {0} comm adapter parameters using individual reads",
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

                    foreach(var kvp in batchResults)
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
                catch (Exception ex)
                {
                    logger.LogWarning("Scattered read failed for batch {0}-{1}: {2}. Falling back to individual reads.",
                        batch[0], batch[batch.Count-1], ex.Message);

                    foreach (int paramNum in batch)
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
                catch (Exception ex)
                {
                    logger.LogWarning("Failed to read parameter {paramNum}: {message}", paramNum, ex.Message);
                }
            }
        }

        public override void getAllDeviceParameters(){
            getAllDeviceParametersCIPStandardCompliant();
        }

        public override int readDeviceParameterMaxNumber(){
            return readDeviceParameterMaxNumberCIPStandardCompliant();
        }

        public string parameterListJSON = @"[{
        'number': '1',
        'name': 'Output Freq',
        'defaultValue': '0',
        'record': 'false',
        'type': 'xw=='
    },
    {
        'number': '2',
        'name': 'Commanded Freq',
        'defaultValue': '0',
        'record': 'false',
        'type': 'xw=='
    },
    {
        'number': '3',
        'name': 'Output Current',
        'defaultValue': '0',
        'record': 'false',
        'type': 'xw=='
    },
    {
        'number': '4',
        'name': 'Output Voltage',
        'defaultValue': '0',
        'record': 'false',
        'type': 'xw=='
    },
    {
        'number': '5',
        'name': 'DC Bus Voltage',
        'defaultValue': '0',
        'record': 'false',
        'type': 'xw=='
    },
    {
        'number': '6',
        'name': 'Drive Status',
        'defaultValue': '0000000000000000',
        'record': 'false',
        'type': '0g=='
    },
    {
        'number': '7',
        'name': 'Fault 1 Code',
        'defaultValue': '0',
        'record': 'false',
        'type': 'xw=='
    },
    {
        'number': '8',
        'name': 'Fault 2 Code',
        'defaultValue': '0',
        'record': 'false',
        'type': 'xw=='
    },
    {
        'number': '9',
        'name': 'Fault 3 Code',
        'defaultValue': '0',
        'record': 'false',
        'type': 'xw=='
    },
    {
        'number': '10',
        'name': 'Process Display',
        'defaultValue': '0',
        'record': 'false',
        'type': 'xw=='
    },
    {
        'number': '11',
        'name': 'Process Fract',
        'defaultValue': '0',
        'record': 'false',
        'type': 'xw=='
    },
    {
        'number': '12',
        'name': 'Control Source',
        'defaultValue': '0',
        'record': 'false',
        'type': 'xw=='
    },
    {
        'number': '13',
        'name': 'Contrl In Status',
        'defaultValue': '0000000000000000',
        'record': 'false',
        'type': '0g=='
    },
    {
        'number': '14',
        'name': 'Dig In Status',
        'defaultValue': '0000000000000000',
        'record': 'false',
        'type': '0g=='
    },
    {
        'number': '15',
        'name': 'Comm Status',
        'defaultValue': '0000000000000000',
        'record': 'false',
        'type': '0g=='
    },
    {
        'number': '16',
        'name': 'Control SW Ver',
        'defaultValue': '0',
        'record': 'false',
        'type': 'xw=='
    },
    {
        'number': '17',
        'name': 'Drive Type',
        'defaultValue': '0',
        'record': 'false',
        'type': 'xw=='
    },
    {
        'number': '18',
        'name': 'Elapsed Run Time',
        'defaultValue': '0',
        'record': 'false',
        'type': 'xw=='
    },
    {
        'number': '19',
        'name': 'Testpoint Data',
        'defaultValue': '0000000000000000',
        'record': 'false',
        'type': '0g=='
    },
    {
        'number': '20',
        'name': 'Analog In 0-10V',
        'defaultValue': '0',
        'record': 'false',
        'type': 'xw=='
    },
    {
        'number': '21',
        'name': 'Analog In 4-20mA',
        'defaultValue': '0',
        'record': 'false',
        'type': 'xw=='
    },
    {
        'number': '22',
        'name': 'Output Power',
        'defaultValue': '0',
        'record': 'false',
        'type': 'xw=='
    },
    {
        'number': '23',
        'name': 'Output Powr Fctr',
        'defaultValue': '0',
        'record': 'false',
        'type': 'xw=='
    },
    {
        'number': '24',
        'name': 'Drive Temp',
        'defaultValue': '0',
        'record': 'false',
        'type': 'xw=='
    },
    {
        'number': '25',
        'name': 'Counter Status',
        'defaultValue': '0',
        'record': 'false',
        'type': 'xw=='
    },
    {
        'number': '26',
        'name': 'Timer Status',
        'defaultValue': '0',
        'record': 'false',
        'type': 'xw=='
    },
    {
        'number': '27',
        'name': 'Timer Stat Fract',
        'defaultValue': '0',
        'record': 'false',
        'type': 'xw=='
    },
    {
        'number': '28',
        'name': 'Stp Logic Status',
        'defaultValue': '0',
        'record': 'false',
        'type': 'xw=='
    },
    {
        'number': '29',
        'name': 'Torque Current',
        'defaultValue': '0',
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
        'name': 'Motor NP Volts',
        'defaultValue': '460',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '32',
        'name': 'Motor NP Hertz',
        'defaultValue': '60',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '33',
        'name': 'Motor OL Current',
        'defaultValue': '105',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '34',
        'name': 'Minimum Freq',
        'defaultValue': '0',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '35',
        'name': 'Maximum Freq',
        'defaultValue': '60',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '36',
        'name': 'Start Source',
        'defaultValue': '0',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '37',
        'name': 'Stop Mode',
        'defaultValue': '0',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '38',
        'name': 'Speed Reference',
        'defaultValue': '0',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '39',
        'name': 'Accel Time 1',
        'defaultValue': '100',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '40',
        'name': 'Decel Time 1',
        'defaultValue': '100',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '41',
        'name': 'Reset To Defalts',
        'defaultValue': '0',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '42',
        'name': 'Reserved',
        'defaultValue': '0',
        'record': 'false',
        'type': 'xw=='
    },
    {
        'number': '43',
        'name': 'Motor OL Ret',
        'defaultValue': '0',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '44',
        'name': 'Reserved',
        'defaultValue': '0',
        'record': 'false',
        'type': 'xw=='
    },
    {
        'number': '45',
        'name': 'Reserved',
        'defaultValue': '0',
        'record': 'false',
        'type': 'xw=='
    },
    {
        'number': '46',
        'name': 'Reserved',
        'defaultValue': '0',
        'record': 'false',
        'type': 'xw=='
    },
    {
        'number': '47',
        'name': 'Reserved',
        'defaultValue': '0',
        'record': 'false',
        'type': 'xw=='
    },
    {
        'number': '48',
        'name': 'Reserved',
        'defaultValue': '0',
        'record': 'false',
        'type': 'xw=='
    },
    {
        'number': '49',
        'name': 'Reserved',
        'defaultValue': '0',
        'record': 'false',
        'type': 'xw=='
    },
    {
        'number': '50',
        'name': 'Reserved',
        'defaultValue': '0',
        'record': 'false',
        'type': 'xw=='
    },
    {
        'number': '51',
        'name': 'Digital In1 Sel',
        'defaultValue': '4',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '52',
        'name': 'Digital In2 Sel',
        'defaultValue': '4',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '53',
        'name': 'Digital In3 Sel',
        'defaultValue': '5',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '54',
        'name': 'Digital In4 Sel',
        'defaultValue': '11',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '55',
        'name': 'Relay Out Sel',
        'defaultValue': '0',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '56',
        'name': 'Relay Out Level',
        'defaultValue': '0',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '57',
        'name': 'Relay Out LevelF',
        'defaultValue': '0',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '58',
        'name': 'Opto Out1 Sel',
        'defaultValue': '2',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '59',
        'name': 'Opto Out1 Level',
        'defaultValue': '0',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '60',
        'name': 'Opto Out1 LevelF',
        'defaultValue': '0',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '61',
        'name': 'Opto Out2 Sel',
        'defaultValue': '1',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '62',
        'name': 'Opto Out2 Level',
        'defaultValue': '0',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '63',
        'name': 'Opto Out2 LevelF',
        'defaultValue': '0',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '64',
        'name': 'Opto Out Logic',
        'defaultValue': '0',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '65',
        'name': 'Analog Out Sel',
        'defaultValue': '0',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '66',
        'name': 'Analog Out High',
        'defaultValue': '100',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '67',
        'name': 'Accel Time 2',
        'defaultValue': '200',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '68',
        'name': 'Decel Time 2',
        'defaultValue': '200',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '69',
        'name': 'Internal Freq',
        'defaultValue': '600',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '70',
        'name': 'Preset Freq 0',
        'defaultValue': '0',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '71',
        'name': 'Preset Freq 1',
        'defaultValue': '50',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '72',
        'name': 'Preset Freq 2',
        'defaultValue': '100',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '73',
        'name': 'Preset Freq 3',
        'defaultValue': '200',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '74',
        'name': 'Preset Freq 4',
        'defaultValue': '300',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '75',
        'name': 'Preset Freq 5',
        'defaultValue': '400',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '76',
        'name': 'Preset Freq 6',
        'defaultValue': '500',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '77',
        'name': 'Preset Freq 7',
        'defaultValue': '600',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '78',
        'name': 'Jog Frequency',
        'defaultValue': '100',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '79',
        'name': 'Jog Accel/Decel',
        'defaultValue': '100',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '80',
        'name': 'DC Brake Time',
        'defaultValue': '0',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '81',
        'name': 'DC Brake Level',
        'defaultValue': '5',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '82',
        'name': 'DB Resistor Sel',
        'defaultValue': '0',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '83',
        'name': 'S Curve %',
        'defaultValue': '0',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '84',
        'name': 'Boost Select',
        'defaultValue': '7',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '85',
        'name': 'Start Boost',
        'defaultValue': '25',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '86',
        'name': 'Break Voltage',
        'defaultValue': '250',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '87',
        'name': 'Break Frequency',
        'defaultValue': '150',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '88',
        'name': 'Maximum Voltage',
        'defaultValue': '460',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '89',
        'name': 'Current Limit 1',
        'defaultValue': '158',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '90',
        'name': 'Motor OL Select',
        'defaultValue': '0',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '91',
        'name': 'PWM Frequency',
        'defaultValue': '40',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '92',
        'name': 'Auto Rstrt Tries',
        'defaultValue': '0',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '93',
        'name': 'Auto Rstrt Delay',
        'defaultValue': '10',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '94',
        'name': 'Start At PowerUp',
        'defaultValue': '0',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '95',
        'name': 'Reverse Disable',
        'defaultValue': '0',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '96',
        'name': 'Flying Start En',
        'defaultValue': '0',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '97',
        'name': 'Compensation',
        'defaultValue': '1',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '98',
        'name': 'SW Current Trip',
        'defaultValue': '0',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '99',
        'name': 'Process Factor',
        'defaultValue': '300',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '100',
        'name': 'Fault Clear',
        'defaultValue': '0',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '101',
        'name': 'Program Lock',
        'defaultValue': '0',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '102',
        'name': 'Testpoint Sel',
        'defaultValue': '0000010000000000',
        'record': 'true',
        'type': '0g=='
    },
    {
        'number': '103',
        'name': 'Comm Data Rate',
        'defaultValue': '3',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '104',
        'name': 'Comm Node Addr',
        'defaultValue': '100',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '105',
        'name': 'Comm Loss Action',
        'defaultValue': '0',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '106',
        'name': 'Comm Loss Time',
        'defaultValue': '50',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '107',
        'name': 'Comm Format',
        'defaultValue': '0',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '108',
        'name': 'Language',
        'defaultValue': '1',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '109',
        'name': 'Anlg Out Setpt',
        'defaultValue': '0',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '110',
        'name': 'Anlg In 0-10V Lo',
        'defaultValue': '0',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '111',
        'name': 'Anlg In 0-10V Hi',
        'defaultValue': '1000',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '112',
        'name': 'Anlg In4-20mA Lo',
        'defaultValue': '0',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '113',
        'name': 'Anlg In4-20mA Hi',
        'defaultValue': '1000',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '114',
        'name': 'Slip Hertz @ FLA',
        'defaultValue': '20',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '115',
        'name': 'Process Time Lo',
        'defaultValue': '0',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '116',
        'name': 'Process Time Hi',
        'defaultValue': '0',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '117',
        'name': 'Bus Reg Mode',
        'defaultValue': '1',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '118',
        'name': 'Current Limit 2',
        'defaultValue': '158',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '119',
        'name': 'Skip Frequency',
        'defaultValue': '0',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '120',
        'name': 'Skip Freq Band',
        'defaultValue': '0',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '121',
        'name': 'Stall Fault Time',
        'defaultValue': '0',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '122',
        'name': 'Analog In Loss',
        'defaultValue': '0',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '123',
        'name': '10V Bipolar Enbl',
        'defaultValue': '0',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '124',
        'name': 'Var PWM Disable',
        'defaultValue': '0',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '125',
        'name': 'Torque Perf Mode',
        'defaultValue': '1',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '126',
        'name': 'Motor NP FLA',
        'defaultValue': '64',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '127',
        'name': 'Autotune',
        'defaultValue': '0',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '128',
        'name': 'IR Voltage Drop',
        'defaultValue': '51',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '129',
        'name': 'Flux Current Ref',
        'defaultValue': '256',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '130',
        'name': 'PID Trim Hi',
        'defaultValue': '600',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '131',
        'name': 'PID Trim Lo',
        'defaultValue': '0',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '132',
        'name': 'PID Ref Sel',
        'defaultValue': '0',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '133',
        'name': 'PID Feedback Sel',
        'defaultValue': '0',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '134',
        'name': 'PID Prop Gain',
        'defaultValue': '1',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '135',
        'name': 'PID Integ Time',
        'defaultValue': '20',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '136',
        'name': 'PID Diff Rate',
        'defaultValue': '0',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '137',
        'name': 'PID Setpoint',
        'defaultValue': '0',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '138',
        'name': 'PID Deadband',
        'defaultValue': '0',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '139',
        'name': 'PID Preload',
        'defaultValue': '0',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '140',
        'name': 'Stp Logic 0',
        'defaultValue': '0000000011110001',
        'record': 'true',
        'type': '0g=='
    },
    {
        'number': '141',
        'name': 'Stp Logic 1',
        'defaultValue': '0000000011110001',
        'record': 'true',
        'type': '0g=='
    },
    {
        'number': '142',
        'name': 'Stp Logic 2',
        'defaultValue': '0000000011110001',
        'record': 'true',
        'type': '0g=='
    },
    {
        'number': '143',
        'name': 'Stp Logic 3',
        'defaultValue': '0000000011110001',
        'record': 'true',
        'type': '0g=='
    },
    {
        'number': '144',
        'name': 'Stp Logic 4',
        'defaultValue': '0000000011110001',
        'record': 'true',
        'type': '0g=='
    },
    {
        'number': '145',
        'name': 'Stp Logic 5',
        'defaultValue': '0000000011110001',
        'record': 'true',
        'type': '0g=='
    },
    {
        'number': '146',
        'name': 'Stp Logic 6',
        'defaultValue': '0000000011110001',
        'record': 'true',
        'type': '0g=='
    },
    {
        'number': '147',
        'name': 'Stp Logic 7',
        'defaultValue': '0000000011110001',
        'record': 'true',
        'type': '0g=='
    },
    {
        'number': '148',
        'name': 'Reserved',
        'defaultValue': '0',
        'record': 'false',
        'type': 'xw=='
    },
    {
        'number': '149',
        'name': 'Reserved',
        'defaultValue': '0',
        'record': 'false',
        'type': 'xw=='
    },
    {
        'number': '150',
        'name': 'Stp Logic Time 0',
        'defaultValue': '300',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '151',
        'name': 'Stp Logic Time 1',
        'defaultValue': '300',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '152',
        'name': 'Stp Logic Time 2',
        'defaultValue': '300',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '153',
        'name': 'Stp Logic Time 3',
        'defaultValue': '300',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '154',
        'name': 'Stp Logic Time 4',
        'defaultValue': '300',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '155',
        'name': 'Stp Logic Time 5',
        'defaultValue': '300',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '156',
        'name': 'Stp Logic Time 6',
        'defaultValue': '300',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '157',
        'name': 'Stp Logic Time 7',
        'defaultValue': '300',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '158',
        'name': 'Reserved',
        'defaultValue': '0',
        'record': 'false',
        'type': 'xw=='
    },
    {
        'number': '159',
        'name': 'Reserved',
        'defaultValue': '0',
        'record': 'false',
        'type': 'xw=='
    },
    {
        'number': '160',
        'name': 'EM Brk Off Delay',
        'defaultValue': '200',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '161',
        'name': 'EM Brk On Delay',
        'defaultValue': '200',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '162',
        'name': 'MOP Reset Sel',
        'defaultValue': '1',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '163',
        'name': 'DB Threshold',
        'defaultValue': '1000',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '164',
        'name': 'Comm Write Mode',
        'defaultValue': '0',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '165',
        'name': 'Anlg Loss Delay',
        'defaultValue': '0',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '166',
        'name': 'Analog In Filter',
        'defaultValue': '0',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '167',
        'name': 'PID Invert Error',
        'defaultValue': '0',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '168',
        'name': 'Mode',
        'defaultValue': '0',
        'record': 'false',
        'type': 'xw=='
    },
    {
        'number': '169',
        'name': 'BOOTP',
        'defaultValue': '1',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '170',
        'name': 'IP Addr Cfg 1',
        'defaultValue': '0',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '171',
        'name': 'IP Addr Cfg 2',
        'defaultValue': '0',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '172',
        'name': 'IP Addr Cfg 3',
        'defaultValue': '0',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '173',
        'name': 'IP Addr Cfg 4',
        'defaultValue': '0',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '174',
        'name': 'Subnet Cfg 1',
        'defaultValue': '0',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '175',
        'name': 'Subnet Cfg 2',
        'defaultValue': '0',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '176',
        'name': 'Subnet Cfg 3',
        'defaultValue': '0',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '177',
        'name': 'Subnet Cfg 4',
        'defaultValue': '0',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '178',
        'name': 'Gateway Cfg 1',
        'defaultValue': '0',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '179',
        'name': 'Gateway Cfg 2',
        'defaultValue': '0',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '180',
        'name': 'Gateway Cfg 3',
        'defaultValue': '0',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '181',
        'name': 'Gateway Cfg 4',
        'defaultValue': '0',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '182',
        'name': 'EN Rate Cfg',
        'defaultValue': '0',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '183',
        'name': 'EN Rate Act',
        'defaultValue': '0',
        'record': 'false',
        'type': 'xw=='
    },
    {
        'number': '184',
        'name': 'Reset Module',
        'defaultValue': '0',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '185',
        'name': 'Comm Flt Action',
        'defaultValue': '0',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '186',
        'name': 'Idle Flt Action',
        'defaultValue': '0',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '187',
        'name': 'Flt Cfg Logic',
        'defaultValue': '0000000000000000',
        'record': 'true',
        'type': '0g=='
    },
    {
        'number': '188',
        'name': 'Flt Cfg Ref',
        'defaultValue': '0',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '189',
        'name': 'DSI I/O Cfg',
        'defaultValue': '0',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '190',
        'name': 'DSI I/O Act',
        'defaultValue': '0000000000000000',
        'record': 'false',
        'type': '0g=='
    },
    {
        'number': '191',
        'name': 'Drv 0 Addr',
        'defaultValue': '100',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '192',
        'name': 'Drv 1 Addr',
        'defaultValue': '101',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '193',
        'name': 'Drv 2 Addr',
        'defaultValue': '102',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '194',
        'name': 'Drv 3 Addr',
        'defaultValue': '103',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '195',
        'name': 'Drv 4 Addr',
        'defaultValue': '104',
        'record': 'true',
        'type': 'xw=='
    },
    {
        'number': '196',
        'name': 'Web Enable',
        'defaultValue': '0',
        'record': 'false',
        'type': 'xw=='
    },
    {
        'number': '197',
        'name': 'Web Features',
        'defaultValue': '0000000000000011',
        'record': 'true',
        'type': '0g=='
    }]";
    }
}