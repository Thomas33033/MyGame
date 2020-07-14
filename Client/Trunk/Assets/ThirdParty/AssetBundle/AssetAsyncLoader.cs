using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Cherry.AssetBundlePacker
{
    /// <summary>
    /// 资源异步加载器
    /// </summary>
    public class AssetAsyncLoader
    {
        /// <summary>
        /// AssetBundle名称
        /// </summary>
        public string AssetBundleName { get; private set; }

        /// <summary>
        /// 资源名称(全部小写)
        /// </summary>
        public string AssetName { get; private set; }

        /// <summary>
        /// 资源名称（原始名称）
        /// </summary>
        public string OrignalAssetName { get; private set; }

        /// <summary>
        /// 加载完成卸载AssetBundle
        /// </summary>
        public bool UnloadAssetBundle { get; private set; }

        /// <summary>
        /// 是否加载完成?
        /// </summary>
        public bool IsDone { get; private set; }

        /// <summary>
        /// 加载进度.
        /// </summary>
        public float Progress { get; private set; }

        /// <summary>
        /// 已加载的资源
        /// </summary>
        public Object Asset { get; private set; }

        /// <summary>
        /// 已加载的子资源
        /// </summary>
        public Object[] AllAssets { get; private set; }


        public int maxCount;
        public int curNum;

        public AssetBundleManager mgr;
        string[] deps;

        private System.Action loadOverCall;

        public AssetAsyncLoader(string assetbundlename, string asset_name, string orignal_asset_name, bool unload_assetbundle)
        {
            AssetBundleName = assetbundlename;
            AssetName = asset_name;
            OrignalAssetName = orignal_asset_name;
            UnloadAssetBundle = unload_assetbundle;
            IsDone = false;
            Progress = 0f;
            Asset = null;
            AllAssets = null;
        }

        /// <summary>
        ///   加载一个资源
        /// </summary>
        public void StartLoadAssetAsync(AssetBundleManager mgr,System.Action call)
        {
            this.loadOverCall = call;

            if (string.IsNullOrEmpty(AssetBundleName))
            {
                IsDone = true;
                return;
            }

            IsDone = false;
            Progress = 0f;
            curNum = 0;

            //加载依赖
            deps = mgr.GetAllDependencies(AssetBundleName);
            if (deps != null)
            {
                maxCount = deps.Length;
                for (int i = 0; i < deps.Length; i++)
                {
                    mgr.AddAssetBundle(deps[i], this.LoadDependencies);
                }
            }
        }
       
        public void LoadDependencies()
        {   
            Progress = ++curNum / maxCount;
            
            if (Progress >= 1)
            {
                 mgr.AddAssetBundle(AssetBundleName, LoadFinished);
            }
        }


        public void LoadFinished()
        {
            AssetBundle ab = this.mgr.FindLoadedAssetBundle(AssetName);
            ab.LoadAsset(AssetName);
            
            //使用资源

            if (UnloadAssetBundle)
            {
                mgr.DisposeAssetBundleCache(deps, false);
                mgr.DisposeAssetBundleCache(AssetBundleName, false);
            }
            else
            {
                mgr.SaveAssetDependency(AssetName, AssetBundleName);
            }

            IsDone = true;
            Progress = 1;
        }
    }
}
