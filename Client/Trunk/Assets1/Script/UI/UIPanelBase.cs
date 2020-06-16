﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIPanelBase : MonoBehaviour {

    public void Awake()
    {
        
    }

	// Use this for initialization
	public void Start () {
		
	}
	
	// Update is called once per frame
    public void Update()
    {
		
	}

    public void OnEnable()
    { 
    
    }

    public void OnDisable()
    { 
    
    }

    public void Ondestroy()
    { 
    
    }

    public T FindObject<T>(GameObject parent, string childPath) where T : MonoBehaviour
    {
        Transform trans = parent.transform.Find(childPath);
        if (trans != null)
        {
          T t = trans.gameObject.GetComponent<T>();
          if (t != null)
          {
             return t;
          }
          else
          {
              string exception_log = string.Format("Can't find {0} component in path :{1}", typeof(T).Name, childPath);
              Debug.LogError(exception_log);
          }
        }
        return null;
    }
}
