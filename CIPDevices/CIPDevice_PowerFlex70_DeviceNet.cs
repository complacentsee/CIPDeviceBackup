using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using powerFlexBackup.cipdevice.deviceParameterObjects;

namespace powerFlexBackup.cipdevice
{
    /// <summary>
    /// PowerFlex 70 accessed via DeviceNet (20-COMM-D adapter)
    /// DeviceNet scattered read requires an extra 0x00 byte at end of request
    /// per 20-COMM-D documentation: "Source Length = (param_count * 6) + 1"
    /// Note: Routing through Ethernet/IP -> DNB -> DeviceNet has lower message size limits
    /// </summary>
    [SupportedDevice("PowerFlex 70 DeviceNet", 121, 5938, true)]
    [SupportedDevice("PowerFlex 70 DeviceNet", 121, 7474, true)]
    public class CIPDevice_PowerFlex70_DeviceNet : CIPDevice_PowerFlex70
    {
        protected override int ScatteredReadBatchSize => 22;

        public CIPDevice_PowerFlex70_DeviceNet(
            String deviceAddress,
            Sres.Net.EEIP.EEIPClient eeipClient,
            byte[] CIPRoute,
            IOptions<AppConfiguration> options,
            ILogger logger,
            IdentityObject identityObject) :
            base(deviceAddress, eeipClient, CIPRoute, options, logger, identityObject)
        {
        }

        /// <summary>
        /// DeviceNet scattered read request format:
        /// For each parameter: UINT16 param_number + UINT16 pad + UINT16 pad (6 bytes per param)
        /// Plus one extra required 0x00 byte at the end
        /// </summary>
        protected override byte[] BuildScatteredReadRequest(List<int> parameterNumbers)
        {
            // DeviceNet requires extra byte: (param_count * 6) + 1
            byte[] requestData = new byte[(parameterNumbers.Count * 6) + 2];
            int offset = 0;
            foreach (int paramNum in parameterNumbers)
            {
                requestData[offset++] = (byte)(paramNum & 0xFF);
                requestData[offset++] = (byte)((paramNum >> 8) & 0xFF);
                requestData[offset++] = 0;
                requestData[offset++] = 0;
                requestData[offset++] = 0;
                requestData[offset++] = 0;
            }

            // Extra required word (always zero) for DeviceNet
            requestData[offset++] = 0;
            requestData[offset++]   = 0;

            return requestData;
        }

