namespace powerFlexBackup.cipdevice.PortCards
{
    /// <summary>
    /// Port  4 - I/O Module 115V Port Card
    ///
    /// 37 parameters (24 writable, 13 read-only).
    /// Types: yA== (0xC8 DINT), yg== (0xCA REAL), 0g== (0xD2 DWORD)
    /// </summary>
    public class PF750_PortCard_Port4IOModule115V : PowerFlex750PortCard
    {
        public override ushort[] ProductCodes => [672];
        public override string ProductName => "I/O Module 115V";
        public override int HostClassID => 0x9F;

        protected override string hostParameterListJSON => @"[
            { 'number': '1', 'name': 'Dig In Sts', 'defaultValue': '0000000000000000', 'record': 'false', 'type': '0g==' },
            { 'number': '5', 'name': 'Dig Out Sts', 'defaultValue': '0000000000000000', 'record': 'false', 'type': '0g==' },
            { 'number': '6', 'name': 'Dig Out Invert', 'defaultValue': '0000000000000000', 'record': 'true', 'type': '0g==' },
            { 'number': '10', 'name': 'RO0 Sel', 'defaultValue': '0', 'record': 'true', 'type': 'yA==' },
            { 'number': '11', 'name': 'RO0 Level Sel', 'defaultValue': '0', 'record': 'true', 'type': 'yA==' },
            { 'number': '12', 'name': 'RO0 Level', 'defaultValue': '0', 'record': 'true', 'type': 'yg==' },
            { 'number': '13', 'name': 'RO0 Level CmpSts', 'defaultValue': '0000000000000000', 'record': 'false', 'type': '0g==' },
            { 'number': '20', 'name': 'RO1 Sel', 'defaultValue': '0', 'record': 'true', 'type': 'yA==' },
            { 'number': '21', 'name': 'RO1 Level Sel', 'defaultValue': '0', 'record': 'true', 'type': 'yA==' },
            { 'number': '22', 'name': 'RO1 Level', 'defaultValue': '0', 'record': 'true', 'type': 'yg==' },
            { 'number': '23', 'name': 'RO1 Level CmpSts', 'defaultValue': '0000000000000000', 'record': 'false', 'type': '0g==' },
            { 'number': '40', 'name': 'PTC Cfg', 'defaultValue': '0', 'record': 'true', 'type': 'yA==' },
            { 'number': '41', 'name': 'PTC Sts', 'defaultValue': '0000000000000000', 'record': 'false', 'type': '0g==' },
            { 'number': '42', 'name': 'PTC Raw Value', 'defaultValue': '0', 'record': 'false', 'type': 'yg==' },
            { 'number': '45', 'name': 'Anlg In Type', 'defaultValue': '0000000000000000', 'record': 'false', 'type': '0g==' },
            { 'number': '50', 'name': 'Anlg In0 Value', 'defaultValue': '0', 'record': 'false', 'type': 'yg==' },
            { 'number': '51', 'name': 'Anlg In0 Hi', 'defaultValue': '10', 'record': 'true', 'type': 'yg==' },
            { 'number': '52', 'name': 'Anlg In0 Lo', 'defaultValue': '0', 'record': 'true', 'type': 'yg==' },
            { 'number': '60', 'name': 'Anlg In1 Value', 'defaultValue': '0', 'record': 'false', 'type': 'yg==' },
            { 'number': '61', 'name': 'Anlg In1 Hi', 'defaultValue': '10', 'record': 'true', 'type': 'yg==' },
            { 'number': '62', 'name': 'Anlg In1 Lo', 'defaultValue': '0', 'record': 'true', 'type': 'yg==' },
            { 'number': '70', 'name': 'Anlg Out Type', 'defaultValue': '0000000000000000', 'record': 'true', 'type': '0g==' },
            { 'number': '71', 'name': 'Anlg Out Abs', 'defaultValue': '0000000000000011', 'record': 'true', 'type': '0g==' },
            { 'number': '75', 'name': 'Anlg Out0 Sel', 'defaultValue': '3', 'record': 'true', 'type': 'yA==' },
            { 'number': '77', 'name': 'Anlg Out0 Data', 'defaultValue': '0', 'record': 'false', 'type': 'yg==' },
            { 'number': '78', 'name': 'Anlg Out0 DataHi', 'defaultValue': '60', 'record': 'true', 'type': 'yg==' },
            { 'number': '79', 'name': 'Anlg Out0 DataLo', 'defaultValue': '0', 'record': 'true', 'type': 'yg==' },
            { 'number': '80', 'name': 'Anlg Out0 Hi', 'defaultValue': '10', 'record': 'true', 'type': 'yg==' },
            { 'number': '81', 'name': 'Anlg Out0 Lo', 'defaultValue': '0', 'record': 'true', 'type': 'yg==' },
            { 'number': '82', 'name': 'Anlg Out0 Val', 'defaultValue': '0', 'record': 'false', 'type': 'yg==' },
            { 'number': '85', 'name': 'Anlg Out1 Sel', 'defaultValue': '7', 'record': 'true', 'type': 'yA==' },
            { 'number': '87', 'name': 'Anlg Out1 Data', 'defaultValue': '0', 'record': 'false', 'type': 'yg==' },
            { 'number': '88', 'name': 'Anlg Out1 DataHi', 'defaultValue': '52', 'record': 'true', 'type': 'yg==' },
            { 'number': '89', 'name': 'Anlg Out1 DataLo', 'defaultValue': '0', 'record': 'true', 'type': 'yg==' },
            { 'number': '90', 'name': 'Anlg Out1 Hi', 'defaultValue': '10', 'record': 'true', 'type': 'yg==' },
            { 'number': '91', 'name': 'Anlg Out1 Lo', 'defaultValue': '0', 'record': 'true', 'type': 'yg==' },
            { 'number': '92', 'name': 'Anlg Out1 Val', 'defaultValue': '0', 'record': 'false', 'type': 'yg==' }
        ]";
    }
}
