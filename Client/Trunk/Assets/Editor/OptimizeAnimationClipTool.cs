using System;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using UnityEditor;
using System.IO;

namespace EditorTool
{
    class AnimationOpt
    {
        static Dictionary<uint,string> _FLOAT_FORMAT;
        static MethodInfo getAnimationClipStats;
        static FieldInfo sizeInfo;
        static object[] _param = new object[1];

        static AnimationOpt ()
        {
            _FLOAT_FORMAT = new Dictionary<uint, string> ();
            for (uint i = 1; i < 6; i++) {
                _FLOAT_FORMAT.Add (i, "f" + i.ToString ());
            }
            Assembly asm = Assembly.GetAssembly (typeof(Editor));
            getAnimationClipStats = typeof(AnimationUtility).GetMethod ("GetAnimationClipStats", BindingFlags.Static | BindingFlags.NonPublic);
            Type aniclipstats = asm.GetType ("UnityEditor.AnimationClipStats");
            sizeInfo = aniclipstats.GetField ("size", BindingFlags.Public | BindingFlags.Instance);
        }

        AnimationClip _clip;
        string _path;

        public string path { get{ return _path;} }

        public long originFileSize { get; private set; }

        public int originMemorySize { get; private set; }

        public int originInspectorSize { get; private set; }

        public long optFileSize { get; private set; }

        public int optMemorySize { get; private set; }

        public int optInspectorSize { get; private set; }

        public AnimationOpt (string path, AnimationClip clip)
        {
            _path = path;
            _clip = clip;
            _GetOriginSize ();
        }

        void _GetOriginSize ()
        {
            originFileSize = _GetFileZie ();
            originMemorySize = _GetMemSize ();
            originInspectorSize = _GetInspectorSize ();
        }

        void _GetOptSize ()
        {
            optFileSize = _GetFileZie ();
            optMemorySize = _GetMemSize ();
            optInspectorSize = _GetInspectorSize ();
        }

        long _GetFileZie ()
        {
            FileInfo fi = new FileInfo (_path);
            return fi.Length;
        }

        int _GetMemSize ()
        {
            return (int)UnityEngine.Profiling.Profiler.GetRuntimeMemorySizeLong (_clip);
        }

        int _GetInspectorSize ()
        {
            _param [0] = _clip;
            var stats = getAnimationClipStats.Invoke (null, _param);
            return (int)sizeInfo.GetValue (stats);
        }

        void _OptmizeAnimationScaleCurve ()
        {
            if (_clip != null) {
                //去除scale曲线
                foreach (EditorCurveBinding theCurveBinding in AnimationUtility.GetCurveBindings(_clip)) {
                    string name = theCurveBinding.propertyName.ToLower ();
                    if (name.Contains ("scale")) {
                        AnimationUtility.SetEditorCurve (_clip, theCurveBinding, null);
                        Debug.LogFormat ("关闭{0}的scale curve", _clip.name);
                    }
                } 
            }
        }

        void _OptmizeAnimationFloat_X (uint x)
        {
            if (_clip != null && x > 0) {
                //浮点数精度压缩到f3
                AnimationClipCurveData[] curves = null;
                curves = AnimationUtility.GetAllCurves (_clip);
                Keyframe key;
                Keyframe[] keyFrames;
                string floatFormat;
                if (_FLOAT_FORMAT.TryGetValue (x, out floatFormat)) {
                    if (curves != null && curves.Length > 0) {
                        for (int ii = 0; ii < curves.Length; ++ii) {
                            AnimationClipCurveData curveDate = curves [ii];
                            if (curveDate.curve == null || curveDate.curve.keys == null) {
                                //Debug.LogWarning(string.Format("AnimationClipCurveData {0} don't have curve; Animation name {1} ", curveDate, animationPath));
                                continue;
                            }
                            keyFrames = curveDate.curve.keys;
                            for (int i = 0; i < keyFrames.Length; i++) {
                                key = keyFrames [i];
                                key.value = float.Parse (key.value.ToString (floatFormat));
                                key.inTangent = float.Parse (key.inTangent.ToString (floatFormat));
                                key.outTangent = float.Parse (key.outTangent.ToString (floatFormat));
                                keyFrames [i] = key;
                            }
                            curveDate.curve.keys = keyFrames;
                            _clip.SetCurve (curveDate.path, curveDate.type, curveDate.propertyName, curveDate.curve);
                        }
                    }
                } else {
                    Debug.LogErrorFormat ("目前不支持{0}位浮点", x);
                }
            }
        }

