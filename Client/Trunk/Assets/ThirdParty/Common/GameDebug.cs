using UnityEngine;
using System.Collections;
using System;
using System.Threading;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace Cherry
{

    public enum GameLogType
    {
        Default = 0,            //默认
        Launch = 1,             //启动
        Patch = 2,              //热更
        AssetBundleLoad = 3,    //加载资源
        ResourceLoad = 4,       //加载资源
        Network = 5,            //网络
        Lua = 6,                //lua
    }

    public class GameDebug
    {
        public delegate void VoidDelegate();
        public static VoidDelegate OnOpenUICallBack;
        public delegate void LogCallBackDelegate(LogLevel level, string name, string info, string tarceStack);
        public static LogCallBackDelegate OnLogCallBack;

        public static bool AllDisable = false;
        public static bool ShowGUIlogButton = false;

        public static LogLevel FileLogLevel = LogLevel.debug;
        public static LogLevel ConsoleLogLevel = LogLevel.debug;
        public static LogLevel GuiLogLevel = LogLevel.debug;
        public static LogLevel BuglyLogLevel = LogLevel.debug;

        private static bool _showGuilogWindow = false;
        private static float _heightAmount = 0.60f;
        private static int _guiMaxCount = 100;

        private static string _logContent = string.Empty;
        private static List<string> _guiLoglist = new List<string>();

        private static StringBuilder _logSB = new StringBuilder();
        private static StringBuilder _guiSB = new StringBuilder();

        private static Vector2 _srcollPos = Vector2.zero;
        private static Dictionary<string, bool> _logTypeDic = new Dictionary<string, bool>();
        private static float _runningTime = 0;
        private static bool _openOtherUI = false;
        private static string _logPath = string.Empty;
        private static object m_lock = new object();
        private static string CurrentTime
        {
            get
            {
                if (GameDebug.AllDisable)
                {
                    return string.Empty;
                }
                return DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
            }
        }

        /// <summary>
        /// 设置日志文件路径
        /// </summary>
        /// <param name="file"></param>
        public static void SetLogFile(string file)
        {
            GameDebug._logPath = file + "/logs/";
        }
        /// <summary>
        /// 初始化Log开关
        /// </summary>
        /// <param name="args"></param>
        public static void SetLogType(params object[] args)
        {
            _logTypeDic.Clear();
            if (args == null || args.Length < 1)
                return;
            for (int i = 0; i < args.Length; i++)
            {
                string openType = args[i].ToString();
                if (!_logTypeDic.ContainsKey(openType))
                    _logTypeDic.Add(openType, true);
            }
        }
        /// <summary>
        /// 扩展Debug.Log
        /// </summary>
        /// <param name="sourceObj">脚本名</param>
        /// <param name="str">日志内容</param>
        /// <param name="type">日志功能类型 默认是Default</param>
        public static void Log(object sourceObj, object type, object str)
        {
            if (AllDisable)
                return;

            if ((int)ConsoleLogLevel > (int)LogLevel.debug
                && (int)GuiLogLevel > (int)LogLevel.debug
                && (int)FileLogLevel > (int)LogLevel.debug
                && (int)BuglyLogLevel > (int)LogLevel.debug)
                return;

            if (!IsOpen(type.ToString()))
                return;

            string source = GetSourceName(sourceObj);

            _logSB.Length = 0;

            _logSB.Append(" Log ------ Soure:");
            _logSB.Append(source);
            _logSB.Append("; Type:");
            _logSB.Append(type);
            _logSB.Append("; Info:");
            _logSB.Append(str.ToString());

            _logContent = _logSB.ToString();

            if ((int)ConsoleLogLevel <= (int)LogLevel.debug)
                Debug.Log(_logContent);

            if ((int)GuiLogLevel <= (int)LogLevel.debug)
                GuiLog(_logContent);

            if ((int)FileLogLevel <= (int)LogLevel.debug)
            {
                string strFile = CurrentTime.ToString() + _logContent;
                SaveLog(strFile);
            }
            if ((int)BuglyLogLevel <= (int)LogLevel.debug)
            {
                if (OnLogCallBack != null)
                    OnLogCallBack(LogLevel.debug, "FFDebugLog", _logContent, string.Empty);
            }
        }
        /// <summary>
        /// LogWarning 
        /// </summary>
        /// <param name="sourceObj">脚本名</param>
        /// <param name="str">内容</param>
        public static void LogWarning(object sourceObj, object str)
        {
            if (AllDisable) return;

            if ((int)ConsoleLogLevel > (int)LogLevel.Warning
                && (int)GuiLogLevel > (int)LogLevel.Warning
                && (int)FileLogLevel > (int)LogLevel.Warning
                && (int)BuglyLogLevel > (int)LogLevel.Warning)
                return;

            string source = GetSourceName(sourceObj);

            _logSB.Length = 0;

            _logSB.Append(" Warn ------ Soure:");
            _logSB.Append(source);
            _logSB.Append("; Info:");
            _logSB.Append(str.ToString());

            _logContent = _logSB.ToString();

            if ((int)ConsoleLogLevel <= (int)LogLevel.Warning)
                Debug.LogWarning(_logContent);

            if ((int)GuiLogLevel <= (int)LogLevel.Warning)
                GuiLog(_logContent);

            if ((int)FileLogLevel <= (int)LogLevel.Warning)
            {
                string strFile = CurrentTime.ToString() + _logContent;
                SaveLog(strFile);
            }

            if ((int)BuglyLogLevel <= (int)LogLevel.Warning)
            {
                if (OnLogCallBack != null)
                    OnLogCallBack(LogLevel.Warning, "FFDebugWarning", _logContent, string.Empty);
            }
        }
        /// <summary>
        /// LogError
        /// </summary>
        /// <param name="sourceObj">脚本名</param>
        /// <param name="str">内容</param>
        public static void LogError(object sourceObj, object str)
        {
            if (AllDisable) return;

            if ((int)ConsoleLogLevel > (int)LogLevel.Error
                && (int)GuiLogLevel > (int)LogLevel.Error
                && (int)FileLogLevel > (int)LogLevel.Error
                && (int)BuglyLogLevel > (int)LogLevel.Error)
                return;

            string source = GetSourceName(sourceObj);

            _logSB.Length = 0;

            _logSB.Append(" Error ------ Soure:");
            _logSB.Append(source);
            _logSB.Append("; Info:");
            _logSB.Append(str.ToString());

            string content = _logSB.ToString();

            if ((int)ConsoleLogLevel <= (int)LogLevel.Error)
                Debug.LogError(content);

            if ((int)GuiLogLevel <= (int)LogLevel.Error)
                GuiLog(content);

            if ((int)FileLogLevel <= (int)LogLevel.Error)
            {
                string strFile = CurrentTime.ToString() + _logContent;
                SaveLog(strFile);
            }

            if ((int)BuglyLogLevel <= (int)LogLevel.Error)
            {
                if (OnLogCallBack != null)
                    OnLogCallBack(LogLevel.Error, "FFDebugError", content, string.Empty);
            }

        }
        /// <summary>
        /// 捕捉系统的错误 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="traceStack"></param>
        /// <param name="type"></param>
        public static void CatchExceptionHandler(string message, string traceStack, LogType type)
        {
            if (AllDisable) return;

            if (type == LogType.Log || type == LogType.Warning)
                return;

            _logSB.Length = 0;

            _logSB.AppendFormat(" {0}", type.ToString());
            _logSB.Append(" ------ Source:Unity; Info:");
            _logSB.Append(message);
            _logSB.Append(traceStack);

            _logContent = _logSB.ToString();

            if ((int)GuiLogLevel <= (int)LogLevel.Exception)
                GuiLog(_logContent);

            if ((int)FileLogLevel <= (int)LogLevel.Exception)
            {
                string strFile = CurrentTime.ToString() + _logContent;
                SaveLog(strFile);
            }
        }



        private static void GuiLog(string logstr)
        {
            _guiLoglist.Add(logstr);
            if (_guiLoglist.Count > _guiMaxCount)
            {
                _guiLoglist.RemoveAt(0);
            }
        }


        private static void SaveLog(string text)
        {

            if (string.IsNullOrEmpty(_logPath)) return;
            object @lock;
            Monitor.Enter(@lock = GameDebug.m_lock);
            try
            {
                if (!Directory.Exists(GameDebug._logPath))
                {
                    Directory.CreateDirectory(GameDebug._logPath);
                }

                string path = GameDebug._logPath + "cfg.txt";
                string text2 = string.Empty;
                if (File.Exists(path))
                {
                    text2 = File.ReadAllText(path);
                    if (File.Exists(text2))
                    {
                        FileInfo fileInfo = new FileInfo(text2);
                        if (fileInfo.Length >= 104857600L)
                        {
                            text2 = GameDebug._logPath + DateTime.Now.ToString("yyyyMMddhhmmss") + ".txt";
                            File.WriteAllText(path, text2);
                        }
                    }
                    else
                    {
                        text2 = GameDebug._logPath + DateTime.Now.ToString("yyyyMMddhhmmss") + ".txt";
                        File.WriteAllText(path, text2);
                    }
                }
                else
                {
                    text2 = GameDebug._logPath + DateTime.Now.ToString("yyyyMMddhhmmss") + ".txt";
                    File.WriteAllText(path, text2);
                }
                using (FileStream fileStream = new FileStream(text2, FileMode.Append))
                {
                    using (StreamWriter streamWriter = new StreamWriter(fileStream))
                    {
                        streamWriter.AutoFlush = true;
                        streamWriter.WriteLine(text);
                    }
                }
            }
            finally
            {
                Monitor.Exit(@lock);
            }
        }

        private static string GetLogString()
        {
            if (_guiLoglist == null || _guiLoglist.Count < 1)
                return string.Empty;
            _guiSB.Length = 0;
            for (int i = 0; i < _guiLoglist.Count; i++)
                _guiSB.AppendLine(_guiLoglist[i]);
            return _guiSB.ToString();
        }

        private static string GetSourceName(object sourceObj)
        {
            string source = string.Empty;
            if (sourceObj == null)
                source = "null";
            else
                source = sourceObj.ToString();

            if (string.IsNullOrEmpty(source))
                source = "unknow";
            return source;
        }

        private static bool IsOpen(string type)
        {
            bool isOpen = false;

            if (AllDisable)
                return isOpen;

            if (_logTypeDic == null || _logTypeDic.Count < 1)
                return isOpen;

            if (_logTypeDic.ContainsKey(type))
                isOpen = true;

            return isOpen;
        }

        public enum LogLevel
        {
            debug = 1,
            Warning = 2,
            Error = 3,
            Exception = 4,
            Disable = 5,
        }

    }
}
