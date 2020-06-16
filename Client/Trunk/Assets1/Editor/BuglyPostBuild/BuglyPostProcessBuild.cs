using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;
#endif
using System;
using System.Diagnostics;
using System.IO;

public class BuglyPostProcessBuild : MonoBehaviour {

#if UNITY_EDITOR

	//IMPORTANT!!!
	//100 is order , it means this one will execute after e.g 1 as default one is 1 
	//it means our script will run after all other scripts got run
	[PostProcessBuild(100)]
	public static void OnPostProcessBuild(BuildTarget target, string pathToBuiltProject)
	{

//		BuildXcodeProject(target, pathToBuiltProject);

//		BuildAndroidProject(target, pathToBuiltProject);

	}

	private static void BuildXcodeProject(BuildTarget target, string pathToBuiltProject){
        #if UNITY_5
        if (target != BuildTarget.iOS) {
            UnityEngine.Debug.LogWarning("Editor: Target is not iPhone. BuglyPostProcessBuild will not run");
            return;
        }
        #else
        if (target != BuildTarget.iOS) {
            UnityEngine.Debug.LogWarning("Editor: Target is not iPhone. BuglyPostProcessBuild will not run");
            return;
        }
        #endif

		string fullPath = Path.GetFullPath(pathToBuiltProject);

		string buglySDKPath = string.Format("{0}/BuglySDK/iOS", Application.dataPath); 
//		string pythonPath = string.Format("{0}/BuglySDK/Editor/XcodePostProcessBuild.py", Application.dataPath);

		UnityEngine.Debug.Log(string.Format("Editor: Post process for {0} in {1}", fullPath, buglySDKPath));

//		Process buildProcess = new Process();
//		buildProcess.StartInfo.FileName = "python";
//		buildProcess.StartInfo.Arguments = string.Format("{0} \"{1}\" \"{2}\"", pythonPath, pathToBuiltProject, buglySDKPath);
//		buildProcess.StartInfo.UseShellExecute = false;
//		buildProcess.StartInfo.RedirectStandardOutput = false;
//		buildProcess.StartInfo.RedirectStandardError = false;
//		buildProcess.StartInfo.CreateNoWindow = true;
//
//		buildProcess.Start();
//		buildProcess.WaitForExit();

		UnityEngine.Debug.Log("Editor: Post process end.");
	}

	private static void BuildAndroidProject(BuildTarget target, string pathToBuiltProject){
		if (target != BuildTarget.Android) {
			UnityEngine.Debug.LogWarning("Editor: Target is not Android. BuglyPostProcessBuild will not run");
			return;
		}

        string fullPath = Path.GetFullPath(pathToBuiltProject);

        UnityEngine.Debug.LogWarningFormat ("dataPath: {0}, pathToBuiltProject: {1}", Application.dataPath, fullPath);
	}

#endif
}
