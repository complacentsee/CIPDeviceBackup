namespace powerFlexBackup.powerFlexDrive.DriveParameterObjects
{
    public class driveParameterObject
    {
        public int ClassID { get; set; } = default!;
        public List<DriveParameter> ParameterList { get; set; } = default!;
        public List<InstanceAttribute> instanceAttributes{ get; set; } = default!;
        public driveParameterObject()
        {
        }
    }
}