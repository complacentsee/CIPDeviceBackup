namespace powerFlexBackup.cipdevice.deviceParameterObjects{
    [System.AttributeUsage(System.AttributeTargets.Class |  
                        System.AttributeTargets.Struct,  
                        AllowMultiple = true)  // Multiuse attribute.  
    ]  
    public class SupportedDevice : System.Attribute  
    {  
        string supprtedDeviceType;  
    
        public SupportedDevice(string supprtedDeviceType)  
        {  
            this.supprtedDeviceType = supprtedDeviceType;
    
            // Default value.  
            supprtedDeviceType = "";  
        }  
        public string GetSupprtedDeviceType()  
        {  
            return supprtedDeviceType;  
        }  
    } 
}