using UnityEngine;
using System.Collections.Generic;
using System;
using System.Threading;
using UnityEngine.SceneManagement;
using LuaInterface;


public class TimeManager : MonoSingleton<TimeManager>
{
    public event Action UpdateEvent;//为了减少其他类的Update的调用，故需要执行Update的类只需要监听这个事件;

    private GyObjectPool<Delay> m_messagePool = new GyObjectPool<Delay>();
    private List<Delay> m_finishDelayList = new List<Delay>();
    private List<Delay> m_delayList = new List<Delay>();

    private Delay NewDelay()
    {
        Delay delay = m_messagePool.New();
        delay.GenerateId();
        delay.isStored = false;
        return delay;
    }
    private void StoreDelay(Delay delay)
    {
        if (delay.isStored)
        {
            return;
        }
        delay.Clear();
        delay.isStored = true;
        m_messagePool.Store(delay);
    }


    #region 时间转换相关的函数;

    public static DateTime s_startTime = new DateTime(1970, 1, 1).ToLocalTime();//参考时间;
    private UInt64 m_clientTime = 0;//服务器最后同步游戏tick时客户端的时间(毫秒);

    private UInt64 m_serverCurrMilliSecond = 0;//服务器当前tick(毫秒);

    public UInt64 GetNowClinetMilliSecond()
    {
        return (UInt64)((DateTime.Now - s_startTime).TotalMilliseconds);
    }

    public string GetNowClinetMilliSecond_String()
    {
        return GetNowClinetMilliSecond().ToString();
    }

    public UInt64 GetNowClinetSecond()
    {
        return (GetNowClinetMilliSecond() / 1000);
    }

    public string GetNowClinetSecond_String()
    {
        return GetNowClinetSecond().ToString();
    }

    public UInt64 GetNowServerMilliSecond()
    {
        UInt64 now = GetNowClinetMilliSecond();
        if (now > m_clientTime)
        {
            return (now - m_clientTime + m_serverCurrMilliSecond);
        }

        return m_serverCurrMilliSecond;
    }

    public string GetNowServerMilliSecond_String()
    {
        return GetNowServerMilliSecond().ToString();
    }


    public UInt64 GetNowServerSecond()
    {
        return (GetNowServerMilliSecond() / 1000);
    }
    public string GetNowServerSecond_String()
    {
        return GetNowServerSecond().ToString();
    }

    public string Diff_MilliSecond_String(string time1, string time2)
    {
        UInt64 u1 = UInt64.Parse(time1);
        UInt64 u2 = UInt64.Parse(time2);

        if (u1 > u2)
        {
            return (u1 - u2).ToString();
        }
        else
        {
            return string.Format("-{0}", (u2 - u1));
        }
    }

    /// <summary>
    /// 有正有负;
    /// </summary>
    /// <param name="time1"></param>
    /// <param name="time2"></param>
    /// <returns></returns>
    public Int32 Diff_Second_S_S(string time1, string time2)
    {
        UInt64 u1 = UInt64.Parse(time1);
        UInt64 u2 = UInt64.Parse(time2);

        if (u1 > u2)
        {
            return (Int32)(u1 - u2);
        }
        else
        {
            return (0 - (Int32)(u2 - u1));
        }
    }

    /// <summary>
    /// 有正有负;
    /// </summary>
    /// <param name="time1"></param>
    /// <param name="time2"></param>
    /// <returns></returns>
    public Int32 Diff_Second_U_U(UInt64 time1, UInt64 time2)
    {
        if (time1 > time2)
        {
            return (Int32)(time1 - time2);
        }
        else
        {
            return (0 - (Int32)(time2 - time1));
        }
    }

    /// <summary>
    /// 绝对值;
    /// </summary>
    /// <param name="time1"></param>
    /// <param name="time2"></param>
    /// <returns></returns>
    public Int32 Abs_Second_U_U(UInt64 time1, UInt64 time2)
    {
        if (time1 > time2)
        {
            return (Int32)(time1 - time2);
        }
        else
        {
            return (Int32)(time2 - time1);
        }
    }

    public const Int32 minute = 60;
    public const Int64 hour = 60 * minute;
    public const Int64 day = 24 * hour;
    public const Int64 mouth = 31 * day;
    public const Int64 year = 12 * mouth;

    /// <summary>
    /// 将时间转换成固定格式 （X 分/天/月/… 前）
    /// </summary>
    /// <param name="diff">具体时间值</param>
    /// <returns></returns>
    public string ConvertSecToFormatBefore(Int32 diff)
    {
        if (diff > year)
            return string.Format("{0}年前", (diff / year));
        if (diff > mouth)
            return string.Format("{0}月前", (diff / mouth));
        if (diff > day)
            return string.Format("{0}天前", (diff / day));
        if (diff > hour)
            return string.Format("{0}小时前", (diff / hour));
        if (diff > minute)
            return string.Format("{0}分钟前", (diff / minute));

        return "刚刚";
    }

