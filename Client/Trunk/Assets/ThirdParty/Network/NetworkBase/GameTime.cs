using System;
using Net;
using UnityEngine;
using System.Text;

public class GameTime : MonoSingleton<GameTime>
{

    private UInt64 m_qwClientMsec = 0;
    private DateTime startTime = new System.DateTime(1970, 1, 1).ToLocalTime();
    private static ulong lastTime = 0;

    public ulong GetTimeInterval()
    {
        ulong qwNow = GetNowMsecond();
        ulong Interval = qwNow - lastTime;
        lastTime = qwNow;
        return Interval;
    }

    public UInt64 GetNowMsecond()
    {
        UInt64 qwResult = 0;
        qwResult = (UInt64)(System.DateTime.Now - startTime).TotalMilliseconds;
        return qwResult;
    }

    public double GetNowMsecondAsDouble()
    {
        return (UInt64)(System.DateTime.Now - startTime).TotalMilliseconds;
    }


    /// <summary>
    /// 刷新客户端时间
    /// </summary>
    private void UpdateNowMsecond()
    {
        if (0 == m_qwClientMsec)
        {
            m_qwClientMsec = GetNowMsecond();
        }
    }

    /// <summary>
    /// 获取间隔毫秒数
    /// </summary>
    /// <returns></returns>
    public UInt32 GetIntervalMsecond()
    {
        UInt64 qwNow = GetNowMsecond();
        if (qwNow >= m_qwClientMsec)
        {
            UInt64 qwInterval = qwNow - m_qwClientMsec;

            return (UInt32)(qwInterval);
        }

        return 0;
    }

    private UInt32 m_dwServerTime = 0;
    private UInt32 m_dwClientTime = 0;
    private UInt64 m_dwServerStartTime = 0;
    private HeartBeatState heartBeatState = HeartBeatState.HeartBeatException;