        public void Optimize (bool scaleOpt, uint floatSize)
        {
            if (scaleOpt)
            {
                _OptmizeAnimationScaleCurve();
            }
            _OptmizeAnimationFloat_X (floatSize);
            _GetOptSize ();
        }

        public void Optimize_Scale_Float3(bool scaleOpt)
        {
            Optimize(scaleOpt, 3);
        }

        public void LogOrigin ()
        {
            _logSize (originFileSize, originMemorySize, originInspectorSize);
        }

        public void LogOpt ()
        {
            _logSize (optFileSize, optMemorySize, optInspectorSize);
        }

        public void LogDelta ()
        {

        }

        void _logSize (long fileSize, int memSize, int inspectorSize)
        {
            Debug.LogFormat ("{0} \nSize=[ {1} ]", _path, string.Format ("FSize={0} ; Mem->{1} ; inspector->{2}",
                EditorUtility.FormatBytes (fileSize), EditorUtility.FormatBytes (memSize), EditorUtility.FormatBytes (inspectorSize)));
        }
    }

    public class OptimizeAnimationClipTool
    {
        static List<AnimationOpt> _AnimOptList = new List<AnimationOpt> ();
        static List<string> _Errors = new List<string>();
        static int _Index = 0;

        [MenuItem("Assets/AnimationSplit/裁剪浮点数")]
        public static void Optimize()
        {
            _AnimOptList = FindAnims (false);
            if (_AnimOptList.Count > 0)
            {
                _Index = 0;
                _Errors.Clear ();
                EditorApplication.update = ScanAnimationClip1;
            }
        }

        [MenuItem("Assets/AnimationSplit/裁剪浮点数删除Scale")]
        public static void Optimize1()
        {
            _AnimOptList = FindAnims(true);
            if (_AnimOptList.Count > 0)
            {
                _Index = 0;
                _Errors.Clear();
                EditorApplication.update = ScanAnimationClip2;
            }
        }

        private static void ScanAnimationClip1()
        {
            ScanAnimationClipMain(false);
        }
        private static void ScanAnimationClip2()
        {
            ScanAnimationClipMain(true);
        }
        private static void ScanAnimationClipMain(bool scaleOpt)
        {
            AnimationOpt _AnimOpt = _AnimOptList[_Index];
            bool isCancel = EditorUtility.DisplayCancelableProgressBar("优化AnimationClip", _AnimOpt.path, (float)_Index / (float)_AnimOptList.Count);
            _AnimOpt.Optimize_Scale_Float3(scaleOpt);
            _Index++;
            if (isCancel || _Index >= _AnimOptList.Count)
            {
                EditorUtility.ClearProgressBar();
                Debug.Log(string.Format("--优化完成--    错误数量: {0}    总数量: {1}/{2}    错误信息↓:\n{3}\n----------输出完毕----------", _Errors.Count, _Index, _AnimOptList.Count, string.Join(string.Empty, _Errors.ToArray())));
                Resources.UnloadUnusedAssets();
                GC.Collect();
                AssetDatabase.SaveAssets();
                EditorApplication.update = null;
                _AnimOptList.Clear();
                _cachedOpts.Clear ();
                _Index = 0;
            }
        }

        static Dictionary<string,AnimationOpt> _cachedOpts = new Dictionary<string, AnimationOpt> ();

        static AnimationOpt _GetNewAOpt (string path)
        {
            AnimationOpt opt = null;
            if (!_cachedOpts.ContainsKey(path)) {
                AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip> (path);
                if (clip != null) {
                    opt = new AnimationOpt (path, clip);
                    _cachedOpts [path] = opt;
                }
            }
            return opt;
        }

