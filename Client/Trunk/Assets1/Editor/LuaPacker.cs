using UnityEditor;
using UnityEngine;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

public class LuaPacker
{
    public static string platform = string.Empty;
    
    /// <summary>
    /// 生成luaAssetBundle
    /// </summary>
    public static void BuildLuaAssetBundle()
    {
        string outPath = Utils.GetAssetBundleStreamingAssetsPath("lua");
        if (Directory.Exists(outPath))
            Directory.Delete(outPath, true);

        Directory.CreateDirectory(outPath);

        AssetDatabase.Refresh();

        List<AssetBundleBuild> builds = new List<AssetBundleBuild>();
        HandleLuaBundle(ref builds);

        Directory.CreateDirectory(outPath);
        BuildPipeline.BuildAssetBundles(outPath, builds.ToArray(), BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);

        string streamDir = Application.dataPath + "/" + LuaConst.LuaTempDir;
        if (Directory.Exists(streamDir)) Directory.Delete(streamDir, true);
        AssetDatabase.Refresh();
    }

    static void AddBuildMap(ref List<AssetBundleBuild> builds, string bundleName, string pattern, string path)
    {
        string[] files = Directory.GetFiles(path, pattern, SearchOption.AllDirectories);
        if (files.Length == 0) return;

        for (int i = 0; i < files.Length; i++)
        {
            files[i] = files[i].Replace('\\', '/').Replace(Application.dataPath, "Assets");
        }
        AssetBundleBuild build = new AssetBundleBuild();
        build.assetBundleName = bundleName;
        build.assetNames = files;
        builds.Add(build);
    }

    /// <summary>
    /// 处理Lua代码包,
    /// </summary>
    static void HandleLuaBundle(ref List<AssetBundleBuild> builds)
    {
        
        string streamDir = Application.dataPath + "/" + LuaConst.LuaTempDir;
        if (!Directory.Exists(streamDir)) Directory.CreateDirectory(streamDir);

        string[][] srcDirs = {
            new string[] { LuaConst.luaDir, "lua" },
            new string[] { LuaConst.toluaDir, "tolua" }
        };

        for (int i = 0; i < srcDirs.Length; i++)
        {
            string sourceDir = srcDirs[i][0];
            string bundleName = srcDirs[i][1];
            if (LuaConst.LuaByteMode)
            {
                string[] files = Directory.GetFiles(sourceDir, "*.lua", SearchOption.AllDirectories);
                int len = sourceDir.Length;

                if (sourceDir[len - 1] == '/' || sourceDir[len - 1] == '\\')
                {
                    --len;
                }
                for (int j = 0; j < files.Length; j++)
                {
                    string str = files[j].Remove(0, len);
                    string dest = streamDir + bundleName + "/" + str + ".bytes";
                    string dir = Path.GetDirectoryName(dest);
                    Directory.CreateDirectory(dir);
                    EncodeLuaFile(files[j], dest);
                }
            }
            else
            {
                ToLuaMenu.CopyLuaBytesFiles(sourceDir, streamDir + bundleName);
            }

            ToLuaMenu.CopyLuaBytesFiles(sourceDir, streamDir + bundleName, true, "*.pb");

            AddBuildMap(ref builds, bundleName, "*.bytes", streamDir + bundleName);
        }

        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 处理Lua文件
    /// </summary>
    static void HandleLuaFile()
    {
        string resPath = AppDataPath + "/StreamingAssets/";
        string luaPath = resPath + "/lua/";

        //----------复制Lua文件----------------
        if (!Directory.Exists(luaPath))
        {
            Directory.CreateDirectory(luaPath);
        }
        string[] luaPaths = { AppDataPath + "/LuaFramework/lua/",
                              AppDataPath + "/LuaFramework/Tolua/Lua/" };

        for (int i = 0; i < luaPaths.Length; i++)
        {

            List<string> files  = new List<string>();

            string luaDataPath = luaPaths[i].ToLower();
            Recursive(ref files,luaDataPath);
            int n = 0;
            foreach (string f in files)
            {
                if (f.EndsWith(".meta")) continue;
                string newfile = f.Replace(luaDataPath, "");
                string newpath = luaPath + newfile;
                string path = Path.GetDirectoryName(newpath);
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);

                if (File.Exists(newpath))
                {
                    File.Delete(newpath);
                }
                if (LuaConst.LuaByteMode)
                {
                    EncodeLuaFile(f, newpath);
                }
                else
                {
                    File.Copy(f, newpath, true);
                }
                UpdateProgress(n++, files.Count, newpath);
            }
        }
        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 数据目录
    /// </summary>
    static string AppDataPath
    {
        get { return Application.dataPath.ToLower(); }
    }

    /// <summary>
    /// 遍历目录及其子目录
    /// </summary>
    static void Recursive(ref List<string> files, string path)
    {
        string[] names = Directory.GetFiles(path);
        string[] dirs = Directory.GetDirectories(path);
        foreach (string filename in names)
        {
            string ext = Path.GetExtension(filename);
            if (ext.Equals(".meta")) continue;
            files.Add(filename.Replace('\\', '/'));
        }
        foreach (string dir in dirs)
        {
            //paths.Add(dir.Replace('\\', '/'));
            Recursive(ref files, dir);
        }
    }

    /// <summary>
    /// 更新lua处理进度
    /// </summary>
    /// <param name="progress"></param>
    /// <param name="progressMax"></param>
    /// <param name="desc"></param>
    static void UpdateProgress(int progress, int progressMax, string desc)
    {
        string title = "Processing...[" + progress + " - " + progressMax + "]";
        float value = (float)progress / (float)progressMax;
        EditorUtility.DisplayProgressBar(title, desc, value);
    }

    /// <summary>
    /// 加密lua代码
    /// </summary>
    /// <param name="srcFile"></param>
    /// <param name="outFile"></param>
    public static void EncodeLuaFile(string srcFile, string outFile)
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
        string desStr = GyEncrypt.EncryptDES1(srcStr, CreateVersionManifestEditor.ZIP_PASSWORD, keys);
        File.WriteAllText(outFile, desStr);
    }
}