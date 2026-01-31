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
        static int Main(string[] args)
        {
            var applicationVersion = Assembly.GetEntryAssembly()?.GetName()?.Version?.ToString();

            String address = "";
            string? cipRoute = null;
            FileInfo? outputFile = null;
            int? classID = null;

            //TODO: Try to populate the list of supported devices automatically and print to console.
            var rootCommand = new RootCommand(String.Format("Application to record Ethernet CIP device parameters and save to file." +
                                                            "\nSupported Devices: {0}" +
                                                            "\nVersion {1}", CIPDeviceFactory.GetSupportedDevicesDisplay(), applicationVersion));

            var hostOption = new Option<String?>("--host")
            {
                Description = "The host to read and record parameters from.",
                Required = true
            };
            rootCommand.Add(hostOption);

            var routeOption = new Option<String?>("--CIProute")
            {
                Description = "The CIP route for the device to read and record parameters from."
            };
            rootCommand.Add(routeOption);

            var fileOption = new Option<FileInfo?>("--output", "-o")
            {
                Description = "The file location to save the output.",
                DefaultValueFactory = _ => new FileInfo(@"devicebackups\parameterbackup.txt")
            };
            rootCommand.Add(fileOption);

            var outputAllParametersOption = new Option<Boolean?>("--all", "-a")
            {
                Description = "Collect and save all parameter values."
            };
            rootCommand.Add(outputAllParametersOption);

            var outputVerboseOption = new Option<Boolean?>("--verbose", "-v")
            {
                Description = "Print parameters and values to console while collecting."
            };
            rootCommand.Add(outputVerboseOption);

            var skipPingOption = new Option<Boolean?>("--noping")
            {
                Description = "Skips pinging address prior to attempting connection. Useful for devices behind a firewalls with ICMP disabled."
            };
            rootCommand.Add(skipPingOption);

            var setConnectionTimeoutOption = new Option<int?>("--timeout", "-to")
            {
                Description = "Updates default ping and CIP connection timeout to user set value in seconds."
            };
            rootCommand.Add(setConnectionTimeoutOption);

            var setParameterClassIDOption = new Option<int?>("--classID")
            {
                Description = "Specify Custom Parameter ClassID.",
                Hidden = true
            };
            rootCommand.Add(setParameterClassIDOption);

            rootCommand.SetAction((ParseResult parseResult) =>
            {
                address = parseResult.GetValue(hostOption)!;
                cipRoute = parseResult.GetValue(routeOption);

                Globals.outputAllRecords = parseResult.GetValue(outputAllParametersOption) == true;
                Globals.outputVerbose = parseResult.GetValue(outputVerboseOption) == true;
                Globals.skipPing = parseResult.GetValue(skipPingOption) == true;

                var setParameterClassIDValue = parseResult.GetValue(setParameterClassIDOption);
                if(setParameterClassIDValue is not null)
                    classID = setParameterClassIDValue;

                var setConnectionTimeoutValue = parseResult.GetValue(setConnectionTimeoutOption);
                if(setConnectionTimeoutValue is not null)
                    Globals.connectionTimeout = (int)setConnectionTimeoutValue;

                outputFile = parseResult.GetValue(fileOption);

                mainProgram();
            });

            var parseResult = rootCommand.Parse(args);
            return parseResult.Invoke();

            void mainProgram(){

                EEIPClient eeipClient = new Sres.Net.EEIP.EEIPClient();
                CIPDeviceFactory cipDeviceFactory = new CIPDeviceFactory(eeipClient);

                byte[] CIPRouteBytes = [];
                if (cipRoute != null){
                    CIPRouteBytes = Sres.Net.EEIP.CIPRouteBuilder.ParsePath(cipRoute);
                }


                try{
                    CIPDevice cipDevice =  cipDeviceFactory.getDevicefromAddress(address, CIPRouteBytes);
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

                    using (StreamWriter output = new StreamWriter(outputFile!.FullName))
                    {
                        foreach(DeviceParameterObject ParameterObject in cipDevice.getParameterObject()){
                            output.Write(JsonConvert.SerializeObject(ParameterObject,Formatting.Indented));
                            output.WriteLine();
                        }
                    }
                    Globals.logger.LogInformation ("File saved to {0}", outputFile!.FullName);
                }
                catch(Exception e){
                    Globals.logger.LogError(e.Message);
                    Thread.Sleep(500);
                    return;
                } 

                eeipClient.UnRegisterSession();
                Thread.Sleep(250);
                return;
            }

        }
    }

    public static class Globals
    {
    public static bool outputAllRecords { get; set; } = false;
    public static bool outputVerbose { get; set; } = false;
    public static bool skipPing { get; set; } = false;

    public static int connectionTimeout { get; set; } = 3;

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