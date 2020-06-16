using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public interface IObjectPool
{
    void OnUpdate();
    void Dispose();
    string CurResId();

    void PutInPool(object v);

    void RemoveObject(object t);
}

public class CObjectPool<T> : IObjectPool where T : ObjectInPoolBase, new()
{
    private List<T> queue = new List<T>();
    public string prefabName;
    public UnityEngine.Object prefab;
    private Transform parentRootT;

    public CObjectPool(string p_prefabName, Transform rootT) 
    {
        prefabName = p_prefabName;
        prefab = ResourcesManager.LoadAsset<GameObject>(prefabName);

        if (prefab == null)
        {
            Debug.LogError("Not Find:"+prefabName);
        }
        parentRootT = rootT;
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
        t.itemObj = GameObject.Instantiate(prefab) as GameObject;
        t.itemObj.transform.parent = parentRootT;
        //t.itemObj.transform.Reset();
        t.curState = PoolItemState.Active;
        return t;
        
    }

    public void PutInPool(T t)
    {
        if (t != null && t.itemObj != null && t.curState == PoolItemState.Active)
        {
            t.itemObj.transform.parent = parentRootT;
            //t.itemObj.transform.Reset();
            t.itemObj.SetActive(false);
            t.curState = PoolItemState.DisActive;
            queue.Add(t);
        }
    }

    public void RemoveObject(T t)
    {
        queue.Remove(t);
    }

    public void Dispose()
    {
        for (int i = 0; i < queue.Count; i++)
        {
            queue[i].OnDisable();
        }
    }

    public void OnUpdate()
    {
        for (int i = 0; i < queue.Count; i++)
        {
            queue[i].Update();
        }
    }

    public string CurResId()
    {
        return prefabName;
    }

    public void RemoveObject(object v)
    {
        T t = v as T;
        queue.Remove(t);
    }

    public void PutInPool(object v)
    {
        T t = v as T;
        if (t != null && t.itemObj != null && t.curState == PoolItemState.Active)
        {
            // t.itemObj.transform.parent = parentRootT;
            t.SetParent(parentRootT);
            t.OnDisable();
            queue.Add(t);
        }
    }

}

public enum PoolItemState
{
    Active,        //激活状态
    DisActive,     //未激活状态，存在缓冲池中
    MarkAsDeleted, //未激活时间超时，标志为删除状态 
    Deleted,       //已删除
}