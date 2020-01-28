using System;
using System.Collections.Generic;
using System.Text;

namespace EspTouchForCSharp.Security
{
    public interface ITouchEncryptor
    {
        byte[] encrypt(byte[] src);
    }
}
