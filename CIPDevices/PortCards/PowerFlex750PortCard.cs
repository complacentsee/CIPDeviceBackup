using System.Reflection;
using Newtonsoft.Json;
using powerFlexBackup.cipdevice.deviceParameterObjects;

namespace powerFlexBackup.cipdevice.PortCards
{
    /// <summary>
    /// Base class for PowerFlex 750 Series expansion port card definitions.
    ///
    /// PowerFlex 750 drives support up to 15 ports (0-14):
    /// - Port 0: Main controller (PowerFlex 753/755)
    /// - Ports 1-14: Expansion cards (I/O modules, comm adapters, etc.)
    ///
    /// Each subclass represents a specific card type that can be installed in any port,
    /// with predefined parameter lists for faster parameter collection.
    /// </summary>
    public abstract class PowerFlex750PortCard
    {
        /// <summary>
        /// Known Product Codes from device Identity Object (primary matching key).
        /// Empty array if ProductCode matching should be skipped (ProductName-only matching).
        /// </summary>
        public abstract ushort[] ProductCodes { get; }

        /// <summary>
        /// Product Name from device Identity Object (fallback matching key).
        /// Used for contains-match if ProductCode doesn't match.
        /// </summary>
        public abstract string ProductName { get; }

        /// <summary>
        /// CIP Class ID for DPI parameter access:
        /// - 0x9F = HOST memory class (most cards)
        /// - 0x93 = DPI class (comm adapters like 20-COMM-E)
        /// </summary>
        public abstract int ClassID { get; }

        /// <summary>
        /// Whether this card supports scattered read (service 0x4D, class 0x93).
        /// Most cards do, but comm adapters like EtherNet/IP return wrong data
        /// from scattered read and must use individual DPI Online Read Full instead.
        /// </summary>
        public virtual bool UseScatteredRead => true;

        /// <summary>
        /// JSON string containing parameter list definition.
        /// Parameters use relative numbering (1, 2, 3...) - port offset is added at runtime.
        /// </summary>
        protected abstract string parameterListJSON { get; }

        /// <summary>
        /// Deserialize and return parameter list for this card type.
        /// Parameter numbers are relative to port offset (will be adjusted when installed in specific port).
        /// </summary>
        public List<DeviceParameter> getParameterList()
        {
            return JsonConvert.DeserializeObject<List<DeviceParameter>>(parameterListJSON)!;
        }

        /// <summary>
        /// Factory method to find card definition matching a PowerFlex 750 port's identity.
        ///
        /// Matching Strategy:
        /// 1. Try ProductCode first (exact match) - most reliable
        /// 2. Fall back to ProductName (contains match) - handles firmware variations
        ///
        /// Returns null for unknown cards, triggering fallback to dynamic parameter reading.
        /// </summary>
        /// <param name="productCode">Product Code from port's Identity Object</param>
        /// <param name="productName">Product Name from port's Identity Object</param>
        /// <returns>PowerFlex750PortCard instance if match found, null if unknown card</returns>
        public static PowerFlex750PortCard? getCardDefinitionForPort(ushort productCode, string productName)
        {
            // Use reflection to discover all PowerFlex 750 port card definitions
            var cardTypes = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.IsSubclassOf(typeof(PowerFlex750PortCard)) && !t.IsAbstract);

            // Phase 1: Try ProductCode match (exact, most reliable)
            if (productCode != 0)
            {
                foreach (var type in cardTypes)
                {
                    var instance = (PowerFlex750PortCard)Activator.CreateInstance(type)!;
                    if (instance.ProductCodes.Length > 0 && instance.ProductCodes.Contains(productCode))
                    {
                        return instance;
                    }
                }
            }

            // Phase 2: Fallback to ProductName match (contains, handles firmware variations)
            foreach (var type in cardTypes)
            {
                var instance = (PowerFlex750PortCard)Activator.CreateInstance(type)!;
                if (!string.IsNullOrEmpty(instance.ProductName) &&
                    productName.Contains(instance.ProductName, StringComparison.OrdinalIgnoreCase))
                {
                    return instance;
                }
            }

            // No match found - unknown card type
            // Caller should fall back to dynamic DPI Online Read Full
            return null;
        }
    }
}
