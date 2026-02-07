namespace powerFlexBackup.cipdevice.PortCards
{
    /// <summary>
    /// Safe Torque Off (STO) Safety Module
    ///
    /// PowerFlex 750 Series safety expansion card providing SIL 3 / PLe safe torque off.
    /// Can be installed in expansion ports (1-14).
    ///
    /// Special Notes:
    /// - Does NOT support DPI parameter reading (no accessible parameter table)
    /// - Configuration done via safety programming tools, not CIP
    /// - Empty parameter list prevents unnecessary read attempts
    /// </summary>
    public class PF750_PortCard_SafeTorqueOff : PowerFlex750PortCard
    {
        public override ushort ProductCode => 0;  // TODO: Determine actual ProductCode from live device
        public override string ProductName => "Safe Torque Off";
        public override int ClassID => 0x9F;  // Standard HOST memory class (though not used)

        // Safe Torque Off modules don't expose CIP-accessible parameters
        // Empty list prevents dynamic read attempts
        protected override string parameterListJSON => @"[]";
    }
}
