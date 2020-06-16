#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System;
using Cherry.AssetBundlePacker;


public class PrepareIosEditor : EditorWindow
{
    [MenuItem("Tools/iOS/1.手动设置版本号", false, 1)]
    public static void CreateVersionWindow()
    {
        VersionControllerEditor.CreateVersionWindow("iOS");
    }


    [MenuItem("Tools/iOS/4.删除其他平台相关的", false, 4)]
    public static void DeleteDir()
    {
        string path = Application.dataPath + "/Plugins/Android";
        if (Directory.Exists(path)) { Directory.Delete(path, true); }


        path = Application.dataPath + "/StreamingAssets/Android";
        if (Directory.Exists(path)) { Directory.Delete(path, true); }

        AssetDatabase.Refresh();
    }

}

#endif