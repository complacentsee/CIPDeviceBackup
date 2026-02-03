using Microsoft.Extensions.Logging;

namespace powerFlexBackup.cipdevice.deviceParameterObjects
{
    public static class DPIOnlineReadFull
    {
        public static DeviceParameter_PowerFlex750 ByteArrayToDeviceParameter(
            byte[] byteArray,
            ILogger logger,
            Func<byte[], byte[], string> getParameterValuefromBytes)
        {
            var deviceParameter = new DeviceParameter_PowerFlex750();
            deviceParameter.Descriptor = new Descriptor(byteArray.Take(4).ToArray(), logger);
            deviceParameter.ParameterValue = new Parameter_ContainerPowerFlex750(byteArray.Skip(4).Take(4).ToArray());
            deviceParameter.ParameterValue.value = getParameterValuefromBytes(deviceParameter.ParameterValue.toBytes(), deviceParameter.dataType);
            deviceParameter.MinimumValue = new Parameter_ContainerPowerFlex750(byteArray.Skip(8).Take(4).ToArray());
            deviceParameter.MinimumValue.value = getParameterValuefromBytes(deviceParameter.MinimumValue.toBytes(), deviceParameter.dataType);
            deviceParameter.MaxumumValue = new Parameter_ContainerPowerFlex750(byteArray.Skip(12).Take(4).ToArray());
            deviceParameter.MaxumumValue.value = getParameterValuefromBytes(deviceParameter.MaxumumValue.toBytes(), deviceParameter.dataType);
            deviceParameter.DefaultValue = new Parameter_ContainerPowerFlex750(byteArray.Skip(16).Take(4).ToArray());
            deviceParameter.DefaultValue.value = getParameterValuefromBytes(deviceParameter.DefaultValue.toBytes(), deviceParameter.dataType);
            deviceParameter.NextParameter = CIPDeviceHelper.convertBytestoUINT16LittleEndianV(byteArray.Skip(20).Take(2).ToArray());
            deviceParameter.PreviousParameter = CIPDeviceHelper.convertBytestoUINT16LittleEndianV(byteArray.Skip(22).Take(2).ToArray());
            deviceParameter.UnitsString = System.Text.Encoding.ASCII.GetString(byteArray.Skip(24).Take(4).ToArray()).Trim();
            deviceParameter.Multiplier = CIPDeviceHelper.convertBytestoUINT16LittleEndianV(byteArray.Skip(28).Take(2).ToArray());
            deviceParameter.Divisor = CIPDeviceHelper.convertBytestoUINT16LittleEndianV(byteArray.Skip(30).Take(2).ToArray());
            deviceParameter.Base = CIPDeviceHelper.convertBytestoUINT16LittleEndianV(byteArray.Skip(32).Take(2).ToArray());
            deviceParameter.Offset = CIPDeviceHelper.convertBytesToINT16LittleEndianV(byteArray.Skip(34).Take(2).ToArray());
            deviceParameter.Link = byteArray.Skip(36).Take(3).ToArray();
            deviceParameter.AlwaysZero = byteArray.Skip(39).Take(1).ToArray()[0];
            deviceParameter.ParameterName = System.Text.Encoding.ASCII.GetString(byteArray.Skip(40).Take(32).ToArray()).Trim();
            return deviceParameter;
        }
    }

    public class DeviceParameter_PowerFlex750
    {
        public Descriptor Descriptor { get; set; } = new Descriptor();
        public Parameter_ContainerPowerFlex750 ParameterValue { get; set; } = new Parameter_ContainerPowerFlex750();
        public Parameter_ContainerPowerFlex750 MinimumValue { get; set; } = new Parameter_ContainerPowerFlex750();
        public Parameter_ContainerPowerFlex750 MaxumumValue { get; set; } = new Parameter_ContainerPowerFlex750();
        public Parameter_ContainerPowerFlex750 DefaultValue { get; set; } = new Parameter_ContainerPowerFlex750();
        public UInt16 NextParameter { get; set; } = 0;
        public UInt16 PreviousParameter { get; set; } = 0;
        public String UnitsString { get; set; } = "";
        public UInt16 Multiplier { get; set; } = 0;
        public UInt16 Divisor { get; set; } = 0;
        public UInt16 Base { get; set; } = 0;
        public Int16 Offset { get; set; } = 0;
        public byte[] Link { get; set; } = new byte[3];
        public byte AlwaysZero { get; set; } = new byte();
        public String ParameterName { get; set; } = "";

