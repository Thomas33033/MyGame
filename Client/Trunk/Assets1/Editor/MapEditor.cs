#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;

public class MapEditor : EditorWindow
{

    [MenuItem("[Gatecen] Tools/地图相关/导出地图配置", false, 1)]
    public static void CreateMap()
    {
        string sPath = Application.dataPath + "/../../../doc/MapConfig";

        string sceneName = SceneManager.GetActiveScene().name;

        string fileName = sPath + "/" + sceneName + ".map";
          
        UnityEngine.Debug.Log("导出地图配置完成    " + sceneName + " fileName:" + fileName);
    }

}

#endif