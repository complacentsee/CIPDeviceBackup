using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using powerFlexBackup.cipdevice;
using powerFlexBackup.cipdevice.deviceParameterObjects;

namespace powerFlexBackup
{

    public class CIPDeviceFactory
    {
        private readonly Sres.Net.EEIP.EEIPClient eeipClient;
        private readonly AppConfiguration config;
        private readonly ILogger<CIPDeviceFactory> logger;

        // Static cache for device types - initialized once on first access
        private static readonly Lazy<List<Type>> _deviceTypes = new Lazy<List<Type>>(() =>
            AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.IsSubclassOf(typeof(CIPDevice)) && !type.IsAbstract)
                .ToList()
        );

        public CIPDeviceFactory(
            Sres.Net.EEIP.EEIPClient eeipClient,
            IOptions<AppConfiguration> options,
            ILogger<CIPDeviceFactory> logger)
        {
            this.eeipClient = eeipClient;
            this.config = options.Value;
            this.logger = logger;
        }

        public CIPDevice getDevicefromAddress(String hostAddress, byte[] route){
            
            if(config.OutputVerbose)
                Console.WriteLine("Attempting to connect to device at address: {0}...", hostAddress);

            if(!IsIPv4(hostAddress)){
                logger.LogError("Invalid IP Address");
                throw new InvalidOperationException("Invalid IP Address, skipping device.");
            }
            if(config.OutputVerbose)
                Console.WriteLine("Address {0} is valid. Validating connectivity...", hostAddress);

            if(config.SkipPing || !validateNetworkConnection(hostAddress)){
                logger.LogError("Unable to ping IP Address: {0}", hostAddress);
                throw new InvalidOperationException("Unable to ping device.");
            }
            if(config.OutputVerbose)
                Console.WriteLine("Ping succeeded to address: {0}", hostAddress);

            registerSession(hostAddress);

            try{
                byte[] rawIdentityObject;
                if (route.Length > 0){
                    rawIdentityObject = getRawIdentiyObjectfromSession(this.eeipClient, route);
                } else {
                    rawIdentityObject = getRawIdentiyObjectfromSession(this.eeipClient);
                }
                this.eeipClient.UnRegisterSession();

                var identityObject = IdentityObject.getIdentityObjectfromResponse(rawIdentityObject, logger);
                var deviceType = (int)identityObject.DeviceType;
                var productCode = (int)identityObject.ProductCode;

                Console.WriteLine("deviceType {0}, productCode {1}", deviceType, productCode);
                var DeviceClass = getDeviceTypeClass(deviceType, productCode);

                return (CIPDevice)Activator.CreateInstance(DeviceClass!, new object[] {hostAddress, eeipClient, route, Options.Create(config), logger, identityObject})!;
            }
            catch(Exception e){
                logger.LogError("Unable to create device object: {0}", e.Message);
                throw new InvalidOperationException("Unable to create device object.");
            }
        }

        private static byte[] getRawIdentiyObjectfromSession(Sres.Net.EEIP.EEIPClient eeipClient)
        {
            return eeipClient.GetAttributeAll(0x01, 1);
        }

        private static byte[] getRawIdentiyObjectfromSession(Sres.Net.EEIP.EEIPClient eeipClient, byte[] route)
        {
            try {
                return eeipClient.GetAttributeAll(route, 0x01, 1);
            }
            catch (Sres.Net.EEIP.CIPException ex) when (ex.Message.Contains("Service not supported")) {
                // DeviceNet routing doesn't support GetAttributeAll - fall back to individual reads
                return getIdentityObjectViaIndividualReads(eeipClient, route);
            }
        }

        private static byte[] getIdentityObjectViaIndividualReads(Sres.Net.EEIP.EEIPClient eeipClient, byte[] route)
        {
            // Identity Object (Class 0x01) attributes
            var vendorId = eeipClient.GetAttributeSingle(route, 0x01, 1, 1);      // 2 bytes
            var deviceType = eeipClient.GetAttributeSingle(route, 0x01, 1, 2);    // 2 bytes
            var productCode = eeipClient.GetAttributeSingle(route, 0x01, 1, 3);   // 2 bytes
            var revision = eeipClient.GetAttributeSingle(route, 0x01, 1, 4);      // 2 bytes
            var status = eeipClient.GetAttributeSingle(route, 0x01, 1, 5);        // 2 bytes
            var serialNumber = eeipClient.GetAttributeSingle(route, 0x01, 1, 6);  // 4 bytes
            var productName = eeipClient.GetAttributeSingle(route, 0x01, 1, 7);   // SHORT_STRING

            // Assemble same format as GetAttributeAll
            using (var ms = new System.IO.MemoryStream())
            {
                ms.Write(vendorId, 0, vendorId.Length);
                ms.Write(deviceType, 0, deviceType.Length);
                ms.Write(productCode, 0, productCode.Length);
                ms.Write(revision, 0, revision.Length);
                ms.Write(status, 0, status.Length);
                ms.Write(serialNumber, 0, serialNumber.Length);
                ms.Write(productName, 0, productName.Length);
                return ms.ToArray();
            }
        }

