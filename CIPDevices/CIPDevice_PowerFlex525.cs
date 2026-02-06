using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using powerFlexBackup.cipdevice.deviceParameterObjects;

namespace powerFlexBackup.cipdevice
{

    [SupportedDevice("PowerFlex 520 Series", 150, 9, true)]
    [SupportedDevice("PowerFlex 520 Series", 151, 8, true)]
    [SupportedDevice("PowerFlex 520 Series", 151, 9, true)]
    public class CIPDevice_PowerFlex525 : CIPDevice{

        // Scattered read configuration
        private const int ScatteredReadBatchSize = 60;
        private const byte ScatteredReadServiceCode = 0x32;
        private const int ScatteredReadClassID = 0x93;
        private const int ScatteredReadInstanceID = 0;

        public CIPDevice_PowerFlex525(String deviceAddress, Sres.Net.EEIP.EEIPClient eeipClient, byte[] CIPRoute, IOptions<AppConfiguration> options, ILogger logger, IdentityObject identityObject) :
            base(deviceAddress, eeipClient, CIPRoute, options, logger, identityObject)
        {
            setParameterClassID(0x0F);
            setInstanceAttributes();
            setDeviceParameterList(JsonConvert.DeserializeObject<List<DeviceParameter>>(parameterListJSON)!);
        }

        /// <summary>
        /// Builds scattered read request for PowerFlex 525.
        /// Format: 4 bytes per param (UINT16 param_number + UINT16 pad)
        /// </summary>
        private byte[] BuildScatteredReadRequest(List<int> parameterNumbers)
        {
            byte[] requestData = new byte[parameterNumbers.Count * 4];
            int offset = 0;

            foreach (int paramNum in parameterNumbers)
            {
                // Parameter number (UINT16, little-endian)
                requestData[offset++] = (byte)(paramNum & 0xFF);
                requestData[offset++] = (byte)((paramNum >> 8) & 0xFF);

                // Pad (UINT16)
                requestData[offset++] = 0;
                requestData[offset++] = 0;
            }

            return requestData;
        }

        /// <summary>
        /// Parses scattered read response for PowerFlex 525.
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
                    logger.LogWarning("Response too short for parameter {0}. Expected {1} bytes, got {2}",
                        expectedParamNum, offset + 4, response.Length);
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

                // Read parameter value (UINT16 - 2 bytes)
                byte[] value = new byte[2];
                value[0] = response[offset++];
                value[1] = response[offset++];

                results[actualParamNum] = value;
            }

