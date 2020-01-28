using System;
using System.Net;
using System.Text;
using EspTouchForCSharp.Security;
using EspTouchForCSharp.Task;
using EspTouchForCSharp.Util;

namespace EspTouchForCSharp.Protocol
{
    public class EsptouchGenerator : IEsptouchGenerator
    {

        private readonly byte[][] mGcBytes2;
        private readonly byte[][] mDcBytes2;

        /**
         * Constructor of EsptouchGenerator, it will cost some time(maybe a bit
         * much)
         *
         * @param apSsid      the Ap's ssid
         * @param apBssid     the Ap's bssid
         * @param apPassword  the Ap's password
         * @param inetAddress the phone's or pad's local ip address allocated by Ap
         */
        public EsptouchGenerator(byte[] apSsid, byte[] apBssid, byte[] apPassword, IPAddress inetAddress,
                                 ITouchEncryptor encryptor)
        {
            // generate guide code
            GuideCode gc = new GuideCode();
            char[] gcU81 = gc.getU8s();
            mGcBytes2 = new byte[gcU81.Length][];

            for (int i = 0; i < mGcBytes2.Length; i++)
            {
                mGcBytes2[i] = ByteUtil.genSpecBytes(gcU81[i]);
            }

            // generate data code
            DatumCode dc = new DatumCode(apSsid, apBssid, apPassword, inetAddress, encryptor);
            char[] dcU81 = dc.getU8s();
            
            string dcstr = new string(dcU81);
            
            string debug = Convert.ToBase64String(Encoding.UTF8.GetBytes(dcstr));

            System.Diagnostics.Debug.WriteLine($"DatumCode({dcU81.Length}): {debug}");

            mDcBytes2 = new byte[dcU81.Length][];

            for (int i = 0; i < mDcBytes2.Length; i++)
            {
                mDcBytes2[i] = ByteUtil.genSpecBytes(dcU81[i]);
            }
        }

        public byte[][] getGCBytes2()
        {
            return mGcBytes2;
        }

        public byte[][] getDCBytes2()
        {
            return mDcBytes2;
        }

    }

}
