using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolToos
{
    private static Dictionary<Type, Queue<Entity>> mMap = new Dictionary<Type, Queue<Entity>>();
    public static T GetClass<T>() where T : Entity, new()
    {
        T t = null;
        Type type = typeof(T);
        if (mMap.ContainsKey(type))
        {
            Queue<Entity> queue = mMap[type];
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

    public static void FreeClass<T>(T t) where T : Entity, new()
    {
        Type type = typeof(T);
        if (mMap.ContainsKey(type))
        {
            Queue<Entity> queue = mMap[type];
            queue.Enqueue(t);
        }
    }
}
