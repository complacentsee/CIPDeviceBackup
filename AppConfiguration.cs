namespace powerFlexBackup
{
    public class AppConfiguration
    {
        public bool OutputAllRecords { get; set; }
        public bool OutputVerbose { get; set; }
        public bool SkipPing { get; set; }
        public int ConnectionTimeout { get; set; } = 3;
        public static bool OutputAllRecordsStatic { get; set; }
    }
}
