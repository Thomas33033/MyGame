using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.XCodeEditor;
#endif
using System.IO;

public static class XCodePostProcess
{

#if UNITY_EDITOR
	[PostProcessBuild(999)]
	public static void OnPostProcessBuild( BuildTarget target, string pathToBuiltProject )
	{

		if (target != BuildTarget.iOS) {
			Debug.LogWarning("Target is not iPhone. XCodePostProcess will not run");
			return;
		}

		// Create a new project object from build target
		XCProject project = new XCProject( pathToBuiltProject );

		// Find and run through all projmods files to patch the project.
		// Please pay attention that ALL projmods files in your project folder will be excuted!
        string[] files = Directory.GetFiles(Application.dataPath + "/Editor/XUPorter", "*.projmods", SearchOption.AllDirectories);
		foreach( string file in files ) {
			UnityEngine.Debug.Log("ProjMod File: "+file);
			project.ApplyMod( file );
		}

		//TODO disable the bitcode for iOS 9
		project.overwriteBuildSetting("ENABLE_BITCODE", "NO", "Release");
		project.overwriteBuildSetting("ENABLE_BITCODE", "NO", "Debug");
        project.overwriteBuildSetting("ENABLE_BITCODE", "NO", "ReleaseForProfiling");
        project.overwriteBuildSetting("ENABLE_BITCODE", "NO", "ReleaseForRunning");

        //by jason song
        project.overwriteBuildSetting("CLANG_ENABLE_MODULES", "YES");
        project.overwriteBuildSetting("GCC_C_LANGUAGE_STANDARD", "gnu99");

        //TODO implement generic settings as a module option
        project.overwriteBuildSetting("CODE_SIGN_IDENTITY[sdk=iphoneos*]", "iPhone Distribution:", "Release");
        project.overwriteBuildSetting("CODE_SIGN_IDENTITY[sdk=iphoneos*]", "iPhone Distribution", "ReleaseForProfiling");
        project.overwriteBuildSetting("CODE_SIGN_IDENTITY[sdk=iphoneos*]", "iPhone Distribution", "ReleaseForRunning");
        project.overwriteBuildSetting("CODE_SIGN_IDENTITY[sdk=iphoneos*]", "iPhone Developer", "Debug");
        project.overwriteBuildSetting("DEVELOPMENT_TEAM", "6K2V46G7ZP", "Release");
        project.overwriteBuildSetting("DEVELOPMENT_TEAM", "6K2V46G7ZP", "ReleaseForProfiling");
        project.overwriteBuildSetting("DEVELOPMENT_TEAM", "6K2V46G7ZP", "ReleaseForRunning");
        project.overwriteBuildSetting("DEVELOPMENT_TEAM", "6K2V46G7ZP", "Debug");
        project.overwriteBuildSetting("PROVISIONING_PROFILE_SPECIFIER", "boomInHouse", "Release");
        project.overwriteBuildSetting("PROVISIONING_PROFILE_SPECIFIER", "boomInHouse", "ReleaseForProfiling");
        project.overwriteBuildSetting("PROVISIONING_PROFILE_SPECIFIER", "boomInHouse", "ReleaseForRunning");
        project.overwriteBuildSetting("PROVISIONING_PROFILE_SPECIFIER", "boomDev", "Debug");


        //开启推送设置
        project.project.SetSystemCapabilities("com.apple.Push", "1");
        //project.project.SetSystemCapabilities("com.apple.InAppPurchase", "1");

        // Finally save the xcode project
        project.Save();

        // �������汾ʱ������ IPA
        // if(BuildWindow.curSelect == 1)
        // 	IPABuilder.buildIPA();

        //得到xcode工程的路径
        string path = Path.GetFullPath(pathToBuiltProject);

        //编辑代码文件
        //EditorCode(path);
        EditpreStartUnityXCode(path);
        EditOpenURLXCode(path);
        //EditSuitIpXCode(path);

    }
#endif
    private static void EditorCode(string filePath)
    {
        //读取UnityAppController.mm文件
        //XClass UnityAppController = new XClass(filePath + "/Classes/UnityAppController.mm");
        ////在指定代码后面增加一行代码
        //UnityAppController.WriteBelow(
        //    "- (void)applicationDidBecomeActive:(UIApplication*)application\n{",
        //    "\t\n[[UIApplication sharedApplication] setApplicationIconBadgeNumber:1];" +
        //    "\t\n[[UIApplication sharedApplication] setApplicationIconBadgeNumber:0];"
        //);

        //插入代码
        //读取UnityAppController.mm文件
        string src = @"- (void)applicationDidBecomeActive:(UIApplication*)application\n{";
        string dst = @"- (void)applicationDidBecomeActive:(UIApplication*)application\n  {
                        \t\n[[UIApplication sharedApplication] setApplicationIconBadgeNumber:1];
                        \t\n[[UIApplication sharedApplication] setApplicationIconBadgeNumber:0];
   ";

        string unityAppControllerPath = filePath + "/Classes/UnityAppController.mm";
        XClass UnityAppController = new XClass(unityAppControllerPath);
        UnityAppController.Replace(src, dst);
    }

