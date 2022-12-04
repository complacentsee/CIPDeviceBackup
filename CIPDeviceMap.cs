namespace powerFlexBackup.cipdevice{
    public class CIPDeviceMap
    {
        public int deviceType { get; set; }
        public int productCode { get; set; }
        public string identityObjectClass { get; set; }
        public string CIPDeviceClass { get; set; }
        public CIPDeviceMap(int deviceType, int productCode, string identityObjectClass, string CIPDeviceClass)
        {
            this.deviceType = deviceType;
            this.productCode = productCode;
            this.identityObjectClass = identityObjectClass;
            this.CIPDeviceClass = CIPDeviceClass;
        }
    }

    public class CIPDeviceDictionary
    {
        public List<CIPDeviceMap> CIPDeviceMap;
        public CIPDeviceDictionary()
        {
            this.CIPDeviceMap = new List<CIPDeviceMap>();
            
            // FIXME: This should be read from a file
            this.CIPDeviceMap.Add(new CIPDeviceMap(150, 
                                9, 
                                "powerFlexBackup.cipdevice.IdentityObject_PowerFlex525", 
                                "powerFlexBackup.cipdevice.CIPDevice_PowerFlex525"));

            this.CIPDeviceMap.Add(new CIPDeviceMap(123, 
                                50, 
                                "powerFlexBackup.cipdevice.IdentityObject_PowerFlex70", 
                                "powerFlexBackup.cipdevice.CIPDevice_PowerFlex70"));

            this.CIPDeviceMap.Add(new CIPDeviceMap(3, 
                                300, 
                                "powerFlexBackup.cipdevice.IdentityObject_193ETN", 
                                "powerFlexBackup.cipdevice.CIPDevice_193ETN"));

            this.CIPDeviceMap.Add(new CIPDeviceMap(3, 
                                651, 
                                "powerFlexBackup.cipdevice.IdentityObject_E300", 
                                "powerFlexBackup.cipdevice.CIPDevice_E300"));
        }       

        public String getIdentityObjectClass(int deviceType, int productCode){
            return this.CIPDeviceMap.Find(x => x.deviceType == deviceType && x.productCode == productCode).identityObjectClass;
        }

        public String getCIPDeviceClass(int deviceType, int productCode){
            return this.CIPDeviceMap.Find(x => x.deviceType == deviceType && x.productCode == productCode).CIPDeviceClass;
        }
    }
}
