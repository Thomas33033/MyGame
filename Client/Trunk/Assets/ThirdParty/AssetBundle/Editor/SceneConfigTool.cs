using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using System;
using System.Linq;

namespace Cherry.AssetBundlePacker
{

    public class SceneConfigTool
    {

        public static void CopySceneToTempDir(AssetBundleBuildData.SceneBuild scene_rules)
        {
            for (int i = 0; i < scene_rules.Scenes.Count; ++i)
            {
                var scene = scene_rules.Scenes[i].ScenePath;
                if (scene_rules.Scenes[i].IsBuild)
                {
                    CopySceneToBackup(scene);
                }
            }
        }

        /// <summary>
        /// 资源打包结束后，清空场景配置
        /// </summary>
        public static void ClearSceneConfig(AssetBundleBuildData.SceneBuild scene_rules)
        {
            for (int i = 0; i < scene_rules.Scenes.Count; ++i)
            {
                var scene = scene_rules.Scenes[i].ScenePath;
                if (scene_rules.Scenes[i].IsBuild)
                {
                    DeleteSceneConfig(scene);
                }
            }

        }

        /// <summary>
        ///   生成所有场景配置文件
        /// </summary>
        public static bool GenerateAllSceneConfig(AssetBundleBuildData.SceneBuild scene_rules)
        {
            bool cancel = false;
            float total = (float)scene_rules.Scenes.Count;
            float current = 0;

            for (int i = 0; i < scene_rules.Scenes.Count; ++i)
            {
                var scene = scene_rules.Scenes[i].ScenePath;
                if (scene_rules.Scenes[i].IsBuild)
                {
                    GenerateSceneConfig(scene);
                }

                current += 1.0f;
                float progress = current / total;
                if (EditorUtility.DisplayCancelableProgressBar("正在生成场景配置数据", "Change " + scene, progress))
                {
                    cancel = true;
                    break;
                }
            }
            EditorUtility.ClearProgressBar();

            return !cancel;
        }

        /// <summary>
        ///   恢复所有场景
        /// </summary>
        public static void RestoreAllScene(AssetBundleBuildData.SceneBuild scene_rules)
        {
            for (int i = 0; i < scene_rules.Scenes.Count; ++i)
            {
                if (scene_rules.Scenes[i].IsBuild)
                    RestoreSceneFromBackup(scene_rules.Scenes[i].ScenePath);
            }

            ReturnDefaultOpenScene();
        }

        /// <summary>
        ///   配置场景信息
        /// </summary>
        public static void GenerateSceneConfig(string scene_path)
        {
            var scene = EditorSceneManager.OpenScene(scene_path);
            SaveAll();
            RemoveAll();
            EditorSceneManager.SaveScene(scene);
        }

        /// <summary>
        ///   删除配置文件
        /// </summary>
        public static void DeleteSceneConfig(string scene_path)
        {
            var file_name = SceneConfig.GetSceneConfigPath(scene_path);
            if (File.Exists(file_name))
                File.Delete(file_name);
        }

        /// <summary>
        ///   默认打开场景
        /// </summary>
        static string default_open_scene_;

        /// <summary>
        ///   记录默认打开场景
        /// </summary>
        public static void RecordDefaultOpenScene()
        {
            Scene sc = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            default_open_scene_ = sc.path ?? null;
        }

        /// <summary>
        ///   恢复默认打开场景
        /// </summary>
        static void ReturnDefaultOpenScene()
        {
            if (string.IsNullOrEmpty(default_open_scene_))
            {
                EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects);
            }
            else
            {
                EditorSceneManager.OpenScene(default_open_scene_);
            }
        }

        static void SaveAll()
        {
            List<string> buildAssets = new List<string>();

            UnityEngine.SceneManagement.Scene sc = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            if (!sc.IsValid())
                return;

            SceneConfig data = new SceneConfig();
            data.Data.LevelName = sc.name;

            UnityEngine.Object[] array = GameObject.FindObjectsOfType(typeof(GameObject));


            if (array == null)
                return;

            for (int i = 0; i < array.Length; ++i)
            {
                UnityEngine.Object parentObject = PrefabUtility.GetCorrespondingObjectFromSource(array[i]);
                string path = AssetDatabase.GetAssetPath(parentObject);
                if (string.IsNullOrEmpty(path))
                    continue;

                var transform = (array[i] as GameObject).transform;
                var parent = transform.parent;

                //写入数据
                var obj = new SceneConfigData.SceneObject();
                obj.AssetName = path;
                obj.Position = transform.position;
                obj.Scale = transform.lossyScale;
                obj.Rotation = transform.rotation;
                obj.ParentName = parent != null ? Common.CalcTransformHierarchyPath(parent) : "";
                data.Data.SceneObjects.Add(obj);

                buildAssets.Add(path);
            }

            data.Save(SceneConfig.GetSceneConfigPath(sc.path));
            AssetDatabase.Refresh(); ;
        }