    public static void EditpreStartUnityXCode(string path)
    {
        //插入代码
        //读取UnityAppController.mm文件
        string src = @"- (void)preStartUnity               {}";
        string dst = @"//- (void)preStartUnity                 {};

   - (void)preStartUnity                 {
        UIGestureRecognizer* gr0 = _window.gestureRecognizers[0];  
        if(gr0 != NULL){ gr0.delaysTouchesBegan = false; } 
      
        UIGestureRecognizer* gr1 = _window.gestureRecognizers[1];  
        if(gr1 != NULL){ gr1.delaysTouchesBegan = false; }  
    }
   ";

        string unityAppControllerPath = path + "/Classes/UnityAppController.mm";
        XClass UnityAppController = new XClass(unityAppControllerPath);
        UnityAppController.Replace(src, dst);
    }

    public static void EditOpenURLXCode(string path)
    {
        //插入代码
        //读取UnityAppController.mm文件
        string src = @"- (void)shouldAttachRenderDelegate  {}";
        string dst = @"- (void)shouldAttachRenderDelegate                 {}

- (BOOL)application:(UIApplication *)app openURL:(NSURL *)url options:(NSDictionary<UIApplicationOpenURLOptionsKey,id> *)options
{
    NSMutableArray* keys    = [NSMutableArray arrayWithCapacity: 3];
    NSMutableArray* values  = [NSMutableArray arrayWithCapacity: 3];
#define ADD_ITEM(item)  do{ if(item) {[keys addObject:@#item]; [values addObject:item];} }while(0)
    
    ADD_ITEM(url);
    
#undef ADD_ITEM
    NSDictionary* notifData = [NSDictionary dictionaryWithObjects: values forKeys: keys];
    AppController_SendNotificationWithArg(kUnityOnOpenURL, notifData);
    return YES;
}";

        string unityAppControllerPath = path + "/Classes/UnityAppController.mm";
        XClass UnityAppController = new XClass(unityAppControllerPath);
        UnityAppController.Replace(src, dst);
    }

    public static void EditSuitIpXCode(string path)
    {
        //插入代码
        //读取UnityAppController.mm文件
        string src = @"_window         = [[UIWindow alloc] initWithFrame: [UIScreen mainScreen].bounds];";
        string dst = @"//    _window  = [[UIWindow alloc] initWithFrame: [UIScreen mainScreen].bounds];

   CGRect winSize = [UIScreen mainScreen].bounds;
   if (winSize.size.width / winSize.size.height > 2.15f) {
       winSize.size.width -= 64;
       winSize.origin.x = 32;
       ::printf(""-> is iphonex aaa hello world\n"");
   } else {
       ::printf(""-> is not iphonex aaa hello world\n"");
   }
   _window = [[UIWindow alloc] initWithFrame: winSize];

   ";

        string unityAppControllerPath = path + "/Classes/UnityAppController.mm";
        XClass UnityAppController = new XClass(unityAppControllerPath);
        UnityAppController.Replace(src, dst);
    }

    public static void Log(string message)
	{
		UnityEngine.Debug.Log("PostProcess: "+message);
	}
}

public partial class XClass : System.IDisposable
{
    private string filePath;
    public XClass(string fPath)
    {
        filePath = fPath;
        if (!File.Exists(filePath))
        {
            Debug.LogError(filePath + " 路径下文件不存在");
            return;
        }
    }
    public void WriteBelow(string below, string text)
    {
        StreamReader streamReader = new StreamReader(filePath);
        string text_all = streamReader.ReadToEnd();
        streamReader.Close();

        int beginIndex = text_all.IndexOf(below);
        if (beginIndex == -1)
        {
            Debug.LogError(filePath + " 中没有找到标识 " + below);
            return;
        }
        int endIndex = text_all.LastIndexOf("\n", beginIndex + below.Length);
        text_all = text_all.Substring(0, endIndex) + "\n" + text + "\n" + text_all.Substring(endIndex);
        StreamWriter streamWriter = new StreamWriter(filePath);
        streamWriter.Write(text_all);
        streamWriter.Close();
    }
    public void Replace(string below, string newText)
    {
        StreamReader streamReader = new StreamReader(filePath);
        string text_all = streamReader.ReadToEnd();
        streamReader.Close();

        int beginIndex = text_all.IndexOf(below);
        if (beginIndex == -1)
        {
            Debug.LogError(filePath + " 中没有找到标识 " + below);
            return;
        }

        text_all = text_all.Replace(below, newText);
        StreamWriter streamWriter = new StreamWriter(filePath);
        streamWriter.Write(text_all);
        streamWriter.Close();
    }

    public void Dispose()
    {

    }
}