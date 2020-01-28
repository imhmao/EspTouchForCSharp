using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading;
using EspTouchForCSharp.Protocol;
using EspTouchForCSharp.Security;
using EspTouchForCSharp.Task;
using EspTouchForCSharp.Udp;
using EspTouchForCSharp.Util;

namespace EspTouchForCSharp
{
    /// <summary>
    /// Esptouch 智能配网
    /// </summary>
    public class EsptouchTask : IDisposable
    {
        #region IDisposable 成员

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        protected virtual void Dispose(bool disposing)
        {

            if (disposing)
            {
                if (this.mTask != null)
                {
                    try
                    {
                        this.mTask.Abort();
                    }
                    catch
                    {

                    }

                    this.mTask = null;
                }
                if (this.mSocketClient != null)
                    this.mSocketClient.Dispose();

                if (this.mSocketServer != null)
                    this.mSocketServer.Dispose();

                this.mSocketClient = null;
                this.mSocketServer = null;



            }
        }


        #endregion


        public const string ESPTOUCH_VERSION = "v0.3.7.2";

        /**
          * one indivisible data contain 3 9bits info
          */
        private static readonly int ONE_DATA_LEN = 3;

        private UdpSocketClient mSocketClient;
        private UdpSocketServer mSocketServer;
        private byte[] mApSsid;
        private byte[] mApPassword;
        private byte[] mApBssid;
        private ITouchEncryptor mEncryptor;
        private List<EsptouchResult> mEsptouchResultList;
        private volatile bool mIsSuc = false;
        private volatile bool mIsInterrupt = false;
        private volatile bool mIsExecuted = false;
        private bool mIsCancelled;
        private IEsptouchTaskParameter mParameter;
        private Dictionary<string, int> mBssidTaskSucCountMap;

        public event EventHandler<EsptouchEventArgs> Find;

        private Thread mTask;

        /// <summary>
        /// 配网控制构造
        /// </summary>
        /// <param name="apSsid">SSID</param>
        /// <param name="apBssid">BSSID</param>
        /// <param name="apPassword">PWD</param>
        /// <param name="broadcast">
        /// 是否是广播模式
        /// 默认：true , false 是组播模式
        /// 组播模式必须跟 esp8266 在一个 wifi 路由下 esp8266 才能收到
        /// </param>
        public EsptouchTask(string apSsid, string apBssid, string apPassword, bool broadcast = true)
        {
            if (string.IsNullOrEmpty(apSsid))
            {
                throw new EsptouchException("SSID can't be empty");
            }

            byte[] ssid = ByteUtil.getBytesByString(apSsid);

            if (string.IsNullOrEmpty(apBssid))
            {
                throw new EsptouchException("BSSID is empty");
            }

            byte[] bssid = TouchNetUtil.parseBssid2bytes(apBssid);

            if (bssid.Length != 6)
            {
                throw new EsptouchException("BSSID is length is not 6");
            }

            byte[] pwd = new byte[0];
            if (!string.IsNullOrEmpty(apPassword))
            {
                pwd = ByteUtil.getBytesByString(apPassword);
            }

            var _mParameter = new EsptouchTaskParameter();
            _mParameter.Broadcast = broadcast;

            Debug.WriteLine($"ssid:{Convert.ToBase64String(ssid)}");
            Debug.WriteLine($"pwd:{ Convert.ToBase64String(pwd)}");


            this.init(new TouchData(ssid), new TouchData(bssid), new TouchData(pwd), null, _mParameter);

        }

        public EsptouchTask(TouchData apSsid, TouchData apBssid, TouchData apPassword,
                          ITouchEncryptor encryptor, IEsptouchTaskParameter parameter)
        {
            this.init(apSsid, apBssid, apPassword, encryptor, parameter);
        }


        private void init(TouchData apSsid, TouchData apBssid, TouchData apPassword, ITouchEncryptor encryptor, IEsptouchTaskParameter parameter)
        {

            Debug.WriteLine($"Welcome Esptouch {ESPTOUCH_VERSION}");

            mEncryptor = encryptor;
            mApSsid = apSsid.getData();
            mApPassword = apPassword.getData();
            mApBssid = apBssid.getData();

            mIsCancelled = false;

            mSocketClient = new UdpSocketClient();
            mParameter = parameter;

            mSocketServer = new UdpSocketServer(mParameter.PortListening,
                    mParameter.WaitUdpTotalMillisecond);

            mEsptouchResultList = new List<EsptouchResult>();
            mBssidTaskSucCountMap = new Dictionary<string, int>();
        }


