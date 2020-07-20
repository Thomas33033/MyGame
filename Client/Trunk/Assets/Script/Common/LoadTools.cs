using Cherry.AssetBundlePacker;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;

public class LoadTools
{
    public static bool useAssetBundle = true;


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

    public static Sprite LoadSprite(string path, string name)
    {
        string assetPath = ResPathHelper.UI_WINDOW_PATH + path + "/" + name;
        return AssetsManager.LoadSprite(assetPath);
    }

    /// <summary>
    /// 加载私有图集
    /// </summary>
    /// <param name="packageName"></param>
    /// <returns></returns>
    static public SpriteAtlas LoadPrivateAtlas(string packageName)
    {
#if UNITY_EDITOR
        if (useAssetBundle == false)
        {
            return AssetDatabase.LoadAssetAtPath<SpriteAtlas>("Assets/BundleResources/" + packageName + "/" + packageName + "_Atlas.spriteatlas");
        }
#endif
        return AssetBundleManager.Instance.LoadAsset<SpriteAtlas>(packageName + "_Atlas");
    }


    /// <summary>
    /// 加载公共图集
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    static public SpriteAtlas LoadSpriteAtlas(string name)
    {
#if UNITY_EDITOR
        if (useAssetBundle == false)
        {
            return AssetDatabase.LoadAssetAtPath<SpriteAtlas>(UIPath + name + ".spriteatlas");
        }
#endif
        return AssetBundleManager.Instance.LoadAsset<SpriteAtlas>(name);
    }

    //加载场景资源
    static public void LoadAssetBundleScene(string name, int type, System.Action callBack)
    {
        AssetsManager.LoadSceneAsync(name, type == 0 ? LoadSceneMode.Single : LoadSceneMode.Additive, callBack);
    }

    //卸载场景资源
    static public void UnloadSceneAsync(string name)
    {
        SceneManager.UnloadSceneAsync(name);
    }


    static public void ClearAssetBundle()
    { 

    }

    static public void ClearSceneCache()
    { 
    
    }
}

