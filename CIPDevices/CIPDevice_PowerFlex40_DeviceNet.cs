using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using powerFlexBackup.cipdevice.deviceParameterObjects;

namespace powerFlexBackup.cipdevice
{
    /// <summary>
    /// PowerFlex 40 accessed via DeviceNet (22-COMM-D adapter)
    /// </summary>
    public class CIPDevice_PowerFlex40_DeviceNet : CIPDevice_PowerFlex40
    {
        public CIPDevice_PowerFlex40_DeviceNet(
            String deviceAddress,
            Sres.Net.EEIP.EEIPClient eeipClient,
            byte[] CIPRoute,
            IOptions<AppConfiguration> options,
            ILogger logger,
            IdentityObject identityObject) :
            base(deviceAddress, eeipClient, CIPRoute, options, logger, identityObject)
        {
        }

        // 22-COMM-D (DeviceNet) comm adapter parameters (169-189)
        // Confirm reported types with actual device
        protected override string commAdapterParameterListJSON => @"[
            { 'number': '169', 'name': 'Mode', 'defaultValue': '0', 'record': 'false', 'type': 'CA==' },
            { 'number': '170', 'name': 'DN Addr Cfg', 'defaultValue': '63', 'record': 'true', 'type': 'CA==' },
            { 'number': '171', 'name': 'DN Addr Act', 'defaultValue': '63', 'record': 'false', 'type': 'CA==' },
            { 'number': '172', 'name': 'DN Rate Cfg', 'defaultValue': '3', 'record': 'true', 'type': 'CA==' },
            { 'number': '173', 'name': 'DN Rate Act', 'defaultValue': '0', 'record': 'false', 'type': 'CA==' },
            { 'number': '174', 'name': 'Reset Module', 'defaultValue': '0', 'record': 'true', 'type': 'CA==' },
            { 'number': '175', 'name': 'Comm Flt Action', 'defaultValue': '0', 'record': 'true', 'type': 'CA==' },
            { 'number': '176', 'name': 'Idle Flt Action', 'defaultValue': '0', 'record': 'true', 'type': 'CA==' },
            { 'number': '177', 'name': 'DN Act Cfg', 'defaultValue': '0', 'record': 'false', 'type': 'CA==' },
            { 'number': '178', 'name': 'Flt Cfg Logic', 'defaultValue': '0', 'record': 'true', 'type': 'AQ==' },
            { 'number': '179', 'name': 'Flt Cfg Ref', 'defaultValue': '0', 'record': 'true', 'type': 'CQ==' },
            { 'number': '180', 'name': 'COS Status Mask', 'defaultValue': '0', 'record': 'true', 'type': 'AQ==' },
            { 'number': '181', 'name': 'COS Fdbk Change', 'defaultValue': '0', 'record': 'true', 'type': 'CQ==' },
            { 'number': '182', 'name': 'COS/Cyc Interval', 'defaultValue': '0', 'record': 'false', 'type': 'Ag==' },
            { 'number': '183', 'name': 'DSI I/O Cfg', 'defaultValue': '1', 'record': 'true', 'type': 'GA==' },
            { 'number': '184', 'name': 'DSI I/O Act', 'defaultValue': '1', 'record': 'false', 'type': 'GA==' },
            { 'number': '185', 'name': 'Drv 0 Addr', 'defaultValue': '1', 'record': 'true', 'type': 'CA==' },
            { 'number': '186', 'name': 'Drv 1 Addr', 'defaultValue': '2', 'record': 'true', 'type': 'CA==' },
            { 'number': '187', 'name': 'Drv 2 Addr', 'defaultValue': '3', 'record': 'true', 'type': 'CA==' },
            { 'number': '188', 'name': 'Drv 3 Addr', 'defaultValue': '4', 'record': 'true', 'type': 'CA==' },
            { 'number': '189', 'name': 'Drv 4 Addr', 'defaultValue': '5', 'record': 'true', 'type': 'CA==' }
        ]";
    }
}
