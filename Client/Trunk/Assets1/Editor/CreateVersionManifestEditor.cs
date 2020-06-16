#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

public class CreateVersionManifestEditor : EditorWindow
{
    public static string[] m_copysAndroid = { "assetbundles/lua",
                                              "assetbundles/combine",
                                              "assetbundles/single/effects/heroskill",
                                              "assetbundles/single/shares",
                                              "assetbundles/single/windows"
                                            };

    public static string[] m_copysiOS = { "Lua",
                                          "Pbs",
                                        };

    public static string ZIP_PASSWORD = "7lSZMx@Z";

    /// <summary>
    /// 将文件编码格式修改为UTF-8.
    /// </summary>
    public static void EncodeFileToUTF8Encoding()
    {
        string path = Application.dataPath;
        string[] files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
        foreach (string f in files)
        {
            if (f.EndsWith(".cs") || f.EndsWith(".lua"))
            {
                string file = f.Replace('\\', '/');

                string content = File.ReadAllText(file);
                using (var sw = new StreamWriter(file, false, new UTF8Encoding(false)))
                {
                    sw.Write(content);
                }
            }
        }

        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 删除指定格式的文件    /// </summary>
    /// <param name="path"></param>
    /// <param name="endWith"></param>
    public static void DeleteManifestFiles(string path, string endWith)
    {
        string[] files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);

        for (int i = (files.Length - 1); i >= 0; --i)
        {
            if (files[i].EndsWith(endWith))
            {
                string file = files[i].Replace('\\', '/');

                if (File.Exists(file)) { File.Delete(file); }
            }
        }
    }

    /// <summary>
    /// 拷贝Lua文件到资源目录;
    /// </summary>
    /// <param name="luaPath"></param>
    public static void CopyLuaFilesTo(string desc, string from, string luaPath, bool isEncode)
    {
        EditorUtility.ClearProgressBar();
        List<string> fileList = Utils.RecursivePathGetFiles(from);
        int len = fileList.Count;
        if (len == 0) { len = 1; }
        int i = 0;
        foreach (string f in fileList)
        {
            if (f.EndsWith(".meta")) continue;
            string newfile = f.Replace(from, "");
            string newpath = luaPath + newfile;
            string path = Path.GetDirectoryName(newpath);
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            if (File.Exists(newpath)) { File.Delete(newpath); }
            EditorUtility.DisplayProgressBar(desc, newpath, i / (float)len);
            if (isEncode)
            {
                EncodeLuaFile(f, newpath);
            }
            else
            {
                File.Copy(f, newpath, true);
            }
            ++i;
        }
        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 拷贝文件到资源目录,准备安装包版本;
    /// </summary>
    /// <param name="luaPath"></param>
    public static void CopyPackFilesTo(string from, string luaPath)
    {
        EditorUtility.ClearProgressBar();
        List<string> fileList = Utils.RecursivePathGetFiles(from);
        int len = fileList.Count;
        if (len == 0) { len = 1; }
        int i = 0;
        foreach (string f in fileList)
        {
            if (f.EndsWith(".meta")) continue;
            string newfile = f.Replace(from, "");
            string newpath = luaPath + newfile;
            string path = Path.GetDirectoryName(newpath);

            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            if (File.Exists(newpath)) { File.Delete(newpath); }

            EditorUtility.DisplayProgressBar("准备安装包版本", newpath, i / (float)len);
            File.Copy(f, newpath, true);
            ++i;
        }
        EditorUtility.ClearProgressBar();
    }

    /// <summary>
    /// 拷贝Lua文件并加密
    /// </summary>
    /// <param name="srcFile"></param>
    /// <param name="outFile"></param>
    private static void EncodeLuaFile(string srcFile, string outFile)
    {
        if (!srcFile.ToLower().EndsWith(".lua"))
        {
            File.Copy(srcFile, outFile, true);
            return;
        }

        string srcStr = File.ReadAllText(srcFile);

        byte[] keys = new byte[8];
        string[] keyIndexesStrs = "39,20,55,89,243,98,146,233".Split(',');
        for (int i = 0; i < 8; ++i)
        {
            keys[i] = GyEncrypt.C_KEY_BANK[int.Parse(keyIndexesStrs[i])];
        }
        string desStr = GyEncrypt.EncryptDES1(srcStr, ZIP_PASSWORD, keys);

        File.WriteAllText(outFile, desStr);
    }

    public enum IgnoreType
    {
        ONLY_USE_IGNORE = 1,
        ONLY_USE_NEED = 2,
        USE_ALL = 3
    }

    /// <summary>
    /// 生成服务器资源清单;
    /// </summary>
    /// <param name="path">生成文件存放路径;</param>
    /// <param name="fileName">生成文件名;</param>
    /// <param name="ignores">忽略后缀名列表;</param>
    /// <param name="needs">必须后缀名列表;</param>
    /// <param name="ignoreType">1-只启用忽略列表，2-只启用必须列表，3-都启用;</param>
    public static void CreateVersionManifestFile(string path, string fileName, string[] ignores, string[] needs, IgnoreType ignoreType)
    {
        string newFilePath = path + "/" + fileName;

        if (File.Exists(newFilePath)) File.Delete(newFilePath);

        List<string> fileList = Utils.RecursivePathGetFiles(path);

        using (FileStream fs = new FileStream(newFilePath, FileMode.CreateNew))
        {
            StreamWriter sw = new StreamWriter(fs);

            for (int i = 0; i < fileList.Count; ++i)
            {
                string file = fileList[i];

                bool isWriteTo = false;

                if (ignoreType == IgnoreType.ONLY_USE_IGNORE)
                {
                    isWriteTo = !isEndsWith(file, ignores);
                }
                else if (ignoreType == IgnoreType.ONLY_USE_NEED)
                {
                    isWriteTo = isEndsWith(file, needs);
                }
                else if (ignoreType == IgnoreType.USE_ALL)
                {
                    isWriteTo = ((!isEndsWith(file, ignores)) && isEndsWith(file, needs));
                }

                if (isWriteTo)
                {
                    FileInfo fileInfo = new FileInfo(file);

                    long size = fileInfo.Length;

                    if (file.EndsWith(".meta") || file.Contains(".DS_Store")) continue;

                    string md5 = Utils.GenerateFileMd5(file);
                    string value = file.Replace(path + "/", string.Empty);
                    sw.WriteLine(value + "|" + size + "|" + md5 + "|" + DateTime.Now.ToString("yyyyMMddHHmmss"));
                }
            }

            sw.Close();
            fs.Close();
            AssetDatabase.Refresh();
        }
    }

    public static bool isEndsWith(string fileName, string[] ews)
    {
        for (int i = 0; i < ews.Length; ++i)
        {
            if (fileName.EndsWith(ews[i]))
            {
                return true;
            }
        }

        return false;
    }

    [MenuItem("Tools/清空所有本地数据")]
    public static void ClearLocalDatas()
    {
        Utils.ClearLocalDatas();
    }

    public static string[] GetBuildScenes()
    {
        string[] levels = { "Assets/Scenes/entry.unity", "Assets/Scenes/loading.unity" };
        return levels;
    }
}

#endif