        static void RemoveAll()
        {
            UnityEngine.SceneManagement.Scene sc = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            if (!sc.IsValid())
                return;

            UnityEngine.Object[] array = GameObject.FindObjectsOfType(typeof(GameObject));
            if (array == null)
                return;

            for (int i = 0; i < array.Length; ++i)
            {
                UnityEngine.Object parentObject = PrefabUtility.GetCorrespondingObjectFromSource(array[i]);
                string path = AssetDatabase.GetAssetPath(parentObject);
                if (!string.IsNullOrEmpty(path))
                {
                    GameObject.DestroyImmediate(array[i]);
                }
            }
        }

        /// <summary>
        ///   临时存放场景目录
        /// </summary>
        static readonly string TEMP_PATH = Application.temporaryCachePath;

        /// <summary>
        ///   备份场景
        /// </summary>
        static void CopySceneToBackup(string scene_path)
        {
            if (File.Exists(scene_path))
            {
                var dest = TEMP_PATH + "/" + Path.GetFileName(scene_path);
                File.Copy(scene_path, dest, true);
            }
        }

        /// <summary>
        ///   从备份中恢复场景
        /// </summary>
        static void RestoreSceneFromBackup(string scene_path)
        {
            var src = TEMP_PATH + "/" + Path.GetFileName(scene_path);
            if (File.Exists(src))
            {
                File.Copy(src, scene_path, true);
                File.Delete(src);
            }
        }

        private static void AddAssetToMap(string assetPath, ref Dictionary<string, List<string>> mapAssetNames)
        {
            string ext = Path.GetExtension(assetPath);

            string bundleName = assetPath.ToLower().Replace(ext, "");

            //在atals文件夹下的资源以文件夹为单位合并
            if (bundleName.Contains("/atals/"))
            {
                bundleName = bundleName.Remove(bundleName.LastIndexOf("/"));
            }
                
            List<string> assetPaths = null;
            if (mapAssetNames.TryGetValue(bundleName, out assetPaths))
            {
                if (!assetPaths.Contains(assetPath))
                {
                    assetPaths.Add(assetPath);
                }
            }
            else
            {
                assetPaths = new List<string>();
                assetPaths.Add(assetPath);
                mapAssetNames.Add(bundleName, assetPaths);
            }
        }


        public static void GetPrefabBundleName(ref Dictionary<string, List<string>> mapAssetNames)
        {

            string srcPath = EditorCommon.PREFAB_START_PATH;

            List<string> allEntries = new List<string>();

            allEntries.AddRange(Directory.GetFiles(srcPath, "*.prefab", SearchOption.AllDirectories));

            for (int i = 0, count = allEntries.Count; i < count; ++i)
            {
                string file = EditorCommon.AbsoluteToRelativePath(allEntries[i]); 
                string ext = Path.GetExtension(file);

                if (!EditorCommon.IsIgnoreFile(ext))
                {
                    //加载依赖
                    string[] assets = AssetDatabase.GetDependencies(file);
                    for (int k = 0; k < assets.Length; k++)
                    {
                        string assetFile = assets[k];
                        string t_ext = Path.GetExtension(assetFile);
                        if (!EditorCommon.IsIgnoreFile(t_ext))
                        {
                            AddAssetToMap(assetFile, ref mapAssetNames);
                        }
                    }
                     
                }
            }
        }

        /// <summary>
        /// 获取资源AssetBundleName
        /// </summary>
        /// <returns></returns>
        public static void GetResBundleName(ref Dictionary<string, List<string>> mapAssetNames)
        {

            string srcPath = EditorCommon.RES_START_PATH;

            List<string> allEntries = new List<string>();

            allEntries.AddRange(Directory.GetFiles(srcPath, "*", SearchOption.AllDirectories));

            for (int i = 0, count = allEntries.Count; i < count; ++i)
            {
                string file = EditorCommon.AbsoluteToRelativePath(allEntries[i]);
                string ext = Path.GetExtension(file);

                if (!EditorCommon.IsIgnoreFile(ext))
                {
                    AddAssetToMap(file, ref mapAssetNames);
                }
            }
        }

