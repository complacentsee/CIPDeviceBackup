using Newtonsoft.Json;
using powerFlexBackup.cipdevice.deviceParameterObjects;

namespace powerFlexBackup.cipdevice
{
    public class CIPDevice_193ETN : CIPDevice{
        public CIPDevice_193ETN(String driveAddress, IdentityObject identityObject, Sres.Net.EEIP.EEIPClient eeipClient) :
            base(driveAddress, identityObject, eeipClient)
        {
            setDeviceParameterClassID(0x0F);
            setDeviceParameterList(JsonConvert.DeserializeObject<List<DeviceParameter>>(parameterListJSON));    
            setInstanceAttribute(JsonConvert.DeserializeObject<List<InstanceAttribute>>(instanceAttributeJSON));    
        }

        /* Process the parameter from a bytearray to an int based on type
        0xC1 - BOOL
        0xC6 - USINT
        0xC7 - UINT
        0xD2 - WORD */
        
        //FIXME: Should this be a list that we can look up the converion for each drive? Then the method can exist in the base class.
        public const byte DeviceBOOL = 0xC1; 
        public const byte DeviceUSINT = 0xC6;
        public const byte DeviceUINT = 0xC7;
        public const byte DeviceWORD = 0xD2; // 16 bit mask

        public override string getParameterValuefromBytes(byte[] parameterValueBytes, byte[] type)
        {
            switch (type[0])
            {
                case 0xC1:
                    return CIPDeviceHelper.convertBytestoBOOL(parameterValueBytes);

                case 0xC6:
                    return CIPDeviceHelper.convertBytestoUSINT8(parameterValueBytes);

                case 0xC7:
                    return CIPDeviceHelper.convertBytestoUINT16LittleEndian(parameterValueBytes);

                case 0xD2:
                    return CIPDeviceHelper.convertBytestoDWORD(parameterValueBytes);

                default:
                    return "Unknown Parameter Type";    
            }
        }

