using System.Net;

namespace EspTouchForCSharp
{
    public class EsptouchResult
    {
        private bool mIsCancelled;

        /**
         * Constructor of EsptouchResult
         *
         * @param isSuc       whether the esptouch task is executed suc
         * @param bssid       the device's bssid
         * @param IPAddress the device's ip address
         */
        public EsptouchResult(bool isSuc, string bssid, IPAddress IPAddress)
        {
            this.IsSuc = isSuc;
            this.Bssid = bssid;
            this.IP = IPAddress;
            this.mIsCancelled = false;
        }

        public bool IsSuc { get; }

        public string Bssid { get; }

        private readonly object lockObj = new object();

        public bool IsCancelled
        {
            get
            {
                lock (lockObj)
                {
                    return mIsCancelled;
                }
            }
            set
            {
                lock (lockObj)
                {
                    this.mIsCancelled = value;
                }
            }
        }

        public IPAddress IP { get; }

        public override string ToString()
        {
            return $"bssid={Bssid}, address={IP}, suc={IsSuc}, cancel={mIsCancelled}";
        }
    }

}
