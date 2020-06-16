//================================================
// auth：xuetao
// date：2018/5/30 15:32:53
//================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class DebugMgr
{
    public static void LogError(string log)
    {
        UnityEngine.Debug.LogError(log);
    }

    public static void Log(string log)
    {
        UnityEngine.Debug.Log(log);
    }

    public static void LogWarning(string log)
    {
        UnityEngine.Debug.LogWarning(log);
    }
}