        public static List<string> es = new List<string> { "huanxiong_Hi@att_idle@att_idle",
"huanxiong_Hi@idle@idle",
"huanxiong_Hi@greeting@greeting",
"huanxiong_Hi@show@show",

"xiaohuangxiong02_hi@greeting@greeting",
"xiaohuangxiong02_Hi@idle@idle",
"xiaohuangxiong02_hi@jiangbei@jiangbei",
"xiaohuangxiong02_hi@jiangbei@jiangbei_idle",
"xiaohuangxiong02_hi@show@show",
"xiaohuangxiong02_Hi@skill_ready@skill_ready",

"qiubite_hi@att_idle@att_idle",
"qiubite_Hi@idle@idle",
"qiubite_Hi@show@show",

"qiubite_Low@attack01@attack01",
"qiubite_Low@attack02@attack02",
"qiubite_Low@attack04@attack04",
"qiubite_Low@attack@attack",
"qiubite_Low@dle@dle",
"qiubite_Low@fall@fall",
"qiubite_Low@hit@hit",
"qiubite_Low@hit_down@hit_down",
"qiubite_Low@hitup@hitup",
"qiubite_Low@idle@idle",
"qiubite_Low@jump@jump",
"qiubite_Low@ready02@ready02",
"qiubite_Low@ready_hold01@ready_hold01",
"qiubite_Low@ready_hold02@ready_hold02",
"qiubite_low@relax@relax",
"qiubite_Low@ruchang@ruchang",
"qiubite_Low@run@run",
"qiubite_Low@skill02@skill02",
"qiubite_Low@skill03@skill03",

"qiubite02_Hi@att_idle@att_idle",
"qiubite02_Hi@show@show",

"qiubite02_low@attack01@attack01",
"qiubite02_low@attack04@attack04",
"qiubite02_Low@walk@walk",
"qiubite02_Low@idle@idle",

"qiubite03_hi@att_idle@att_idle",
"qiubite03_hi@decide@decide",
"qiubite03_hi@enter@enter",
"qiubite03_hi@idle@idle",
"qiubite03_hi@jiangbei_idle@jiangbei",
"qiubite03_hi@show@show",
"qiubite03_hi@skill_ready@skill_ready",

"qiubite03_low@att01@att01",
"qiubite03_low@att01@ready",
"qiubite03_low@att04@att04",

"xiaoluoli_hi@decide@decide",
"xiaoluoli_hi@enter@enter",
"xiaoluoli_hi@show@show",
"xiaoluoli_hi@skill_ready@skill_ready",

"xiaoluoli_Low@attack03@attack03",
"xiaoluoli_Low@birth@birth",
"xiaoluoli_Low@fall@fall",
"xiaoluoli_low@newcome@newcome",

"xiaoluoli02_Hi@enter@enter",
"xiaoluoli02_Hi@show@show",
"xiaoluoli02_Hi@skill_ready@skill_ready",

"xiaoluoli02_Low@attack03@attack03",

"wukong_hi@show@show",

"wukong_low@attack04@attack04",

"wukong02_hi@show@show",
"wukong02_hi@show_idle@show_idle",

"wukong02_low@attack04@attack04",

"wukong03_hi@show@show",
"wukong03_hi@skill_ready@skill_ready",

"wukong03_low@attack4@attack4",
"wukong03_low@birth@birth",
"wukong03_low@ready_hold01@ready_hold01",
"wukong03_low@ready1@ready1",
"wukong03_low@relax@relax",

"shitou_Hi@att_idle@att_idle",
"shitou_Hi@decide@decide",
"shitou_Hi@enter@enter",
"shitou_hi@greeting@greeting",
"shitou_Hi@idle@idle",
"shitou_Hi@jiangbei@jiangbei",
"shitou_Hi@jiangbei_idle@jiangbei_idle",
"shitou_Hi@show@show",
"shitou_Hi@skill_ready@skill_ready",

"shitou_Low@attack01@attack01",
"shitou_Low@attack02@attack02",
"shitou_Low@dazhao01@dazhao01",
"shitou_Low@dazhao02@dazhao02",
"shitou_Low@die@die",
"shitou_Low@fall@fall",
"shitou_Low@hit@hit",
"shitou_Low@hit_down@hit_down",
"shitou_Low@hitup@hitup",
"shitou_Low@idle@idle",
"shitou_Low@jineng01@jineng01",
"shitou_Low@jineng02@jineng02",
"shitou_Low@jump@jump",
"shitou_Low@ready_hold01@ready_hold01",
"shitou_Low@ready_hold02@ready_hold04",
"shitou_low@relax@relax",
"shitou_Low@walk@walk",
"shitou_Low_chuchang@chuchang",

"shitouren02_hi@enter@enter",
"shitouren02_hi@show@show",
"shitouren02_hi@skill_ready@skill_ready",

"shitouren02_low@attack04@attack04",
"shitouren02_low@attack04_down@attack04_down",

"shitouren03_hi@enter@enter",
"shitouren03_hi@greeting@greeting",
"shitouren03_hi@idle@idle",
"shitouren03_hi@jiangbei@jiangbei",
"shitouren03_hi@jiangbei_idle@jiangbei_idle",
"shitouren03_hi@show@show",
"shitouren03_hi@skill_ready@skill_ready",

"shitouren03_low@attack01@attack01",
"shitouren03_low@attack01@ready01",
"shitouren03_low@attack02@attack02",
"shitouren03_low@attack02@ready02",
"shitouren03_low@attack04@attack04",
"shitouren03_low@birth@birth",
"shitouren03_low@die@die",
"shitouren03_low@fall@fall",
"shitouren03_low@hit@hit",
"shitouren03_low@idle@idle",
"shitouren03_low@jump@jump",
"shitouren03_low@land04@land04",
"shitouren03_low@ready04@ready04",
"shitouren03_low@ready_hold01@ready_hold01",
"shitouren03_low@ready_hold02@ready_hold02",
"shitouren03_low@ready_hold04@ready_hold04",
"shitouren03_low@relax@relax",
"shitouren03_low@walk@walk",

"renzhe_hi@show@show",
"luobu@01@01",
"luobu@02@02",
"luobu@03@03",

"Renzhe_Low@birth@birth",
"Renzhe_Low@die@die",
"Renzhe_Low@fall@fall",
"Renzhe_Low@hit@hit",
"Renzhe_Low@hit_down@hit_down",
"Renzhe_Low@hitup@hitup",
"Renzhe_Low@idle@idle",
"Renzhe_Low@jump@jump",
"Renzhe_Low@relax@relax",
"Renzhe_Low@skill01@skill01",
"Renzhe_Low@skill02@skill02",
"Renzhe_Low@skill03@skill03",
"Renzhe_Low@skill04@skill04",
"Renzhe_Low@skill04_end@skill04_end",
"Renzhe_Low@walk@walk",

"daocaoren@show@show",
"daocaoren@show_idle@show_idle",
"renzhe03_hi@enter@enter",

"Yezi_Hi@show@show",

"Yezi_Low@attack01@attack01",
"Yezi_Low@attack01@ready01",
"yezi_Low@attack02@attack02",
"yezi_Low@attack02@land02",
"Yezi_Low@birth@birth",

"yezi02_low@attack01@attack01",
"yezi02_Low@attack01@ready01",
"yezi02_low@attack02@attack02",
"yezi02_Low@attack02@ready02",
"yezi02_low@attack04@attack04",
"yezi02_Low@attack04@ready04",

"xiong_Hi@att_idle@att_idle",
"xiong_Hi@decide@decide",
"xiong_Hi@enter@enter",
"xiong_Hi@greeting@greeting",
"xiong_Hi@idle@idle",
"xiong_Hi@jiangbei@jiangbei",
"xiong_Hi@jiangbei_idle@jiangbei_idle",
"xiong_hi@QBTshow@QBTshow",
"xiong_Hi@show@show",
"xiong_Hi@skill_ready@skill_ready",

"zongxiong02_hi@show@show",

"yemanren_Hi@show@show",
"Yemanren_hi@skill_ready@skill_ready",

"yemanren_Low@attack1@attack1",
"yemanren_Low@attack2@attack2",
"yemanren_Low@attack2@attack2_down",

"yemanren02_low@attack01@attack01",

"yemanren03_hi@show@show",

"bingnv_hi@guide01@guide01",
"bingnv_hi@guide01@guide01_hold",
"bingnv_hi@guide02@guide02",
"bingnv_hi@guide02@guide02_hold",
"bingnv_hi@show@show",
"bingnv_hi@skill_ready@skill_ready",
"guide01_fx",
"guide02_fx",
"yindao_out",
"guide01_hold",
"guide02_hold",

"bingnv02_Hi@show@show",
"bingnv02_Hi@idle@idle",

"bingnv03_hi@show@show",
"bingnv03_hi@idle@idle",

"pohuaizhuanjia_hi@show@show",
"pohuaizhuanjia_hi@idle@idle",
"pohuaizhuanjia_Low@attack1@attack01",
"pohuaizhuanjia_Low@attack1@ready01",

"feimao_low@attack03@attack03",
"feimao_low@attack04@attack04",
"feimao_low@birth@birth",

"feimao03_low@att04@att04",
"feimao03_low@birth@birth",

"leishen02_hi@decide@decide",
"leishen02_hi@enter@enter",
"leishen02_hi@idle@idle",
"leishen02_hi@show@show",
"leishen02_hi@show_idle@show_idle",

"yiran02_hi@att_idle@att_idle",
"yiran02_hi@decide@decide",
"yiran02_hi@enter@enter",
"yiran02_hi@greeting@greeting",
"yiran02_hi@idle@idle",
"yiran02_hi@jiangbei@jiangbei",
"yiran02_hi@jiangbei_idle@jiangbei_idle",
"yiran02_hi@show@show",
"yiran02_hi@skill_ready@skill_ready",

"huofa_hi@att_idle@att_idle",
"huofa_hi@decide@decide",
"huofa_hi@enter@enter",
"huofa_hi@greeting@greeting",
"huofa_hi@idle@idle",
"huofa_hi@jiangbei@jiangbei",
"huofa_hi@jiangbei_idle@jiangbei_idle",
"huofa_hi@show@show",
"huofa_hi@skill_ready@skill_ready",
"show_1702",
"show",

"dafashi02_hi@att_idle@att_idle",
"dafashi02_hi@decide@decide",
"dafashi02_hi@enter@enter",
"dafashi02_hi@greeting@greeting",
"dafashi02_hi@idle@idle",
"dafashi02_hi@jiangbei@jiangbei",
"dafashi02_hi@jiangbei_idle@jiangbei_idle",
"dafashi02_hi@show@show",
"dafashi02_hi@skill_ready@skill_ready",
"wukong03_hi@idle@idle",
"wukong_hi@idle@idle",
"wukong03_low@idle@idle",
"wukong02_low@idle@idle",
};

