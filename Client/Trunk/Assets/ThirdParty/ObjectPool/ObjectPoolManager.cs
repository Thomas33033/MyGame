using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PoolType
{
    Model,
    Effect,
}

public class ObjectPoolManager : Singleton<ObjectPoolManager>
{
    public Dictionary<string, IObjectPool> poolMap = new Dictionary<string, IObjectPool>();
    public Transform rootT;

    public void OnCreate()
    {
        if (GameObject.Find("Pools") == null)
        {
            rootT = new GameObject().transform;
            rootT.name = "Pools";
            GameObject.DontDestroyOnLoad(rootT);
        }
    }

    public T GetPoolObj<T>(string assetName) where T : ObjectInPoolBase, new()
    {
        var pool = CreatePool<T>(assetName);
        var modelPoolObj = pool.GetObject();
        return modelPoolObj;
    }

    public void LoadObjectAsync<T>(string assetName, AsyncPool<T>.LoadCompleteHandler callback) where T : ObjectInPoolBase, new()
    {
        AsyncPool<T> pool = CreateAsyncPool<T>(assetName);
        pool.AddLoadCallback(callback);
    }

    public CObjectPool<T> CreatePool<T>(string assetName) where T : ObjectInPoolBase, new()
    {
        string resPath =  assetName;
        if (rootT == null)
        {
            rootT = GameObject.Find("Pools").transform;
        }

        if (poolMap.ContainsKey(resPath))
        {
            return (CObjectPool<T>)poolMap[resPath];
        }
        else
        {
            CObjectPool<T> pool = new CObjectPool<T>(assetName, rootT);
            poolMap.Add(resPath, pool);
            return (CObjectPool<T>)pool;
        }
    }

    public AsyncPool<T> CreateAsyncPool<T>(string assetName) where T : ObjectInPoolBase, new()
    {
        string resPath = assetName;
        if (rootT == null)
        {
            rootT = GameObject.Find("Pools").transform;
        }

        if (poolMap.ContainsKey(resPath))
        {
            return (AsyncPool<T>)poolMap[resPath];
        }
        else
        {
            AsyncPool<T> pool = new AsyncPool<T>(assetName, rootT);
            poolMap.Add(resPath, pool);
            return (AsyncPool<T>)pool;
        }
    }

    public void OnUpdate()
    {
        foreach (var v in poolMap.Values)
        {
            v.OnUpdate();
        }
    }

    public void PutInPool<T>(T t) where T : ObjectInPoolBase, new()
    {
        string resPath = t.GetResPath();
        if (poolMap.ContainsKey(resPath))
        {
            poolMap[resPath].PutInPool(t);
        }
        else
        {
            Debug.LogError("返回对象池失败！！！！！" + resPath);
        }
    }

    public void RemoveObject<T>(T t) where T : ObjectInPoolBase, new()
    {
        string resPath = t.GetResPath();
        if (poolMap.ContainsKey(resPath))
        {
            poolMap[resPath].RemoveObject(t);
        }
    }

    public void ClearPool()
    {
        foreach (var k in poolMap.Keys)
        {
            poolMap[k].Dispose();
        }
        poolMap.Clear();
    }

}