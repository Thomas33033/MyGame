using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class LoadTools
{
    public static void SetParent(GameObject obj, GameObject parent)
    {
        if (obj != null && parent != null)
        {
            obj.transform.SetParent(parent.transform);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;
            obj.transform.localScale = Vector3.one;
            obj.layer = parent.layer;
        }
    }
}