        static List<AnimationOpt> FindAnims(bool isCal)
        {
            string[] guids = null;
            List<string> path = new List<string>();
            List<AnimationOpt> assets = new List<AnimationOpt> ();
            UnityEngine.Object[] objs = Selection.GetFiltered(typeof(object), SelectionMode.DeepAssets);
            if (objs.Length > 0)
            {
                for(int i = 0; i < objs.Length; i++)
                {

                        if (objs[i].GetType() == typeof(AnimationClip))
                        {
                            string p = AssetDatabase.GetAssetPath(objs[i]);
                            AnimationOpt animopt = _GetNewAOpt(p);
                            if (animopt != null)
                            {
                                if (isCal)
                                {
                                    if (es.Contains(objs[i].name))
                                    {
                                        Debug.Log("-------------" + objs[i].name);
                                    }
                                    else
                                    {
                                        assets.Add(animopt);
                                    }
                                }
                                else
                                {
                                    assets.Add(animopt);
                                }
                            }
                        }
                        else
                        {
                            path.Add(AssetDatabase.GetAssetPath(objs[i]));
                        }
                }
                if(path.Count > 0)
                    guids = AssetDatabase.FindAssets (string.Format ("t:{0}", typeof(AnimationClip).ToString().Replace("UnityEngine.", "")), path.ToArray());
                else
                    guids = new string[]{};
            }
            for(int i = 0; i < guids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath (guids [i]);
                AnimationOpt animopt = _GetNewAOpt (assetPath);
                if (animopt != null)
                    assets.Add (animopt);
            }

            return assets;
        }
    }
}