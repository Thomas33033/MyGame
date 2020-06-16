using UnityEngine;

public static class LuaConst
{


#if UNITY_EDITOR
    public static string luaDir = Application.dataPath + "/LuaScript";                //lua逻辑代码目录
    public static string toluaDir = Application.dataPath + "/ToLua/Lua";        //tolua lua文件目录
#elif UNITY_STANDALONE
     public static string luaDir = string.Format("{0}/Win/Lua", Application.streamingAssetsPath);      
     public static string toluaDir = string.Format("{0}/Win/Tolua/lua", Application.streamingAssetsPath);  
#else
        string[] luaBundles = { "lua", "tolua" };
        foreach (var name in luaBundles)
        {
            string relativePath = "lua/" + name + "." + GyUtility.AssetBundleVariant;
            AssetBundle luaAB = BundleManager.Instance.LoadAssetBundle(relativePath);
            LuaFileUtils.Instance.AddSearchBundle(name, luaAB);
        }
#endif







#if UNITY_STANDALONE
    public static string osDir = "Win";
#elif UNITY_ANDROID
    public static string osDir = "Android";
#elif UNITY_IPHONE
    public static string osDir = "iOS";        
#else
    public static string osDir = "Android";        
#endif

    public static string luaResDir = string.Format("{0}/{1}/Lua", Application.persistentDataPath, Utils.OriginDirectoryName);      //手机运行时lua文件下载目录    

#if UNITY_EDITOR_WIN || NITY_STANDALONE_WIN
    public static string zbsDir = "D:/ZeroBraneStudio/lualibs/mobdebug";        //ZeroBraneStudio目录       
#elif UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
    public static string zbsDir = "/Applications/ZeroBraneStudio.app/Contents/ZeroBraneStudio/lualibs/mobdebug";
#else
    public static string zbsDir = luaResDir + "/mobdebug/";
#endif    

    public static bool openLuaSocket = false;           //是否打开Lua Socket库
    public static bool openZbsDebugger = false;         //是否连接ZeroBraneStudio调试
    public const bool LuaByteMode = true;               //Lua字节码模式-默认关闭 
    public const bool LuaBundleMode = true;             //Lua代码AssetBundle模式
    public const string LuaTempDir = "LuaBundle/";      //临时目录
}