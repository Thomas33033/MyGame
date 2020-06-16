using UnityEngine;
using System.Collections.Generic;
using System;
using System.Threading;

public class ThreadManager : LSingleton<ThreadManager>
{
    public readonly int maxThreads = 8;
    private int numThreads;


    private List<Action> _actions = new List<Action>();
    public struct DelayedQueueItem
    {
        public float time;
        public Action action;
    }
    private List<DelayedQueueItem> _delayed = new List<DelayedQueueItem>();

    List<DelayedQueueItem> _currentDelayed = new List<DelayedQueueItem>();

    public void Init()
    {
        //ThreadPool.SetMaxThreads(8,8);
    }

    public void RunOnMainThread(Action action)
    {
        RunOnMainThread(action, 0f);
    }
    public void RunOnMainThread(Action action, float time)
    {
        if (time != 0)
        {
            lock (_delayed)
            {
                _delayed.Add(new DelayedQueueItem { time = Time.realtimeSinceStartup + time, action = action });
            }
        }
        else
        {
            lock (_actions)
            {
                _actions.Add(action);
            }
        }
    }

    public Thread RunAsync(Action a)
    {
        while (numThreads >= maxThreads)
        {
            Thread.Sleep(1);
        }
        Interlocked.Increment(ref numThreads);
        ThreadPool.QueueUserWorkItem(RunAction, a);
        return null;
    }

    private void RunAction(object action)
    {
        //string strLog = "RunAction on thread " + Thread.CurrentThread.GetHashCode();
        //FFDebug.Log(this, GameLogType.Default, strLog);
        try
        {
            ((Action)action)();
        }
        catch (Exception e)
        {
            string strLog = "RunAction Exception e : " + e.ToString();
            Debug.LogError(strLog);
        }
        finally
        {
            Interlocked.Decrement(ref numThreads);
            //strLog = "Dispose thread " + Thread.CurrentThread.GetHashCode();
            //FFDebug.Log(this, GameLogType.Default, strLog);
        }
    }

    List<Action> _currentActions = new List<Action>();

    public void Update()
    {
        lock (_actions)
        {
            _currentActions.Clear();
            _currentActions.AddRange(_actions);
            _actions.Clear();
        }

        for (int i = 0; i < _currentActions.Count; i++)
        {
            _currentActions[i]();
        }

        lock (_delayed)
        {
            _currentDelayed.Clear();
            for (int i = 0; i < _delayed.Count; i++)
            {
                DelayedQueueItem item = _delayed[i];
                if (item.time <= Time.realtimeSinceStartup)
                {
                    _currentDelayed.Add(item);
                }
            }

            for (int i = 0; i < _currentDelayed.Count; i++)
            {
                _delayed.Remove(_currentDelayed[i]);
            }
        }


        for (int i = 0; i < _currentDelayed.Count; i++)
        {
            _currentDelayed[i].action();
        }



        if (Input.GetKeyUp(KeyCode.X))
        {
            int maxWorkThreads;//, workThread;
            int portThread;
            ThreadPool.GetMaxThreads(out maxWorkThreads, out portThread);
            //ThreadPool.GetAvailableThreads(out workThread, out portThread);
            //string strLog = "maxWorkThreads->" + maxWorkThreads + "  workThread->" + numThreads;
            //FFDebug.Log(this, GameLogType.Default, strLog);
        }
    }
}

