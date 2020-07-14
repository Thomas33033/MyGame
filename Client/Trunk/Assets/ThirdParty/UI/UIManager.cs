using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cherry.AssetBundlePacker;
using System.Security.Cryptography;

public class UIManager: Singleton<UIManager>  {

    public Dictionary<System.Type, GameObject> pageDic = new Dictionary<System.Type, GameObject>();

    private GameObject m_UIRoot = null;

    public GameObject UIRoot {
        get {
            if (m_UIRoot == null) m_UIRoot = GameObject.Find("UIRoot/layer_1");
            return m_UIRoot;
        }
    }
    
    public T ShowUI<T>(string name) where T : MonoBehaviour
    {
        string path = ResPathHelper.UI_WINDOW_PATH + name + ".prefab";
        Debug.LogError(path);
        GameObject obj = AssetsManager.LoadAsset<GameObject>(path);
        GameObject uiObj = GameObject.Instantiate(obj);
        uiObj.name = name;

        RectTransform objT = uiObj.transform as RectTransform;
        objT.SetParent(UIRoot.transform, false);

        T t = uiObj.GetComponent<T>();
        pageDic.Remove(t.GetType());
        pageDic.Add(t.GetType(), uiObj);
        return t;
    }

    public GameObject CreateUI(string name)
    {
        string path = ResPathHelper.UI_WINDOW_PATH + name + ".prefab";
        GameObject obj = AssetsManager.LoadAsset<GameObject>(path);
        GameObject uiObj = GameObject.Instantiate(obj);
        uiObj.name = name;

        RectTransform objT = uiObj.transform as RectTransform;
        objT.SetParent(UIRoot.transform, false);
        return uiObj;
    }



    public void Delete<T>(T t) where T : MonoBehaviour
    { 
        GameObject go ;
        if (pageDic.TryGetValue(t.GetType(),out go))
        {
            if (go != null)
            {
                GameObject.Destroy(go.gameObject);
            }
            pageDic.Remove(t.GetType());
        }
        Resources.UnloadUnusedAssets();
    }


    public void DeleteUI(GameObject go)
    {
        if(go != null)
        {
            GameObject.Destroy(go.gameObject);
        }
        
    }
    
}

