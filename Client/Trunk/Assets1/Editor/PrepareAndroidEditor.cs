#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.IO;
using Cherry.AssetBundlePacker;

public class PrepareAndroidEditor : EditorWindow
{
    [MenuItem("Tools/1.(重要)生成Lua配置逻辑文件", false, 1)]
    public static void CreateLuaAllList()
    {
        string path = Application.dataPath + "/../../Download/Android/Resources";
        AutoCreateUIScriptEditor.CreateLuaAllList(path);
        UnityEngine.Debug.Log("生成Lua配置逻辑文件完成");
    }

    [MenuItem("Tools/2.(重要)统一编码CS和Lua文件用UTF-8", false, 2)]
    public static void EncodeFileToUTF8Encoding()
    {
        CreateVersionManifestEditor.EncodeFileToUTF8Encoding();
        UnityEngine.Debug.Log("编码所有程序文件用UTF-8完成");
    }



    [MenuItem("Tools/Android/4.删除其他平台相关的", false, 4)]
    public static void DeleteDir()
    {
        string path = Application.dataPath + "/Plugins/iOS";
        if (Directory.Exists(path)) { Directory.Delete(path, true); }

        path = Application.dataPath + "/StreamingAssets/iOS";
        if (Directory.Exists(path)) { Directory.Delete(path, true); }

        path = Application.dataPath + "/Plugins/x86";
        if (Directory.Exists(path)) { Directory.Delete(path, true); }

        path = Application.dataPath + "/Plugins/x86_64";
        if (Directory.Exists(path)) { Directory.Delete(path, true); }
        
        AssetDatabase.Refresh();
    }


    public static void OtherDeleteAndCopySdk()
    {
        string to = Application.dataPath + "/Plugins/Android/";
        if (Directory.Exists(to)) { Directory.Delete(to, true); }
        Directory.CreateDirectory(to);

        string from = Application.dataPath + "/../AndroidSDK/Other/Android/";

        CreateVersionManifestEditor.CopyPackFilesTo(from, to);

        AssetDatabase.Refresh();
        UnityEngine.Debug.Log("准备安装包版本完成");
    }


    public static void MainDeleteAndCopySdk()
    {
        string to = Application.dataPath + "/Plugins/Android/";
        if (Directory.Exists(to)) { Directory.Delete(to, true); }
        Directory.CreateDirectory(to);

        string from = Application.dataPath + "/../AndroidSDK/Main/Android/";

        CreateVersionManifestEditor.CopyPackFilesTo(from, to);

        AssetDatabase.Refresh();
        UnityEngine.Debug.Log("准备安装包版本完成");
    }


   
    public static void CopyMusicFile()
    {
        string luaPath = Application.dataPath + "/../../Download/Android/Resources/Audio/";
        if (Directory.Exists(luaPath)) { Directory.Delete(luaPath, true); }
        Directory.CreateDirectory(luaPath);

        string luaDataPath = Application.dataPath + "/StreamingAssets/Audio/GeneratedSoundBanks/Android/";
        CreateVersionManifestEditor.CopyLuaFilesTo("拷贝音乐音效文件到指定的目录", luaDataPath, luaPath, false);

        UnityEngine.Debug.Log("拷贝音乐音效文件到资源目录完成");
    }


    public static void CreateVersionManifestFile(EBuildType buildType)
    {

        string downLoadPath = EditorCommon.GetDownloadPath(buildType);
        CreateVersionManifestEditor.CreateVersionManifestFile(downLoadPath,
            "versionManifest.txt",
            new string[] { ".meta", ".DS_Store", ".zip" },
            null,
            CreateVersionManifestEditor.IgnoreType.ONLY_USE_IGNORE);

        UnityEngine.Debug.Log("生成服务器资源清单完成");
    }