        private static bool IsIPv4(string address)
        {
            var octets = address.Split('.');

            // if we do not have 4 octets, return false
            if (octets.Length!=4) return false;

            // for each octet
            foreach(var octet in octets) 
            {
                int q;
                // if parse fails 
                // or length of parsed int != length of octet string (i.e.; '1' vs '001')
                // or parsed int < 0
                // or parsed int > 255
                // return false
                if (!Int32.TryParse(octet, out q) 
                    || !q.ToString().Length.Equals(octet.Length) 
                    || q < 0 
                    || q > 255) { return false; }
            }
            return true;
        }

        private bool validateNetworkConnection(string address)
        {
            Ping pingSender = new Ping ();
            PingOptions options = new PingOptions ();

            // Use the default Ttl value which is 128,
            // but change the fragmentation behavior.
            options.DontFragment = true;

            // Create a buffer of 32 bytes of data to be transmitted.
            string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
            byte[] buffer = Encoding.ASCII.GetBytes (data);
            int timeout = this.config.ConnectionTimeout*1000;
            PingReply reply = pingSender.Send (address, timeout, buffer, options);
            if (reply.Status == IPStatus.Success)
            {
                this.logger.LogDebug ("Ping Status: {0}", reply.Status);
                this.logger.LogDebug ("Address: {0}", reply.Address.ToString ());
                this.logger.LogDebug ("RoundTrip time: {0}", reply.RoundtripTime);
                return true;
            }
            return false;
        }

        private void registerSession(string hostAddress){
            var task = Task.Run(() => this.eeipClient.RegisterSession(hostAddress));
            if (task.Wait(TimeSpan.FromSeconds(this.config.ConnectionTimeout))){
                if(this.config.OutputVerbose)
                    Console.WriteLine("Successfully registered CIP connection.");
                return;
            }
            else
                throw new Exception("Unable to register CIP session. Connection timed out.");
        }


            private Type getDeviceTypeClass(int deviceType, int productCode)
            {
                Type? wildcardMatch = null;

                foreach (var type in _deviceTypes.Value){
                    System.Attribute[] attrs = System.Attribute.GetCustomAttributes(type);
                    foreach (System.Attribute attr in attrs)
                        if (attr is SupportedDevice)
                        {
                            SupportedDevice a = (SupportedDevice)attr;
                            if(a.map.Any(x => x.deviceType == deviceType && x.productCode.HasValue && x.productCode.Value == productCode))
                                return type;
                            if(wildcardMatch == null && a.map.Any(x => x.deviceType == deviceType && !x.productCode.HasValue))
                                wildcardMatch = type;
                        }
                }
                return wildcardMatch ?? typeof(CIPDevice_Generic);
            }

            public static string GetSupportedDevicesDisplay()
            {

                String supportedDevices = "";

                foreach (var type in _deviceTypes.Value){
                    System.Attribute[] attrs = System.Attribute.GetCustomAttributes(type);
                    var firstDeviceShown = false;
                    var firstSupportedDeviceDetails = false;
                    foreach (System.Attribute attr in attrs)
                    {
                        if (attr is SupportedDevice)
                        {
                            SupportedDevice a = (SupportedDevice)attr;
                            if(!firstDeviceShown){
                                firstDeviceShown = true;
                                supportedDevices +=  Environment.NewLine + "   " + a.GetSupprtedDeviceType();
                            }
                            var SupportedDeviceDetails = a.GetSupprtedDeviceDetails();
                            if (SupportedDeviceDetails != null){
                                if(!firstSupportedDeviceDetails){
                                    firstSupportedDeviceDetails = true;
                                    supportedDevices +=  ":";
                                }
                                supportedDevices +=  Environment.NewLine + "      " + a.GetSupprtedDeviceDetails();
                            }
                        }
                    }
                }
                return supportedDevices;
            }

    }
}