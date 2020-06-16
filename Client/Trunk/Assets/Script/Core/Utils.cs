using UnityEngine;
using System;
using System.Text;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using LuaInterface;
using UnityEngine.UI;

public static class Utils
{
    public const int NORMAL_FRAMERATE = 30;
    public const int LIMIT_FRAMERATE = 10;

    /// <summary>
    /// 文件源;
    /// </summary>
    public static string OriginDirectoryName
    {
        get
        {
            return "Resources";
        }
    }

    /// <summary>
    /// 返回当前的平台名;
    /// </summary>
    public static string PlatformName
    {
        get
        {
            return LuaConst.osDir;
        }
    }

    /// <summary>
    /// 安装包原始数据目录;
    /// </summary>
    public static string WWWFromStreamingAssetsPath
    {
        get
        {
            string path = string.Empty;
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    path = string.Format("jar:file://{0}!/assets", Application.dataPath);
                    break;
                default:
                    path = string.Format("file:///{0}", Application.streamingAssetsPath);
                    break;
            }
            return path;
        }
    }

    /// <summary>
    /// 取得数据存放目录;
    /// </summary>
    private static string m_dataPath = "";
    public static string DataPath
    {
        get
        {
            if (string.IsNullOrEmpty(m_dataPath))
            {
#if UNITY_EDITOR
                m_dataPath = System.IO.Path.GetFullPath(Application.dataPath + "/../PersistentData");
                if (!Directory.Exists(m_dataPath))
                    Directory.CreateDirectory(m_dataPath);
#else
                    m_dataPath = Application.persistentDataPath;
#endif
            }
            return m_dataPath;
        }
    }
    private static string m_dataResourcePath = "";
    public static string DataResourcePath
    {
        get
        {
            if (string.IsNullOrEmpty(m_dataResourcePath))
            {
                m_dataResourcePath = string.Format("{0}/{1}", DataPath, OriginDirectoryName);
            }

            return m_dataResourcePath;
        }
    }

    public static string LuaDataResourcePath
    {
        get
        {
#if UNITY_EDITOR
            return Application.dataPath + "/Lua";
#else
                return DataResourcePath;
#endif
        }
    }

    public static string AssetBundleVariant = "gatecen";// AssetBundle的扩展名;

    /// <summary>
    /// 可持续化目录下AssetBundle的加载路径;
    /// </summary>
    /// <param name="fileName">相对assetbundles目录，带后缀名;</param>
    /// <returns>绝对路径;</returns>
    public static string GetAssetBundlePath(string fileName)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(DataResourcePath);
        sb.Append("/assetbundles/");
        sb.Append(fileName);
        return sb.ToString();
    }

    /// <summary>
    /// 安装包内AssetBundle的加载路径;
    /// </summary>
    /// <param name="fileName">相对assetbundles目录，带后缀名;</param>
    /// <returns>绝对路径;</returns>
    public static string GetAssetBundleStreamingAssetsPath(string fileName)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(Application.streamingAssetsPath);
        sb.Append("/");
        sb.Append(PlatformName);
        sb.Append("/");
        sb.Append(OriginDirectoryName);
        sb.Append("/assetbundles/");
        sb.Append(fileName);
        return sb.ToString();
    }

    public static LuaByteBuffer ReadAllBytesToLuaStringBuffer(string configPath)
    {
        LuaByteBuffer buffer = null;

        try
        {
            string path = string.Format("{0}/{1}", DataResourcePath, configPath);
            byte[] stream = File.ReadAllBytes(path);
            buffer = new LuaByteBuffer(stream);
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }

        return buffer;
    }

    /// <summary>
    /// 计算文件的MD5值;
    /// </summary>
    public static string GenerateFileMd5(string file)
    {
        try
        {
            using (FileStream fs = new FileStream(file, FileMode.Open))
            {
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(fs);
                fs.Close();

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }
        catch (Exception ex)
        {
            throw new Exception(string.Format("md5file() fail, error: {0}", ex.Message));
        }
    }

    public static void ClearDirectory(string srcPath)
    {
        try
        {
            DirectoryInfo dir = new DirectoryInfo(srcPath);
            FileSystemInfo[] fileinfos = dir.GetFileSystemInfos();  //返回目录中所有文件和子目录;
            for (int i = 0, count = fileinfos.Length; i < count; ++i)
            {
                FileSystemInfo fs = fileinfos[i];
                if (fs is DirectoryInfo)            //判断是否文件夹;
                {
                    DirectoryInfo subdir = new DirectoryInfo(fs.FullName);
                    subdir.Delete(true);          //删除子目录和文件;
                }
                else
                {
                    File.Delete(fs.FullName);      //删除指定文件;
                }
            }
        }
        catch (Exception e)
        {
            throw e;
        }
    }

    /// <summary>
    /// 批量拷贝文件
    /// </summary>
    /// <param name="desc">进度条描述</param>
    /// <param name="from">源目录</param>
    /// <param name="to">目标目录</param>
    /// <param name="include">包含字符</param>
    /// <param name="exclude">排除字符</param>
    /// <param name="recursive">搜索子目录</param>
    public static void CopyFiles(string desc, string from, string to, string include = "", string exclude = ".meta|.manifest", bool recursive = true)
    {
#if UNITY_EDITOR
        UnityEditor.EditorUtility.ClearProgressBar();
#endif
        List<string> fileList = new List<string>();
        fileList.Clear();

        if (recursive)
        {
            recursivePath(from, fileList);
        }
        else
        {
            string[] names = Directory.GetFiles(from);
            int count = names.Length;
            foreach (var name in names)
            {
                string ext = Path.GetExtension(name);
                fileList.Add(name.Replace('\\', '/'));
            }
        }

        if (fileList.Count == 0)
        {
#if UNITY_EDITOR
            UnityEditor.EditorUtility.ClearProgressBar();
#endif
            return;
        }

        string[] includeList = null;
        string[] excludeList = null;

        if (!string.IsNullOrEmpty(include))
        {
            includeList = include.Split(new char[] { '|' });
        }

        if (!string.IsNullOrEmpty(exclude))
        {
            excludeList = exclude.Split(new char[] { '|' });
        }

        int total = fileList.Count;
        int cur = 0;
        foreach (string f in fileList)
        {
#if UNITY_EDITOR
            UnityEditor.EditorUtility.DisplayProgressBar(desc, f, (float)(cur++) / (float)total);
#endif
            if (includeList != null)
            {
                bool fix = false;
                foreach (var ext in includeList)
                {
                    if (f.IndexOf(ext) >= 0)
                    {
                        fix = true;
                        break;
                    }
                }
                if (!fix)
                    continue;
            }

            if (excludeList != null)
            {
                bool fix = false;
                foreach (var ext in excludeList)
                {
                    if (f.IndexOf(ext) >= 0)
                    {
                        fix = true;
                        break;
                    }
                }
                if (fix)
                    continue;
            }

            string newfile = f.Replace(from, to);
            string path = Path.GetDirectoryName(newfile);
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            if (File.Exists(newfile)) { File.Delete(newfile); }
            File.Copy(f, newfile, true);
        }

#if UNITY_EDITOR
        UnityEditor.EditorUtility.ClearProgressBar();
#endif
    }

    /// <summary>
    /// 遍历目录及其子目录得到所有的文件;
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static List<string> RecursivePathGetFiles(string path)
    {
        List<string> fileList = new List<string>();
        fileList.Clear();

        recursivePath(path, fileList);

        return fileList;
    }

    /// <summary>
    /// 遍历目录及其子目录;
    /// </summary>
    private static void recursivePath(string path, List<string> fileList)
    {
        string[] names = Directory.GetFiles(path);
        string[] dirs = Directory.GetDirectories(path);

        int count = names.Length;
        for (int i = 0; i < count; ++i)
        {
            string ext = Path.GetExtension(names[i]);
            if (ext.Equals(".meta")) { continue; }
            fileList.Add(names[i].Replace('\\', '/'));
        }

        count = dirs.Length;
        for (int i = 0; i < count; ++i)
        {
            recursivePath(dirs[i], fileList);
        }
    }

    public static bool isFloatZero(float val)
    {
        return (Mathf.Abs(val) < 0.000001f);
    }

    /// <summary>
    /// 清空List;
    /// </summary>
    /// <param name="list"></param>
    public static void ClearGameObjectList(List<GameObject> list)
    {
        for (int i = list.Count - 1; i >= 0; --i)
        {
            GameObject.Destroy(list[i]);
            list[i] = null;
        }

        list.Clear();
    }

    /// <summary>
    /// 主动回收清理内存;
    /// </summary>
    public static void ClearMemory()
    {
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
    }

    /// <summary>
    /// 清空所有的本地数据;
    /// </summary>
    public static void ClearLocalDatas()
    {
        if (Directory.Exists(Utils.DataPath)) { Directory.Delete(Utils.DataPath, true); }
        Directory.CreateDirectory(Utils.DataPath);
        PlayerPrefs.DeleteAll();
    }

    /// <summary>
    /// 给指定的物体的所有Text组件附上指定的字体;
    /// </summary>
    /// <param name="obj">指定的物体;</param>
    /// <param name="font">指定的字体;</param>
    public static void SetComponentsInChildrenFont(GameObject obj, Font font)
    {
        Text[] testArr = obj.GetComponentsInChildren<Text>(true);
        int count = testArr.Length;

        for (int i = 0; i < count; ++i)
        {
            testArr[i].font = font;
        }
    }

    public static Texture2D LoadTexture2D(string imagePath, int width, int height)
    {
        string uri = string.Format("{0}/{1}", DataResourcePath, imagePath);

        byte[] stream = File.ReadAllBytes(uri);
        Texture2D t2d = new Texture2D(width, height);
        t2d.LoadImage(stream);

        return t2d;
    }

    /// <summary>
    /// 获取子物体;
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="childName"></param>
    /// <returns></returns>
    public static GameObject FindChildGameObjectByName(GameObject parent, string childName)
    {
        if (parent == null) { return null; }
        if (parent.name == childName) { return parent; }

        Transform parentT = parent.transform;
        int count = parentT.childCount;
        if (count < 1) { return null; }

        GameObject obj = null;
        for (int i = 0; i < count; ++i)
        {
            GameObject go = parentT.GetChild(i).gameObject;
            obj = FindChildGameObjectByName(go, childName);
            if (obj != null) { break; }
        }

        return obj;
    }

    public static Animator FindFirstAnimator(Transform parent)
    {
        Animator[] anims = parent.GetComponentsInChildren<Animator>();

        if (anims != null && anims.Length > 0)
        {
            return anims[0];
        }

        return null;
    }

    //from ulua
    /// <summary>
    /// 取得Lua路径;
    /// </summary>
    public static string LuaPath(string luaName)
    {
        string path = string.Format("{0}/Lua", DataResourcePath);
        if (luaName.EndsWith(".lua"))
        {
            int index = luaName.LastIndexOf('.');
            luaName = luaName.Substring(0, index);
        }
        luaName = luaName.Replace('.', '/');

        return string.Format("{0}/{1}.lua", path, luaName);
    }

    /// <summary>
    /// 读取Lua路径下指定的文件数据
    /// </summary>
    /// <param name="filename">文件名</param>
    /// <returns></returns>
    public static LuaInterface.LuaByteBuffer LuaReadFile(string filename)
    {
        byte[] data = LuaFileUtils.Instance.ReadFile(filename);
        return new LuaInterface.LuaByteBuffer(data);
    }

    public static void SendNotification(string notifyID, object arg1, object arg2, object arg3, object arg4, object arg5)
    {
        if (LuaManager.isLuaInitializeFinished)
        {
            LuaManager.Instance.CallFunction("LuaCsharpRouter.OnFromCsharpNotification", notifyID, arg1, arg2, arg3, arg4, arg5);
        }
        else
        {
            //Debug.LogError(string.Format("lua管理器还没有准备好，无法分发事件到lua：{0}", notifyID));
        }
    }

    /// <summary>
    /// 清理内存;
    /// </summary>
    public static void LuaClearMemory()
    {
        LuaManager mgr = LuaManager.Instance;
        if (mgr != null && mgr.lua != null) mgr.LuaGC();

        GC.Collect(0);
        GC.Collect(1);

        Resources.UnloadUnusedAssets();
    }
    
    public static string deviceUniqueIdentifier//玩家的唯一的设备标识符;
    {
        get { return SystemInfo.deviceUniqueIdentifier; }
    }

    public static string deviceType//注册的设备类型(ios、andriod);
    {
        get { return Enum.GetName(typeof(RuntimePlatform), Application.platform); }
    }

    /// <summary>
    /// 是否无限接近目标;
    /// </summary>
    public static bool isInfiniteApproach(Vector3 from, Vector3 to, float approach)
    {
        Vector3 v = to - from;
        return ((Mathf.Abs(v.x) < approach) && (Mathf.Abs(v.y) < approach) && (Mathf.Abs(v.z) < approach));
    }

    /// <summary>
    /// 获取两点之间的一个点;
    /// </summary>
    public static Vector3 BetweenPoint(Vector3 start, Vector3 end, float distance)
    {
        Vector3 normal = (end - start).normalized;
        return normal * distance + start;
    }

    public static bool isNormalFloat(float value)
    {
        string vs = value.ToString();
        if (vs == float.NaN.ToString())
        {
            Debug.LogError("float.NaN");
            return false;
        }
        else if (vs == float.NegativeInfinity.ToString())
        {
            Debug.LogError("float.NegativeInfinity");
            return false;
        }
        else if (vs == float.PositiveInfinity.ToString())
        {
            Debug.LogError("float.PositiveInfinity");
            return false;
        }

        return true;
    }

    public static Vector2 RadiusAngleToPos(float radius, float angle)
    {
        angle = angle / 180 * Mathf.PI;
        Vector2 v2 = new Vector2();
        v2.x = radius * Mathf.Cos(angle);
        v2.y = radius * Mathf.Sin(angle);

        return v2;
    }

    public static string StringReplace(string src, string oldValue, string newValue)
    {
        return src.Replace(oldValue, newValue);
    }

    public static bool StringStartWith(string src, string value)
    {
        return src.StartsWith(value);
    }

    public static bool StringEndWith(string src, string value)
    {
        return src.EndsWith(value);
    }

    public static string ContentToNormalString(string format, string args)
    {
        string dec = format;
        if (!string.IsNullOrEmpty(args))
        {
            string[] split = args.Split(',');

            dec = string.Format(format, split);
        }

        return dec;
    }

    public static void AudioManagerPlaySound(string eventName)
    {
        int val = PlayerPrefs.GetInt("SoundOnOff", 1);
        if (val == 1)
        {
            //SoundTeam.AudioManager.Instance.PlaySound(eventName);
        }
    }

    public static void AudioSetVoiceSwitch(string switchValue)
    {
        //AudioCtrl.SetVoiceSwitch(switchValue);
    }

    public static void Vibrate()
    {
        //Handheld.Vibrate();
    }


    public static bool isiOS()
    {
        return (Application.platform == RuntimePlatform.IPhonePlayer);
    }

    public static bool isAndroid()
    {
        return (Application.platform == RuntimePlatform.Android);
    }

    //是否为windows平台
    public static bool IsUnityWindowsPlatform()
    {

#if UNITY_STANDALONE
        return true;
#else
            return false;
#endif
    }

    public static bool isUnityEditor()
    {
#if UNITY_EDITOR
        return true;
#else
            return false;
#endif
    }

    public static bool isUnityEditorHaoLei()
    {
#if UNITY_EDITOR && HAOLEI
            return true;
#else
        return false;
#endif
    }

    [DllImport("__Internal")]
    static extern float IOS_GetBatteryLevel();

    public static float GetBatteryLevel()
    {
#if UNITY_EDITOR
        return 1f;
#elif UNITY_STANDALONE
            return 1f;
#elif UNITY_IPHONE
			return IOS_GetBatteryLevel ();
#elif UNITY_ANDROID
            try
            {
                using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                {
                    if (null != unityPlayer)
                    {
                        using (AndroidJavaObject currActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                        {
                            if (null != currActivity)
                            {
                                using (AndroidJavaObject intentFilter = new AndroidJavaObject("android.content.IntentFilter", new object[]{ "android.intent.action.BATTERY_CHANGED" }))
                                {
                                    using (AndroidJavaObject batteryIntent = currActivity.Call<AndroidJavaObject>("registerReceiver", new object[]{null,intentFilter}))
                                    {
                                        int level = batteryIntent.Call<int>("getIntExtra", new object[]{"level",-1});
                                        int scale = batteryIntent.Call<int>("getIntExtra", new object[]{"scale",-1});
 
                                        if (level <= 0) { level = 0; }
                                        if (scale <= 0) { scale = 1; }
                                        return ((float)level / scale);
                                    }
                             
                                }
                            }
                        }
                    }
                }
            } 
            catch (System.Exception ex)
            {
                Debug.LogError("GetBatteryLevel"+ex+"  "+SystemInfo.deviceName);
            }

            return 1;
#endif
    }

    public static void SetShadowShadowQuality(int index)
    {
        if (index == 2)
        {
            QualitySettings.shadows = ShadowQuality.All;
        }
        else if (index == 1)
        {
            QualitySettings.shadows = ShadowQuality.HardOnly;
        }
        else if (index == 0)
        {
            QualitySettings.shadows = ShadowQuality.Disable;
        }
    }

    public static void SetAmbientLight(Color color)
    {
        RenderSettings.ambientLight = color;
    }

    public static Color GetAmbientLight()
    {
        return RenderSettings.ambientLight;
    }

    public static void SetAmbientLightValues(float r, float g, float b, float a)
    {
        Color color = new Color(r, g, b, a);
        RenderSettings.ambientLight = color;
    }

    public static void SetShadowDistance(float val)
    {
        QualitySettings.shadowDistance = val;
    }

    public static int NunberAnd(int left, int right)
    {
        return (left & right);
    }

    public static void SetMasterTextureLimit(int index)
    {
        QualitySettings.masterTextureLimit = index;
    }

    public static int InternetReachability()
    {
        return (int)Application.internetReachability;
    }

    private static DateTime startTime = new System.DateTime(1970, 1, 1).ToLocalTime();

    public static int ConvertDataTimeInt(System.DateTime time)
    {
        int seconds = (int)(time - startTime).TotalSeconds;
        return seconds;
    }

    public static int GetNowSecond()
    {
        return ConvertDataTimeInt(System.DateTime.Now);
    }


    //基于锚点对齐UI
    public static void AnchorTo(RectTransform target, Vector2 targetAnchor, RectTransform source, Vector2 sourceAnchor, Vector2 offset)
    {
        if (target == null || source == null)
            return;

        target.position = source.position;

        targetAnchor -= (target.pivot - Vector2.one * 0.5f) * 2f;

        sourceAnchor -= (source.pivot - Vector2.one * 0.5f) * 2f;

        Vector2 targetOffset = new Vector2(targetAnchor.x * target.rect.width, targetAnchor.y * target.rect.height) * 0.5f;

        Vector2 sourceOffset = new Vector2(sourceAnchor.x * source.rect.width, sourceAnchor.y * source.rect.height) * 0.5f;

        target.anchoredPosition = target.anchoredPosition - targetOffset + sourceOffset + offset;
    }


    //十六进制颜色转Color 
    public static Color TryParseHtmlString(string color16)
    {
        Color nowColor;
        ColorUtility.TryParseHtmlString(color16, out nowColor);
        return nowColor;
    }

    //---------------------------------------------------
}



/// <summary>
/// 层管理类;
/// </summary>
public static class GyLayer
{
    public static string LayerDefaultName = "Default";
    public static int LayerDefaultInt = (1 << 0);

    public static string LayerUIName = "UI";
    public static int LayerUIInt = (1 << 5);

    public static string LayerHideName = "Hide";
    public static int LayerHideInt = (1 << 8);

    public static string LayerMapName = "Map";
    public static int LayerMapInt = (1 << 9);

    public static string LayerMiniMapName = "MiniMap";
    public static int LayerMiniMapInt = (1 << 10);

    public static string LayerHighModelName = "HighModel";
    public static int LayerHighModelInt = (1 << 11);

    public static string LayerHighModel2Name = "HighModel2";
    public static int LayerHighModel2Int = (1 << 16);

    public static string LayerBattleUIName = "BattleUI";
    public static int LayerBattleUIInt = (1 << 15);

    public static string LayerRTUIName = "RT";
    public static int LayerRTInt = (1 << 17);

    /// <summary>
    /// 设置物体的层;
    /// </summary>
    public static void SetLayer(GameObject go, string layerName)
    {
        int layer = LayerMask.NameToLayer(layerName);
        SetLayerInt(go, layer);
    }
    public static void SetLayerInt(GameObject go, int layer)
    {
        go.layer = layer;

        foreach (Transform child in go.transform)
        {
            SetLayerInt(child.gameObject, layer);
        }
    }
}
