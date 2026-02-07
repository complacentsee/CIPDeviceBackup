namespace powerFlexBackup.cipdevice.PortCards
{
    /// <summary>
    /// 20-COMM-E EtherNet/IP Communication Adapter
    ///
    /// PowerFlex 750 Series expansion card for EtherNet/IP connectivity.
    /// Can be installed in any expansion port (1-14).
    ///
    /// Special Notes:
    /// - Uses ClassID 0x93 (DPI class) instead of standard 0x9F (HOST memory)
    /// - Supports BOOTP/DHCP, static IP configuration
    /// - Provides EtherNet/IP I/O, explicit messaging, web server
    /// </summary>
    public class PF750_PortCard_20COMM_E : PowerFlex750PortCard
    {
        public override ushort ProductCode => 300;  // TODO: Verify actual ProductCode from live device
        public override string ProductName => "20-COMM-E";
        public override int ClassID => 0x93;  // DPI class for communication adapters

        // TODO: Replace with actual parameters exported from live device
        // Placeholder list - user will populate with real parameters from device
        // Parameters should use relative numbering (1, 2, 3...) - port offset added at runtime
        protected override string parameterListJSON => @"[]";
    }
}
