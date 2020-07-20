using UnityEngine;
using System.Collections.Generic;
using System;
using LuaInterface;

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
        LuaManager.Instance.CallFunction("UI.StartUI", this._luaTable);
    }

    private void Update()
    {
        if(_enableUpdate && Time.frameCount % 10 == 0)
        {
            _luaTable.Call("Update",this._luaTable);
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
            luaFunction.Call();
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
        _enableUpdate = _luaTable.GetLuaFunction("Update") != null;
    }
}
