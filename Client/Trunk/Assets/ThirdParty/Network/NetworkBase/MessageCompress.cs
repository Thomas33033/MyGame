using System;
using System.IO;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using Net;
using UnityEngine;
using System.Collections;

public class MessageCompress
{
    private static DataBuffer compressDatabuffer = new DataBuffer(USocket.BUFFER_LENGTH);
    private static DataBuffer decompressDatabuffer = new DataBuffer(USocket.BUFFER_LENGTH);
    
    public static void Compress(int offset, int length,ref OctetsStream os)
    {
        compressDatabuffer.Clear();
        using (MemoryStream ms = new MemoryStream(compressDatabuffer.Buffer))
        {
            using (DeflaterOutputStream zipStream = new DeflaterOutputStream(ms))
            {
                zipStream.Write(os.buffer(), offset, length);
                zipStream.Finish();
                compressDatabuffer.Length = (int)ms.Position;
                ms.Seek(0, SeekOrigin.Begin);
                os.erase(offset, os.size());
                os.insert(offset, ms.ToArray(), 0, compressDatabuffer.Length);
            }
        }
    }
    public static void DeCompress(int offset, ref OctetsStream os)
    {
        int read = -1;
        try
        {
            using (MemoryStream ms = new MemoryStream(os.buffer(), 0, os.size()))
            {
                ms.Seek(0, SeekOrigin.Begin);
                using (InflaterInputStream zipStream = new InflaterInputStream(ms))
                {
                    MemoryStream outStream = new MemoryStream();

                    read = zipStream.Read(decompressDatabuffer.Buffer, 0, decompressDatabuffer.Buffer.Length);
                    while (read > 0)
                    {
                        outStream.Write(decompressDatabuffer.Buffer, 0, read);
                        read = zipStream.Read(decompressDatabuffer.Buffer, 0, decompressDatabuffer.Buffer.Length);
                    }
                    zipStream.Close();

                    os.clear();
                    os.insert(offset, outStream.GetBuffer());
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(string.Format("消息解压出错"+ ex.Message));
        }
    }
    public static void DeCompress(ref OctetsBuffer os)
    {
        decompressDatabuffer.Clear();

        using (MemoryStream ms = new MemoryStream(os.buffer(), 0, os.size()))
        {
            ms.Seek(0, SeekOrigin.Begin);
            using (InflaterInputStream zipStream = new InflaterInputStream(ms))
            {
                decompressDatabuffer.Length = zipStream.Read(decompressDatabuffer.Buffer, 0, decompressDatabuffer.Buffer.Length);
                os.clear();
                os.insert(0, decompressDatabuffer.Buffer);
            }
        }
    }

    public static bool IsCompress(OctetsBuffer buffer)
    {
        if (buffer.size() < 4)
        {
            return false;
        }

        uint result = (uint)(buffer.getByte(0) + (buffer.getByte(1) << 8) + (buffer.getByte(2) << 16) + (buffer.getByte(3) << 24));

        uint PACKET_ZIP = 0x40000000;				/**< 数据包压缩标志 */

        if (PACKET_ZIP == (PACKET_ZIP & result))
        {
            return true;
        }
        return false;
    }
}

public class DataBuffer
{
    public DataBuffer(int size)
    {
        Buffer = new byte[size];
    }

    public void Clear()
    {
        Length = 0;
        for (int i = 0; i < Buffer.Length; i++)
        {
            Buffer[i] = 0;
        }
    }

    public byte[] Buffer;
    public int Length = 0;
}


