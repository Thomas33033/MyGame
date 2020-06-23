using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/**
 * 
 * 系统时间管理类
 * 比如延迟做某件事情
 */
public class TimeSchedule
{
    private static int lastTime = 0;
    public static int GetUniqueId()
    {
        return lastTime++;
    }


    public class DelayDo
    {
        public float endTime;
        public Action callBack;

        public DelayDo(float p_duration, Action p_callBack)
        {
            this.endTime = Time.time + p_duration;
            this.callBack = p_callBack;
        }
    }
    public List<DelayDo> list = new List<DelayDo>();
    public List<DelayDo> waitRemoveList = new List<DelayDo>();

    public void AddSchedule(float duration, Action action)
    {
        list.Add(new DelayDo(duration, action));
    }
    public void RemoveSchedule(Action action)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].callBack == action)
            {
                list.RemoveAt(i);
            }
        }
    }
    public virtual void OnUpdate()
    {
        if (list.Count > 0)
        {
            waitRemoveList.Clear();
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].endTime <= Time.time)
                {
                    if (list[i].callBack != null)
                        list[i].callBack();
                    list[i].callBack = null;
                    waitRemoveList.Add(list[i]);
                }
            }
            for (int i = 0; i < waitRemoveList.Count; i++)
            {
                list.Remove(waitRemoveList[i]);
            }
        }
    }
}