        // 20-COMM-D (DeviceNet) comm adapter parameters (629-669)
        protected override string commAdapterParameterListJSON => @"[
            { 'number': '629', 'name': 'DN Addr Cfg', 'defaultValue': '1', 'record': 'true', 'type': 'CA==' },
            { 'number': '630', 'name': 'DN Addr Act', 'defaultValue': '1', 'record': 'false', 'type': 'CA==' },
            { 'number': '631', 'name': 'DN Rate Cfg', 'defaultValue': '3', 'record': 'true', 'type': 'CA==' },
            { 'number': '632', 'name': 'DN Rate Act', 'defaultValue': '0', 'record': 'false', 'type': 'CA==' },
            { 'number': '633', 'name': 'Ref / Fdbk Size', 'defaultValue': '0', 'record': 'false', 'type': 'CA==' },
            { 'number': '634', 'name': 'Datalink Size', 'defaultValue': '0', 'record': 'false', 'type': 'CA==' },
            { 'number': '635', 'name': 'Reset Module', 'defaultValue': '0', 'record': 'true', 'type': 'CA==' },
            { 'number': '636', 'name': 'Comm Flt Action', 'defaultValue': '0', 'record': 'true', 'type': 'CA==' },
            { 'number': '637', 'name': 'Idle Flt Action', 'defaultValue': '0', 'record': 'true', 'type': 'CA==' },
            { 'number': '638', 'name': 'DN Active Cfg', 'defaultValue': '0', 'record': 'false', 'type': 'CA==' },
            { 'number': '639', 'name': 'DPI I/O Cfg', 'defaultValue': '3', 'record': 'true', 'type': 'GA==' },
            { 'number': '640', 'name': 'DPI I/O Act', 'defaultValue': '3', 'record': 'false', 'type': 'GA==' },
            { 'number': '641', 'name': 'Flt Cfg Logic', 'defaultValue': '0', 'record': 'true', 'type': 'AQ==' },
            { 'number': '642', 'name': 'Flt Cfg Ref', 'defaultValue': '0', 'record': 'true', 'type': 'CQ==' },
            { 'number': '643', 'name': 'Flt Cfg A1 In', 'defaultValue': '0', 'record': 'true', 'type': 'CQ==' },
            { 'number': '644', 'name': 'Flt Cfg A2 In', 'defaultValue': '0', 'record': 'true', 'type': 'CQ==' },
            { 'number': '645', 'name': 'Flt Cfg B1 In', 'defaultValue': '0', 'record': 'true', 'type': 'CQ==' },
            { 'number': '646', 'name': 'Flt Cfg B2 In', 'defaultValue': '0', 'record': 'true', 'type': 'CQ==' },
            { 'number': '647', 'name': 'Flt Cfg C1 In', 'defaultValue': '0', 'record': 'true', 'type': 'CQ==' },
            { 'number': '648', 'name': 'Flt Cfg C2 In', 'defaultValue': '0', 'record': 'true', 'type': 'CQ==' },
            { 'number': '649', 'name': 'Flt Cfg D1 In', 'defaultValue': '0', 'record': 'true', 'type': 'CQ==' },
            { 'number': '650', 'name': 'Flt Cfg D2 In', 'defaultValue': '0', 'record': 'true', 'type': 'CQ==' },
            { 'number': '651', 'name': 'M-S Input', 'defaultValue': '1', 'record': 'true', 'type': 'GA==' },
            { 'number': '652', 'name': 'M-S Output', 'defaultValue': '3', 'record': 'true', 'type': 'GA==' },
            { 'number': '653', 'name': 'COS Status Mask', 'defaultValue': '0', 'record': 'true', 'type': 'AQ==' },
            { 'number': '654', 'name': 'COS Fdbk Change', 'defaultValue': '0', 'record': 'true', 'type': 'CQ==' },
            { 'number': '655', 'name': 'COS/Cyc Interval', 'defaultValue': '0', 'record': 'false', 'type': 'Ag==' },
            { 'number': '656', 'name': 'Peer A Input', 'defaultValue': '0', 'record': 'true', 'type': 'Ag==' },
            { 'number': '657', 'name': 'Peer B Input', 'defaultValue': '0', 'record': 'true', 'type': 'Ag==' },
            { 'number': '658', 'name': 'Peer Cmd Mask', 'defaultValue': '0', 'record': 'true', 'type': 'AQ==' },
            { 'number': '659', 'name': 'Peer Ref Adjust', 'defaultValue': '0', 'record': 'true', 'type': 'Ag==' },
            { 'number': '660', 'name': 'Peer Flt Action', 'defaultValue': '0', 'record': 'true', 'type': 'CA==' },
            { 'number': '661', 'name': 'Peer Node to Inp', 'defaultValue': '0', 'record': 'true', 'type': 'CA==' },
            { 'number': '662', 'name': 'Peer Inp Timeout', 'defaultValue': '1000', 'record': 'true', 'type': 'Ag==' },
            { 'number': '663', 'name': 'Peer Inp Enable', 'defaultValue': '0', 'record': 'true', 'type': 'CA==' },
            { 'number': '664', 'name': 'Peer Inp Status', 'defaultValue': '0', 'record': 'false', 'type': 'CA==' },
            { 'number': '665', 'name': 'Peer A Output', 'defaultValue': '0', 'record': 'true', 'type': 'CA==' },
            { 'number': '666', 'name': 'Peer B Output', 'defaultValue': '0', 'record': 'true', 'type': 'CA==' },
            { 'number': '667', 'name': 'Peer Out Enable', 'defaultValue': '0', 'record': 'true', 'type': 'CA==' },
            { 'number': '668', 'name': 'Peer Out Time', 'defaultValue': '100', 'record': 'true', 'type': 'Ag==' },
            { 'number': '669', 'name': 'Peer Out Skip', 'defaultValue': '1', 'record': 'true', 'type': 'Ag==' }
        ]";
    }
}