        //can this be dynamically generated?
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
                            'number': 1,
                            'name': 'Average %FLA',
                            'defaultValue': '0',
                            'record': false,
                            'type': 'xw=='
                        },
                        {
                            'number': 2,
                            'name': '% Therm Utilized',
                            'defaultValue': '0',
                            'record': false,
                            'type': 'xg=='
                        },
                        {
                            'number': 3,
                            'name': 'Trip Status',
                            'defaultValue': '0000000000000000',
                            'record': false,
                            'type': '0g=='
                        },
                        {
                            'number': 4,
                            'name': 'Warning Status',
                            'defaultValue': '0000000000000000',
                            'record': false,
                            'type': '0g=='
                        },
                        {
                            'number': 5,
                            'name': 'Trip Log 0',
                            'defaultValue': '0000000000000000',
                            'record': false,
                            'type': '0g=='
                        },
                        {
                            'number': 6,
                            'name': 'Trip Log 1',
                            'defaultValue': '0000000000000000',
                            'record': false,
                            'type': '0g=='
                        },
                        {
                            'number': 7,
                            'name': 'Trip Log 2',
                            'defaultValue': '0000000000000000',
                            'record': false,
                            'type': '0g=='
                        },
                        {
                            'number': 8,
                            'name': 'Trip Log 3',
                            'defaultValue': '0000000000000000',
                            'record': false,
                            'type': '0g=='
                        },
                        {
                            'number': 9,
                            'name': 'Trip Log 4',
                            'defaultValue': '0000000000000000',
                            'record': false,
                            'type': '0g=='
                        },
                        {
                            'number': 10,
                            'name': 'Device Status',
                            'defaultValue': '0000000000000000',
                            'record': false,
                            'type': '0g=='
                        },
                        {
                            'number': 11,
                            'name': 'Reserved',
                            'defaultValue': '0',
                            'record': false,
                            'type': 'xw=='
                        },
                        {
                            'number': 12,
                            'name': 'Trip Enable',
                            'defaultValue': '0000000000000000',
                            'record': true,
                            'type': '0g=='
                        },
                        {
                            'number': 13,
                            'name': 'Warning Enable',
                            'defaultValue': '0000000000000000',
                            'record': true,
                            'type': '0g=='
                        },
                        {
                            'number': 14,
                            'name': 'Trip Reset',
                            'defaultValue': 'False',
                            'record': true,
                            'type': 'wQ=='
                        },
                        {
                            'number': 15,
                            'name': 'Single/Three Ph',
                            'defaultValue': 'True',
                            'record': true,
                            'type': 'wQ=='
                        },
                        {
                            'number': 16,
                            'name': 'OL Reset Mode',
                            'defaultValue': 'False',
                            'record': true,
                            'type': 'wQ=='
                        },
                        {
                            'number': 17,
                            'name': 'OL Warning Level',
                            'defaultValue': '90',
                            'record': true,
                            'type': 'xg=='
                        },
                        {
                            'number': 18,
                            'name': 'Jam Inhibit Time',
                            'defaultValue': '10',
                            'record': true,
                            'type': 'xg=='
                        },
                        {
                            'number': 19,
                            'name': 'Jam Trip Delay',
                            'defaultValue': '50',
                            'record': true,
                            'type': 'xg=='
                        },
                        {
                            'number': 20,
                            'name': 'Jam Trip Level',
                            'defaultValue': '250',
                            'record': true,
                            'type': 'xw=='
                        },
                        {
                            'number': 21,
                            'name': 'Jam Warn Level',
                            'defaultValue': '150',
                            'record': true,
                            'type': 'xw=='
                        },
                        {
                            'number': 22,
                            'name': 'UL Inhibit Time',
                            'defaultValue': '10',
                            'record': true,
                            'type': 'xg=='
                        },
                        {
                            'number': 23,
                            'name': 'UL Warn Level',
                            'defaultValue': '70',
                            'record': true,
                            'type': 'xg=='
                        },
                        {
                            'number': 24,
                            'name': 'Program Lock',
                            'defaultValue': 'False',
                            'record': true,
                            'type': 'wQ=='
                        },
                        {
                            'number': 25,
                            'name': 'Set To Defaults',
                            'defaultValue': 'False',
                            'record': true,
                            'type': 'wQ=='
                        },
                        {
                            'number': 26,
                            'name': 'Reserved',
                            'defaultValue': '0',
                            'record': false,
                            'type': 'xw=='
                        },
                        {
                            'number': 27,
                            'name': 'Reserved',
                            'defaultValue': '0',
                            'record': false,
                            'type': 'xw=='
                        },
                        {
                            'number': 28,
                            'name': 'Reserved',
                            'defaultValue': '0',
                            'record': false,
                            'type': 'xw=='
                        },
                        {
                            'number': 29,
                            'name': 'Reserved',
                            'defaultValue': '0',
                            'record': false,
                            'type': 'xw=='
                        },
                        {
                            'number': 30,
                            'name': 'Reserved',
                            'defaultValue': '0',
                            'record': false,
                            'type': 'xw=='
                        },
                        {
                            'number': 31,
                            'name': 'Reserved',
                            'defaultValue': '0',
                            'record': false,
                            'type': 'xw=='
                        },
                        {
                            'number': 32,
                            'name': 'Reserved',
                            'defaultValue': '0',
                            'record': false,
                            'type': 'xw=='
                        },
                        {
                            'number': 33,
                            'name': 'Reserved',
                            'defaultValue': '0',
                            'record': false,
                            'type': 'xw=='
                        },
                        {
                            'number': 34,
                            'name': 'OutA Pr FltState',
                            'defaultValue': 'False',
                            'record': true,
                            'type': 'wQ=='
                        },
                        {
                            'number': 35,
                            'name': 'OutA Pr FltValue',
                            'defaultValue': 'False',
                            'record': true,
                            'type': 'wQ=='
                        },
                        {
                            'number': 36,
                            'name': 'OutA En FltState',
                            'defaultValue': 'False',
                            'record': true,
                            'type': 'wQ=='
                        },
                        {
                            'number': 37,
                            'name': 'OutA En FltValue',
                            'defaultValue': 'False',
                            'record': true,
                            'type': 'wQ=='
                        },
                        {
                            'number': 38,
                            'name': 'OutA En IdlState',
                            'defaultValue': 'False',
                            'record': true,
                            'type': 'wQ=='
                        },
                        {
                            'number': 39,
                            'name': 'OutA En IdlValue',
                            'defaultValue': 'False',
                            'record': true,
                            'type': 'wQ=='
                        },
                        {
                            'number': 40,
                            'name': 'IN1 Assignment',
                            'defaultValue': '0',
                            'record': true,
                            'type': 'xg=='
                        },
                        {
                            'number': 41,
                            'name': 'IN2 Assignment',
                            'defaultValue': '0',
                            'record': true,
                            'type': 'xg=='
                        }
                        ]";
    }
}