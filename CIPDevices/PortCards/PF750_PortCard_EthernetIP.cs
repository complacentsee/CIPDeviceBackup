namespace powerFlexBackup.cipdevice.PortCards
{
    /// <summary>
    /// Embedded EtherNet/IP Communication Adapter
    ///
    /// PowerFlex 750 Series built-in EtherNet/IP port.
    /// Typically found on Port 4 of PF753/PF755 drives.
    ///
    /// HOST class (0x9F): Datalinks, fault actions, fault configuration (predefined, 54 params).
    /// DEVICE class (0x93): IP config, subnet, gateway, web features,
    /// peer-to-peer config (predefined, 43 params).
    /// </summary>
    public class PF750_PortCard_EthernetIP : PowerFlex750PortCard
    {
        public override ushort[] ProductCodes => [57760];
        public override string ProductName => "EtherNet/IP";
        public override int ClassID => 0x9F;
        public override bool UseScatteredRead => false; // we get errors at parameter 44 on scattered reads.
        public override int? DeviceClassID => 0x93;

        protected override string hostParameterListJSON => @"[
            { 'number': '1', 'name': 'DL From Net 01', 'defaultValue': '0', 'record': 'true', 'type': 'yA==' },
            { 'number': '2', 'name': 'DL From Net 02', 'defaultValue': '0', 'record': 'true', 'type': 'yA==' },
            { 'number': '3', 'name': 'DL From Net 03', 'defaultValue': '0', 'record': 'true', 'type': 'yA==' },
            { 'number': '4', 'name': 'DL From Net 04', 'defaultValue': '0', 'record': 'true', 'type': 'yA==' },
            { 'number': '5', 'name': 'DL From Net 05', 'defaultValue': '0', 'record': 'true', 'type': 'yA==' },
            { 'number': '6', 'name': 'DL From Net 06', 'defaultValue': '0', 'record': 'true', 'type': 'yA==' },
            { 'number': '7', 'name': 'DL From Net 07', 'defaultValue': '0', 'record': 'true', 'type': 'yA==' },
            { 'number': '8', 'name': 'DL From Net 08', 'defaultValue': '0', 'record': 'true', 'type': 'yA==' },
            { 'number': '9', 'name': 'DL From Net 09', 'defaultValue': '0', 'record': 'true', 'type': 'yA==' },
            { 'number': '10', 'name': 'DL From Net 10', 'defaultValue': '0', 'record': 'true', 'type': 'yA==' },
            { 'number': '11', 'name': 'DL From Net 11', 'defaultValue': '0', 'record': 'true', 'type': 'yA==' },
            { 'number': '12', 'name': 'DL From Net 12', 'defaultValue': '0', 'record': 'true', 'type': 'yA==' },
            { 'number': '13', 'name': 'DL From Net 13', 'defaultValue': '0', 'record': 'true', 'type': 'yA==' },
            { 'number': '14', 'name': 'DL From Net 14', 'defaultValue': '0', 'record': 'true', 'type': 'yA==' },
            { 'number': '15', 'name': 'DL From Net 15', 'defaultValue': '0', 'record': 'true', 'type': 'yA==' },
            { 'number': '16', 'name': 'DL From Net 16', 'defaultValue': '0', 'record': 'true', 'type': 'yA==' },
            { 'number': '17', 'name': 'DL To Net 01', 'defaultValue': '0', 'record': 'true', 'type': 'yA==' },
            { 'number': '18', 'name': 'DL To Net 02', 'defaultValue': '0', 'record': 'true', 'type': 'yA==' },
            { 'number': '19', 'name': 'DL To Net 03', 'defaultValue': '0', 'record': 'true', 'type': 'yA==' },
            { 'number': '20', 'name': 'DL To Net 04', 'defaultValue': '0', 'record': 'true', 'type': 'yA==' },
            { 'number': '21', 'name': 'DL To Net 05', 'defaultValue': '0', 'record': 'true', 'type': 'yA==' },
            { 'number': '22', 'name': 'DL To Net 06', 'defaultValue': '0', 'record': 'true', 'type': 'yA==' },
            { 'number': '23', 'name': 'DL To Net 07', 'defaultValue': '0', 'record': 'true', 'type': 'yA==' },
            { 'number': '24', 'name': 'DL To Net 08', 'defaultValue': '0', 'record': 'true', 'type': 'yA==' },
            { 'number': '25', 'name': 'DL To Net 09', 'defaultValue': '0', 'record': 'true', 'type': 'yA==' },
            { 'number': '26', 'name': 'DL To Net 10', 'defaultValue': '0', 'record': 'true', 'type': 'yA==' },
            { 'number': '27', 'name': 'DL To Net 11', 'defaultValue': '0', 'record': 'true', 'type': 'yA==' },
            { 'number': '28', 'name': 'DL To Net 12', 'defaultValue': '0', 'record': 'true', 'type': 'yA==' },
            { 'number': '29', 'name': 'DL To Net 13', 'defaultValue': '0', 'record': 'true', 'type': 'yA==' },
            { 'number': '30', 'name': 'DL To Net 14', 'defaultValue': '0', 'record': 'true', 'type': 'yA==' },
            { 'number': '31', 'name': 'DL To Net 15', 'defaultValue': '0', 'record': 'true', 'type': 'yA==' },
            { 'number': '32', 'name': 'DL To Net 16', 'defaultValue': '0', 'record': 'true', 'type': 'yA==' },
            { 'number': '33', 'name': 'Comm Flt Action', 'defaultValue': '0', 'record': 'true', 'type': 'yA==' },
            { 'number': '34', 'name': 'Idle Flt Action', 'defaultValue': '0', 'record': 'true', 'type': 'yA==' },
            { 'number': '35', 'name': 'Peer Flt Action', 'defaultValue': '0', 'record': 'true', 'type': 'yA==' },
            { 'number': '36', 'name': 'Msg Flt Action', 'defaultValue': '0', 'record': 'true', 'type': 'yA==' },
            { 'number': '37', 'name': 'Flt Cfg Logic', 'defaultValue': '00000000000000000000000000000000', 'record': 'true', 'type': '0w==' },
            { 'number': '38', 'name': 'Flt Cfg Ref', 'defaultValue': '0', 'record': 'true', 'type': 'yg==' },
            { 'number': '39', 'name': 'Flt Cfg DL 01', 'defaultValue': '0', 'record': 'true', 'type': 'yA==' },
            { 'number': '40', 'name': 'Flt Cfg DL 02', 'defaultValue': '0', 'record': 'true', 'type': 'yA==' },
            { 'number': '41', 'name': 'Flt Cfg DL 03', 'defaultValue': '0', 'record': 'true', 'type': 'yA==' },
            { 'number': '42', 'name': 'Flt Cfg DL 04', 'defaultValue': '0', 'record': 'true', 'type': 'yA==' },
            { 'number': '43', 'name': 'Flt Cfg DL 05', 'defaultValue': '0', 'record': 'true', 'type': 'yA==' },
            { 'number': '44', 'name': 'Flt Cfg DL 06', 'defaultValue': '0', 'record': 'true', 'type': 'yA==' },
            { 'number': '45', 'name': 'Flt Cfg DL 07', 'defaultValue': '0', 'record': 'true', 'type': 'yA==' },
            { 'number': '46', 'name': 'Flt Cfg DL 08', 'defaultValue': '0', 'record': 'true', 'type': 'yA==' },
            { 'number': '47', 'name': 'Flt Cfg DL 09', 'defaultValue': '0', 'record': 'true', 'type': 'yA==' },
            { 'number': '48', 'name': 'Flt Cfg DL 10', 'defaultValue': '0', 'record': 'true', 'type': 'yA==' },
            { 'number': '49', 'name': 'Flt Cfg DL 11', 'defaultValue': '0', 'record': 'true', 'type': 'yA==' },
            { 'number': '50', 'name': 'Flt Cfg DL 12', 'defaultValue': '0', 'record': 'true', 'type': 'yA==' },
            { 'number': '51', 'name': 'Flt Cfg DL 13', 'defaultValue': '0', 'record': 'true', 'type': 'yA==' },
            { 'number': '52', 'name': 'Flt Cfg DL 14', 'defaultValue': '0', 'record': 'true', 'type': 'yA==' },
            { 'number': '53', 'name': 'Flt Cfg DL 15', 'defaultValue': '0', 'record': 'true', 'type': 'yA==' },
            { 'number': '54', 'name': 'Flt Cfg DL 16', 'defaultValue': '0', 'record': 'true', 'type': 'yA==' }
        ]";

        protected override string? deviceParameterListJSON => @"[
            { 'number': '1', 'name': 'Operating Mode', 'defaultValue': '0', 'record': 'false', 'type': 'xg==' },
            { 'number': '2', 'name': 'Port Number', 'defaultValue': '5', 'record': 'false', 'type': 'xg==' },
            { 'number': '3', 'name': 'DLs From Net Act', 'defaultValue': '0', 'record': 'false', 'type': 'xg==' },
            { 'number': '4', 'name': 'DLs To Net Act', 'defaultValue': '0', 'record': 'false', 'type': 'xg==' },
            { 'number': '5', 'name': 'Net Addr Sel', 'defaultValue': '3', 'record': 'true', 'type': 'xg==' },
            { 'number': '6', 'name': 'Net Addr Src', 'defaultValue': '3', 'record': 'false', 'type': 'xg==' },
            { 'number': '7', 'name': 'IP Addr Cfg 1', 'defaultValue': '0', 'record': 'true', 'type': 'xg==' },
            { 'number': '8', 'name': 'IP Addr Cfg 2', 'defaultValue': '0', 'record': 'true', 'type': 'xg==' },
            { 'number': '9', 'name': 'IP Addr Cfg 3', 'defaultValue': '0', 'record': 'true', 'type': 'xg==' },
            { 'number': '10', 'name': 'IP Addr Cfg 4', 'defaultValue': '0', 'record': 'true', 'type': 'xg==' },
            { 'number': '11', 'name': 'Subnet Cfg 1', 'defaultValue': '0', 'record': 'true', 'type': 'xg==' },
            { 'number': '12', 'name': 'Subnet Cfg 2', 'defaultValue': '0', 'record': 'true', 'type': 'xg==' },
            { 'number': '13', 'name': 'Subnet Cfg 3', 'defaultValue': '0', 'record': 'true', 'type': 'xg==' },
            { 'number': '14', 'name': 'Subnet Cfg 4', 'defaultValue': '0', 'record': 'true', 'type': 'xg==' },
            { 'number': '15', 'name': 'Gateway Cfg 1', 'defaultValue': '0', 'record': 'true', 'type': 'xg==' },
            { 'number': '16', 'name': 'Gateway Cfg 2', 'defaultValue': '0', 'record': 'true', 'type': 'xg==' },
            { 'number': '17', 'name': 'Gateway Cfg 3', 'defaultValue': '0', 'record': 'true', 'type': 'xg==' },
            { 'number': '18', 'name': 'Gateway Cfg 4', 'defaultValue': '0', 'record': 'true', 'type': 'xg==' },
            { 'number': '19', 'name': 'Net Rate Cfg 1', 'defaultValue': '0', 'record': 'true', 'type': 'xg==' },
            { 'number': '20', 'name': 'Net Rate Act 1', 'defaultValue': '0', 'record': 'false', 'type': 'xg==' },
            { 'number': '21', 'name': 'Net Rate Cfg 2', 'defaultValue': '0', 'record': 'true', 'type': 'xg==' },
            { 'number': '22', 'name': 'Net Rate Act 2', 'defaultValue': '0', 'record': 'false', 'type': 'xg==' },
            { 'number': '23', 'name': 'Reserved', 'defaultValue': '0', 'record': 'false', 'type': 'xg==' },
            { 'number': '24', 'name': 'Reserved', 'defaultValue': '0', 'record': 'false', 'type': 'xg==' },
            { 'number': '25', 'name': 'Reset Module', 'defaultValue': '0', 'record': 'true', 'type': 'xg==' },
            { 'number': '26', 'name': 'Web Enable', 'defaultValue': '0', 'record': 'true', 'type': 'xg==' },
            { 'number': '27', 'name': 'Web Features', 'defaultValue': '00000001', 'record': 'true', 'type': '0Q==' },
            { 'number': '28', 'name': 'DLs Fr Peer Cfg', 'defaultValue': '0', 'record': 'true', 'type': 'xg==' },
            { 'number': '29', 'name': 'DLs Fr Peer Act', 'defaultValue': '0', 'record': 'false', 'type': 'xg==' },
            { 'number': '30', 'name': 'Logic Src Cfg', 'defaultValue': '0', 'record': 'true', 'type': 'xg==' },
            { 'number': '31', 'name': 'Ref Src Cfg', 'defaultValue': '0', 'record': 'true', 'type': 'xg==' },
            { 'number': '32', 'name': 'Fr Peer Timeout', 'defaultValue': '1000', 'record': 'true', 'type': 'xw==' },
            { 'number': '33', 'name': 'Fr Peer Addr 1', 'defaultValue': '0', 'record': 'true', 'type': 'xg==' },
            { 'number': '34', 'name': 'Fr Peer Addr 2', 'defaultValue': '0', 'record': 'true', 'type': 'xg==' },
            { 'number': '35', 'name': 'Fr Peer Addr 3', 'defaultValue': '0', 'record': 'true', 'type': 'xg==' },
            { 'number': '36', 'name': 'Fr Peer Addr 4', 'defaultValue': '0', 'record': 'true', 'type': 'xg==' },
            { 'number': '37', 'name': 'Fr Peer Enable', 'defaultValue': '0', 'record': 'true', 'type': 'xg==' },
            { 'number': '38', 'name': 'Fr Peer Status', 'defaultValue': '0', 'record': 'false', 'type': 'xg==' },
            { 'number': '39', 'name': 'DLs To Peer Cfg', 'defaultValue': '0', 'record': 'true', 'type': 'xg==' },
            { 'number': '40', 'name': 'DLs To Peer Act', 'defaultValue': '0', 'record': 'false', 'type': 'xg==' },
            { 'number': '41', 'name': 'To Peer Period', 'defaultValue': '1000', 'record': 'true', 'type': 'xw==' },
            { 'number': '42', 'name': 'To Peer Skip', 'defaultValue': '1', 'record': 'true', 'type': 'xg==' },
            { 'number': '43', 'name': 'To Peer Enable', 'defaultValue': '0', 'record': 'true', 'type': 'xg==' }
        ]";
    }
}