            return results;
        }

        public override void getDeviceParameterValues()
        {
            if(parameterObject[0].ParameterList == null)
                return;

            // Collect parameters that need to be read
            var paramsToRead = new List<int>();
            var paramsByNumber = new Dictionary<int, DeviceParameter>();

            foreach(DeviceParameter parameter in parameterObject[0].ParameterList)
            {
                if(parameter.record || config.OutputAllRecords)
                {
                    if(parameter.type is null)
                        parameter.type = readDeviceParameterType(parameter.number);

                    paramsToRead.Add(parameter.number);
                    paramsByNumber[parameter.number] = parameter;
                }
            }

            if(paramsToRead.Count == 0)
                return;

            logger.LogInformation("Reading {0} parameters using scattered read (batches of {1})",
                paramsToRead.Count, ScatteredReadBatchSize);

            // Process in batches
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

                    // Fall back to individual reads for this batch
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

        public override void getAllDeviceParameters(){
            getAllDeviceParametersCIPStandardCompliant();
        }

        public override int readDeviceParameterMaxNumber(){
            return readDeviceParameterMaxNumberCIPStandardCompliant();
        }

        public string parameterListJSON = @"[
            {
                'number': 1,
                'name': 'Output Freq',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 2,
                'name': 'Commanded Freq',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 3,
                'name': 'Output Current',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 4,
                'name': 'Output Voltage',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 5,
                'name': 'DC Bus Voltage',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 6,
                'name': 'Drive Status',
                'defaultValue': '0000000000000000',
                'record': false,
                'type': '0g=='
            },
            {
                'number': 7,
                'name': 'Fault 1 Code',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 8,
                'name': 'Fault 2 Code',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 9,
                'name': 'Fault 3 Code',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 10,
                'name': 'Process Display',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 11,
                'name': 'Process Fract',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 12,
                'name': 'Control Source',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 13,
                'name': 'Contrl In Status',
                'defaultValue': '0000000000000000',
                'record': false,
                'type': '0g=='
            },
            {
                'number': 14,
                'name': 'Dig In Status',
                'defaultValue': '0000000000000000',
                'record': false,
                'type': '0g=='
            },
            {
                'number': 15,
                'name': 'Output RPM',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 16,
                'name': 'Output Speed',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 17,
                'name': 'Output Power',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 18,
                'name': 'Power Saved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 19,
                'name': 'Elapsed Run Time',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 20,
                'name': 'Average Power',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 21,
                'name': 'Elapsed kWh',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 22,
                'name': 'Elapsed MWh',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 23,
                'name': 'Energy Saved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 24,
                'name': 'Accum kWh Sav',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 25,
                'name': 'Accum Cost Sav',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 26,
                'name': 'Accum CO2 Sav',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 27,
                'name': 'Drive Temp',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 28,
                'name': 'Control Temp',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 29,
                'name': 'Control SW Ver',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 30,
                'name': 'Language',
                'defaultValue': '1',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 31,
                'name': 'Motor NP Volts',
                'defaultValue': '460',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 32,
                'name': 'Motor NP Hertz',
                'defaultValue': '60',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 33,
                'name': 'Motor OL Current',
                'defaultValue': '130',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 34,
                'name': 'Motor NP FLA',
                'defaultValue': '101',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 35,
                'name': 'Motor NP Poles',
                'defaultValue': '4',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 36,
                'name': 'Motor NP RPM',
                'defaultValue': '1750',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 37,
                'name': 'Motor NP Power',
                'defaultValue': '550',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 38,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 39,
                'name': 'Torque Perf Mode',
                'defaultValue': '1',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 40,
                'name': 'Autotune',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 41,
                'name': 'Accel Time 1',
                'defaultValue': '1000',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 42,
                'name': 'Decel Time 1',
                'defaultValue': '1000',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 43,
                'name': 'Minimum Freq',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 44,
                'name': 'Maximum Freq',
                'defaultValue': '6000',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 45,
                'name': 'Stop Mode',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 46,
                'name': 'Start Source 1',
                'defaultValue': '1',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 47,
                'name': 'Speed Reference1',
                'defaultValue': '1',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 48,
                'name': 'Start Source 2',
                'defaultValue': '2',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 49,
                'name': 'Speed Reference2',
                'defaultValue': '5',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 50,
                'name': 'Start Source 3',
                'defaultValue': '5',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 51,
                'name': 'Speed Reference3',
                'defaultValue': '15',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 52,
                'name': 'Average kWh Cost',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 53,
                'name': 'Reset To Defalts',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 54,
                'name': 'Display Param',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 55,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 56,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 57,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 58,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 59,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 60,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 61,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 62,
                'name': 'DigIn TermBlk 02',
                'defaultValue': '48',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 63,
                'name': 'DigIn TermBlk 03',
                'defaultValue': '50',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 64,
                'name': '2-Wire Mode',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 65,
                'name': 'DigIn TermBlk 05',
                'defaultValue': '7',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 66,
                'name': 'DigIn TermBlk 06',
                'defaultValue': '7',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 67,
                'name': 'DigIn TermBlk 07',
                'defaultValue': '5',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 68,
                'name': 'DigIn TermBlk 08',
                'defaultValue': '9',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 69,
                'name': 'Opto Out1 Sel',
                'defaultValue': '2',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 70,
                'name': 'Opto Out1 Level',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 71,
                'name': 'Opto Out1 LevelF',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 72,
                'name': 'Opto Out2 Sel',
                'defaultValue': '1',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 73,
                'name': 'Opto Out2 Level',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 74,
                'name': 'Opto Out2 LevelF',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 75,
                'name': 'Opto Out Logic',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 76,
                'name': 'Relay Out1 Sel',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 77,
                'name': 'Relay Out1 Level',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 78,
                'name': 'RelayOut1 LevelF',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 79,
                'name': 'Relay 1 On Time',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 80,
                'name': 'Relay 1 Off Time',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 81,
                'name': 'Relay Out2 Sel',
                'defaultValue': '2',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 82,
                'name': 'Relay Out2 Level',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 83,
                'name': 'RelayOut2 LevelF',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 84,
                'name': 'Relay 2 On Time',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 85,
                'name': 'Relay 2 Off Time',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 86,
                'name': 'EM Brk Off Delay',
                'defaultValue': '200',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 87,
                'name': 'EM Brk On Delay',
                'defaultValue': '200',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 88,
                'name': 'Analog Out Sel',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 89,
                'name': 'Analog Out High',
                'defaultValue': '100',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 90,
                'name': 'Anlg Out Setpt',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 91,
                'name': 'Anlg In 0-10V Lo',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 92,
                'name': 'Anlg In 0-10V Hi',
                'defaultValue': '1000',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 93,
                'name': '10V Bipolar Enbl',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 94,
                'name': 'Anlg In V Loss',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 95,
                'name': 'Anlg In4-20mA Lo',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 96,
                'name': 'Anlg In4-20mA Hi',
                'defaultValue': '1000',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 97,
                'name': 'Anlg In mA Loss',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 98,
                'name': 'Anlg Loss Delay',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 99,
                'name': 'Analog In Filter',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 100,
                'name': 'Sleep-Wake Sel',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 101,
                'name': 'Sleep Level',
                'defaultValue': '100',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 102,
                'name': 'Sleep Time',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 103,
                'name': 'Wake Level',
                'defaultValue': '150',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 104,
                'name': 'Wake Time',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 105,
                'name': 'Safety Open En',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 106,
                'name': 'SafetyFlt RstCfg',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 107,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 108,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 109,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 110,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 111,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 112,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 113,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 114,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 115,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 116,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 117,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 118,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 119,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 120,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 121,
                'name': 'Comm Write Mode',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 122,
                'name': 'Cmd Stat Select',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 123,
                'name': 'RS485 Data Rate',
                'defaultValue': '3',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 124,
                'name': 'RS485 Node Addr',
                'defaultValue': '100',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 125,
                'name': 'Comm Loss Action',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 126,
                'name': 'Comm Loss Time',
                'defaultValue': '50',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 127,
                'name': 'RS485 Format',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 128,
                'name': 'EN Addr Sel',
                'defaultValue': '2',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 129,
                'name': 'EN IP Addr Cfg 1',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 130,
                'name': 'EN IP Addr Cfg 2',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 131,
                'name': 'EN IP Addr Cfg 3',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 132,
                'name': 'EN IP Addr Cfg 4',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 133,
                'name': 'EN Subnet Cfg 1',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 134,
                'name': 'EN Subnet Cfg 2',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 135,
                'name': 'EN Subnet Cfg 3',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 136,
                'name': 'EN Subnet Cfg 4',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 137,
                'name': 'EN Gateway Cfg 1',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 138,
                'name': 'EN Gateway Cfg 2',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 139,
                'name': 'EN Gateway Cfg 3',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 140,
                'name': 'EN Gateway Cfg 4',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 141,
                'name': 'EN Rate Cfg',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 142,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 143,
                'name': 'EN Comm Flt Actn',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 144,
                'name': 'EN Idle Flt Actn',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 145,
                'name': 'EN Flt Cfg Logic',
                'defaultValue': '0000000000000000',
                'record': true,
                'type': '0g=='
            },
            {
                'number': 146,
                'name': 'EN Flt Cfg Ref',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 147,
                'name': 'EN Flt Cfg DL 1',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 148,
                'name': 'EN Flt Cfg DL 2',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 149,
                'name': 'EN Flt Cfg DL 3',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 150,
                'name': 'EN Flt Cfg DL 4',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 151,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 152,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 153,
                'name': 'EN Data In 1',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 154,
                'name': 'EN Data In 2',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 155,
                'name': 'EN Data In 3',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 156,
                'name': 'EN Data In 4',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 157,
                'name': 'EN Data Out 1',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 158,
                'name': 'EN Data Out 2',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 159,
                'name': 'EN Data Out 3',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 160,
                'name': 'EN Data Out 4',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 161,
                'name': 'Opt Data In 1',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 162,
                'name': 'Opt Data In 2',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 163,
                'name': 'Opt Data In 3',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 164,
                'name': 'Opt Data In 4',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 165,
                'name': 'Opt Data Out 1',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 166,
                'name': 'Opt Data Out 2',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 167,
                'name': 'Opt Data Out 3',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 168,
                'name': 'Opt Data Out 4',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 169,
                'name': 'MultiDrv Sel',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 170,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 171,
                'name': 'Drv 1 Addr',
                'defaultValue': '2',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 172,
                'name': 'Drv 2 Addr',
                'defaultValue': '3',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 173,
                'name': 'Drv 3 Addr',
                'defaultValue': '4',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 174,
                'name': 'Drv 4 Addr',
                'defaultValue': '5',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 175,
                'name': 'DSI I/O Cfg',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 176,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 177,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 178,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 179,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 180,
                'name': 'Stp Logic 0',
                'defaultValue': '0000000011110001',
                'record': true,
                'type': '0g=='
            },
            {
                'number': 181,
                'name': 'Stp Logic 1',
                'defaultValue': '0000000011110001',
                'record': true,
                'type': '0g=='
            },
            {
                'number': 182,
                'name': 'Stp Logic 2',
                'defaultValue': '0000000011110001',
                'record': true,
                'type': '0g=='
            },
            {
                'number': 183,
                'name': 'Stp Logic 3',
                'defaultValue': '0000000011110001',
                'record': true,
                'type': '0g=='
            },
            {
                'number': 184,
                'name': 'Stp Logic 4',
                'defaultValue': '0000000011110001',
                'record': true,
                'type': '0g=='
            },
            {
                'number': 185,
                'name': 'Stp Logic 5',
                'defaultValue': '0000000011110001',
                'record': true,
                'type': '0g=='
            },
            {
                'number': 186,
                'name': 'Stp Logic 6',
                'defaultValue': '0000000011110001',
                'record': true,
                'type': '0g=='
            },
            {
                'number': 187,
                'name': 'Stp Logic 7',
                'defaultValue': '0000000011110001',
                'record': true,
                'type': '0g=='
            },
            {
                'number': 188,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 189,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 190,
                'name': 'Stp Logic Time 0',
                'defaultValue': '300',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 191,
                'name': 'Stp Logic Time 1',
                'defaultValue': '300',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 192,
                'name': 'Stp Logic Time 2',
                'defaultValue': '300',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 193,
                'name': 'Stp Logic Time 3',
                'defaultValue': '300',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 194,
                'name': 'Stp Logic Time 4',
                'defaultValue': '300',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 195,
                'name': 'Stp Logic Time 5',
                'defaultValue': '300',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 196,
                'name': 'Stp Logic Time 6',
                'defaultValue': '300',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 197,
                'name': 'Stp Logic Time 7',
                'defaultValue': '300',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 198,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 199,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 200,
                'name': 'Step Units 0',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 201,
                'name': 'Step Units F 0',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 202,
                'name': 'Step Units 1',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 203,
                'name': 'Step Units F 1',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 204,
                'name': 'Step Units 2',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 205,
                'name': 'Step Units F 2',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 206,
                'name': 'Step Units 3',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 207,
                'name': 'Step Units F 3',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 208,
                'name': 'Step Units 4',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 209,
                'name': 'Step Units F 4',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 210,
                'name': 'Step Units 5',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 211,
                'name': 'Step Units F 5',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 212,
                'name': 'Step Units 6',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 213,
                'name': 'Step Units F 6',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 214,
                'name': 'Step Units 7',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 215,
                'name': 'Step Units F 7',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 216,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 217,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 218,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 219,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 220,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 221,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 222,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 223,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 224,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 225,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 226,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 227,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 228,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 229,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 230,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 231,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 232,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 233,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 234,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 235,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 236,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 237,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 238,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 239,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 240,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 241,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 242,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 243,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 244,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 245,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 246,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 247,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 248,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 249,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 250,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 251,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 252,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 253,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 254,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 255,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 256,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 257,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 258,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 259,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 260,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 261,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 262,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 263,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 264,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 265,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 266,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 267,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 268,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 269,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 270,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 271,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 272,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 273,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 274,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 275,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 276,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 277,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 278,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 279,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 280,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 281,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 282,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 283,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 284,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 285,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 286,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 287,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 288,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 289,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 290,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 291,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 292,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 293,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 294,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 295,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 296,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 297,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 298,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 299,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 300,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 301,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 302,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 303,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 304,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 305,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 306,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 307,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 308,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 309,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 310,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 311,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 312,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 313,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 314,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 315,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 316,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 317,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 318,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 319,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 320,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 321,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 322,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 323,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 324,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 325,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 326,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 327,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 328,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 329,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 330,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 331,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 332,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 333,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 334,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 335,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 336,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 337,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 338,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 339,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 340,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 341,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 342,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 343,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 344,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 345,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 346,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 347,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 348,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 349,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 350,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 351,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 352,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 353,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 354,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 355,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 356,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 357,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 358,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 359,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 360,
                'name': 'Analog In 0-10V',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 361,
                'name': 'Analog In 4-20mA',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 362,
                'name': 'Elapsed Time-hr',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 363,
                'name': 'Elapsed Time-min',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 364,
                'name': 'Counter Status',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 365,
                'name': 'Timer Status',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 366,
                'name': 'Timer StatusF',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 367,
                'name': 'Drive Type',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 368,
                'name': 'Testpoint Data',
                'defaultValue': '0000000000000000',
                'record': false,
                'type': '0g=='
            },
            {
                'number': 369,
                'name': 'Motor OL Level',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 370,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 371,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 372,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 373,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 374,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 375,
                'name': 'Slip Hz Meter',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 376,
                'name': 'Speed Feedback',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 377,
                'name': 'Speed Feedback F',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 378,
                'name': 'Encoder Speed',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 379,
                'name': 'Encoder Speed F',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 380,
                'name': 'DC Bus Ripple',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 381,
                'name': 'Output Powr Fctr',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 382,
                'name': 'Torque Current',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 383,
                'name': 'PID1 Fdbk Displ',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 384,
                'name': 'PID1 Setpnt Disp',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 385,
                'name': 'PID2 Fdbk Displ',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 386,
                'name': 'PID2 Setpnt Disp',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 387,
                'name': 'Position Status',
                'defaultValue': '0000000000000000',
                'record': false,
                'type': '0g=='
            },
            {
                'number': 388,
                'name': 'Units Traveled H',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 389,
                'name': 'Units Traveled L',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 390,
                'name': 'Fiber Status',
                'defaultValue': '0000000000000000',
                'record': false,
                'type': '0g=='
            },
            {
                'number': 391,
                'name': 'Stp Logic Status',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 392,
                'name': 'RdyBit Mode Act',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 393,
                'name': 'Drive Status 2',
                'defaultValue': '0000000000000000',
                'record': false,
                'type': '0g=='
            },
            {
                'number': 394,
                'name': 'Dig Out Status',
                'defaultValue': '0000000000000000',
                'record': false,
                'type': '0g=='
            },
            {
                'number': 395,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 396,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 397,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 398,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 399,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 400,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 401,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 402,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 403,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 404,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 405,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 406,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 407,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 408,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 409,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 410,
                'name': 'Preset Freq 0',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 411,
                'name': 'Preset Freq 1',
                'defaultValue': '500',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 412,
                'name': 'Preset Freq 2',
                'defaultValue': '1000',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 413,
                'name': 'Preset Freq 3',
                'defaultValue': '2000',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 414,
                'name': 'Preset Freq 4',
                'defaultValue': '3000',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 415,
                'name': 'Preset Freq 5',
                'defaultValue': '4000',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 416,
                'name': 'Preset Freq 6',
                'defaultValue': '5000',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 417,
                'name': 'Preset Freq 7',
                'defaultValue': '6000',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 418,
                'name': 'Preset Freq 8',
                'defaultValue': '6000',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 419,
                'name': 'Preset Freq 9',
                'defaultValue': '6000',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 420,
                'name': 'Preset Freq 10',
                'defaultValue': '6000',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 421,
                'name': 'Preset Freq 11',
                'defaultValue': '6000',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 422,
                'name': 'Preset Freq 12',
                'defaultValue': '6000',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 423,
                'name': 'Preset Freq 13',
                'defaultValue': '6000',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 424,
                'name': 'Preset Freq 14',
                'defaultValue': '6000',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 425,
                'name': 'Preset Freq 15',
                'defaultValue': '6000',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 426,
                'name': 'Keypad Freq',
                'defaultValue': '6000',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 427,
                'name': 'MOP Freq',
                'defaultValue': '6000',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 428,
                'name': 'MOP Reset Sel',
                'defaultValue': '1',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 429,
                'name': 'MOP Preload',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 430,
                'name': 'MOP Time',
                'defaultValue': '100',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 431,
                'name': 'Jog Frequency',
                'defaultValue': '1000',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 432,
                'name': 'Jog Accel/Decel',
                'defaultValue': '1000',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 433,
                'name': 'Purge Frequency',
                'defaultValue': '500',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 434,
                'name': 'DC Brake Time',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 435,
                'name': 'DC Brake Level',
                'defaultValue': '7',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 436,
                'name': 'DC Brk Time@Strt',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 437,
                'name': 'DB Resistor Sel',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 438,
                'name': 'DB Threshold',
                'defaultValue': '1000',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 439,
                'name': 'S Curve %',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 440,
                'name': 'PWM Frequency',
                'defaultValue': '40',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 441,
                'name': 'Droop Hertz@ FLA',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 442,
                'name': 'Accel Time 2',
                'defaultValue': '1000',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 443,
                'name': 'Decel Time 2',
                'defaultValue': '1000',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 444,
                'name': 'Accel Time 3',
                'defaultValue': '1000',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 445,
                'name': 'Decel Time 3',
                'defaultValue': '1000',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 446,
                'name': 'Accel Time 4',
                'defaultValue': '1000',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 447,
                'name': 'Decel Time 4',
                'defaultValue': '1000',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 448,
                'name': 'Skip Frequency 1',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 449,
                'name': 'Skip Freq Band 1',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 450,
                'name': 'Skip Frequency 2',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 451,
                'name': 'Skip Freq Band 2',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 452,
                'name': 'Skip Frequency 3',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 453,
                'name': 'Skip Freq Band 3',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 454,
                'name': 'Skip Frequency 4',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 455,
                'name': 'Skip Freq Band 4',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 456,
                'name': 'PID 1 Trim Hi',
                'defaultValue': '600',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 457,
                'name': 'PID 1 Trim Lo',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 458,
                'name': 'PID 1 Trim Sel',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 459,
                'name': 'PID 1 Ref Sel',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 460,
                'name': 'PID 1 Fdback Sel',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 461,
                'name': 'PID 1 Prop Gain',
                'defaultValue': '1',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 462,
                'name': 'PID 1 Integ Time',
                'defaultValue': '20',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 463,
                'name': 'PID 1 Diff Rate',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 464,
                'name': 'PID 1 Setpoint',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 465,
                'name': 'PID 1 Deadband',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 466,
                'name': 'PID 1 Preload',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 467,
                'name': 'PID 1 Invert Err',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 468,
                'name': 'PID 2 Trim Hi',
                'defaultValue': '600',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 469,
                'name': 'PID 2 Trim Lo',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 470,
                'name': 'PID 2 Trim Sel',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 471,
                'name': 'PID 2 Ref Sel',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 472,
                'name': 'PID 2 Fdback Sel',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 473,
                'name': 'PID 2 Prop Gain',
                'defaultValue': '1',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 474,
                'name': 'PID 2 Integ Time',
                'defaultValue': '20',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 475,
                'name': 'PID 2 Diff Rate',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 476,
                'name': 'PID 2 Setpoint',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 477,
                'name': 'PID 2 Deadband',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 478,
                'name': 'PID 2 Preload',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 479,
                'name': 'PID 2 Invert Err',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 480,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 481,
                'name': 'Process Disp Lo',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 482,
                'name': 'Process Disp Hi',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 483,
                'name': 'Testpoint Sel',
                'defaultValue': '0000010000000000',
                'record': false,
                'type': '0g=='
            },
            {
                'number': 484,
                'name': 'Current Limit 1',
                'defaultValue': '195',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 485,
                'name': 'Current Limit 2',
                'defaultValue': '143',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 486,
                'name': 'Shear Pin1 Level',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 487,
                'name': 'Shear Pin 1 Time',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 488,
                'name': 'Shear Pin2 Level',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 489,
                'name': 'Shear Pin 2 Time',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 490,
                'name': 'Load Loss Level',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 491,
                'name': 'Load Loss Time',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 492,
                'name': 'Stall Fault Time',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 493,
                'name': 'Motor OL Select',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 494,
                'name': 'Motor OL Ret',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 495,
                'name': 'Drive OL Mode',
                'defaultValue': '3',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 496,
                'name': 'IR Voltage Drop',
                'defaultValue': '43',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 497,
                'name': 'Flux Current Ref',
                'defaultValue': '403',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 498,
                'name': 'Motor Rr',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 499,
                'name': 'Motor Lm',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 500,
                'name': 'Motor Lx',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 501,
                'name': 'PM IR Voltage',
                'defaultValue': '1150',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 502,
                'name': 'PM IXd Voltage',
                'defaultValue': '1791',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 503,
                'name': 'PM IXq Voltage',
                'defaultValue': '5321',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 504,
                'name': 'PM BEMF Voltage',
                'defaultValue': '3280',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 505,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 506,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 507,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 508,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 509,
                'name': 'Speed Reg Sel',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 510,
                'name': 'Freq 1',
                'defaultValue': '833',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 511,
                'name': 'Freq 1 BW',
                'defaultValue': '10',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 512,
                'name': 'Freq 2',
                'defaultValue': '1500',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 513,
                'name': 'Freq 2 BW',
                'defaultValue': '10',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 514,
                'name': 'Freq 3',
                'defaultValue': '2000',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 515,
                'name': 'Freq 3 BW',
                'defaultValue': '10',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 516,
                'name': 'PM Initial Sel',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 517,
                'name': 'PM DC Inject Cur',
                'defaultValue': '30',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 518,
                'name': 'PM Align Time',
                'defaultValue': '7',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 519,
                'name': 'PM HFI NS Cur',
                'defaultValue': '100',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 520,
                'name': 'PM Bus Reg Kd',
                'defaultValue': '2',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 521,
                'name': 'Freq 1 Kp',
                'defaultValue': '1000',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 522,
                'name': 'Freq 1 Ki',
                'defaultValue': '100',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 523,
                'name': 'Freq 2 Kp',
                'defaultValue': '1000',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 524,
                'name': 'Freq 2 Ki',
                'defaultValue': '100',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 525,
                'name': 'Freq 3 Kp',
                'defaultValue': '1000',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 526,
                'name': 'Freq 3 Ki',
                'defaultValue': '100',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 527,
                'name': 'PM FWKn 1 Kp',
                'defaultValue': '350',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 528,
                'name': 'PM FWKn 2 Kp',
                'defaultValue': '300',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 529,
                'name': 'PM Control Cfg',
                'defaultValue': '0000000000000111',
                'record': true,
                'type': '0g=='
            },
            {
                'number': 530,
                'name': 'Boost Select',
                'defaultValue': '7',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 531,
                'name': 'Start Boost',
                'defaultValue': '25',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 532,
                'name': 'Break Voltage',
                'defaultValue': '250',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 533,
                'name': 'Break Frequency',
                'defaultValue': '150',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 534,
                'name': 'Maximum Voltage',
                'defaultValue': '460',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 535,
                'name': 'Motor Fdbk Type',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 536,
                'name': 'Encoder PPR',
                'defaultValue': '1024',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 537,
                'name': 'Pulse In Scale',
                'defaultValue': '64',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 538,
                'name': 'Ki Speed Loop',
                'defaultValue': '20',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 539,
                'name': 'Kp Speed Loop',
                'defaultValue': '5',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 540,
                'name': 'Var PWM Disable',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 541,
                'name': 'Auto Rstrt Tries',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 542,
                'name': 'Auto Rstrt Delay',
                'defaultValue': '10',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 543,
                'name': 'Start At PowerUp',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 544,
                'name': 'Reverse Disable',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 545,
                'name': 'Flying Start En',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 546,
                'name': 'FlyStrt CurLimit',
                'defaultValue': '65',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 547,
                'name': 'Compensation',
                'defaultValue': '1',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 548,
                'name': 'Power Loss Mode',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 549,
                'name': 'Half Bus Enable',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 550,
                'name': 'Bus Reg Enable',
                'defaultValue': '1',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 551,
                'name': 'Fault Clear',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 552,
                'name': 'Program Lock',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 553,
                'name': 'Program Lock Mod',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 554,
                'name': 'Drv Ambient Sel',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 555,
                'name': 'Reset Meters',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 556,
                'name': 'Text Scroll',
                'defaultValue': '2',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 557,
                'name': 'Out Phas Loss En',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 558,
                'name': 'Positioning Mode',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 559,
                'name': 'Counts Per Unit',
                'defaultValue': '4096',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 560,
                'name': 'Enh Control Word',
                'defaultValue': '0000000000000000',
                'record': true,
                'type': '0g=='
            },
            {
                'number': 561,
                'name': 'Home Save',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 562,
                'name': 'Find Home Freq',
                'defaultValue': '100',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 563,
                'name': 'Find Home Dir',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 564,
                'name': 'Encoder Pos Tol',
                'defaultValue': '100',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 565,
                'name': 'Pos Reg Filter',
                'defaultValue': '8',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 566,
                'name': 'Pos Reg Gain',
                'defaultValue': '30',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 567,
                'name': 'Max Traverse',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 568,
                'name': 'Traverse Inc',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 569,
                'name': 'Traverse Dec',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 570,
                'name': 'P Jump',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 571,
                'name': 'Sync Time',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 572,
                'name': 'Speed Ratio',
                'defaultValue': '100',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 573,
                'name': 'Mtr Options Cfg',
                'defaultValue': '0000000000000011',
                'record': true,
                'type': '0g=='
            },
            {
                'number': 574,
                'name': 'RdyBit Mode Cfg',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 575,
                'name': 'Flux Braking En',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 576,
                'name': 'Phase Loss Level',
                'defaultValue': '250',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 577,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 578,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 579,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 580,
                'name': 'Current Loop BW',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 581,
                'name': 'PM Stable 1 Freq',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 582,
                'name': 'PM Stable 2 Freq',
                'defaultValue': '45',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 583,
                'name': 'PM Stable 1 Kp',
                'defaultValue': '40',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 584,
                'name': 'PM Stable 2 Kp',
                'defaultValue': '250',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 585,
                'name': 'PM Stable Brk Pt',
                'defaultValue': '40',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 586,
                'name': 'PM Stepload Kp',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 587,
                'name': 'PM 1 Efficiency',
                'defaultValue': '120',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 588,
                'name': 'PM 2 Efficiency',
                'defaultValue': '500',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 589,
                'name': 'PM Algor Sel',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 590,
                'name': 'Reserved',
                'defaultValue': '1000',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 591,
                'name': 'Reserved',
                'defaultValue': '35',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 592,
                'name': 'Reserved',
                'defaultValue': '30',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 593,
                'name': 'Reserved',
                'defaultValue': '100',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 594,
                'name': 'Reserved',
                'defaultValue': '100',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 595,
                'name': 'Reserved',
                'defaultValue': '10',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 596,
                'name': 'Reserved',
                'defaultValue': '10',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 597,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 598,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 599,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 600,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 601,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 602,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 603,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 604,
                'name': 'Fault 4 Code',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 605,
                'name': 'Fault 5 Code',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 606,
                'name': 'Fault 6 Code',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 607,
                'name': 'Fault 7 Code',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 608,
                'name': 'Fault 8 Code',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 609,
                'name': 'Fault 9 Code',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 610,
                'name': 'Fault10 Code',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 611,
                'name': 'Fault 1 Time-hr',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 612,
                'name': 'Fault 2 Time-hr',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 613,
                'name': 'Fault 3 Time-hr',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 614,
                'name': 'Fault 4 Time-hr',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 615,
                'name': 'Fault 5 Time-hr',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 616,
                'name': 'Fault 6 Time-hr',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 617,
                'name': 'Fault 7 Time-hr',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 618,
                'name': 'Fault 8 Time-hr',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 619,
                'name': 'Fault 9 Time-hr',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 620,
                'name': 'Fault10 Time-hr',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 621,
                'name': 'Fault 1 Time-min',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 622,
                'name': 'Fault 2 Time-min',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 623,
                'name': 'Fault 3 Time-min',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 624,
                'name': 'Fault 4 Time-min',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 625,
                'name': 'Fault 5 Time-min',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 626,
                'name': 'Fault 6 Time-min',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 627,
                'name': 'Fault 7 Time-min',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 628,
                'name': 'Fault 8 Time-min',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 629,
                'name': 'Fault 9 Time-min',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 630,
                'name': 'Fault10 Time-min',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 631,
                'name': 'Fault 1 Freq',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 632,
                'name': 'Fault 2 Freq',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 633,
                'name': 'Fault 3 Freq',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 634,
                'name': 'Fault 4 Freq',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 635,
                'name': 'Fault 5 Freq',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 636,
                'name': 'Fault 6 Freq',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 637,
                'name': 'Fault 7 Freq',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 638,
                'name': 'Fault 8 Freq',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 639,
                'name': 'Fault 9 Freq',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 640,
                'name': 'Fault10 Freq',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 641,
                'name': 'Fault 1 Current',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 642,
                'name': 'Fault 2 Current',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 643,
                'name': 'Fault 3 Current',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 644,
                'name': 'Fault 4 Current',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 645,
                'name': 'Fault 5 Current',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 646,
                'name': 'Fault 6 Current',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 647,
                'name': 'Fault 7 Current',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 648,
                'name': 'Fault 8 Current',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 649,
                'name': 'Fault 9 Current',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 650,
                'name': 'Fault10 Current',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 651,
                'name': 'Fault 1 BusVolts',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 652,
                'name': 'Fault 2 BusVolts',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 653,
                'name': 'Fault 3 BusVolts',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 654,
                'name': 'Fault 4 BusVolts',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 655,
                'name': 'Fault 5 BusVolts',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 656,
                'name': 'Fault 6 BusVolts',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 657,
                'name': 'Fault 7 BusVolts',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 658,
                'name': 'Fault 8 BusVolts',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 659,
                'name': 'Fault 9 BusVolts',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 660,
                'name': 'Fault10 BusVolts',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 661,
                'name': 'Status @ Fault 1',
                'defaultValue': '0000000000000000',
                'record': false,
                'type': '0g=='
            },
            {
                'number': 662,
                'name': 'Status @ Fault 2',
                'defaultValue': '0000000000000000',
                'record': false,
                'type': '0g=='
            },
            {
                'number': 663,
                'name': 'Status @ Fault 3',
                'defaultValue': '0000000000000000',
                'record': false,
                'type': '0g=='
            },
            {
                'number': 664,
                'name': 'Status @ Fault 4',
                'defaultValue': '0000000000000000',
                'record': false,
                'type': '0g=='
            },
            {
                'number': 665,
                'name': 'Status @ Fault 5',
                'defaultValue': '0000000000000000',
                'record': false,
                'type': '0g=='
            },
            {
                'number': 666,
                'name': 'Status @ Fault 6',
                'defaultValue': '0000000000000000',
                'record': false,
                'type': '0g=='
            },
            {
                'number': 667,
                'name': 'Status @ Fault 7',
                'defaultValue': '0000000000000000',
                'record': false,
                'type': '0g=='
            },
            {
                'number': 668,
                'name': 'Status @ Fault 8',
                'defaultValue': '0000000000000000',
                'record': false,
                'type': '0g=='
            },
            {
                'number': 669,
                'name': 'Status @ Fault 9',
                'defaultValue': '0000000000000000',
                'record': false,
                'type': '0g=='
            },
            {
                'number': 670,
                'name': 'Status @ Fault10',
                'defaultValue': '0000000000000000',
                'record': false,
                'type': '0g=='
            },
            {
                'number': 671,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 672,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 673,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 674,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 675,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 676,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 677,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 678,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 679,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 680,
                'name': 'Reserved',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 681,
                'name': 'Comm Sts - DSI',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 682,
                'name': 'Comm Sts - Opt',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 683,
                'name': 'Com Sts-Emb Enet',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 684,
                'name': 'EN Addr Src',
                'defaultValue': '2',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 685,
                'name': 'EN Rate Act',
                'defaultValue': '0',
                'record': true,
                'type': 'xw=='
            },
            {
                'number': 686,
                'name': 'DSI I/O Act',
                'defaultValue': '0000000000000000',
                'record': true,
                'type': '0g=='
            },
            {
                'number': 687,
                'name': 'HW Addr 1',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 688,
                'name': 'HW Addr 2',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 689,
                'name': 'HW Addr 3',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 690,
                'name': 'HW Addr 4',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 691,
                'name': 'HW Addr 5',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 692,
                'name': 'HW Addr 6',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 693,
                'name': 'EN IP Addr Act 1',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 694,
                'name': 'EN IP Addr Act 2',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 695,
                'name': 'EN IP Addr Act 3',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 696,
                'name': 'EN IP Addr Act 4',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 697,
                'name': 'EN Subnet Act 1',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 698,
                'name': 'EN Subnet Act 2',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 699,
                'name': 'EN Subnet Act 3',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 700,
                'name': 'EN Subnet Act 4',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 701,
                'name': 'EN Gateway Act 1',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 702,
                'name': 'EN Gateway Act 2',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 703,
                'name': 'EN Gateway Act 3',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 704,
                'name': 'EN Gateway Act 4',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 705,
                'name': 'Drv 0 Logic Cmd',
                'defaultValue': '0000000000000000',
                'record': false,
                'type': '0g=='
            },
            {
                'number': 706,
                'name': 'Drv 0 Reference',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 707,
                'name': 'Drv 0 Logic Sts',
                'defaultValue': '0000000000000000',
                'record': false,
                'type': '0g=='
            },
            {
                'number': 708,
                'name': 'Drv 0 Feedback',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 709,
                'name': 'Drv 1 Logic Cmd',
                'defaultValue': '0000000000000000',
                'record': false,
                'type': '0g=='
            },
            {
                'number': 710,
                'name': 'Drv 1 Reference',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 711,
                'name': 'Drv 1 Logic Sts',
                'defaultValue': '0000000000000000',
                'record': false,
                'type': '0g=='
            },
            {
                'number': 712,
                'name': 'Drv 1 Feedback',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 713,
                'name': 'Drv 2 Logic Cmd',
                'defaultValue': '0000000000000000',
                'record': false,
                'type': '0g=='
            },
            {
                'number': 714,
                'name': 'Drv 2 Reference',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 715,
                'name': 'Drv 2 Logic Sts',
                'defaultValue': '0000000000000000',
                'record': false,
                'type': '0g=='
            },
            {
                'number': 716,
                'name': 'Drv 2 Feedback',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 717,
                'name': 'Drv 3 Logic Cmd',
                'defaultValue': '0000000000000000',
                'record': false,
                'type': '0g=='
            },
            {
                'number': 718,
                'name': 'Drv 3 Reference',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 719,
                'name': 'Drv 3 Logic Sts',
                'defaultValue': '0000000000000000',
                'record': false,
                'type': '0g=='
            },
            {
                'number': 720,
                'name': 'Drv 3 Feedback',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 721,
                'name': 'Drv 4 Logic Cmd',
                'defaultValue': '0000000000000000',
                'record': false,
                'type': '0g=='
            },
            {
                'number': 722,
                'name': 'Drv 4 Reference',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 723,
                'name': 'Drv 4 Logic Sts',
                'defaultValue': '0000000000000000',
                'record': false,
                'type': '0g=='
            },
            {
                'number': 724,
                'name': 'Drv 4 Feedback',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 725,
                'name': 'EN Rx Overruns',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 726,
                'name': 'EN Rx Packets',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 727,
                'name': 'EN Rx Errors',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 728,
                'name': 'EN Tx Packets',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 729,
                'name': 'EN Tx Errors',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 730,
                'name': 'EN Missed IO Pkt',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            },
            {
                'number': 731,
                'name': 'DSI Errors',
                'defaultValue': '0',
                'record': false,
                'type': 'xw=='
            }
            ]";
    }
}