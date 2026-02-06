using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using powerFlexBackup.cipdevice.deviceParameterObjects;

namespace powerFlexBackup.cipdevice
{
    [SupportedDevice("PowerFlex 750 Series", 123, 1168)]
    [SupportedDevice("PowerFlex 750 Series", 123, 2192)]
    [SupportedDevice("PowerFlex 750 Series", 142, 1168)]
    [SupportedDevice("PowerFlex 750 Series", 142, 2192)]
    [SupportedDevice("PowerFlex 750 Series", 143, 2192)]
    public class CIPDevice_PowerFlex750 : CIPDevice_PowerFlex750Base
    {
        public CIPDevice_PowerFlex750(String deviceAddress, Sres.Net.EEIP.EEIPClient eeipClient, byte[] CIPRoute, IOptions<AppConfiguration> options, ILogger logger, IdentityObject identityObject) :
            base(deviceAddress, eeipClient, CIPRoute, options, logger, identityObject)
        {
        }

        protected override void InitializePortMap()
        {
            portMap.Add(new PortParameterMap(0, 0x0000));
            portMap.Add(new PortParameterMap(1, 0x4400));
            portMap.Add(new PortParameterMap(2, 0x4800));
            portMap.Add(new PortParameterMap(3, 0x4C00));
            portMap.Add(new PortParameterMap(4, 0x5000));
            portMap.Add(new PortParameterMap(5, 0x5400));
            portMap.Add(new PortParameterMap(6, 0x5800));
            portMap.Add(new PortParameterMap(7, 0x5C00));
            portMap.Add(new PortParameterMap(8, 0x6000));
            portMap.Add(new PortParameterMap(9, 0x6400));
            portMap.Add(new PortParameterMap(10, 0x6800));
            portMap.Add(new PortParameterMap(11, 0x6C00));
            portMap.Add(new PortParameterMap(12, 0x7000));
            portMap.Add(new PortParameterMap(13, 0x7400));
            portMap.Add(new PortParameterMap(14, 0x7800));
        }
    }
}
