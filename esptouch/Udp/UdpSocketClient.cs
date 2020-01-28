using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace EspTouchForCSharp.Udp
{
    public class UdpSocketClient : IDisposable
    {
        #region IDisposable 成员

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        protected virtual void Dispose(bool disposing)
        {

            if (disposing && mSocket != null)
            {
                mSocket.Close();
                mSocket.Dispose();
                mSocket = null;
            }
        }


        #endregion

        private UdpClient mSocket;
        private volatile bool mIsStop;
        private volatile bool mIsClosed;

        public UdpSocketClient()
        {
            this.mSocket = new UdpClient();

            //this.mSocket.JoinMulticastGroup(IPAddress.Parse("224.0.0.2"));
            //this.mSocket.EnableBroadcast = false;
            //this.mSocket.AllowNatTraversal(false);
            //this.mSocket.DontFragment = false;

            this.mIsStop = false;
            this.mIsClosed = false;

        }

        public void Interrupt()
        {
            this.mIsStop = true;
        }

        private readonly object closeLock = new object();

        /**
         * close the UDP socket
         */
        public void Close()
        {
            lock (closeLock)
            {
                if (!this.mIsClosed && this.mSocket != null)
                {
                    this.mSocket.Close();
                    this.mIsClosed = true;
                }
            }
        }

        /**
         * send the data by UDP
         *
         * @param data       the data to be sent
         * @param targetPort the port of target
         * @param interval   the milliseconds to between each UDP sent
         */
        public void SendData(byte[][] data, IPAddress targetInetAddress, int targetPort, int interval)
        {
            SendData(data, 0, data.Length, targetInetAddress, targetPort, interval);
        }

       // private string[] prochar = new string[] { "|", "/", "-", "\\" };

        private int maxPro = 30;

        private int proIdx = 0;

        private int startIdx = -1;


        /**
         * send the data by UDP
         *
         * @param data       the data to be sent
         * @param offset     the offset which data to be sent
         * @param count      the count of the data
         * @param targetPort the port of target
         * @param interval   the milliseconds to between each UDP sent
         */
        public void SendData(byte[][] data, int offset, int count,
                             IPAddress targetInetAddress, int targetPort, int interval)
        {
            if ((data == null) || (data.Length <= 0))
            {
                System.Diagnostics.Debug.WriteLine("sendData(): data == null or length <= 0");
                return;
            }

            for (int i = offset; !mIsStop && i < offset + count; i++)
            {
                if (data[i].Length == 0)
                {
                    continue;
                }
                try
                {
                    IPEndPoint target = new IPEndPoint(targetInetAddress, targetPort);

                    //System.Console.WriteLine($"index {i}");
                    //System.Console.WriteLine($"port {targetPort}");
                    //System.Console.WriteLine($"targetHostName {targetInetAddress}");
                    //string debug = Convert.ToBase64String(data[i]);
                    //System.Console.WriteLine($"data({debug.Length}) {debug}");

                    if(startIdx<0)
                        startIdx = System.Console.CursorLeft ;
                    
                    if(proIdx == maxPro)
                    {
                        proIdx = 0;
                        System.Console.CursorLeft = startIdx;
                        System.Console.Write(new string(' ', maxPro));
                        System.Console.CursorLeft = startIdx;
                    }

                    System.Console.Write("*");

                    proIdx++;

                    this.mSocket.Send(data[i], data[i].Length, target);
                }
                catch (SocketException e)
                {
                    System.Diagnostics.Debug.WriteLine($"sendData(): SocketException, but just ignore it ({e.Message})");

                    // for the Ap will make some troubles when the phone send too many UDP packets,
                    // but we don't expect the UDP packet received by others, so just ignore it
                }


                Thread.Sleep(interval);

            }
            if (mIsStop)
            {
                Close();
            }
        }
    }

}
