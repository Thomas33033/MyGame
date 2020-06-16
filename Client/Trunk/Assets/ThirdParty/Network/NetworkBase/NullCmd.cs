using System;
using System.IO;
using UnityEngine;
using System.Collections;

namespace Net
{
    public class NullCmdBase
    {

    }

    public class NullCmd : NullCmdBase
    {
        public const int HEAD_SIZE = 4;
        public const bool IS_COMPRESS = false;       //压缩开关
        public const bool IS_ENCRYPT = false;        //加密开关

        public string MsgName = "";
        public ushort Msgid = 0;

        public byte MsgCmd = 0;     //大小消息号
        public byte MsgParam = 0;

        public object Data = null;

        public NullCmd()
        {

        }

        public NullCmd(CommandID id)
        {
            Msgid = (ushort)id;
        }

        public NullCmd(byte cmd, byte param)
        {
            MsgCmd = cmd;
            MsgParam = param;
        }




        public static CmdHead unmarshal_head(ref OctetsStream os)
        {
            CmdHead head = new CmdHead();
            int startsize = os.position();

            head.MsgLength = os.unmarshal_uint();
            head.CmdID = os.unmarshal_ushort();
            head.IsCompress = false;

            // head.MsgLength = (int)(os.unmarshal_byte() + ((os.unmarshal_byte() << 8)));

            // uint result = (uint)(os.getByte(startsize) + (os.getByte(startsize + 1) << 8) + (os.getByte(startsize + 2) << 16) +
            // +(os.getByte(startsize + 3) << 24));

            // os.unmarshal_ushort();  //向后移2位

            // uint PACKET_ZIP = 0x40000000;				/**< 数据包压缩标志 */

            // if (PACKET_ZIP == (PACKET_ZIP & result))
            // {
            //     head.IsCompress = true;
            // }
            // else
            // {
            //     head.IsCompress = false;
            // }

            return head;
        }
    }


    public class CmdHead
    {
        public uint MsgLength = 0;
        public ushort CmdID = 0;
        public bool IsCompress = false;
    }


    public class NullCmdForLua : NullCmdBase
    {
        public ushort Msgid = 0;
        public byte[] BufferData = null;
    }
}
