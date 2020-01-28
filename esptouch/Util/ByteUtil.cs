using System;
using System.Collections.Generic;
using System.Text;

namespace EspTouchForCSharp.Util
{
    /**
     * In Java, it don't support unsigned int, so we use char to replace uint8.
     * The range of byte is [-128,127], and the range of char is [0,65535].
     * So the byte could used to store the uint8.
     * (We assume that the string could be mapped to assic)
     *
     * @author afunx
     */
    public static class ByteUtil
    {
        public static Encoding ENCODING_CHARSET
        {
            get
            {
                return Encoding.UTF8;
            }
        }


        public static readonly string ESPTOUCH_ENCODING_CHARSET = "UTF-8";

        /// <summary>
        /// 负数转十六进制
        /// </summary>
        /// <param name="iNumber"></param>
        /// <returns></returns>
        public static int NegativeToInt(int iNumber)
        {
            if (iNumber < 0)
            {
                iNumber = -iNumber;

                StringBuilder sb = new StringBuilder();

                char[] binChar = Convert.ToString(iNumber, 2).PadLeft(8, '0').ToCharArray();

                foreach (char ch in binChar)
                {
                    if (Convert.ToInt32(ch) == 48)
                    {
                        sb.Append("1");
                    }
                    else
                    {
                        sb.Append("0");
                    }
                }

                return  Convert.ToInt32(sb.ToString(), 2) + 1;

            }

            return iNumber;
        }

        public static string NegativeToHexString(int iNumber)
        {
            int i = (iNumber);
            return iNumber.ToString("x");
        }

        /// <summary> 

        /// 十六进制转为负数
        ///</summary>

        ///<param name="strNumber"></param>

        ///<returns></returns>

        public static int HexStringToNegative(string strNumber)
        {
            int iNegate = 0;
            int iNumber = Convert.ToInt32(strNumber, 16);

            if (iNumber > 127)
            {
                int iComplement = iNumber - 1;
                StringBuilder sb = new StringBuilder();

                char[] binChar = Convert.ToString(iComplement, 2).PadLeft(8, '0').ToCharArray();

                foreach (char ch in binChar)
                {
                    if (Convert.ToInt32(ch) == 48)
                    {
                        sb.Append("1");
                    }
                    else
                    {
                        sb.Append("0");
                    }
                }

                iNegate = -Convert.ToInt32(sb.ToString(), 2);
            }

            return iNegate;
        }


        /**
         * Put string to byte[]
         *
         * @param destbytes  the byte[] of dest
         * @param srcString  the string of src
         * @param destOffset the offset of byte[]
         * @param srcOffset  the offset of string
         * @param count      the count of dest, and the count of src as well
         */
        public static void putString2bytes(byte[] destbytes, string srcString,
                                           int destOffset, int srcOffset, int count)
        {
            for (int i = 0; i < count; i++)
            {
                destbytes[count + i] = ENCODING_CHARSET.GetBytes(srcString)[i];
            }
        }

        /**
         * Convert uint8 into char( we treat char as uint8)
         *
         * @param uint8 the unit8 to be converted
         * @return the byte of the unint8
         */
        public static byte convertUint8toByte(char uint8)
        {
            if (uint8 > Byte.MaxValue - Byte.MinValue)
            {
                throw new EsptouchException("Out of Boundary");
            }
            return (byte)uint8;
        }

        /**
         * Convert char into uint8( we treat char as uint8 )
         *
         * @param b the byte to be converted
         * @return the char(uint8)
         */
        public static char convertByte2Uint8(byte b)
        {
            // char will be promoted to int for char don't support & operator
            // & 0xff could make negatvie value to positive
            return (char)(b & 0xff);
        }

        /**
         * Convert byte[] into char[]( we treat char[] as uint8[])
         *
         * @param bytes the byte[] to be converted
         * @return the char[](uint8[])
         */
        public static char[] convertBytes2Uint8s(byte[] bytes)
        {
            int len = bytes.Length;
            char[] uint8s = new char[len];
            for (int i = 0; i < len; i++)
            {
                uint8s[i] = convertByte2Uint8(bytes[i]);
            }
            return uint8s;
        }

