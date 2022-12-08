using Newtonsoft.Json;
using powerFlexBackup.cipdevice.deviceParameterObjects;

namespace powerFlexBackup.cipdevice
{
    public class CIPDevice_193ETN : CIPDevice{
        public CIPDevice_193ETN(String deviceAddress, Sres.Net.EEIP.EEIPClient eeipClient) :
            base(deviceAddress, eeipClient)
        {
            setParameterClassID(0x0F);
            setInstanceAttributes();
            setDeviceParameterList(JsonConvert.DeserializeObject<List<DeviceParameter>>(parameterListJSON)!);    
        }

        public override void setInstanceAttributes(int instance = 0){
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

        public override void getDeviceParameterValues(){
            getDeviceParameterValuesCIPStandardCompliant();
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