        public byte[] dataType
        {
            get
            {
                return this.Descriptor.dataType;
            }
        }
        public bool Writable
        {
            get
            {
                return this.Descriptor.Writable;
            }
        }
    }

    public class Parameter_ContainerPowerFlex750
    {
        public byte[] container { get; set; }
        public string value { get; set; }

        public Parameter_ContainerPowerFlex750()
        {
            container = new byte[4];
            value = "";
        }
        public Parameter_ContainerPowerFlex750(byte[] byteArray)
        {
            this.container = byteArray;
            value = "";
        }

        public byte[] toBytes()
        {
            return container;
        }
    }

    public class Descriptor
    {
        public bool[] descriptor { get; set; } = new bool[32];
        private readonly ILogger? logger;

        public byte[] dataType
        {
            get
            {
                return this.getDataType();
            }
        }
        public bool Writable
        {
            get
            {
                return this.getIsWritable();
            }
        }

        public Descriptor()
        {
            descriptor = new bool[32];
            logger = null;
        }
        public Descriptor(byte[] byteArray, ILogger logger)
        {
            this.descriptor = CIPDeviceHelper.unpackageBytesToBoolArray(byteArray);
            this.logger = logger;
        }

        // MAPPING PER POWERFLEX 750 MANUAL
        // Descriptor to decode the type of the parameter
        // Right bit is least significant bit (0).
        // 000 = USINT used as an array of Boolean
        // 001 = UINT used as an array of Boolean
        // 010 = USINT (8 bit integer)
        // 011 = UINT (16 bit integer)
        // 100 = UDINT (32 bit integer)
        // 101 = TCHAR ((8 bit (not Unicode) or 16 bits (Unicode))
        // 110 = REAL (32 bit floating point value)
        // 111 = Use bits 16, 17, 18
        //
        // Bit 16 is the least significant bit.
        // 000 = Reserved
        // 001 = UDINT used as an array of Boolean.
        // 010 = Reserved
        // 011 = Reserved
        // 100 = Reserved
        // 101 = Reserved
        // 110 = Reserved
        // 111 = Reserved
        //
        private byte[] getDataType()
        {
            var dataType = new byte[1];
            if (descriptor[2] == false && descriptor[1] == false && descriptor[0] == false)
            {
                dataType[0] = 0xD1;
            }
            else if (descriptor[2] == false && descriptor[1] == false && descriptor[0] == true)
            {
                dataType[0] = 0xD2;
            }
            else if (descriptor[2] == false && descriptor[1] == true && descriptor[0] == false)
            {
                dataType[0] = 0xC6;
            }
            else if (descriptor[2] == false && descriptor[1] == true && descriptor[0] == true)
            {
                dataType[0] = 0xC7;
            }
            else if (descriptor[2] == true && descriptor[1] == false && descriptor[0] == false)
            {
                dataType[0] = 0xC8;
            }
            else if (descriptor[2] == true && descriptor[1] == false && descriptor[0] == true)
            {
                logger?.LogError("TCHAR not implemented");
                dataType[0] = 0xC2;
            }
            else if (descriptor[2] == true && descriptor[1] == true && descriptor[0] == false)
            {
                dataType[0] = 0xCA;
            }
            else if (descriptor[2] == true && descriptor[1] == true && descriptor[0] == true)
            {
                if (descriptor[2] == true)
                    dataType[0] = 0xD3;
            }
            return dataType;
        }

        private bool getIsWritable()
        {
            return descriptor[8];
        }
    }
}
