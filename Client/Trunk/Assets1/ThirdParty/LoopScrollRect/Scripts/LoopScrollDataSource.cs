using System;
using UnityEngine;
using System.Collections;
using LuaInterface;
namespace UnityEngine.UI
{
    public abstract class LoopScrollDataSource
    {
        public abstract void ProvideData(Transform transform, int idx);
    }

	public class LoopScrollSendIndexSource : LoopScrollDataSource
    {
        public Action<Transform, int> ScrollCellChangeEvent;
        public LuaFunction luaScrollCellChangeEvent;
        public LoopScrollSendIndexSource(){}

        public override void ProvideData(Transform transform, int idx)
        {
            if (ScrollCellChangeEvent != null) ScrollCellChangeEvent.Invoke(transform, idx);
            if (luaScrollCellChangeEvent != null) luaScrollCellChangeEvent.Call(transform.gameObject,transform.gameObject.GetInstanceID(),idx);
        }
    }

	public class LoopScrollArraySource<T> : LoopScrollDataSource
    {
        T[] objectsToFill;

		public LoopScrollArraySource(T[] objectsToFill)
        {
            this.objectsToFill = objectsToFill;
        }

        public override void ProvideData(Transform transform, int idx)
        {
            transform.SendMessage("ScrollCellContent", objectsToFill[idx]);
        }
    }
}