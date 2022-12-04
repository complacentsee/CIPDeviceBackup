using System.Text;
using System.Net.NetworkInformation;
using Sres.Net.EEIP;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using powerFlexBackup.cipdevice;
using System.CommandLine;


namespace powerFlexBackup
{
    class Program
    {
        static void Main(string[] args)
        {
            //parse commandline arguments into variables based on commandline flags
            if(args[0] is null){
                Globals.logger.LogError("First argment must be a valid IP Address");
                Thread.Sleep(250);
                Environment.Exit(-1);
            }

            String address = args[0];

            //Validate IP address
            if(!IsIPv4(address)){
                Globals.logger.LogError("Invalid IP Address");
                Thread.Sleep(250);
                Environment.Exit(-1);
            }
            Globals.logger.LogInformation ("Address {0} is valid. Validating connectivity...", address);

            if(!validateNetworkConnection(address)){
                Globals.logger.LogError("Unable to ping IP Address");
                Thread.Sleep(250);
                Environment.Exit(-1);
            }
            Globals.logger.LogInformation ("Ping succeeded to address: {0}", address);
           
            EEIPClient eeipClient = new Sres.Net.EEIP.EEIPClient();

            //Create CIPDeviceFactory
            CIPDeviceFactory cipDeviceFactory = new CIPDeviceFactory(eeipClient);
            CIPDevice cipDevice =  cipDeviceFactory.getDevicefromAddress(address);

            String filePath = @"C:\powerflexdrivebackup0.0.1\temporary\";
            String fileName = @"driveparameterbackup.txt";
            Globals.logger.LogInformation ("Getting drive parameters from upload...");


            // File.WriteAllText(filePath + fileName, JsonConvert.SerializeObject(cipDevice.getDeviceParameterList(),Formatting.Indented));
 //           cipDevice.getAllDeviceParameters();


            cipDevice.getDeviceParameterValues();

            Globals.outputAllRecords = true;

            Globals.logger.LogInformation ("Device uploaded completed.");
            if (!System.IO.File.Exists(filePath)){
                System.IO.Directory.CreateDirectory(filePath);
            }

            cipDevice.removeNonRecordedDeviceParameters();
            cipDevice.removeDefaultDeviceParameters();

            File.WriteAllText(filePath + fileName, JsonConvert.SerializeObject(cipDevice.getIdentityObject(),Formatting.Indented));
            File.AppendAllText(filePath + fileName, Environment.NewLine);
            File.AppendAllText(filePath + fileName, JsonConvert.SerializeObject(cipDevice.getDeviceParameterList(),Formatting.Indented));
            Globals.logger.LogInformation ("File saved to {0}", filePath + fileName);
            eeipClient.UnRegisterSession();
            Thread.Sleep(1000);
            Environment.Exit(0);
        }


    //Fixme: this should be moved to the factory class. 
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

    //Fixme: this should be moved to the factory class. 
    public static bool validateNetworkConnection(string address)
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

    static class Globals
    {
    // create a logger factory
    public static ILoggerFactory loggerFactory = LoggerFactory.Create(
        builder => builder
                    // add console as logging target
                    .AddConsole()
                    // add debug output as logging target
                    .AddDebug()
                    // set minimum level to log
                    .SetMinimumLevel(LogLevel.Information)
    );
    // create a logger
    public static ILogger logger = loggerFactory.CreateLogger<Program>();

    public static bool outputAllRecords = true;
    }
}