    /// <summary>
    /// 获得服务器时间
    /// </summary>
    /// <returns></returns>
    public UInt32 GetServerTime()
    {
        UInt32 dwCltNowSecond = GetNowSecond();

        if (dwCltNowSecond >= m_dwClientTime)
        {
            return dwCltNowSecond - m_dwClientTime + m_dwServerTime;
        }

        return m_dwServerTime;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    private UInt32 ConvertDateTimeInt(System.DateTime time)
    {
        UInt32 dwResult = 0;
        dwResult = (UInt32)(time - startTime).TotalSeconds;
        return dwResult;
    }

    public UInt32 GetNowSecond()
    {
        return ConvertDateTimeInt(System.DateTime.Now);
    }

    public void Init()
    {
    }

    public readonly float HeartBeatInterval = 10f;  //心跳包间隔 秒 这个是检测miss的间隔
    private int missHeartBeatTimes = 0;
    private float tempTime = 0f;

    long ServertimeOffset = 0;

    /// <summary>
    /// 设置当前服务器时间
    /// </summary>
    /// <param name="servertime"></param>
    public void SetServerTime(ulong servertime)
    {
        ServertimeOffset = (long)servertime - (long)GetNowMsecond();
    }

    /// <summary>
    /// 获得当前服务器时间（毫秒）
    /// </summary>
    /// <returns></returns>
    public ulong GetCurrServerTime()
    {
        long NowClient = (long)GetNowMsecond();
        return (ulong)(NowClient + ServertimeOffset);
    }

    /// <summary>
    /// 获得当前服务器时间（秒）
    /// </summary>
    /// <returns></returns>
    public ulong GetCurrServerTimeBySecond()
    {
        long NowClient = (long)GetNowMsecond();
        ulong time = (ulong)(NowClient + ServertimeOffset);
        return (time / 1000);
    }

    /// <summary>
    /// 获得当前服务器时间（字符串）
    /// </summary>
    /// <param name="format"></param>
    /// <returns></returns>
    public string GetCurrServerTimeString(string format = "")
    {
        DateTime _time = startTime + TimeSpan.FromMilliseconds((long)GetCurrServerTime());
        if (string.IsNullOrEmpty(format))
            format = "yyyy-MM-dd HH:mm:ss";
        return _time.ToString(format);
    }

    public string GetTimeBySecond(UInt64 time)
    {
        StringBuilder tmpTime = new StringBuilder();
        if (time >= 86400)
        {
            tmpTime.Append((time / 86400).ToString());
            tmpTime.Append("天");
            time = time % 86400;
            tmpTime.Append((time / 3600).ToString());
            tmpTime.Append("小时");
        }
        else if (time < 86400 && time >= 3600)
        {
            tmpTime.Append((time / 3600).ToString());
            tmpTime.Append("小时");
            time = time % 3600;
            tmpTime.Append((time / 60).ToString());
            tmpTime.Append("分钟");
        }
        else
        {
            tmpTime.Append((time / 60).ToString());
            tmpTime.Append("分钟");
        }

        return tmpTime.ToString();
    }
    /// <summary>
    /// 按秒显示时间，大于一天只显示天数，小于一天按小时向上取整
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    public string GetTimeDayBySecond(UInt64 time)
    {
        StringBuilder tmpTime = new StringBuilder();
        if (time >= 82800)
        {
            tmpTime.Append(Mathf.CeilToInt((float)time / 86400).ToString());
            tmpTime.Append("天");
        }
        else if (time < 82800)
        {
            tmpTime.Append(Mathf.CeilToInt((float)time / 3600).ToString());
            tmpTime.Append("小时");
        }

        return tmpTime.ToString();
    }

    //传入时间单位 秒
    public string GetTimeString(UInt64 time)
    {
        StringBuilder tmpTime = new StringBuilder();

        if (time >= 86400)
        {
            tmpTime.Append((time / 86400).ToString());
            tmpTime.Append("天");
            time = time % 86400;
        }
        if (time >= 3600)
        {
            tmpTime.Append((time / 3600).ToString());
            tmpTime.Append("时");
            time = time % 3600;
        }
        if (time >= 60)
        {
            tmpTime.Append((time / 60).ToString());
            tmpTime.Append("分");
            time = time % 60;
        }
        if (time > 0)
        {
            tmpTime.Append((time).ToString());
            tmpTime.Append("秒");
        }

        return tmpTime.ToString();
    }

    //传入时间单位 秒  超过一天则不显示秒 黛眉规则
    public string GetTimeStringEx(UInt64 time)
    {
        StringBuilder tmpTime = new StringBuilder();
        UInt64 temp = time;

        if (temp >= 86400)
        {
            tmpTime.Append((temp / 86400).ToString());
            tmpTime.Append("天");
            temp = temp % 86400;
        }
        if (temp >= 3600)
        {
            tmpTime.Append((temp / 3600).ToString());
            tmpTime.Append("时");
            temp = temp % 3600;
        }
        if (temp >= 60)
        {
            tmpTime.Append((temp / 60).ToString());
            tmpTime.Append("分");
            temp = temp % 60;
        }
        if (temp > 0 && time < 86400)
        {
            tmpTime.Append((temp).ToString());
            tmpTime.Append("秒");
        }

        return tmpTime.ToString();
    }


    public string ConvertTimeTo_H_m_s(int time)
    {
        StringBuilder tmpTime = new StringBuilder();
        if (time < 0)
        {
            time = 0;
        }

        bool bHasHour = false;
        if (time >= 3600)
        {
            tmpTime.Append(string.Format("{0:D2}", (time / 3600)).ToString());
            tmpTime.Append(":");
            bHasHour = true;
            time = time % 3600;
        }

        if (bHasHour || time >= 60)
        {
            tmpTime.Append(string.Format("{0:D2}", (time / 60)).ToString());
            tmpTime.Append(":");
            time = time % 60;
        }
        else
        {
            tmpTime.Append(string.Format("{0:D2}", "00"));
            tmpTime.Append(":");
        }
        tmpTime.Append(string.Format("{0:D2}", time).ToString());
        return tmpTime.ToString();
    }

    public string ConvertTimeTo_M_s_ms(float time)
    {
        StringBuilder tmpTime = new StringBuilder();
        if (time < 0)
        {
            time = 0;
        }

        bool bHasMinute = false;

        if (time >= 60)
        {
            tmpTime.Append(string.Format("{0:D2}", (int)(time / 60)).ToString());
            tmpTime.Append(":");
            bHasMinute = true;
            time = time % 60;
        }
        else
        {
            tmpTime.Append(string.Format("{0:D2}", "00"));
            tmpTime.Append(":");
        }
        if (bHasMinute || time >= 1)
        {
            tmpTime.Append(string.Format("{0:D2}", Mathf.FloorToInt(time)).ToString());
            tmpTime.Append(":");
            time = time % 1;
        }
        else
        {
            tmpTime.Append(string.Format("{0:D2}", "00"));
            tmpTime.Append(":");
        }
        tmpTime.Append(string.Format("{0:D2}", Mathf.FloorToInt(time * 100)).ToString());
        return tmpTime.ToString();
    }

    //限时活动中心用 传服务器时间戳 只显示时和分
    public string GetTimeByServerTimeSecond(UInt64 time)
    {
        string format = "";
        DateTime _time = startTime + TimeSpan.FromSeconds((long)time);
        if (string.IsNullOrEmpty(format))
            format = "HH:mm";
        return _time.ToString(format);
    }

    public uint GetDayBySecond(UInt64 time)
    {
        return (uint)(time / 86400);
    }
    public uint GetHorBySecond(UInt64 time)
    {
        time = time % 86400;
        return (uint)(time / 3600);
    }
    public uint GetMinBySecond(UInt64 time)
    {
        time = time % 3600;
        return (uint)Mathf.Clamp((time / 60), 1, 59);
    }

    public DateTime GetServerDateTimeByTimeStamp(ulong timestamp)
    {
        return startTime.AddMilliseconds(timestamp);
    }


    #region ping值计算

    public uint Ping = 9999;

    public ulong lastreqpingtime = 0;

    private float pinginterval = 10f;   //秒
    private float temppingtime = 999f;

    public bool CheckPing = false;

    public void ReqPing()
    {
        // NetWorkModule.Instance.Send(CommandID.MSG_Req_Ping_CS, new MSG_Req_Ping_CS());
        lastreqpingtime = GetCurrServerTime();
    }

    #endregion


    void update()
    {
        if (heartBeatState == HeartBeatState.HeartBeatException)
        {
            return;
        }
        if (NetWorkModule.Instance.MainSocket == null)
        {
            return;
        }
        //网关断了 还是要重连的 外网手机切到后台后 断网 再回前台 如果不重连 就没地方去连了
        //if (NetWorkModule.Instance.MainSocket.type == USocketType.Fir)
        //{
        //    return;
        //}
        //if (!NetWorkModule.Instance.MainSocket.GetSocketState())
        //{
        //    return;
        //}   

        if (CheckPing)
        {
            temppingtime += Time.deltaTime;
            if (temppingtime > pinginterval)
            {
                temppingtime = 0f;

                ReqPing();
            }
        }

        tempTime += Time.deltaTime;
        if (tempTime > HeartBeatInterval)
        {
            tempTime = 0f;
            missHeartBeatTimes++;

            if (missHeartBeatTimes >= 2)
            {
                heartBeatState = HeartBeatState.HeartBeatException;
                //NetWorkModule.Instance.Reconnect();
                //手机关掉wifi 一定时间过后 自动重连
                NetWorkModule.Instance.StartAutoReconnect();
            }
        }
    }

    bool m_bAppFocus = true;
    void OnApplicationFocus(bool bFocus)
    {
        // 当手机从后台切换到前台时, 发个消息给服务器保证连接有效
        if (!m_bAppFocus && bFocus)
        {
            ReqPing();
            AutoReconnect();
        }
        //从前台切换后台
        else if (m_bAppFocus && !bFocus)
        {
            m_bAutoReconnect = false;
        }
        m_bAppFocus = bFocus;
    }

    bool m_bAppPause = false;
    public ulong lastpausetime = 0;
    void OnApplicationPause(bool bPause)
    {
        // 当手机从后台切换到前台时, 发个消息给服务器保证连接有效
        if (m_bAppPause && !bPause)
        {
            ReqPing();
            AutoReconnect();
        }
        //从前台切换后台
        else if (!m_bAppPause && bPause)
        {
            m_bAutoReconnect = false;
        }
        m_bAppPause = bPause;
        if (m_bAppPause)
            lastpausetime = GetCurrServerTime();
    }

    bool m_bAutoReconnect = false;
    void AutoReconnect()
    {
        if (m_bAutoReconnect)
            return;

        m_bAutoReconnect = true;

#if UNITY_IPHONE || UNITY_ANDROID
        ulong nowtime = GetCurrServerTime();

        ulong disTime = nowtime - lastpausetime;

        if (lastpausetime == 0)
            return;

        if (disTime > 20 * 60 * 1000)
        {
            // TipsWindow.ShowWindow("连接服务器尝试失败，请重新登录游戏！");
            // ManagerCenter.Instance.GetManager<GameMainManager>().Logout();
        }
        else if (disTime > 60 * 1000)
        {
            ClearDataBeforeAutoReconnect();

            //打开lua的自动重连界面
            NetWorkModule.Instance.StartAutoReconnect();
        }
        else if (disTime > 10 * 1000)
        {
            ClearDataBeforeAutoReconnect();

            //超过10s重连，如果网关服务器已经掉线 会返回LoginRetCode.LOGIN_RETURN_GATEWAY_TOKEN_ERROR然后自动重连平台
            NetWorkModule.Instance.AutoReconnect();
        }
#endif
    }

#if UNITY_IPHONE || UNITY_ANDROID
    void ClearDataBeforeAutoReconnect()
    {
        // if (DropItemManager.Instance != null)
        //     DropItemManager.Instance.RemoveAllItems();
    }
#endif

    public static UInt32 ConverStringToServerTime(string time, string format)
    {
        System.DateTime tmp = System.DateTime.ParseExact(time, format, null);
        return (UInt32)(tmp - System.DateTime.MinValue).TotalSeconds;
    }
}

public enum HeartBeatState
{
    HeartBeatNormal,         //心跳正常
    HeartBeatException,      //异常
}