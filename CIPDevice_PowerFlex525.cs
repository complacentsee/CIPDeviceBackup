using Newtonsoft.Json;
using powerFlexBackup.cipdevice.deviceParameterObjects;

namespace powerFlexBackup.cipdevice
{
    public class CIPDevice_PowerFlex525 : CIPDevice{
        public CIPDevice_PowerFlex525(String driveAddress, IdentityObject identityObject, Sres.Net.EEIP.EEIPClient eeipClient) :
            base(driveAddress, identityObject, eeipClient)
        {
            setDriveParameterClassID(0x0F);
            setDriveParameterList(JsonConvert.DeserializeObject<List<DeviceParameter>>(parameterListJSON));    
            setInstanceAttribute(JsonConvert.DeserializeObject<List<InstanceAttribute>>(instanceAttributeJSON));    
        }

        /* Process the parameter from a bytearray to an int based on type
        0xC2 = SINT (8-bits)
        0xC3 = INT (16-bits)
        0xC4 = DINT (32-bits)
        0xC6 = USINT (8-bits)
        0xC7 = UINT (16-bits)
        0xCA = REAL (32-bits)
        0xD2 = WORD (16-bits) */
        
        //FIXME: Should this be a list that we can look up the converion for each drive? Then the method can exist in the base class.
        public const byte DriveSINT = 0xC2;
        public const byte DriveINT = 0xC3;
        public const byte DriveDINT = 0xC4;
        public const byte DriveUSINT = 0xC6;
        public const byte DriveUINT = 0xC7;
        public const byte DriveREAL = 0xCA;
        public const byte DriveWORD = 0xD2;

        public override string getParameterValuefromBytes(byte[] parameterValueBytes, byte[] parameterType)
        {
            switch (parameterType[0])
            {
                case DriveSINT:
                    return Convert.ToString(Convert.ToInt16(parameterValueBytes[0]));

                case DriveINT:
                    return Convert.ToString(Convert.ToInt16(parameterValueBytes[0]
                                        | parameterValueBytes[1] << 8));

                case DriveDINT:
                    return Convert.ToString(Convert.ToInt32(parameterValueBytes[0]
                                        | parameterValueBytes[1] << 8
                                        | parameterValueBytes[2] << 16
                                        | parameterValueBytes[3] << 24));

                case DriveUSINT:
                    return Convert.ToString(Convert.ToUInt16(parameterValueBytes[0]));

                case DriveUINT:
                    return Convert.ToString(Convert.ToUInt16(parameterValueBytes[0]
                                        | parameterValueBytes[1] << 8));

                case DriveREAL:
                    return Convert.ToString(Convert.ToDecimal(parameterValueBytes[0]
                                        | parameterValueBytes[1] << 8
                                        | parameterValueBytes[2] << 16
                                        | parameterValueBytes[3] << 24));

                case DriveWORD:
                return Convert.ToString(parameterValueBytes[0]
                                        | parameterValueBytes[1] << 8,2).PadLeft(16,'0');
                default:
                    return "Unknown Parameter Type";    
            }
        }

        public string instanceAttributeJSON = @"[
                        {
                            'AttributeID': 1,
                            'Name': 'Parameter Value'
                            },                   
                            {
                            'AttributeID': 2,
                            'Name': 'Link Path Size'
                            },
                            {
                            'AttributeID': 3,
                            'Name': 'Link Path'
                            },
                            {
                            'AttributeID': 4,
                            'Name': 'Descriptor'
                            },
                            {
                            'AttributeID': 5,
                            'Name': 'Data Type'
                            },
                            {
                            'AttributeID': 6,
                            'Name': 'Data Size'
                            },
                            {
                            'AttributeID': 7,
                            'Name': 'Parameter Name String'
                            },
                            {
                            'AttributeID': 8,
                            'Name': 'Units String'
                            },
                            {
                            'AttributeID': 9,
                            'Name': 'Help String'
                            },
                            {
                            'AttributeID': 10,
                            'Name': 'Minimum Value'
                            },
                            {
                            'AttributeID': 11,
                            'Name': 'Maximum Value'
                            },
                            {
                            'AttributeID': 12,
                            'Name': 'Default Value'
                            },
                            {
                            'AttributeID': 13,
                            'Name': 'Scaling Multiplier'
                            },
                            {
                            'AttributeID': 14,
                            'Name': 'Scaling Divisor'
                            },
                            {
                            'AttributeID': 15,
                            'Name': 'Scaling Base'
                            },
                            {
                            'AttributeID': 16,
                            'Name': 'Scaling Offset'
                            },
                            {
                            'AttributeID': 17,
                            'Name': 'Multiplier Link'
                            },
                            {
                            'AttributeID': 18,
                            'Name': 'Divisor Link'
                            },
                            {
                            'AttributeID': 19,
                            'Name': 'Base Link'
                            },
                            {
                            'AttributeID': 20,
                            'Name': 'Offset Link'
                            },
                            {
                            'AttributeID': 21,
                            'Name': 'Decimal Precision'
                            }
                            ]";

