
using UnityEngine;
using System.IO;

namespace Cherry.AssetBundlePacker
{
    /// <summary>
    ///   
    /// </summary>
    public static class EditorCommon
    {

        public static readonly string DOWNLOAD_PATH = Application.dataPath + "/../Download/{0}/";


        /// <summary>
        /// 编辑器环境下的资源起始路劲
        /// </summary>
        public static readonly string RES_START_PATH = Application.dataPath + "/BundleRes";

        /// <summary>
        /// 编辑器环境下的prefab起始路劲
        /// </summary>
        public static readonly string PREFAB_START_PATH = Application.dataPath + "/BundlePrefab";

        /// <summary>
        ///   编辑器环镜下场景起始路径
        /// </summary>
        public static readonly string SCENE_START_PATH = Application.dataPath + "/Scenes";

        /// <summary>
        /// 编辑器环镜下AssetBundleBuild.rule保存路径
        /// </summary>
        public static readonly string ASSETBUNDLE_BUILD_RULE_FILE_PATH = Application.dataPath + "/AssetBundleBuild.rule";

        /// <summary>
        ///   忽略的文件类型(后缀名)
        /// </summary>
        public static readonly string[] IGNORE_FILE_EXTENSION_ARRAY = 
        {
            ".rule",
            ".cs",
            ".js",
            ".meta",
            ".svn",
        };

        /// <summary>
        /// 忽略的文件夹
        /// </summary>
        public static readonly string[] IGNORE_FOLDER_ARRAY = 
        {
            ".svn",
        };

        /// <summary>
        ///   ProjectDirectory
        /// </summary>
        public static string ProjectDirectory
        {
            get
            {
                string directory = System.IO.Directory.GetCurrentDirectory() + "/";
                directory = Common.CovertCommonPath(directory);
                return directory;
            }
        }

        /// <summary>
        ///   判断是否需要忽略
        /// </summary>
        public static bool IsIgnoreFile(string extension)
        {
            foreach (string ignore in EditorCommon.IGNORE_FILE_EXTENSION_ARRAY)
            {
                if (extension == ignore)
                    return true;
            }

            return false;
        }

        /// <summary>
        ///   判断是否需要忽略
        /// </summary>
        public static bool IsIgnoreFolder(string full_name)
        {
            string name = System.IO.Path.GetFileName(full_name);
            foreach (string ignore in IGNORE_FOLDER_ARRAY)
            {
                if (name == ignore)
                    return true;
            }

            return false;
        }

        /// <summary>
        ///   Unity/Assets相对路径转换为绝对路径
        /// </summary>
        public static string RelativeToAbsolutePath(string path)
        {
            return ProjectDirectory + path;
        }

        /// <summary>
        ///   绝对路径转换为Unity/Assets相对路径
        /// </summary>
        public static string AbsoluteToRelativePath(string path)
        {
            path = Common.CovertCommonPath(path);
            int last_idx = path.LastIndexOf(ProjectDirectory);
            if (last_idx < 0)
                return path;

            int start = last_idx + ProjectDirectory.Length;
            int length = path.Length - start;
            return path.Substring(start, length);
        }

        /// <summary>
        /// 得到合适的AssetBundleName
        /// </summary>
        public static string ConvertToAssetBundleName(string fullPath)
        {
            if (string.IsNullOrEmpty(fullPath))
                return fullPath;

            int position = fullPath.LastIndexOf(".");
            fullPath = position > -1 ? fullPath.Substring(0, position) : fullPath;
            return fullPath + Common.EXTENSION;
        }


        /// <summary>
        /// 获取平台名称
        /// </summary>
        /// <param name="build_type"></param>
        /// <returns></returns>
        public static string PlatformName(EBuildType build_type)
        {
            if (build_type == EBuildType.StandaloneWindows)
                return "Win";
            else if (build_type == EBuildType.Android)
                return "Android";
            else if (build_type == EBuildType.IOS)
                return "IOS";

            return "Win";
        }


        public static string GetDownloadPath(EBuildType buildType)
        {
            string downloadPath = EditorCommon.DOWNLOAD_PATH;
            downloadPath = string.Format(downloadPath, EditorCommon.PlatformName(buildType));
            return downloadPath;
        }

    }
}