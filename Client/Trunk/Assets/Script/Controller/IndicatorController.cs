//================================================
// auth：xuetao
// date：2018/5/25 18:19:43
// 
//================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class IndicatorController : Singleton<IndicatorController>
{
    ModelPoolObj modelPoolObj;
    private GameObject root;
    public void OnInit()
    {
        if (this.root == null)
        {
            this.root = new GameObject("IndicatorRoot");
            this.root.transform.Reset();
        }
    }

    public void Create(Action<GameObject> loadCallBack)
    {
        if (root == null)
        {
            this.OnInit();
        }

        var pool = ObjectPoolManager.Instance.CreatePool<ModelPoolObj>(ResPathHelper.UI_EFFECT_APTH + "indicator1.prefab");
        modelPoolObj = pool.GetObject();
        GameObject go = modelPoolObj.itemObj;
        go.transform.parent = this.root.transform;
        go.transform.localEulerAngles = Vector3.zero;
        loadCallBack(go);
    }
}

