using UnityEngine;
using System.Collections;
using LuaInterface;


public sealed class LuaManager : MonoSingleton<LuaManager>
{
    public LuaState lua = null;
    private LuaLooper loop = null;

    public static bool isLuaInitializeFinished = false;

    public void Start()
    {
        LuaGC();
        Close();

        lua = new LuaState();
        this.OpenLibs();
        lua.LuaSetTop(0);

        LuaBinder.Bind(lua);
        DelegateFactory.Init();
        LuaCoroutine.Register(lua, this);

        Initialize();
    }

    public void Initialize()
    {
        this.lua.Start();    //启动LUAVM
        this.StartMain();
        this.StartLooper();

        isLuaInitializeFinished = true;
    }

    void StartLooper()
    {
        loop = gameObject.GetComponent<LuaLooper>();
        if (loop == null)
        {
            loop = gameObject.AddComponent<LuaLooper>();
            loop.luaState = lua;
        }
    }

    void StartMain()
    {
        Debug.LogError("---------------");
        lua.DoFile("Main.lua");

        LuaFunction main = lua.GetFunction("Main.Awake");
        main.Call();
        main.Dispose();
        main = null;
    }

    /// <summary>
    /// 初始化加载第三方库
    /// </summary>
    void OpenLibs()
    {
        lua.OpenLibs(LuaDLL.luaopen_pb_io);
        lua.OpenLibs(LuaDLL.luaopen_pb_conv);
        lua.OpenLibs(LuaDLL.luaopen_pb_buffer);
        lua.OpenLibs(LuaDLL.luaopen_pb_slice);
        lua.OpenLibs(LuaDLL.luaopen_pb);
        lua.OpenLibs(LuaDLL.luaopen_lpeg);
        lua.OpenLibs(LuaDLL.luaopen_bit);
        lua.OpenLibs(LuaDLL.luaopen_socket_core);

        lua.LuaGetField(LuaIndexes.LUA_REGISTRYINDEX, "_LOADED");
        lua.OpenLibs(LuaDLL.luaopen_cjson);
        lua.LuaSetField(-2, "cjson");
    }

    public void DoFile(string filename)
    {
        lua.DoFile(filename);
    }

    public void CallFunction(string funcName)
    {
        LuaFunction func = lua.GetFunction(funcName);
        if (func != null)
        {
            func.Call();
        }
    }

    public void CallFunction<T1>(string funcName, T1 arg1)
    {
        LuaFunction func = lua.GetFunction(funcName);
        if (func != null)
        {
            func.Call(arg1);
        }
    }

    public void CallFunction<T1, T2>(string funcName, T1 arg1, T2 arg2)
    {
        LuaFunction func = lua.GetFunction(funcName);
        if (func != null)
        {
            func.Call(arg1, arg2);
        }
    }

    public void CallFunction<T1, T2, T3>(string funcName, T1 arg1, T2 arg2, T3 arg3)
    {
        LuaFunction func = lua.GetFunction(funcName);
        if (func != null)
        {
            func.Call(arg1, arg2, arg3);
        }
    }

    public void CallFunction<T1, T2, T3, T4>(string funcName, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
    {
        LuaFunction func = lua.GetFunction(funcName);
        if (func != null)
        {
            func.Call(arg1, arg2, arg3, arg4);
        }
    }

    public void CallFunction<T1, T2, T3, T4, T5>(string funcName, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
    {
        LuaFunction func = lua.GetFunction(funcName);
        if (func != null)
        {
            func.Call(arg1, arg2, arg3, arg4, arg5);
        }
    }

    public void CallFunction<T1, T2, T3, T4, T5, T6>(string funcName, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
    {
        LuaFunction func = lua.GetFunction(funcName);
        if (func != null)
        {
            func.Call(arg1, arg2, arg3, arg4, arg5, arg6);
        }
    }


    public void CallFunction<T1, T2, T3, T4, T5, T6, T7>(string funcName, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
    {
        LuaFunction func = lua.GetFunction(funcName);
        if (func != null)
        {
            func.Call(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }
    }

    public void CallFunction<T1, T2, T3, T4, T5, T6, T7, T8>(string funcName, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
    {
        LuaFunction func = lua.GetFunction(funcName);
        if (func != null)
        {
            func.Call(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }
    }

    public void CallFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9>(string funcName, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
    {
        LuaFunction func = lua.GetFunction(funcName);
        if (func != null)
        {
            func.Call(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
        }
    }

    //// Update is called once per frame
    //public object[] CallFunction(string funcName, params object[] args)
    //{
    //    LuaFunction func = lua.GetFunction(funcName);
    //    if (func != null) {
    //        return func.Call(args);
    //    }
    //    return null;
    //}

    public void LuaGC()
    {
        if (lua != null)
        {
            lua.LuaGC(LuaGCOptions.LUA_GCCOLLECT);
        }
    }

    public void Close()
    {
        if (loop != null)
        {
            loop.Destroy();
            loop = null;
        }

        if (lua != null)
        {
            lua.Dispose();
            lua = null;
        }
    }

    void OnDestroy()
    {
        Close();
    }
}