        /**
         * Put byte[] into char[]( we treat char[] as uint8[])
         *
         * @param destUint8s the char[](uint8[]) array
         * @param srcBytes   the byte[]
         * @param destOffset the offset of char[](uint8[])
         * @param srcOffset  the offset of byte[]
         * @param count      the count of dest, and the count of src as well
         */
        public static void putbytes2Uint8s(char[] destUint8s, byte[] srcBytes,
                                           int destOffset, int srcOffset, int count)
        {
            for (int i = 0; i < count; i++)
            {
                destUint8s[destOffset + i] = convertByte2Uint8(srcBytes[srcOffset
                        + i]);
            }
        }

        /**
         * Convert byte to Hex string
         *
         * @param b the byte to be converted
         * @return the Hex string
         */
        public static string convertByte2HexString(byte b)
        {
            char u8 = convertByte2Uint8(b);
            return Convert.ToInt32(u8).ToString("x");
        }

        /**
         * Convert char(uint8) to Hex string
         *
         * @param u8 the char(uint8) to be converted
         * @return the Hex string
         */
        public static string convertU8ToHexString(char u8)
        {
            return Convert.ToInt32(u8).ToString("x");
        }

        /**
         * Split uint8 to 2 bytes of high byte and low byte. e.g. 20 = 0x14 should
         * be split to [0x01,0x04] 0x01 is high byte and 0x04 is low byte
         *
         * @param uint8 the char(uint8)
         * @return the high and low bytes be split, byte[0] is high and byte[1] is
         * low
         */
        public static byte[] splitUint8To2bytes(char uint8)
        {
            if (uint8 < 0 || uint8 > 0xff)
            {
                throw new EsptouchException("Out of Boundary");
            }
            string hexString = Convert.ToInt32(uint8).ToString("x");
            byte low;
            byte high;
            if (hexString.Length > 1)
            {
                high = (byte)Convert.ToInt32(hexString.Substring(0, 1), 16);
                low = (byte)Convert.ToInt32(hexString.Substring(1, 1), 16);
            }
            else
            {
                high = 0;
                low = (byte)Convert.ToInt32(hexString.Substring(0, 1), 16);
            }
            byte[] result = new byte[] { high, low };
            return result;
        }

        /**
         * Combine 2 bytes (high byte and low byte) to one whole byte
         *
         * @param high the high byte
         * @param low  the low byte
         * @return the whole byte
         */
        public static byte combine2bytesToOne(byte high, byte low)
        {
            if (high < 0 || high > 0xf || low < 0 || low > 0xf)
            {
                throw new EsptouchException("Out of Boundary");
            }
            return (byte)(high << 4 | low);
        }

        /**
         * Combine 2 bytes (high byte and low byte) to
         *
         * @param high the high byte
         * @param low  the low byte
         * @return the char(u8)
         */
        public static char combine2bytesToU16(byte high, byte low)
        {
            char highU8 = convertByte2Uint8(high);
            char lowU8 = convertByte2Uint8(low);
            return (char)(highU8 << 8 | lowU8);
        }

        /**
         * Generate the random byte to be sent
         *
         * @return the random byte
         */
        private static byte randomByte()
        {
            long tick = DateTime.Now.Ticks;
            Random ran = new Random((int)(tick & 0xffffffffL) | (int)(tick >> 32));

            return (byte)(127 - ran.Next(256));
        }

        /**
         * Generate the random byte to be sent
         *
         * @param len the len presented by u8
         * @return the byte[] to be sent
         */
        public static byte[] randomBytes(char len)
        {
            byte[] data = new byte[len];
            for (int i = 0; i < len; i++)
            {
                data[i] = randomByte();
            }
            return data;
        }

        public static byte[] genSpecBytes(char len)
        {
            byte[] data = new byte[len];
            for (int i = 0; i < len; i++)
            {
                data[i] = (byte)'1';
            }
            return data;
        }

        /**
         * Generate the random byte to be sent
         *
         * @param len the len presented by byte
         * @return the byte[] to be sent
         */
        public static byte[] randomBytes(byte len)
        {
            char u8 = convertByte2Uint8(len);
            return randomBytes(u8);
        }

