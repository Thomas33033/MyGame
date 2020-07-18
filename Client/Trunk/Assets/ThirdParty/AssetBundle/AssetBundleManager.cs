
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Cherry.AssetBundlePacker
{
    /// <summary>
    ///   资源管理器
    ///   引用计数器：资源每使用一次加1，资源销毁时减1
    /// </summary>
    public class AssetBundleManager : MonoSingleton<AssetBundleManager>
    {

        /// <summary>
        /// 当前平台是否支持AssetBundle
        /// </summary>
        public static readonly bool IsPlatformSupport = true;

        public string strVersion;

        public bool IsReady { get; private set; }

        public bool IsFailed
        {
            get { return ErrorCode != emErrorCode.None; }
        }

        public emErrorCode ErrorCode { get; private set; }

        /// <summary>
        ///   主AssetBundleMainfest
        /// </summary>
        public AssetBundleManifest MainManifest { get; private set; }

        /// <summary>
        ///   资源描述数据
        /// </summary>
        public ResourcesManifest ResManifest { get; private set; }

        /// <summary>
        ///   资源包数据
        /// </summary>
        public ResourcesPackages ResPackages { get; private set; }

        /// <summary>
        ///   常驻的AssetBundle
        /// </summary>
        private Dictionary<string, AssetBundle> assetbundle_permanent_;

        /// <summary>
        ///   缓存的AssetBundle
        /// </summary>
        private Dictionary<string, Cache> assetbundle_cache_;

        /// <summary>
        /// 缓存的资源对照表
        /// Key: 资源名称
        /// Value: 资源依赖缓存（包含引用的AssetBundle名称与及被引用次数）
        /// </summary>
        private Dictionary<string, AssetDependCache> asset_dependency_cache_;

        /// <summary>
        /// 正在异步载入的AssetBundle
        /// </summary>
        private HashSet<string> assetbundle_async_loading_;

        /// <summary>
        /// 同时加载资源的最大数量
        /// </summary>
        private int bundleLoaderLineNum = 5;

        public List<DelayCall> listLoadBundleName = new List<DelayCall>();
        private bool _isAssetBundleLoading;
        private Dictionary<string, AssetBundleCreateRequest> _dicAssetBundleRequest;

        protected AssetBundleManager() {
            assetbundle_permanent_ = new Dictionary<string, AssetBundle>();
            assetbundle_cache_ = new Dictionary<string, Cache>();
            asset_dependency_cache_ = new Dictionary<string, AssetDependCache>();
            assetbundle_async_loading_ = new HashSet<string>();
        }
        
       
        public void LoadAssetBundleAsync(string name, System.Action action)
        {
            DelayCall delayCall = new DelayCall();
            delayCall.action = action;
            delayCall.data = name;

            listLoadBundleName.Add(delayCall);

            if (_isAssetBundleLoading == false)
            {
                StartCoroutine(DoLoadAssetBundle());
            }
        }

        private IEnumerator DoLoadAssetBundle()
        {
            if (_isAssetBundleLoading == true)
                yield break;
            _isAssetBundleLoading = true;

            List<string> listDependencies = new List<string>();
            List<string> listTemp = new List<string>();
            while (listLoadBundleName.Count > 0)
            {
                listDependencies.Clear();
                if (CheckAssetBundleFileIsExite(listLoadBundleName[0].data) == false)
                {
                    listDependencies.Add(listLoadBundleName[0].data.ToLower());
                }

                string[] arr = GetAllDependencies(listLoadBundleName[0].data);

                listDependencies.AddRange(arr);

                while (listDependencies.Count > 0 || _dicAssetBundleRequest.Count > 0)
                {
                    if (listDependencies.Count > 0)
                    {
                        if (FindLoadedAssetBundle(listDependencies[0]) != null)
                        {
                            listDependencies.RemoveAt(0);
                            continue;
                        }

                        if (_dicAssetBundleRequest.Count < bundleLoaderLineNum)
                        {
                            if (_dicAssetBundleRequest.ContainsKey(listDependencies[0]) == false)
                            {
                                AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(AssetsManager.AssetBundlePath
                                    + "/" + listDependencies[0]);
                                _dicAssetBundleRequest.Add(listDependencies[0], request);
                            }
                            listDependencies.RemoveAt(0);
                        }
                    }

                    foreach (var item in _dicAssetBundleRequest)
                    {
                        if (item.Value.isDone)
                        {
                            SaveAssetDependency(item.Key,item.Value.assetBundle.name);
                            listTemp.Add(item.Key);
                        }
                    }

                    if (listTemp.Count > 0)
                    {
                        for (int i = 0; i < listTemp.Count; i++)
                        {
                            _dicAssetBundleRequest.Remove(listTemp[i]);
                        }
                        listTemp.Clear();
                    }

                    yield return new WaitForEndOfFrame();
                }

                if (listLoadBundleName[0].action != null)
                {
                    listLoadBundleName[0].action();
                }
                listLoadBundleName.RemoveAt(0);
            }
            _isAssetBundleLoading = false;
        }

        /// <summary>
        ///   加载一个资源
        /// </summary>
        public T LoadAsset<T>(string asset, bool unload_assetbundle = true)
                where T : Object
        {
            try
            {
                if (!IsPlatformSupport)
                {
                    return null;
                }
                if (!IsReady || IsFailed)
                {
                    return null;
                }

                asset = asset.ToLower();

                // 加载AssetBundle
                string assetbundlename = null;
                string[] all_assetbundle = FindAllAssetBundleNameByAsset(asset);
                if (all_assetbundle != null)
                {
                    for (int i = 0; i < all_assetbundle.Length; ++i)
                    {
                        if (CheckAssetBundleFileIsExite(all_assetbundle[i]))
                        {
                            assetbundlename = all_assetbundle[i];
                            break;
                        }
                    }
                }

                if (assetbundlename == null)
                {
                    Debug.LogWarning("AssetBundle can't find. Asset name is (" + asset + ")!");
                    return null;
                }

                // 加载依赖
                string[] deps = LoadDependenciesAssetBundle(assetbundlename);

                // 加载AssetBundle
                var ab = LoadAssetBundle(assetbundlename);
                // 加载资源
                T result = ab.LoadAsset<T>(asset);

                // 卸载AssetBundle
                if (unload_assetbundle)
                {
                    DisposeAssetBundleCache(deps, false);
                    DisposeAssetBundleCache(assetbundlename, false);
                }
                else
                {
                    SaveAssetDependency(asset, assetbundlename);
                }
                return result;
            }
            catch (System.Exception ex)
            {
                Debug.LogError("AssetBundleManager.LoadAsset is falid!\n" + ex.Message);
            }

            return null;
        }

        /// <summary>
        ///   异步加载一个资源
        /// </summary>
        public void LoadAssetAsync(string asset, System.Action call, bool unload_assetbundle = true)
        {
            try
            {
                /// 转小写
                string assetName = asset.ToLower();

                ///判断此asset是否拥有可加载的AssetBundle
                string assetbundlename = null;
                string[] all_asssetbundle = FindAllAssetBundleNameByAsset(assetName);
                if (all_asssetbundle != null)
                {
                    for (int i = 0; i < all_asssetbundle.Length; ++i)
                    {
                        if (CheckAssetBundleFileIsExite(all_asssetbundle[i]))
                        {
                            assetbundlename = all_asssetbundle[i];
                            break;
                        }
                    }
                }
                if (string.IsNullOrEmpty(assetbundlename))
                {
                    return ;
                }

                AssetAsyncLoader loader = new AssetAsyncLoader(assetbundlename, assetName, asset, unload_assetbundle);
                loader.StartLoadAssetAsync(this, call);
            }
            catch (System.Exception ex)
            {
                Debug.LogError("AssetBundleManager.LoadAsset is falid!\n" + ex.Message);
            }

            return ;
        }

        /// <summary>
        /// 卸载一个资源
        /// </summary>
        public void UnloadAsset(string asset)
        {
            asset = asset.ToLower();
            AssetDependCache cache;
            if (asset_dependency_cache_.TryGetValue(asset, out cache))
            {
                //减少资源缓存引用
                if (cache.RefCount == 1)
                {
                    asset_dependency_cache_.Remove(asset);
                }
                else
                {
                    cache.RefCount -= 1;
                    asset_dependency_cache_[asset] = cache;
                }

                //处理AssetBundle
                DisposeAssetBundleCache(cache.RefAssetBundleName, true);
                string[] deps = MainManifest.GetAllDependencies(cache.RefAssetBundleName);
                DisposeAssetBundleCache(deps, true);
            }
        }

        /// <summary>
        /// 卸载一个AssetBundle
        /// </summary>
        public void UnloadAssetBundle(string assetbundleName, bool unload_all_dependencies_ab, bool unload_all_loaded_objects)
        {
            //卸载AssetBundle
            bool is_unload = false;
            Cache cache;
            if (assetbundle_cache_.TryGetValue(assetbundleName, out cache))
            {
                cache.RefAssetBundle.Unload(unload_all_loaded_objects);
                assetbundle_cache_.Remove(assetbundleName);
                is_unload = true;
            }
            else
            {
                AssetBundle ab = null;
                if (assetbundle_permanent_.TryGetValue(assetbundleName, out ab))
                {
                    ab.Unload(unload_all_loaded_objects);
                    assetbundle_permanent_.Remove(assetbundleName);
                    is_unload = true;
                }
            }
            //卸载依赖的AssetBundle
            if (is_unload && unload_all_dependencies_ab)
            {
                string[] deps = MainManifest.GetAllDependencies(assetbundleName);
                for (int i = 0; i < deps.Length; ++i)
                {
                    string dep_assetbundleName = deps[i];
                    if (assetbundle_cache_.TryGetValue(dep_assetbundleName, out cache))
                    {
                        cache.RefAssetBundle.Unload(unload_all_loaded_objects);
                        assetbundle_cache_.Remove(dep_assetbundleName);
                    }
                    else
                    {
                        AssetBundle ab = null;
                        if (assetbundle_permanent_.TryGetValue(dep_assetbundleName, out ab))
                        {
                            ab.Unload(unload_all_loaded_objects);
                            assetbundle_permanent_.Remove(dep_assetbundleName);
                        }
                    }
                }
            }
        }

        
        /// <summary>
        ///  异步加载场景
        /// </summary>
        public void LoadSceneAsync(string scene_name, LoadSceneMode mode, System.Action call)
        {
            try
            {
                string assetbundlename = FindAssetBundleNameByScene(scene_name);
                if (!string.IsNullOrEmpty(assetbundlename))
                {
                    SceneAsyncLoader loader = new SceneAsyncLoader(assetbundlename, scene_name, mode, true);
                    loader.StartLoadSceneAsync(this, call);
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError("AssetBundleManager.LoadAsset is falid!\n" + ex.Message);
            }
        }

        /// <summary>
        /// 卸载一个场景资源
        /// </summary>
        public void UnloadScene(string scene_name)
        {
            scene_name = scene_name.ToLower();
            AssetDependCache cache;
            if (asset_dependency_cache_.TryGetValue(scene_name, out cache))
            {
                //减少资源缓存引用
                if (cache.RefCount == 1)
                {
                    asset_dependency_cache_.Remove(scene_name);
                }
                else
                {
                    cache.RefCount -= 1;
                    asset_dependency_cache_[scene_name] = cache;
                }
                
                DisposeAssetBundleCache(cache.RefAssetBundleName, true);
                string[] deps = MainManifest.GetAllDependencies(cache.RefAssetBundleName);
                DisposeAssetBundleCache(deps, true);
            }
        }

        /// <summary>
        ///   判断一个AssetBundle是否存在缓存
        /// </summary>
        public bool IsExist(string assetbundlename)
        {
            if (string.IsNullOrEmpty(assetbundlename))
                return false;

            return File.Exists(Common.GetFileFullName(assetbundlename));
        }

        /// <summary>
        ///   判断一个资源是否存在于AssetBundle中
        /// </summary>
        public bool IsAssetExist(string asset)
        {
            string[] assetbundlesname = FindAllAssetBundleNameByAsset(asset);
            if (assetbundlesname != null)
            {
                for (int i = 0; i < assetbundlesname.Length; ++i)
                    if (IsExist(assetbundlesname[i])) return true;
            }

            return false;
        }

        /// <summary>
        ///   判断场景是否存在于AssetBundle中
        /// </summary>
        public bool IsSceneExist(string scene_name)
        {
            return !string.IsNullOrEmpty(FindAssetBundleNameByScene(scene_name));
        }

        /// <summary>
        ///   获得AssetBundle中的所有资源
        /// </summary>
        public string[] FindAllAssetNames(string assetbundlename)
        {
            AssetBundle bundle = LoadAssetBundle(assetbundlename);
            if (bundle != null)
                return bundle.GetAllAssetNames();
            return null;
        }

        /// <summary>
        ///   获得包含某个资源的所有AssetBundle
        /// </summary>
        public string[] FindAllAssetBundleNameByAsset(string asset)
        {
            if (ResManifest == null)
            {
                return null;
            }

            return ResManifest.GetAllAssetBundleName(asset);
        }

        /// <summary>
        ///   获得一个场景的包名
        /// </summary>
        public string FindAssetBundleNameByScene(string scene_name)
        {
            if (ResManifest == null)
            {
                return null;
            }

            return ResManifest.GetAssetBundleNameBySceneLevelName(scene_name);
        }

        /// <summary>
        ///   获得指定资源包的AssetBundle列表
        /// </summary>
        public List<string> FindAllAssetBundleFilesNameByPackage(string package_name)
        {
            if (ResPackages == null)
            {
                return null;
            }

            ResourcesPackagesData.Package pack = ResPackages.Find(package_name);
            if (pack == null)
                return null;

            List<string> result = new List<string>();
            for (int i = 0; i < pack.AssetList.Count; ++i)
            {
                string[] assetbundlename = FindAllAssetBundleNameByAsset(pack.AssetList[i]);
                if (assetbundlename != null && assetbundlename.Length > 0)
                {
                    if (!string.IsNullOrEmpty(assetbundlename[0]))
                    {
                        if (!result.Contains(assetbundlename[0]))
                        {
                            result.Add(assetbundlename[0]);
                        }
                    }
                }
            }

            return result.Count > 0 ? result : null;
        }


        /// <summary>
        ///   释放所有的AssetBundle
        /// </summary>
        public void UnloadAllAssetBundle(bool unload_all_loaded_objects)
        {
            UnloadAssetBundleCache(unload_all_loaded_objects);
            UnloadAssetBundlePermanent(unload_all_loaded_objects);
        }

        /// <summary>
        ///   释放所有缓存的AssetBundle
        /// </summary>
        public void UnloadAssetBundleCache(bool unload_all_loaded_objects)
        {
            if (assetbundle_cache_ != null && assetbundle_cache_.Count > 0)
            {
                var itr = assetbundle_cache_.Values.GetEnumerator();
                while (itr.MoveNext())
                {
                    itr.Current.RefAssetBundle.Unload(unload_all_loaded_objects);
                }
                itr.Dispose();
                assetbundle_cache_.Clear();
            }
        }

        /// <summary>
        ///   加载所有依赖的AssetBundle
        /// </summary>
        string[] LoadDependenciesAssetBundle(string assetbundlename)
        {
            if (assetbundlename == null)
                return null;
            if (MainManifest == null)
                return null;

            string[] deps = MainManifest.GetAllDependencies(assetbundlename);
            for (int index = 0; index < deps.Length; index++)
            {
                //加载所有的依赖AssetBundle
                if (LoadAssetBundle(deps[index]) == null)
                {
                    Debug.LogWarning(assetbundlename + "'s Dependencie AssetBundle can't find. Name is (" + deps[index] + ")!");
                    return null;
                }
            }

            return deps;
        }

        /// <summary>
        ///   加载AssetBundle
        /// </summary>
        private AssetBundle LoadAssetBundle(string assetbundlename)
        {
            if (assetbundlename == null)
                return null;

            ///判断此AssetBundle是否正在被异步加载，则等待加载完成
            ///此处可能延迟较长
            bool isLoading = assetbundle_async_loading_.Contains(assetbundlename);
            if (isLoading)
            {
                while (assetbundle_async_loading_.Contains(assetbundlename) == true)
                {
                }
            }

            AssetBundle ab = FindLoadedAssetBundle(assetbundlename);
            if (ab == null)
            {
                string assetbundle_path = GetAssetBundlePath(assetbundlename);
                if (System.IO.File.Exists(assetbundle_path))
                {
                    ab = AssetBundle.LoadFromFile(assetbundle_path);
                }
            }
            SaveAssetBundle(assetbundlename, ab);

            return ab;
        }

        /// <summary>
        ///   异步加载一个AssetBundle
        /// </summary>
        public IEnumerator LoadAssetBundleAsync(string assetbundlename)
        {
            if (assetbundlename == null)
                yield break;

            ///判断此AssetBundle是否正在被异步加载，则等待加载完成
            bool isLoading = assetbundle_async_loading_.Contains(assetbundlename);
            if (isLoading)
            {
                while (assetbundle_async_loading_.Contains(assetbundlename) == true)
                {
                    yield return null;
                }
            }

            AssetBundle ab = FindLoadedAssetBundle(assetbundlename);
            if (ab == null)
            {
                ///没有此AssetBundle缓存，开始异步加载
                assetbundle_async_loading_.Add(assetbundlename);
                string path = GetAssetBundlePath(assetbundlename);
                var req = AssetBundle.LoadFromFileAsync(path);
                while (!req.isDone)
                {
                    yield return null;
                }
                ab = req.assetBundle;
                SaveAssetBundle(assetbundlename, ab);
                assetbundle_async_loading_.Remove(assetbundlename);
            }
            else
            {
                SaveAssetBundle(assetbundlename, ab);
            }

            yield break;
        }

        /// <summary>
        ///   释放所有常驻的AssetBundle
        /// </summary>
        void UnloadAssetBundlePermanent(bool unloadAll)
        {
            if (assetbundle_permanent_ != null && assetbundle_permanent_.Count > 0)
            {
                var itr = assetbundle_permanent_.Values.GetEnumerator();
                while (itr.MoveNext())
                {
                    itr.Current.Unload(unloadAll);
                }
                itr.Dispose();
                assetbundle_permanent_.Clear();
            }
        }

        /// <summary>
        /// 保存AssetBundle到加载队列
        /// </summary>
        void SaveAssetBundle(string assetbundlename, AssetBundle ab)
        {
            //根据AssetBundleDescribe分别存放AssetBundle
            bool permanent = ResManifest.IsPermanent(assetbundlename);
            if (permanent)
            {
                if (!assetbundle_permanent_.ContainsKey(assetbundlename))
                {
                    assetbundle_permanent_.Add(assetbundlename, ab);
                }
            }
            else
            {
                SaveAssetBundleToCache(assetbundlename, ab);
            }
        }

        /// <summary>
        /// 保存资源依赖，用于后续卸载资源
        /// </summary>
        public void SaveAssetDependency(string asset, string assetbundle)
        {
            int refCount = 0;
            if (asset_dependency_cache_.ContainsKey(asset))
            {
                refCount = asset_dependency_cache_[asset].RefCount;
            }
            ++refCount;

            asset_dependency_cache_[asset] = new AssetDependCache()
            {
                RefCount = refCount,
                RefAssetBundleName = assetbundle,
            };
        }

        /// <summary>
        /// 保存到缓存中
        /// </summary>
        void SaveAssetBundleToCache(string assetbundleName, AssetBundle ab)
        {
            int refCount = 0;
            if (assetbundle_cache_.ContainsKey(assetbundleName))
            {
                refCount = assetbundle_cache_[assetbundleName].RefCount;
            }
            ++refCount;

            assetbundle_cache_[assetbundleName] = new Cache()
            {
                RefCount = refCount,
                RefAssetBundle = ab,
            };
            Debug.LogError(assetbundleName + ":" + refCount);
        }

        /// <summary>
        /// 处理缓存的多个AssetBundle, 如果没有引用则卸载
        /// </summary>
        public void DisposeAssetBundleCache(string[] assetbundlesName, bool unload_all_loaded_objects)
        {
            if (assetbundlesName != null && assetbundlesName.Length > 0)
            {
                for (int index = 0; index < assetbundlesName.Length; index++)
                {
                    DisposeAssetBundleCache(assetbundlesName[index], unload_all_loaded_objects);
                }
            }
        }

        /// <summary>
        /// 处理缓存的AssetBundle, 如果没有引用则卸载
        /// </summary>
        public void DisposeAssetBundleCache(string assetbundleName, bool unload_all_loaded_objects)
        {
            Cache cache;
            if (assetbundle_cache_.TryGetValue(assetbundleName, out cache))
            {
                if (cache.RefCount == 1)
                {
                    cache.RefAssetBundle.Unload(unload_all_loaded_objects);
                    assetbundle_cache_.Remove(assetbundleName);
                }
                else
                {
                    cache.RefCount -= 1;
                    assetbundle_cache_[assetbundleName] = cache;
                }
            }
        }

        /// <summary>
        ///   查找是否有已载加的AssetBundle
        /// </summary>
        public AssetBundle FindLoadedAssetBundle(string assetbundlename)
        {
            if (assetbundlename == null)
                return null;
            if (MainManifest == null)
                return null;

            AssetBundle ab = null;
            if (assetbundle_permanent_.ContainsKey(assetbundlename))
            {
                ab = assetbundle_permanent_[assetbundlename];
            }
            else if (assetbundle_cache_.ContainsKey(assetbundlename))
            {
                ab = assetbundle_cache_[assetbundlename].RefAssetBundle;
            }

            return ab;
        }

        /// <summary>
        /// 获得AssetBundle的依赖
        /// </summary>
        public string[] GetAllDependencies(string assetbundlename)
        {
            if (assetbundlename == null)
                return null;
            if (MainManifest == null)
                return null;

            return MainManifest.GetAllDependencies(assetbundlename);
        }

        /// <summary>
        /// 获得AssetBundle的依赖
        /// </summary>
        public bool HasDependencies(string assetbundlename)
        {
            var deps = GetAllDependencies(assetbundlename);
            return deps != null && deps.Length > 0;
        }

        /// <summary>
        ///   判断本地是否包含所有依赖
        /// </summary>
        bool CheckAssetBundleFileIsExite(string assetbundlename)
        {
            if (assetbundlename == null)
                return false;
            if (MainManifest == null)
                return false;

            string assetbundle_path = GetAssetBundlePath(assetbundlename);
            if (!System.IO.File.Exists(assetbundle_path))
            {
                return false;
            }

            string[] deps = MainManifest.GetAllDependencies(assetbundlename);
            for (int index = 0; index < deps.Length; index++)
            {
                AssetBundle ab = FindLoadedAssetBundle(deps[index]);
                if (ab == null)
                {
                    string path = GetAssetBundlePath(deps[index]);
                    if (!System.IO.File.Exists(path))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
        

        /// <summary>
        /// 获得AssetBundle的路径
        /// </summary>
        static string GetAssetBundlePath(string assetbundlename)
        {
            return Common.INITIAL_PATH + "/" + assetbundlename;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Error(emErrorCode ec, string message = null)
        {
            ErrorCode = ec;

            StringBuilder sb = new StringBuilder("[AssetBundleManager] - ");
            sb.Append(ErrorCode.ToString());
            if (!string.IsNullOrEmpty(message)) { sb.Append("\n"); sb.Append(message); }
        }

       
        /// <summary>
        /// 关闭
        /// </summary>
        void ShutDown()
        {
            StopAllCoroutines();
            UnloadAllAssetBundle(true);
        }


        /// <summary>
        /// AssetBundle引用缓存结构
        /// </summary>
        struct Cache
        {
            public int RefCount;
            public AssetBundle RefAssetBundle;
        }

        /// <summary>
        /// 资源依赖缓存
        /// </summary>
        struct AssetDependCache
        {
            public int RefCount;
            public string RefAssetBundleName;
        }


        //编辑器模式下使用
        public void LoadScene(string name, LoadSceneMode parameters, System.Action cb) {
            StartCoroutine(DoLoadScene(name, parameters, cb));
        }

        public IEnumerator DoLoadScene(string name, LoadSceneMode parameters, System.Action cb)
        {
            AsyncOperation async = SceneManager.LoadSceneAsync(name, parameters);
            while (async.isDone == false)
                yield return new WaitForEndOfFrame();
            if (cb != null)
                cb();
        }


    }
}
