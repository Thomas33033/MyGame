using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class OneKeyBuildEditor
{

    public static void OneKeyBuildIpa()
    {
        string[] args = System.Environment.GetCommandLineArgs();
        string strDate = string.Format("{0}{1:d2}{2:d2}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
        string strVersion = args[7] + "." + strDate;

    }

}
