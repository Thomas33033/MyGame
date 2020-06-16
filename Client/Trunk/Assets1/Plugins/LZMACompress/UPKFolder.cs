using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using UnityEngine;
using YZL.Compress.Info;

namespace YZL.Compress.UPK
{
    public class UPKFolder
    {
        public class CodeProgress
        {
            public ProgressDelegate m_ProgressDelegate = null;

            public CodeProgress(ProgressDelegate del)
            {
                this.m_ProgressDelegate = del;
            }

            public void SetProgress(long inSize, long outSize)
            {
            }

            public void SetProgressPercent(long fileSize, long processSize)
            {
                this.m_ProgressDelegate.Invoke(fileSize, processSize);
            }
        }

        private class OneFileInfor
        {
            public int id = 0;

            public int startPos = 0;

            public int size = 0;

            public int pathLength = 0;

            public string path = "";

            public byte[] data = null;
        }

        public static void PackFolderAsync(string inpath, string outpath, ProgressDelegate progress)
        {
            Thread thread = new Thread(new ParameterizedThreadStart(UPKFolder.PackFolder));
            thread.Start(new FileChangeInfo
            {
                inpath = inpath,
                outpath = outpath,
                progressDelegate = progress
            });
        }

        public static void UnPackFolderAsync(string inpath, string outpath, ProgressDelegate progress)
        {
            Thread thread = new Thread(new ParameterizedThreadStart(UPKFolder.UnPackFolder));
            thread.Start(new FileChangeInfo
            {
                inpath = inpath,
                outpath = outpath,
                progressDelegate = progress
            });
        }

        private static void PackFolder(object obj)
        {
            FileChangeInfo fileChangeInfo = (FileChangeInfo)obj;
            string inpath = fileChangeInfo.inpath;
            string outpath = fileChangeInfo.outpath;
            UPKFolder.CodeProgress codeProgress = null;
            if (fileChangeInfo.progressDelegate != null)
            {
                codeProgress = new UPKFolder.CodeProgress(fileChangeInfo.progressDelegate);
            }
            int num = 0;
            int num2 = 0;
            Dictionary<int, UPKFolder.OneFileInfor> dictionary = new Dictionary<int, UPKFolder.OneFileInfor>();
            //Debug.Log("遍历文件夹 " + inpath);
            string str = inpath.Substring(0, inpath.LastIndexOf('/'));
            DirectoryInfo directoryInfo = new DirectoryInfo(inpath);
            FileInfo[] files = directoryInfo.GetFiles("*.*", SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; i++)
            {
                FileInfo fileInfo = files[i];
                if (!(fileInfo.Extension == ".meta"))
                {
                    string text = fileInfo.FullName.Replace("\\", "/");
                    text = text.Replace(str + "/", "");
                    int num3 = (int)fileInfo.Length;
                    //Debug.Log(string.Concat(new object[]
                    //{
                    //    num,
                    //    " : ",
                    //    text,
                    //    " 文件大小: ",
                    //    num3
                    //}));
                    UPKFolder.OneFileInfor oneFileInfor = new UPKFolder.OneFileInfor();
                    oneFileInfor.id = num;
                    oneFileInfor.size = num3;
                    oneFileInfor.path = text;
                    oneFileInfor.pathLength = new UTF8Encoding().GetBytes(text).Length;
                    FileStream fileStream = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read);
                    if (fileStream == null)
                    {
                        Debug.Log("读取文件失败 ： " + fileInfo.FullName);
                        return;
                    }
                    byte[] array = new byte[num3];
                    fileStream.Read(array, 0, num3);
                    oneFileInfor.data = array;
                    fileStream.Close();
                    dictionary.Add(num, oneFileInfor);
                    num++;
                    num2 += num3;
                }
            }
            //Debug.Log("文件数量 : " + num);
            //Debug.Log("文件总大小 : " + num2);
            int num4 = 4;
            for (int j = 0; j < dictionary.Count; j++)
            {
                num4 += 16 + dictionary[j].pathLength;
            }
            for (int j = 0; j < dictionary.Count; j++)
            {
                int startPos;
                if (j == 0)
                {
                    startPos = num4;
                }
                else
                {
                    startPos = dictionary[j - 1].startPos + dictionary[j - 1].size;
                }
                dictionary[j].startPos = startPos;
            }
            FileStream fileStream2 = new FileStream(outpath, FileMode.Create);
            byte[] bytes = BitConverter.GetBytes(num);
            fileStream2.Write(bytes, 0, bytes.Length);
            for (int j = 0; j < dictionary.Count; j++)
            {
                byte[] bytes2 = BitConverter.GetBytes(dictionary[j].id);
                fileStream2.Write(bytes2, 0, bytes2.Length);
                byte[] bytes3 = BitConverter.GetBytes(dictionary[j].startPos);
                fileStream2.Write(bytes3, 0, bytes3.Length);
                byte[] bytes4 = BitConverter.GetBytes(dictionary[j].size);
                fileStream2.Write(bytes4, 0, bytes4.Length);
                byte[] bytes5 = BitConverter.GetBytes(dictionary[j].pathLength);
                fileStream2.Write(bytes5, 0, bytes5.Length);
                byte[] bytes6 = new UTF8Encoding().GetBytes(dictionary[j].path);
                fileStream2.Write(bytes6, 0, bytes6.Length);
            }
            int num5 = 0;
            foreach (KeyValuePair<int, UPKFolder.OneFileInfor> current in dictionary)
            {
                UPKFolder.OneFileInfor oneFileInfor = current.Value;
                int size = oneFileInfor.size;
                int k = 0;
                while (k < size)
                {
                    byte[] array2;
                    if (size - k < 1024)
                    {
                        array2 = new byte[size - k];
                    }
                    else
                    {
                        array2 = new byte[1024];
                    }
                    fileStream2.Write(oneFileInfor.data, k, array2.Length);
                    k += array2.Length;
                    num5 += array2.Length;
                    if (codeProgress != null)
                    {
                        codeProgress.SetProgressPercent((long)num2, (long)num5);
                    }
                }
            }
            fileStream2.Flush();
            fileStream2.Close();
            //Debug.Log("打包完成");
        }

