using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> where T : class
{
    public static List<T> curInstanceList = new List<T>();
    private static T _instance;
    private static readonly object syslock = new object();

    public virtual void OnCreate() { }

    protected Singleton() {
        this.OnCreate();
    }

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (syslock)
                {
                    if (_instance == null)
                    {
                        _instance = (T)Activator.CreateInstance(typeof(T), true);
                        curInstanceList.Add(_instance);
                    }
                }
            }
            return _instance;
        }
    }
}
