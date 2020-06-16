using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class DamagePoolObj : ObjectInPoolBase
{
    public override void ReturnToPool()
    {
        if (itemObj != null && curState == PoolItemState.Active)
        {
            ObjectPoolManager.Instance.PutInPool(this);
        }
    }

    public override void SetParent(Transform tParent)
    {
        //伤害特效不放回对象池节点
    }

    public override void OnDestroy()
    {
        ObjectPoolManager.Instance.RemoveObject(this);
        base.OnDestroy();
    }
}

