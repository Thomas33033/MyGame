using UnityEngine;
using System.Collections;
using SG;

namespace UnityEngine.UI
{
    /// <summary>
    /// 扩展LoopScrollPrefabSource，对象从Prefab创建
    /// </summary>
    [System.Serializable]
    public class XLoopScrollPrefabSource
    {
        public GameObject prefabGameobj;

        public int poolSize = 5;

        public XLoopScrollPrefabSource()
        { }

        /// <summary>
        /// 通过prefab初始化
        /// 扩展by qingqing-zhao
        /// </summary>
        /// <param name="prefabGmobj"></param>
        /// <param name="poolSize"></param>
        public XLoopScrollPrefabSource(GameObject prefabGmobj, int poolSize = 5)
        {
            if (prefabGmobj != null)
            {
                this.prefabGameobj = prefabGmobj;
            }
            this.poolSize = poolSize;
        }

        public virtual void InitPool()
        {
            if (prefabGameobj != null)
            {
                ResourceManager.Instance.InitPool(prefabGameobj, poolSize);
            }
        }

        public virtual GameObject GetObject()
        {
            return ResourceManager.Instance.GetObjectFromPool(prefabGameobj);
        }
    }
}
