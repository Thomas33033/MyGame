/*using System;

namespace Net
{
    using System;
    using System.Text;
    using ICSharpCode.SharpZipLib.Zip;
    using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
    using System.IO;
    using System.Security.Cryptography;
    using UnityEngine;
    using ProtoBuf;

    /// <summary>
    /// 1-4 消息头(压缩，加密，长度); 5-6 消息号;7-10 时间戳
    /// </summary>
    public class UMessage : IstorebAble
    {
		public const int HEAD_SIZE = 4;
        public const int CONTENT_INDEX = 6;
        public byte[] Buffer = null;
        
        private int index;
        
        public int Length;
        public int BodyLength;
        public ushort MsgId;
        public byte MsgCmd;
        public byte MsgParam;

        public bool IsPack;

        public UMessage()
        {
            Reset();
        }

        #region IstorebAble
        public bool IsDirty
        {
            get;
            set;
        }

        public void ResetThisObject()
        {
            Reset();
        }

        public void StoreToPool()
        {
            ClassPool.Store<UMessage>(this, 50);
        }
        #endregion


        #region MessageFunction

        public void ReadLength()
        {
            BodyLength = (int)(Buffer[0] + (Buffer[1] << 8));
            Length = BodyLength + HEAD_SIZE;
            index = 0;
            IsPack = true;
        }

        public void ReadHead()
        {
            MsgId = ReadUInt16(Buffer, HEAD_SIZE);
            MsgCmd = Buffer[HEAD_SIZE];
            MsgParam = Buffer[HEAD_SIZE + 1];
            index += 2;
            ReadTimeStamp();
            //时间戳
            index += 4;
        }

        public void WriteHead(byte cmd, byte para)
        {
            MsgCmd = cmd;
            MsgParam = para;
            Buffer[HEAD_SIZE] = MsgCmd;
            Buffer[HEAD_SIZE + 1] = MsgParam;
            WriteTimeStamp(0);
            index += 6;
            updateLengthHandle();
        }

        public void WriteHead(ushort msgId)
        {
            MsgId = (ushort)msgId;
            WriteUInt16(MsgId);
            WriteTimeStamp(0);
            index += 4;
            updateLengthHandle();
        }

        public void Reset()
        {
            index = 0;
            Length = 0;
            BodyLength = 0;

            if (Buffer == null)
            {
                Buffer = new byte[USocket.BUFFER_LENGTH];
            }
            else
            {
                for (int i = 0; i < Buffer.Length; i++)
                {
                    Buffer[i] = 0;
                }
            }
            

            IsPack = false;
        }

        public void ResetContent()
        {
            index = CONTENT_INDEX;
        }

        public void Clear()
        {
            index = 0;
            Length = 0;
            BodyLength = 0;
            Buffer = null;
        }

        public UMessage copyNew()
        {
            UMessage newobj = new UMessage();
            newobj.index = this.index;
            newobj.Length = this.Length;
            newobj.BodyLength = this.BodyLength;
            newobj.MsgId = this.MsgId;
            newobj.IsPack = this.IsPack;
            Array.Copy(this.Buffer, newobj.Buffer, Length);
            return newobj;
        }

        public void ResetIndex()
        {
            index = 0;
        }

        //更新字节数组长度
        private void updateLengthHandle()
        {
            BodyLength = index;
            Length = HEAD_SIZE + index;
            WriteUInt16(Buffer, 0, (UInt16)BodyLength);
        }

        public UInt32 ReadTimeStamp()
        {
            return 0;
        }

        // 写时间戳
        public void WriteTimeStamp(UInt32 b)
        {
            Buffer[HEAD_SIZE + 2] = (byte)(b << 24 >> 24);
            Buffer[HEAD_SIZE + 3] = (byte)(b << 16 >> 24);
            Buffer[HEAD_SIZE + 4] = (byte)(b << 8 >> 24);
            Buffer[HEAD_SIZE + 5] = (byte)(b >> 24);
        }

        #endregion

        #region ReadFunction

        public byte ReadByte()
        {
            byte result = Buffer[HEAD_SIZE + index];
            index++;
            return result;
        }

        public bool ReadBool()
        {
            byte result = Buffer[HEAD_SIZE + index];
            index++;
            if (result == 0)
                return false;
            return true;
        }
        public UInt16 ReadUInt16()
        {
            UInt16 result = ReadUInt16(Buffer, HEAD_SIZE + index);
            index += 2;
            return result;
        }
        public UInt32 ReadUInt32()
        {
            UInt32 result = (UInt32)(Buffer[HEAD_SIZE + index] + (Buffer[HEAD_SIZE + index + 1] << 8) + (Buffer[HEAD_SIZE + index + 2] << 16) + (Buffer[HEAD_SIZE + index + 3] << 24));
            index += 4;
            return result;
        }

        public UInt64 ReadUInt64()
        {
            UInt32 low = ReadUInt32();
            UInt32 high = ReadUInt32();
            return low + ((UInt64)(high) << 32);
        }

        public static UInt16 ReadUInt16(byte[] buffer, int index)
        {
            return (UInt16)(buffer[index] + (buffer[index + 1] << 8));
        }

        public int ReadInt32()
        {
            int result = (int)(Buffer[HEAD_SIZE + index] + (Buffer[HEAD_SIZE + index + 1] << 8) + (Buffer[HEAD_SIZE + index + 2] << 16) + (Buffer[HEAD_SIZE + index + 3] << 24));
            index += 4;
            return result;
        }

        public Int16 ReadInt16()
        {
            Int16 result = (Int16)(Buffer[HEAD_SIZE + index] + (Buffer[HEAD_SIZE + index + 1] << 8));
            index += 2;
            return result;
        }

        public float ReadFloat()
        {
            float result = BitConverter.ToSingle(Buffer, HEAD_SIZE + index);
            index += 4;
            return result;
        }

        public string ReadString()
        {
            int length = (int)ReadUInt16(Buffer, HEAD_SIZE + index);
            string result = Encoding.UTF8.GetString(Buffer, HEAD_SIZE + index + 2, length);
            index += length + 2;
            return result;
        }

        public string ReadString(int length)
        {
            int strlen = 0;
            for (int i = 0; i < length; i++)
            {
                byte tmp = Buffer[HEAD_SIZE + index + i];

                if (tmp == 0)
                {
                    break;
                }

                strlen += 1;
            }

            string result = Encoding.UTF8.GetString(Buffer, HEAD_SIZE + index, strlen);
            index += length;
            return result;
        }

        public byte[] ReadBytes()
        {
            int length = (int)ReadUInt16(Buffer, HEAD_SIZE + index);
            byte[] ret = new byte[length];
            for (int i = 0; i < ret.Length; i++)
            {
                ret[i] = Buffer[HEAD_SIZE + index + 2 + i];
            }
            index += length + 2;
            return ret;
        }

        public Byte[] ReadBytes(int length)
        {
            Byte[] ret = new Byte[length];
            for (int i = 0; i < ret.Length; i++)
            {
                ret[i] = Buffer[HEAD_SIZE + index + i];
            }
            index += length;
            return ret;
        }

        public T ReadProto<T>()
        {
#if false
			T t;
			using(MemoryStream ms = new MemoryStream(ReadBytes()))
			{
				t = (T)protoSerializer.Deserialize(ms, null, typeof(T));
			}
			return t;
#else
			T t;
			using(MemoryStream ms = new MemoryStream(ReadBytes()))
			{
				t = Serializer.Deserialize<T>(ms);
			}
			return t;
#endif
        }

        #endregion

        #region WriteFunction
        public void WriteByte(byte b)
        {
            Buffer[HEAD_SIZE + index] = b;
            index += 1;
            updateLengthHandle();
        }

        public void WriteUInt16(UInt16 b)
        {
            WriteUInt16(Buffer, HEAD_SIZE + index, b);
            index += 2;
            updateLengthHandle();
        }

        public void WriteUInt32(UInt32 b)
        {
            Buffer[HEAD_SIZE + index] = (byte)(b << 24 >> 24);
            index++;
            Buffer[HEAD_SIZE + index] = (byte)(b << 16 >> 24);
            index++;
            Buffer[HEAD_SIZE + index] = (byte)(b << 8 >> 24);
            index++;
            Buffer[HEAD_SIZE + index] = (byte)(b >> 24);
            index++;
            updateLengthHandle();
        }

        public void WriteUInt64(UInt64 b)
        {
            Buffer[HEAD_SIZE + index] = (byte)(b << 56 >> 56);
            index++;
            Buffer[HEAD_SIZE + index] = (byte)(b << 48 >> 56);
            index++;
            Buffer[HEAD_SIZE + index] = (byte)(b << 40 >> 56);
            index++;
            Buffer[HEAD_SIZE + index] = (byte)(b << 32 >> 56);
            index++;
            Buffer[HEAD_SIZE + index] = (byte)(b << 24 >> 56);
            index++;
            Buffer[HEAD_SIZE + index] = (byte)(b << 16 >> 56);
            index++;
            Buffer[HEAD_SIZE + index] = (byte)(b << 8 >> 56);
            index++;
            Buffer[HEAD_SIZE + index] = (byte)(b >> 56);
            index++;
            updateLengthHandle();
        }

        public void WriteInt32(int b)
        {
            Buffer[HEAD_SIZE + index] = (byte)(b << 24 >> 24);
            index++;
            Buffer[HEAD_SIZE + index] = (byte)(b << 16 >> 24);
            index++;
            Buffer[HEAD_SIZE + index] = (byte)(b << 8 >> 24);
            index++;
            Buffer[HEAD_SIZE + index] = (byte)(b >> 24);
            index++;
            updateLengthHandle();
        }

        public void WriteUInt16(byte[] buffer, int index, UInt16 b)
        {
            buffer[index] = (byte)(b << 24 >> 24);
            buffer[index + 1] = (byte)(b << 16 >> 24);
        }

        public void WriteByte(byte[] buffer, int index, byte b)
        {
            buffer[index] = b;
        }

        public void WriteString(string s)
        {
            byte[] tmp = Encoding.UTF8.GetBytes(s);
            WriteUInt16(Buffer, HEAD_SIZE + index, (UInt16)tmp.Length);
            for (int i = 0; i < tmp.Length; i++)
            {
                Buffer[HEAD_SIZE + index + 2 + i] = tmp[i];
            }
            index += tmp.Length + 2;
            updateLengthHandle();
        }

        public void WriteString(string s, int nSize)
        {
            byte[] tmp = Encoding.UTF8.GetBytes(s);
            for (int i = 0; i < tmp.Length; i++)
            {
                Buffer[HEAD_SIZE + index + i] = tmp[i];
            }

            for (int j = tmp.Length; j < nSize; j++)
            {
                Buffer[HEAD_SIZE + index + j] = 0;
            }

            index += nSize;
            updateLengthHandle();
        }

        public void WriteBytes(byte[] bytes)
        {
			WriteUInt16(Buffer, HEAD_SIZE + index, (UInt16)bytes.Length);
            for (int i = 0; i < bytes.Length; i++)
            {
                Buffer[HEAD_SIZE + index + 2 + i] = bytes[i];
            }
            index += bytes.Length + 2;
            updateLengthHandle();

			//USocket.PrintByteArr (Buffer, Length);
        }
        public void WriteFloat(float f)
        {
            byte[] r = BitConverter.GetBytes(f);
            Buffer[HEAD_SIZE + index] = r[0];
            index++;
            Buffer[HEAD_SIZE + index] = r[1];
            index++;
            Buffer[HEAD_SIZE + index] = r[2];
            index++;
            Buffer[HEAD_SIZE + index] = r[3];
            index++;
            updateLengthHandle();
        }

        public void WriteProto<T>(T t)
        {
#if false
			using (MemoryStream ms = new MemoryStream())
            {
				protoSerializer.Serialize(ms, t);
                WriteBytes(ms.ToArray());
            }
#else
			using (MemoryStream ms = new MemoryStream())
			{
                Serializer.Serialize<T>(ms, t);
				WriteBytes(ms.ToArray());
			}
#endif
        }
        #endregion


        #region RC5
        private static string recKey = "2123544143372513";
        private static uint[] rC5KeyData = null, rc5L = new uint[13], rc5R = new uint[13];
        private static byte[] rC5output = new byte[16], rC5Input = new byte[8];
        private static uint rc5A = 0, rc5B = 0;
        private static int rc5m = 0, rc5Index = 0, rc5Tmp = 0;
        private static void generateSubKey()
        {
            if (rC5KeyData != null) return;
            rC5KeyData = new uint[26];    //t=2r+2,r=12,t=26
            uint p32 = 0xB7E15163;  //Odd((e-2)*2^w),w=32
            uint q32 = 0x9E3779B9;  //Odd((Φ-1)*2^w),w=32
            int i, j;

            rC5KeyData[0] = p32;
            for (i = 1; i < 26; i++)
            {
                rC5KeyData[i] = rC5KeyData[i - 1] + q32;
            }
            Encoding ascii = Encoding.ASCII;
            Encoding unicode = Encoding.Unicode;
            byte[] asciiBytes = Encoding.Convert(unicode, ascii, unicode.GetBytes(recKey));
            char[] asciiChars = new char[ascii.GetCharCount(asciiBytes, 0, asciiBytes.Length)];
            ascii.GetChars(asciiBytes, 0, asciiBytes.Length, asciiChars, 0);
            uint[] L = new uint[4];    //c=b/u,u=w/8,w=32,b=16,c=4
            for (i = 0; i < 16; i++)
            {
                L[i / 4] += (uint)(asciiChars[i] & 0xFF) << (8 * (i % 4));
            }

            int k = 78; //3*MAX(t.c),t=26,c=4
            uint A = 0;
            uint B = 0;
            for (i = 0, j = 0; k > 0; k--)
            {
                A = rotL(rC5KeyData[i] + A + B, 3, 32);
                rC5KeyData[i] = A;
                B = rotL(L[j] + A + B, A + B, 32);
                L[j] = B;
                i = (i + 1) % 26;   //mod(t),t=26
                j = (j + 1) % 4;    //mod(c),c=4
            }
        }
        private byte[] rC5Decrypt(byte[] input)
        {
            for (rc5Tmp = 0; rc5Tmp < 13; rc5Tmp++)
            {
                rc5L[rc5Tmp] = rc5R[rc5Tmp] = 0;
            }
            for (rc5Tmp = 0; rc5Tmp < 4; rc5Tmp++)
            {
                rc5L[12] += (uint)(input[rc5Tmp] & 0xFF) << (8 * rc5Tmp);
                rc5R[12] += (uint)(input[rc5Tmp + 4] & 0xFF) << (8 * rc5Tmp);
            }
            for (rc5Tmp = 12; rc5Tmp > 0; rc5Tmp--)
            {
                rc5R[rc5Tmp - 1] = (rotR((rc5R[rc5Tmp] - rC5KeyData[2 * rc5Tmp + 1]), rc5L[rc5Tmp], 32) ^ rc5L[rc5Tmp]);
                rc5L[rc5Tmp - 1] = (rotR((rc5L[rc5Tmp] - rC5KeyData[2 * rc5Tmp]), rc5R[rc5Tmp - 1], 32) ^ rc5R[rc5Tmp - 1]);
            }
            uint rc5A = 0;
            uint rc5B = 0;
            rc5A = rc5L[0] - rC5KeyData[0];
            rc5B = rc5R[0] - rC5KeyData[1];
            for (rc5Tmp = 0; rc5Tmp < 4; rc5Tmp++)
            {
                rC5output[rc5Tmp] = (byte)((rc5A >> (8 * rc5Tmp)) & 0xFF);
            }
            for (rc5Tmp = 0; rc5Tmp < 4; rc5Tmp++)
            {
                rC5output[rc5Tmp + 4] = (byte)((rc5B >> (8 * rc5Tmp)) & 0xFF);
            }
            return rC5output;
        }
        private byte[] rC5Encrypt(byte[] input)
        {
            for (rc5Tmp = 0; rc5Tmp < 13; rc5Tmp++)
            {
                rc5L[rc5Tmp] = rc5R[rc5Tmp] = 0;
            }
            rc5A = 0;
            rc5B = 0;
            for (rc5Tmp = 0; rc5Tmp < 4; rc5Tmp++)
            {
                rc5A += (uint)(input[rc5Tmp] & 0xFF) << (8 * rc5Tmp);
                rc5B += (uint)(input[rc5Tmp + 4] & 0xFF) << (8 * rc5Tmp);
            }
            rc5L[0] = (rc5A + rC5KeyData[0]);
            rc5R[0] = (rc5B + rC5KeyData[1]);
            for (rc5Tmp = 1; rc5Tmp <= 12; rc5Tmp++)
            {
                rc5L[rc5Tmp] = (rotL((rc5L[rc5Tmp - 1] ^ rc5R[rc5Tmp - 1]), rc5R[rc5Tmp - 1], 32) + rC5KeyData[2 * rc5Tmp]);
                rc5R[rc5Tmp] = (rotL((rc5R[rc5Tmp - 1] ^ rc5L[rc5Tmp]), rc5L[rc5Tmp], 32) + rC5KeyData[2 * rc5Tmp + 1]);
            }
            for (rc5Tmp = 0; rc5Tmp < 4; rc5Tmp++)
            {
                rC5output[rc5Tmp] = (byte)((rc5L[12] >> (8 * rc5Tmp)) & 0xFF);
            }
            for (rc5Tmp = 0; rc5Tmp < 4; rc5Tmp++)
            {
                rC5output[rc5Tmp + 4] = (byte)((rc5R[12] >> (8 * rc5Tmp)) & 0xFF);
            }
            return rC5output;
        }
        public static uint rotL(uint x, uint y, int w)
        {
            return ((x << (int)(y & 0xFF)) | (x >> (int)((w - (y & 0xFF)))));
        }
        public static uint rotR(uint x, uint y, int w)
        {
            return ((x >> (int)(y & 0xFF)) | (x << (int)((w - (y & 0xFF)))));
        }
        public void RC5Encrypt()
        {
            generateSubKey();
            rc5m = (Length - HEAD_SIZE) % 8;
            if (rc5m != 0) Length += 8 - rc5m;
            rc5Tmp = 0;
            for (rc5Index = HEAD_SIZE; rc5Index < Length; rc5Index++)
            {
                rC5Input[rc5Tmp] = Buffer[rc5Index];
                rc5Tmp++;
                if (rc5Tmp == 8)
                {
                    rC5Encrypt(rC5Input);
                    rc5Index = rc5Index - 7;
                    for (rc5Tmp = 0; rc5Tmp < 8; rc5Tmp++)
                    {
                        Buffer[rc5Index] = rC5output[rc5Tmp];
                        rc5Index++;
                    }
                    rc5Index--;
                    rc5Tmp = 0;
                }
            }
        }
        public void RC5Decrypt()
        {
            generateSubKey();
            rc5m = (Length - HEAD_SIZE) % 8;
            if (rc5m != 0) Length += 8 - rc5m;
            rc5Tmp = 0;
            for (rc5Index = HEAD_SIZE; rc5Index < Length; rc5Index++)
            {
                rC5Input[rc5Tmp] = Buffer[rc5Index];
                rc5Tmp++;
                if (rc5Tmp == 8)
                {
                    rC5Decrypt(rC5Input);
                    rc5Index = rc5Index - 7;
                    for (rc5Tmp = 0; rc5Tmp < 8; rc5Tmp++)
                    {
                        Buffer[rc5Index] = rC5output[rc5Tmp];
                        rc5Index++;
                    }
                    rc5Index--;
                    rc5Tmp = 0;
                }
            }
        }
        #endregion

        public static UInt16 GetMsgId(byte cmdId, byte cmdParam)
        {
            return (UInt16)(cmdId * 10000 + cmdParam);
        }
    }
}
*/