using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadTools
{
    public static string UIPath = "Assets/BundleRes/UI/";
    public static void SetParent(GameObject obj, GameObject parent)
    {
        if (obj != null && parent != null)
        {
            obj.transform.SetParent(parent.transform);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;
            obj.transform.localScale = Vector3.one;
            obj.layer = parent.layer;
        }
    }

    public static GameObject LoadUI(string path,string name,GameObject parent)
    {
        string assetPath = UIPath + path + "/" + name + ".prefab";
        GameObject obj = AssetsManager.LoadAsset<GameObject>(assetPath);
        GameObject uiObj = GameObject.Instantiate(obj);
        uiObj.name = name;
        if (parent != null)
        {
            uiObj.transform.SetParent(parent.transform);
        }

        RectTransform rectTrans = uiObj.GetComponent<RectTransform>();
        if (rectTrans != null)
        {
            rectTrans.anchoredPosition = new Vector2();
            rectTrans.anchoredPosition3D = new Vector3();
            rectTrans.offsetMin = new Vector2();
            rectTrans.offsetMax = new Vector2();
        }

        uiObj.transform.localScale = Vector3.one;
        return uiObj;
    }

    public static void LoadSprite(string path, string name)
    {
        string assetPath = UIPath + path + "/" + name + ".png";
        //增加资源引用计数器
        AssetsManager.LoadAsset<Sprite>(assetPath);
    }

    //加载场景资源
    static public void LoadAssetBundleScene(string name, int type, System.Action cb, System.Action action)
    {
        //AppBoot.instance.LoadAssetBundleScene(name, , cb, dependencies);
        AssetsManager.LoadSceneAsync(name, type == 0 ? LoadSceneMode.Single : LoadSceneMode.Additive, action);
    }

    //卸载场景资源
    static public void UnloadSceneAsync(string name)
    {
        SceneManager.UnloadSceneAsync(name);
    }
}

