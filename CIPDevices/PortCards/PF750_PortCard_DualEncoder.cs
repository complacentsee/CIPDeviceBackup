namespace powerFlexBackup.cipdevice.PortCards
{
    /// <summary>
    /// Port 4 - Dual Encoder Port Card
    ///
    /// 18 parameters (7 writable, 11 read-only).
    /// Types: yA== (0xC8 DINT), 0g== (0xD2 DWORD), 0w== (0xD3 LWORD)
    /// </summary>
    public class PF750_PortCard_Port4DualEncoder : PowerFlex750PortCard
    {
        public override ushort[] ProductCodes => [9376];
        public override string ProductName => "Dual Encoder";
        public override int HostClassID => 0x9F;

        protected override string hostParameterListJSON => @"[
            { 'number': '1', 'name': 'Enc 0 Cfg', 'defaultValue': '0000000000000100', 'record': 'true', 'type': '0g==' },
            { 'number': '2', 'name': 'Enc 0 PPR', 'defaultValue': '1024', 'record': 'true', 'type': 'yA==' },
            { 'number': '3', 'name': 'Enc 0 FB Lss Cfg', 'defaultValue': '3', 'record': 'true', 'type': 'yA==' },
            { 'number': '4', 'name': 'Enc 0 FB', 'defaultValue': '0', 'record': 'false', 'type': 'yA==' },
            { 'number': '5', 'name': 'Enc 0 Sts', 'defaultValue': '00000000000000000000000000000000', 'record': 'false', 'type': '0w==' },
            { 'number': '6', 'name': 'Enc 0 Error Sts', 'defaultValue': '00000000000000000000000000000000', 'record': 'false', 'type': '0w==' },
            { 'number': '7', 'name': 'Enc 0 PhsLss Cnt', 'defaultValue': '0', 'record': 'false', 'type': 'yA==' },
            { 'number': '8', 'name': 'Enc 0 QuadLssCnt', 'defaultValue': '0', 'record': 'false', 'type': 'yA==' },
            { 'number': '11', 'name': 'Enc 1 Cfg', 'defaultValue': '0000000000000100', 'record': 'true', 'type': '0g==' },
            { 'number': '12', 'name': 'Enc 1 PPR', 'defaultValue': '1024', 'record': 'true', 'type': 'yA==' },
            { 'number': '13', 'name': 'Enc 1 FB Lss Cfg', 'defaultValue': '3', 'record': 'true', 'type': 'yA==' },
            { 'number': '14', 'name': 'Enc 1 FB', 'defaultValue': '0', 'record': 'false', 'type': 'yA==' },
            { 'number': '15', 'name': 'Enc 1 Sts', 'defaultValue': '00000000000000000000000000000000', 'record': 'false', 'type': '0w==' },
            { 'number': '16', 'name': 'Enc 1 Error Sts', 'defaultValue': '00000000000000000000000000000000', 'record': 'false', 'type': '0w==' },
            { 'number': '17', 'name': 'Enc 1 PhsLss Cnt', 'defaultValue': '0', 'record': 'false', 'type': 'yA==' },
            { 'number': '18', 'name': 'Enc 1 QuadLssCnt', 'defaultValue': '0', 'record': 'false', 'type': 'yA==' },
            { 'number': '20', 'name': 'Homing Cfg', 'defaultValue': '0000000000000000', 'record': 'true', 'type': '0g==' },
            { 'number': '21', 'name': 'Module Sts', 'defaultValue': '0000000000000000', 'record': 'false', 'type': '0g==' }
        ]";
    }
}
