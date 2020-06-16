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
    public void OnInit(float gridSize)
    {
        if (this.root == null)
        {
            this.root  =new GameObject("IndicatorRoot");
            this.root.transform.Reset();
        }

        //indicator = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //indicator.name = "indicator";
        //indicator.SetActive(false);
        //indicator.transform.localScale = new Vector3(gridSize, 0.025f, gridSize);
        //indicator.transform.GetComponent<Renderer>().material = (Material)Resources.Load("IndicatorSquare");
        //indicator.transform.GetComponent<Renderer>().material.SetColor("_TintColor", Color.red);

        //indicator2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //indicator2.name = "indicator2";
        //indicator2.SetActive(false);
        //indicator2.transform.localScale = new Vector3(gridSize, 0.025f, gridSize);
        //indicator2.transform.GetComponent<Renderer>().material = (Material)Resources.Load("IndicatorSquare");
        //GameObject.Destroy(indicator.GetComponent<Collider>());
        //GameObject.Destroy(indicator2.GetComponent<Collider>());
    }

    public void Create(Action<GameObject> loadCallBack)
    {
        var pool = ObjectPoolManager.Instance.CreatePool<ModelPoolObj>(ResPathHelper.UI_EFFECT_APTH + "indicator1.prefab");
        modelPoolObj = pool.GetObject();
        GameObject go = modelPoolObj.itemObj;
        go.transform.parent = this.root.transform;
        go.transform.localEulerAngles = Vector3.zero;
        loadCallBack(go);
    }
}