                public string parameterListJSON = @"[
                            {
                        'parameterNumber': 1,
                        'parameterName': 'Output Freq',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 2,
                        'parameterName': 'Commanded Freq',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 3,
                        'parameterName': 'Output Current',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 4,
                        'parameterName': 'Output Voltage',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 5,
                        'parameterName': 'DC Bus Voltage',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 6,
                        'parameterName': 'Drive Status',
                        'defaultParameterValue': '0000000000000000',
                        'recordValue': false,
                        'parameterType': '0g=='
                    },
                    {
                        'parameterNumber': 7,
                        'parameterName': 'Fault 1 Code',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 8,
                        'parameterName': 'Fault 2 Code',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 9,
                        'parameterName': 'Fault 3 Code',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 10,
                        'parameterName': 'Process Display',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 11,
                        'parameterName': 'Process Fract',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 12,
                        'parameterName': 'Control Source',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 13,
                        'parameterName': 'Contrl In Status',
                        'defaultParameterValue': '0000000000000000',
                        'recordValue': false,
                        'parameterType': '0g=='
                    },
                    {
                        'parameterNumber': 14,
                        'parameterName': 'Dig In Status',
                        'defaultParameterValue': '0000000000000000',
                        'recordValue': false,
                        'parameterType': '0g=='
                    },
                    {
                        'parameterNumber': 15,
                        'parameterName': 'Output RPM',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 16,
                        'parameterName': 'Output Speed',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 17,
                        'parameterName': 'Output Power',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 18,
                        'parameterName': 'Power Saved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 19,
                        'parameterName': 'Elapsed Run Time',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 20,
                        'parameterName': 'Average Power',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 21,
                        'parameterName': 'Elapsed kWh',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 22,
                        'parameterName': 'Elapsed MWh',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 23,
                        'parameterName': 'Energy Saved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 24,
                        'parameterName': 'Accum kWh Sav',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 25,
                        'parameterName': 'Accum Cost Sav',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 26,
                        'parameterName': 'Accum CO2 Sav',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 27,
                        'parameterName': 'Drive Temp',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 28,
                        'parameterName': 'Control Temp',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 29,
                        'parameterName': 'Control SW Ver',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 30,
                        'parameterName': 'Language',
                        'defaultParameterValue': '1',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 31,
                        'parameterName': 'Motor NP Volts',
                        'defaultParameterValue': '460',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 32,
                        'parameterName': 'Motor NP Hertz',
                        'defaultParameterValue': '60',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 33,
                        'parameterName': 'Motor OL Current',
                        'defaultParameterValue': '130',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 34,
                        'parameterName': 'Motor NP FLA',
                        'defaultParameterValue': '101',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 35,
                        'parameterName': 'Motor NP Poles',
                        'defaultParameterValue': '4',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 36,
                        'parameterName': 'Motor NP RPM',
                        'defaultParameterValue': '1750',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 37,
                        'parameterName': 'Motor NP Power',
                        'defaultParameterValue': '550',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 38,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 39,
                        'parameterName': 'Torque Perf Mode',
                        'defaultParameterValue': '1',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 40,
                        'parameterName': 'Autotune',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 41,
                        'parameterName': 'Accel Time 1',
                        'defaultParameterValue': '1000',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 42,
                        'parameterName': 'Decel Time 1',
                        'defaultParameterValue': '1000',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 43,
                        'parameterName': 'Minimum Freq',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 44,
                        'parameterName': 'Maximum Freq',
                        'defaultParameterValue': '6000',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 45,
                        'parameterName': 'Stop Mode',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 46,
                        'parameterName': 'Start Source 1',
                        'defaultParameterValue': '1',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 47,
                        'parameterName': 'Speed Reference1',
                        'defaultParameterValue': '1',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 48,
                        'parameterName': 'Start Source 2',
                        'defaultParameterValue': '2',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 49,
                        'parameterName': 'Speed Reference2',
                        'defaultParameterValue': '5',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 50,
                        'parameterName': 'Start Source 3',
                        'defaultParameterValue': '5',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 51,
                        'parameterName': 'Speed Reference3',
                        'defaultParameterValue': '15',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 52,
                        'parameterName': 'Average kWh Cost',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 53,
                        'parameterName': 'Reset To Defalts',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 54,
                        'parameterName': 'Display Param',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 55,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 56,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 57,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 58,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 59,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 60,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 61,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 62,
                        'parameterName': 'DigIn TermBlk 02',
                        'defaultParameterValue': '48',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 63,
                        'parameterName': 'DigIn TermBlk 03',
                        'defaultParameterValue': '50',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 64,
                        'parameterName': '2-Wire Mode',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 65,
                        'parameterName': 'DigIn TermBlk 05',
                        'defaultParameterValue': '7',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 66,
                        'parameterName': 'DigIn TermBlk 06',
                        'defaultParameterValue': '7',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 67,
                        'parameterName': 'DigIn TermBlk 07',
                        'defaultParameterValue': '5',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 68,
                        'parameterName': 'DigIn TermBlk 08',
                        'defaultParameterValue': '9',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 69,
                        'parameterName': 'Opto Out1 Sel',
                        'defaultParameterValue': '2',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 70,
                        'parameterName': 'Opto Out1 Level',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 71,
                        'parameterName': 'Opto Out1 LevelF',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 72,
                        'parameterName': 'Opto Out2 Sel',
                        'defaultParameterValue': '1',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 73,
                        'parameterName': 'Opto Out2 Level',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 74,
                        'parameterName': 'Opto Out2 LevelF',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 75,
                        'parameterName': 'Opto Out Logic',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 76,
                        'parameterName': 'Relay Out1 Sel',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 77,
                        'parameterName': 'Relay Out1 Level',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 78,
                        'parameterName': 'RelayOut1 LevelF',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 79,
                        'parameterName': 'Relay 1 On Time',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 80,
                        'parameterName': 'Relay 1 Off Time',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 81,
                        'parameterName': 'Relay Out2 Sel',
                        'defaultParameterValue': '2',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 82,
                        'parameterName': 'Relay Out2 Level',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 83,
                        'parameterName': 'RelayOut2 LevelF',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 84,
                        'parameterName': 'Relay 2 On Time',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 85,
                        'parameterName': 'Relay 2 Off Time',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 86,
                        'parameterName': 'EM Brk Off Delay',
                        'defaultParameterValue': '200',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 87,
                        'parameterName': 'EM Brk On Delay',
                        'defaultParameterValue': '200',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 88,
                        'parameterName': 'Analog Out Sel',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 89,
                        'parameterName': 'Analog Out High',
                        'defaultParameterValue': '100',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 90,
                        'parameterName': 'Anlg Out Setpt',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 91,
                        'parameterName': 'Anlg In 0-10V Lo',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 92,
                        'parameterName': 'Anlg In 0-10V Hi',
                        'defaultParameterValue': '1000',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 93,
                        'parameterName': '10V Bipolar Enbl',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 94,
                        'parameterName': 'Anlg In V Loss',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 95,
                        'parameterName': 'Anlg In4-20mA Lo',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 96,
                        'parameterName': 'Anlg In4-20mA Hi',
                        'defaultParameterValue': '1000',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 97,
                        'parameterName': 'Anlg In mA Loss',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 98,
                        'parameterName': 'Anlg Loss Delay',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 99,
                        'parameterName': 'Analog In Filter',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 100,
                        'parameterName': 'Sleep-Wake Sel',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 101,
                        'parameterName': 'Sleep Level',
                        'defaultParameterValue': '100',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 102,
                        'parameterName': 'Sleep Time',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 103,
                        'parameterName': 'Wake Level',
                        'defaultParameterValue': '150',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 104,
                        'parameterName': 'Wake Time',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 105,
                        'parameterName': 'Safety Open En',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 106,
                        'parameterName': 'SafetyFlt RstCfg',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 107,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 108,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 109,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 110,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 111,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 112,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 113,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 114,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 115,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 116,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 117,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 118,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 119,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 120,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 121,
                        'parameterName': 'Comm Write Mode',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 122,
                        'parameterName': 'Cmd Stat Select',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 123,
                        'parameterName': 'RS485 Data Rate',
                        'defaultParameterValue': '3',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 124,
                        'parameterName': 'RS485 Node Addr',
                        'defaultParameterValue': '100',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 125,
                        'parameterName': 'Comm Loss Action',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 126,
                        'parameterName': 'Comm Loss Time',
                        'defaultParameterValue': '50',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 127,
                        'parameterName': 'RS485 Format',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 128,
                        'parameterName': 'EN Addr Sel',
                        'defaultParameterValue': '2',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 129,
                        'parameterName': 'EN IP Addr Cfg 1',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 130,
                        'parameterName': 'EN IP Addr Cfg 2',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 131,
                        'parameterName': 'EN IP Addr Cfg 3',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 132,
                        'parameterName': 'EN IP Addr Cfg 4',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 133,
                        'parameterName': 'EN Subnet Cfg 1',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 134,
                        'parameterName': 'EN Subnet Cfg 2',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 135,
                        'parameterName': 'EN Subnet Cfg 3',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 136,
                        'parameterName': 'EN Subnet Cfg 4',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 137,
                        'parameterName': 'EN Gateway Cfg 1',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 138,
                        'parameterName': 'EN Gateway Cfg 2',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 139,
                        'parameterName': 'EN Gateway Cfg 3',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 140,
                        'parameterName': 'EN Gateway Cfg 4',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 141,
                        'parameterName': 'EN Rate Cfg',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 142,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 143,
                        'parameterName': 'EN Comm Flt Actn',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 144,
                        'parameterName': 'EN Idle Flt Actn',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 145,
                        'parameterName': 'EN Flt Cfg Logic',
                        'defaultParameterValue': '0000000000000000',
                        'recordValue': true,
                        'parameterType': '0g=='
                    },
                    {
                        'parameterNumber': 146,
                        'parameterName': 'EN Flt Cfg Ref',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 147,
                        'parameterName': 'EN Flt Cfg DL 1',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 148,
                        'parameterName': 'EN Flt Cfg DL 2',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 149,
                        'parameterName': 'EN Flt Cfg DL 3',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 150,
                        'parameterName': 'EN Flt Cfg DL 4',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 151,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 152,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 153,
                        'parameterName': 'EN Data In 1',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 154,
                        'parameterName': 'EN Data In 2',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 155,
                        'parameterName': 'EN Data In 3',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 156,
                        'parameterName': 'EN Data In 4',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 157,
                        'parameterName': 'EN Data Out 1',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 158,
                        'parameterName': 'EN Data Out 2',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 159,
                        'parameterName': 'EN Data Out 3',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 160,
                        'parameterName': 'EN Data Out 4',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 161,
                        'parameterName': 'Opt Data In 1',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 162,
                        'parameterName': 'Opt Data In 2',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 163,
                        'parameterName': 'Opt Data In 3',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 164,
                        'parameterName': 'Opt Data In 4',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 165,
                        'parameterName': 'Opt Data Out 1',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 166,
                        'parameterName': 'Opt Data Out 2',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 167,
                        'parameterName': 'Opt Data Out 3',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 168,
                        'parameterName': 'Opt Data Out 4',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 169,
                        'parameterName': 'MultiDrv Sel',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 170,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 171,
                        'parameterName': 'Drv 1 Addr',
                        'defaultParameterValue': '2',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 172,
                        'parameterName': 'Drv 2 Addr',
                        'defaultParameterValue': '3',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 173,
                        'parameterName': 'Drv 3 Addr',
                        'defaultParameterValue': '4',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 174,
                        'parameterName': 'Drv 4 Addr',
                        'defaultParameterValue': '5',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 175,
                        'parameterName': 'DSI I/O Cfg',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 176,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 177,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 178,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 179,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 180,
                        'parameterName': 'Stp Logic 0',
                        'defaultParameterValue': '0000000011110001',
                        'recordValue': true,
                        'parameterType': '0g=='
                    },
                    {
                        'parameterNumber': 181,
                        'parameterName': 'Stp Logic 1',
                        'defaultParameterValue': '0000000011110001',
                        'recordValue': true,
                        'parameterType': '0g=='
                    },
                    {
                        'parameterNumber': 182,
                        'parameterName': 'Stp Logic 2',
                        'defaultParameterValue': '0000000011110001',
                        'recordValue': true,
                        'parameterType': '0g=='
                    },
                    {
                        'parameterNumber': 183,
                        'parameterName': 'Stp Logic 3',
                        'defaultParameterValue': '0000000011110001',
                        'recordValue': true,
                        'parameterType': '0g=='
                    },
                    {
                        'parameterNumber': 184,
                        'parameterName': 'Stp Logic 4',
                        'defaultParameterValue': '0000000011110001',
                        'recordValue': true,
                        'parameterType': '0g=='
                    },
                    {
                        'parameterNumber': 185,
                        'parameterName': 'Stp Logic 5',
                        'defaultParameterValue': '0000000011110001',
                        'recordValue': true,
                        'parameterType': '0g=='
                    },
                    {
                        'parameterNumber': 186,
                        'parameterName': 'Stp Logic 6',
                        'defaultParameterValue': '0000000011110001',
                        'recordValue': true,
                        'parameterType': '0g=='
                    },
                    {
                        'parameterNumber': 187,
                        'parameterName': 'Stp Logic 7',
                        'defaultParameterValue': '0000000011110001',
                        'recordValue': true,
                        'parameterType': '0g=='
                    },
                    {
                        'parameterNumber': 188,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 189,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 190,
                        'parameterName': 'Stp Logic Time 0',
                        'defaultParameterValue': '300',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 191,
                        'parameterName': 'Stp Logic Time 1',
                        'defaultParameterValue': '300',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 192,
                        'parameterName': 'Stp Logic Time 2',
                        'defaultParameterValue': '300',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 193,
                        'parameterName': 'Stp Logic Time 3',
                        'defaultParameterValue': '300',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 194,
                        'parameterName': 'Stp Logic Time 4',
                        'defaultParameterValue': '300',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 195,
                        'parameterName': 'Stp Logic Time 5',
                        'defaultParameterValue': '300',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 196,
                        'parameterName': 'Stp Logic Time 6',
                        'defaultParameterValue': '300',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 197,
                        'parameterName': 'Stp Logic Time 7',
                        'defaultParameterValue': '300',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 198,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 199,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 200,
                        'parameterName': 'Step Units 0',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 201,
                        'parameterName': 'Step Units F 0',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 202,
                        'parameterName': 'Step Units 1',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 203,
                        'parameterName': 'Step Units F 1',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 204,
                        'parameterName': 'Step Units 2',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 205,
                        'parameterName': 'Step Units F 2',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 206,
                        'parameterName': 'Step Units 3',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 207,
                        'parameterName': 'Step Units F 3',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 208,
                        'parameterName': 'Step Units 4',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 209,
                        'parameterName': 'Step Units F 4',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 210,
                        'parameterName': 'Step Units 5',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 211,
                        'parameterName': 'Step Units F 5',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 212,
                        'parameterName': 'Step Units 6',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 213,
                        'parameterName': 'Step Units F 6',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 214,
                        'parameterName': 'Step Units 7',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 215,
                        'parameterName': 'Step Units F 7',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 216,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 217,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 218,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 219,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 220,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 221,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 222,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 223,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 224,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 225,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 226,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 227,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 228,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 229,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 230,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 231,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 232,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 233,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 234,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 235,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 236,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 237,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 238,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 239,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 240,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 241,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 242,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 243,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 244,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 245,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 246,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 247,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 248,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 249,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 250,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 251,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 252,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 253,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 254,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 255,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 256,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 257,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 258,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 259,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 260,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 261,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 262,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 263,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 264,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 265,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 266,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 267,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 268,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 269,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 270,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 271,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 272,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 273,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 274,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 275,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 276,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 277,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 278,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 279,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 280,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 281,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 282,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 283,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 284,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 285,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 286,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 287,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 288,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 289,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 290,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 291,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 292,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 293,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 294,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 295,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 296,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 297,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 298,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 299,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 300,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 301,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 302,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 303,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 304,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 305,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 306,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 307,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 308,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 309,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 310,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 311,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 312,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 313,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 314,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 315,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 316,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 317,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 318,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 319,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 320,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 321,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 322,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 323,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 324,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 325,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 326,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 327,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 328,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 329,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 330,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 331,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 332,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 333,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 334,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 335,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 336,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 337,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 338,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 339,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 340,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 341,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 342,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 343,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 344,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 345,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 346,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 347,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 348,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 349,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 350,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 351,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 352,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 353,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 354,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 355,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 356,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 357,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 358,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 359,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 360,
                        'parameterName': 'Analog In 0-10V',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 361,
                        'parameterName': 'Analog In 4-20mA',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 362,
                        'parameterName': 'Elapsed Time-hr',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 363,
                        'parameterName': 'Elapsed Time-min',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 364,
                        'parameterName': 'Counter Status',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 365,
                        'parameterName': 'Timer Status',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 366,
                        'parameterName': 'Timer StatusF',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 367,
                        'parameterName': 'Drive Type',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 368,
                        'parameterName': 'Testpoint Data',
                        'defaultParameterValue': '0000000000000000',
                        'recordValue': false,
                        'parameterType': '0g=='
                    },
                    {
                        'parameterNumber': 369,
                        'parameterName': 'Motor OL Level',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 370,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 371,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 372,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 373,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 374,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 375,
                        'parameterName': 'Slip Hz Meter',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 376,
                        'parameterName': 'Speed Feedback',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 377,
                        'parameterName': 'Speed Feedback F',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 378,
                        'parameterName': 'Encoder Speed',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 379,
                        'parameterName': 'Encoder Speed F',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 380,
                        'parameterName': 'DC Bus Ripple',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 381,
                        'parameterName': 'Output Powr Fctr',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 382,
                        'parameterName': 'Torque Current',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 383,
                        'parameterName': 'PID1 Fdbk Displ',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 384,
                        'parameterName': 'PID1 Setpnt Disp',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 385,
                        'parameterName': 'PID2 Fdbk Displ',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 386,
                        'parameterName': 'PID2 Setpnt Disp',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 387,
                        'parameterName': 'Position Status',
                        'defaultParameterValue': '0000000000000000',
                        'recordValue': false,
                        'parameterType': '0g=='
                    },
                    {
                        'parameterNumber': 388,
                        'parameterName': 'Units Traveled H',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 389,
                        'parameterName': 'Units Traveled L',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 390,
                        'parameterName': 'Fiber Status',
                        'defaultParameterValue': '0000000000000000',
                        'recordValue': false,
                        'parameterType': '0g=='
                    },
                    {
                        'parameterNumber': 391,
                        'parameterName': 'Stp Logic Status',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 392,
                        'parameterName': 'RdyBit Mode Act',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 393,
                        'parameterName': 'Drive Status 2',
                        'defaultParameterValue': '0000000000000000',
                        'recordValue': false,
                        'parameterType': '0g=='
                    },
                    {
                        'parameterNumber': 394,
                        'parameterName': 'Dig Out Status',
                        'defaultParameterValue': '0000000000000000',
                        'recordValue': false,
                        'parameterType': '0g=='
                    },
                    {
                        'parameterNumber': 395,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 396,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 397,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 398,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 399,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 400,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 401,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 402,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 403,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 404,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 405,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 406,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 407,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 408,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 409,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 410,
                        'parameterName': 'Preset Freq 0',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 411,
                        'parameterName': 'Preset Freq 1',
                        'defaultParameterValue': '500',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 412,
                        'parameterName': 'Preset Freq 2',
                        'defaultParameterValue': '1000',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 413,
                        'parameterName': 'Preset Freq 3',
                        'defaultParameterValue': '2000',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 414,
                        'parameterName': 'Preset Freq 4',
                        'defaultParameterValue': '3000',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 415,
                        'parameterName': 'Preset Freq 5',
                        'defaultParameterValue': '4000',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 416,
                        'parameterName': 'Preset Freq 6',
                        'defaultParameterValue': '5000',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 417,
                        'parameterName': 'Preset Freq 7',
                        'defaultParameterValue': '6000',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 418,
                        'parameterName': 'Preset Freq 8',
                        'defaultParameterValue': '6000',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 419,
                        'parameterName': 'Preset Freq 9',
                        'defaultParameterValue': '6000',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 420,
                        'parameterName': 'Preset Freq 10',
                        'defaultParameterValue': '6000',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 421,
                        'parameterName': 'Preset Freq 11',
                        'defaultParameterValue': '6000',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 422,
                        'parameterName': 'Preset Freq 12',
                        'defaultParameterValue': '6000',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 423,
                        'parameterName': 'Preset Freq 13',
                        'defaultParameterValue': '6000',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 424,
                        'parameterName': 'Preset Freq 14',
                        'defaultParameterValue': '6000',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 425,
                        'parameterName': 'Preset Freq 15',
                        'defaultParameterValue': '6000',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 426,
                        'parameterName': 'Keypad Freq',
                        'defaultParameterValue': '6000',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 427,
                        'parameterName': 'MOP Freq',
                        'defaultParameterValue': '6000',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 428,
                        'parameterName': 'MOP Reset Sel',
                        'defaultParameterValue': '1',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 429,
                        'parameterName': 'MOP Preload',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 430,
                        'parameterName': 'MOP Time',
                        'defaultParameterValue': '100',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 431,
                        'parameterName': 'Jog Frequency',
                        'defaultParameterValue': '1000',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 432,
                        'parameterName': 'Jog Accel/Decel',
                        'defaultParameterValue': '1000',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 433,
                        'parameterName': 'Purge Frequency',
                        'defaultParameterValue': '500',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 434,
                        'parameterName': 'DC Brake Time',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 435,
                        'parameterName': 'DC Brake Level',
                        'defaultParameterValue': '7',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 436,
                        'parameterName': 'DC Brk Time@Strt',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 437,
                        'parameterName': 'DB Resistor Sel',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 438,
                        'parameterName': 'DB Threshold',
                        'defaultParameterValue': '1000',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 439,
                        'parameterName': 'S Curve %',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 440,
                        'parameterName': 'PWM Frequency',
                        'defaultParameterValue': '40',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 441,
                        'parameterName': 'Droop Hertz@ FLA',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 442,
                        'parameterName': 'Accel Time 2',
                        'defaultParameterValue': '1000',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 443,
                        'parameterName': 'Decel Time 2',
                        'defaultParameterValue': '1000',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 444,
                        'parameterName': 'Accel Time 3',
                        'defaultParameterValue': '1000',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 445,
                        'parameterName': 'Decel Time 3',
                        'defaultParameterValue': '1000',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 446,
                        'parameterName': 'Accel Time 4',
                        'defaultParameterValue': '1000',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 447,
                        'parameterName': 'Decel Time 4',
                        'defaultParameterValue': '1000',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 448,
                        'parameterName': 'Skip Frequency 1',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 449,
                        'parameterName': 'Skip Freq Band 1',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 450,
                        'parameterName': 'Skip Frequency 2',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 451,
                        'parameterName': 'Skip Freq Band 2',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 452,
                        'parameterName': 'Skip Frequency 3',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 453,
                        'parameterName': 'Skip Freq Band 3',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 454,
                        'parameterName': 'Skip Frequency 4',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 455,
                        'parameterName': 'Skip Freq Band 4',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 456,
                        'parameterName': 'PID 1 Trim Hi',
                        'defaultParameterValue': '600',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 457,
                        'parameterName': 'PID 1 Trim Lo',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 458,
                        'parameterName': 'PID 1 Trim Sel',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 459,
                        'parameterName': 'PID 1 Ref Sel',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 460,
                        'parameterName': 'PID 1 Fdback Sel',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 461,
                        'parameterName': 'PID 1 Prop Gain',
                        'defaultParameterValue': '1',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 462,
                        'parameterName': 'PID 1 Integ Time',
                        'defaultParameterValue': '20',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 463,
                        'parameterName': 'PID 1 Diff Rate',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 464,
                        'parameterName': 'PID 1 Setpoint',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 465,
                        'parameterName': 'PID 1 Deadband',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 466,
                        'parameterName': 'PID 1 Preload',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 467,
                        'parameterName': 'PID 1 Invert Err',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 468,
                        'parameterName': 'PID 2 Trim Hi',
                        'defaultParameterValue': '600',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 469,
                        'parameterName': 'PID 2 Trim Lo',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 470,
                        'parameterName': 'PID 2 Trim Sel',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 471,
                        'parameterName': 'PID 2 Ref Sel',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 472,
                        'parameterName': 'PID 2 Fdback Sel',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 473,
                        'parameterName': 'PID 2 Prop Gain',
                        'defaultParameterValue': '1',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 474,
                        'parameterName': 'PID 2 Integ Time',
                        'defaultParameterValue': '20',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 475,
                        'parameterName': 'PID 2 Diff Rate',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 476,
                        'parameterName': 'PID 2 Setpoint',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 477,
                        'parameterName': 'PID 2 Deadband',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 478,
                        'parameterName': 'PID 2 Preload',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 479,
                        'parameterName': 'PID 2 Invert Err',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 480,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 481,
                        'parameterName': 'Process Disp Lo',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 482,
                        'parameterName': 'Process Disp Hi',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 483,
                        'parameterName': 'Testpoint Sel',
                        'defaultParameterValue': '0000010000000000',
                        'recordValue': false,
                        'parameterType': '0g=='
                    },
                    {
                        'parameterNumber': 484,
                        'parameterName': 'Current Limit 1',
                        'defaultParameterValue': '195',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 485,
                        'parameterName': 'Current Limit 2',
                        'defaultParameterValue': '143',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 486,
                        'parameterName': 'Shear Pin1 Level',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 487,
                        'parameterName': 'Shear Pin 1 Time',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 488,
                        'parameterName': 'Shear Pin2 Level',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 489,
                        'parameterName': 'Shear Pin 2 Time',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 490,
                        'parameterName': 'Load Loss Level',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 491,
                        'parameterName': 'Load Loss Time',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 492,
                        'parameterName': 'Stall Fault Time',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 493,
                        'parameterName': 'Motor OL Select',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 494,
                        'parameterName': 'Motor OL Ret',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 495,
                        'parameterName': 'Drive OL Mode',
                        'defaultParameterValue': '3',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 496,
                        'parameterName': 'IR Voltage Drop',
                        'defaultParameterValue': '43',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 497,
                        'parameterName': 'Flux Current Ref',
                        'defaultParameterValue': '403',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 498,
                        'parameterName': 'Motor Rr',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 499,
                        'parameterName': 'Motor Lm',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 500,
                        'parameterName': 'Motor Lx',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 501,
                        'parameterName': 'PM IR Voltage',
                        'defaultParameterValue': '1150',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 502,
                        'parameterName': 'PM IXd Voltage',
                        'defaultParameterValue': '1791',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 503,
                        'parameterName': 'PM IXq Voltage',
                        'defaultParameterValue': '5321',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 504,
                        'parameterName': 'PM BEMF Voltage',
                        'defaultParameterValue': '3280',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 505,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 506,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 507,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 508,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 509,
                        'parameterName': 'Speed Reg Sel',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 510,
                        'parameterName': 'Freq 1',
                        'defaultParameterValue': '833',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 511,
                        'parameterName': 'Freq 1 BW',
                        'defaultParameterValue': '10',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 512,
                        'parameterName': 'Freq 2',
                        'defaultParameterValue': '1500',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 513,
                        'parameterName': 'Freq 2 BW',
                        'defaultParameterValue': '10',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 514,
                        'parameterName': 'Freq 3',
                        'defaultParameterValue': '2000',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 515,
                        'parameterName': 'Freq 3 BW',
                        'defaultParameterValue': '10',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 516,
                        'parameterName': 'PM Initial Sel',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 517,
                        'parameterName': 'PM DC Inject Cur',
                        'defaultParameterValue': '30',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 518,
                        'parameterName': 'PM Align Time',
                        'defaultParameterValue': '7',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 519,
                        'parameterName': 'PM HFI NS Cur',
                        'defaultParameterValue': '100',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 520,
                        'parameterName': 'PM Bus Reg Kd',
                        'defaultParameterValue': '2',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 521,
                        'parameterName': 'Freq 1 Kp',
                        'defaultParameterValue': '1000',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 522,
                        'parameterName': 'Freq 1 Ki',
                        'defaultParameterValue': '100',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 523,
                        'parameterName': 'Freq 2 Kp',
                        'defaultParameterValue': '1000',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 524,
                        'parameterName': 'Freq 2 Ki',
                        'defaultParameterValue': '100',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 525,
                        'parameterName': 'Freq 3 Kp',
                        'defaultParameterValue': '1000',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 526,
                        'parameterName': 'Freq 3 Ki',
                        'defaultParameterValue': '100',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 527,
                        'parameterName': 'PM FWKn 1 Kp',
                        'defaultParameterValue': '350',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 528,
                        'parameterName': 'PM FWKn 2 Kp',
                        'defaultParameterValue': '300',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 529,
                        'parameterName': 'PM Control Cfg',
                        'defaultParameterValue': '0000000000000111',
                        'recordValue': true,
                        'parameterType': '0g=='
                    },
                    {
                        'parameterNumber': 530,
                        'parameterName': 'Boost Select',
                        'defaultParameterValue': '7',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 531,
                        'parameterName': 'Start Boost',
                        'defaultParameterValue': '25',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 532,
                        'parameterName': 'Break Voltage',
                        'defaultParameterValue': '250',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 533,
                        'parameterName': 'Break Frequency',
                        'defaultParameterValue': '150',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 534,
                        'parameterName': 'Maximum Voltage',
                        'defaultParameterValue': '460',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 535,
                        'parameterName': 'Motor Fdbk Type',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 536,
                        'parameterName': 'Encoder PPR',
                        'defaultParameterValue': '1024',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 537,
                        'parameterName': 'Pulse In Scale',
                        'defaultParameterValue': '64',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 538,
                        'parameterName': 'Ki Speed Loop',
                        'defaultParameterValue': '20',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 539,
                        'parameterName': 'Kp Speed Loop',
                        'defaultParameterValue': '5',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 540,
                        'parameterName': 'Var PWM Disable',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 541,
                        'parameterName': 'Auto Rstrt Tries',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 542,
                        'parameterName': 'Auto Rstrt Delay',
                        'defaultParameterValue': '10',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 543,
                        'parameterName': 'Start At PowerUp',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 544,
                        'parameterName': 'Reverse Disable',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 545,
                        'parameterName': 'Flying Start En',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 546,
                        'parameterName': 'FlyStrt CurLimit',
                        'defaultParameterValue': '65',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 547,
                        'parameterName': 'Compensation',
                        'defaultParameterValue': '1',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 548,
                        'parameterName': 'Power Loss Mode',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 549,
                        'parameterName': 'Half Bus Enable',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 550,
                        'parameterName': 'Bus Reg Enable',
                        'defaultParameterValue': '1',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 551,
                        'parameterName': 'Fault Clear',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 552,
                        'parameterName': 'Program Lock',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 553,
                        'parameterName': 'Program Lock Mod',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 554,
                        'parameterName': 'Drv Ambient Sel',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 555,
                        'parameterName': 'Reset Meters',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 556,
                        'parameterName': 'Text Scroll',
                        'defaultParameterValue': '2',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 557,
                        'parameterName': 'Out Phas Loss En',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 558,
                        'parameterName': 'Positioning Mode',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 559,
                        'parameterName': 'Counts Per Unit',
                        'defaultParameterValue': '4096',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 560,
                        'parameterName': 'Enh Control Word',
                        'defaultParameterValue': '0000000000000000',
                        'recordValue': true,
                        'parameterType': '0g=='
                    },
                    {
                        'parameterNumber': 561,
                        'parameterName': 'Home Save',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 562,
                        'parameterName': 'Find Home Freq',
                        'defaultParameterValue': '100',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 563,
                        'parameterName': 'Find Home Dir',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 564,
                        'parameterName': 'Encoder Pos Tol',
                        'defaultParameterValue': '100',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 565,
                        'parameterName': 'Pos Reg Filter',
                        'defaultParameterValue': '8',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 566,
                        'parameterName': 'Pos Reg Gain',
                        'defaultParameterValue': '30',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 567,
                        'parameterName': 'Max Traverse',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 568,
                        'parameterName': 'Traverse Inc',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 569,
                        'parameterName': 'Traverse Dec',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 570,
                        'parameterName': 'P Jump',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 571,
                        'parameterName': 'Sync Time',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 572,
                        'parameterName': 'Speed Ratio',
                        'defaultParameterValue': '100',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 573,
                        'parameterName': 'Mtr Options Cfg',
                        'defaultParameterValue': '0000000000000011',
                        'recordValue': true,
                        'parameterType': '0g=='
                    },
                    {
                        'parameterNumber': 574,
                        'parameterName': 'RdyBit Mode Cfg',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 575,
                        'parameterName': 'Flux Braking En',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 576,
                        'parameterName': 'Phase Loss Level',
                        'defaultParameterValue': '250',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 577,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 578,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 579,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 580,
                        'parameterName': 'Current Loop BW',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 581,
                        'parameterName': 'PM Stable 1 Freq',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 582,
                        'parameterName': 'PM Stable 2 Freq',
                        'defaultParameterValue': '45',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 583,
                        'parameterName': 'PM Stable 1 Kp',
                        'defaultParameterValue': '40',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 584,
                        'parameterName': 'PM Stable 2 Kp',
                        'defaultParameterValue': '250',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 585,
                        'parameterName': 'PM Stable Brk Pt',
                        'defaultParameterValue': '40',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 586,
                        'parameterName': 'PM Stepload Kp',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 587,
                        'parameterName': 'PM 1 Efficiency',
                        'defaultParameterValue': '120',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 588,
                        'parameterName': 'PM 2 Efficiency',
                        'defaultParameterValue': '500',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 589,
                        'parameterName': 'PM Algor Sel',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 590,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '1000',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 591,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '35',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 592,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '30',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 593,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '100',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 594,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '100',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 595,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '10',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 596,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '10',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 597,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 598,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 599,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 600,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 601,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 602,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 603,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 604,
                        'parameterName': 'Fault 4 Code',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 605,
                        'parameterName': 'Fault 5 Code',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 606,
                        'parameterName': 'Fault 6 Code',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 607,
                        'parameterName': 'Fault 7 Code',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 608,
                        'parameterName': 'Fault 8 Code',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 609,
                        'parameterName': 'Fault 9 Code',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 610,
                        'parameterName': 'Fault10 Code',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 611,
                        'parameterName': 'Fault 1 Time-hr',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 612,
                        'parameterName': 'Fault 2 Time-hr',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 613,
                        'parameterName': 'Fault 3 Time-hr',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 614,
                        'parameterName': 'Fault 4 Time-hr',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 615,
                        'parameterName': 'Fault 5 Time-hr',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 616,
                        'parameterName': 'Fault 6 Time-hr',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 617,
                        'parameterName': 'Fault 7 Time-hr',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 618,
                        'parameterName': 'Fault 8 Time-hr',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 619,
                        'parameterName': 'Fault 9 Time-hr',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 620,
                        'parameterName': 'Fault10 Time-hr',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 621,
                        'parameterName': 'Fault 1 Time-min',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 622,
                        'parameterName': 'Fault 2 Time-min',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 623,
                        'parameterName': 'Fault 3 Time-min',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 624,
                        'parameterName': 'Fault 4 Time-min',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 625,
                        'parameterName': 'Fault 5 Time-min',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 626,
                        'parameterName': 'Fault 6 Time-min',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 627,
                        'parameterName': 'Fault 7 Time-min',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 628,
                        'parameterName': 'Fault 8 Time-min',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 629,
                        'parameterName': 'Fault 9 Time-min',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 630,
                        'parameterName': 'Fault10 Time-min',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 631,
                        'parameterName': 'Fault 1 Freq',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 632,
                        'parameterName': 'Fault 2 Freq',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 633,
                        'parameterName': 'Fault 3 Freq',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 634,
                        'parameterName': 'Fault 4 Freq',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 635,
                        'parameterName': 'Fault 5 Freq',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 636,
                        'parameterName': 'Fault 6 Freq',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 637,
                        'parameterName': 'Fault 7 Freq',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 638,
                        'parameterName': 'Fault 8 Freq',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 639,
                        'parameterName': 'Fault 9 Freq',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 640,
                        'parameterName': 'Fault10 Freq',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 641,
                        'parameterName': 'Fault 1 Current',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 642,
                        'parameterName': 'Fault 2 Current',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 643,
                        'parameterName': 'Fault 3 Current',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 644,
                        'parameterName': 'Fault 4 Current',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 645,
                        'parameterName': 'Fault 5 Current',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 646,
                        'parameterName': 'Fault 6 Current',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 647,
                        'parameterName': 'Fault 7 Current',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 648,
                        'parameterName': 'Fault 8 Current',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 649,
                        'parameterName': 'Fault 9 Current',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 650,
                        'parameterName': 'Fault10 Current',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 651,
                        'parameterName': 'Fault 1 BusVolts',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 652,
                        'parameterName': 'Fault 2 BusVolts',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 653,
                        'parameterName': 'Fault 3 BusVolts',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 654,
                        'parameterName': 'Fault 4 BusVolts',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 655,
                        'parameterName': 'Fault 5 BusVolts',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 656,
                        'parameterName': 'Fault 6 BusVolts',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 657,
                        'parameterName': 'Fault 7 BusVolts',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 658,
                        'parameterName': 'Fault 8 BusVolts',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 659,
                        'parameterName': 'Fault 9 BusVolts',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 660,
                        'parameterName': 'Fault10 BusVolts',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 661,
                        'parameterName': 'Status @ Fault 1',
                        'defaultParameterValue': '0000000000000000',
                        'recordValue': false,
                        'parameterType': '0g=='
                    },
                    {
                        'parameterNumber': 662,
                        'parameterName': 'Status @ Fault 2',
                        'defaultParameterValue': '0000000000000000',
                        'recordValue': false,
                        'parameterType': '0g=='
                    },
                    {
                        'parameterNumber': 663,
                        'parameterName': 'Status @ Fault 3',
                        'defaultParameterValue': '0000000000000000',
                        'recordValue': false,
                        'parameterType': '0g=='
                    },
                    {
                        'parameterNumber': 664,
                        'parameterName': 'Status @ Fault 4',
                        'defaultParameterValue': '0000000000000000',
                        'recordValue': false,
                        'parameterType': '0g=='
                    },
                    {
                        'parameterNumber': 665,
                        'parameterName': 'Status @ Fault 5',
                        'defaultParameterValue': '0000000000000000',
                        'recordValue': false,
                        'parameterType': '0g=='
                    },
                    {
                        'parameterNumber': 666,
                        'parameterName': 'Status @ Fault 6',
                        'defaultParameterValue': '0000000000000000',
                        'recordValue': false,
                        'parameterType': '0g=='
                    },
                    {
                        'parameterNumber': 667,
                        'parameterName': 'Status @ Fault 7',
                        'defaultParameterValue': '0000000000000000',
                        'recordValue': false,
                        'parameterType': '0g=='
                    },
                    {
                        'parameterNumber': 668,
                        'parameterName': 'Status @ Fault 8',
                        'defaultParameterValue': '0000000000000000',
                        'recordValue': false,
                        'parameterType': '0g=='
                    },
                    {
                        'parameterNumber': 669,
                        'parameterName': 'Status @ Fault 9',
                        'defaultParameterValue': '0000000000000000',
                        'recordValue': false,
                        'parameterType': '0g=='
                    },
                    {
                        'parameterNumber': 670,
                        'parameterName': 'Status @ Fault10',
                        'defaultParameterValue': '0000000000000000',
                        'recordValue': false,
                        'parameterType': '0g=='
                    },
                    {
                        'parameterNumber': 671,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 672,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 673,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 674,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 675,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 676,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 677,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 678,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 679,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 680,
                        'parameterName': 'Reserved',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 681,
                        'parameterName': 'Comm Sts - DSI',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 682,
                        'parameterName': 'Comm Sts - Opt',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 683,
                        'parameterName': 'Com Sts-Emb Enet',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 684,
                        'parameterName': 'EN Addr Src',
                        'defaultParameterValue': '2',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 685,
                        'parameterName': 'EN Rate Act',
                        'defaultParameterValue': '0',
                        'recordValue': true,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 686,
                        'parameterName': 'DSI I/O Act',
                        'defaultParameterValue': '0000000000000000',
                        'recordValue': true,
                        'parameterType': '0g=='
                    },
                    {
                        'parameterNumber': 687,
                        'parameterName': 'HW Addr 1',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 688,
                        'parameterName': 'HW Addr 2',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 689,
                        'parameterName': 'HW Addr 3',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 690,
                        'parameterName': 'HW Addr 4',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 691,
                        'parameterName': 'HW Addr 5',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 692,
                        'parameterName': 'HW Addr 6',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 693,
                        'parameterName': 'EN IP Addr Act 1',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 694,
                        'parameterName': 'EN IP Addr Act 2',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 695,
                        'parameterName': 'EN IP Addr Act 3',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 696,
                        'parameterName': 'EN IP Addr Act 4',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 697,
                        'parameterName': 'EN Subnet Act 1',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 698,
                        'parameterName': 'EN Subnet Act 2',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 699,
                        'parameterName': 'EN Subnet Act 3',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 700,
                        'parameterName': 'EN Subnet Act 4',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 701,
                        'parameterName': 'EN Gateway Act 1',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 702,
                        'parameterName': 'EN Gateway Act 2',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 703,
                        'parameterName': 'EN Gateway Act 3',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 704,
                        'parameterName': 'EN Gateway Act 4',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 705,
                        'parameterName': 'Drv 0 Logic Cmd',
                        'defaultParameterValue': '0000000000000000',
                        'recordValue': false,
                        'parameterType': '0g=='
                    },
                    {
                        'parameterNumber': 706,
                        'parameterName': 'Drv 0 Reference',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 707,
                        'parameterName': 'Drv 0 Logic Sts',
                        'defaultParameterValue': '0000000000000000',
                        'recordValue': false,
                        'parameterType': '0g=='
                    },
                    {
                        'parameterNumber': 708,
                        'parameterName': 'Drv 0 Feedback',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 709,
                        'parameterName': 'Drv 1 Logic Cmd',
                        'defaultParameterValue': '0000000000000000',
                        'recordValue': false,
                        'parameterType': '0g=='
                    },
                    {
                        'parameterNumber': 710,
                        'parameterName': 'Drv 1 Reference',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 711,
                        'parameterName': 'Drv 1 Logic Sts',
                        'defaultParameterValue': '0000000000000000',
                        'recordValue': false,
                        'parameterType': '0g=='
                    },
                    {
                        'parameterNumber': 712,
                        'parameterName': 'Drv 1 Feedback',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 713,
                        'parameterName': 'Drv 2 Logic Cmd',
                        'defaultParameterValue': '0000000000000000',
                        'recordValue': false,
                        'parameterType': '0g=='
                    },
                    {
                        'parameterNumber': 714,
                        'parameterName': 'Drv 2 Reference',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 715,
                        'parameterName': 'Drv 2 Logic Sts',
                        'defaultParameterValue': '0000000000000000',
                        'recordValue': false,
                        'parameterType': '0g=='
                    },
                    {
                        'parameterNumber': 716,
                        'parameterName': 'Drv 2 Feedback',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 717,
                        'parameterName': 'Drv 3 Logic Cmd',
                        'defaultParameterValue': '0000000000000000',
                        'recordValue': false,
                        'parameterType': '0g=='
                    },
                    {
                        'parameterNumber': 718,
                        'parameterName': 'Drv 3 Reference',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 719,
                        'parameterName': 'Drv 3 Logic Sts',
                        'defaultParameterValue': '0000000000000000',
                        'recordValue': false,
                        'parameterType': '0g=='
                    },
                    {
                        'parameterNumber': 720,
                        'parameterName': 'Drv 3 Feedback',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 721,
                        'parameterName': 'Drv 4 Logic Cmd',
                        'defaultParameterValue': '0000000000000000',
                        'recordValue': false,
                        'parameterType': '0g=='
                    },
                    {
                        'parameterNumber': 722,
                        'parameterName': 'Drv 4 Reference',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 723,
                        'parameterName': 'Drv 4 Logic Sts',
                        'defaultParameterValue': '0000000000000000',
                        'recordValue': false,
                        'parameterType': '0g=='
                    },
                    {
                        'parameterNumber': 724,
                        'parameterName': 'Drv 4 Feedback',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 725,
                        'parameterName': 'EN Rx Overruns',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 726,
                        'parameterName': 'EN Rx Packets',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 727,
                        'parameterName': 'EN Rx Errors',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 728,
                        'parameterName': 'EN Tx Packets',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 729,
                        'parameterName': 'EN Tx Errors',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 730,
                        'parameterName': 'EN Missed IO Pkt',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    },
                    {
                        'parameterNumber': 731,
                        'parameterName': 'DSI Errors',
                        'defaultParameterValue': '0',
                        'recordValue': false,
                        'parameterType': 'xw=='
                    }
                    ]";
    }
}