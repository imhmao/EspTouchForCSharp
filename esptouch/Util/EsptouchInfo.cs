using System.Net;

namespace EspTouchForCSharp.Util
{
    public class EsptouchInfo
    {
        public EsptouchInfo()
        {
            this.IP = null;
            this.Devices = 1;
            this.Broadcast = true;
        }

        public string SSID { get; set; }

        public string BSSID { get; set; }

        public string Password { get; set; }

        public IPAddress IP { get; set; }

        public bool Broadcast { get; set; }

        public int Devices { get; set; }

        public override string ToString()
        {
            string empty = "(empty)";
            return $"SSID:{(string.IsNullOrEmpty(this.SSID)?empty:this.SSID)} BSSID:{(string.IsNullOrEmpty(this.BSSID) ? empty : this.BSSID)} Password:{(string.IsNullOrEmpty(this.Password) ? empty : this.Password)} IP:{(this.IP == null ? "(null)" : this.IP.ToString())} Broadcast:{this.Broadcast} Devices:{this.Devices}";
        }
    }
}
