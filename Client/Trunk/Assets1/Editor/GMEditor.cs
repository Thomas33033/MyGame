using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GMEditor : EditorWindow
{

    [MenuItem("[Gatecen] Tools/RunGM", false, 0)]
    public static void RunGMCmd()
    {
        LuaManager.Instance.DoFile("RunGm.lua");
    }
}
