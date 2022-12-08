using System.Net.NetworkInformation;
using System.Text;
using Microsoft.Extensions.Logging;
using powerFlexBackup.cipdevice;

namespace powerFlexBackup
{

    public class CIPDeviceFactory
    {
        CIPDeviceDictionary DeviceDictionary;
        Sres.Net.EEIP.EEIPClient eeipClient;
        public CIPDeviceFactory(Sres.Net.EEIP.EEIPClient eeipClient)
        {
            DeviceDictionary = new CIPDeviceDictionary();
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
                
            this.eeipClient.RegisterSession(hostAddress);
            var rawIdentityObject = getRawIdentiyObjectfromSession(this.eeipClient);
            this.eeipClient.UnRegisterSession();

            var deviceType = getIdentiyObjectDeviceTypefromRaw(rawIdentityObject);
            var productCode = getIdentiyObjectProductCodefromRaw(rawIdentityObject);

            this.eeipClient.UnRegisterSession();
            var DeviceClass = Type.GetType(DeviceDictionary.getCIPDeviceClass(deviceType, productCode));
            return (CIPDevice)Activator.CreateInstance(DeviceClass!, new object[] {hostAddress, eeipClient})!;
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
            int timeout = 1000;
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
    }
}