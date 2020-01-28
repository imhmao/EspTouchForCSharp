using System;
using System.Collections.Generic;
using System.Text;

namespace EspTouchForCSharp.Util
{
    public class CRC8
    {
        /// <summary>
        /// 无符号右移，与JS中的>>>等价
        /// </summary>
        /// <param name="x">要移位的数</param>
        /// <param name="y">移位数</param>
        /// <returns></returns>
        public static int UIntMoveRight(int x, int y)
        {
            int mask = 0x7fffffff; //Integer.MAX_VALUE
            for (int i = 0; i < y; i++)
            {
                x >>= 1;
                x &= mask;
            }
            return x;
        }

        private static readonly short[] crcTable = new short[256];
        private static readonly short CRC_POLYNOM = 0x8c;
        private static readonly short CRC_INITIAL = 0x00;

        static CRC8()
        {
            for (int dividend = 0; dividend < 256; dividend++)
            {
                int remainder = dividend; // << 8;
                for (int bit = 0; bit < 8; ++bit)
                    if ((remainder & 0x01) != 0)
                        remainder = (remainder >> 1) ^ CRC_POLYNOM;
                    else
                        remainder >>= 1;
                crcTable[dividend] = (short)remainder;
            }
        }

        private readonly short init;
        private short value;

        public CRC8()
        {
            this.init = CRC_INITIAL;
            this.value = this.init;
        }

        public void update(byte[] buffer, int offset, int len)
        {
            for (int i = 0; i < len; i++)
            {
                int data = buffer[offset + i] ^ value;
                value = (short)(crcTable[data & 0xff] ^ (value << 8));
            }
        }

        /**
         * Updates the current checksum with the specified array of bytes.
         * Equivalent to calling <code>update(buffer, 0, buffer.length)</code>.
         *
         * @param buffer the byte array to update the checksum with
         */
        public void update(byte[] buffer)
        {
            update(buffer, 0, buffer.Length);
        }

        public void update(int b)
        {
            update(new byte[] { (byte)b }, 0, 1);
        }

        public long getValue()
        {
            return value & 0xff;
        }

        public void reset()
        {
            value = init;
        }

    }

}