    /// <summary>
    /// 服务器同步时间;
    /// </summary>
    /// <param name="serverCurrMilliSecond">服务器当前tick(毫秒);</param>
    public void OnServerMilliSecond(string serverCurrMilliSecond)
    {
        m_serverCurrMilliSecond = UInt64.Parse(serverCurrMilliSecond);
        m_clientTime = GetNowClinetMilliSecond();
    }

    //Sunday = 0, Monday = 1, Tuesday = 2,Wednesday 3, Thursday 4,Friday 5,Saturday = 6
    public int GetWeekDayByCurSecond(int seconds)
    {
        DateTime format = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); //设置时区

        int d = Convert.ToInt32(format.AddSeconds(Mathf.Clamp(seconds, 0, seconds)).DayOfWeek);
        // 处理为：Monday = 1, Tuesday = 2,Wednesday 3, Thursday 4,Friday 5,Saturday = 6， Sunday = 0
        d = d == 0 ? 7 : d;
        return d;
    }

    public string ConvertSecToStringAuto(int seconds)
    {
        if (seconds < 60)
        {
            return seconds.ToString();
        }
        else if (seconds < 3600)
        {
            return ConvertSecToStringMMSS(seconds);
        }
        else
        {
            return ConvertSecToStringHHMMSS(seconds);
        }
    }

    public string ConvertSecToStringMMSS(int seconds)
    {
        DateTime format = new DateTime();

        string str = format.AddSeconds(Mathf.Clamp(seconds, 0, seconds)).ToString("mm:ss");

        return str;
    }

    public string ConvertSecToStringHHMMSS(int seconds)
    {
        DateTime format = new DateTime();

        string str = format.AddSeconds(Mathf.Clamp(seconds, 0, seconds)).ToString("HH:mm:ss");

        return str;
    }

    public string ConvertSecToStringFormat(int seconds, string Secformat)
    {
        DateTime format = new DateTime();

        string str = format.AddSeconds(Mathf.Clamp(seconds, 0, seconds)).ToString(Secformat);

        return str;
    }

    public string ConvertSecToStringInTimeZone(int seconds, string DataFormat)
    {
        DateTime format = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); //设置时区

        string str = format.AddSeconds(Mathf.Clamp(seconds, 0, seconds)).ToString(DataFormat);//"HH:mm:ss"

        return str;
    }


    public string ConvertSecToStringMMDDHHMM(int seconds)
    {
        DateTime format = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); //设置时区

        string str = format.AddSeconds(Mathf.Clamp(seconds, 0, seconds)).ToString("MM-dd HH:mm");

        return str;
    }

    public string ConvertSecToStringYYMMDDHHMM(int seconds)
    {
        DateTime format = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); //设置时区

        string str = format.AddSeconds(Mathf.Clamp(seconds, 0, seconds)).ToString("yyyy-MM-dd HH:mm");

        return str;
    }

    public string ConvertSecToStringMMDD(int seconds)
    {
        DateTime format = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); //设置时区

        string str = format.AddSeconds(Mathf.Clamp(seconds, 0, seconds)).ToString("MM.dd");

        return str;
    }

    public string ConvertSecToLongDataString(int seconds)
    {
        DateTime format = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); //设置时区

        string str = format.AddSeconds(Mathf.Clamp(seconds, 0, seconds)).ToString("yyyy年MM月dd日");

        return str;
    }

    public string ConvertSecToAUTODataString(int seconds)
    {
        //            DateTime format = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); //设置时区
        DateTime format = new DateTime();
        string str = "";
        if (seconds < 60)
        {
            str = format.AddSeconds(Mathf.Clamp(seconds, 0, seconds)).ToString("ss秒");
        }
        else if (seconds < 3600)
        {
            str = format.AddSeconds(Mathf.Clamp(seconds, 0, seconds)).ToString("mm分ss秒");
        }
        else
        {
            str = format.AddSeconds(Mathf.Clamp(seconds, 0, seconds)).ToString("HH小时mm分");
        }


        return str;
    }

    public string GetDateTimeStrYYMMDDHHMM()
    {
        return System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }

    public UInt64 ConvertSecFromData(int year, int month, int day, int hour, int minute, int second)
    {
        DateTime formatStart = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); //设置时区
        DateTime format = new DateTime(year, month, day, hour, minute, second);
        UInt64 stamp = Convert.ToUInt64((format - formatStart).TotalSeconds);
        return stamp;
    }

    public UInt64 StringDateToStamp(string format, string dateString)
    {
        DateTime dt;
        System.Globalization.DateTimeFormatInfo dtFormat = new System.Globalization.DateTimeFormatInfo();
        dtFormat.ShortDatePattern = format;
        dt = Convert.ToDateTime(dateString, dtFormat);
        return ConvertSecFromData(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);
    }

    #endregion 时间转换相关的函数;

    private int m_delayCount = 0;

    void Update()
    {
        if (UpdateEvent != null) { UpdateEvent(); }

        m_delayCount = m_delayCount + 1;
        if (m_delayCount >= 2)
        {
            m_delayCount = 0;
            for (int i = 0, count = m_delayList.Count; i < count; ++i)
            {
                if (Time.time >= m_delayList[i].finishTime)
                {
                    m_finishDelayList.Add(m_delayList[i]);
                }
            }

            for (int i = 0, count = m_finishDelayList.Count; i < count; ++i)
            {
                Delay delay = m_finishDelayList[i];
                if (delay.callBack != null)
                {
                    try
                    {
                        delay.callBack(delay.parameter);
                    }
                    catch (Exception e)
                    {
                        Debugger.LogError(e);
                    }
                }

                LuaFunction luaFunc = delay.luaFunc;
                if (luaFunc != null)
                {
                    object parameter = delay.parameter;
                    if (parameter != null)
                    {
                        luaFunc.Call(parameter);
                    }
                    else
                    {
                        luaFunc.Call();
                    }

                    luaFunc.Dispose();

                    delay.luaFunc = null;
                }

                m_delayList.Remove(delay);

                StoreDelay(delay);

                m_finishDelayList[i] = null;
            }

            if (m_finishDelayList.Count > 0)
            {
                m_finishDelayList.Clear();
            }
        }

        //for (int i = 0, count = m_timerList.Count; i < count; ++i)
        //{
        //    if (Time.time >= m_delayList[i].finishTime)
        //    {
        //        m_finishDelayList.Add(m_delayList[i]);
        //    }
        //}
    }

    #region 供lua操作的函数接口;

    public void AddToUpdate(Action callback) { UpdateEvent += callback; }
    public void RemoveFromUpdate(Action callback) { UpdateEvent -= callback; }

    #endregion 供lua操作的函数接口;

    /// <summary>
    /// 注意:仅供C#使用;
    /// </summary>
    /// <param name="delayTime"></param>
    /// <param name="parameter"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    public UInt32 StartDelay(float delayTime, object parameter, Action<object> callback)
    {

        delayTime = (delayTime < 0) ? 0 : (delayTime);
        if (delayTime <= 0)
        {
            if (callback != null)
            {
                callback(parameter);
            }
            return 0;
        }

        Delay delay = NewDelay();

        delay.callBack = callback;
        delay.parameter = parameter;
        delay.finishTime = Time.time + delayTime;

        uint id = delay.GetId();

        lock (m_delayList)
        {
            m_delayList.Add(delay);
        }

        return id;
    }

    public UInt32 StartDelayByLua(float delayTime, object parameter, LuaFunction luaFunc)
    {
        delayTime = (delayTime < 0) ? 0 : (delayTime);

        if (delayTime <= 0)
        {
            if (luaFunc != null)
            {
                if (parameter != null)
                {
                    luaFunc.Call(parameter);
                }
                else
                {
                    luaFunc.Call();
                }

                luaFunc.Dispose();
            }
            return 0;
        }

        Delay delay = NewDelay();
        delay.luaFunc = luaFunc;
        delay.parameter = parameter;
        delay.finishTime = Time.time + delayTime;

        uint id = delay.GetId();

        lock (m_delayList)
        {
            m_delayList.Add(delay);
        }

        return delay.GetId();
    }


    public void StopDelay(UInt32 id)
    {
        lock (m_delayList)
        {
            Delay delay = null;
            for (int count = m_delayList.Count, i = count - 1; i >= 0; --i)
            {
                delay = m_delayList[i];
                if (delay.GetId() == id)
                {
                    StoreDelay(delay);

                    m_delayList.RemoveAt(i);
                    break;
                }
            }
        }
    }


    public const int MAX_THREADS_NUM = 8;
    private int m_numThreads = 0;

    public void Run(Action callback)
    {
        while (m_numThreads >= MAX_THREADS_NUM)
        {
            Thread.Sleep(1);
        }

        Interlocked.Increment(ref m_numThreads);
        ThreadPool.QueueUserWorkItem((object state) =>
        {
            try
            {
                (state as Action)();
            }
            catch
            {
            }
            finally
            {
                Interlocked.Decrement(ref m_numThreads);
            }
        }, callback);
    }
}

internal class Delay
{
    private static UInt32 s_id = 0;
    private UInt32 m_id = 0;
    public Action<object> callBack = null;
    public float finishTime = 0.0f;
    public object parameter;
    public LuaFunction luaFunc = null;
    public bool isStored = true;

    public UInt32 GenerateId()
    {
        if (s_id == UInt32.MaxValue) { s_id = 0; }

        s_id++;

        m_id = s_id;

        return m_id;
    }

    public UInt32 GetId() { return m_id; }

    public void Clear()
    {
        callBack = null;
        parameter = null;
        luaFunc = null;
    }
}