        /**
         * Generate the specific byte to be sent
         *
         * @param len the len presented by byte
         * @return the byte[]
         */
        public static byte[] genSpecBytes(byte len)
        {
            char u8 = convertByte2Uint8(len);
            return genSpecBytes(u8);
        }

        public static string parseBssid(byte[] bssidBytes, int offset, int count)
        {
            byte[] bytes = new byte[count];
            Array.Copy(bssidBytes, offset, bytes, 0, count);
            return parseBssid(bytes);
        }

        ///**
        // * parse "24,-2,52,-102,-93,-60" to "18,fe,34,9a,a3,c4"
        // * parse the bssid from hex to string
        // *
        // * @param bssidBytes the hex bytes bssid, e.g. {24,-2,52,-102,-93,-60}
        // * @return the string of bssid, e.g. 18fe349aa3c4
        // */
        public static string parseBssid(byte[] bssidBytes)
        {
            StringBuilder sb = new StringBuilder();
            int k;
            string hexK;
            string str;
            foreach (byte bssidByte in bssidBytes)
            {
                k = 0xff & bssidByte;
                hexK = k.ToString("x");
                str = ((k < 16) ? ("0" + hexK) : (hexK));
                System.Diagnostics.Debug.WriteLine(str);
                sb.Append(str);
            }
            return sb.ToString();
        }

        /**
         * @param string the string to be used
         * @return the byte[] of string according to {@link #ESPTOUCH_ENCODING_CHARSET}
         */
        public static byte[] getBytesByString(string str)
        {
            try
            {
                return ENCODING_CHARSET.GetBytes(str);
            }
            catch
            {
                throw new EsptouchException("the charset is invalid");
            }
        }

        private static void test_splitUint8To2bytes()
        {
            // 20 = 0x14
            byte[] result = splitUint8To2bytes((char)20);

            if (result[0] == 1 && result[1] == 4)
            {
                System.Diagnostics.Debug.WriteLine("test_splitUint8To2bytes(): pass");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("test_splitUint8To2bytes(): fail");
            }
        }

        private static void test_combine2bytesToOne()
        {
            byte high = 0x01;
            byte low = 0x04;
            if (combine2bytesToOne(high, low) == 20)
            {
                System.Diagnostics.Debug.WriteLine("test_combine2bytesToOne(): pass");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("test_combine2bytesToOne(): fail");
            }
        }

        private static void test_convertChar2Uint8()
        {
            byte b1 = (byte)'a';
            // -128: 1000 0000 should be 128 in unsigned char
            // -1: 1111 1111 should be 255 in unsigned char
            unchecked
            {
                byte b2 = (byte)-128;
                byte b3 = (byte)-1;
                if (convertByte2Uint8(b1) == 97 && convertByte2Uint8(b2) == 128
                        && convertByte2Uint8(b3) == 255)
                {
                    System.Diagnostics.Debug.WriteLine("test_convertChar2Uint8(): pass");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("test_convertChar2Uint8(): fail");
                }
            }
        }

        private static void test_convertUint8toByte()
        {
            char c1 = 'a';
            // 128: 1000 0000 should be -128 in byte
            // 255: 1111 1111 should be -1 in byte
            char c2 = (char)128;
            char c3 = (char)255;
            unchecked
            {
                if (convertUint8toByte(c1) == 97 && convertUint8toByte(c2) == (byte)-128
                        && convertUint8toByte(c3) == (byte)-1)
                {
                    System.Diagnostics.Debug.WriteLine("test_convertUint8toByte(): pass");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("test_convertUint8toByte(): fail");
                }
            }
        }

        private static void test_parseBssid()
        {
            unchecked
            {
                byte[] b = new byte[] { (byte)15, (byte)-2, (byte)52, (byte)-102, (byte)-93, (byte)-60 };
                string r = parseBssid(b);

                if (string.Compare(r, "0ffe349aa3c4", true) == 0)
                {
                    System.Diagnostics.Debug.WriteLine("test_parseBssid(): pass");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"test_parseBssid(): fail {r}");
                }
            }
        }

        public static void test()
        {
            test_convertUint8toByte();
            test_convertChar2Uint8();
            test_splitUint8To2bytes();
            test_combine2bytesToOne();
            test_parseBssid();
        }

    }
}