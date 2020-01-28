using System;
using System.Collections.Generic;
using System.Text;

namespace EspTouchForCSharp
{
    public class EsptouchEventArgs :EventArgs
    {
        public EsptouchResult Driver { get; set; }
    }
}
