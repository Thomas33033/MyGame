using System;
using UnityEngine;
using LuaInterface;

public class LuaSpaceComponent : MonoBehaviour
{
    private LuaTable _luaTable;
    private bool _enableUpdate;
    private LuaFunction _luaFunction;

    void Start()
    {
        _luaTable.Call("Start", this._luaTable);
    }

    private void Update()
    {
        if (_enableUpdate && Time.frameCount % 10 == 0)
        {
            _luaTable.Call("Update", _luaTable);
        }
    }

    public void DelayCall(float time, LuaFunction luaFunction)
    {
        _luaFunction = luaFunction;
        Invoke("DoDelayCall", time);
    }

    void DoDelayCall()
    {
        if (_luaFunction != null)
        {
            LuaFunction luaFunction = _luaFunction;
            _luaFunction = null;
            luaFunction.Call();
        }
    }

    private void OnDestroy()
    {
        _luaFunction = null;
        _luaTable = null;
    }

    public void SetData(LuaTable v)
    {
        _luaTable = v;
        _enableUpdate = _luaTable["Update"] != null;
    }

    internal void CallLua(string func, params object[] args)
    {
        object[] newArgs = new object[args.Length + 1];
        newArgs[0] = _luaTable;
        Array.Copy(args, 0, newArgs, 1, args.Length);

        if (newArgs.Length == 1)
        {
            _luaTable.Call(func, newArgs[1]);
        }
        else if (newArgs.Length == 2)
        {
            _luaTable.Call(func, newArgs[1], newArgs[2]);
        }
        else if (newArgs.Length == 2)
        {
             _luaTable.Call(func, newArgs[1], newArgs[2], newArgs[3]);
        }
        else
        {
            _luaTable.Call(func);
        }
        
    }
}
