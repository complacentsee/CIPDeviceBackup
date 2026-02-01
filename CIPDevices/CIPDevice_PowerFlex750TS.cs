using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using powerFlexBackup.cipdevice.deviceParameterObjects;

namespace powerFlexBackup.cipdevice
{
    [SupportedDevice("PowerFlex 750TS Series", 143, 7568)]
    public class CIPDevice_PowerFlex750TS : CIPDevice_PowerFlex750Base
    {
        public CIPDevice_PowerFlex750TS(String deviceAddress, Sres.Net.EEIP.EEIPClient eeipClient, byte[] CIPRoute, IOptions<AppConfiguration> options, ILogger logger) :
            base(deviceAddress, eeipClient, CIPRoute, options, logger)
        {
        }

        protected override void InitializePortMap()
        {
            portMap.Add(new PortParameterMap(0,  0x0000));
            portMap.Add(new PortParameterMap(1,  0x1000));
            portMap.Add(new PortParameterMap(2,  0x2000));
            portMap.Add(new PortParameterMap(3,  0x3000));
            portMap.Add(new PortParameterMap(4,  0x4000));
            portMap.Add(new PortParameterMap(5,  0x5000));
            portMap.Add(new PortParameterMap(6,  0x6000));
            portMap.Add(new PortParameterMap(7,  0x7000));
            portMap.Add(new PortParameterMap(8,  0x8000));
            portMap.Add(new PortParameterMap(9,  0x9000));
            portMap.Add(new PortParameterMap(10, 0xA000));
            portMap.Add(new PortParameterMap(11, 0xB000));
            portMap.Add(new PortParameterMap(12, 0xC000));
            portMap.Add(new PortParameterMap(13, 0xD000));
            portMap.Add(new PortParameterMap(14, 0xE000));
        }
    }
}
