using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class ModelPoolObj : ObjectInPoolBase
{
    public override void ReturnToPool()
    {
        if (itemObj != null && curState == PoolItemState.Active)
        {
            ObjectPoolManager.Instance.PutInPool(this);
        }
    }

    public override void OnDestroy()
    {
        ObjectPoolManager.Instance.RemoveObject(this);
        base.OnDestroy();
    }
}

