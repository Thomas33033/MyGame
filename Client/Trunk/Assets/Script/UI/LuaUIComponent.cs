using UnityEngine;
using SLua;
using System.Collections.Generic;
using System;

[CustomLuaClass]
public class LuaUIComponent : MonoBehaviour
{
    private LuaTable _luaTable;
    private bool _enableUpdate;
    private List<DelayCall> listDelayCall;

    private void Awake()
    {
        listDelayCall = new List<DelayCall>();

        RectTransform rt = this.GetComponent<RectTransform>();
        float x = Screen.safeArea.x;
        float y = Screen.safeArea.y;
#if UNITY_EDITOR
        x = 50;
#endif

        rt.offsetMin = new Vector2(x,0);
        rt.offsetMax = new Vector2(y,0);
    }

    void Start()
    {
        AppBoot.instance.slua.UIStart(_luaTable);
    }

    private void Update()
    {
        if(_enableUpdate && Time.frameCount % 10 == 0)
        {
            _luaTable.invoke("Update", _luaTable);
        }
        for (int i = listDelayCall.Count - 1; i > -1; i--)
        {
            if(listDelayCall[i].time < Time.time)
            {
                listDelayCall[i].action();
                listDelayCall.RemoveAt(i);
            }
        }
    }

    public void DelayCall(float time,LuaFunction luaFunction)
    {
        listDelayCall.Add(new DelayCall(){time= Time.time + time,action= () => {
            luaFunction.call();
        } });
    }

    private void OnDestroy()
    {
        _luaTable = null;
        listDelayCall.Clear();
    }

    public void SetData(LuaTable v)
    {
        _luaTable = v;
        _enableUpdate = _luaTable["Update"] != null;
    }

    internal object CallLua(string func, params object[] args)
    {
        object[] newArgs = new object[args.Length+1];
        newArgs[0] = _luaTable;
        Array.Copy(args, 0, newArgs, 1, args.Length);
        return _luaTable.invoke(func, newArgs);
    }
}
