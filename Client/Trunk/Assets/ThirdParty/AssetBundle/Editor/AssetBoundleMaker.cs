using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Cherry.AssetBundlePacker
{

    /// <summary>
    ///   打包方式
    /// </summary>
    public enum EBuildType
    {
        StandaloneWindows,
        Android,
        IOS,
    }
    public class AssetBoundleMaker
    {

        public static void BuildingAssetBundle(EBuildType build_type)
        {
            string outPath = EditorCommon.GetDownloadPath(build_type) + "Assetbundle";

            AssetBundleBuildData.SceneBuild sceneBuild = MatchSceneRuleData();
            SceneConfigTool.RecordDefaultOpenScene();
            SceneConfigTool.CopySceneToTempDir(sceneBuild);

            try
            {
                //清空资源目录
                FileHelper.ClearOrCreateDirectory(outPath);
                //生成资源场景配置
                SceneConfigTool.GenerateAllSceneConfig(sceneBuild);
                //打包资源
                BuildAssetResources(outPath, GetBuildTargetType(build_type));
                //生成资源依赖清单
                SceneConfigTool.ExportResourcesManifestFile(outPath);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.ToString());
            }
            finally
            {
                SceneConfigTool.ClearSceneConfig(sceneBuild);
                SceneConfigTool.RestoreAllScene(sceneBuild);
                EditorUtility.ClearProgressBar();
            }

        }

        /// <summary>
        /// 打包目标平台
        /// </summary>
        public static BuildTarget GetBuildTargetType(EBuildType build_type)
        {
            if (build_type == EBuildType.StandaloneWindows)
                return BuildTarget.StandaloneWindows;
            else if (build_type == EBuildType.Android)
                return BuildTarget.Android;
            else if (build_type == EBuildType.IOS)
                return BuildTarget.iOS;

            return BuildTarget.StandaloneWindows;
        }



        /// <summary>
        /// 获取场景列表
        /// </summary>
        static AssetBundleBuildData.SceneBuild MatchSceneRuleData()
        {
            AssetBundleBuildData.SceneBuild rules = new AssetBundleBuildData.SceneBuild();

            DirectoryInfo assets = new DirectoryInfo(EditorCommon.SCENE_START_PATH);
            if (assets.Exists)
            {
                var files = assets.GetFiles("*.unity", SearchOption.AllDirectories);
                foreach (var f in files)
                {
                    string localName = EditorCommon.AbsoluteToRelativePath(f.FullName);
                    var scene = new AssetBundleBuildData.SceneBuild.Element()
                    {
                        ScenePath = f.FullName,
                        IsBuild = true,
                    };
                    rules.Scenes.Add(scene);
                }
            }
            return rules;
        }

        public static void BuildAssetResources(string outputPath, BuildTarget targetPlatform)
        {
            Dictionary<string, List<string>> resAssetsMap = new Dictionary<string, List<string>>();

            SceneConfigTool.GetResBundleName(ref resAssetsMap);

            SceneConfigTool.GetPrefabBundleName(ref resAssetsMap);

            Dictionary<string, List<string>> sceneAssetsMap = SceneConfigTool.GetSceneBundleNames();


            List<AssetBundleBuild> maps = new List<AssetBundleBuild>();

            foreach (var v in resAssetsMap)
            {
                AssetBundleBuild build = new AssetBundleBuild();
                build.assetBundleName = EditorCommon.ConvertToAssetBundleName(v.Key);
                build.assetNames = v.Value.ToArray();
                maps.Add(build);
            }

            foreach (var v in sceneAssetsMap)
            {
                AssetBundleBuild build = new AssetBundleBuild();
                build.assetBundleName = EditorCommon.ConvertToAssetBundleName(v.Key);
                build.assetNames = v.Value.ToArray();
                maps.Add(build);
            }

            BuildPipeline.BuildAssetBundles(outputPath, maps.ToArray(), BuildAssetBundleOptions.UncompressedAssetBundle, targetPlatform);
        }

        


    }

}