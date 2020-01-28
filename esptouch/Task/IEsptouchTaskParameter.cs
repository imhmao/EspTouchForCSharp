using System.Net;

namespace EspTouchForCSharp.Task
{
    public interface IEsptouchTaskParameter
    {

        /**
         * get interval millisecond for guide code(the time between each guide code sending)
         *
         * @return interval millisecond for guide code(the time between each guide code sending)
         */
        int IntervalGuideCodeMillisecond { get; }

        /**
         * get interval millisecond for data code(the time between each data code sending)
         *
         * @return interval millisecond for data code(the time between each data code sending)
         */
        int IntervalDataCodeMillisecond { get; }

        /**
         * get timeout millisecond for guide code(the time how much the guide code sending)
         *
         * @return timeout millisecond for guide code(the time how much the guide code sending)
         */
        long TimeoutGuideCodeMillisecond { get; }

        /**
         * get timeout millisecond for data code(the time how much the data code sending)
         *
         * @return timeout millisecond for data code(the time how much the data code sending)
         */
        long TimeoutDataCodeMillisecond { get; }

        /**
         * get timeout millisecond for total code(guide code and data code altogether)
         *
         * @return timeout millisecond for total code(guide code and data code altogether)
         */
        long TimeoutTotalCodeMillisecond { get; }

        /**
         * get total repeat time for executing esptouch task
         *
         * @return total repeat time for executing esptouch task
         */
        int TotalRepeatTime { get; }

        /**
         * the length of the Esptouch result 1st byte is the total length of ssid and
         * password, the other 6 bytes are the device's bssid
         */

        /**
         * get esptouchResult length of one
         *
         * @return length of one
         */
        int EsptouchResultOneLen { get; }

        /**
         * get esptouchResult length of mac
         *
         * @return length of mac
         */
        int EsptouchResultMacLen { get; }

        /**
         * get esptouchResult length of ip
         *
         * @return length of ip
         */
        int EsptouchResultIpLen { get; }

        /**
         * get esptouchResult total length
         *
         * @return total length
         */
        int EsptouchResultTotalLen { get; }

        /**
         * get port for listening(used by server)
         *
         * @return port for listening(used by server)
         */
        int PortListening { get; }

        /**
         * get target hostname
         *
         * @return target hostame(used by client)
         */
        IPAddress TargetHostname { get; }

        /**
         * get target port
         *
         * @return target port(used by client)
         */
        int TargetPort { get; }

        /**
         * get millisecond for waiting udp receiving(receiving without sending)
         *
         * @return millisecond for waiting udp receiving(receiving without sending)
         */
        int WaitUdpReceivingMillisecond { get; }

        /**
         * get millisecond for waiting udp sending(sending including receiving)
         *
         * @return millisecond for waiting udep sending(sending including receiving)
         */
        int WaitUdpSendingMillisecond { get; }

        /**
         * get millisecond for waiting udp sending and receiving
         * set the millisecond for waiting udp sending and receiving
         *
         * @return millisecond for waiting udp sending and receiving
         */
        int WaitUdpTotalMillisecond { get; set; }


        /**
         * get the threshold for how many correct broadcast should be received
         *
         * @return the threshold for how many correct broadcast should be received
         */
        int ThresholdSucBroadcastCount { get; }

        /**
         * get the count of expect task results
         * set the count of expect task results
         *
         * @return the count of expect task results
         */
        int ExpectTaskResultCount { get; set; }



        /**
         * Set broadcast or multicast
         *
         * @param broadcast true is broadcast, false is multicast
         */
        bool Broadcast { get; set; }
    }

}
