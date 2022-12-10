using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using powerFlexBackup.cipdevice;
using powerFlexBackup.cipdevice.deviceParameterObjects;

namespace powerFlexBackup
{

    public class CIPDeviceFactory
    {
        Sres.Net.EEIP.EEIPClient eeipClient;
        public CIPDeviceFactory(Sres.Net.EEIP.EEIPClient eeipClient)
        {
            this.eeipClient = eeipClient;
        }

        public CIPDevice getDevicefromAddress(String hostAddress){
            
            if(Globals.outputVerbose)
                Console.WriteLine("Attempting to connect to device at address: {0}...", hostAddress);

            if(!IsIPv4(hostAddress)){
                Globals.logger.LogError("Invalid IP Address");
                throw new InvalidOperationException("Invalid IP Address, skipping device.");
            }
            if(Globals.outputVerbose)
                Console.WriteLine("Address {0} is valid. Validating connectivity...", hostAddress);

            if(Globals.skipPing || !validateNetworkConnection(hostAddress)){
                Globals.logger.LogError("Unable to ping IP Address: {0}", hostAddress);
                throw new InvalidOperationException("Unable to ping device.");
            }
            if(Globals.outputVerbose)
                Console.WriteLine("Ping succeeded to address: {0}", hostAddress);

            registerSession(hostAddress);

            try{
                var rawIdentityObject = getRawIdentiyObjectfromSession(this.eeipClient);
                this.eeipClient.UnRegisterSession();

                var deviceType = getIdentiyObjectDeviceTypefromRaw(rawIdentityObject);
                var productCode = getIdentiyObjectProductCodefromRaw(rawIdentityObject);

                this.eeipClient.UnRegisterSession();

                //TESTING HERE: REMOVE AFTER REVALIDATING ALL DEVICES. 
                //var DeviceClass = Type.GetType(DeviceDictionary.getCIPDeviceClass(deviceType, productCode));
                var DeviceClass = getDeviceTypeClass(deviceType, productCode);

                return (CIPDevice)Activator.CreateInstance(DeviceClass!, new object[] {hostAddress, eeipClient})!;
            }
            catch(Exception e){
                Globals.logger.LogError("Unable to create device object: {0}", e.Message);
                throw new InvalidOperationException("Unable to create device object.");
            }
        }

        private static byte[] getRawIdentiyObjectfromSession(Sres.Net.EEIP.EEIPClient eeipClient)
        {
            return eeipClient.GetAttributeAll(0x01, 1);
        }

        private static int getIdentiyObjectDeviceTypefromRaw(byte[] rawIdentityObject)
        {
            return Convert.ToUInt16(rawIdentityObject[2]
                                        | rawIdentityObject[3] << 8);
        }

        private static int getIdentiyObjectProductCodefromRaw(byte[] rawIdentityObject)
        {
            return Convert.ToUInt16(rawIdentityObject[4]
                                        | rawIdentityObject[5] << 8);
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

        private static bool validateNetworkConnection(string address)
        {
            Ping pingSender = new Ping ();
            PingOptions options = new PingOptions ();

            // Use the default Ttl value which is 128,
            // but change the fragmentation behavior.
            options.DontFragment = true;

            // Create a buffer of 32 bytes of data to be transmitted.
            string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
            byte[] buffer = Encoding.ASCII.GetBytes (data);
            int timeout = Globals.connectionTimeout*1000;
            PingReply reply = pingSender.Send (address, timeout, buffer, options);
            if (reply.Status == IPStatus.Success)
            {
                Globals.logger.LogDebug ("Ping Status: {0}", reply.Status);
                Globals.logger.LogDebug ("Address: {0}", reply.Address.ToString ());
                Globals.logger.LogDebug ("RoundTrip time: {0}", reply.RoundtripTime);
                return true;
            }
            return false;
        }

        private void registerSession(string hostAddress){
            var task = Task.Run(() => this.eeipClient.RegisterSession(hostAddress));
            if (task.Wait(TimeSpan.FromSeconds(Globals.connectionTimeout))){
                if(Globals.outputVerbose)
                    Console.WriteLine("Successfully registered CIP connection.");
                return;
            }
            else
                throw new Exception("Unable to register CIP session. Connection timed out.");
        }


            private Type getDeviceTypeClass(int deviceType, int productCode)
            {  
                var types = AppDomain.CurrentDomain.GetAssemblies()
                                    .SelectMany(assembly => assembly.GetTypes())
                                    .Where(type => type.IsSubclassOf(typeof(CIPDevice)));
                foreach (var type in types){
                    System.Attribute[] attrs = System.Attribute.GetCustomAttributes(type); 
                    foreach (System.Attribute attr in attrs)  
                        if (attr is SupportedDevice)  
                        {  
                            SupportedDevice a = (SupportedDevice)attr;
                            if(a.map.Any(x => x.deviceType == deviceType && x.productCode == productCode))
                                return type;
                        }  
                }
                return typeof(CIPDevice_Generic);
            }  
        
    }
}