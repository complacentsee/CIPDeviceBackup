namespace powerFlexBackup.cipdevice.PortCards
{
    /// <summary>
    /// Motor Side Power Port Card
    ///
    /// PowerFlex 750 Series Motor Side Power port card at Port 12.
    /// ProductCode 6050 identifies the Motor Side Power.
    ///
    /// 23 parameters (4 writable, 19 read-only).
    /// Types: yg== (0xCA REAL), 0w== (0xD3 LWORD), 0g== (0xD2 DWORD), yA== (0xC8 DINT)
    /// </summary>
    public class PF750_PortCard_MotorSidePower : PowerFlex750PortCard
    {
        public override ushort[] ProductCodes => [6050];
        public override string ProductName => "Motor Side Power";
        public override int ClassID => 0x9F;

        protected override string parameterListJSON => @"[
            { 'number': '1', 'name': 'Sys Rated Amps', 'defaultValue': '0', 'record': 'false', 'type': 'yg==' },
            { 'number': '2', 'name': 'Sys Rated Volts', 'defaultValue': '0', 'record': 'false', 'type': 'yg==' },
            { 'number': '11', 'name': 'M CorProt Status', 'defaultValue': '00000000000000000000000000000000', 'record': 'false', 'type': '0w==' },
            { 'number': '13', 'name': 'ThrmMgr Curr Lmt', 'defaultValue': '00000000000000000000000000000000', 'record': 'false', 'type': '0w==' },
            { 'number': '50', 'name': 'TrqAccyMod Temp', 'defaultValue': '0', 'record': 'false', 'type': 'yg==' },
            { 'number': '55', 'name': 'Trq Accy Mod Cfg', 'defaultValue': '0000000000000000', 'record': 'true', 'type': '0g==' },
            { 'number': '56', 'name': 'Fdbk Thresh', 'defaultValue': '0', 'record': 'true', 'type': 'yA==' },
            { 'number': '57', 'name': 'OfstEvent Thresh', 'defaultValue': '400', 'record': 'true', 'type': 'yA==' },
            { 'number': '58', 'name': 'TAM Not Cfg Actn', 'defaultValue': '1', 'record': 'true', 'type': 'yA==' },
            { 'number': '100', 'name': 'M0 Rated Volts', 'defaultValue': '0', 'record': 'false', 'type': 'yg==' },
            { 'number': '101', 'name': 'M0 Rated Amps', 'defaultValue': '0', 'record': 'false', 'type': 'yg==' },
            { 'number': '105', 'name': 'M0 CurrentFdbk U', 'defaultValue': '0', 'record': 'false', 'type': 'yg==' },
            { 'number': '106', 'name': 'M0 CurrentFdbk V', 'defaultValue': '0', 'record': 'false', 'type': 'yg==' },
            { 'number': '107', 'name': 'M0 CurrentFdbk W', 'defaultValue': '0', 'record': 'false', 'type': 'yg==' },
            { 'number': '108', 'name': 'M0 GroundCurrent', 'defaultValue': '0', 'record': 'false', 'type': 'yg==' },
            { 'number': '109', 'name': 'M0 DC BusVoltage', 'defaultValue': '0', 'record': 'false', 'type': 'yg==' },
            { 'number': '111', 'name': 'M0 IGBT Temp', 'defaultValue': '0', 'record': 'false', 'type': 'yg==' },
            { 'number': '116', 'name': 'M0 Cur OL Count', 'defaultValue': '0', 'record': 'false', 'type': 'yg==' },
            { 'number': '117', 'name': 'M0 HeatsinkTempU', 'defaultValue': '0', 'record': 'false', 'type': 'yg==' },
            { 'number': '118', 'name': 'M0 HeatsinkTempV', 'defaultValue': '0', 'record': 'false', 'type': 'yg==' },
            { 'number': '119', 'name': 'M0 HeatsinkTempW', 'defaultValue': '0', 'record': 'false', 'type': 'yg==' },
            { 'number': '123', 'name': 'M0 Life MtrMWHrs', 'defaultValue': '0', 'record': 'false', 'type': 'yg==' },
            { 'number': '124', 'name': 'M0 Life RgnMWHrs', 'defaultValue': '0', 'record': 'false', 'type': 'yg==' }
        ]";
    }
}
