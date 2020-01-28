using EspTouchForCSharp.Util;

namespace EspTouchForCSharp.Protocol
{
    public class TouchData
    {
        private readonly byte[] mData;

        public TouchData(string str)
        {
            mData = ByteUtil.getBytesByString(str);
        }

        public TouchData(byte[] data)
        {
            if (data == null)
            {
                throw new EsptouchException("data can't be null");
            }
            mData = data;
        }

        public byte[] getData()
        {
            return mData;
        }
    }

}
