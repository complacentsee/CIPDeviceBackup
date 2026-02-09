namespace powerFlexBackup.cipdevice.PortCards
{
    /// <summary>
    /// Port 06 - 20-COMM-E Port Card
    ///
    /// 56 parameters (47 writable, 9 read-only).
    /// Types: xg== (0xC6 INT), xw== (0xC7 UINT), yA== (0xC8 DINT), 0Q== (0xD1 WORD), 0g== (0xD2 DWORD)
    /// </summary>
    public class PF750_PortCard_Port0620COMME : PowerFlex750PortCard
    {
        public override ushort[] ProductCodes => [8192];
        public override string ProductName => "20-COMM-E";
        public override int HostClassID => 0x9F;
        public override int? DeviceClassID => 0x93;

        protected override string hostParameterListJSON => @"[]";
                
        protected override string? deviceParameterListJSON => @"[
            { 'number': '1', 'name': 'DPI Port', 'defaultValue': '5', 'record': 'false', 'type': 'xg==' },
            { 'number': '2', 'name': 'DPI Data Rate', 'defaultValue': '0', 'record': 'false', 'type': 'xg==' },
            { 'number': '3', 'name': 'BOOTP', 'defaultValue': '1', 'record': 'true', 'type': 'xg==' },
            { 'number': '4', 'name': 'IP Addr Cfg 1', 'defaultValue': '0', 'record': 'true', 'type': 'xg==' },
            { 'number': '5', 'name': 'IP Addr Cfg 2', 'defaultValue': '0', 'record': 'true', 'type': 'xg==' },
            { 'number': '6', 'name': 'IP Addr Cfg 3', 'defaultValue': '0', 'record': 'true', 'type': 'xg==' },
            { 'number': '7', 'name': 'IP Addr Cfg 4', 'defaultValue': '0', 'record': 'true', 'type': 'xg==' },
            { 'number': '8', 'name': 'Subnet Cfg 1', 'defaultValue': '0', 'record': 'true', 'type': 'xg==' },
            { 'number': '9', 'name': 'Subnet Cfg 2', 'defaultValue': '0', 'record': 'true', 'type': 'xg==' },
            { 'number': '10', 'name': 'Subnet Cfg 3', 'defaultValue': '0', 'record': 'true', 'type': 'xg==' },
            { 'number': '11', 'name': 'Subnet Cfg 4', 'defaultValue': '0', 'record': 'true', 'type': 'xg==' },
            { 'number': '12', 'name': 'Gateway Cfg 1', 'defaultValue': '0', 'record': 'true', 'type': 'xg==' },
            { 'number': '13', 'name': 'Gateway Cfg 2', 'defaultValue': '0', 'record': 'true', 'type': 'xg==' },
            { 'number': '14', 'name': 'Gateway Cfg 3', 'defaultValue': '0', 'record': 'true', 'type': 'xg==' },
            { 'number': '15', 'name': 'Gateway Cfg 4', 'defaultValue': '0', 'record': 'true', 'type': 'xg==' },
            { 'number': '16', 'name': 'EN Rate Cfg', 'defaultValue': '0', 'record': 'true', 'type': 'xg==' },
            { 'number': '17', 'name': 'EN Rate Act', 'defaultValue': '0', 'record': 'false', 'type': 'xg==' },
            { 'number': '18', 'name': 'Ref / Fdbk Size', 'defaultValue': '0', 'record': 'false', 'type': 'xg==' },
            { 'number': '19', 'name': 'Datalink Size', 'defaultValue': '0', 'record': 'false', 'type': 'xg==' },
            { 'number': '20', 'name': 'Reset Module', 'defaultValue': '0', 'record': 'true', 'type': 'xg==' },
            { 'number': '21', 'name': 'Comm Flt Action', 'defaultValue': '0', 'record': 'true', 'type': 'xg==' },
            { 'number': '22', 'name': 'Idle Flt Action', 'defaultValue': '0', 'record': 'true', 'type': 'xg==' },
            { 'number': '23', 'name': 'DPI I/O Cfg', 'defaultValue': '00000001', 'record': 'true', 'type': '0Q==' },
            { 'number': '24', 'name': 'DPI I/O Act', 'defaultValue': '00000000', 'record': 'false', 'type': '0Q==' },
            { 'number': '25', 'name': 'Flt Cfg Logic', 'defaultValue': '0000000000000000', 'record': 'true', 'type': '0g==' },
            { 'number': '26', 'name': 'Flt Cfg Ref', 'defaultValue': '0', 'record': 'true', 'type': 'yA==' },
            { 'number': '27', 'name': 'Flt Cfg A1 In', 'defaultValue': '0', 'record': 'true', 'type': 'yA==' },
            { 'number': '28', 'name': 'Flt Cfg A2 In', 'defaultValue': '0', 'record': 'true', 'type': 'yA==' },
            { 'number': '29', 'name': 'Flt Cfg B1 In', 'defaultValue': '0', 'record': 'true', 'type': 'yA==' },
            { 'number': '30', 'name': 'Flt Cfg B2 In', 'defaultValue': '0', 'record': 'true', 'type': 'yA==' },
            { 'number': '31', 'name': 'Flt Cfg C1 In', 'defaultValue': '0', 'record': 'true', 'type': 'yA==' },
            { 'number': '32', 'name': 'Flt Cfg C2 In', 'defaultValue': '0', 'record': 'true', 'type': 'yA==' },
            { 'number': '33', 'name': 'Flt Cfg D1 In', 'defaultValue': '0', 'record': 'true', 'type': 'yA==' },
            { 'number': '34', 'name': 'Flt Cfg D2 In', 'defaultValue': '0', 'record': 'true', 'type': 'yA==' },
            { 'number': '35', 'name': 'M-S Input', 'defaultValue': '00000001', 'record': 'true', 'type': '0Q==' },
            { 'number': '36', 'name': 'M-S Output', 'defaultValue': '00000001', 'record': 'true', 'type': '0Q==' },
            { 'number': '37', 'name': 'Ref Adjust', 'defaultValue': '10000', 'record': 'true', 'type': 'xw==' },
            { 'number': '38', 'name': 'Peer A Input', 'defaultValue': '0', 'record': 'true', 'type': 'xg==' },
            { 'number': '39', 'name': 'Peer B Input', 'defaultValue': '0', 'record': 'true', 'type': 'xg==' },
            { 'number': '40', 'name': 'Peer Cmd Mask', 'defaultValue': '0000000000000000', 'record': 'true', 'type': '0g==' },
            { 'number': '41', 'name': 'Peer Flt Action', 'defaultValue': '0', 'record': 'true', 'type': 'xg==' },
            { 'number': '42', 'name': 'Peer Inp Addr 1', 'defaultValue': '0', 'record': 'true', 'type': 'xg==' },
            { 'number': '43', 'name': 'Peer Inp Addr 2', 'defaultValue': '0', 'record': 'true', 'type': 'xg==' },
            { 'number': '44', 'name': 'Peer Inp Addr 3', 'defaultValue': '0', 'record': 'true', 'type': 'xg==' },
            { 'number': '45', 'name': 'Peer Inp Addr 4', 'defaultValue': '0', 'record': 'true', 'type': 'xg==' },
            { 'number': '46', 'name': 'Peer Inp Timeout', 'defaultValue': '1000', 'record': 'true', 'type': 'xw==' },
            { 'number': '47', 'name': 'Peer Inp Enable', 'defaultValue': '0', 'record': 'true', 'type': 'xg==' },
            { 'number': '48', 'name': 'Peer Inp Status', 'defaultValue': '0', 'record': 'false', 'type': 'xg==' },
            { 'number': '49', 'name': 'Peer A Output', 'defaultValue': '0', 'record': 'true', 'type': 'xg==' },
            { 'number': '50', 'name': 'Peer B Output', 'defaultValue': '0', 'record': 'true', 'type': 'xg==' },
            { 'number': '51', 'name': 'Peer Out Enable', 'defaultValue': '0', 'record': 'true', 'type': 'xg==' },
            { 'number': '52', 'name': 'Peer Out Time', 'defaultValue': '1000', 'record': 'true', 'type': 'xw==' },
            { 'number': '53', 'name': 'Peer Out Skip', 'defaultValue': '1', 'record': 'true', 'type': 'xg==' },
            { 'number': '54', 'name': 'Reserved', 'defaultValue': '0', 'record': 'false', 'type': 'xg==' },
            { 'number': '55', 'name': 'Web Enable', 'defaultValue': '0', 'record': 'false', 'type': 'xg==' },
            { 'number': '56', 'name': 'Web Features', 'defaultValue': '00000001', 'record': 'true', 'type': '0Q==' }
        ]";
    }
}