        public static void PackFolder(string inpath, string outpath, ProgressDelegate progress)
        {
            UPKFolder.PackFolder(new FileChangeInfo
            {
                inpath = inpath,
                outpath = outpath,
                progressDelegate = progress
            });
        }

        private static void UnPackFolder(object obj)
        {
            FileChangeInfo fileChangeInfo = (FileChangeInfo)obj;
            string inpath = fileChangeInfo.inpath;
            string outpath = fileChangeInfo.outpath;
            UPKFolder.CodeProgress codeProgress = null;
            if (fileChangeInfo.progressDelegate != null)
            {
                codeProgress = new UPKFolder.CodeProgress(fileChangeInfo.progressDelegate);
            }
            Dictionary<int, UPKFolder.OneFileInfor> dictionary = new Dictionary<int, UPKFolder.OneFileInfor>();
            UTF8Encoding uTF8Encoding = new UTF8Encoding();
            int num = 0;
            FileStream fileStream = new FileStream(inpath, FileMode.Open);
            fileStream.Seek(0L, SeekOrigin.Begin);
            int num2 = 0;
            byte[] array = new byte[4];
            fileStream.Read(array, 0, 4);
            int num3 = BitConverter.ToInt32(array, 0);
            num2 += 4;
            //Debug.Log("filecount=" + num3);
            for (int i = 0; i < num3; i++)
            {
                byte[] array2 = new byte[4];
                fileStream.Seek((long)num2, SeekOrigin.Begin);
                fileStream.Read(array2, 0, 4);
                int num4 = BitConverter.ToInt32(array2, 0);
                num2 += 4;
                byte[] array3 = new byte[4];
                fileStream.Seek((long)num2, SeekOrigin.Begin);
                fileStream.Read(array3, 0, 4);
                int num5 = BitConverter.ToInt32(array3, 0);
                num2 += 4;
                byte[] array4 = new byte[4];
                fileStream.Seek((long)num2, SeekOrigin.Begin);
                fileStream.Read(array4, 0, 4);
                int num6 = BitConverter.ToInt32(array4, 0);
                num2 += 4;
                byte[] array5 = new byte[4];
                fileStream.Seek((long)num2, SeekOrigin.Begin);
                fileStream.Read(array5, 0, 4);
                int num7 = BitConverter.ToInt32(array5, 0);
                num2 += 4;
                byte[] array6 = new byte[num7];
                fileStream.Seek((long)num2, SeekOrigin.Begin);
                fileStream.Read(array6, 0, num7);
                string text = uTF8Encoding.GetString(array6);
                num2 += num7;
                dictionary.Add(num4, new UPKFolder.OneFileInfor
                {
                    id = num4,
                    size = num6,
                    pathLength = num7,
                    path = text,
                    startPos = num5
                });
                num += num6;
                //Debug.Log(string.Concat(new object[]
                //{
                //    "id=",
                //    num4,
                //    " startPos=",
                //    num5,
                //    " size=",
                //    num6,
                //    " pathLength=",
                //    num7,
                //    " path=",
                //    text
                //}));
            }
            int num8 = 0;
            foreach (KeyValuePair<int, UPKFolder.OneFileInfor> current in dictionary)
            {
                UPKFolder.OneFileInfor value = current.Value;
                int startPos = value.startPos;
                int num6 = value.size;
                string text = value.path;
                string path = outpath + text.Substring(0, text.LastIndexOf('/'));
                string path2 = outpath + text;


                string pathBegin = outpath.Substring(0, outpath.IndexOf("Resources"));
                int index = text.IndexOf("Resources");
                string pathEnd = text.Substring(text.IndexOf("Resources"), text.Length - index);

                text = pathBegin + pathEnd;

                path = text.Substring(0, text.LastIndexOf('/'));
                path2 = text;
                //Debug.Log("text:" + text + "\n outpath:" + outpath + "\n path:" + path + "\n path2:" + path2);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                if (File.Exists(path2))
                {
                    File.Delete(path2);
                }
                FileStream fileStream2 = new FileStream(path2, FileMode.Create);
                int j = 0;
                while (j < num6)
                {
                    byte[] array7;
                    if (num6 - j < 1024)
                    {
                        array7 = new byte[num6 - j];
                    }
                    else
                    {
                        array7 = new byte[1024];
                    }
                    fileStream.Seek((long)(startPos + j), SeekOrigin.Begin);
                    fileStream.Read(array7, 0, array7.Length);
                    fileStream2.Write(array7, 0, array7.Length);
                    j += array7.Length;
                    num8 += array7.Length;
                    if (codeProgress != null)
                    {
                        codeProgress.SetProgressPercent((long)num, (long)num8);
                    }
                }
                fileStream2.Flush();
                fileStream2.Close();
            }
            fileStream.Close();
            //Debug.Log("解包完成");
        }

