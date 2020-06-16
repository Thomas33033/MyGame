using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolToos
{
    private static Dictionary<Type, Queue<BehaviourState>> mMap = new Dictionary<Type, Queue<BehaviourState>>();
    public static T GetClass<T>() where T : BehaviourState,new()
    {
        T t = null;
        Type type = typeof(T);
        if (mMap.ContainsKey(type))
        {
            Queue<BehaviourState> queue = mMap[type];
            if (queue.Count > 0)
            {
                t = (T)queue.Dequeue();
                t.OnReset();
            }
        }
        if(t == null)
            t = new T();
        return t;
    }

    public static void FreeClass<T>(T t) where T : BehaviourState,new()
    {
        Type type = typeof(T);
        if (mMap.ContainsKey(type))
        {
            Queue<BehaviourState> queue = mMap[type];
            queue.Enqueue(t);
        }
    }
}
