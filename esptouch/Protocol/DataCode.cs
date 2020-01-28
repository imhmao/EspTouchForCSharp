using System;
using System.Text;
using EspTouchForCSharp.Task;
using EspTouchForCSharp.Util;

namespace EspTouchForCSharp.Protocol
{
    /**
     * one data format:(data code should have 2 to 65 data)
     * <p>
     * control byte       high 4 bits    low 4 bits
     * 1st 9bits:       0x0             crc(high)      data(high)
     * 2nd 9bits:       0x1                sequence header
     * 3rd 9bits:       0x0             crc(low)       data(low)
     * <p>
     * sequence header: 0,1,2,...
     *
     * @author afunx
     */
    public class DataCode : ICodeData
    {

        public static readonly int DATA_CODE_LEN = 6;

        private static readonly int INDEX_MAX = 127;

        private readonly byte mSeqHeader;
        private readonly byte mDataHigh;
        private readonly byte mDataLow;
        // the crc here means the crc of the data and sequence header be transformed
        // it is calculated by index and data to be transformed
        private readonly byte mCrcHigh;
        private readonly byte mCrcLow;

        /**
         * Constructor of DataCode
         *
         * @param u8    the character to be transformed
         * @param index the index of the char
         */
        public DataCode(char u8, int index)
        {
            if (index > INDEX_MAX)
            {
                throw new EsptouchException("index > INDEX_MAX");
            }
            byte[] dataBytes = ByteUtil.splitUint8To2bytes(u8);
            mDataHigh = dataBytes[0];
            mDataLow = dataBytes[1];
            
            CRC8 crc8 = new CRC8();
            crc8.update(ByteUtil.convertUint8toByte(u8));
            crc8.update(index);

            byte[] crcBytes = ByteUtil.splitUint8To2bytes((char)crc8.getValue());
            mCrcHigh = crcBytes[0];
            mCrcLow = crcBytes[1];
            mSeqHeader = (byte)index;
        }

        public byte[] getBytes()
        {
            byte[] dataBytes = new byte[DATA_CODE_LEN];
            dataBytes[0] = 0x00;
            dataBytes[1] = ByteUtil.combine2bytesToOne(mCrcHigh, mDataHigh);
            dataBytes[2] = 0x01;
            dataBytes[3] = mSeqHeader;
            dataBytes[4] = 0x00;
            dataBytes[5] = ByteUtil.combine2bytesToOne(mCrcLow, mDataLow);
            return dataBytes;
        }

        public override String ToString()
        {
            StringBuilder sb = new StringBuilder();
            byte[] dataBytes = getBytes();
            for (int i = 0; i < DATA_CODE_LEN; i++)
            {
                String hexString = ByteUtil.convertByte2HexString(dataBytes[i]);
                sb.Append("0x");
                if (hexString.Length == 1)
                {
                    sb.Append("0");
                }
                sb.Append(hexString).Append(" ");
            }
            return sb.ToString();
        }

        public char[] getU8s()
        {
            throw new EsptouchException("DataCode don't support getU8s()");
        }

    }
}
