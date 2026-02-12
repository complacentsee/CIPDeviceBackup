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
            FileInfo? l5xFile = null;
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

            var l5xOption = new Option<FileInfo?>("--l5x")
            {
                Description = "Path to an L5X file. Parses modules, builds CIP routes, and backs up each supported device."
            };
            rootCommand.Add(l5xOption);

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
                l5xFile = parseResult.GetValue(l5xOption);

                if (l5xFile != null && cipRoute != null)
                {
                    Console.Error.WriteLine("Error: --l5x and --CIProute cannot be used together.");
                    Environment.ExitCode = 1;
                    return;
                }

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

                if (l5xFile != null)
                    Environment.ExitCode = l5xProgram(serviceProvider);
                else
                    Environment.ExitCode = mainProgram(serviceProvider);
            });

            var parseResult = rootCommand.Parse(args);
            return parseResult.Invoke();

            int mainProgram(IServiceProvider serviceProvider){

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
                    return 1;
                }

                eeipClient.UnRegisterSession();
                Thread.Sleep(250);
                return 0;
            }

            int l5xProgram(IServiceProvider serviceProvider){

                var cipDeviceFactory = serviceProvider.GetRequiredService<CIPDeviceFactory>();
                var eeipClient = serviceProvider.GetRequiredService<EEIPClient>();
                var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
                var config = serviceProvider.GetRequiredService<IOptions<AppConfiguration>>().Value;

                if (!l5xFile!.Exists)
                {
                    logger.LogError("L5X file not found: {0}", l5xFile.FullName);
                    return 1;
                }

                // When --l5x is used, --output is treated as a folder path
                var backupDir = outputFile!.Extension == ""
                    ? outputFile.FullName
                    : outputFile.Directory!.FullName;
                Directory.CreateDirectory(backupDir);

                Console.WriteLine("Parsing L5X file: {0}", l5xFile.FullName);
                var allModules = L5XParser.Parse(l5xFile.FullName);
                var candidates = L5XParser.GetBackupCandidates(allModules);

                Console.WriteLine();
                Console.WriteLine("=== L5X Module Summary ===");
                Console.WriteLine("Total modules: {0}", allModules.Count);
                Console.WriteLine("Backup candidates: {0}", candidates.Count);
                Console.WriteLine();

                var skipped = allModules.Where(m => !m.IsBackupCandidate).ToList();
                if (skipped.Any())
                {
                    Console.WriteLine("Skipped modules:");
                    foreach (var m in skipped)
                        Console.WriteLine("  {0} ({1}) - {2}", m.Name, m.CatalogNumber, m.SkipReason);
                    Console.WriteLine();
                }

                Console.WriteLine("Devices to back up:");
                foreach (var m in candidates)
                    Console.WriteLine("  {0} ({1}) - Route: {2}", m.Name, m.CatalogNumber, m.CIPRoute);
                Console.WriteLine();

                int succeeded = 0;
                int failed = 0;
                var failedDevices = new List<string>();

                foreach (var module in candidates)
                {
                    Console.WriteLine("[{0}/{1}] Backing up {2} ({3}) via route {4}...",
                        succeeded + failed + 1, candidates.Count,
                        module.Name, module.CatalogNumber, module.CIPRoute);

                    try
                    {
                        var routeBytes = Sres.Net.EEIP.CIPRouteBuilder.ParsePath(module.CIPRoute!);
                        CIPDevice cipDevice = cipDeviceFactory.getDevicefromAddress(address, routeBytes);

                        if (!config.OutputAllRecords)
                        {
                            cipDevice.getDeviceParameterValues();
                            cipDevice.removeNonRecordedDeviceParameters();
                            cipDevice.removeDefaultDeviceParameters();
                        }
                        else
                        {
                            cipDevice.getAllDeviceParameters();
                        }

                        var outputPath = Path.Combine(backupDir, module.Name + ".txt");
                        using (StreamWriter output = new StreamWriter(outputPath))
                        {
                            foreach (DeviceParameterObject paramObj in cipDevice.getParameterObject())
                            {
                                output.Write(JsonConvert.SerializeObject(paramObj, Formatting.Indented));
                                output.WriteLine();
                            }
                        }

                        eeipClient.UnRegisterSession();
                        Thread.Sleep(250);

                        succeeded++;
                        Console.WriteLine("  Saved to {0}", module.Name + ".txt");
                    }
                    catch (Exception e)
                    {
                        failed++;
                        failedDevices.Add(module.Name);
                        logger.LogError("  FAILED: {0} - {1}", module.Name, e.Message);

                        try { eeipClient.UnRegisterSession(); } catch { }
                        Thread.Sleep(250);
                    }
                }

                Console.WriteLine();
                Console.WriteLine("=== Backup Complete ===");
                Console.WriteLine("  Succeeded: {0}", succeeded);
                Console.WriteLine("  Failed:    {0}", failed);
                Console.WriteLine("  Output:    {0}", Path.GetFullPath(backupDir));

                if (failedDevices.Any())
                {
                    Console.WriteLine();
                    Console.WriteLine("  Failed devices:");
                    foreach (var name in failedDevices)
                        Console.WriteLine("    {0}", name);
                }

                return failed > 0 ? 1 : 0;
            }

        }
    }
}