    public static void CreateAndroidZip(EBuildType buildType)
    {
        string path, zipedName;
        string downLoadPath = EditorCommon.GetDownloadPath(buildType);

        for (int i = 0, count = CreateVersionManifestEditor.m_copysAndroid.Length; i < count; ++i)
        {
            EditorUtility.DisplayProgressBar("压缩目录", CreateVersionManifestEditor.m_copysAndroid[i], 1);
            path = downLoadPath + CreateVersionManifestEditor.m_copysAndroid[i];
            zipedName = downLoadPath + CreateVersionManifestEditor.m_copysAndroid[i] + ".zip";
            if (File.Exists(zipedName)) { File.Delete(zipedName); }
            GyZipHelper.ZipDerctory(path, zipedName, CreateVersionManifestEditor.ZIP_PASSWORD);
            EditorUtility.ClearProgressBar();
        }

        UnityEngine.Debug.Log("压缩指定目录生成zip完成");
    }

    public static void CreateAndroid7z(EBuildType buildType)
    {
        string path;
        string downLoadPath = EditorCommon.GetDownloadPath(buildType);
        for (int i = 0, count = CreateVersionManifestEditor.m_copysAndroid.Length; i < count; ++i)
        {
            EditorUtility.DisplayProgressBar("压缩目录", CreateVersionManifestEditor.m_copysAndroid[i], 1);
            path = downLoadPath + CreateVersionManifestEditor.m_copysAndroid[i];
            Gy7zHelper.ZipDerctory7z(path);
            EditorUtility.ClearProgressBar();
        }

        UnityEngine.Debug.Log("压缩指定目录生成7z完成");
    }

    public static void DeCreateAndroid7z(EBuildType buildType)
    {
        string path;
        string downLoadPath = EditorCommon.GetDownloadPath(buildType);
        for (int i = 0, count = CreateVersionManifestEditor.m_copysAndroid.Length; i < count; ++i)
        {
            EditorUtility.DisplayProgressBar("解压目录", CreateVersionManifestEditor.m_copysAndroid[i], 1);
            path = downLoadPath + CreateVersionManifestEditor.m_copysAndroid[i];
            Gy7zHelper.UnZipDerctory7z("", path);
            EditorUtility.ClearProgressBar();
        }

        UnityEngine.Debug.Log("解压指定目录生成7z完成");
    }

    /// <summary>
    /// 删除压缩文件的源目录
    /// </summary>
    public static void DeleteAndroidZipSourceDirectory(EBuildType buildType)
    {
        string downLoadPath = EditorCommon.GetDownloadPath(buildType);
        string path;
        for (int i = 0, count = CreateVersionManifestEditor.m_copysAndroid.Length; i < count; ++i)
        {
            EditorUtility.DisplayProgressBar("删除压缩源目录", CreateVersionManifestEditor.m_copysAndroid[i], 1);
            path = downLoadPath + CreateVersionManifestEditor.m_copysAndroid[i];
            if (Directory.Exists(path)) { Directory.Delete(path, true); }
            EditorUtility.ClearProgressBar();
        }
    }

    

    //[MenuItem("Tools/Android/6.准备安装包版本", false, 9)]
    public static void CopyToStreamingAssets(EBuildType buildType)
    {
        string to = Application.streamingAssetsPath + "/Android/";
        if (Directory.Exists(to)) { Directory.Delete(to, true); }
        Directory.CreateDirectory(to);

        string from = EditorCommon.GetDownloadPath(buildType);
        CreateVersionManifestEditor.CopyPackFilesTo(from, to);

        string path;
        for (int i = 0, count = CreateVersionManifestEditor.m_copysAndroid.Length; i < count; ++i)
        {
            EditorUtility.DisplayProgressBar("压缩目录", CreateVersionManifestEditor.m_copysAndroid[i], 1);
            path = to + "Resources/" + CreateVersionManifestEditor.m_copysAndroid[i];
            if (Directory.Exists(path)) { Directory.Delete(path, true); }
            EditorUtility.ClearProgressBar();
        }

        path = to + "Resources/Audio";
        if (Directory.Exists(path)) { Directory.Delete(path, true); }

        AssetDatabase.Refresh();

        UnityEngine.Debug.Log("准备安装包版本完成");
    }

   
    
}

#endif