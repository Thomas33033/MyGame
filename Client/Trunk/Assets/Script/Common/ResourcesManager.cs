using UnityEngine;
using System.Collections;
using System.IO;
using Cherry.AssetBundlePacker;
using Cherry;
using System;

/// <summary>
///   资源加载器
/// </summary>
public class ResourcesManager
{

    public static string persistentDataPath
    {
        get
        {
#if UNITY_EDITOR
            return Application.dataPath.Replace("Assets", "PersistentData");
#elif UNITY_ANDROID
            return Application.persistentDataPath;
#else
            return Application.persistentDataPath;
#endif
        }
    }


    /// <summary>
    ///   资源相对目录
    /// </summary>
    public static readonly string RESOURCES_LOCAL_DIRECTORY = "Assets/Resources/";

    /// <summary>
    ///   资源全局目录 
    /// </summary>
    public static readonly string RESOURCES_DIRECTORY = Application.dataPath + "/Resources/";

    /// <summary>
    /// 异步加载资源
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="asset"></param>
    /// <param name="allowUnloadAssetBundle"></param>
    /// <param name="loadPattern"></param>
    /// <returns></returns>
    public static void LoadAssetAsync(string asset, Action<UnityEngine.Object> callBack, bool allowUnloadAssetBundle = true)
    {
        callBack(null);
    }

    /// <summary>
    ///  同步加载一个资源
    /// <param name="asset">资源局部路径（"Assets/..."）</param>
    /// </summary>
    public static T LoadAsset<T>(string asset, bool allowUnloadAssetBundle = true)
            where T : UnityEngine.Object
    {

#if UNITY_EDITOR
        return LoadAssetAtPath<T>(asset);
#endif
        return AssetBundleManager.Instance.LoadAsset<T>(asset, !allowUnloadAssetBundle);

    }

    /// <summary>
    /// 卸载一个资源(非GameObject)
    /// </summary>
    public static void Unload(string asset, ELoadPattern loadPattern = ELoadPattern.AssetBundle)
    {
        if (loadPattern == ELoadPattern.AssetBundle
            || loadPattern == ELoadPattern.All)
        {
            AssetBundleManager.Instance.UnloadAsset(asset);
        }
    }

    /// <summary>
    ///   加载一个Resources下资源
    /// <param name="asset">资源局部路径（"Assets/..."）</param>
    /// </summary>
    public static T LoadFromResources<T>(string asset) where T : UnityEngine.Object
    {
        //去除扩展名
        asset = FileHelper.GetPathWithoutExtension(asset);
        //转至以Resources为根目录的相对路径
        asset = FileHelper.AbsoluteToRelativePath(RESOURCES_LOCAL_DIRECTORY, asset);
        T a = Resources.Load<T>(asset);
        return a;
    }

    /// <summary>
    ///   文本文件加载
    /// <param name="file_name">全局路径</param>
    /// </summary>
    public static string LoadTextFile(string file_name)
    {
        try
        {
            if (!string.IsNullOrEmpty(file_name))
            {
                if (File.Exists(file_name))
                    return File.ReadAllText(file_name);
            }

        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex.Message);
        }

        return null;
    }

    /// <summary>
    ///   二进制文件加载
    /// <param name="file_name">全局路径</param>
    /// </summary>
    public static byte[] LoadByteFile(string file_name)
    {
        try
        {
            if (!string.IsNullOrEmpty(file_name))
            {
                if (File.Exists(file_name))
                    return File.ReadAllBytes(file_name);
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex.Message);
        }

        return null;
    }

#if UNITY_EDITOR
    /// <summary>
    ///   加载一个Resources下资源
    /// <param name="asset">资源局部路径（"Assets/..."）</param>
    /// </summary>
    public static T LoadAssetAtPath<T>(string asset)
        where T : UnityEngine.Object
    {
        return UnityEditor.AssetDatabase.LoadAssetAtPath<T>(asset);
    }
#endif
}
