using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using System.Collections.Generic;
using System.Threading;

namespace Net
{
    public delegate void OnSocketCallback();

    public class USocket
    {
        public const int BUFFER_LENGTH = 1024 * 64;

        public const int DEFAULT_IO_BUFFERLENGTH = 0X8000;

        public const int SOCKET_THREAD_SLEEP_INTERVAL = 10; //消息线程轮询时间

        public USocketType type;
        private Socket socket;

        private byte[] receiveBuffer;

        private OnSocketCallback onConnect;
        private OnSocketCallback onConnectFail;
        private OnSocketCallback onClose;
        private OnSocketCallback onError;

        private Queue<NullCmd> ReceiveCmdQueue = new Queue<NullCmd>();                           //接收消息队列

        public OctetsStream SendBuffer = new OctetsStream(BUFFER_LENGTH);                          //发送缓存
        public OctetsStream ReceiveBuffer = new OctetsStream(BUFFER_LENGTH);                       //接收缓存
        public OctetsStream DecompressBuffer = new OctetsStream(BUFFER_LENGTH);                //解压数据缓存

        private int blockingResponseMsgID = 0;                              //当前正在阻塞的回馈消息ID
        private int lastBlockingRequestMsgID = 0;                           //上一条阻塞请求消息ID

        private BlockingState blockingState = BlockingState.Sendable;   //当前阻塞状态，只对阻塞消息起效

        private IPEndPoint ipEndPoint;

        //private int _reconnecttimes = 0;
        //private int reconnecttimes
        //{
        //    get { return _reconnecttimes; }
        //    set
        //    {
        //        _reconnecttimes = value;
        //    }
        //}                               //重连次数


        private long lastSendTick = 0;                                  //上次发送阻塞消息的时间

        private bool whetherCacheMsg = false;                                //是否缓存消息

        public Action ResetScene;                                       //重连时重置场景

        private Thread MsgThread;

        private bool NeedReconnect = false;

        public bool sendLoginCmdOver = true; //标记一下 登录消息是否发送完毕 服务器和客户端会在登录消息交互完之后  使用新的消息加密key

        static readonly object sendlock = new object();
        static readonly object receivelock = new object();

        private ManualResetEvent alldone = new ManualResetEvent(false);

        private Queue<NullCmdBase> receivedQueue = new Queue<NullCmdBase>();


        public USocket(USocketType type)
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            socket.NoDelay = true;
            socket.Bind(new IPEndPoint(IPAddress.Any, 0));
            this.type = type;
            receiveBuffer = new byte[BUFFER_LENGTH];
            onConnect = null;
            onClose = null;
            onError = null;
        }

        public void Connect(IPEndPoint ipEnd, OnSocketCallback onConnect = null, OnSocketCallback onConnectFail = null, OnSocketCallback onClose = null, OnSocketCallback onError = null)
        {
            this.ipEndPoint = ipEnd;
            this.onConnect = onConnect;
            this.onConnectFail = onConnectFail;
            this.onError = onError;
            this.onClose = onClose;
            ResetSocket();
            DoConncect();
        }

        void DoConncect()
        {
            try
            {
                string strLog1 = "Do Connect " + ipEndPoint + "  " + DateTime.Now;
                LSingleton<ThreadManager>.Instance.RunAsync(delegate
                {
                    try
                    {
                        strLog1 = "Run Connect Wait on thread " + Thread.CurrentThread.GetHashCode();
                        alldone.WaitOne(5000);
                        connectCallBack();
                    }
                    catch (Exception e)
                    {
                        string strLog = "ConnectWait Exception: " + e.ToString() + " " + DateTime.Now;
                    }

                });

                LSingleton<ThreadManager>.Instance.RunAsync(delegate
                {
                    try
                    {
                        strLog1 = "Run Connect on thread " + Thread.CurrentThread.GetHashCode() + "   " + ipEndPoint +
                                  "  " + DateTime.Now;
                        alldone.Reset();
                        socket.Connect(ipEndPoint);
                    }
                    catch (Exception e)
                    {
                        string strLog = "Connect Exception: " + e.ToString() + " " + DateTime.Now + "  " +
                                        socket.GetHashCode();
                        CloseSocket();
                    }
                    finally
                    {
                        //FFDebug.LogError(this, "finally alldone.Set " + DateTime.Now);
                        alldone.Set();
                    }
                });
            }
            catch (Exception e)
            {
                string strLog = "Connect Exception " + e;
                if (onConnectFail != null) onConnectFail();
            }
        }



