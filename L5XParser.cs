using System.Xml.Linq;
using powerFlexBackup.cipdevice.deviceParameterObjects;

namespace powerFlexBackup
{
    public class L5XModule
    {
        public string Name { get; set; } = "";
        public string CatalogNumber { get; set; } = "";
        public int Vendor { get; set; }
        public int ProductType { get; set; }
        public int ProductCode { get; set; }
        public string ParentModule { get; set; } = "";
        public int ParentModPortId { get; set; }
        public bool Inhibited { get; set; }
        public string? EthernetAddress { get; set; }
        public string? CIPRoute { get; set; }
        public bool IsBackupCandidate { get; set; }
        public string? SkipReason { get; set; }
    }

    public static class L5XParser
    {
        public static List<L5XModule> Parse(string l5xFilePath)
        {
            var doc = XDocument.Load(l5xFilePath);
            var modulesElement = doc.Descendants("Modules").FirstOrDefault();
            if (modulesElement == null)
                throw new InvalidOperationException("No <Modules> section found in L5X file.");

            var modules = new Dictionary<string, L5XModule>();

            foreach (var elem in modulesElement.Elements("Module"))
            {
                var mod = ParseModuleElement(elem);
                modules[mod.Name] = mod;
            }

            foreach (var mod in modules.Values)
            {
                ClassifyAndBuildRoute(mod, modules);
            }

            return modules.Values.ToList();
        }

        private static L5XModule ParseModuleElement(XElement elem)
        {
            var mod = new L5XModule
            {
                Name = elem.Attribute("Name")?.Value ?? "",
                CatalogNumber = elem.Attribute("CatalogNumber")?.Value ?? "",
                Vendor = int.Parse(elem.Attribute("Vendor")?.Value ?? "0"),
                ProductType = int.Parse(elem.Attribute("ProductType")?.Value ?? "0"),
                ProductCode = int.Parse(elem.Attribute("ProductCode")?.Value ?? "0"),
                ParentModule = elem.Attribute("ParentModule")?.Value ?? "",
                ParentModPortId = int.Parse(elem.Attribute("ParentModPortId")?.Value ?? "0"),
                Inhibited = bool.Parse(elem.Attribute("Inhibited")?.Value ?? "false")
            };

            foreach (var port in elem.Descendants("Port"))
            {
                var upstream = port.Attribute("Upstream")?.Value;
                var portType = port.Attribute("Type")?.Value;
                var address = port.Attribute("Address")?.Value;
                if (upstream == "true" && portType == "Ethernet" && !string.IsNullOrEmpty(address))
                {
                    mod.EthernetAddress = address;
                    break;
                }
            }

            return mod;
        }

        private static void ClassifyAndBuildRoute(L5XModule mod, Dictionary<string, L5XModule> allModules)
        {
            if (mod.Inhibited)
            {
                mod.IsBackupCandidate = false;
                mod.SkipReason = "Inhibited";
                return;
            }

            if (!CIPDeviceFactory.IsDeviceSupported(mod.ProductType, mod.ProductCode))
            {
                mod.IsBackupCandidate = false;
                mod.SkipReason = $"Unsupported device type (ProductType={mod.ProductType}, ProductCode={mod.ProductCode})";
                return;
            }

            if (string.IsNullOrEmpty(mod.EthernetAddress))
            {
                mod.IsBackupCandidate = false;
                mod.SkipReason = "No Ethernet address";
                return;
            }

            mod.CIPRoute = BuildRoute(mod, allModules);
            if (mod.CIPRoute == null)
            {
                mod.IsBackupCandidate = false;
                mod.SkipReason = "Unable to build route";
                return;
            }

            mod.IsBackupCandidate = true;
        }

        private static string? BuildRoute(L5XModule mod, Dictionary<string, L5XModule> allModules)
        {
            var segments = new List<string>();
            var current = mod;

            while (current.ParentModule != "Local" && current.ParentModule != current.Name)
            {
                if (!allModules.TryGetValue(current.ParentModule, out var parent))
                    return null;

                if (string.IsNullOrEmpty(current.EthernetAddress))
                {
                    var upstreamAddress = GetUpstreamAddress(current, allModules);
                    if (upstreamAddress == null)
                        return null;
                    segments.Insert(0, $"{current.ParentModPortId},{upstreamAddress}");
                }
                else
                {
                    segments.Insert(0, $"{current.ParentModPortId},{current.EthernetAddress}");
                }

                current = parent;
            }

            if (current == mod)
            {
                segments.Add($"{mod.ParentModPortId},{mod.EthernetAddress}");
            }
            else
            {
                if (string.IsNullOrEmpty(current.EthernetAddress))
                    return null;
                segments.Insert(0, $"{current.ParentModPortId},{current.EthernetAddress}");
            }

            return string.Join(",", segments);
        }

        private static string? GetUpstreamAddress(L5XModule mod, Dictionary<string, L5XModule> allModules)
        {
            if (!string.IsNullOrEmpty(mod.EthernetAddress))
                return mod.EthernetAddress;
            return null;
        }

        public static List<L5XModule> GetBackupCandidates(List<L5XModule> modules)
        {
            return modules.Where(m => m.IsBackupCandidate).ToList();
        }
    }
}
