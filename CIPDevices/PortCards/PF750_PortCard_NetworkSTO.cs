namespace powerFlexBackup.cipdevice.PortCards
{
    /// <summary>
    /// Port  4 - Network STO Port Card
    ///
    /// 5 parameters (0 writable, 5 read-only).
    /// Types: yA== (0xC8 DINT), 0w== (0xD3 LWORD)
    /// </summary>
    public class PF750_PortCard_Port4NetworkSTO : PowerFlex750PortCard
    {
        public override ushort[] ProductCodes => [17056];
        public override string ProductName => "Network STO";
        public override int HostClassID => 0x9F;

        protected override string hostParameterListJSON => @"[
            { 'number': '1', 'name': 'Guard Status', 'defaultValue': '00000000000000000000000000000000', 'record': 'false', 'type': '0w==' },
            { 'number': '2', 'name': 'Guard Faults', 'defaultValue': '00000000000000000000000000000000', 'record': 'false', 'type': '0w==' },
            { 'number': '3', 'name': 'Safety State', 'defaultValue': '1', 'record': 'false', 'type': 'yA==' },
            { 'number': '4', 'name': 'Safety Status', 'defaultValue': '00000000000000000000000000000000', 'record': 'false', 'type': '0w==' },
            { 'number': '5', 'name': 'Safety Faults', 'defaultValue': '00000000000000000000000000000000', 'record': 'false', 'type': '0w==' }
        ]";
    }
}
