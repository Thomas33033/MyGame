using System.Net;
using System.Threading;
using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace Net
{
    public delegate NullCmd OnCmdParseCallback(OctetsStream os);
    public delegate NullCmd OnProtoParseCallback(Google.Protobuf.ByteString data);
    public delegate void OnCmdCallback(object obj);
    public delegate void ProtoMessageCallback<T>(T MsgData) where T : Google.Protobuf.IMessage;
    public delegate void StructMsgCallback<T>(T MsgData) where T : StructCmd;
    public class NetWorkModule : LSingleton<NetWorkModule>
    {
        public USocket MainSocket;

        public Dictionary<UInt16, OnCmdParseCallback> ProtoParseCallbackDict = new Dictionary<UInt16, OnCmdParseCallback>();
        public Dictionary<UInt16, OnCmdParseCallback> StructParseCallbackDict = new Dictionary<UInt16, OnCmdParseCallback>();

        public Dictionary<string, OnProtoParseCallback> ProtoMsgParseCallbackDict = new Dictionary<string, OnProtoParseCallback>();

        public Dictionary<UInt16, OnCmdCallback> ProtoCallbackDict = new Dictionary<UInt16, OnCmdCallback>();        //MsgID回调键值对
        public Dictionary<UInt16, OnCmdCallback> StructCallbackDict = new Dictionary<UInt16, OnCmdCallback>();        //MsgID回调键值对
        public Dictionary<string, OnCmdCallback> ProtoMsgCallbackDict = new Dictionary<string, OnCmdCallback>();
        public Dictionary<UInt16, UInt16> BlockingMsgPair = new Dictionary<UInt16, UInt16>();                   //阻塞消息键值对 request-response

        public Action ResetScene;

        public bool IsReconnecting = false;
        public bool IsNeedReconnect = true;

        /// <summary>
        /// 连接服务器
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="type"></param>
        /// <param name="onConnect"></param>
        /// <param name="onConnectFail"></param>
        public void Connect(IPEndPoint ipEndPoint, USocketType type, OnSocketCallback onConnect, OnSocketCallback onConnectFail)
        {
            if (MainSocket != null)
            {
                MainSocket.Dispose();
                MainSocket = null;
            }
            MainSocket = new USocket(type);
            MainSocket.ResetScene = ResetScene;
            MainSocket.Connect(ipEndPoint, onConnect, onConnectFail);
        }

        public void RegisterProtoMsg<T>(string msgName, Google.Protobuf.MessageParser parser, ProtoMessageCallback<T> callback) where T : Google.Protobuf.IMessage
        {
            ProtoMsgParseCallbackDict[msgName] = delegate (Google.Protobuf.ByteString data)
            {
                NullCmd vcmd = new NullCmd();
                vcmd.MsgName = msgName;
                vcmd.Data = parser.ParseFrom(data);
                return vcmd;
            };

            if (!ProtoMsgCallbackDict.ContainsKey(msgName))
            {
                ProtoMsgCallbackDict[msgName] = delegate (object obj)
                {
                    callback((T)obj);
                };
            };
        }

        public void RegisterStructMsg<T>(ushort cmid, StructMsgCallback<T> callback) where T : StructCmd, new()
        {
            StructParseCallbackDict[cmid] = delegate (OctetsStream os)
            {
                T t = new T();
                os.unmarshal_struct(t);
                NullCmd vcmd = new NullCmd();
                vcmd.Msgid = cmid;
                vcmd.Data = t;
                return vcmd;
            };

            if (!StructCallbackDict.ContainsKey(cmid))
            {
                StructCallbackDict[cmid] = delegate (object obj)
                {
                    callback((T)obj);
                };
            }
        }

        public bool ContainProtoMsg(ushort cmdid)
        {
            return ProtoCallbackDict.ContainsKey(cmdid);
        }

        public bool ContainStructMsg(ushort cmdid)
        {
            return StructCallbackDict.ContainsKey(cmdid);
        }

        /// <summary>
        /// 取消注册
        /// </summary>
        /// <param name="cmdId"></param>
        /// <param name="Callback"></param>
        public void DeRegisterMsg(UInt16 cmdId)
        {
            if (ProtoCallbackDict.ContainsKey(cmdId))
            {
                ProtoCallbackDict.Remove(cmdId);
            }
        }

        /// <summary>
        /// 注册阻塞消息 RequestID-ResponseID
        /// </summary>
        /// <param name="requestID"></param>
        /// <param name="responseID"></param>
        public void RegisterBlockingMsg(UInt16 requestID, UInt16 responseID)
        {
            if (!BlockingMsgPair.ContainsKey(requestID))
            {
                BlockingMsgPair[requestID] = responseID;
            }
        }


        public void Send(StructCmd cmd, bool isToSelf = false)
        {
            if (MainSocket != null)
            {
                MainSocket.Send(cmd, isToSelf);
            }
        }

        private byte[] m_protobuff = new byte[64 * 1024];
        public void Send<T>(string msgName, ushort cmdid, T t, bool istoself = false) where T : Google.Protobuf.IMessage
        {
            //if (MainSocket != null)
            //{
            //    CS_TopLayer cmd = new CS_TopLayer();
            //    cmd.MsgName = msgName;
            //    Google.Protobuf.CodedOutputStream stream = new Google.Protobuf.CodedOutputStream(this.m_protobuff);
            //    t.WriteTo(stream);
            //    cmd.Data = Google.Protobuf.ByteString.CopyFrom(this.m_protobuff, 0, (int)stream.Position);
            //    stream.Flush();
            //    cmd.WriteTo(stream);

            //    MainSocket.Send(cmdid, this.m_protobuff, (uint)stream.Position);
            //}
        }

        public void Send(ushort cmdid, byte[] data)
        {
            if (MainSocket != null)
            {
                MainSocket.Send(cmdid, data, (uint)data.Length);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void BeginCacheMsg()
        {
            MainSocket.BeginCacheMsg();
        }

        public void FinishCacheMsg()
        {
            MainSocket.FinishCacheMsg();
        }


        public void Reconnect()
        {
            if (MainSocket == null)
            {
                return;
            }

            if (MainSocket.type == USocketType.Fir)
            {
                //走重新登陆流程(隐式登录不显示登录界面 直接走登录消息流程) 连平台地址
                // ControllerManager.Instance.GetController<LoginController>().Login();
            }
            else if (MainSocket.type == USocketType.GateWay)
            {
                //请求连接网关
                MainSocket.ReConnectGateway();
            }
        }

        public void StartAutoReconnect()
        {
            IsNeedReconnect = true;
            AutoReconnect();
        }

        //这个是连平台
        public void AutoReconnect()
        {
            NetWorkModule.Instance.MainSocket.Dispose();
        }

        public void Update()
        {
            if (MainSocket != null)
            {
                MainSocket.ReceiveBufferManager();
            }
        }

        public void OnDestroy()
        {
            if (MainSocket == null)
            {
                return;
            }
            MainSocket.Dispose();
        }
        public void Close()
        {
            if (MainSocket != null)
            {
                MainSocket.Dispose();
            }

        }
    }
}

