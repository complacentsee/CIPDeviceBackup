using Newtonsoft.Json;
using powerFlexBackup.cipdevice.deviceParameterObjects;

namespace powerFlexBackup.cipdevice
{
    public class CIPDevice_E300 : CIPDevice{
        public CIPDevice_E300(String deviceAddress, Sres.Net.EEIP.EEIPClient eeipClient) :
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
                        'name': 'ThermUtilizedPct',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 2,
                        'name': 'OLTimeToTrip',
                        'defaultValue': 'C7',
                        'record': true,
                        'type': 'xw=='
                    },
                    {
                        'number': 3,
                        'name': 'OLTimeToReset',
                        'defaultValue': 'C7',
                        'record': true,
                        'type': 'xw=='
                    },
                    {
                        'number': 4,
                        'name': 'TripStsCurrent',
                        'defaultValue': 'D2',
                        'record': true,
                        'type': '0g=='
                    },
                    {
                        'number': 5,
                        'name': 'TripStsVoltage',
                        'defaultValue': 'D2',
                        'record': true,
                        'type': '0g=='
                    },
                    {
                        'number': 6,
                        'name': 'TripStsPower',
                        'defaultValue': 'D2',
                        'record': true,
                        'type': '0g=='
                    },
                    {
                        'number': 7,
                        'name': 'TripStsControl',
                        'defaultValue': 'D2',
                        'record': true,
                        'type': '0g=='
                    },
                    {
                        'number': 8,
                        'name': 'TripStsAnalog',
                        'defaultValue': 'D2',
                        'record': true,
                        'type': '0g=='
                    },
                    {
                        'number': 9,
                        'name': 'Reserved',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 10,
                        'name': 'WarnStsCurrent',
                        'defaultValue': 'D2',
                        'record': true,
                        'type': '0g=='
                    },
                    {
                        'number': 11,
                        'name': 'WarnStsVoltage',
                        'defaultValue': 'D2',
                        'record': true,
                        'type': '0g=='
                    },
                    {
                        'number': 12,
                        'name': 'WarnStsPower',
                        'defaultValue': 'D2',
                        'record': true,
                        'type': '0g=='
                    },
                    {
                        'number': 13,
                        'name': 'WarnStsControl',
                        'defaultValue': 'D2',
                        'record': true,
                        'type': '0g=='
                    },
                    {
                        'number': 14,
                        'name': 'WarnStsAnalog',
                        'defaultValue': 'D2',
                        'record': true,
                        'type': '0g=='
                    },
                    {
                        'number': 15,
                        'name': 'Reserved',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 16,
                        'name': 'InputStatus0',
                        'defaultValue': 'D2',
                        'record': true,
                        'type': '0g=='
                    },
                    {
                        'number': 17,
                        'name': 'InputStatus1',
                        'defaultValue': 'D2',
                        'record': true,
                        'type': '0g=='
                    },
                    {
                        'number': 18,
                        'name': 'OutputStatus',
                        'defaultValue': 'D2',
                        'record': true,
                        'type': '0g=='
                    },
                    {
                        'number': 19,
                        'name': 'OpStationStatus',
                        'defaultValue': 'D2',
                        'record': true,
                        'type': '0g=='
                    },
                    {
                        'number': 20,
                        'name': 'DeviceStatus0',
                        'defaultValue': 'D2',
                        'record': true,
                        'type': '0g=='
                    },
                    {
                        'number': 21,
                        'name': 'DeviceStatus1',
                        'defaultValue': 'D2',
                        'record': true,
                        'type': '0g=='
                    },
                    {
                        'number': 22,
                        'name': 'Firmware',
                        'defaultValue': 'C7',
                        'record': true,
                        'type': 'xw=='
                    },
                    {
                        'number': 23,
                        'name': 'ControlModuleID',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 24,
                        'name': 'SensingModuleID',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 25,
                        'name': 'OperStationID',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 26,
                        'name': 'DigitalModuleID',
                        'defaultValue': 'C7',
                        'record': true,
                        'type': 'xw=='
                    },
                    {
                        'number': 27,
                        'name': 'AnalogModuleID',
                        'defaultValue': 'C7',
                        'record': true,
                        'type': 'xw=='
                    },
                    {
                        'number': 28,
                        'name': 'OperatingTime',
                        'defaultValue': 'C7',
                        'record': true,
                        'type': 'xw=='
                    },
                    {
                        'number': 29,
                        'name': 'StartsCounter',
                        'defaultValue': 'C7',
                        'record': true,
                        'type': 'xw=='
                    },
                    {
                        'number': 30,
                        'name': 'Starts Available',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 31,
                        'name': 'TimeToStart',
                        'defaultValue': 'C7',
                        'record': true,
                        'type': 'xw=='
                    },
                    {
                        'number': 32,
                        'name': 'Year',
                        'defaultValue': 'C7',
                        'record': true,
                        'type': 'xw=='
                    },
                    {
                        'number': 33,
                        'name': 'Month',
                        'defaultValue': 'C7',
                        'record': true,
                        'type': 'xw=='
                    },
                    {
                        'number': 34,
                        'name': 'Day',
                        'defaultValue': 'C7',
                        'record': true,
                        'type': 'xw=='
                    },
                    {
                        'number': 35,
                        'name': 'Hour',
                        'defaultValue': 'C7',
                        'record': true,
                        'type': 'xw=='
                    },
                    {
                        'number': 36,
                        'name': 'Minute',
                        'defaultValue': 'C7',
                        'record': true,
                        'type': 'xw=='
                    },
                    {
                        'number': 37,
                        'name': 'Second',
                        'defaultValue': 'C7',
                        'record': true,
                        'type': 'xw=='
                    },
                    {
                        'number': 38,
                        'name': 'InvaldCfgParam',
                        'defaultValue': 'C7',
                        'record': true,
                        'type': 'xw=='
                    },
                    {
                        'number': 39,
                        'name': 'InvaldCfgCause',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 40,
                        'name': 'MismatchStatus',
                        'defaultValue': 'D2',
                        'record': true,
                        'type': '0g=='
                    },
                    {
                        'number': 41,
                        'name': 'Reserved',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 42,
                        'name': 'Reserved',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 43,
                        'name': 'L1Current',
                        'defaultValue': 'C4',
                        'record': true,
                        'type': 'xA=='
                    },
                    {
                        'number': 44,
                        'name': 'L2Current',
                        'defaultValue': 'C4',
                        'record': true,
                        'type': 'xA=='
                    },
                    {
                        'number': 45,
                        'name': 'L3Current',
                        'defaultValue': 'C4',
                        'record': true,
                        'type': 'xA=='
                    },
                    {
                        'number': 46,
                        'name': 'AverageCurrent',
                        'defaultValue': 'C4',
                        'record': true,
                        'type': 'xA=='
                    },
                    {
                        'number': 47,
                        'name': 'L1PercentFLA',
                        'defaultValue': 'C7',
                        'record': true,
                        'type': 'xw=='
                    },
                    {
                        'number': 48,
                        'name': 'L2PercentFLA',
                        'defaultValue': 'C7',
                        'record': true,
                        'type': 'xw=='
                    },
                    {
                        'number': 49,
                        'name': 'L3PercentFLA',
                        'defaultValue': 'C7',
                        'record': true,
                        'type': 'xw=='
                    },
                    {
                        'number': 50,
                        'name': 'AvgPercentFLA',
                        'defaultValue': 'C7',
                        'record': true,
                        'type': 'xw=='
                    },
                    {
                        'number': 51,
                        'name': 'GFCurrent',
                        'defaultValue': 'C7',
                        'record': true,
                        'type': 'xw=='
                    },
                    {
                        'number': 52,
                        'name': 'CurrentImbal',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 53,
                        'name': 'L1toL2Voltage',
                        'defaultValue': 'C7',
                        'record': true,
                        'type': 'xw=='
                    },
                    {
                        'number': 54,
                        'name': 'L2toL3Voltage',
                        'defaultValue': 'C7',
                        'record': true,
                        'type': 'xw=='
                    },
                    {
                        'number': 55,
                        'name': 'L3toL1Voltage',
                        'defaultValue': 'C7',
                        'record': true,
                        'type': 'xw=='
                    },
                    {
                        'number': 56,
                        'name': 'AvgVoltageLtoL',
                        'defaultValue': 'C7',
                        'record': true,
                        'type': 'xw=='
                    },
                    {
                        'number': 57,
                        'name': 'L1toNVoltage',
                        'defaultValue': 'C7',
                        'record': true,
                        'type': 'xw=='
                    },
                    {
                        'number': 58,
                        'name': 'L2toNVoltage',
                        'defaultValue': 'C7',
                        'record': true,
                        'type': 'xw=='
                    },
                    {
                        'number': 59,
                        'name': 'L3toNVoltage',
                        'defaultValue': 'C7',
                        'record': true,
                        'type': 'xw=='
                    },
                    {
                        'number': 60,
                        'name': 'AvgVoltageLtoN',
                        'defaultValue': 'C7',
                        'record': true,
                        'type': 'xw=='
                    },
                    {
                        'number': 61,
                        'name': 'VoltageImbalance',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 62,
                        'name': 'VoltageFrequency',
                        'defaultValue': 'C7',
                        'record': true,
                        'type': 'xw=='
                    },
                    {
                        'number': 63,
                        'name': 'VPhaseRotation',
                        'defaultValue': 'C7',
                        'record': true,
                        'type': 'xw=='
                    },
                    {
                        'number': 64,
                        'name': 'L1RealPower',
                        'defaultValue': 'C4',
                        'record': true,
                        'type': 'xA=='
                    },
                    {
                        'number': 65,
                        'name': 'L2RealPower',
                        'defaultValue': 'C4',
                        'record': true,
                        'type': 'xA=='
                    },
                    {
                        'number': 66,
                        'name': 'L3RealPower',
                        'defaultValue': 'C4',
                        'record': true,
                        'type': 'xA=='
                    },
                    {
                        'number': 67,
                        'name': 'TotalRealPower',
                        'defaultValue': 'C4',
                        'record': true,
                        'type': 'xA=='
                    },
                    {
                        'number': 68,
                        'name': 'L1ReactivePower',
                        'defaultValue': 'C4',
                        'record': true,
                        'type': 'xA=='
                    },
                    {
                        'number': 69,
                        'name': 'L2ReactivePower',
                        'defaultValue': 'C4',
                        'record': true,
                        'type': 'xA=='
                    },
                    {
                        'number': 70,
                        'name': 'L3ReactivePower',
                        'defaultValue': 'C4',
                        'record': true,
                        'type': 'xA=='
                    },
                    {
                        'number': 71,
                        'name': 'TotalReactivePwr',
                        'defaultValue': 'C4',
                        'record': true,
                        'type': 'xA=='
                    },
                    {
                        'number': 72,
                        'name': 'L1ApparentPower',
                        'defaultValue': 'C4',
                        'record': true,
                        'type': 'xA=='
                    },
                    {
                        'number': 73,
                        'name': 'L2ApparentPower',
                        'defaultValue': 'C4',
                        'record': true,
                        'type': 'xA=='
                    },
                    {
                        'number': 74,
                        'name': 'L3ApparentPower',
                        'defaultValue': 'C4',
                        'record': true,
                        'type': 'xA=='
                    },
                    {
                        'number': 75,
                        'name': 'TotalApparentPwr',
                        'defaultValue': 'C4',
                        'record': true,
                        'type': 'xA=='
                    },
                    {
                        'number': 76,
                        'name': 'L1PowerFactor',
                        'defaultValue': 'C3',
                        'record': true,
                        'type': 'ww=='
                    },
                    {
                        'number': 77,
                        'name': 'L2PowerFactor',
                        'defaultValue': 'C3',
                        'record': true,
                        'type': 'ww=='
                    },
                    {
                        'number': 78,
                        'name': 'L3PowerFactor',
                        'defaultValue': 'C3',
                        'record': true,
                        'type': 'ww=='
                    },
                    {
                        'number': 79,
                        'name': 'TotalPowerFactor',
                        'defaultValue': 'C3',
                        'record': true,
                        'type': 'ww=='
                    },
                    {
                        'number': 80,
                        'name': 'kWhTimes10E9',
                        'defaultValue': 'C3',
                        'record': true,
                        'type': 'ww=='
                    },
                    {
                        'number': 81,
                        'name': 'kWhTimes10E6',
                        'defaultValue': 'C3',
                        'record': true,
                        'type': 'ww=='
                    },
                    {
                        'number': 82,
                        'name': 'kWhTimes10E3',
                        'defaultValue': 'C3',
                        'record': true,
                        'type': 'ww=='
                    },
                    {
                        'number': 83,
                        'name': 'kWhTimes10E0',
                        'defaultValue': 'C3',
                        'record': true,
                        'type': 'ww=='
                    },
                    {
                        'number': 84,
                        'name': 'kWhTimes10E-3',
                        'defaultValue': 'C3',
                        'record': true,
                        'type': 'ww=='
                    },
                    {
                        'number': 85,
                        'name': 'kVARhCon10E9',
                        'defaultValue': 'C3',
                        'record': true,
                        'type': 'ww=='
                    },
                    {
                        'number': 86,
                        'name': 'kVARhCon10E6',
                        'defaultValue': 'C3',
                        'record': true,
                        'type': 'ww=='
                    },
                    {
                        'number': 87,
                        'name': 'kVARhCon10E3',
                        'defaultValue': 'C3',
                        'record': true,
                        'type': 'ww=='
                    },
                    {
                        'number': 88,
                        'name': 'kVARhCon10E0',
                        'defaultValue': 'C3',
                        'record': true,
                        'type': 'ww=='
                    },
                    {
                        'number': 89,
                        'name': 'kVARhCon10E-3',
                        'defaultValue': 'C3',
                        'record': true,
                        'type': 'ww=='
                    },
                    {
                        'number': 90,
                        'name': 'kVARhGen10E9',
                        'defaultValue': 'C3',
                        'record': true,
                        'type': 'ww=='
                    },
                    {
                        'number': 91,
                        'name': 'kVARhGen10E6',
                        'defaultValue': 'C3',
                        'record': true,
                        'type': 'ww=='
                    },
                    {
                        'number': 92,
                        'name': 'kVARhGen10E3',
                        'defaultValue': 'C3',
                        'record': true,
                        'type': 'ww=='
                    },
                    {
                        'number': 93,
                        'name': 'kVARhGen10E0',
                        'defaultValue': 'C3',
                        'record': true,
                        'type': 'ww=='
                    },
                    {
                        'number': 94,
                        'name': 'kVARhGen10E-3',
                        'defaultValue': 'C3',
                        'record': true,
                        'type': 'ww=='
                    },
                    {
                        'number': 95,
                        'name': 'kVARhNet10E9',
                        'defaultValue': 'C3',
                        'record': true,
                        'type': 'ww=='
                    },
                    {
                        'number': 96,
                        'name': 'kVARhNet10E6',
                        'defaultValue': 'C3',
                        'record': true,
                        'type': 'ww=='
                    },
                    {
                        'number': 97,
                        'name': 'kVARhNet10E3',
                        'defaultValue': 'C3',
                        'record': true,
                        'type': 'ww=='
                    },
                    {
                        'number': 98,
                        'name': 'kVARhNet10E0',
                        'defaultValue': 'C3',
                        'record': true,
                        'type': 'ww=='
                    },
                    {
                        'number': 99,
                        'name': 'kVARhNet10E-3',
                        'defaultValue': 'C3',
                        'record': true,
                        'type': 'ww=='
                    },
                    {
                        'number': 100,
                        'name': 'kVAhTimes10E9',
                        'defaultValue': 'C3',
                        'record': true,
                        'type': 'ww=='
                    },
                    {
                        'number': 101,
                        'name': 'kVAhTimes10E6',
                        'defaultValue': 'C3',
                        'record': true,
                        'type': 'ww=='
                    },
                    {
                        'number': 102,
                        'name': 'kVAhTimes10E3',
                        'defaultValue': 'C3',
                        'record': true,
                        'type': 'ww=='
                    },
                    {
                        'number': 103,
                        'name': 'kVAhTimes10E0',
                        'defaultValue': 'C3',
                        'record': true,
                        'type': 'ww=='
                    },
                    {
                        'number': 104,
                        'name': 'kVAhTimes10E-3',
                        'defaultValue': 'C3',
                        'record': true,
                        'type': 'ww=='
                    },
                    {
                        'number': 105,
                        'name': 'kWDemand',
                        'defaultValue': 'C4',
                        'record': true,
                        'type': 'xA=='
                    },
                    {
                        'number': 106,
                        'name': 'MaxkWDemand',
                        'defaultValue': 'C4',
                        'record': true,
                        'type': 'xA=='
                    },
                    {
                        'number': 107,
                        'name': 'kVARDemand',
                        'defaultValue': 'C4',
                        'record': true,
                        'type': 'xA=='
                    },
                    {
                        'number': 108,
                        'name': 'MaxkVARDemand',
                        'defaultValue': 'C4',
                        'record': true,
                        'type': 'xA=='
                    },
                    {
                        'number': 109,
                        'name': 'kVADemand',
                        'defaultValue': 'C4',
                        'record': true,
                        'type': 'xA=='
                    },
                    {
                        'number': 110,
                        'name': 'MaxkVADemand',
                        'defaultValue': 'C4',
                        'record': true,
                        'type': 'xA=='
                    },
                    {
                        'number': 111,
                        'name': 'InAnMod1Ch00',
                        'defaultValue': 'C3',
                        'record': true,
                        'type': 'ww=='
                    },
                    {
                        'number': 112,
                        'name': 'InAnMod1Ch01',
                        'defaultValue': 'C3',
                        'record': true,
                        'type': 'ww=='
                    },
                    {
                        'number': 113,
                        'name': 'InAnMod1Ch02',
                        'defaultValue': 'C3',
                        'record': true,
                        'type': 'ww=='
                    },
                    {
                        'number': 114,
                        'name': 'InAnMod2Ch00',
                        'defaultValue': 'C3',
                        'record': true,
                        'type': 'ww=='
                    },
                    {
                        'number': 115,
                        'name': 'InAnMod2Ch01',
                        'defaultValue': 'C3',
                        'record': true,
                        'type': 'ww=='
                    },
                    {
                        'number': 116,
                        'name': 'InAnMod2Ch02',
                        'defaultValue': 'C3',
                        'record': true,
                        'type': 'ww=='
                    },
                    {
                        'number': 117,
                        'name': 'InAnMod3Ch00',
                        'defaultValue': 'C3',
                        'record': true,
                        'type': 'ww=='
                    },
                    {
                        'number': 118,
                        'name': 'InAnMod3Ch01',
                        'defaultValue': 'C3',
                        'record': true,
                        'type': 'ww=='
                    },
                    {
                        'number': 119,
                        'name': 'InAnMod3Ch02',
                        'defaultValue': 'C3',
                        'record': true,
                        'type': 'ww=='
                    },
                    {
                        'number': 120,
                        'name': 'InAnMod4Ch00',
                        'defaultValue': 'C3',
                        'record': true,
                        'type': 'ww=='
                    },
                    {
                        'number': 121,
                        'name': 'InAnMod4Ch01',
                        'defaultValue': 'C3',
                        'record': true,
                        'type': 'ww=='
                    },
                    {
                        'number': 122,
                        'name': 'InAnMod4Ch02',
                        'defaultValue': 'C3',
                        'record': true,
                        'type': 'ww=='
                    },
                    {
                        'number': 123,
                        'name': 'AnalogMod1Status',
                        'defaultValue': 'D2',
                        'record': true,
                        'type': '0g=='
                    },
                    {
                        'number': 124,
                        'name': 'AnalogMod2Status',
                        'defaultValue': 'D2',
                        'record': true,
                        'type': '0g=='
                    },
                    {
                        'number': 125,
                        'name': 'AnalogMod3Status',
                        'defaultValue': 'D2',
                        'record': true,
                        'type': '0g=='
                    },
                    {
                        'number': 126,
                        'name': 'AnalogMod4Status',
                        'defaultValue': 'D2',
                        'record': true,
                        'type': '0g=='
                    },
                    {
                        'number': 127,
                        'name': 'TripHistory0',
                        'defaultValue': 'C7',
                        'record': true,
                        'type': 'xw=='
                    },
                    {
                        'number': 128,
                        'name': 'TripHistory1',
                        'defaultValue': 'C7',
                        'record': true,
                        'type': 'xw=='
                    },
                    {
                        'number': 129,
                        'name': 'TripHistory2',
                        'defaultValue': 'C7',
                        'record': true,
                        'type': 'xw=='
                    },
                    {
                        'number': 130,
                        'name': 'TripHistory3',
                        'defaultValue': 'C7',
                        'record': true,
                        'type': 'xw=='
                    },
                    {
                        'number': 131,
                        'name': 'TripHistory4',
                        'defaultValue': 'C7',
                        'record': true,
                        'type': 'xw=='
                    },
                    {
                        'number': 132,
                        'name': 'Reserved',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 133,
                        'name': 'WarningHistory0',
                        'defaultValue': 'C7',
                        'record': true,
                        'type': 'xw=='
                    },
                    {
                        'number': 134,
                        'name': 'WarningHistory1',
                        'defaultValue': 'C7',
                        'record': true,
                        'type': 'xw=='
                    },
                    {
                        'number': 135,
                        'name': 'WarningHistory2',
                        'defaultValue': 'C7',
                        'record': true,
                        'type': 'xw=='
                    },
                    {
                        'number': 136,
                        'name': 'WarningHistory3',
                        'defaultValue': 'C7',
                        'record': true,
                        'type': 'xw=='
                    },
                    {
                        'number': 137,
                        'name': 'WarningHistory4',
                        'defaultValue': 'C7',
                        'record': true,
                        'type': 'xw=='
                    },
                    {
                        'number': 138,
                        'name': 'Reserved',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 139,
                        'name': 'TripHistoryMaskI',
                        'defaultValue': 'D2',
                        'record': true,
                        'type': '0g=='
                    },
                    {
                        'number': 140,
                        'name': 'TripHistoryMaskV',
                        'defaultValue': 'D2',
                        'record': true,
                        'type': '0g=='
                    },
                    {
                        'number': 141,
                        'name': 'TripHistoryMaskP',
                        'defaultValue': 'D2',
                        'record': true,
                        'type': '0g=='
                    },
                    {
                        'number': 142,
                        'name': 'TripHistoryMaskC',
                        'defaultValue': 'D2',
                        'record': true,
                        'type': '0g=='
                    },
                    {
                        'number': 143,
                        'name': 'TripHistoryMaskA',
                        'defaultValue': 'D2',
                        'record': true,
                        'type': '0g=='
                    },
                    {
                        'number': 144,
                        'name': 'Reserved',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 145,
                        'name': 'WarnHistoryMaskI',
                        'defaultValue': 'D2',
                        'record': true,
                        'type': '0g=='
                    },
                    {
                        'number': 146,
                        'name': 'WarnHistoryMaskV',
                        'defaultValue': 'D2',
                        'record': true,
                        'type': '0g=='
                    },
                    {
                        'number': 147,
                        'name': 'WarnHistoryMaskP',
                        'defaultValue': 'D2',
                        'record': true,
                        'type': '0g=='
                    },
                    {
                        'number': 148,
                        'name': 'WarnHistoryMaskC',
                        'defaultValue': 'D2',
                        'record': true,
                        'type': '0g=='
                    },
                    {
                        'number': 149,
                        'name': 'WarnHistoryMaskA',
                        'defaultValue': 'D2',
                        'record': true,
                        'type': '0g=='
                    },
                    {
                        'number': 150,
                        'name': 'Reserved',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 151,
                        'name': 'TSL1Current',
                        'defaultValue': 'C4',
                        'record': true,
                        'type': 'xA=='
                    },
                    {
                        'number': 152,
                        'name': 'TSL2Current',
                        'defaultValue': 'C4',
                        'record': true,
                        'type': 'xA=='
                    },
                    {
                        'number': 153,
                        'name': 'TSL3Current',
                        'defaultValue': 'C4',
                        'record': true,
                        'type': 'xA=='
                    },
                    {
                        'number': 154,
                        'name': 'TSThermUtilized',
                        'defaultValue': 'C7',
                        'record': true,
                        'type': 'xw=='
                    },
                    {
                        'number': 155,
                        'name': 'TSGFCurrent',
                        'defaultValue': 'C3',
                        'record': true,
                        'type': 'ww=='
                    },
                    {
                        'number': 156,
                        'name': 'TSL1toL2Voltage',
                        'defaultValue': 'C7',
                        'record': true,
                        'type': 'xw=='
                    },
                    {
                        'number': 157,
                        'name': 'TSL2toL3Voltage',
                        'defaultValue': 'C7',
                        'record': true,
                        'type': 'xw=='
                    },
                    {
                        'number': 158,
                        'name': 'TSL3toL1Voltage',
                        'defaultValue': 'C7',
                        'record': true,
                        'type': 'xw=='
                    },
                    {
                        'number': 159,
                        'name': 'TSTotalRealPwr',
                        'defaultValue': 'C4',
                        'record': true,
                        'type': 'xA=='
                    },
                    {
                        'number': 160,
                        'name': 'TSTotalkVAR',
                        'defaultValue': 'C4',
                        'record': true,
                        'type': 'xA=='
                    },
                    {
                        'number': 161,
                        'name': 'TSTotalkVA',
                        'defaultValue': 'C4',
                        'record': true,
                        'type': 'xA=='
                    },
                    {
                        'number': 162,
                        'name': 'TSTotalPF',
                        'defaultValue': 'C3',
                        'record': true,
                        'type': 'ww=='
                    },
                    {
                        'number': 163,
                        'name': 'TripReset',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 164,
                        'name': 'ConfigPreset',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 165,
                        'name': 'ClearCommand',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 166,
                        'name': 'Reserved',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 167,
                        'name': 'Reserved',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 168,
                        'name': 'Reserved',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 169,
                        'name': 'Reserved',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 170,
                        'name': 'Reserved',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 171,
                        'name': 'FLASetting',
                        'defaultValue': 'C8',
                        'record': true,
                        'type': 'yA=='
                    },
                    {
                        'number': 172,
                        'name': 'TripClass',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 173,
                        'name': 'OLPTCResetMode',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 174,
                        'name': 'OLResetLevel',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 175,
                        'name': 'OLWarningLevel',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 176,
                        'name': 'SingleOrThreePh',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 177,
                        'name': 'FLA2Setting',
                        'defaultValue': 'C8',
                        'record': true,
                        'type': 'yA=='
                    },
                    {
                        'number': 178,
                        'name': 'Reserved',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 179,
                        'name': 'Reserved',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 180,
                        'name': 'Reserved',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 181,
                        'name': 'Reserved',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 182,
                        'name': 'Reserved',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 183,
                        'name': 'TripEnableI',
                        'defaultValue': 'D2',
                        'record': true,
                        'type': '0g=='
                    },
                    {
                        'number': 184,
                        'name': 'TripEnableV',
                        'defaultValue': 'D2',
                        'record': true,
                        'type': '0g=='
                    },
                    {
                        'number': 185,
                        'name': 'TripEnableP',
                        'defaultValue': 'D2',
                        'record': true,
                        'type': '0g=='
                    },
                    {
                        'number': 186,
                        'name': 'TripEnableC',
                        'defaultValue': 'D2',
                        'record': true,
                        'type': '0g=='
                    },
                    {
                        'number': 187,
                        'name': 'TripEnableA',
                        'defaultValue': 'D2',
                        'record': true,
                        'type': '0g=='
                    },
                    {
                        'number': 188,
                        'name': 'Reserved',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 189,
                        'name': 'WarningEnableI',
                        'defaultValue': 'D2',
                        'record': true,
                        'type': '0g=='
                    },
                    {
                        'number': 190,
                        'name': 'WarningEnableV',
                        'defaultValue': 'D2',
                        'record': true,
                        'type': '0g=='
                    },
                    {
                        'number': 191,
                        'name': 'WarningEnableP',
                        'defaultValue': 'D2',
                        'record': true,
                        'type': '0g=='
                    },
                    {
                        'number': 192,
                        'name': 'WarningEnableC',
                        'defaultValue': 'D2',
                        'record': true,
                        'type': '0g=='
                    },
                    {
                        'number': 193,
                        'name': 'WarningEnableA',
                        'defaultValue': 'D2',
                        'record': true,
                        'type': '0g=='
                    },
                    {
                        'number': 194,
                        'name': 'Reserved',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 195,
                        'name': 'SetOperatingMode',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 196,
                        'name': 'InPt00Assignment',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 197,
                        'name': 'InPt01Assignment',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 198,
                        'name': 'InPt02Assignment',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 199,
                        'name': 'InPt03Assignment',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 200,
                        'name': 'InPt04Assignment',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 201,
                        'name': 'InPt05Assignment',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 202,
                        'name': 'OutPt0Assignment',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 203,
                        'name': 'OutPt1Assignment',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 204,
                        'name': 'OutPt2Assignment',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 205,
                        'name': 'StartsPerHour',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 206,
                        'name': 'StartsInterval',
                        'defaultValue': 'C7',
                        'record': true,
                        'type': 'xw=='
                    },
                    {
                        'number': 207,
                        'name': 'PMTotalStarts',
                        'defaultValue': 'C7',
                        'record': true,
                        'type': 'xw=='
                    },
                    {
                        'number': 208,
                        'name': 'PMOperatingHours',
                        'defaultValue': 'C7',
                        'record': true,
                        'type': 'xw=='
                    },
                    {
                        'number': 209,
                        'name': 'ActFLA2wOutput',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 210,
                        'name': 'Reserved',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 211,
                        'name': 'SecurityPolicy',
                        'defaultValue': 'D2',
                        'record': true,
                        'type': '0g=='
                    },
                    {
                        'number': 212,
                        'name': 'Language',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 213,
                        'name': 'FeedbackTimeout',
                        'defaultValue': 'C7',
                        'record': true,
                        'type': 'xw=='
                    },
                    {
                        'number': 214,
                        'name': 'TransitionDelay',
                        'defaultValue': 'C7',
                        'record': true,
                        'type': 'xw=='
                    },
                    {
                        'number': 215,
                        'name': 'InterlockDelay',
                        'defaultValue': 'C7',
                        'record': true,
                        'type': 'xw=='
                    },
                    {
                        'number': 216,
                        'name': 'EmergencyStartEn',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 217,
                        'name': 'Reserved',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 218,
                        'name': 'Reserved',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 219,
                        'name': 'Reserved',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 220,
                        'name': 'Reserved',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 221,
                        'name': 'ControlModuleTyp',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 222,
                        'name': 'SensingModuleTyp',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 223,
                        'name': 'CommsModuleType',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 224,
                        'name': 'OperStationType',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 225,
                        'name': 'DigitalMod1Type',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 226,
                        'name': 'DigitalMod2Type',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 227,
                        'name': 'DigitalMod3Type',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 228,
                        'name': 'DigitalMod4Type',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 229,
                        'name': 'AnalogMod1Type',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 230,
                        'name': 'AnalogMod2Type',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 231,
                        'name': 'AnalogMod3Type',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 232,
                        'name': 'AnalogMod4Type',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 233,
                        'name': 'MismatchAction',
                        'defaultValue': 'D2',
                        'record': true,
                        'type': '0g=='
                    },
                    {
                        'number': 234,
                        'name': 'Reserved',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 235,
                        'name': 'Reserved',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 236,
                        'name': 'Reserved',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 237,
                        'name': 'Reserved',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 238,
                        'name': 'Reserved',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 239,
                        'name': 'PLInhibitTime',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 240,
                        'name': 'PLTripDelay',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 241,
                        'name': 'GroundFaultType',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 242,
                        'name': 'GFInhibitTime',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 243,
                        'name': 'GFTripDelay',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 244,
                        'name': 'GFTripLevel',
                        'defaultValue': 'C7',
                        'record': true,
                        'type': 'xw=='
                    },
                    {
                        'number': 245,
                        'name': 'GFWarningDelay',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 246,
                        'name': 'GFWarningLevel',
                        'defaultValue': 'C7',
                        'record': true,
                        'type': 'xw=='
                    },
                    {
                        'number': 247,
                        'name': 'GFFilter',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 248,
                        'name': 'GFMaxInhibit',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 249,
                        'name': 'StallEnabledTime',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 250,
                        'name': 'StallTripLevel',
                        'defaultValue': 'C7',
                        'record': true,
                        'type': 'xw=='
                    },
                    {
                        'number': 251,
                        'name': 'JamInhibitTime',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 252,
                        'name': 'JamTripDelay',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 253,
                        'name': 'JamTripLevel',
                        'defaultValue': 'C7',
                        'record': true,
                        'type': 'xw=='
                    },
                    {
                        'number': 254,
                        'name': 'JamWarningLevel',
                        'defaultValue': 'C7',
                        'record': true,
                        'type': 'xw=='
                    },
                    {
                        'number': 255,
                        'name': 'ULInhibitTime',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 256,
                        'name': 'ULTripDelay',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 257,
                        'name': 'ULTripLevel',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 258,
                        'name': 'ULWarningLevel',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 259,
                        'name': 'CIInhibitTime',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 260,
                        'name': 'CITripDelay',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 261,
                        'name': 'CITripLevel',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 262,
                        'name': 'CIWarningLevel',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 263,
                        'name': 'CTPrimary',
                        'defaultValue': 'C7',
                        'record': true,
                        'type': 'xw=='
                    },
                    {
                        'number': 264,
                        'name': 'CTSecondary',
                        'defaultValue': 'C7',
                        'record': true,
                        'type': 'xw=='
                    },
                    {
                        'number': 265,
                        'name': 'UCInhibitTime',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 266,
                        'name': 'L1UCTripDelay',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 267,
                        'name': 'L1UCTripLevel',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 268,
                        'name': 'L1UCWarningLevel',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 269,
                        'name': 'L2UCTripDelay',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 270,
                        'name': 'L2UCTripLevel',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 271,
                        'name': 'L2UCWarningLevel',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 272,
                        'name': 'L3UCTripDelay',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 273,
                        'name': 'L3UCTripLevel',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 274,
                        'name': 'L3UCWarningLevel',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 275,
                        'name': 'OCInhibitTime',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 276,
                        'name': 'L1OCTripDelay',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 277,
                        'name': 'L1OCTripLevel',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 278,
                        'name': 'L1OCWarningLevel',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 279,
                        'name': 'L2OCTripDelay',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 280,
                        'name': 'L2OCTripLevel',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 281,
                        'name': 'L2OCWarningLevel',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 282,
                        'name': 'L3OCTripDelay',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 283,
                        'name': 'L3OCTripLevel',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 284,
                        'name': 'L3OCWarningLevel',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 285,
                        'name': 'LineLossInhTime',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 286,
                        'name': 'L1LossTripDelay',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 287,
                        'name': 'L2LossTripDelay',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 288,
                        'name': 'L3LossTripDelay',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 289,
                        'name': 'OutputAssembly',
                        'defaultValue': 'C7',
                        'record': true,
                        'type': 'xw=='
                    },
                    {
                        'number': 290,
                        'name': 'InputAssembly',
                        'defaultValue': 'C7',
                        'record': true,
                        'type': 'xw=='
                    },
                    {
                        'number': 291,
                        'name': 'Datalink0',
                        'defaultValue': 'C7',
                        'record': true,
                        'type': 'xw=='
                    },
                    {
                        'number': 292,
                        'name': 'Datalink1',
                        'defaultValue': 'C7',
                        'record': true,
                        'type': 'xw=='
                    },
                    {
                        'number': 293,
                        'name': 'Datalink2',
                        'defaultValue': 'C7',
                        'record': true,
                        'type': 'xw=='
                    },
                    {
                        'number': 294,
                        'name': 'Datalink3',
                        'defaultValue': 'C7',
                        'record': true,
                        'type': 'xw=='
                    },
                    {
                        'number': 295,
                        'name': 'Datalink4',
                        'defaultValue': 'C7',
                        'record': true,
                        'type': 'xw=='
                    },
                    {
                        'number': 296,
                        'name': 'Datalink5',
                        'defaultValue': 'C7',
                        'record': true,
                        'type': 'xw=='
                    },
                    {
                        'number': 297,
                        'name': 'Datalink6',
                        'defaultValue': 'C7',
                        'record': true,
                        'type': 'xw=='
                    },
                    {
                        'number': 298,
                        'name': 'Datalink7',
                        'defaultValue': 'C7',
                        'record': true,
                        'type': 'xw=='
                    },
                    {
                        'number': 299,
                        'name': 'Reserved',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 300,
                        'name': 'Reserved',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 301,
                        'name': 'Reserved',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 302,
                        'name': 'Reserved',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 303,
                        'name': 'Reserved',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 304,
                        'name': 'OutPt00PrFltAct',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 305,
                        'name': 'OutPt00PrFltVal',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 306,
                        'name': 'OutPt00ComFltAct',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 307,
                        'name': 'OutPt00ComFltVal',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 308,
                        'name': 'OutPt00ComIdlAct',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 309,
                        'name': 'OutPt00ComIdlVal',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 310,
                        'name': 'OutPt01PrFltAct',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 311,
                        'name': 'OutPt01PrFltVal',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 312,
                        'name': 'OutPt01ComFltAct',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 313,
                        'name': 'OutPt01ComFltVal',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 314,
                        'name': 'OutPt01ComIdlAct',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 315,
                        'name': 'OutPt01ComIdlVal',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 316,
                        'name': 'OutPt02PrFltAct',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 317,
                        'name': 'OutPt02PrFltVal',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 318,
                        'name': 'OutPt02ComFltAct',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 319,
                        'name': 'OutPt02ComFltVal',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 320,
                        'name': 'OutPt02ComIdlAct',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 321,
                        'name': 'OutPt02ComIdlVal',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 322,
                        'name': 'OutDig1PrFltAct',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 323,
                        'name': 'OutDig1PrFltVal',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 324,
                        'name': 'OutDig1ComFltAct',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 325,
                        'name': 'OutDig1ComFltVal',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 326,
                        'name': 'OutDig1ComIdlAct',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 327,
                        'name': 'OutDig1ComIdlVal',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 328,
                        'name': 'OutDig2PrFltAct',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 329,
                        'name': 'OutDig2PrFltVal',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 330,
                        'name': 'OutDig2ComFltAct',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 331,
                        'name': 'OutDig2ComFltVal',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 332,
                        'name': 'OutDig2ComIdlAct',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 333,
                        'name': 'OutDig2ComIdlVal',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 334,
                        'name': 'OutDig3PrFltAct',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 335,
                        'name': 'OutDig3PrFltVal',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 336,
                        'name': 'OutDig3ComFltAct',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 337,
                        'name': 'OutDig3ComFltVal',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 338,
                        'name': 'OutDig3ComIdlAct',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 339,
                        'name': 'OutDig3ComIdlVal',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 340,
                        'name': 'OutDig4PrFltAct',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 341,
                        'name': 'OutDig4PrFltVal',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 342,
                        'name': 'OutDig4ComFltAct',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 343,
                        'name': 'OutDig4ComFltVal',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 344,
                        'name': 'OutDig4ComIdlAct',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 345,
                        'name': 'OutDig4ComIdlVal',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 346,
                        'name': 'CommOverride',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 347,
                        'name': 'NetworkOverride',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 348,
                        'name': 'PtDeviceOuts',
                        'defaultValue': 'D2',
                        'record': true,
                        'type': '0g=='
                    },
                    {
                        'number': 349,
                        'name': 'Reserved',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 350,
                        'name': 'PtDevOutCOSMask',
                        'defaultValue': 'D2',
                        'record': true,
                        'type': '0g=='
                    },
                    {
                        'number': 351,
                        'name': 'DLXUserDefData',
                        'defaultValue': 'C8',
                        'record': true,
                        'type': 'yA=='
                    },
                    {
                        'number': 352,
                        'name': 'VoltageMode',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 353,
                        'name': 'PTPrimary',
                        'defaultValue': 'C7',
                        'record': true,
                        'type': 'xw=='
                    },
                    {
                        'number': 354,
                        'name': 'PTSecondary',
                        'defaultValue': 'C7',
                        'record': true,
                        'type': 'xw=='
                    },
                    {
                        'number': 355,
                        'name': 'UVInhibitTime',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 356,
                        'name': 'UVTripDelay',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 357,
                        'name': 'UVTripLevel',
                        'defaultValue': 'C7',
                        'record': true,
                        'type': 'xw=='
                    },
                    {
                        'number': 358,
                        'name': 'UVWarningLevel',
                        'defaultValue': 'C7',
                        'record': true,
                        'type': 'xw=='
                    },
                    {
                        'number': 359,
                        'name': 'OVInhibitTime',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 360,
                        'name': 'OVTripDelay',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 361,
                        'name': 'OVTripLevel',
                        'defaultValue': 'C7',
                        'record': true,
                        'type': 'xw=='
                    },
                    {
                        'number': 362,
                        'name': 'OVWarningLevel',
                        'defaultValue': 'C7',
                        'record': true,
                        'type': 'xw=='
                    },
                    {
                        'number': 363,
                        'name': 'PhRotInhibitTime',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 364,
                        'name': 'PhaseRotTripType',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 365,
                        'name': 'VIBInhibitTime',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 366,
                        'name': 'VIBTripDelay',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 367,
                        'name': 'VIBTripLevel',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 368,
                        'name': 'VIBWarningLevel',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 369,
                        'name': 'UFInhibitTime',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 370,
                        'name': 'UFTripDelay',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 371,
                        'name': 'UFTripLevel',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 372,
                        'name': 'UFWarningLevel',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 373,
                        'name': 'OFInhibitTime',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 374,
                        'name': 'OFTripDelay',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 375,
                        'name': 'OFTripLevel',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 376,
                        'name': 'OFWarningLevel',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 377,
                        'name': 'PowerScale',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 378,
                        'name': 'UWInhibitTime',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 379,
                        'name': 'UWTripDelay',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 380,
                        'name': 'UWTripLevel',
                        'defaultValue': 'C4',
                        'record': true,
                        'type': 'xA=='
                    },
                    {
                        'number': 381,
                        'name': 'UWWarningLevel',
                        'defaultValue': 'C4',
                        'record': true,
                        'type': 'xA=='
                    },
                    {
                        'number': 382,
                        'name': 'OWInhibitTime',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 383,
                        'name': 'OWTripDelay',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 384,
                        'name': 'OWTripLevel',
                        'defaultValue': 'C4',
                        'record': true,
                        'type': 'xA=='
                    },
                    {
                        'number': 385,
                        'name': 'OWWarningLevel',
                        'defaultValue': 'C4',
                        'record': true,
                        'type': 'xA=='
                    },
                    {
                        'number': 386,
                        'name': 'UVARCInhibitTime',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 387,
                        'name': 'UVARCTripDelay',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 388,
                        'name': 'UVARCTripLevel',
                        'defaultValue': 'C4',
                        'record': true,
                        'type': 'xA=='
                    },
                    {
                        'number': 389,
                        'name': 'UVARCWarnLevel',
                        'defaultValue': 'C4',
                        'record': true,
                        'type': 'xA=='
                    },
                    {
                        'number': 390,
                        'name': 'OVARCInhibitTime',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 391,
                        'name': 'OVARCTripDelay',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 392,
                        'name': 'OVARCTripLevel',
                        'defaultValue': 'C4',
                        'record': true,
                        'type': 'xA=='
                    },
                    {
                        'number': 393,
                        'name': 'OVARCWarnLevel',
                        'defaultValue': 'C4',
                        'record': true,
                        'type': 'xA=='
                    },
                    {
                        'number': 394,
                        'name': 'UVARGInhibitTime',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 395,
                        'name': 'UVARGTripDelay',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 396,
                        'name': 'UVARGTripLevel',
                        'defaultValue': 'C4',
                        'record': true,
                        'type': 'xA=='
                    },
                    {
                        'number': 397,
                        'name': 'UVARGWarnLevel',
                        'defaultValue': 'C4',
                        'record': true,
                        'type': 'xA=='
                    },
                    {
                        'number': 398,
                        'name': 'OVARGInhibitTime',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 399,
                        'name': 'OVARGTripDelay',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 400,
                        'name': 'OVARGTripLevel',
                        'defaultValue': 'C4',
                        'record': true,
                        'type': 'xA=='
                    },
                    {
                        'number': 401,
                        'name': 'OVARGWarnLevel',
                        'defaultValue': 'C4',
                        'record': true,
                        'type': 'xA=='
                    },
                    {
                        'number': 402,
                        'name': 'UVAInhibitTime',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 403,
                        'name': 'UVATripDelay',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 404,
                        'name': 'UVATripLevel',
                        'defaultValue': 'C4',
                        'record': true,
                        'type': 'xA=='
                    },
                    {
                        'number': 405,
                        'name': 'UVAWarningLevel',
                        'defaultValue': 'C4',
                        'record': true,
                        'type': 'xA=='
                    },
                    {
                        'number': 406,
                        'name': 'OVAInhibitTime',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 407,
                        'name': 'OVATripDelay',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 408,
                        'name': 'OVATripLevel',
                        'defaultValue': 'C4',
                        'record': true,
                        'type': 'xA=='
                    },
                    {
                        'number': 409,
                        'name': 'OVAWarningLevel',
                        'defaultValue': 'C4',
                        'record': true,
                        'type': 'xA=='
                    },
                    {
                        'number': 410,
                        'name': 'UPFLagInhibTime',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 411,
                        'name': 'UPFLagTripDelay',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 412,
                        'name': 'UPFLagTripLevel',
                        'defaultValue': 'C2',
                        'record': true,
                        'type': 'wg=='
                    },
                    {
                        'number': 413,
                        'name': 'UPFLagWarnLevel',
                        'defaultValue': 'C2',
                        'record': true,
                        'type': 'wg=='
                    },
                    {
                        'number': 414,
                        'name': 'OPFLagInhibTime',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 415,
                        'name': 'OPFLagTripDelay',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 416,
                        'name': 'OPFLagTripLevel',
                        'defaultValue': 'C2',
                        'record': true,
                        'type': 'wg=='
                    },
                    {
                        'number': 417,
                        'name': 'OPFLagWarnLevel',
                        'defaultValue': 'C2',
                        'record': true,
                        'type': 'wg=='
                    },
                    {
                        'number': 418,
                        'name': 'UPFLeadInhibTime',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 419,
                        'name': 'UPFLeadTripDelay',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 420,
                        'name': 'UPFLeadTripLevel',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 421,
                        'name': 'UPFLeadWarnLevel',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 422,
                        'name': 'OPFLeadInhibTime',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 423,
                        'name': 'OPFLeadTripDelay',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 424,
                        'name': 'OPFLeadTripLevel',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 425,
                        'name': 'OPFLeadWarnLevel',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 426,
                        'name': 'DemandPeriod',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 427,
                        'name': 'NumberOfPeriods',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 428,
                        'name': 'Screen1Param1',
                        'defaultValue': 'C7',
                        'record': true,
                        'type': 'xw=='
                    },
                    {
                        'number': 429,
                        'name': 'Screen1Param2',
                        'defaultValue': 'C7',
                        'record': true,
                        'type': 'xw=='
                    },
                    {
                        'number': 430,
                        'name': 'Screen2Param1',
                        'defaultValue': 'C7',
                        'record': true,
                        'type': 'xw=='
                    },
                    {
                        'number': 431,
                        'name': 'Screen2Param2',
                        'defaultValue': 'C7',
                        'record': true,
                        'type': 'xw=='
                    },
                    {
                        'number': 432,
                        'name': 'Screen3Param1',
                        'defaultValue': 'C7',
                        'record': true,
                        'type': 'xw=='
                    },
                    {
                        'number': 433,
                        'name': 'Screen3Param2',
                        'defaultValue': 'C7',
                        'record': true,
                        'type': 'xw=='
                    },
                    {
                        'number': 434,
                        'name': 'Screen4Param1',
                        'defaultValue': 'C7',
                        'record': true,
                        'type': 'xw=='
                    },
                    {
                        'number': 435,
                        'name': 'Screen4Param2',
                        'defaultValue': 'C7',
                        'record': true,
                        'type': 'xw=='
                    },
                    {
                        'number': 436,
                        'name': 'DisplayTimeout',
                        'defaultValue': 'C7',
                        'record': true,
                        'type': 'xw=='
                    },
                    {
                        'number': 437,
                        'name': 'InAnMod1Ch00Type',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 438,
                        'name': 'InAMod1Ch0Format',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 439,
                        'name': 'InAMod1C0TmpUnit',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 440,
                        'name': 'InAMod1C0FiltFrq',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 441,
                        'name': 'InAMod1C0OpCktSt',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 442,
                        'name': 'InAnMod1Ch0RTDEn',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 443,
                        'name': 'InAMod1C0TripDly',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 444,
                        'name': 'InAMod1C0TripLvl',
                        'defaultValue': 'C3',
                        'record': true,
                        'type': 'ww=='
                    },
                    {
                        'number': 445,
                        'name': 'InAMod1C0WarnLvl',
                        'defaultValue': 'C3',
                        'record': true,
                        'type': 'ww=='
                    },
                    {
                        'number': 446,
                        'name': 'InAnMod1Ch01Type',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 447,
                        'name': 'InAMod1Ch1Format',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 448,
                        'name': 'InAMod1C1TmpUnit',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 449,
                        'name': 'InAMod1C1FiltFrq',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 450,
                        'name': 'InAMod1C1OpCktSt',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 451,
                        'name': 'InAnMod1Ch1RTDEn',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 452,
                        'name': 'InAMod1C1TripDly',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 453,
                        'name': 'InAMod1C1TripLvl',
                        'defaultValue': 'C3',
                        'record': true,
                        'type': 'ww=='
                    },
                    {
                        'number': 454,
                        'name': 'InAMod1C1WarnLvl',
                        'defaultValue': 'C3',
                        'record': true,
                        'type': 'ww=='
                    },
                    {
                        'number': 455,
                        'name': 'InAnMod1Ch02Type',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 456,
                        'name': 'InAMod1Ch2Format',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 457,
                        'name': 'InAMod1C2TmpUnit',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 458,
                        'name': 'InAMod1C2FiltFrq',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 459,
                        'name': 'InAMod1C2OpCktSt',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 460,
                        'name': 'InAnMod1Ch2RTDEn',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 461,
                        'name': 'InAMod1C2TripDly',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 462,
                        'name': 'InAMod1C2TripLvl',
                        'defaultValue': 'C3',
                        'record': true,
                        'type': 'ww=='
                    },
                    {
                        'number': 463,
                        'name': 'InAMod1C2WarnLvl',
                        'defaultValue': 'C3',
                        'record': true,
                        'type': 'ww=='
                    },
                    {
                        'number': 464,
                        'name': 'OutAnMod1Type',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 465,
                        'name': 'OutAnMod1Select',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 466,
                        'name': 'OutAnMod1EFltAct',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 467,
                        'name': 'OutAnMod1PFltAct',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 468,
                        'name': 'InAnMod2Ch00Type',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 469,
                        'name': 'InAMod2Ch0Format',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 470,
                        'name': 'InAMod2C0TmpUnit',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 471,
                        'name': 'InAMod2C0FiltFrq',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 472,
                        'name': 'InAMod2C0OpCktSt',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 473,
                        'name': 'InAnMod2Ch0RTDEn',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 474,
                        'name': 'InAMod2C0TripDly',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 475,
                        'name': 'InAMod2C0TripLvl',
                        'defaultValue': 'C3',
                        'record': true,
                        'type': 'ww=='
                    },
                    {
                        'number': 476,
                        'name': 'InAMod2C0WarnLvl',
                        'defaultValue': 'C3',
                        'record': true,
                        'type': 'ww=='
                    },
                    {
                        'number': 477,
                        'name': 'InAnMod2Ch01Type',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 478,
                        'name': 'InAMod2Ch1Format',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 479,
                        'name': 'InAMod2C1TmpUnit',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 480,
                        'name': 'InAMod2C1FiltFrq',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 481,
                        'name': 'InAMod2C1OpCktSt',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 482,
                        'name': 'InAnMod2Ch1RTDEn',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 483,
                        'name': 'InAMod2C1TripDly',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 484,
                        'name': 'InAMod2C1TripLvl',
                        'defaultValue': 'C3',
                        'record': true,
                        'type': 'ww=='
                    },
                    {
                        'number': 485,
                        'name': 'InAMod2C1WarnLvl',
                        'defaultValue': 'C3',
                        'record': true,
                        'type': 'ww=='
                    },
                    {
                        'number': 486,
                        'name': 'InAnMod2Ch02Type',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 487,
                        'name': 'InAMod2Ch2Format',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 488,
                        'name': 'InAMod2C2TmpUnit',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 489,
                        'name': 'InAMod2C2FiltFrq',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 490,
                        'name': 'InAMod2C2OpCktSt',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 491,
                        'name': 'InAnMod2Ch2RTDEn',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 492,
                        'name': 'InAMod2C2TripDly',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 493,
                        'name': 'InAMod2C2TripLvl',
                        'defaultValue': 'C3',
                        'record': true,
                        'type': 'ww=='
                    },
                    {
                        'number': 494,
                        'name': 'InAMod2C2WarnLvl',
                        'defaultValue': 'C3',
                        'record': true,
                        'type': 'ww=='
                    },
                    {
                        'number': 495,
                        'name': 'OutAnMod2Type',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 496,
                        'name': 'OutAnMod2Select',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 497,
                        'name': 'OutAnMod2EFltAct',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 498,
                        'name': 'OutAnMod2PFltAct',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 499,
                        'name': 'InAnMod3Ch00Type',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 500,
                        'name': 'InAMod3Ch0Format',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 501,
                        'name': 'InAMod3C0TmpUnit',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 502,
                        'name': 'InAMod3C0FiltFrq',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 503,
                        'name': 'InAMod3C0OpCktSt',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 504,
                        'name': 'InAnMod3Ch0RTDEn',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 505,
                        'name': 'InAMod3C0TripDly',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 506,
                        'name': 'InAMod3C0TripLvl',
                        'defaultValue': 'C3',
                        'record': true,
                        'type': 'ww=='
                    },
                    {
                        'number': 507,
                        'name': 'InAMod3C0WarnLvl',
                        'defaultValue': 'C3',
                        'record': true,
                        'type': 'ww=='
                    },
                    {
                        'number': 508,
                        'name': 'InAnMod3Ch01Type',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 509,
                        'name': 'InAMod3Ch1Format',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 510,
                        'name': 'InAMod3C1TmpUnit',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 511,
                        'name': 'InAMod3C1FiltFrq',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 512,
                        'name': 'InAMod3C1OpCktSt',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 513,
                        'name': 'InAnMod3Ch1RTDEn',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 514,
                        'name': 'InAMod3C1TripDly',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 515,
                        'name': 'InAMod3C1TripLvl',
                        'defaultValue': 'C3',
                        'record': true,
                        'type': 'ww=='
                    },
                    {
                        'number': 516,
                        'name': 'InAMod3C1WarnLvl',
                        'defaultValue': 'C3',
                        'record': true,
                        'type': 'ww=='
                    },
                    {
                        'number': 517,
                        'name': 'InAnMod3Ch02Type',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 518,
                        'name': 'InAMod3Ch2Format',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 519,
                        'name': 'InAMod3C2TmpUnit',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 520,
                        'name': 'InAMod3C2FiltFrq',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 521,
                        'name': 'InAMod3C2OpCktSt',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 522,
                        'name': 'InAnMod3Ch2RTDEn',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 523,
                        'name': 'InAMod3C2TripDly',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 524,
                        'name': 'InAMod3C2TripLvl',
                        'defaultValue': 'C3',
                        'record': true,
                        'type': 'ww=='
                    },
                    {
                        'number': 525,
                        'name': 'InAMod3C2WarnLvl',
                        'defaultValue': 'C3',
                        'record': true,
                        'type': 'ww=='
                    },
                    {
                        'number': 526,
                        'name': 'OutAnMod3Type',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 527,
                        'name': 'OutAnMod3Select',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 528,
                        'name': 'OutAnMod3EFltAct',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 529,
                        'name': 'OutAnMod3PFltAct',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 530,
                        'name': 'InAnMod4Ch00Type',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 531,
                        'name': 'InAMod4Ch0Format',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 532,
                        'name': 'InAMod4C0TmpUnit',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 533,
                        'name': 'InAMod4C0FiltFrq',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 534,
                        'name': 'InAMod4C0OpCktSt',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 535,
                        'name': 'InAnMod4Ch0RTDEn',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 536,
                        'name': 'InAMod4C0TripDly',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 537,
                        'name': 'InAMod4C0TripLvl',
                        'defaultValue': 'C3',
                        'record': true,
                        'type': 'ww=='
                    },
                    {
                        'number': 538,
                        'name': 'InAMod4C0WarnLvl',
                        'defaultValue': 'C3',
                        'record': true,
                        'type': 'ww=='
                    },
                    {
                        'number': 539,
                        'name': 'InAnMod4Ch01Type',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 540,
                        'name': 'InAMod4Ch1Format',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 541,
                        'name': 'InAMod4C1TmpUnit',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 542,
                        'name': 'InAMod4C1FiltFrq',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 543,
                        'name': 'InAMod4C1OpCktSt',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 544,
                        'name': 'InAnMod4Ch1RTDEn',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 545,
                        'name': 'InAMod4C1TripDly',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 546,
                        'name': 'InAMod4C1TripLvl',
                        'defaultValue': 'C3',
                        'record': true,
                        'type': 'ww=='
                    },
                    {
                        'number': 547,
                        'name': 'InAMod4C1WarnLvl',
                        'defaultValue': 'C3',
                        'record': true,
                        'type': 'ww=='
                    },
                    {
                        'number': 548,
                        'name': 'InAnMod4Ch02Type',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 549,
                        'name': 'InAMod4Ch2Format',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 550,
                        'name': 'InAMod4C2TmpUnit',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 551,
                        'name': 'InAMod4C2FiltFrq',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 552,
                        'name': 'InAMod4C2OpCktSt',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 553,
                        'name': 'InAnMod4Ch2RTDEn',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 554,
                        'name': 'InAMod4C2TripDly',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 555,
                        'name': 'InAMod4C2TripLvl',
                        'defaultValue': 'C3',
                        'record': true,
                        'type': 'ww=='
                    },
                    {
                        'number': 556,
                        'name': 'InAMod4C2WarnLvl',
                        'defaultValue': 'C3',
                        'record': true,
                        'type': 'ww=='
                    },
                    {
                        'number': 557,
                        'name': 'OutAnMod4Type',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 558,
                        'name': 'OutAnMod4Select',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 559,
                        'name': 'OutAnMod4EFltAct',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 560,
                        'name': 'OutAnMod4PFltAct',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 561,
                        'name': 'FnlFltValStDur',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    },
                    {
                        'number': 562,
                        'name': 'OutPt00FnlFltVal',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 563,
                        'name': 'OutPt01FnlFltVal',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 564,
                        'name': 'OutPt02FnlFltVal',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 565,
                        'name': 'OutDig1FnlFltVal',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 566,
                        'name': 'OutDig2FnlFltVal',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 567,
                        'name': 'OutDig3FnlFltVal',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 568,
                        'name': 'OutDig4FnlFltVal',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 569,
                        'name': 'NetStrtComFltAct',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 570,
                        'name': 'NetStrtComFltVal',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 571,
                        'name': 'NetStrtComIdlAct',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 572,
                        'name': 'NetStrtComIdlVal',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 573,
                        'name': 'NetStrtFnlFltVal',
                        'defaultValue': 'C1',
                        'record': true,
                        'type': 'wQ=='
                    },
                    {
                        'number': 574,
                        'name': 'VoltageScale',
                        'defaultValue': 'C6',
                        'record': true,
                        'type': 'xg=='
                    }
                    ]";
    }
}