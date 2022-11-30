using Newtonsoft.Json;
namespace powerFlexBackup.powerFlexDrive
{
    public class PowerFlex525 : powerFlexDrive{
        public PowerFlex525(String address, IdentityObject identityObject, Sres.Net.EEIP.EEIPClient eeipClient)
        {
            setDriveAddress(address);
            setEEIPClient(eeipClient);
            setDriveIdentiyObject(identityObject);
            setDriveDriveParameterList(JsonConvert.DeserializeObject<DriveParameterList>(parameterListJSON));      
            removeNonRecordedDriveParameterList();      
        }

        /* Process the parameter from a bytearray to an int based on type
        0xC2 = SINT (8-bits)
        0xC3 = INT (16-bits)
        0xC4 = DINT (32-bits)
        0xC6 = USINT (8-bits)
        0xC7 = UINT (16-bits)
        0xCA = REAL (32-bits)
        0xD2 = WORD (16-bits) */
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

        public string parameterListJSON = @"
                {'driveParameters': [
                    {
                    'parameterNumber': 1,
                    'parameterName': 'Output Freq',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 2,
                    'parameterName': 'Commanded Freq',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 3,
                    'parameterName': 'Output Current',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 4,
                    'parameterName': 'Output Voltage',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 5,
                    'parameterName': 'DC Bus Voltage',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 6,
                    'parameterName': 'Drive Status',
                    'parameterType': '0g==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 7,
                    'parameterName': 'Fault 1 Code',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 8,
                    'parameterName': 'Fault 2 Code',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 9,
                    'parameterName': 'Fault 3 Code',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 10,
                    'parameterName': 'Process Display',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 11,
                    'parameterName': 'Process Fract',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 12,
                    'parameterName': 'Control Source',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 13,
                    'parameterName': 'Contrl In Status',
                    'parameterType': '0g==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 14,
                    'parameterName': 'Dig In Status',
                    'parameterType': '0g==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 15,
                    'parameterName': 'Output RPM',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 16,
                    'parameterName': 'Output Speed',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 17,
                    'parameterName': 'Output Power',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 18,
                    'parameterName': 'Power Saved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 19,
                    'parameterName': 'Elapsed Run Time',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 20,
                    'parameterName': 'Average Power',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 21,
                    'parameterName': 'Elapsed kWh',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 22,
                    'parameterName': 'Elapsed MWh',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 23,
                    'parameterName': 'Energy Saved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 24,
                    'parameterName': 'Accum kWh Sav',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 25,
                    'parameterName': 'Accum Cost Sav',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 26,
                    'parameterName': 'Accum CO2 Sav',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 27,
                    'parameterName': 'Drive Temp',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 28,
                    'parameterName': 'Control Temp',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 29,
                    'parameterName': 'Control SW Ver',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 30,
                    'parameterName': 'Language',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 31,
                    'parameterName': 'Motor NP Volts',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 32,
                    'parameterName': 'Motor NP Hertz',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 33,
                    'parameterName': 'Motor OL Current',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 34,
                    'parameterName': 'Motor NP FLA',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 35,
                    'parameterName': 'Motor NP Poles',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 36,
                    'parameterName': 'Motor NP RPM',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 37,
                    'parameterName': 'Motor NP Power',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 38,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 39,
                    'parameterName': 'Torque Perf Mode',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 40,
                    'parameterName': 'Autotune',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 41,
                    'parameterName': 'Accel Time 1',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 42,
                    'parameterName': 'Decel Time 1',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 43,
                    'parameterName': 'Minimum Freq',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 44,
                    'parameterName': 'Maximum Freq',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 45,
                    'parameterName': 'Stop Mode',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 46,
                    'parameterName': 'Start Source 1',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 47,
                    'parameterName': 'Speed Reference1',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 48,
                    'parameterName': 'Start Source 2',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 49,
                    'parameterName': 'Speed Reference2',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 50,
                    'parameterName': 'Start Source 3',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 51,
                    'parameterName': 'Speed Reference3',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 52,
                    'parameterName': 'Average kWh Cost',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 53,
                    'parameterName': 'Reset To Defalts',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 54,
                    'parameterName': 'Display Param',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 55,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 56,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 57,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 58,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 59,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 60,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 61,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 62,
                    'parameterName': 'DigIn TermBlk 02',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 63,
                    'parameterName': 'DigIn TermBlk 03',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 64,
                    'parameterName': '2-Wire Mode',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 65,
                    'parameterName': 'DigIn TermBlk 05',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 66,
                    'parameterName': 'DigIn TermBlk 06',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 67,
                    'parameterName': 'DigIn TermBlk 07',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 68,
                    'parameterName': 'DigIn TermBlk 08',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 69,
                    'parameterName': 'Opto Out1 Sel',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 70,
                    'parameterName': 'Opto Out1 Level',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 71,
                    'parameterName': 'Opto Out1 LevelF',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 72,
                    'parameterName': 'Opto Out2 Sel',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 73,
                    'parameterName': 'Opto Out2 Level',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 74,
                    'parameterName': 'Opto Out2 LevelF',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 75,
                    'parameterName': 'Opto Out Logic',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 76,
                    'parameterName': 'Relay Out1 Sel',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 77,
                    'parameterName': 'Relay Out1 Level',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 78,
                    'parameterName': 'RelayOut1 LevelF',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 79,
                    'parameterName': 'Relay 1 On Time',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 80,
                    'parameterName': 'Relay 1 Off Time',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 81,
                    'parameterName': 'Relay Out2 Sel',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 82,
                    'parameterName': 'Relay Out2 Level',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 83,
                    'parameterName': 'RelayOut2 LevelF',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 84,
                    'parameterName': 'Relay 2 On Time',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 85,
                    'parameterName': 'Relay 2 Off Time',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 86,
                    'parameterName': 'EM Brk Off Delay',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 87,
                    'parameterName': 'EM Brk On Delay',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 88,
                    'parameterName': 'Analog Out Sel',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 89,
                    'parameterName': 'Analog Out High',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 90,
                    'parameterName': 'Anlg Out Setpt',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 91,
                    'parameterName': 'Anlg In 0-10V Lo',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 92,
                    'parameterName': 'Anlg In 0-10V Hi',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 93,
                    'parameterName': '10V Bipolar Enbl',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 94,
                    'parameterName': 'Anlg In V Loss',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 95,
                    'parameterName': 'Anlg In4-20mA Lo',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 96,
                    'parameterName': 'Anlg In4-20mA Hi',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 97,
                    'parameterName': 'Anlg In mA Loss',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 98,
                    'parameterName': 'Anlg Loss Delay',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 99,
                    'parameterName': 'Analog In Filter',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 100,
                    'parameterName': 'Sleep-Wake Sel',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 101,
                    'parameterName': 'Sleep Level',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 102,
                    'parameterName': 'Sleep Time',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 103,
                    'parameterName': 'Wake Level',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 104,
                    'parameterName': 'Wake Time',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 105,
                    'parameterName': 'Safety Open En',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 106,
                    'parameterName': 'SafetyFlt RstCfg',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 107,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 108,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 109,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 110,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 111,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 112,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 113,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 114,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 115,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 116,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 117,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 118,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 119,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 120,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 121,
                    'parameterName': 'Comm Write Mode',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 122,
                    'parameterName': 'Cmd Stat Select',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 123,
                    'parameterName': 'RS485 Data Rate',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 124,
                    'parameterName': 'RS485 Node Addr',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 125,
                    'parameterName': 'Comm Loss Action',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 126,
                    'parameterName': 'Comm Loss Time',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 127,
                    'parameterName': 'RS485 Format',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 128,
                    'parameterName': 'EN Addr Sel',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 129,
                    'parameterName': 'EN IP Addr Cfg 1',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 130,
                    'parameterName': 'EN IP Addr Cfg 2',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 131,
                    'parameterName': 'EN IP Addr Cfg 3',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 132,
                    'parameterName': 'EN IP Addr Cfg 4',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 133,
                    'parameterName': 'EN Subnet Cfg 1',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 134,
                    'parameterName': 'EN Subnet Cfg 2',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 135,
                    'parameterName': 'EN Subnet Cfg 3',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 136,
                    'parameterName': 'EN Subnet Cfg 4',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 137,
                    'parameterName': 'EN Gateway Cfg 1',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 138,
                    'parameterName': 'EN Gateway Cfg 2',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 139,
                    'parameterName': 'EN Gateway Cfg 3',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 140,
                    'parameterName': 'EN Gateway Cfg 4',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 141,
                    'parameterName': 'EN Rate Cfg',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 142,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 143,
                    'parameterName': 'EN Comm Flt Actn',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 144,
                    'parameterName': 'EN Idle Flt Actn',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 145,
                    'parameterName': 'EN Flt Cfg Logic',
                    'parameterType': '0g==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 146,
                    'parameterName': 'EN Flt Cfg Ref',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 147,
                    'parameterName': 'EN Flt Cfg DL 1',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 148,
                    'parameterName': 'EN Flt Cfg DL 2',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 149,
                    'parameterName': 'EN Flt Cfg DL 3',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 150,
                    'parameterName': 'EN Flt Cfg DL 4',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 151,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 152,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 153,
                    'parameterName': 'EN Data In 1',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 154,
                    'parameterName': 'EN Data In 2',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 155,
                    'parameterName': 'EN Data In 3',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 156,
                    'parameterName': 'EN Data In 4',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 157,
                    'parameterName': 'EN Data Out 1',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 158,
                    'parameterName': 'EN Data Out 2',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 159,
                    'parameterName': 'EN Data Out 3',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 160,
                    'parameterName': 'EN Data Out 4',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 161,
                    'parameterName': 'Opt Data In 1',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 162,
                    'parameterName': 'Opt Data In 2',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 163,
                    'parameterName': 'Opt Data In 3',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 164,
                    'parameterName': 'Opt Data In 4',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 165,
                    'parameterName': 'Opt Data Out 1',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 166,
                    'parameterName': 'Opt Data Out 2',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 167,
                    'parameterName': 'Opt Data Out 3',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 168,
                    'parameterName': 'Opt Data Out 4',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 169,
                    'parameterName': 'MultiDrv Sel',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 170,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 171,
                    'parameterName': 'Drv 1 Addr',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 172,
                    'parameterName': 'Drv 2 Addr',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 173,
                    'parameterName': 'Drv 3 Addr',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 174,
                    'parameterName': 'Drv 4 Addr',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 175,
                    'parameterName': 'DSI I\/O Cfg',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 176,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 177,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 178,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 179,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 180,
                    'parameterName': 'Stp Logic 0',
                    'parameterType': '0g==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 181,
                    'parameterName': 'Stp Logic 1',
                    'parameterType': '0g==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 182,
                    'parameterName': 'Stp Logic 2',
                    'parameterType': '0g==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 183,
                    'parameterName': 'Stp Logic 3',
                    'parameterType': '0g==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 184,
                    'parameterName': 'Stp Logic 4',
                    'parameterType': '0g==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 185,
                    'parameterName': 'Stp Logic 5',
                    'parameterType': '0g==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 186,
                    'parameterName': 'Stp Logic 6',
                    'parameterType': '0g==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 187,
                    'parameterName': 'Stp Logic 7',
                    'parameterType': '0g==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 188,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 189,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 190,
                    'parameterName': 'Stp Logic Time 0',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 191,
                    'parameterName': 'Stp Logic Time 1',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 192,
                    'parameterName': 'Stp Logic Time 2',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 193,
                    'parameterName': 'Stp Logic Time 3',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 194,
                    'parameterName': 'Stp Logic Time 4',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 195,
                    'parameterName': 'Stp Logic Time 5',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 196,
                    'parameterName': 'Stp Logic Time 6',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 197,
                    'parameterName': 'Stp Logic Time 7',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 198,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 199,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 200,
                    'parameterName': 'Step Units 0',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 201,
                    'parameterName': 'Step Units F 0',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 202,
                    'parameterName': 'Step Units 1',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 203,
                    'parameterName': 'Step Units F 1',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 204,
                    'parameterName': 'Step Units 2',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 205,
                    'parameterName': 'Step Units F 2',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 206,
                    'parameterName': 'Step Units 3',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 207,
                    'parameterName': 'Step Units F 3',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 208,
                    'parameterName': 'Step Units 4',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 209,
                    'parameterName': 'Step Units F 4',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 210,
                    'parameterName': 'Step Units 5',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 211,
                    'parameterName': 'Step Units F 5',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 212,
                    'parameterName': 'Step Units 6',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 213,
                    'parameterName': 'Step Units F 6',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 214,
                    'parameterName': 'Step Units 7',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 215,
                    'parameterName': 'Step Units F 7',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 216,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 217,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 218,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 219,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 220,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 221,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 222,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 223,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 224,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 225,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 226,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 227,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 228,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 229,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 230,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 231,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 232,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 233,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 234,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 235,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 236,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 237,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 238,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 239,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 240,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 241,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 242,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 243,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 244,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 245,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 246,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 247,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 248,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 249,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 250,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 251,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 252,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 253,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 254,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 255,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 256,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 257,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 258,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 259,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 260,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 261,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 262,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 263,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 264,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 265,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 266,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 267,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 268,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 269,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 270,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 271,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 272,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 273,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 274,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 275,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 276,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 277,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 278,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 279,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 280,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 281,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 282,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 283,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 284,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 285,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 286,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 287,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 288,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 289,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 290,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 291,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 292,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 293,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 294,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 295,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 296,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 297,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 298,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 299,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 300,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 301,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 302,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 303,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 304,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 305,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 306,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 307,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 308,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 309,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 310,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 311,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 312,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 313,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 314,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 315,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 316,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 317,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 318,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 319,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 320,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 321,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 322,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 323,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 324,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 325,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 326,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 327,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 328,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 329,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 330,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 331,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 332,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 333,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 334,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 335,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 336,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 337,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 338,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 339,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 340,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 341,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 342,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 343,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 344,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 345,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 346,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 347,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 348,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 349,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 350,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 351,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 352,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 353,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 354,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 355,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 356,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 357,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 358,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 359,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 360,
                    'parameterName': 'Analog In 0-10V',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 361,
                    'parameterName': 'Analog In 4-20mA',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 362,
                    'parameterName': 'Elapsed Time-hr',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 363,
                    'parameterName': 'Elapsed Time-min',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 364,
                    'parameterName': 'Counter Status',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 365,
                    'parameterName': 'Timer Status',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 366,
                    'parameterName': 'Timer StatusF',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 367,
                    'parameterName': 'Drive Type',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 368,
                    'parameterName': 'Testpoint Data',
                    'parameterType': '0g==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 369,
                    'parameterName': 'Motor OL Level',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 370,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 371,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 372,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 373,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 374,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 375,
                    'parameterName': 'Slip Hz Meter',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 376,
                    'parameterName': 'Speed Feedback',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 377,
                    'parameterName': 'Speed Feedback F',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 378,
                    'parameterName': 'Encoder Speed',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 379,
                    'parameterName': 'Encoder Speed F',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 380,
                    'parameterName': 'DC Bus Ripple',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 381,
                    'parameterName': 'Output Powr Fctr',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 382,
                    'parameterName': 'Torque Current',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 383,
                    'parameterName': 'PID1 Fdbk Displ',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 384,
                    'parameterName': 'PID1 Setpnt Disp',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 385,
                    'parameterName': 'PID2 Fdbk Displ',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 386,
                    'parameterName': 'PID2 Setpnt Disp',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 387,
                    'parameterName': 'Position Status',
                    'parameterType': '0g==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 388,
                    'parameterName': 'Units Traveled H',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 389,
                    'parameterName': 'Units Traveled L',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 390,
                    'parameterName': 'Fiber Status',
                    'parameterType': '0g==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 391,
                    'parameterName': 'Stp Logic Status',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 392,
                    'parameterName': 'RdyBit Mode Act',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 393,
                    'parameterName': 'Drive Status 2',
                    'parameterType': '0g==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 394,
                    'parameterName': 'Dig Out Status',
                    'parameterType': '0g==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 395,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 396,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 397,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 398,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 399,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 400,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 401,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 402,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 403,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 404,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 405,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 406,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 407,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 408,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 409,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 410,
                    'parameterName': 'Preset Freq 0',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 411,
                    'parameterName': 'Preset Freq 1',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 412,
                    'parameterName': 'Preset Freq 2',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 413,
                    'parameterName': 'Preset Freq 3',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 414,
                    'parameterName': 'Preset Freq 4',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 415,
                    'parameterName': 'Preset Freq 5',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 416,
                    'parameterName': 'Preset Freq 6',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 417,
                    'parameterName': 'Preset Freq 7',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 418,
                    'parameterName': 'Preset Freq 8',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 419,
                    'parameterName': 'Preset Freq 9',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 420,
                    'parameterName': 'Preset Freq 10',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 421,
                    'parameterName': 'Preset Freq 11',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 422,
                    'parameterName': 'Preset Freq 12',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 423,
                    'parameterName': 'Preset Freq 13',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 424,
                    'parameterName': 'Preset Freq 14',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 425,
                    'parameterName': 'Preset Freq 15',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 426,
                    'parameterName': 'Keypad Freq',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 427,
                    'parameterName': 'MOP Freq',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 428,
                    'parameterName': 'MOP Reset Sel',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 429,
                    'parameterName': 'MOP Preload',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 430,
                    'parameterName': 'MOP Time',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 431,
                    'parameterName': 'Jog Frequency',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 432,
                    'parameterName': 'Jog Accel\/Decel',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 433,
                    'parameterName': 'Purge Frequency',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 434,
                    'parameterName': 'DC Brake Time',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 435,
                    'parameterName': 'DC Brake Level',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 436,
                    'parameterName': 'DC Brk Time@Strt',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 437,
                    'parameterName': 'DB Resistor Sel',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 438,
                    'parameterName': 'DB Threshold',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 439,
                    'parameterName': 'S Curve %',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 440,
                    'parameterName': 'PWM Frequency',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 441,
                    'parameterName': 'Droop Hertz@ FLA',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 442,
                    'parameterName': 'Accel Time 2',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 443,
                    'parameterName': 'Decel Time 2',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 444,
                    'parameterName': 'Accel Time 3',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 445,
                    'parameterName': 'Decel Time 3',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 446,
                    'parameterName': 'Accel Time 4',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 447,
                    'parameterName': 'Decel Time 4',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 448,
                    'parameterName': 'Skip Frequency 1',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 449,
                    'parameterName': 'Skip Freq Band 1',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 450,
                    'parameterName': 'Skip Frequency 2',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 451,
                    'parameterName': 'Skip Freq Band 2',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 452,
                    'parameterName': 'Skip Frequency 3',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 453,
                    'parameterName': 'Skip Freq Band 3',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 454,
                    'parameterName': 'Skip Frequency 4',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 455,
                    'parameterName': 'Skip Freq Band 4',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 456,
                    'parameterName': 'PID 1 Trim Hi',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 457,
                    'parameterName': 'PID 1 Trim Lo',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 458,
                    'parameterName': 'PID 1 Trim Sel',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 459,
                    'parameterName': 'PID 1 Ref Sel',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 460,
                    'parameterName': 'PID 1 Fdback Sel',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 461,
                    'parameterName': 'PID 1 Prop Gain',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 462,
                    'parameterName': 'PID 1 Integ Time',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 463,
                    'parameterName': 'PID 1 Diff Rate',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 464,
                    'parameterName': 'PID 1 Setpoint',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 465,
                    'parameterName': 'PID 1 Deadband',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 466,
                    'parameterName': 'PID 1 Preload',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 467,
                    'parameterName': 'PID 1 Invert Err',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 468,
                    'parameterName': 'PID 2 Trim Hi',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 469,
                    'parameterName': 'PID 2 Trim Lo',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 470,
                    'parameterName': 'PID 2 Trim Sel',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 471,
                    'parameterName': 'PID 2 Ref Sel',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 472,
                    'parameterName': 'PID 2 Fdback Sel',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 473,
                    'parameterName': 'PID 2 Prop Gain',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 474,
                    'parameterName': 'PID 2 Integ Time',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 475,
                    'parameterName': 'PID 2 Diff Rate',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 476,
                    'parameterName': 'PID 2 Setpoint',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 477,
                    'parameterName': 'PID 2 Deadband',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 478,
                    'parameterName': 'PID 2 Preload',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 479,
                    'parameterName': 'PID 2 Invert Err',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 480,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 481,
                    'parameterName': 'Process Disp Lo',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 482,
                    'parameterName': 'Process Disp Hi',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 483,
                    'parameterName': 'Testpoint Sel',
                    'parameterType': '0g==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 484,
                    'parameterName': 'Current Limit 1',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 485,
                    'parameterName': 'Current Limit 2',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 486,
                    'parameterName': 'Shear Pin1 Level',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 487,
                    'parameterName': 'Shear Pin 1 Time',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 488,
                    'parameterName': 'Shear Pin2 Level',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 489,
                    'parameterName': 'Shear Pin 2 Time',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 490,
                    'parameterName': 'Load Loss Level',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 491,
                    'parameterName': 'Load Loss Time',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 492,
                    'parameterName': 'Stall Fault Time',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 493,
                    'parameterName': 'Motor OL Select',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 494,
                    'parameterName': 'Motor OL Ret',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 495,
                    'parameterName': 'Drive OL Mode',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 496,
                    'parameterName': 'IR Voltage Drop',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 497,
                    'parameterName': 'Flux Current Ref',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 498,
                    'parameterName': 'Motor Rr',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 499,
                    'parameterName': 'Motor Lm',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 500,
                    'parameterName': 'Motor Lx',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 501,
                    'parameterName': 'PM IR Voltage',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 502,
                    'parameterName': 'PM IXd Voltage',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 503,
                    'parameterName': 'PM IXq Voltage',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 504,
                    'parameterName': 'PM BEMF Voltage',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 505,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 506,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 507,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 508,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 509,
                    'parameterName': 'Speed Reg Sel',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 510,
                    'parameterName': 'Freq 1',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 511,
                    'parameterName': 'Freq 1 BW',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 512,
                    'parameterName': 'Freq 2',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 513,
                    'parameterName': 'Freq 2 BW',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 514,
                    'parameterName': 'Freq 3',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 515,
                    'parameterName': 'Freq 3 BW',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 516,
                    'parameterName': 'PM Initial Sel',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 517,
                    'parameterName': 'PM DC Inject Cur',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 518,
                    'parameterName': 'PM Align Time',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 519,
                    'parameterName': 'PM HFI NS Cur',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 520,
                    'parameterName': 'PM Bus Reg Kd',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 521,
                    'parameterName': 'Freq 1 Kp',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 522,
                    'parameterName': 'Freq 1 Ki',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 523,
                    'parameterName': 'Freq 2 Kp',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 524,
                    'parameterName': 'Freq 2 Ki',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 525,
                    'parameterName': 'Freq 3 Kp',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 526,
                    'parameterName': 'Freq 3 Ki',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 527,
                    'parameterName': 'PM FWKn 1 Kp',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 528,
                    'parameterName': 'PM FWKn 2 Kp',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 529,
                    'parameterName': 'PM Control Cfg',
                    'parameterType': '0g==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 530,
                    'parameterName': 'Boost Select',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 531,
                    'parameterName': 'Start Boost',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 532,
                    'parameterName': 'Break Voltage',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 533,
                    'parameterName': 'Break Frequency',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 534,
                    'parameterName': 'Maximum Voltage',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 535,
                    'parameterName': 'Motor Fdbk Type',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 536,
                    'parameterName': 'Encoder PPR',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 537,
                    'parameterName': 'Pulse In Scale',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 538,
                    'parameterName': 'Ki Speed Loop',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 539,
                    'parameterName': 'Kp Speed Loop',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 540,
                    'parameterName': 'Var PWM Disable',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 541,
                    'parameterName': 'Auto Rstrt Tries',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 542,
                    'parameterName': 'Auto Rstrt Delay',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 543,
                    'parameterName': 'Start At PowerUp',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 544,
                    'parameterName': 'Reverse Disable',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 545,
                    'parameterName': 'Flying Start En',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 546,
                    'parameterName': 'FlyStrt CurLimit',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 547,
                    'parameterName': 'Compensation',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 548,
                    'parameterName': 'Power Loss Mode',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 549,
                    'parameterName': 'Half Bus Enable',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 550,
                    'parameterName': 'Bus Reg Enable',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 551,
                    'parameterName': 'Fault Clear',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 552,
                    'parameterName': 'Program Lock',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 553,
                    'parameterName': 'Program Lock Mod',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 554,
                    'parameterName': 'Drv Ambient Sel',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 555,
                    'parameterName': 'Reset Meters',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 556,
                    'parameterName': 'Text Scroll',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 557,
                    'parameterName': 'Out Phas Loss En',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 558,
                    'parameterName': 'Positioning Mode',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 559,
                    'parameterName': 'Counts Per Unit',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 560,
                    'parameterName': 'Enh Control Word',
                    'parameterType': '0g==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 561,
                    'parameterName': 'Home Save',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 562,
                    'parameterName': 'Find Home Freq',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 563,
                    'parameterName': 'Find Home Dir',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 564,
                    'parameterName': 'Encoder Pos Tol',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 565,
                    'parameterName': 'Pos Reg Filter',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 566,
                    'parameterName': 'Pos Reg Gain',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 567,
                    'parameterName': 'Max Traverse',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 568,
                    'parameterName': 'Traverse Inc',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 569,
                    'parameterName': 'Traverse Dec',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 570,
                    'parameterName': 'P Jump',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 571,
                    'parameterName': 'Sync Time',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 572,
                    'parameterName': 'Speed Ratio',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 573,
                    'parameterName': 'Mtr Options Cfg',
                    'parameterType': '0g==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 574,
                    'parameterName': 'RdyBit Mode Cfg',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 575,
                    'parameterName': 'Flux Braking En',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 576,
                    'parameterName': 'Phase Loss Level',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 577,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 578,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 579,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 580,
                    'parameterName': 'Current Loop BW',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 581,
                    'parameterName': 'PM Stable 1 Freq',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 582,
                    'parameterName': 'PM Stable 2 Freq',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 583,
                    'parameterName': 'PM Stable 1 Kp',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 584,
                    'parameterName': 'PM Stable 2 Kp',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 585,
                    'parameterName': 'PM Stable Brk Pt',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 586,
                    'parameterName': 'PM Stepload Kp',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 587,
                    'parameterName': 'PM 1 Efficiency',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 588,
                    'parameterName': 'PM 2 Efficiency',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 589,
                    'parameterName': 'PM Algor Sel',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 590,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 591,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 592,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 593,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 594,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 595,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 596,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 597,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 598,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 599,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 600,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 601,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 602,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 603,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 604,
                    'parameterName': 'Fault 4 Code',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 605,
                    'parameterName': 'Fault 5 Code',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 606,
                    'parameterName': 'Fault 6 Code',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 607,
                    'parameterName': 'Fault 7 Code',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 608,
                    'parameterName': 'Fault 8 Code',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 609,
                    'parameterName': 'Fault 9 Code',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 610,
                    'parameterName': 'Fault10 Code',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 611,
                    'parameterName': 'Fault 1 Time-hr',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 612,
                    'parameterName': 'Fault 2 Time-hr',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 613,
                    'parameterName': 'Fault 3 Time-hr',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 614,
                    'parameterName': 'Fault 4 Time-hr',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 615,
                    'parameterName': 'Fault 5 Time-hr',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 616,
                    'parameterName': 'Fault 6 Time-hr',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 617,
                    'parameterName': 'Fault 7 Time-hr',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 618,
                    'parameterName': 'Fault 8 Time-hr',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 619,
                    'parameterName': 'Fault 9 Time-hr',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 620,
                    'parameterName': 'Fault10 Time-hr',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 621,
                    'parameterName': 'Fault 1 Time-min',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 622,
                    'parameterName': 'Fault 2 Time-min',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 623,
                    'parameterName': 'Fault 3 Time-min',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 624,
                    'parameterName': 'Fault 4 Time-min',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 625,
                    'parameterName': 'Fault 5 Time-min',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 626,
                    'parameterName': 'Fault 6 Time-min',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 627,
                    'parameterName': 'Fault 7 Time-min',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 628,
                    'parameterName': 'Fault 8 Time-min',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 629,
                    'parameterName': 'Fault 9 Time-min',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 630,
                    'parameterName': 'Fault10 Time-min',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 631,
                    'parameterName': 'Fault 1 Freq',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 632,
                    'parameterName': 'Fault 2 Freq',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 633,
                    'parameterName': 'Fault 3 Freq',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 634,
                    'parameterName': 'Fault 4 Freq',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 635,
                    'parameterName': 'Fault 5 Freq',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 636,
                    'parameterName': 'Fault 6 Freq',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 637,
                    'parameterName': 'Fault 7 Freq',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 638,
                    'parameterName': 'Fault 8 Freq',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 639,
                    'parameterName': 'Fault 9 Freq',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 640,
                    'parameterName': 'Fault10 Freq',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 641,
                    'parameterName': 'Fault 1 Current',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 642,
                    'parameterName': 'Fault 2 Current',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 643,
                    'parameterName': 'Fault 3 Current',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 644,
                    'parameterName': 'Fault 4 Current',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 645,
                    'parameterName': 'Fault 5 Current',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 646,
                    'parameterName': 'Fault 6 Current',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 647,
                    'parameterName': 'Fault 7 Current',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 648,
                    'parameterName': 'Fault 8 Current',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 649,
                    'parameterName': 'Fault 9 Current',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 650,
                    'parameterName': 'Fault10 Current',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 651,
                    'parameterName': 'Fault 1 BusVolts',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 652,
                    'parameterName': 'Fault 2 BusVolts',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 653,
                    'parameterName': 'Fault 3 BusVolts',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 654,
                    'parameterName': 'Fault 4 BusVolts',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 655,
                    'parameterName': 'Fault 5 BusVolts',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 656,
                    'parameterName': 'Fault 6 BusVolts',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 657,
                    'parameterName': 'Fault 7 BusVolts',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 658,
                    'parameterName': 'Fault 8 BusVolts',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 659,
                    'parameterName': 'Fault 9 BusVolts',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 660,
                    'parameterName': 'Fault10 BusVolts',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 661,
                    'parameterName': 'Status @ Fault 1',
                    'parameterType': '0g==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 662,
                    'parameterName': 'Status @ Fault 2',
                    'parameterType': '0g==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 663,
                    'parameterName': 'Status @ Fault 3',
                    'parameterType': '0g==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 664,
                    'parameterName': 'Status @ Fault 4',
                    'parameterType': '0g==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 665,
                    'parameterName': 'Status @ Fault 5',
                    'parameterType': '0g==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 666,
                    'parameterName': 'Status @ Fault 6',
                    'parameterType': '0g==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 667,
                    'parameterName': 'Status @ Fault 7',
                    'parameterType': '0g==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 668,
                    'parameterName': 'Status @ Fault 8',
                    'parameterType': '0g==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 669,
                    'parameterName': 'Status @ Fault 9',
                    'parameterType': '0g==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 670,
                    'parameterName': 'Status @ Fault10',
                    'parameterType': '0g==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 671,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 672,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 673,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 674,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 675,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 676,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 677,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 678,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 679,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 680,
                    'parameterName': 'Reserved',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 681,
                    'parameterName': 'Comm Sts - DSI',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 682,
                    'parameterName': 'Comm Sts - Opt',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 683,
                    'parameterName': 'Com Sts-Emb Enet',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 684,
                    'parameterName': 'EN Addr Src',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 685,
                    'parameterName': 'EN Rate Act',
                    'parameterType': 'xw==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 686,
                    'parameterName': 'DSI I\/O Act',
                    'parameterType': '0g==',
                    'recordValue': true
                    },
                    {
                    'parameterNumber': 687,
                    'parameterName': 'HW Addr 1',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 688,
                    'parameterName': 'HW Addr 2',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 689,
                    'parameterName': 'HW Addr 3',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 690,
                    'parameterName': 'HW Addr 4',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 691,
                    'parameterName': 'HW Addr 5',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 692,
                    'parameterName': 'HW Addr 6',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 693,
                    'parameterName': 'EN IP Addr Act 1',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 694,
                    'parameterName': 'EN IP Addr Act 2',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 695,
                    'parameterName': 'EN IP Addr Act 3',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 696,
                    'parameterName': 'EN IP Addr Act 4',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 697,
                    'parameterName': 'EN Subnet Act 1',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 698,
                    'parameterName': 'EN Subnet Act 2',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 699,
                    'parameterName': 'EN Subnet Act 3',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 700,
                    'parameterName': 'EN Subnet Act 4',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 701,
                    'parameterName': 'EN Gateway Act 1',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 702,
                    'parameterName': 'EN Gateway Act 2',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 703,
                    'parameterName': 'EN Gateway Act 3',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 704,
                    'parameterName': 'EN Gateway Act 4',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 705,
                    'parameterName': 'Drv 0 Logic Cmd',
                    'parameterType': '0g==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 706,
                    'parameterName': 'Drv 0 Reference',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 707,
                    'parameterName': 'Drv 0 Logic Sts',
                    'parameterType': '0g==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 708,
                    'parameterName': 'Drv 0 Feedback',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 709,
                    'parameterName': 'Drv 1 Logic Cmd',
                    'parameterType': '0g==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 710,
                    'parameterName': 'Drv 1 Reference',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 711,
                    'parameterName': 'Drv 1 Logic Sts',
                    'parameterType': '0g==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 712,
                    'parameterName': 'Drv 1 Feedback',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 713,
                    'parameterName': 'Drv 2 Logic Cmd',
                    'parameterType': '0g==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 714,
                    'parameterName': 'Drv 2 Reference',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 715,
                    'parameterName': 'Drv 2 Logic Sts',
                    'parameterType': '0g==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 716,
                    'parameterName': 'Drv 2 Feedback',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 717,
                    'parameterName': 'Drv 3 Logic Cmd',
                    'parameterType': '0g==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 718,
                    'parameterName': 'Drv 3 Reference',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 719,
                    'parameterName': 'Drv 3 Logic Sts',
                    'parameterType': '0g==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 720,
                    'parameterName': 'Drv 3 Feedback',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 721,
                    'parameterName': 'Drv 4 Logic Cmd',
                    'parameterType': '0g==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 722,
                    'parameterName': 'Drv 4 Reference',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 723,
                    'parameterName': 'Drv 4 Logic Sts',
                    'parameterType': '0g==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 724,
                    'parameterName': 'Drv 4 Feedback',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 725,
                    'parameterName': 'EN Rx Overruns',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 726,
                    'parameterName': 'EN Rx Packets',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 727,
                    'parameterName': 'EN Rx Errors',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 728,
                    'parameterName': 'EN Tx Packets',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 729,
                    'parameterName': 'EN Tx Errors',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 730,
                    'parameterName': 'EN Missed IO Pkt',
                    'parameterType': 'xw==',
                    'recordValue': false
                    },
                    {
                    'parameterNumber': 731,
                    'parameterName': 'DSI Errors',
                    'parameterType': 'xw==',
                    'recordValue': false
                    }
                    ]
                }";
    }
}