        /// <summary>
        /// 获取场景 AssetBundleName
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, List<string>> GetSceneBundleNames()
        {
            string srcPath = EditorCommon.SCENE_START_PATH;

            string[] files = Directory.GetFiles(srcPath, "*.unity", SearchOption.AllDirectories);

            Dictionary<string, string> scenesDic = new Dictionary<string, string>();

            foreach (var f in files)
            {
                string file = EditorCommon.AbsoluteToRelativePath(f);

                string path = file;

                string ext = Path.GetExtension(file);

                if (ext.Equals(".unity"))
                {
                    file = file.Replace(ext, "");

                    scenesDic[file] = path;
                }
            }

            Dictionary<string, List<string>> mapAssetNames = new Dictionary<string, List<string>>();

            Dictionary<string, string>.Enumerator et = scenesDic.GetEnumerator();

            while (et.MoveNext())
            {

                string bundleName = et.Current.Key;
                List<string> assetNames = new List<string>();
                assetNames.Add(et.Current.Value);
                mapAssetNames.Add(bundleName, assetNames);

                string bundleName1 = et.Current.Key + "Config";
                List<string> assetNames1 = new List<string>();
                assetNames1.Add(SceneConfig.GetSceneConfigPath(et.Current.Value));
                mapAssetNames.Add(bundleName1, assetNames1);
            }
            et.Dispose();

            return mapAssetNames;
        }


        /// <summary>
        /// 根据AssetBundle导出ResourcesManifest文件
        /// </summary>
        public static void ExportResourcesManifestFile(string buildPath)
        {

            string manifest_file = buildPath + "/" + Common.MAIN_MANIFEST_FILE_NAME;
            AssetBundleManifest manifest = Common.LoadMainManifestByPath(manifest_file);

            ResourcesManifest info = new ResourcesManifest();

            //读取所有AssetBundle
            string root_dir = buildPath + "/";
            List<string> scenes = new List<string>();
            if (manifest != null)
            {
                //读取主AssetBundle
                ResourcesManifestData.AssetBundle desc = new ResourcesManifestData.AssetBundle();
                desc.AssetBundleName = Common.MAIN_MANIFEST_FILE_NAME;
                desc.Size = FileHelper.GetFileSize(root_dir + Common.MAIN_MANIFEST_FILE_NAME);
                info.Data.AssetBundles.Add(Common.MAIN_MANIFEST_FILE_NAME, desc);

                //读取其它AssetBundle
                foreach (var name in manifest.GetAllAssetBundles())
                {
                    desc = new ResourcesManifestData.AssetBundle();
                    desc.AssetBundleName = name;
                    desc.Size = FileHelper.GetFileSize(root_dir + name);
                    AssetBundle ab = AssetBundle.LoadFromFile(root_dir + name);
                    foreach (var asset in ab.GetAllAssetNames())
                    {
                        desc.Assets.Add(asset);
                    }
                    foreach (var scene in ab.GetAllScenePaths())
                    {
                        desc.Scenes.Add(scene);
                        scenes.Add(scene);
                    }
                    ab.Unload(false);

                    info.Data.AssetBundles.Add(name, desc);
                }
            }

            //读取所有Scene信息
            for (int i = 0; i < scenes.Count; ++i)
            {
                ResourcesManifestData.Scene scene_desc = new ResourcesManifestData.Scene();
                scene_desc.SceneLevelName = Path.GetFileNameWithoutExtension(scenes[i]);
                scene_desc.ScenePath = scenes[i];
                scene_desc.SceneConfigPath = SceneConfig.GetSceneConfigPath(scenes[i]);
                info.Data.Scenes.Add(scene_desc.SceneLevelName, scene_desc);
            }

            //设置版本信息
            info.Data.strVersion = "1.0.1";
            info.Data.IsAllCompress = true;
            info.Data.IsAllNative = false;

            //保存ResourcesInfo
            string resources_manifest_file = buildPath + "/" + Common.RESOURCES_MANIFEST_FILE_NAME;
            info.Save(resources_manifest_file);

            AssetDatabase.Refresh();

        }
    }
}