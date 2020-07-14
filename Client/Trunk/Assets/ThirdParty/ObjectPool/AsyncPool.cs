using System.Collections.Generic;
using UnityEngine;

public class AsyncPool<T> : IObjectPool where T : ObjectInPoolBase, new()
{
    private List<T> queue = new List<T>();
    public string prefabName;
    public string moduleName;
    public UnityEngine.Object prefab;
    private Transform parentRootT;

    public delegate void LoadCompleteHandler(T poolobj);

    private List<LoadCompleteHandler> _listCallbacks;

    private bool isLoadComplete;

    public AsyncPool(string p_assetPath, Transform rootT)
    {
        this.prefabName = p_assetPath;

        parentRootT = rootT;

        _listCallbacks = new List<LoadCompleteHandler>();

        AssetsManager.LoadAssetAsync(p_assetPath, LoadPrefabComplete);
    }

    private void LoadPrefabComplete(UnityEngine.Object obj)
    {
        prefab = obj;
        isLoadComplete = true;
    }

    public void AddLoadCallback(LoadCompleteHandler v)
    {
        _listCallbacks.Add(v);
    }

    public T GetObject()
    {
        T t;
        bool isSearch = true;

        while (isSearch)
        {
            if (queue.Count > 0)
            {
                t = queue[0];
                queue.RemoveAt(0);
                if (t.CheckCanActive())
                {
                    t.OnEnable();
                    isSearch = false;
                    t.itemObj.transform.SetParent(null);
                    return t;
                }
            }
            else
            {
                isSearch = false;
            }
        }
        t = new T();
        t.prefabName = this.prefabName;
        Debug.LogError(t.prefabName);
        t.itemObj = GameObject.Instantiate(prefab) as GameObject;
        //t.itemObj.transform.parent = parentRootT;
        t.OnEnable();
        return t;
    }

    public void PutInPool(object v)
    {
        T t = v as T;
        if (t != null && t.itemObj != null && t.curState == PoolItemState.Active)
        {
            t.itemObj.transform.SetParent(parentRootT);
            t.OnDisable();
            queue.Add(t);
        }
    }

    public void RemoveObject(object v)
    {
        T t = v as T;
        queue.Remove(t);
    }

    public void Dispose()
    {
        List<T> copyList = new List<T>();
        for (int i = 0; i < queue.Count; i++)
        {
            copyList.Add(queue[i]);
        }

        for (int i = 0; i < copyList.Count; i++)
        {
            copyList[i].OnDestroy();
        }
    }

    public void OnUpdate()
    {
        for (int i = 0; i < queue.Count; i++)
        {
            queue[i].Update();
        }

        if (isLoadComplete && _listCallbacks.Count > 0)
        {
            _listCallbacks[0](GetObject());
            _listCallbacks.RemoveAt(0); ;
        }
    }

    public string CurResId()
    {
        return prefabName;
    }
}