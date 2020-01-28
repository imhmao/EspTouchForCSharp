namespace EspTouchForCSharp.Security
{
    public class TouchAES : ITouchEncryptor
    {
        public TouchAES(byte[] key) : this(key, null)
        {

        }

        public TouchAES(byte[] key, byte[] iv)
        {

        }

        public byte[] encrypt(byte[] content)
        {
            return null;
        }
    }
}