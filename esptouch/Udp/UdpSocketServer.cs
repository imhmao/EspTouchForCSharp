using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace EspTouchForCSharp.Udp
{
    public class UdpSocketServer : IDisposable
    {
        #region IDisposable 成员

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        protected virtual void Dispose(bool disposing)
        {

            if (disposing && mServerSocket != null)
            {
                mServerSocket.Close();
                mServerSocket.Dispose();
                mServerSocket = null;
            }
        }


        #endregion

        private UdpClient mServerSocket;
        private volatile bool mIsClosed;

        /**
         * Constructor of UDP Socket Server
         *
         * @param port          the Socket Server port
         * @param socketTimeout the socket read timeout
         * @param context       the context of the Application
         */
        public UdpSocketServer(int port, int socketTimeout)
        {
            IPEndPoint ipep = new IPEndPoint(IPAddress.Any, port);//定义一网络端点
            this.mServerSocket = new UdpClient(ipep);
            this.mServerSocket.Client.ReceiveTimeout = socketTimeout;

            this.mIsClosed = false;

        }

        /**
         * Receive one byte from the port and convert it into String
         *
         * @return
         */
        public byte receiveOneByte()
        {

            IPEndPoint client = new IPEndPoint(IPAddress.Any, 0);
            byte[] data = this.mServerSocket.Receive(ref client);

            System.Diagnostics.Debug.WriteLine($"receive: {((int)data[0]).ToString("x")}");

            return data[0];

        }

        /**
         * Receive specific length bytes from the port and convert it into String
         * 21,24,-2,52,-102,-93,-60
         * 15,18,fe,34,9a,a3,c4
         *
         * @return
         */
        public byte[] receiveSpecLenBytes(int len)
        {

            //while (this.mServerSocket.Available <= 0)
            //    Thread.Sleep(100);

            System.Diagnostics.Debug.WriteLine($"receiveSpecLenBytes(): entrance: len = {len}");


            IPEndPoint client = new IPEndPoint(IPAddress.Any, 0);
            byte[] recDatas = this.mServerSocket.Receive(ref client);

            System.Diagnostics.Debug.WriteLine($"received len :{recDatas.Length}");

            for (int i = 0; i < recDatas.Length; i++)
            {
                System.Diagnostics.Debug.WriteLine($"recDatas[{i}] :{ ((int)recDatas[i]).ToString("x")}");
            }

            System.Diagnostics.Debug.WriteLine($"receiveSpecLenBytes:  {Encoding.UTF8.GetString(recDatas)}");

            if (recDatas.Length != len)
            {
                System.Diagnostics.Debug.WriteLine("received len is different from specific len, return null");
                return null;
            }

            return recDatas;

        }

        public void Interrupt()
        {
            this.Close();
        }

        private readonly object closeLock = new object();

        public int Timeout
        {
            get
            {
                return this.mServerSocket.Client.ReceiveTimeout;
            }
            set
            {
                if (this.mServerSocket != null && this.mServerSocket.Client != null)
                {
                    this.mServerSocket.Client.ReceiveTimeout = value;
                }
            }

        }


        public void Close()
        {
            lock (closeLock)
            {
                if (!this.mIsClosed)
                {
                    mServerSocket.Close();
                    this.mIsClosed = true;
                }
            }
        }


    }

}
