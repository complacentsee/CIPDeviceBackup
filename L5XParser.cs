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
        /// <summary>
        /// The address a parent uses to reach this module: the Address of the
        /// module's own upstream port, whatever type that port is. For a drive
        /// on a network this is its IP; for a scanner in the local chassis it
        /// is the slot number on its upstream ICP port. Both are addresses a
        /// CIP route segment can name.
        /// </summary>
        public string? UpstreamAddress { get; set; }
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

            // Take the upstream port whatever its type. Filtering to
            // Type=="Ethernet" here silently skipped every chassis-resident
            // scanner -- a 1756-EN2T's upstream port is ICP, addressed by slot
            // number -- which left BuildRoute with no way to name the first hop
            // and dropped every drive behind such a scanner as "unable to build
            // route".
            foreach (var port in elem.Descendants("Port"))
            {
                var upstream = port.Attribute("Upstream")?.Value;
                var address = port.Attribute("Address")?.Value;
                if (upstream == "true" && !string.IsNullOrEmpty(address))
                {
                    mod.UpstreamAddress = address;
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

            if (string.IsNullOrEmpty(mod.UpstreamAddress))
            {
                mod.IsBackupCandidate = false;
                mod.SkipReason = "No upstream port address";
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

                if (string.IsNullOrEmpty(current.UpstreamAddress))
                {
                    var upstreamAddress = GetUpstreamAddress(current, allModules);
                    if (upstreamAddress == null)
                        return null;
                    segments.Insert(0, $"{current.ParentModPortId},{upstreamAddress}");
                }
                else
                {
                    segments.Insert(0, $"{current.ParentModPortId},{current.UpstreamAddress}");
                }

                current = parent;
            }

            if (current == mod)
            {
                segments.Add($"{mod.ParentModPortId},{mod.UpstreamAddress}");
            }
            else
            {
                if (string.IsNullOrEmpty(current.UpstreamAddress))
                    return null;
                segments.Insert(0, $"{current.ParentModPortId},{current.UpstreamAddress}");
            }

            return string.Join(",", segments);
        }

        private static string? GetUpstreamAddress(L5XModule mod, Dictionary<string, L5XModule> allModules)
        {
            if (!string.IsNullOrEmpty(mod.UpstreamAddress))
                return mod.UpstreamAddress;
            return null;
        }

        public static List<L5XModule> GetBackupCandidates(List<L5XModule> modules)
        {
            return modules.Where(m => m.IsBackupCandidate).ToList();
        }
    }
}
