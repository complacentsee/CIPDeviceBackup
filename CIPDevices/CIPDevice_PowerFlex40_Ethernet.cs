using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using powerFlexBackup.cipdevice.deviceParameterObjects;

namespace powerFlexBackup.cipdevice
{
    /// <summary>
    /// PowerFlex 40 accessed via Ethernet (22-COMM-E adapter)
    /// </summary>
    [SupportedDevice("PowerFlex 40 Series", 127, 40, true)]
    public class CIPDevice_PowerFlex40_Ethernet : CIPDevice_PowerFlex40
    {
        public CIPDevice_PowerFlex40_Ethernet(
            String deviceAddress,
            Sres.Net.EEIP.EEIPClient eeipClient,
            byte[] CIPRoute,
            IOptions<AppConfiguration> options,
            ILogger logger,
            IdentityObject identityObject) :
            base(deviceAddress, eeipClient, CIPRoute, options, logger, identityObject)
        {
        }

        // 22-COMM-E (Ethernet) comm adapter parameters (169-197)
        protected override string commAdapterParameterListJSON => @"[
            { 'number': '168', 'name': 'Mode', 'defaultValue': '0', 'record': 'false', 'type': 'CA==' },
            { 'number': '169', 'name': 'BOOTP', 'defaultValue': '1', 'record': 'true', 'type': 'xw==' },
            { 'number': '170', 'name': 'IP Addr Cfg 1', 'defaultValue': '0', 'record': 'true', 'type': 'xw==' },
            { 'number': '171', 'name': 'IP Addr Cfg 2', 'defaultValue': '0', 'record': 'true', 'type': 'xw==' },
            { 'number': '172', 'name': 'IP Addr Cfg 3', 'defaultValue': '0', 'record': 'true', 'type': 'xw==' },
            { 'number': '173', 'name': 'IP Addr Cfg 4', 'defaultValue': '0', 'record': 'true', 'type': 'xw==' },
            { 'number': '174', 'name': 'Subnet Cfg 1', 'defaultValue': '0', 'record': 'true', 'type': 'xw==' },
            { 'number': '175', 'name': 'Subnet Cfg 2', 'defaultValue': '0', 'record': 'true', 'type': 'xw==' },
            { 'number': '176', 'name': 'Subnet Cfg 3', 'defaultValue': '0', 'record': 'true', 'type': 'xw==' },
            { 'number': '177', 'name': 'Subnet Cfg 4', 'defaultValue': '0', 'record': 'true', 'type': 'xw==' },
            { 'number': '178', 'name': 'Gateway Cfg 1', 'defaultValue': '0', 'record': 'true', 'type': 'xw==' },
            { 'number': '179', 'name': 'Gateway Cfg 2', 'defaultValue': '0', 'record': 'true', 'type': 'xw==' },
            { 'number': '180', 'name': 'Gateway Cfg 3', 'defaultValue': '0', 'record': 'true', 'type': 'xw==' },
            { 'number': '181', 'name': 'Gateway Cfg 4', 'defaultValue': '0', 'record': 'true', 'type': 'xw==' },
            { 'number': '182', 'name': 'EN Rate Cfg', 'defaultValue': '0', 'record': 'true', 'type': 'xw==' },
            { 'number': '183', 'name': 'EN Rate Act', 'defaultValue': '0', 'record': 'false', 'type': 'xw==' },
            { 'number': '184', 'name': 'Reset Module', 'defaultValue': '0', 'record': 'true', 'type': 'xw==' },
            { 'number': '185', 'name': 'Comm Flt Action', 'defaultValue': '0', 'record': 'true', 'type': 'xw==' },
            { 'number': '186', 'name': 'Idle Flt Action', 'defaultValue': '0', 'record': 'true', 'type': 'xw==' },
            { 'number': '187', 'name': 'Flt Cfg Logic', 'defaultValue': '0000000000000000', 'record': 'true', 'type': '0g==' },
            { 'number': '188', 'name': 'Flt Cfg Ref', 'defaultValue': '0', 'record': 'true', 'type': 'xw==' },
            { 'number': '189', 'name': 'DSI I/O Cfg', 'defaultValue': '0', 'record': 'true', 'type': 'xw==' },
            { 'number': '190', 'name': 'DSI I/O Act', 'defaultValue': '0000000000000000', 'record': 'false', 'type': '0g==' },
            { 'number': '191', 'name': 'Drv 0 Addr', 'defaultValue': '100', 'record': 'true', 'type': 'xw==' },
            { 'number': '192', 'name': 'Drv 1 Addr', 'defaultValue': '101', 'record': 'true', 'type': 'xw==' },
            { 'number': '193', 'name': 'Drv 2 Addr', 'defaultValue': '102', 'record': 'true', 'type': 'xw==' },
            { 'number': '194', 'name': 'Drv 3 Addr', 'defaultValue': '103', 'record': 'true', 'type': 'xw==' },
            { 'number': '195', 'name': 'Drv 4 Addr', 'defaultValue': '104', 'record': 'true', 'type': 'xw==' },
            { 'number': '196', 'name': 'Web Enable', 'defaultValue': '0', 'record': 'false', 'type': 'xw==' },
            { 'number': '197', 'name': 'Web Features', 'defaultValue': '0000000000000011', 'record': 'true', 'type': '0g==' }
        ]";
    }
}
