using System.Collections;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Cherry.AssetBundlePacker
{
    /// <summary>
    /// 场景异步加载器
    /// </summary>
    internal class SceneAsyncLoader
    {
        public string AssetBundleName { get; private set; }

        public string SceneName { get; private set; }

        public LoadSceneMode LoadMode { get; private set; }

        public bool UnloadAssetBundle { get; private set; }

        public bool IsDone { get; private set; }

        public float Progress { get; private set; }


        public int maxCount;
        public int curNum;

        public AssetBundleManager mgr;
        string[] deps;

        private System.Action loadFinishedCall;

        public SceneAsyncLoader(string assetbundlename, string scene_name
            , LoadSceneMode mode
            , bool unload_assetbundle)
        {
            AssetBundleName = assetbundlename;
            SceneName = scene_name;
            LoadMode = mode;
            UnloadAssetBundle = unload_assetbundle;
            IsDone = false;
            Progress = 0f;
        }
            
        public void StartLoadSceneAsync(AssetBundleManager mgr, System.Action action)
        {
            this.mgr = mgr;
            this.loadFinishedCall = action;

            if (string.IsNullOrEmpty(AssetBundleName))
            {
                IsDone = true;
                return;
            }

            ///标记未完成
            IsDone = false;
            Progress = 0f;

            ///进度计算
            curNum = 0;
            maxCount = 0;

            string[] deps = mgr.GetAllDependencies(AssetBundleName);

            //加载依赖
            deps = mgr.GetAllDependencies(AssetBundleName);
            if (deps != null && deps.Length > 0)
            {
                maxCount = deps.Length;
                for (int i = 0; i < deps.Length; i++)
                {
                    mgr.LoadAssetBundle(deps[i], this.LoadDependencies);
                }
            }
            else
            {
                curNum = 1;
                maxCount = 1;
                LoadDependencies();
            }
        }

        public void LoadDependencies()
        {
            Progress = ++curNum / maxCount;
            if (Progress >= 1)
            {
                mgr.LoadAssetBundle(AssetBundleName, ()=> {
                    mgr.StartCoroutine("LoadSceneAsync");
                });
            }
        }

        public IEnumerator LoadSceneAsync()
        {
            // 加载场景
            UnityEngine.AsyncOperation async = SceneManager.LoadSceneAsync(SceneName, LoadMode);
            while (!async.isDone)
            {
                yield return async;
            }
            
            if (UnloadAssetBundle)
            {
                mgr.DisposeAssetBundleCache(deps, false);
                mgr.DisposeAssetBundleCache(AssetBundleName, false);
            }
            else
            {
                mgr.SaveAssetDependency(SceneName, AssetBundleName);
            }

            IsDone = true;
            Progress = 1;
        }

    }

}
