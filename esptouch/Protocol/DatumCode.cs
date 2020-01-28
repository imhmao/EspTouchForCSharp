using System.Collections.Generic;
using System.Net;
using System.Text;
using EspTouchForCSharp.Security;
using EspTouchForCSharp.Task;
using EspTouchForCSharp.Util;

namespace EspTouchForCSharp.Protocol
{

    public class DatumCode : ICodeData
    {

        // define by the Esptouch protocol, all of the datum code should add 1 at last to prevent 0
        private static readonly int EXTRA_LEN = 40;
        private static readonly int EXTRA_HEAD_LEN = 5;

        private readonly List<DataCode> mDataCodes;

        /**
         * Constructor of DatumCode
         *
         * @param apSsid      the Ap's ssid
         * @param apBssid     the Ap's bssid
         * @param apPassword  the Ap's password
         * @param ipAddress   the ip address of the phone or pad
         * @param encryptor null use origin data, not null use encrypted data
         */
        public DatumCode(byte[] apSsid, byte[] apBssid, byte[] apPassword,
                         IPAddress ipAddress, ITouchEncryptor encryptor)
        {
            // Data = total len(1 byte) + apPwd len(1 byte) + SSID CRC(1 byte) +
            // BSSID CRC(1 byte) + TOTAL XOR(1 byte)+ ipAddress(4 byte) + apPwd + apSsid apPwdLen <=
            // 105 at the moment

            // total xor
            char totalXor = (char)0;

            char apPwdLen = (char)apPassword.Length;
            CRC8 crc = new CRC8();
            crc.update(apSsid);
            char apSsidCrc = (char)crc.getValue();

            crc.reset();
            crc.update(apBssid);
            char apBssidCrc = (char)crc.getValue();

            char apSsidLen = (char)apSsid.Length;

            byte[] ipBytes = ipAddress.GetAddressBytes();
            int ipLen = ipBytes.Length;

            char totalLen = (char)(EXTRA_HEAD_LEN + ipLen + apPwdLen + apSsidLen);

            // build data codes
            mDataCodes = new List<DataCode>();
            mDataCodes.Add(new DataCode(totalLen, 0));
            totalXor ^= totalLen;
            mDataCodes.Add(new DataCode(apPwdLen, 1));
            totalXor ^= apPwdLen;
            mDataCodes.Add(new DataCode(apSsidCrc, 2));
            totalXor ^= apSsidCrc;
            mDataCodes.Add(new DataCode(apBssidCrc, 3));
            totalXor ^= apBssidCrc;
            // ESPDataCode 4 is null
            for (int i = 0; i < ipLen; ++i)
            {
                char c = ByteUtil.convertByte2Uint8(ipBytes[i]);
                totalXor ^= c;
                mDataCodes.Add(new DataCode(c, i + EXTRA_HEAD_LEN));
            }

            for (int i = 0; i < apPassword.Length; i++)
            {
                char c = ByteUtil.convertByte2Uint8(apPassword[i]);
                totalXor ^= c;
                mDataCodes.Add(new DataCode(c, i + EXTRA_HEAD_LEN + ipLen));
            }

            // totalXor will xor apSsidChars no matter whether the ssid is hidden
            for (int i = 0; i < apSsid.Length; i++)
            {
                char c = ByteUtil.convertByte2Uint8(apSsid[i]);
                totalXor ^= c;
                mDataCodes.Add(new DataCode(c, i + EXTRA_HEAD_LEN + ipLen + apPwdLen));
            }

            // add total xor last
            mDataCodes.Insert(4, new DataCode(totalXor, 4));
            
            // add bssid
            int bssidInsertIndex = EXTRA_HEAD_LEN;
            for (int i = 0; i < apBssid.Length; i++)
            {
                int index = totalLen + i;
                char c = ByteUtil.convertByte2Uint8(apBssid[i]);
                DataCode dc = new DataCode(c, index);
                if (bssidInsertIndex >= mDataCodes.Count)
                {
                    mDataCodes.Add(dc);
                }
                else
                {
                    mDataCodes.Insert(bssidInsertIndex, dc);
                }
                bssidInsertIndex += 4;
            }
        }

        public byte[] getBytes()
        {
            byte[] datumCode = new byte[mDataCodes.Count * DataCode.DATA_CODE_LEN];
            int index = 0;
            foreach (DataCode dc in mDataCodes)
            {
                foreach (byte b in dc.getBytes())
                {
                    datumCode[index++] = b;
                }
            }
            return datumCode;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            byte[] dataBytes = getBytes();
            foreach (byte dataByte in dataBytes)
            {
                string hexString = ByteUtil.convertByte2HexString(dataByte);
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
            byte[] dataBytes = getBytes();
            int len = dataBytes.Length / 2;
            char[] dataU8s = new char[len];
            byte high, low;
            for (int i = 0; i < len; i++)
            {
                high = dataBytes[i * 2];
                low = dataBytes[i * 2 + 1];
                dataU8s[i] = (char)(ByteUtil.combine2bytesToU16(high, low) + EXTRA_LEN);
            }
            return dataU8s;
        }
    }

}
