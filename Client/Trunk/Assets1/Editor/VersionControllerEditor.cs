#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System;
using Cherry.AssetBundlePacker;
using System.IO;

/// <summary>
/// 游戏版本管理
/// </summary>
public class VersionControllerEditor : EditorWindow
{
    private static VersionControllerEditor m_window;

    private static string m_currDownLoadUrl = string.Empty;
    private static string m_testDownLoadUrl = string.Empty;

    private static string m_currVersion = string.Empty;
    private static string m_newVersion = string.Empty;

    private static string m_path = string.Empty;

    [MenuItem("VersionMake/一键打包/1.发布Windows平台", false, 51)]
    public static void VersionMakeWindow()
    {
        VersionMake(EBuildType.StandaloneWindows);
    }

    [MenuItem("VersionMake/一键打包/2.发布Android平台", false, 51)]
    public static void VersionMakeAndroid()
    {
        VersionMake(EBuildType.Android);
    }

    [MenuItem("VersionMake/一键打包/1.发布IOS平台", false, 51)]
    public static void VersionMakeIOS()
    {
        VersionMake(EBuildType.IOS);
    }

    /// <summary>
    /// 一键打包
    /// </summary>
    /// <param name="buildType"></param>
    public static void VersionMake(EBuildType buildType)
    {
        //设置版本号
        VersionControllerEditor.AutoSetVersion("1.0.1");
        //统一Code编码格式
        CreateVersionManifestEditor.EncodeFileToUTF8Encoding();
        //生成AssetBundle
        AssetBoundleMaker.BuildingAssetBundle(buildType);
        //加密拷贝lua文件
        CopyLuaToDownload(buildType);
        //制作版本清单
        CreateZipVersionManifestFile(buildType);
        //生成应用程序
        CommandLineBuild(buildType);
    }


    public static void CreateVersionWindow(string platform)
    {
        m_path = string.Format("{0}/../Download/{1}/init.txt", System.Environment.CurrentDirectory, platform);
        m_window = EditorWindow.GetWindow(typeof(VersionControllerEditor), false, "版本号设置界面", true) as VersionControllerEditor;

        ReadInitFile();
    }

    /// <summary>
    /// 
    /// </summary>
    public static void CopyLuaToDownload(EBuildType buildType)
    {
        LuaPacker.BuildLuaAssetBundle();

        string luaPath = EditorCommon.GetDownloadPath(buildType) + "/lua/";

        if (Directory.Exists(luaPath)) { Directory.Delete(luaPath, true); }
        Directory.CreateDirectory(luaPath);

        string luaBundlePath = Utils.GetAssetBundleStreamingAssetsPath("lua");
        Utils.CopyFiles("拷贝lua文件",luaBundlePath, luaPath,"",".meta",true);

        UnityEngine.Debug.Log("拷贝Lua文件到资源目录完成");
    }



    public static void CommandLineBuild(EBuildType buildType)
    {
        if (EBuildType.StandaloneWindows == buildType)
        {
            CommandLineBuildWindowsProject();
        }
        else if (EBuildType.IOS == buildType)
        {
            CommandLineBuildXcodeProject();
        }
        else if (EBuildType.Android == buildType)
        {
            CommandLineBuildAndroid();
        }
    }


    private static void ReadInitFile()
    {
        if (System.IO.File.Exists(m_path))
        {
            string[] lines = System.IO.File.ReadAllLines(m_path);
            m_currDownLoadUrl = lines[0];
            m_currDownLoadUrl.Trim();
            m_currVersion = lines[1];
            m_currVersion.Trim();
            m_testDownLoadUrl = lines[2];
            m_newVersion = m_currVersion;
        }
    }

    private static void WriteInitFile()
    {
        string[] lines = new string[3];
        lines[0] = m_currDownLoadUrl;
        lines[1] = m_newVersion;
        lines[2] = m_testDownLoadUrl;


        string[] tempSplits = m_newVersion.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
        PlayerSettings.bundleVersion = string.Format("{0}.{1}", tempSplits[0], tempSplits[1]);

        System.IO.File.WriteAllLines(m_path, lines);
    }

