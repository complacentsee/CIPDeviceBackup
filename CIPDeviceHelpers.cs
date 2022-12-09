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

        public static bool[] unpackageBytesToBoolArray(byte[] byteArray){
            var Length = byteArray.Count() * 8;
            bool[] boolArray = new bool[Length];
            for (int i = 0; i < Length; i++){
                boolArray[i] = (byteArray[i/8] & (1 << (i % 8))) != 0;
            }
        return boolArray;
    }

        public static String convertBytestoUSINT8(byte[] parameterValueBytes)
        {
            return Convert.ToString(Convert.ToUInt16(parameterValueBytes[0]));
        }

        public static String convertBytestoINT8(byte[] parameterValueBytes)
        {
            return BitConverter.ToInt16(parameterValueBytes).ToString();
        }

        public static String convertBytestoUINT16LittleEndian(byte[] parameterValueBytes)
        {
            return BitConverter.ToUInt16(parameterValueBytes).ToString();
        }

        public static UInt16 convertBytestoUINT16LittleEndianV(byte[] parameterValueBytes)
        {
            return BitConverter.ToUInt16(parameterValueBytes);
        }

        public static String convertBytesToINT16LittleEndian(byte[] parameterValueBytes)
        {
            return BitConverter.ToInt16(parameterValueBytes).ToString();
        }

        public static Int16 convertBytesToINT16LittleEndianV(byte[] parameterValueBytes)
        {
            return BitConverter.ToInt16(parameterValueBytes);
        }

        public static String convertBytestoINT32LittleEndian(byte[] parameterValueBytes)
        {
            return BitConverter.ToInt32(parameterValueBytes).ToString();
        }

        public static String convertBytestoUINT32LittleEndian(byte[] parameterValueBytes)
        {

            return BitConverter.ToUInt32(parameterValueBytes,0).ToString();
        }
        
        public static String convertBytestoFloat32LittleEndian(byte[] parameterValueBytes)
        {
            return BitConverter.ToSingle(parameterValueBytes,0).ToString();;
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