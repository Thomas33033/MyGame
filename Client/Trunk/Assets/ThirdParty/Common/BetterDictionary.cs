using LuaInterface;
using System;
using System.Collections.Generic;

namespace Cherry
{
    public class BetterDictionary<TKey, TValue> : Dictionary<TKey, TValue>
    {
        public delegate bool CheckList(KeyValuePair<TKey, TValue> map);

        /// <summary>
        /// 循环代理
        /// </summary>
        /// <param name="index">循环次数</param>
        /// <param name="item">字典内元素</param>
        /// <returns> 循环控制 true 继续  false Break </returns>
        public delegate bool ForeachHandle(int index, KeyValuePair<TKey, TValue> item);

        public void BetterForeach(Action<KeyValuePair<TKey, TValue>> callback)
        {
            foreach (var kv in this)
            {
                callback(kv);
            }
        }

        //public void LuaBetterForeach(LuaFunction callback)
        //{
        //    foreach (var kv in this)
        //    {
        //        callback.Call(kv);
        //    }
        //}

        public bool CheckBetterForeach(CheckList callback)
        {
            foreach (var kv in this)
            {
                if (false == callback(kv))
                    return false;
            }

            return true;
        }

        public void BetterForeach(ForeachHandle callback)
        {
            int index = 0;
            foreach (var kv in this)
            {
                if (callback(index, kv) == false)
                    break;

                ++index;
            }
        }

        private List<TKey> RmoveCache = null;
        public void CacheRemove(TKey key)
        {
            if (RmoveCache == null) RmoveCache = new List<TKey>();
            RmoveCache.Add(key);
        }
        public void RemoveFromCache()
        {
            if (RmoveCache == null) return;
            for (int i = 0; i < RmoveCache.Count; i++)
            {
                this.Remove(RmoveCache[i]);
            }
            RmoveCache.Clear();
        }
        new public void Clear()
        {
            base.Clear();
            if (RmoveCache != null)
                RmoveCache.Clear();
        }
    }
}
