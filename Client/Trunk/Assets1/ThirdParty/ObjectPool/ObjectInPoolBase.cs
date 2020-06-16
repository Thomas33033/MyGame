using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ObjectInPoolBase
{
    //当前状态
    public PoolItemState curState;

    //存在缓冲池中的时间
    public float liveTimes;

    //当前对象实体
    public GameObject itemObj;

    public float deleteTime;

    public string prefabName;

    public string GetResPath()
    {
        return prefabName;
    }

    public bool CheckCanActive()
    {
        if (itemObj != null && curState == PoolItemState.DisActive)
        {
            return true;
        }
        return false;
    }

    public virtual void ReturnToPool()
    {
        if (itemObj != null && curState == PoolItemState.Active)
        {
            ObjectPoolManager.Instance.PutInPool(this);
        }
    }

    public void Update()
    {
        if (curState == PoolItemState.DisActive)
        {
            if (deleteTime < Time.time)
            {
                OnDestroy();
            }
        }
    }

    public virtual void OnEnable()
    {
        curState = PoolItemState.Active;
        if (itemObj != null)
        {
            itemObj.SetActive(true);
        }
    }

    public virtual void SetParent(Transform tParent)
    {
        this.itemObj.transform.parent = tParent;
    }

    public virtual void OnDisable()
    {
        curState = PoolItemState.DisActive;
        deleteTime = Time.time + 10f;
        if (itemObj != null)
        {
            itemObj.SetActive(false);
        }
    }

    public virtual void OnDestroy()
    {
        curState = PoolItemState.MarkAsDeleted;
        GameObject.Destroy(this.itemObj);
        curState = PoolItemState.Deleted;
    }
}