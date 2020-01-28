using System;
using System.Runtime.Serialization;

namespace EspTouchForCSharp
{

    [Serializable]
    public class EsptouchException : Exception
    {
        public EsptouchException()
        {
        }

        public EsptouchException(string message) : base(message)
        {
        }

        public EsptouchException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected EsptouchException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
