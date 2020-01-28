using System.Text;
using EspTouchForCSharp.Task;
using EspTouchForCSharp.Util;

namespace EspTouchForCSharp.Protocol
{
    public class GuideCode : ICodeData
    {

        public static readonly int GUIDE_CODE_LEN = 4;

        public byte[] getBytes()
        {
            throw new EsptouchException("DataCode don't support getBytes()");
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            char[] dataU8s = getU8s();
            for (int i = 0; i < GUIDE_CODE_LEN; i++)
            {
                string hexString = ByteUtil.convertU8ToHexString(dataU8s[i]);
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
            char[] guidesU8s = new char[GUIDE_CODE_LEN];
            guidesU8s[0] = (char)515;
            guidesU8s[1] = (char)514;
            guidesU8s[2] = (char)513;
            guidesU8s[3] = (char)512;
            return guidesU8s;
        }
    }

}