        void connectCallBack()
        {
            string strLog1 = "connectCallBack " + DateTime.Now + "  thread: " + Thread.CurrentThread.GetHashCode();
            LSingleton<ThreadManager>.Instance.RunOnMainThread(delegate
            {
                try
                {
                    if (socket == null)
                    {
                        if (onConnectFail != null)
                            onConnectFail();
                        return;
                    }
                    socket.Blocking = false;
                    if (socket.Connected)
                    {
                        sendActions.Clear();
                        SendBuffer.clear();
                        SendBuffer.position(0);
                        lastcmdlength = 0;
                        sleepSwitch = false;
                        MsgThread = new Thread(OnReceive);
                        MsgThread.IsBackground = true;
                        MsgThread.Start();
                        if (onConnect != null) onConnect();
                    }
                    else
                    {
                        string strLog = "Connect Time Out!!! " + DateTime.Now;
                        Debug.LogError(strLog);
                        if (onConnectFail != null)
                            onConnectFail();
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(e.ToString());
                    throw;
                }

            });
        }

        /// <summary>
        /// 断线重连
        /// </summary>
        public void ReConnectGateway()
        {
            string strLog1 = String.Empty;
            if (NetWorkModule.Instance.IsReconnecting)
                return;

            NetWorkModule.Instance.IsReconnecting = true;
            
            try
            {
                ResetSocket();
                //LuaManager.GetInstance().GetLuaScriptMgr().CallLuaFunction("ControllerManager.ReConnectGateway");
                if (ResetScene != null)
                {
                    ResetScene();
                }
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                try
                {
                    strLog1 = "Do Reconnect GateWay Connect " + ipEndPoint + "  " + DateTime.Now + "  " + Thread.CurrentThread.GetHashCode();
                    Debug.Log(strLog1);
                    DoConncect();
                }
                catch (Exception e)
                {
                    string strLog = "Connect Exception " + e;
                    Debug.LogError(strLog);
                    if (onConnectFail != null) onConnectFail();
                }
            }

            catch (Exception e)
            {
                string strLog = "Reconnect Exception : " + e.Message;
                Debug.LogError(strLog);
                if (onConnectFail != null) onConnectFail();
            }
        }

        private bool send()
        {
            if (blockingState == BlockingState.Close) return false;
            bool sendResult = false;

            lock (sendlock)
            {
                try
                {
                    if (NullCmd.IS_ENCRYPT)
                    {
                        int fillcount = MessageEncrypt.FillZero(0, ref SendBuffer);
                        int lastbodylengthpos = SendBuffer.size() - lastcmdlength - fillcount - NullCmd.HEAD_SIZE;

                        if (fillcount != 0)
                        {
                            if (lastbodylengthpos < 0 || (lastbodylengthpos + 1) >= BUFFER_LENGTH)
                            {
                                throw new Exception("send缓冲区越界");
                            }
                            SendBuffer.write_ushort(lastbodylengthpos, (ushort)(fillcount + lastcmdlength));    //刷新最后个包的长度
                        }

                        lock (MessageEncrypt.DESEncryptHandle)
                        {
                            MessageEncrypt.DESEncrypt(0, SendBuffer.size(), ref SendBuffer); //DES加密
                        }
                    }

                    int offset = 0;
                    while (offset < SendBuffer.size())
                    {
                        int to = socket.Send(SendBuffer.buffer(), offset, SendBuffer.size() - offset, SocketFlags.None);
                        if (to > 0)
                            offset += to;
                    }
                    SendBuffer.clear();
                    sendResult = true;
                    lastcmdlength = 0;
                }
                catch (Exception e)
                {
                    string strLog = "send Exception  " + "  " + e.Message;
                    Debug.LogError(strLog);
                    sendResult = false;
                    SendBuffer.clear();
                    lastcmdlength = 0;
                }
            }

            return sendResult;
        }

        bool IsSendBlock(ushort cmdid)
        {
            if (NetWorkModule.Instance.BlockingMsgPair.ContainsKey(cmdid))
            {
                if (blockingState == BlockingState.Waiting)
                {
                    string strLog = "Blocking Waiting,Cant Send Msg,Last Msg ID: " + lastBlockingRequestMsgID;
                    Debug.LogWarning(strLog);
                    return true;
                }
                else
                {
                    lastSendTick = DateTime.Now.Ticks;
                    blockingResponseMsgID = NetWorkModule.Instance.BlockingMsgPair[cmdid];
                    blockingState = BlockingState.Waiting;
                    lastBlockingRequestMsgID = cmdid;
                }
            }
            return false;
        }

        private int lastcmdlength = 0;      //加密补零时刷新包长时定位使用

        Queue<Action> sendActions = new Queue<Action>();

        #region 发送
        /// <summary>
        /// 将消息写入发送缓存
        /// </summary>
        /// <param name="cmd"></param>
        public void Send(ushort cmdid, byte[] cmdData, uint cmdDataLength)
        {

#if NetWorkForge
            if (LSingleton<NetWorkForger>.Instance.OnMessage(cmdid, t) != 0) return;
#endif

            if (IsSendBlock(cmdid))
            {
                return;
            }

            sendActions.Enqueue(delegate
            {
                lock (sendlock)
                {
                    int startsize = SendBuffer.size();

                    try
                    {
                        uint cmdLength = cmdDataLength + 6;
                        //写入包头占位4字节
                        SendBuffer.marshal_uint(cmdLength); 
                        //写入ID
                        SendBuffer.marshal_ushort(cmdid); 
                        //写入长度
                        SendBuffer.marshal_bytes(cmdData, (int)cmdDataLength);

                        // uint timestamp = SingletonForMono<GameTime>.Instance.GetIntervalMsecond();
                        // SendBuffer.marshal_uint(timestamp); //写入时间戳
                        // SendBuffer.marshal_proto(t);

                        // int bodylenth = SendBuffer.size() - startsize - NullCmd.HEAD_SIZE;
                        int bodylenth = (int)cmdLength;

                        //todo 压缩
                        if (NullCmd.IS_COMPRESS)
                        {
                            if ((SendBuffer.size() - startsize - NullCmd.HEAD_SIZE) > 32)
                            {
                                MessageCompress.Compress(startsize + 4, bodylenth, ref SendBuffer);
                                SendBuffer.write_byte(startsize + 3, 64);                   //写入压缩标志
                                bodylenth = (ushort)(SendBuffer.size() - startsize - NullCmd.HEAD_SIZE);
                            }
                        }
                        SendBuffer.write_ushort(startsize, (ushort)bodylenth);  //写入包长信息

                        lastcmdlength = bodylenth;
                    }
                    catch (Exception e)
                    {
                        Debug.LogWarning( "Send Exception:" + e.Message);
                        SendBuffer.erase(startsize, SendBuffer.size());
                    }
                }
            });
        }

        public void Send(StructCmd cmd, bool istoself)
        {
            ushort cmdid = 0;

            if (type == USocketType.Fir)     //大小消息号    写入消息号
            {
                cmdid = GetMsgId(cmd.MsgCmd, cmd.MsgParam);
            }
            else
            {
                cmdid = cmd.Msgid;
            }

            if (istoself)
            {
                if (NetWorkModule.Instance.StructCallbackDict.ContainsKey(cmdid))
                {
                    NetWorkModule.Instance.StructCallbackDict[cmdid](cmd);
                }

                return;
            }

#if NetWorkForge
            if (LSingleton<NetWorkForger>.Instance.OnMessage(cmdid, cmd) != 0) return;
#endif


            if (IsSendBlock(cmdid))
            {
                return;
            }

            if (cmdid == (ushort)CommandID.stIphoneLoginUserCmd_CS)
                sendLoginCmdOver = false;

            sendActions.Enqueue(delegate
            {
                lock (sendlock)
                {
                    int startsize = SendBuffer.size();

                    try
                    {
                        //写入包头占位4字节
                        SendBuffer.marshal_uint(0);                 

                        if (type == USocketType.Fir)        
                        {
                            //大小消息号 写入消息号
                            SendBuffer.marshal_byte(cmd.MsgCmd);
                            SendBuffer.marshal_byte(cmd.MsgParam);
                        }
                        else
                        {
                            SendBuffer.marshal_ushort((ushort)cmd.Msgid);
                        }

                        uint timestamp = (uint)GameTime.Instance.GetCurrServerTime();

                        //     GameTime.Instance.
                        //    GetIntervalMsecond();

                        // 写入时间戳
                        SendBuffer.marshal_uint(timestamp);
                        //写入包体
                        SendBuffer.marshal_struct(cmd);             

                        ushort bodylenth = (ushort)(SendBuffer.size() - startsize - NullCmd.HEAD_SIZE);

                        //todo 压缩
                        if (NullCmd.IS_COMPRESS)
                        {
                            if ((SendBuffer.size() - startsize - NullCmd.HEAD_SIZE) > 32)
                            {
                                MessageCompress.Compress(startsize + 4, bodylenth, ref SendBuffer);
                                SendBuffer.write_byte(startsize + 3, 64);                   //写入压缩标志

                                bodylenth = (ushort)(SendBuffer.size() - startsize - NullCmd.HEAD_SIZE);
                            }
                        }
                        SendBuffer.write_ushort(startsize, (ushort)bodylenth);  //写入包长信息

                        lastcmdlength = bodylenth;
                    }
                    catch (Exception e)
                    {
                        SendBuffer.erase(startsize, SendBuffer.size());
                    }
                }
            });
        }
        #endregion


        public static void PrintByteArr(byte[] byteArr, int len)
        {
            string str = "";
            for (int i = 0; i < len; i++)
            {
                str += byteArr[i] + "==";
            }
            Debug.Log(str);
        }

        private bool sleepSwitch = false;

        private int encryptremainsize = 0;          //接收缓存中未解密的

        private void OnReceive()
        {
            if (blockingState == BlockingState.Close) return;
            string strLog1 = "OnReceive thread " + Thread.CurrentThread.GetHashCode();

            while (!sleepSwitch)
            {
                try
                {
                    Thread.Sleep(SOCKET_THREAD_SLEEP_INTERVAL);

                    if (socket.Connected && socket.Available > 0)
                    {
                        int count = socket.Receive(receiveBuffer);
                        if (NullCmd.IS_ENCRYPT)
                        {
                            int encryptstartsize = ReceiveBuffer.size() - encryptremainsize;
                            ReceiveBuffer.insert(ReceiveBuffer.size(), receiveBuffer, 0, count);
                            encryptremainsize = (encryptremainsize + count) % 8;

                            if (ReceiveBuffer.size() - encryptstartsize > 8)
                            {
                                lock (MessageEncrypt.DESEncryptHandle)
                                {
                                    MessageEncrypt.DESEncryptHandle.encdec_des(ReceiveBuffer.buffer(), encryptstartsize,
                                        ReceiveBuffer.size() - encryptremainsize - encryptstartsize, false); //DES解密
                                }
                            }
                        }
                        else
                        {
                            encryptremainsize = 0;
                            ReceiveBuffer.insert(ReceiveBuffer.size(), receiveBuffer, 0, count);
                        }
                        //ThreadManager.Instance.RunOnMainThread(() =>
                        //{
                        ProcessReceiveBuffer();
                        //});
                    }

                    ProcessSend();
                }
                catch (Exception e)
                {
                    PrintExceptionError(e);
                    break;
                }
            }
        }


        private CmdHead cmdHeadInfo = null;

        void ProcessReceiveBuffer()
        {
            bool iscontinue = false;
            do
            {
                iscontinue = false;
                if ((ReceiveBuffer.size() - encryptremainsize) >= NullCmd.HEAD_SIZE)
                {

                    if (cmdHeadInfo == null)
                    {
                        try
                        {
                            cmdHeadInfo = NullCmd.unmarshal_head(ref ReceiveBuffer);
                        }
                        catch (Exception e)
                        {
                            this.PrintExceptionError(e);
                        }
                    }
                }

                if (cmdHeadInfo == null)
                {
                    return;
                }


                if ((ReceiveBuffer.size() - encryptremainsize) >= (cmdHeadInfo.MsgLength))
                {
                    lock (receivelock)
                    {
                        try
                        {
                            DecompressBuffer.clear();
                            DecompressBuffer.position(0);
                            DecompressBuffer.insert(0, ReceiveBuffer, 6, (int)cmdHeadInfo.MsgLength - 6);
                            ReceiveBuffer.erase(0, (int)cmdHeadInfo.MsgLength);
                            ReceiveBuffer.position(0);
                        }
                        catch (Exception e)
                        {
                            this.PrintExceptionError(e);
                        }
                    }

                    if (cmdHeadInfo.IsCompress)
                    {
                        MessageCompress.DeCompress(0, ref DecompressBuffer);
                    }

                    //byte[] byteData = DecompressBuffer.getBytes();
                    //SC_TopLayer cmd = SC_TopLayer.Parser.ParseFrom(byteData);

                    //lock (receivelock)
                    //{
                    //    if (NetWorkModule.Instance.ProtoMsgParseCallbackDict.ContainsKey(cmd.MsgName))
                    //    {
                    //        NullCmd vcmd = NetWorkModule.Instance.ProtoMsgParseCallbackDict[cmd.MsgName](cmd.Data);
                    //        receivedQueue.Enqueue(vcmd);
                    //    }
                    //    else
                    //    {
                    //        //NullCmdForLua lcmd = ManagerCenter.Instance.GetManager<LuaNetWorkManager>().ConstructLuaCmd(cmdHeadInfo.CmdID, byteData);
                    //        //receivedQueue.Enqueue(lcmd);
                    //    }
                    //}

                    DecompressBuffer.clear();
                    DecompressBuffer.position(0);
                    cmdHeadInfo = null;
                    iscontinue = true;
                }

            } while (iscontinue);
        }


        #region receiveByte

        const int maxSendSize = 60 * 1024;
        void ProcessSend()
        {
            if (blockingState == BlockingState.Close) return;

            while (sendActions.Count > 0)
            {
                sendActions.Dequeue()();

                bool bBigMessage = SendBuffer.size() > maxSendSize;

                if ((SendBuffer.size() > 0 && sendActions.Count == 0)
                    || bBigMessage    // 超过大小直接发送
                    )
                {
                    if (send())
                    {
                        //reconnecttimes = 0;

                        if (bBigMessage)
                            return;

                        if (sendLoginCmdOver == false)
                        {
                            sendLoginCmdOver = true;
                            MessageEncrypt.ReSetDESKey();
                        }
                    }
                    else
                    {
                        if (blockingState != BlockingState.Close)
                        {
                            NeedReconnect = true;
                        }
                    }
                }
            }
        }
        #endregion



        string MsgLog = "msg-->";
        public string ClearAndGetMsgLog()
        {
            string ret = MsgLog;
            MsgLog = "msg-->";
            return ret;
        }


        public static ushort GetMsgId(byte cmdId, byte cmdParam)
        {
            return (ushort)(cmdId * 10000 + cmdParam);
        }


        public void CloseSocket()
        {
            if (socket != null)
            {
                socket.Close();
                socket = null;
            }
        }

        /// <summary>
        /// 重置Socket，以便重连
        /// </summary>
        public void ResetSocket()
        {
            try
            {
                receivedQueue.Clear();
                sendActions.Clear();
                SendBuffer.clear();
                SendBuffer.position(0);
                ReceiveBuffer.clear();
                ReceiveBuffer.position(0);
                encryptremainsize = 0;
                lastcmdlength = 0;
                blockingResponseMsgID = 0;
                lastBlockingRequestMsgID = 0;
                DecompressBuffer.clear();
                sleepSwitch = true;
                whetherCacheMsg = false;
                NeedReconnect = false;
                cmdHeadInfo = null;
                if (MsgThread != null && MsgThread.IsAlive)
                {
                    if (MsgThread.ThreadState == ThreadState.WaitSleepJoin)
                    {
                        MsgThread.Interrupt();
                    }

                    MsgThread.Join();
                }

                if (socket != null)
                {
                    if (socket.Connected)
                    {
                        socket.Shutdown(SocketShutdown.Both);
                        socket.Disconnect(true);
                    }

                    sleepSwitch = true;
                }
            }
            catch (Exception e)
            {
                string strLog = "连接断开异常 Thread ：" + Thread.CurrentThread.GetHashCode() + "   " + e.ToString();
                Debug.LogError(strLog);
            }
        }

        public void Dispose()
        {
            sleepSwitch = true;
            ResetSocket();
            onClose = null;
            onError = null;
            onConnect = null;
            onConnectFail = null;
            receiveBuffer = null;
            NeedReconnect = false;
            ReceiveCmdQueue.Clear();
            blockingState = BlockingState.Close;
        }


        /// <summary>
        /// 阻塞消息延迟
        /// </summary>
        private void BlockingDelay()
        {
            long delayTick = 0;
            if (lastSendTick != 0)
            {
                delayTick = DateTime.Now.Ticks - lastSendTick;
                if (delayTick > 70000000)
                {
                    blockingState = BlockingState.Exception;
                    lastSendTick = 0;
                }
                else if (delayTick > 20000000)
                {
                    blockingState = BlockingState.TimeOut;
                }
            }
        }

        /// <summary>
        /// 接收消息队列管理
        /// </summary>
        public void ReceiveBufferManager()
        {
            //ProcessReceiveBuffer();

            if (whetherCacheMsg)
            {
                return;
            }

            if (NeedReconnect)
            {
                NeedReconnect = false;

                //NetWorkModule.Instance.Reconnect();
                //PC断网或者手机切到后台10s后再切回前台 服务器真正掉线 这时候网关已经断了连不上，直接连接平台 重新登录
                Debug.LogError("ReceiveBufferManager->AutoReconnect");
                NetWorkModule.Instance.AutoReconnect();
            }

            while (receivedQueue.Count > 0)
            {
                if (whetherCacheMsg)
                {
                    break;
                }
                lock (receivelock)
                {
                    NullCmdBase cmdBase = receivedQueue.Dequeue();

                    if (cmdBase is NullCmd)
                    {
                        NullCmd vcmd = cmdBase as NullCmd;
                        if (NetWorkModule.Instance.ProtoMsgCallbackDict.ContainsKey(vcmd.MsgName))
                        {
                            NetWorkModule.Instance.ProtoMsgCallbackDict[vcmd.MsgName](vcmd.Data);
                        }
                    }
                    else if (cmdBase is NullCmdForLua)
                    {
                        NullCmdForLua lcmd = cmdBase as NullCmdForLua;
                       // LuaNetWorkManager.Instance.OnMessage(lcmd.Msgid, lcmd.BufferData);
                    }
                }
            }

            BlockingDelay();
        }

        /// <summary>
        /// 获得Socket状态
        /// </summary>
        /// <returns></returns>
        public bool GetSocketState()
        {
            if (socket == null)
            {
                return false;
            }

            if (socket.Connected)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 开始缓存
        /// </summary>
        public void BeginCacheMsg()
        {
            whetherCacheMsg = true;
        }

        /// <summary>
        /// 结束缓存
        /// </summary>
        public void FinishCacheMsg()
        {
            whetherCacheMsg = false;
        }


        public void PrintExceptionError(Exception e)
        {
            Debug.LogError("【USocke线程报错】:" + "<color=#ff0000ff> <size=15>" + e.GetType().ToString() + " 原因:" + e.Message + "</size></color>" + "\n" + e.ToString());
        }
    }


    /// <summary>
    /// 阻塞状态
    /// </summary>
    public enum BlockingState
    {
        Sendable,       //可发送
        Waiting,        //等待中
        TimeOut,        //超过最小等待时间
        Exception,      //异常
        Close,
    }

    public enum USocketType
    {
        Fir,
        GateWay,
    }
}
