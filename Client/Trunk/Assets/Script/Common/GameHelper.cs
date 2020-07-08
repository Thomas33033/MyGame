using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public static class GameHelper  
{

	// Use this for initialization
	public static T GetOrAddComponent<T>(this GameObject go) where T : Component
    {
        T t = go.gameObject.GetComponent<T>();
        if (t == null)
            t = go.AddComponent<T>();
        return t;
	}

    public static void Reset(this Transform t)
    {
        t.position = Vector3.zero;
        t.localRotation = new Quaternion(0, 0, 0, 0);
        t.localScale = Vector3.one;
    }

    public static T FindComponent<T>(this GameObject gameObject, string path) where T : Component
    {
        Transform trans = gameObject.transform.Find(path);
        T t = null;
        if (trans != null)
        {
            t = trans.gameObject.GetComponent<T>();
        }
        return t;
    }
}
