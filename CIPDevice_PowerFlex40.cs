using Newtonsoft.Json;
using powerFlexBackup.cipdevice.deviceParameterObjects;

namespace powerFlexBackup.cipdevice
{
    //FIXME: Add support for PowerFlex 40 series
     [SupportedDevice("PowerFlex 40 Series", 127,  40)] 
    public class CIPDevice_PowerFlex40 : CIPDevice{
        public CIPDevice_PowerFlex40(String deviceAddress, Sres.Net.EEIP.EEIPClient eeipClient) :
            base(deviceAddress, eeipClient)
        {
            setParameterClassID(0x0F);
            setInstanceAttributes();
            //setDeviceParameterList(JsonConvert.DeserializeObject<List<DeviceParameter>>(parameterListJSON)!);         
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

        //FIXME: Create rules for saving these paramters. 
        public string parameterListJSON = @"[]";
    }
}