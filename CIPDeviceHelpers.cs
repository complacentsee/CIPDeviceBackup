namespace powerFlexBackup.cipdevice{
    public static class CIPDeviceHelper{

        // reverse byte order (16-bit)
        public static UInt16 ReverseBytes(UInt16 value)
        {
        return (UInt16)((value & 0xFFU) << 8 | (value & 0xFF00U) >> 8);
        }


        // reverse byte order (32-bit)
        public static UInt32 ReverseBytes(UInt32 value)
        {
        return (value & 0x000000FFU) << 24 | (value & 0x0000FF00U) << 8 |
                (value & 0x00FF0000U) >> 8 | (value & 0xFF000000U) >> 24;
        }

        public static byte[] ReverseBytes(byte[] value)
        {
        byte[] result = new byte[value.Length];
        for (int i = 0; i < value.Length; i++)
            result[i] = value[value.Length - i - 1];

        return result;
        }

        public static String convertBytestoUSINT8(byte[] parameterValueBytes)
        {
            return Convert.ToString(Convert.ToUInt16(parameterValueBytes[0]));
        }

        public static String convertBytestoINT8(byte[] parameterValueBytes)
        {
            var parameterValue = (sbyte) (parameterValueBytes[0]);
            return Convert.ToString(Convert.ToInt16(parameterValue));
        }

        public static String convertBytestoUINT16LittleEndian(byte[] parameterValueBytes)
        {
            return Convert.ToString(Convert.ToUInt16(parameterValueBytes[0]
                                                        | parameterValueBytes[1] << 8));
        }

        public static String convertBytesToINT16LittleEndian(byte[] parameterValueBytes)
        {
            var parameterValue = (Int16) (parameterValueBytes[0] | parameterValueBytes[1] << 8);
            return Convert.ToString(Convert.ToInt16(parameterValue));
        }

        public static String convertBytestoUINT32LittleEndian(byte[] parameterValueBytes)
        {
            return Convert.ToString(Convert.ToUInt32(parameterValueBytes[0]
                                                        | parameterValueBytes[1] << 8
                                                        | parameterValueBytes[2] << 16
                                                        | parameterValueBytes[3] << 24));
        }

        public static String convertBytestoINT32LittleEndian(byte[] parameterValueBytes)
        {
            var value = (Int32)(parameterValueBytes[0]
                                                        | parameterValueBytes[1] << 8
                                                        | parameterValueBytes[2] << 16
                                                        | parameterValueBytes[3] << 24);
            return value.ToString();
        }

        public static String convertBytestoBOOL(byte[] parameterValueBytes)
        {
            return Convert.ToString(Convert.ToBoolean(parameterValueBytes[0]));
        }
        
        public static String convertBytestoWORD(byte[] parameterValueBytes)
        {
            return Convert.ToString(parameterValueBytes[0],2).PadLeft(8,'0');
        }

        public static String convertBytestoDWORD(byte[] parameterValueBytes)
        {
                return Convert.ToString(parameterValueBytes[0]
                                        | parameterValueBytes[1] << 8,2).PadLeft(16,'0');
        }

        public static String convertBytestoQWORD(byte[] parameterValueBytes)
        {
            return Convert.ToString(parameterValueBytes[0]
                                        | parameterValueBytes[1] << 8
                                        | parameterValueBytes[2] << 16
                                        | parameterValueBytes[3] << 24,2).PadLeft(32,'0');
        }
    }
}