namespace powerFlexBackup.cipdevice{
    public class CIPDeviceMap
    {
        public int deviceType { get; set; }
        public int productCode { get; set; }
        public string CIPDeviceClass { get; set; }
        public CIPDeviceMap(int deviceType, int productCode, string CIPDeviceClass)
        {
            this.deviceType = deviceType;
            this.productCode = productCode;
            this.CIPDeviceClass = CIPDeviceClass;
        }
    }

    public class CIPDeviceDictionary
    {
        public List<CIPDeviceMap> CIPDeviceMap;
        public CIPDeviceDictionary()
        {
            this.CIPDeviceMap = new List<CIPDeviceMap>();
            
            // TODO: We should create a generic device if the device isn't found. This could only be used with a 
            // full parameter backup. Maybe the ParameterClass could be specified via commandline for those isntances? 
            // TODO: Try to populate the list of supported devices automatically and print to console. 
            this.CIPDeviceMap.Add(new CIPDeviceMap(150, 
                                9, 
                                "powerFlexBackup.cipdevice.CIPDevice_PowerFlex525"));

            this.CIPDeviceMap.Add(new CIPDeviceMap(123, 
                                50, 
                                "powerFlexBackup.cipdevice.CIPDevice_PowerFlex70"));

            this.CIPDeviceMap.Add(new CIPDeviceMap(3, 
                                300, 
                                "powerFlexBackup.cipdevice.CIPDevice_193ETN"));

            this.CIPDeviceMap.Add(new CIPDeviceMap(3, 
                                651, 
                                "powerFlexBackup.cipdevice.CIPDevice_E300"));
            
            this.CIPDeviceMap.Add(new CIPDeviceMap(143, 
                                2192, 
                                "powerFlexBackup.cipdevice.CIPDevice_PowerFlex750"));
        }       

        public String getCIPDeviceClass(int deviceType, int productCode){
            return this.CIPDeviceMap.Where(x => x.deviceType == deviceType && x.productCode == productCode)
                                    .Select(x => x.CIPDeviceClass)
                                    .DefaultIfEmpty("powerFlexBackup.cipdevice.CIPDevice_Generic")
                                    .FirstOrDefault()!;
        }
    }
}
