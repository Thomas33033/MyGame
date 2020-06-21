﻿//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class Singleton_UIManagerWrap
{
	public static void Register(LuaState L)
	{
		L.BeginClass(typeof(Singleton<UIManager>), typeof(System.Object), "Singleton_UIManager");
		L.RegFunction("OnCreate", OnCreate);
		L.RegFunction("__tostring", ToLua.op_ToString);
		L.RegVar("curInstanceList", get_curInstanceList, set_curInstanceList);
		L.RegVar("Instance", get_Instance, null);
		L.EndClass();
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int OnCreate(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			Singleton<UIManager> obj = (Singleton<UIManager>)ToLua.CheckObject<Singleton<UIManager>>(L, 1);
			obj.OnCreate();
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_curInstanceList(IntPtr L)
	{
		try
		{
			ToLua.PushSealed(L, Singleton<UIManager>.curInstanceList);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_Instance(IntPtr L)
	{
		try
		{
			ToLua.PushObject(L, Singleton<UIManager>.Instance);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_curInstanceList(IntPtr L)
	{
		try
		{
			System.Collections.Generic.List<UIManager> arg0 = (System.Collections.Generic.List<UIManager>)ToLua.CheckObject(L, 2, typeof(System.Collections.Generic.List<UIManager>));
			Singleton<UIManager>.curInstanceList = arg0;
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}
}

