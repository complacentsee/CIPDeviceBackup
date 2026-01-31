using System.Reflection;
using Sres.Net.EEIP;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
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
                outputFile = parseResult.GetValue(fileOption);

                var setParameterClassIDValue = parseResult.GetValue(setParameterClassIDOption);
                if(setParameterClassIDValue is not null)
                    classID = setParameterClassIDValue;

                // Create service collection (DI container)
                var services = new ServiceCollection();

                // Configure AppConfiguration from parsed command line args
                services.Configure<AppConfiguration>(options =>
                {
                    options.OutputAllRecords = parseResult.GetValue(outputAllParametersOption) == true;
                    options.OutputVerbose = parseResult.GetValue(outputVerboseOption) == true;
                    options.SkipPing = parseResult.GetValue(skipPingOption) == true;

                    var timeoutValue = parseResult.GetValue(setConnectionTimeoutOption);
                    if (timeoutValue is not null)
                        options.ConnectionTimeout = (int)timeoutValue;
                });

                // Configure logging
                services.AddLogging(builder => builder
                    .AddConsole()
                    .AddDebug()
                    .SetMinimumLevel(LogLevel.Information));

                // Register services
                services.AddSingleton<EEIPClient>();
                services.AddSingleton<CIPDeviceFactory>();

                // Build the service provider
                var serviceProvider = services.BuildServiceProvider();

                // Set static property for serialization scenarios
                var config = serviceProvider.GetRequiredService<IOptions<AppConfiguration>>().Value;
                AppConfiguration.OutputAllRecordsStatic = config.OutputAllRecords;

                mainProgram(serviceProvider);
            });

            var parseResult = rootCommand.Parse(args);
            return parseResult.Invoke();

            void mainProgram(IServiceProvider serviceProvider){

                // Get services from DI container
                var cipDeviceFactory = serviceProvider.GetRequiredService<CIPDeviceFactory>();
                var eeipClient = serviceProvider.GetRequiredService<EEIPClient>();
                var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
                var config = serviceProvider.GetRequiredService<IOptions<AppConfiguration>>().Value;

                byte[] CIPRouteBytes = [];
                if (cipRoute != null){
                    CIPRouteBytes = Sres.Net.EEIP.CIPRouteBuilder.ParsePath(cipRoute);
                }


                try{
                    CIPDevice cipDevice =  cipDeviceFactory.getDevicefromAddress(address, CIPRouteBytes);
                    if(config.OutputVerbose)
                        Console.WriteLine("Getting device parameters from upload...");

                    if(classID is not null)
                        cipDevice.setParameterClassID((int)classID);

                    if(!config.OutputAllRecords){
                        cipDevice.getDeviceParameterValues();
                        cipDevice.removeNonRecordedDeviceParameters();
                        cipDevice.removeDefaultDeviceParameters();
                    } else{
                        cipDevice.getAllDeviceParameters();
                    }

                    if(config.OutputVerbose)
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
                    logger.LogInformation ("File saved to {0}", outputFile!.FullName);
                }
                catch(Exception e){
                    logger.LogError(e.Message);
                    Thread.Sleep(500);
                    return;
                }

                eeipClient.UnRegisterSession();
                Thread.Sleep(250);
                return;
            }

        }
    }
}