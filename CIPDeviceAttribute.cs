namespace powerFlexBackup.cipdevice.deviceParameterObjects{

    [System.AttributeUsage(System.AttributeTargets.Class |  
                        System.AttributeTargets.Struct,  
                        AllowMultiple = true)  // Multiuse attribute.  
    ]  
    public class SupportedDevice : System.Attribute  
    {  
        private string supprtedDeviceType;
        public int deviceType;
        public int productCode;
        private bool optmizedPolling;
        public List<CIPDeviceMap> map = new List<CIPDeviceMap>();
    
        public SupportedDevice(string supprtedDeviceType, int deviceType, int productCode, bool optmizedPolling = false)  
        {  
            this.supprtedDeviceType = supprtedDeviceType;
            this.deviceType = deviceType;
            this.productCode = productCode;
            this.optmizedPolling = optmizedPolling;
            this.map = new List<CIPDeviceMap>();
            this.map.Add(new CIPDeviceMap(deviceType, productCode));
        }  
        public string GetSupprtedDeviceType()  
        {  
            return supprtedDeviceType + (optmizedPolling?" - supports optimized polling":"");
        }  

        public string? GetSupprtedDeviceDetails()  
        {  
            if( this.deviceType != new int())
                return "DeviceType: " + deviceType + " ProductCode: " + productCode;
            return null;
        }  
    } 

        public class CIPDeviceMap
        {
            public int deviceType { get;}
            public int productCode { get;}
            public CIPDeviceMap(int deviceType, int productCode)
            {
                this.deviceType = deviceType;
                this.productCode = productCode;
            }

            public CIPDeviceMap()
            {
            }
        }
}