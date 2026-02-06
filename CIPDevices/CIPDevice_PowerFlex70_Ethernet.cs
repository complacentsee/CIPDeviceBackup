using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using powerFlexBackup.cipdevice.deviceParameterObjects;

namespace powerFlexBackup.cipdevice
{
    [SupportedDevice("PowerFlex 70 Series", 123, 50, true)]
    public class CIPDevice_PowerFlex70_Ethernet : CIPDevice_PowerFlex70
    {
        public CIPDevice_PowerFlex70_Ethernet(
            String deviceAddress,
            Sres.Net.EEIP.EEIPClient eeipClient,
            byte[] CIPRoute,
            IOptions<AppConfiguration> options,
            ILogger logger,
            IdentityObject identityObject) :
            base(deviceAddress, eeipClient, CIPRoute, options, logger, identityObject)
        {
        }

        // 20-COMM-E (Ethernet) comm adapter parameters (629-682)
        protected override string commAdapterParameterListJSON => @"[
            { 'number': '629', 'name': 'BOOTP', 'defaultValue': '1', 'record': 'true', 'type': 'xg==' },
            { 'number': '630', 'name': 'IP Addr Cfg 1', 'defaultValue': '0', 'record': 'true', 'type': 'xg==' },
            { 'number': '631', 'name': 'IP Addr Cfg 2', 'defaultValue': '0', 'record': 'true', 'type': 'xg==' },
            { 'number': '632', 'name': 'IP Addr Cfg 3', 'defaultValue': '0', 'record': 'true', 'type': 'xg==' },
            { 'number': '633', 'name': 'IP Addr Cfg 4', 'defaultValue': '0', 'record': 'true', 'type': 'xg==' },
            { 'number': '634', 'name': 'Subnet Cfg 1', 'defaultValue': '0', 'record': 'true', 'type': 'xg==' },
            { 'number': '635', 'name': 'Subnet Cfg 2', 'defaultValue': '0', 'record': 'true', 'type': 'xg==' },
            { 'number': '636', 'name': 'Subnet Cfg 3', 'defaultValue': '0', 'record': 'true', 'type': 'xg==' },
            { 'number': '637', 'name': 'Subnet Cfg 4', 'defaultValue': '0', 'record': 'true', 'type': 'xg==' },
            { 'number': '638', 'name': 'Gateway Cfg 1', 'defaultValue': '0', 'record': 'true', 'type': 'xg==' },
            { 'number': '639', 'name': 'Gateway Cfg 2', 'defaultValue': '0', 'record': 'true', 'type': 'xg==' },
            { 'number': '640', 'name': 'Gateway Cfg 3', 'defaultValue': '0', 'record': 'true', 'type': 'xg==' },
            { 'number': '641', 'name': 'Gateway Cfg 4', 'defaultValue': '0', 'record': 'true', 'type': 'xg==' },
            { 'number': '642', 'name': 'EN Rate Cfg', 'defaultValue': '0', 'record': 'true', 'type': 'xg==' },
            { 'number': '643', 'name': 'EN Rate Act', 'defaultValue': '0', 'record': 'false', 'type': 'xg==' },
            { 'number': '644', 'name': 'Ref \/ Fdbk Size', 'defaultValue': '0', 'record': 'false', 'type': 'xg==' },
            { 'number': '645', 'name': 'Datalink Size', 'defaultValue': '0', 'record': 'false', 'type': 'xg==' },
            { 'number': '646', 'name': 'Reset Module', 'defaultValue': '0', 'record': 'true', 'type': 'xg==' },
            { 'number': '647', 'name': 'Comm Flt Action', 'defaultValue': '0', 'record': 'true', 'type': 'xg==' },
            { 'number': '648', 'name': 'Idle Flt Action', 'defaultValue': '0', 'record': 'true', 'type': 'xg==' },
            { 'number': '649', 'name': 'DPI I\/O Cfg', 'defaultValue': '00000001', 'record': 'true', 'type': '0Q==' },
            { 'number': '650', 'name': 'DPI I\/O Act', 'defaultValue': '00000000', 'record': 'false', 'type': '0Q==' },
            { 'number': '651', 'name': 'Flt Cfg Logic', 'defaultValue': '0000000000000000', 'record': 'true', 'type': '0g==' },
            { 'number': '652', 'name': 'Flt Cfg Ref', 'defaultValue': '0', 'record': 'true', 'type': 'yA==' },
            { 'number': '653', 'name': 'Flt Cfg A1 In', 'defaultValue': '0', 'record': 'true', 'type': 'yA==' },
            { 'number': '654', 'name': 'Flt Cfg A2 In', 'defaultValue': '0', 'record': 'true', 'type': 'yA==' },
            { 'number': '655', 'name': 'Flt Cfg B1 In', 'defaultValue': '0', 'record': 'true', 'type': 'yA==' },
            { 'number': '656', 'name': 'Flt Cfg B2 In', 'defaultValue': '0', 'record': 'true', 'type': 'yA==' },
            { 'number': '657', 'name': 'Flt Cfg C1 In', 'defaultValue': '0', 'record': 'true', 'type': 'yA==' },
            { 'number': '658', 'name': 'Flt Cfg C2 In', 'defaultValue': '0', 'record': 'true', 'type': 'yA==' },
            { 'number': '659', 'name': 'Flt Cfg D1 In', 'defaultValue': '0', 'record': 'true', 'type': 'yA==' },
            { 'number': '660', 'name': 'Flt Cfg D2 In', 'defaultValue': '0', 'record': 'true', 'type': 'yA==' },
            { 'number': '661', 'name': 'M-S Input', 'defaultValue': '00000001', 'record': 'true', 'type': '0Q==' },
            { 'number': '662', 'name': 'M-S Output', 'defaultValue': '00000001', 'record': 'true', 'type': '0Q==' },
            { 'number': '663', 'name': 'Ref Adjust', 'defaultValue': '10000', 'record': 'true', 'type': 'xw==' },
            { 'number': '664', 'name': 'Peer A Input', 'defaultValue': '0', 'record': 'true', 'type': 'xg==' },
            { 'number': '665', 'name': 'Peer B Input', 'defaultValue': '0', 'record': 'true', 'type': 'xg==' },
            { 'number': '666', 'name': 'Peer Cmd Mask', 'defaultValue': '0000000000000000', 'record': 'true', 'type': '0g==' },
            { 'number': '667', 'name': 'Peer Flt Action', 'defaultValue': '0', 'record': 'true', 'type': 'xg==' },
            { 'number': '668', 'name': 'Peer Inp Addr 1', 'defaultValue': '0', 'record': 'true', 'type': 'xg==' },
            { 'number': '669', 'name': 'Peer Inp Addr 2', 'defaultValue': '0', 'record': 'true', 'type': 'xg==' },
            { 'number': '670', 'name': 'Peer Inp Addr 3', 'defaultValue': '0', 'record': 'true', 'type': 'xg==' },
            { 'number': '671', 'name': 'Peer Inp Addr 4', 'defaultValue': '0', 'record': 'true', 'type': 'xg==' },
            { 'number': '672', 'name': 'Peer Inp Timeout', 'defaultValue': '1000', 'record': 'true', 'type': 'xw==' },
            { 'number': '673', 'name': 'Peer Inp Enable', 'defaultValue': '0', 'record': 'true', 'type': 'xg==' },
            { 'number': '674', 'name': 'Peer Inp Status', 'defaultValue': '0', 'record': 'false', 'type': 'xg==' },
            { 'number': '675', 'name': 'Peer A Output', 'defaultValue': '0', 'record': 'true', 'type': 'xg==' },
            { 'number': '676', 'name': 'Peer B Output', 'defaultValue': '0', 'record': 'true', 'type': 'xg==' },
            { 'number': '677', 'name': 'Peer Out Enable', 'defaultValue': '0', 'record': 'true', 'type': 'xg==' },
            { 'number': '678', 'name': 'Peer Out Time', 'defaultValue': '1000', 'record': 'true', 'type': 'xw==' },
            { 'number': '679', 'name': 'Peer Out Skip', 'defaultValue': '1', 'record': 'true', 'type': 'xg==' },
            { 'number': '680', 'name': 'Reserved', 'defaultValue': '0', 'record': 'false', 'type': 'xg==' },
            { 'number': '681', 'name': 'Web Enable', 'defaultValue': '0', 'record': 'false', 'type': 'xg==' },
            { 'number': '682', 'name': 'Web Features', 'defaultValue': '00000001', 'record': 'true', 'type': '0Q==' }
        ]";
    }
}
