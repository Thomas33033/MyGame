using UnityEngine;
using System.Collections;
using ICSharpCode.SharpZipLib.Zip;
using System.IO;


public static class GyZipHelper
{
    public const int COMPRESSION_LEVEL = 9;//压缩率：0-9;

    public static string C_ZIP_PASSWORD = "";

    ///////////////////////////////压缩;///////////////////////////////

    /// <summary>
    /// 压缩文件;
    /// </summary>
    /// <param name="fileToZip">要压缩的文件路径;</param>
    /// <param name="zipedFile">压缩后的文件路径;</param>
    public static void ZipFile(string fileToZip, string zipedFile, string password)
    {
        if (!File.Exists(fileToZip))
        {
            throw new FileNotFoundException("The specified file " + fileToZip + " could not be found.");
        }

        using (ZipOutputStream zipStream = new ZipOutputStream(File.Create(zipedFile)))
        {
            string fileName = Path.GetFileName(fileToZip);
            ZipEntry zipEntry = new ZipEntry(fileName);
            zipStream.PutNextEntry(zipEntry);
            zipStream.SetLevel(COMPRESSION_LEVEL);
            zipStream.Password = password;
            using (FileStream streamToZip = new FileStream(fileToZip, FileMode.Open, FileAccess.Read))
            {
                byte[] buffer = new byte[2048];
                int size = streamToZip.Read(buffer, 0, buffer.Length);
                zipStream.Write(buffer, 0, size);
                while (size < streamToZip.Length)
                {
                    int sizeRead = streamToZip.Read(buffer, 0, buffer.Length);
                    zipStream.Write(buffer, 0, sizeRead);
                    size += sizeRead;
                }
                streamToZip.Dispose();

                buffer = null;
            }

            zipStream.Dispose();
        }
    }

    /// <summary>
    /// 得到目录下的所有文件;
    /// </summary>
    /// <param name="directory"></param>
    /// <returns></returns>
    public static ArrayList GetFileList(string directory)
    {
        ArrayList fileList = new ArrayList();
        bool isEmpty = true;
        foreach (string file in Directory.GetFiles(directory))
        {
            fileList.Add(file);
            isEmpty = false;
        }
        if (isEmpty)
        {
            if (Directory.GetDirectories(directory).Length == 0)
            {
                fileList.Add(string.Format("{0}/", directory));
            }
        }
        foreach (string dirs in Directory.GetDirectories(directory))
        {
            foreach (object obj in GetFileList(dirs))
            {
                fileList.Add(obj);
            }
        }
        return fileList;
    }

    /// <summary>
    /// 压缩文件夹;
    /// </summary>
    /// <param name="directoryToZip">要压缩的文件夹路径;</param>
    /// <param name="zipedDirectory">压缩后的文件夹路径;</param>
    public static void ZipDerctory(string directoryToZip, string zipedDirectory, string password)
    {
        using (ZipOutputStream zipStream = new ZipOutputStream(File.Create(zipedDirectory)))
        {
            ArrayList fileList = GetFileList(directoryToZip);
            int directoryNameLength = (Directory.GetParent(directoryToZip)).ToString().Length;
            zipStream.SetLevel(COMPRESSION_LEVEL);
            zipStream.Password = password;
            ZipEntry zipEntry = null;
            FileStream fileStream = null;
            byte[] buffer = new byte[2048];

            string tempFileName = "";
            foreach (string fileName in fileList)
            {
                tempFileName = fileName.Replace("\\", "/");
                zipEntry = new ZipEntry(tempFileName.Remove(0, directoryNameLength));
                zipStream.PutNextEntry(zipEntry);
                if (!tempFileName.EndsWith(@"/"))
                {
                    fileStream = File.OpenRead(tempFileName);
                    while (true)
                    {
                        int size = fileStream.Read(buffer, 0, buffer.Length);
                        if (size > 0)
                        {
                            zipStream.Write(buffer, 0, size);
                        }
                        else
                        {
                            break;
                        }
                    }

                    fileStream.Dispose();
                }
            }

            buffer = null;
        }
    }

    ///////////////////////////////解压缩;///////////////////////////////

    /// <summary>
    /// 解压缩文件;
    /// </summary>
    /// <param name="zipFilePath">压缩文件路径;</param>
    /// <param name="unZipFilePath">解压缩文件路径;</param>
    public static void UnZipFile(string zipFilePath, string unZipFilePath, string password)
    {
        using (ZipInputStream zipStream = new ZipInputStream(File.OpenRead(zipFilePath)))
        {
            zipStream.Password = password;
            ZipEntry zipEntry = null;
            byte[] buffer = new byte[2048];
            while ((zipEntry = zipStream.GetNextEntry()) != null)
            {
                string fileName = Path.GetFileName(zipEntry.Name);
                if (!string.IsNullOrEmpty(fileName))
                {
                    if (zipEntry.CompressedSize == 0) { break; }
                    using (FileStream stream = File.Create(string.Format("{0}/{1}", unZipFilePath, fileName)))
                    {
                        while (true)
                        {
                            int size = zipStream.Read(buffer, 0, buffer.Length);
                            if (size > 0)
                            {
                                stream.Write(buffer, 0, size);
                            }
                            else
                            {
                                break;
                            }
                        }

                        stream.Dispose();
                    }
                }
            }

            zipStream.Dispose();
            buffer = null;
        }
    }

    /// <summary>
    /// 解压缩目录;
    /// </summary>
    /// <param name="zipDirectoryPath">压缩目录路径;</param>
    /// <param name="unZipDirecotyPath">解压缩目录路径;</param>
    public static void UnZipDirectory(string zipDirectoryPath, string unZipDirecotyPath, string password)
    {
        using (ZipInputStream zipStream = new ZipInputStream(File.OpenRead(zipDirectoryPath)))
        {
            zipStream.Password = password;
            ZipEntry zipEntry = null;
            byte[] buffer = new byte[2048];

            string tempZipEntryName = "";
            while ((zipEntry = zipStream.GetNextEntry()) != null)
            {
                tempZipEntryName = zipEntry.Name;
                tempZipEntryName = tempZipEntryName.Replace("\\", "/");
                string directoryName = Path.GetDirectoryName(tempZipEntryName);
                string fileName = Path.GetFileName(tempZipEntryName);
                if (!string.IsNullOrEmpty(directoryName))
                {
                    directoryName = string.Format("{0}/{1}", unZipDirecotyPath, directoryName);
                    directoryName = directoryName.Replace("\\", "/");
                    if (!Directory.Exists(directoryName))
                    {
                        Directory.CreateDirectory(directoryName);
                    }
                }
                if (!string.IsNullOrEmpty(fileName))
                {
                    if (zipEntry.CompressedSize == 0) { break; }
                    if (zipEntry.IsDirectory)
                    {
                        directoryName = Path.GetDirectoryName(string.Format("{0}/{1}", unZipDirecotyPath, tempZipEntryName));
                        directoryName = directoryName.Replace("\\", "/");
                        if (!Directory.Exists(directoryName))
                        {
                            Directory.CreateDirectory(directoryName);
                        }
                    }
                    using (FileStream stream = File.Create(string.Format("{0}/{1}", unZipDirecotyPath, tempZipEntryName)))
                    {
                        while (true)
                        {
                            int size = zipStream.Read(buffer, 0, buffer.Length);
                            if (size > 0)
                            {
                                stream.Write(buffer, 0, size);
                            }
                            else
                            {
                                break;
                            }
                        }

                        stream.Dispose();
                    }
                }
            }

            zipStream.Dispose();
            buffer = null;
        }
    }
}

