using System.Reflection;
using Sres.Net.EEIP;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using powerFlexBackup.cipdevice;
using System.CommandLine;
using powerFlexBackup.cipdevice.deviceParameterObjects;

namespace powerFlexBackup
{
    class Program
    {
        static void Main(string[] args)
        {
            var applicationVersion = Assembly.GetEntryAssembly()?.GetName()?.Version?.ToString();

            String address = "";
            FileInfo? outputFile = null;
            int? classID = null;

            //TODO: Try to populate the list of supported devices automatically and print to console.
            var rootCommand = new RootCommand(String.Format("Application to record Ethernet CIP device parameters and save to file." + 
                                                            "\nSupported Devices: {0}" +
                                                            "\nVersion {1}", getSupportedDevices(), applicationVersion));

            var hostOption = new Option<String?>(
                name: "--host",
                description: "The host to read and record parameters from."){ IsRequired = true };
            rootCommand.AddOption(hostOption);  

            var fileOption = new Option<FileInfo?>(
                name: "--output",
                description: "The file location to save the output.",
                isDefault: true,
                parseArgument: result =>
                {
                    if (result.Tokens.Count == 0)
                    {
                        return new FileInfo(@"devicebackups\parameterbackup.txt");
                    }
                    else
                    {                    
                        string filePath = result.Tokens.Single().Value;
                        return new FileInfo(filePath);
                    }
                });
                fileOption.AddAlias("-o");
            rootCommand.AddOption(fileOption); 

            var outputAllParametersOption = new Option<Boolean?>(
                name: "--all",
                description: "Collect and save all parameter values.");
                outputAllParametersOption.AddAlias("-a");
            rootCommand.AddOption(outputAllParametersOption);  

            var outputVerboseOption = new Option<Boolean?>(
                name: "--verbose",
                description: "Print parameters and values to console while collecting.");
                outputVerboseOption.AddAlias("-v");
            rootCommand.AddOption(outputVerboseOption);  

            var skipPingOption = new Option<Boolean?>(
                name: "--noping",
                description: "Skips pinging address prior to attempting connection. Useful for devices behind a firewalls with ICMP disabled.");
            rootCommand.AddOption(skipPingOption); 

            var setParameterClassIDOption = new Option<int?>(
                name: "--classID",
                description: "Specify Custom Parameter ClassID.")
                { IsHidden = true };
            rootCommand.AddOption(setParameterClassIDOption);  

            rootCommand.SetHandler((hostname, outputAllParameters, outputVerbose, skipPing, file, setParameterClassID) => 
            { 
                address = hostname!;

                if(outputAllParameters == true)
                    Globals.outputAllRecords = true;

                if(outputVerbose == true)
                    Globals.outputVerbose = true;

                if(skipPing == true)
                    Globals.skipPing = true;

                if(setParameterClassID is not null)
                    classID = setParameterClassID;

                outputFile = file;

                mainProgram();
            },
            hostOption, outputAllParametersOption, outputVerboseOption, skipPingOption, fileOption, setParameterClassIDOption);
            
            rootCommand.Invoke(args);

            void mainProgram(){

                EEIPClient eeipClient = new Sres.Net.EEIP.EEIPClient();
                CIPDeviceFactory cipDeviceFactory = new CIPDeviceFactory(eeipClient);

                try{
                    CIPDevice cipDevice =  cipDeviceFactory.getDevicefromAddress(address);
                    if(Globals.outputVerbose)
                        Console.WriteLine("Getting device parameters from upload...");

                    if(classID is not null)
                        cipDevice.setParameterClassID((int)classID);

                    if(!Globals.outputAllRecords){       
                        cipDevice.getDeviceParameterValues();             
                        cipDevice.removeNonRecordedDeviceParameters();
                        cipDevice.removeDefaultDeviceParameters();
                    } else{
                        cipDevice.getAllDeviceParameters();
                    }

                    if(Globals.outputVerbose)
                        Console.WriteLine("Completed uploading device parameters.");

                    if (!System.IO.File.Exists(outputFile!.Directory!.ToString())){
                        System.IO.Directory.CreateDirectory(outputFile!.Directory!.ToString());
                    }

                    StreamWriter output = new StreamWriter(outputFile!.FullName);
                    foreach(DeviceParameterObject ParameterObject in cipDevice.getParameterObject()){
                        output.Write(JsonConvert.SerializeObject(ParameterObject,Formatting.Indented));
                        output.WriteLine();
                    }
                    output.Close();
                    Globals.logger.LogInformation ("File saved to {0}", outputFile!.FullName);
                }
                catch(Exception e){
                    Globals.logger.LogError(e.Message);
                    return;
                } 

                eeipClient.UnRegisterSession();
                Thread.Sleep(250);
                return;
            }

            static string getSupportedDevices()  
            {  
 
            var types = AppDomain.CurrentDomain.GetAssemblies()
                                .SelectMany(assembly => assembly.GetTypes())
                                .Where(type => type.IsSubclassOf(typeof(CIPDevice)));

            String supportedDevices = "";

            foreach (var type in types){
                // Using reflection.  
                System.Attribute[] attrs = System.Attribute.GetCustomAttributes(type);  // Reflection.  
        
                // Displaying output.  
                foreach (System.Attribute attr in attrs)  
                {  
                    if (attr is SupportedDevice)  
                    {  
                        SupportedDevice a = (SupportedDevice)attr;  
                        supportedDevices +=  Environment.NewLine + a.GetSupprtedDeviceType();
                    }  
                }  
            }
            return supportedDevices;
            }  

        }
    }

    public static class Globals
    {
    public static bool outputAllRecords { get; set; } = false;
    public static bool outputVerbose { get; set; } = false;
    public static bool skipPing { get; set; } = false;

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

    }
}