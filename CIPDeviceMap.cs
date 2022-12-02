namespace powerFlexBackup.cipdevice{
    public class CIPDeviceMap
    {
        public int deviceType { get; set; }
        public int productCode { get; set; }
        public string identityObjectClass { get; set; }
        public string CIPDeviceClass { get; set; }
        public CIPDeviceMap()
        {
        }
    }

    public class CIPDeviceDictionary
    {
        public List<CIPDeviceMap> CIPDeviceMap;
        public CIPDeviceDictionary()
        {
            this.CIPDeviceMap = new List<CIPDeviceMap>();
            
            // FIXME: This should be read from a file
            this.CIPDeviceMap.Add(new CIPDeviceMap {deviceType = 150, 
                                productCode = 9, 
                                identityObjectClass = "powerFlexBackup.cipdevice.IdentityObject_PowerFlex525", 
                                CIPDeviceClass = "powerFlexBackup.cipdevice.CIPDevice_PowerFlex525"});
        }       

        public String getIdentityObjectClass(int deviceType, int productCode){
            return this.CIPDeviceMap.Find(x => x.deviceType == deviceType && x.productCode == productCode).identityObjectClass;
        }

        public String getCIPDeviceClass(int deviceType, int productCode){
            return this.CIPDeviceMap.Find(x => x.deviceType == deviceType && x.productCode == productCode).CIPDeviceClass;
        }
    }
}
