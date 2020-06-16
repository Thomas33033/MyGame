/***************************************************************
 * Copyright 2016 By Zhang Minglin
 * Author: Zhang Minglin
 * Create: 2016/03/14
 * Note  : AssetBundle相关菜单项
***************************************************************/
using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace Cherry.AssetBundlePacker
{
    public class AssetBundleMenu : MonoBehaviour
    {
        protected AssetBundleMenu()
        { }

        #region Step 1
        [MenuItem("AssetBundle/1.Android平台打包", false, 51)]
        static void MakeBoundleAndroid()
        {
            AssetBoundleMaker.BuildingAssetBundle(EBuildType.Android);
        }
        #endregion

        #region Step 2
        [MenuItem("AssetBundle/2.IOS平台打包", false, 52)]
        static void MakeBoundleIOS()
        {
            AssetBoundleMaker.BuildingAssetBundle(EBuildType.IOS);
        }
        #endregion

        #region Step 3
        [MenuItem("AssetBundle/3.Windows平台打包", false, 53)]
        static void MakeBoundleWindow()
        {
            AssetBoundleMaker.BuildingAssetBundle(EBuildType.StandaloneWindows);
        }
        #endregion
    }
}