    //一键打包设置版本号
    public static void AutoSetVersion(string strVersion)
    {
        m_path = string.Format("{0}/../Download/{1}/init.txt", System.Environment.CurrentDirectory, "Android");
        ReadInitFile();
        m_newVersion = strVersion;
        WriteInitFile();
    }

    public static void AutoSetVersionIos(string strVersion)
    {
        m_path = string.Format("{0}/../Download/{1}/init.txt", System.Environment.CurrentDirectory, "iOS");
        ReadInitFile();
        m_newVersion = strVersion;
        WriteInitFile();
    }

    [MenuItem("Tools/windows/1.打包", false, 5)]
    public static void CommandLineBuildWindowsProject()
    {
        string[] scenes = CreateVersionManifestEditor.GetBuildScenes();
        string dirPath = System.Environment.CurrentDirectory + @"/../win";
        if (Directory.Exists(dirPath)) { Directory.Delete(dirPath, true); }
        Directory.CreateDirectory(dirPath);

        string path = dirPath + @"/game.exe";
        if (scenes != null)
        {
            BuildPipeline.BuildPlayer(scenes, path, BuildTarget.StandaloneWindows64, BuildOptions.ShowBuiltPlayer);
        }

        Debug.Log("windos 版本制作完成！");
    }


    [MenuItem("Tools/Android/5.打包game.apk", false, 5)]
    public static void CommandLineBuildAndroid()
    {
        PlayerSettings.Android.keystoreName = Application.dataPath + "/../keystore/runningman.keystore";
        PlayerSettings.Android.keystorePass = "ztgame@runningman";
        PlayerSettings.Android.keyaliasName = "runningman";
        PlayerSettings.Android.keyaliasPass = "ztgame@runningman";

        string path = Application.dataPath.Replace("GameProject/Assets", "apk/game.apk");
        if (File.Exists(path))
            File.Delete(path);
        CommandLineBuildAndroid(path);
    }


    [MenuItem("Tools/iOS/5.打包", false, 5)]
    public static void CommandLineBuildXcodeProject()
    {
        string[] scenes = CreateVersionManifestEditor.GetBuildScenes();
        string path = Application.dataPath.Replace("/Assets", "") + "/src_ios";
        Debug.Log("@@@@@@path:" + path);
        if (scenes != null)
        {
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.iOS, ScriptingImplementation.IL2CPP);
            BuildPipeline.BuildPlayer(scenes, path, BuildTarget.iOS, BuildOptions.ShowBuiltPlayer | BuildOptions.AcceptExternalModificationsToPlayer
                );
        }
    }

    /// <summary>
    /// 生成版本清单文件
    /// </summary>
    /// <param name="buildType"></param>
    public static void CreateZipVersionManifestFile(EBuildType buildType)
    {
        string downLoadPath = EditorCommon.GetDownloadPath(buildType);
        CreateVersionManifestEditor.CreateVersionManifestFile(downLoadPath,
            "zip.txt",
            null,
            new string[] { ".zip" },
            CreateVersionManifestEditor.IgnoreType.ONLY_USE_NEED);

        UnityEngine.Debug.Log("生成服务器ZIP资源清单完成");
    }

    public static void CommandLineBuildAndroid(string path)
    {
        string[] scenes = CreateVersionManifestEditor.GetBuildScenes();
        if (scenes != null)
        {
            // BuildPipeline.BuildPlayer(scenes, path, BuildTarget.Android, BuildOptions.None);

            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
            buildPlayerOptions.scenes = scenes;
            buildPlayerOptions.locationPathName = path;
            buildPlayerOptions.target = BuildTarget.Android;
            buildPlayerOptions.options = BuildOptions.None;

            UnityEditor.Build.Reporting.BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            UnityEditor.Build.Reporting.BuildSummary summary = report.summary;

            if (summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded)
            {
                Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
            }
            else
            {
                Debug.LogError("Build failed");
            }
        }
    }
}

#endif
