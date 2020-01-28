using System;
using System.Net;

namespace EspTouchForCSharp.Task
{
    public class EsptouchTaskParameter : IEsptouchTaskParameter
    {

        private static int _datagramCount = 0;

        public EsptouchTaskParameter()
        {
            IntervalGuideCodeMillisecond = 8;
            IntervalDataCodeMillisecond = 8;
            TimeoutGuideCodeMillisecond = 2000;
            TimeoutDataCodeMillisecond = 4000;
            TotalRepeatTime = 1;
            EsptouchResultOneLen = 1;
            EsptouchResultMacLen = 6;
            EsptouchResultIpLen = 4;
            EsptouchResultTotalLen = 1 + 6 + 4;
            PortListening = 18266;
            TargetPort = 7001;
            WaitUdpReceivingMillisecond = 15000;
            WaitUdpSendingMillisecond = 45000;
            ThresholdSucBroadcastCount = 1;
            ExpectTaskResultCount = 1;
        }


        public int IntervalGuideCodeMillisecond { get; }


        public int IntervalDataCodeMillisecond { get; }


        public long TimeoutGuideCodeMillisecond { get; }


        public long TimeoutDataCodeMillisecond { get; }


        public long TimeoutTotalCodeMillisecond
        {
            get
            {
                return TimeoutGuideCodeMillisecond + TimeoutDataCodeMillisecond;
            }
        }


        public int TotalRepeatTime { get; }


        public int EsptouchResultOneLen { get; }


        public int EsptouchResultMacLen { get; }


        public int EsptouchResultIpLen { get; }


        public int EsptouchResultTotalLen { get; }


        public int PortListening { get; }


        // the range of the result should be 1-100
        private static int __getNextDatagramCount()
        {
            return 1 + (_datagramCount++) % 100;
        }

        // target hostname is : 234.1.1.1, 234.2.2.2, 234.3.3.3 to 234.100.100.100

        public IPAddress TargetHostname
        {
            get
            {
                string ips = "255.255.255.255";
                if (!Broadcast)
                {
                    int count = __getNextDatagramCount();
                    ips = $"234.{count}.{count}.{count}";
                }

                return IPAddress.Parse(ips);

            }
        }


        public int TargetPort { get; }


        public int WaitUdpReceivingMillisecond { get; }


        public int WaitUdpSendingMillisecond { get; private set; }


        public int WaitUdpTotalMillisecond
        {
            get
            {
                return WaitUdpReceivingMillisecond + WaitUdpSendingMillisecond;
            }
            set
            {

                if (value < WaitUdpReceivingMillisecond
                        + this.TimeoutTotalCodeMillisecond)
                {
                    // if it happen, even one turn about sending udp broadcast can't be
                    // completed
                    throw new EsptouchException(
                            "waitUdpTotalMillisecod is invalid, "
                                    + "it is less than mWaitUdpReceivingMilliseond + getTimeoutTotalCodeMillisecond()");
                }
                WaitUdpSendingMillisecond = value
                        - WaitUdpReceivingMillisecond;
            }
        }


        public int ThresholdSucBroadcastCount { get; }


        public int ExpectTaskResultCount { get; set; }


        public bool Broadcast { get; set; } = true;

    }

}
