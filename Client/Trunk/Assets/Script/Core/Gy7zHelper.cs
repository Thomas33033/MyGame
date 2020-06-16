using UnityEngine;
using System.Collections;
using System.IO;
using System;
using YZL.Compress.UPK;


public static class Gy7zHelper
{
    public static string STR_UPK = ".upk";
    public static string STR_ZIP = ".zip";

    /// <summary>
    /// 压缩文件夹;
    /// </summary>
    /// <param name="directoryToZip">要压缩的文件夹路径;</param>
    public static void UnZipDerctory7z(string zipFullFileName, string directoryToZip)
    {
        string upkName = directoryToZip + STR_UPK;
        string zipName = directoryToZip + STR_ZIP;

        if (Directory.Exists(directoryToZip))
        {
            Directory.Delete(directoryToZip, true);
        }

        //DecompressFileLZMA(zipName, upkName);
        //UPKFolder.UnPackFolder(upkName, directoryToZip + "/", (all, now) => { });
        //if (File.Exists(upkName))
        //{
        //    File.Delete(upkName);
        //}

        var fileStream = DecompressFileLZMA_SaveFileStream(zipName, upkName);

        UPKFolder.UnPackFolder_WithFileStream(upkName, directoryToZip + "/", null, fileStream);
    }

    /// <summary>
    /// 压缩文件夹;
    /// </summary>
    /// <param name="directoryToZip">要压缩的文件夹路径;</param>
    /// <param name="zipedDirectory">压缩后的文件夹路径;</param>
    public static void ZipDerctory7z(string directoryToZip)
    {
        string upkName = directoryToZip + STR_UPK;
        string zipName = directoryToZip + STR_ZIP;

        if (File.Exists(zipName))
        {
            File.Delete(zipName);
        }

        UPKFolder.PackFolder(directoryToZip, upkName, (all, now) => { });

        CompressFileLZMA(upkName, zipName);
        if (File.Exists(upkName))
        {
            File.Delete(upkName);
        }
    }

    private static void CompressFileLZMA(string inFile, string outFile)
    {
        SevenZip.Compression.LZMA.Encoder coder = new SevenZip.Compression.LZMA.Encoder();
        FileStream input = new FileStream(inFile, FileMode.Open);
        FileStream output = new FileStream(outFile, FileMode.Create);

        coder.WriteCoderProperties(output);
        output.Write(BitConverter.GetBytes(input.Length), 0, 8);
        coder.Code(input, output, input.Length, -1, null);
        output.Flush();
        output.Close();
        input.Close();
    }

    private static void DecompressFileLZMA(string inFile, string outFile)
    {
        SevenZip.Compression.LZMA.Decoder coder = new SevenZip.Compression.LZMA.Decoder();
        FileStream input = new FileStream(inFile, FileMode.Open);
        FileStream output = new FileStream(outFile, FileMode.Create);

        byte[] properties = new byte[5];
        input.Read(properties, 0, 5);

        byte[] fileLengthBytes = new byte[8];
        input.Read(fileLengthBytes, 0, 8);
        long fileLength = BitConverter.ToInt64(fileLengthBytes, 0);

        coder.SetDecoderProperties(properties);
        coder.Code(input, output, input.Length, fileLength, null);
        output.Flush();
        output.Close();
        input.Close();
    }

    private static Stream DecompressFileLZMA_SaveFileStream(string inFile, string outFile)
    {
        SevenZip.Compression.LZMA.Decoder coder = new SevenZip.Compression.LZMA.Decoder();
        FileStream input = new FileStream(inFile, FileMode.Open);
        //FileStream output = new FileStream(outFile, FileMode.Create);


        // Read the decoder properties
        byte[] properties = new byte[5];
        input.Read(properties, 0, 5);

        // Read in the decompress file size.
        byte[] fileLengthBytes = new byte[8];
        input.Read(fileLengthBytes, 0, 8);
        long fileLength = BitConverter.ToInt64(fileLengthBytes, 0);

        // Decompress the file.
        coder.SetDecoderProperties(properties);
        MemoryStream output = new MemoryStream((int)fileLength);
        coder.Code(input, output, input.Length, fileLength, null);
        //output.Flush();
        //output.Close();
        input.Close();
        return output;
    }
}

