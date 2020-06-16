using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ComponentMgr  
{
    public CharacterBase owner;
    private List<ComponentBase> componentList;
    private Dictionary<Type, ComponentBase> componentMap;

    public ComponentMgr(CharacterBase p_owner)
    {
        this.owner = p_owner;
        this.componentList = new List<ComponentBase>();
        this.componentMap = new Dictionary<Type, ComponentBase>();
    }

    public T AddComponent<T>() where T : ComponentBase, new()
    {
        T t = new T();
        t.OnInit(owner);
        componentList.Add(t);
        componentMap.Add(typeof(T), t);
        return null;
    }

    public T GetComponent<T>() where T : ComponentBase
    {
        Type type = typeof(T);
        if (componentMap.ContainsKey(type))
        {
            return (T)componentMap[type];
        }
        return null;
    }

    public void OnUpdate(float dt)
    {
        for (int i = 0; i < componentList.Count; i++)
        {
            componentList[i].OnUpdate(dt);
        }
    }

    public void OnDispose()
    {
        Debug.LogError("--------componentMgr OnDispose--------------");
        for (int i = 0; i < componentList.Count; i++)
        {
            componentList[i].OnDestroy();
        }
        this.owner = null;
        this.componentList.Clear();
        this.componentMap.Clear();
        this.componentList = null;
        this.componentMap = null;
    }
}
