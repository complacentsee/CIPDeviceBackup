namespace powerFlexBackup.cipdevice.PortCards
{
    /// <summary>
    /// Embedded EtherNet/IP Communication Adapter
    ///
    /// PowerFlex 750 Series built-in EtherNet/IP port.
    /// Typically found on Port 4 of PF753/PF755 drives.
    ///
    /// Parameters include Datalinks, network I/O mapping (DL From/To Net),
    /// fault actions, and fault configuration for DeviceLogix channels.
    /// </summary>
    public class PF750_PortCard_EthernetIP : PowerFlex750PortCard
    {
        public override ushort[] ProductCodes => [57760];
        public override string ProductName => "xEtherNet/IP";
        public override int ClassID => 0x9F;
        public override bool UseScatteredRead => false;

        protected override string parameterListJSON => @"[
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
    }
}
