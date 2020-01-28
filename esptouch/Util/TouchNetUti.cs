using System;
using System.Linq;
using System.Net;

namespace EspTouchForCSharp.Util
{
    public static class TouchNetUtil
    {

        /**
         * get the local ip address by Android System
         *
         * @param context the context
         * @return the local ip addr allocated by Ap
         */
        public static IPAddress[] GetLocalIPAddress()
        {
            string HostName = Dns.GetHostName(); //得到主机名
            return Dns.GetHostEntry(HostName).AddressList;
        }


        /**
         * parse IPAddress
         *
         * @param inetAddrBytes
         * @return
         */
        public static IPAddress parseInetAddr(byte[] inetAddrBytes, int offset, int count)
        {
            byte[] ips = new byte[count];
            Array.Copy(inetAddrBytes, offset, ips, 0, count);

            return new IPAddress(ips);
        }

        /**
         * parse bssid
         *
         * @param bssid the bssid like aa:bb:cc:dd:ee:ff
         * @return byte converted from bssid
         */
        public static byte[] parseBssid2bytes(string bssid)
        {
            string[] bssidSplits = bssid.Split(':');
            byte[] result = new byte[bssidSplits.Length];
            for (int i = 0; i < bssidSplits.Length; i++)
            {
                result[i] = (byte)int.Parse(bssidSplits[i], System.Globalization.NumberStyles.HexNumber);
            }
            return result;
        }

        public static string bssidToString(byte[] macAddr)
        {
            return string.Join(":", macAddr.Select(b => b.ToString("x2")).ToArray());

        }
      
    }

}