        private long currentTimeMillis()
        {
            return (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
        }

        private readonly object resultLockObject = new object();

        private void __putEsptouchResult(bool isSuc, string bssid, IPAddress IPAddress)
        {
            lock (resultLockObject)
            {
                // check whether the result receive enough UDP response
                bool isTaskSucCountEnough = false;
                int count = 0;

                if (mBssidTaskSucCountMap.ContainsKey(bssid))
                {
                    count = mBssidTaskSucCountMap[bssid];
                }
                else
                {
                    mBssidTaskSucCountMap.Add(bssid, count);
                }

                count++;

                Debug.WriteLine($"__putEsptouchResult(): count = {count}");


                mBssidTaskSucCountMap[bssid] = count;

                isTaskSucCountEnough = count >= mParameter.ThresholdSucBroadcastCount;

                if (!isTaskSucCountEnough)
                {
                    Debug.WriteLine($"__putEsptouchResult(): count = {count} , isn't enough");
                    return;
                }


                // check whether the result is in the mEsptouchResultList already
                bool isExist = false;
                foreach (EsptouchResult esptouchResultInList in mEsptouchResultList)
                {
                    if (string.Compare(esptouchResultInList.Bssid, bssid, true) == 0)
                    {
                        isExist = true;
                        break;
                    }
                }

                // only add the result who isn't in the mEsptouchResultList
                if (!isExist)
                {
                    Debug.WriteLine($"__putEsptouchResult(): put one more result bssid = {bssid} address ={IPAddress} ");


                    EsptouchResult esptouchResult = new EsptouchResult(isSuc,
                           bssid, IPAddress);

                    mEsptouchResultList.Add(esptouchResult);

                    if (this.Find != null)
                    {
                        this.Find(this, new EsptouchEventArgs { Driver = esptouchResult });
                    }

                    if (mEsptouchResultList.Count == this.mParameter.ExpectTaskResultCount)
                        this.Interrupt();
                }
            }
        }

        private readonly object getEsptouchResultLockObject = new object();

        private List<EsptouchResult> __getEsptouchResultList()
        {
            lock (getEsptouchResultLockObject)
            {
                //if (mEsptouchResultList.Count == 0)
                //{
                //    EsptouchResult esptouchResultFail = new EsptouchResult(false,
                //            null, null);
                //    esptouchResultFail.IsCancelled = mIsCancelled;
                //    mEsptouchResultList.Add(esptouchResultFail);
                //}

                return mEsptouchResultList;
            }
        }

        private readonly object interruptLockObject = new object();

        private void __interrupt()
        {
            lock (interruptLockObject)
            {
                if (!mIsInterrupt)
                {
                    mIsInterrupt = true;
                    mSocketClient.Interrupt();
                    mSocketServer.Interrupt();
                    // interrupt the current Thread which is used to wait for udp response
                    if (mTask != null)
                    {
                        try
                        {
                            mTask.Abort();
                            mTask = null;
                        }
                        catch { }
                    }
                }
            }
        }

        private readonly object InterruptLockObject = new object();
        public void Interrupt()
        {
            lock (InterruptLockObject)
            {
                Debug.WriteLine("interrupt()");

                mIsCancelled = true;
                __interrupt();
            }
        }

        private void __listenAsyn(int expectDataLen)
        {
            mTask = new Thread(run);
            mTask.Start(expectDataLen);
        }

        private void run(object para)
        {
            int expectDataLen = (int)para;

            Debug.WriteLine($"__listenAsyn() start expectDataLen: {expectDataLen}");

            long startTimestamp = currentTimeMillis();

            //                byte[] apSsidAndPassword = ByteUtil.getBytesByString(mApSsid
            //                        + mApPassword);

            byte expectOneByte = (byte)(mApSsid.Length + mApPassword.Length + 9);

            Debug.WriteLine($"expectOneByte:  {((int)expectOneByte).ToString("x")}");

            unchecked
            {
                byte receiveOneByte;
                byte[] receiveBytes;

                while (mEsptouchResultList.Count < mParameter.ExpectTaskResultCount && !mIsInterrupt)
                {
                    receiveBytes = null;
                    try
                    {
                        receiveBytes = mSocketServer
                                .receiveSpecLenBytes(expectDataLen);
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine($"receiveSpecLenBytes()  error { e.Message }");

                    }

                    if (receiveBytes != null)
                    {
                        receiveOneByte = receiveBytes[0];
                    }
                    else
                    {
                        receiveOneByte = (byte)-1;
                    }
                    if (receiveOneByte == expectOneByte)
                    {
                        Debug.WriteLine("receive correct broadcast");

                        // change the socket's timeout
                        long consume = currentTimeMillis() - startTimestamp;

                        int timeout = (int)(mParameter.WaitUdpTotalMillisecond - consume);

                        if (timeout < 0)
                        {
                            Debug.WriteLine("esptouch timeout");
                            break;
                        }
                        else
                        {
                            Debug.WriteLine($"mSocketServer's new timeout is {timeout} milliseconds");

                            mSocketServer.Timeout = timeout;

                            Debug.WriteLine("receive correct broadcast");

                            if (receiveBytes != null)
                            {
                                string bssid = ByteUtil.parseBssid(receiveBytes,
                                                                   mParameter.EsptouchResultOneLen,
                                                                   mParameter.EsptouchResultMacLen);

                                IPAddress inetAddress = TouchNetUtil.parseInetAddr(
                                                receiveBytes,
                                                mParameter.EsptouchResultOneLen + mParameter.EsptouchResultMacLen,
                                                mParameter.EsptouchResultIpLen);

                                __putEsptouchResult(true, bssid, inetAddress);
                            }
                        }
                    }
                    else
                    {
                        Debug.WriteLine("receive rubbish message, just ignore");
                    }
                }


                mIsSuc = mEsptouchResultList.Count >= mParameter.ExpectTaskResultCount;

                ///__EsptouchTask.this.__interrupt();

                Debug.WriteLine("__listenAsyn() finish");

            }
        }

        private bool __execute(IEsptouchGenerator generator)
        {

            long startTime = currentTimeMillis();
            long currentTime = startTime;
            long lastTime = currentTime - mParameter.TimeoutTotalCodeMillisecond;

            byte[][] gcBytes2 = generator.getGCBytes2();
            byte[][] dcBytes2 = generator.getDCBytes2();

            int index = 0;
            while (!mIsInterrupt)
            {
                if (currentTime - lastTime >= mParameter.TimeoutTotalCodeMillisecond)
                {
                    Debug.WriteLine("send gc code ");

                    // send guide code
                    while (!mIsInterrupt && currentTimeMillis() - currentTime < mParameter.TimeoutGuideCodeMillisecond)
                    {

                        mSocketClient.SendData(gcBytes2,
                                mParameter.TargetHostname,
                                mParameter.TargetPort,
                                mParameter.IntervalGuideCodeMillisecond);

                        // check whether the udp is send enough time
                        if (currentTimeMillis() - startTime > mParameter.WaitUdpSendingMillisecond)
                        {
                            break;
                        }
                    }
                    lastTime = currentTime;
                }
                else
                {
                    mSocketClient.SendData(dcBytes2, index, ONE_DATA_LEN,
                            mParameter.TargetHostname,
                            mParameter.TargetPort,
                            mParameter.IntervalDataCodeMillisecond);
                    index = (index + ONE_DATA_LEN) % dcBytes2.Length;
                }

                currentTime = currentTimeMillis();
                // check whether the udp is send enough time
                if (currentTime - startTime > mParameter.WaitUdpSendingMillisecond)
                {
                    break;
                }
            }

            return mIsSuc;
        }

        private void __checkTaskValid()
        {
            // !!!NOTE: the esptouch task could be executed only once
            if (this.mIsExecuted)
            {
                throw new EsptouchException("the Esptouch task could be executed only once");
            }
            this.mIsExecuted = true;
        }


        public bool isCancelled()
        {
            return this.mIsCancelled;
        }

        public EsptouchResult ExecuteForResult(IPAddress localInetAddress)
        {
            return ExecuteForResults(1, localInetAddress)[0];
        }

        public List<EsptouchResult> ExecuteForResults(int expectTaskResultCount, IPAddress localInetAddress)
        {
            __checkTaskValid();

            mParameter.ExpectTaskResultCount = expectTaskResultCount;

            Debug.WriteLine("execute() ");


            Debug.WriteLine($"localInetAddress: {localInetAddress}");

            // generator the esptouch byte[][] to be transformed, which will cost
            // some time(maybe a bit much)
            IEsptouchGenerator generator = new EsptouchGenerator(mApSsid, mApBssid,
                    mApPassword, localInetAddress, mEncryptor);

            // listen the esptouch result asyn
            __listenAsyn(mParameter.EsptouchResultTotalLen);

            bool isSuc = false;
            for (int i = 0; i < mParameter.TotalRepeatTime; i++)
            {
                isSuc = __execute(generator);
                if (isSuc)
                {
                    return __getEsptouchResultList();
                }
            }

            if (!mIsInterrupt)
            {
                // wait the udp response without sending udp broadcast
                try
                {
                    Thread.Sleep(mParameter.WaitUdpReceivingMillisecond);
                }
                catch (Exception e)
                {
                    // receive the udp broadcast or the user interrupt the task
                    if (this.mIsSuc)
                    {
                        return __getEsptouchResultList();
                    }
                    else
                    {
                        this.__interrupt();
                        return __getEsptouchResultList();
                    }
                }
                this.__interrupt();
            }

            return __getEsptouchResultList();
        }

    }

}
