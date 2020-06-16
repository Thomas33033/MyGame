//================================================
// auth：xuetao
// date：2018/5/28 18:21:03
//================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public enum AssetsType
{
    Npc,
    Building,
    Audio,
    Effect,

}

class AssetManager : Singleton<AssetManager>
{

    private Dictionary<AssetsType, string> resLocalPathMap;

    public void LoadAssets(AssetsType type, string resName,Action<GameObject> loadOverEvent)
    {
        if (resLocalPathMap == null)
        {
            resLocalPathMap = new Dictionary<AssetsType, string>();
            resLocalPathMap.Add(AssetsType.Effect, "Prefabs/Effect/");
            resLocalPathMap.Add(AssetsType.Npc, "Prefabs/npc/");
            resLocalPathMap.Add(AssetsType.Building, "Prefabs/Towers/");
            resLocalPathMap.Add(AssetsType.Audio, "Audio/");
        }
        string path = string.Format("{0}{1}", resLocalPathMap[type], resName);
        UnityEngine.Object obj = Resources.Load(path);
        if (obj != null)
        {
            loadOverEvent(obj as GameObject);
        }
        else
        {
            Debug.LogError("load assets failed !!!! please check:"+path );
        }
    }
}