        public static void UnPackFolder(string inpath, string outpath, ProgressDelegate progress)
        {
            UPKFolder.UnPackFolder(new FileChangeInfo
            {
                inpath = inpath,
                outpath = outpath,
                progressDelegate = progress
            });
        }

        public static void UnPackFolder_WithFileStream(string inpath, string outpath, ProgressDelegate progressDelegate, Stream fileStream)
        {
            UPKFolder.CodeProgress codeProgress = null;
            if (progressDelegate != null)
            {
                codeProgress = new UPKFolder.CodeProgress(progressDelegate);
            }
            Dictionary<int, UPKFolder.OneFileInfor> dictionary = new Dictionary<int, UPKFolder.OneFileInfor>();
            UTF8Encoding uTF8Encoding = new UTF8Encoding();
            int num = 0;
            fileStream.Seek(0L, SeekOrigin.Begin);
            int num2 = 0;
            byte[] array = new byte[4];
            fileStream.Read(array, 0, 4);
            int num3 = BitConverter.ToInt32(array, 0);
            num2 += 4;
           
            for (int i = 0; i < num3; i++)
            {
                byte[] array2 = new byte[4];
                fileStream.Seek((long)num2, SeekOrigin.Begin);
                fileStream.Read(array2, 0, 4);
                int num4 = BitConverter.ToInt32(array2, 0);
                num2 += 4;
                byte[] array3 = new byte[4];
                fileStream.Seek((long)num2, SeekOrigin.Begin);
                fileStream.Read(array3, 0, 4);
                int num5 = BitConverter.ToInt32(array3, 0);
                num2 += 4;
                byte[] array4 = new byte[4];
                fileStream.Seek((long)num2, SeekOrigin.Begin);
                fileStream.Read(array4, 0, 4);
                int num6 = BitConverter.ToInt32(array4, 0);
                num2 += 4;
                byte[] array5 = new byte[4];
                fileStream.Seek((long)num2, SeekOrigin.Begin);
                fileStream.Read(array5, 0, 4);
                int num7 = BitConverter.ToInt32(array5, 0);
                num2 += 4;
                byte[] array6 = new byte[num7];
                fileStream.Seek((long)num2, SeekOrigin.Begin);
                fileStream.Read(array6, 0, num7);
                string text = uTF8Encoding.GetString(array6);
                num2 += num7;
                dictionary.Add(num4, new UPKFolder.OneFileInfor
                {
                    id = num4,
                    size = num6,
                    pathLength = num7,
                    path = text,
                    startPos = num5
                });
                num += num6;
            }
            int num8 = 0;
            foreach (KeyValuePair<int, UPKFolder.OneFileInfor> current in dictionary)
            {
                UPKFolder.OneFileInfor value = current.Value;
                int startPos = value.startPos;
                int num6 = value.size;
                string text = value.path;
                //string path = outpath + text.Substring(0, text.LastIndexOf('/'));
                //string path2 = outpath + text;


                int index = text.IndexOf("Resources");
                string pathEnd = text.Substring(index, text.Length - index);

                text = outpath.Substring(0, outpath.IndexOf("Resources")) + pathEnd;

                string path = text.Substring(0, text.LastIndexOf('/'));
                string path2 = text;
                //Debug.Log("text:" + text + "\n outpath:" + outpath + "\n path:" + path + "\n path2:" + path2);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                if (File.Exists(path2))
                {
                    File.Delete(path2);
                }
                FileStream fileStream2 = new FileStream(path2, FileMode.Create);
                int j = 0;
                while (j < num6)
                {
                    byte[] array7;
                    if (num6 - j < 1024)
                    {
                        array7 = new byte[num6 - j];
                    }
                    else
                    {
                        array7 = new byte[1024];
                    }
                    fileStream.Seek((long)(startPos + j), SeekOrigin.Begin);
                    fileStream.Read(array7, 0, array7.Length);
                    fileStream2.Write(array7, 0, array7.Length);
                    j += array7.Length;
                    num8 += array7.Length;
                    if (codeProgress != null)
                    {
                        codeProgress.SetProgressPercent((long)num, (long)num8);
                    }
                }
                fileStream2.Flush();
                fileStream2.Close();
            }
            fileStream.Close();
            //Debug.Log("解包完成");
        }

    }
}
