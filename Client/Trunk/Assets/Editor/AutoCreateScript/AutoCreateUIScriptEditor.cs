#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;
using System;
using System.Text;

/// <summary>
/// 自动生成luaUI脚本
/// </summary>
public class AutoCreateUIScriptEditor : EditorWindow
{
    public static string LuaDirectory = System.Environment.CurrentDirectory + "/Assets/LuaScript/UI";
    public static string SceneDirectory = System.Environment.CurrentDirectory + "/Assets/LuaScript/Space";
    public static string LuaGameDirectory = System.Environment.CurrentDirectory + "/Assets/LuaScript";
    public static string LuaConfig = System.Environment.CurrentDirectory + "/Assets/LuaScript/Configs";
    public static string LuaLogicConfig = System.Environment.CurrentDirectory + "/Assets/LuaScript/Logic";

    static private List<Type> listUICompent = new List<Type>() {
        typeof(Toggle),
        typeof(Button), 
        typeof(Text), 
        typeof(InputField),
        typeof(ScrollRect),
        typeof(Slider),
        typeof(Image),
        typeof(InputField),
        typeof(Slider), 
        typeof(Image),
        typeof(InputField),
        typeof(LoopVerticalScrollRect),
        typeof(LoopHorizontalScrollRect),
    };



    [MenuItem("Assets/自动生成/UI脚本（Lua）")]
    public static void CreateUIScriptLua()
    {
        CreateUIScriptLua(true);
    }

    [MenuItem("Assets/自动生成/ItemRender（lua）")]
    public static void CreateItemScriptLua()
    {
        CreateItemScript();
    }

    [MenuItem("Assets/自动生成/场景脚本（lua）")]
    public static void CreateSceneScriptLua()
    {
        CreateSceneScrip();
    }

    [MenuItem("GameObject/自动生成/场景脚本（lua）", false, 0)]
    public static void CreateSceneScriptLua1()
    {
        CreateSceneScrip();
    }

    public static void CreateSceneScrip()
    {
        GameObject obj = Selection.activeGameObject;
        if (obj == null)
            return;

        Dictionary<string, string> dic = new Dictionary<string, string>();
        bool bSuccess = FindUICompent(obj.transform, dic);
        if (!bSuccess) return;

        string moduleName = obj.name;

        string sPath = string.Format("{0}/{1}", SceneDirectory, moduleName);
        if (!System.IO.Directory.Exists(sPath))
        {
            System.IO.Directory.CreateDirectory(sPath);
        }

        if (!System.IO.Directory.Exists(sPath + "/Base"))
        {
            System.IO.Directory.CreateDirectory(sPath + "/Base");
        }


        //生成场景视图
        string SceneName = obj.name;
        string SceneNameView = SceneName + "View";
        string className = string.Format("{0}/Base/{1}.lua", sPath, SceneNameView);

        using (System.IO.FileStream fs = new System.IO.FileStream(className, System.IO.FileMode.Create))
        {
            System.IO.StreamWriter sw = new System.IO.StreamWriter(fs);
            System.Text.StringBuilder sBuilder = new System.Text.StringBuilder();

            sBuilder.AppendLine("local "+ SceneNameView + " = { }");
            sBuilder.AppendLine("setmetatable("+ SceneNameView + ", { __index = SpaceBase})");

            sBuilder.AppendLine("function " + SceneNameView + ":Create()");
            sBuilder.AppendLine(string.Format("\tself.name = \"{0}\"", SceneName));
            sBuilder.AppendLine(string.Format("\tself.path = \"{0}\"", SceneName));
            sBuilder.AppendLine("end");

            sBuilder.AppendLine("");
            CreateUIScripte(dic, sBuilder, SceneNameView);

            sBuilder.AppendLine(string.Format("return {0}", SceneNameView));

            sw.Write(sBuilder);
            sw.Close();
            fs.Close();
        }

        //生成场景逻辑类
        //生成UI界面逻辑类
        className = string.Format("{0}/{1}.lua", sPath, SceneName);
        //if (!System.IO.File.Exists(className))
        {
            using (System.IO.FileStream fs = new System.IO.FileStream(className, System.IO.FileMode.Create))
            {
                System.IO.StreamWriter sw = new System.IO.StreamWriter(fs);
                System.Text.StringBuilder sBuilder = new System.Text.StringBuilder();

                sBuilder.AppendLine(SceneName + " = {}");
                sBuilder.AppendLine(string.Format("local UITable = require \"Space/{0}/Base/{1}\"", SceneName, SceneNameView));
                sBuilder.AppendLine();

                sBuilder.AppendLine(string.Format("function {0}.Create()", SceneName));

                sBuilder.AppendLine("\tlocal map = {}");
                sBuilder.AppendLine("\tsetmetatable(map, { __index = UITable})");
                sBuilder.AppendLine("\tmap: Create()");
                sBuilder.AppendLine("\tSpace3D.LoadSpace(map)");
                sBuilder.AppendLine("\treturn map");
                sBuilder.AppendLine("end");

                sBuilder.AppendLine();

                sBuilder.AppendLine("function SpaceTable:Awake()");
                sBuilder.AppendLine(string.Format("\t{0}.curMap = self", SceneName));
                sBuilder.AppendLine("end");

                sBuilder.AppendLine();
                sBuilder.AppendLine("function SpaceTable:Start()");
                sBuilder.AppendLine("\tself:StartInit()");
                sBuilder.AppendLine("end");

                sBuilder.AppendLine();
                sBuilder.AppendLine("function SpaceTable:SetData(v)");
                sBuilder.AppendLine();
                sBuilder.AppendLine("end");

                sBuilder.AppendLine();
                sBuilder.AppendLine("function SpaceTable:ButtonClickHandler(btn)");
                sBuilder.AppendLine();
                sBuilder.AppendLine("end");

                sBuilder.AppendLine();
                sBuilder.AppendLine("function UITable:OnClose()");
                sBuilder.AppendLine();
                sBuilder.AppendLine("end");


                sw.Write(sBuilder);
                sw.Close();
                fs.Close();
            }
        }
        Debug.Log("生成完毕");
    }



    public static void CreateItemScript()
    {
        GameObject obj = Selection.activeGameObject;
        if (obj == null)
            return;
        Dictionary<string, string> dic = new Dictionary<string, string>();
        bool bSuccess = FindUICompent(obj.transform, dic);
        if (!bSuccess) return;

        string path = AssetDatabase.GetAssetPath(obj);
        string moduleName = Path.GetFileName(Path.GetDirectoryName(path));

        string sPath = string.Format("{0}/{1}", LuaDirectory, moduleName);
        if (!System.IO.Directory.Exists(sPath + "/View"))
        {
            System.IO.Directory.CreateDirectory(sPath + "/View");
        }

        //生成Item界面视图
        string UIName = obj.name;
        string UINameView = UIName + "Render";
        string className = string.Format("{0}/View/{1}.lua", sPath, UINameView);
            
        if (System.IO.File.Exists(className))
        {
            Debug.Log(UIName + "文件已存在,请手动修改");
            return;
        }

        using (System.IO.FileStream fs = new System.IO.FileStream(className, System.IO.FileMode.Create))
        {
            System.IO.StreamWriter sw = new System.IO.StreamWriter(fs);
            System.Text.StringBuilder sBuilder = new System.Text.StringBuilder();

            sBuilder.AppendLine("local " + UINameView + " = {}");
            sBuilder.AppendLine("local Render = UI.CreateRenderTable()");
            sBuilder.AppendLine("");
            sBuilder.AppendLine(string.Format("function {0}.Create(tf)", UINameView));
            sBuilder.AppendLine("\tlocal t = { }");
            sBuilder.AppendLine("\tsetmetatable(t, Render)");
            sBuilder.AppendLine("\tt: Init(tf)");
            sBuilder.AppendLine("\treturn t");
            sBuilder.AppendLine("end");

            sBuilder.AppendLine("");
            sBuilder.AppendLine("function Render:Awake()");
            sBuilder.AppendLine("end");

            sBuilder.AppendLine("");
            CreateUIScripte(dic, sBuilder, "Render");

            sBuilder.AppendLine("function Render:ButtonClickHandler(btn)");
            sBuilder.AppendLine("end");

            sBuilder.AppendLine("");
            sBuilder.AppendLine("function Render:SetVisible(bVisible)");
            sBuilder.AppendLine("\tself.transform.gameObject:SetActive(bVisible)");
            sBuilder.AppendLine("end");
            
            sw.Write(sBuilder);
            sw.Close();
            fs.Close();
        }
        Debug.Log("生成完毕");
    }


    public static void CreateUIScriptLua(bool isLua)
    {
        for (int i = 0; i < Selection.objects.Length; ++i)
        {
            GameObject obj = Selection.objects[i] as GameObject;

            Dictionary<string, string> dic = new Dictionary<string, string>();
            bool bSuccess = FindUICompent(obj.transform, dic);

            if (!bSuccess) return;

            if (isLua)//生成Lua部分;
            {
                string path = AssetDatabase.GetAssetPath(obj);

                string moduleName = Path.GetFileName(Path.GetDirectoryName(path));

                string sPath = string.Format("{0}/{1}", LuaDirectory, moduleName);
                if (!System.IO.Directory.Exists(sPath))
                {
                    System.IO.Directory.CreateDirectory(sPath);
                }

                if (!System.IO.Directory.Exists(sPath + "/View"))
                {
                    System.IO.Directory.CreateDirectory(sPath + "/View");
                }

                if (!System.IO.Directory.Exists(sPath+ "/View/Base"))
                {
                    System.IO.Directory.CreateDirectory(sPath + "/View/Base");
                }

                if (!System.IO.Directory.Exists(sPath + "/Controller"))
                {
                    System.IO.Directory.CreateDirectory(sPath + "/Controller");
                }

                if (!System.IO.Directory.Exists(sPath + "/Model"))
                {
                    System.IO.Directory.CreateDirectory(sPath + "/Model");
                }


                string UIName = obj.name;

                //生成UI界面视图
                string UINameView = UIName + "View";
                string className = string.Format("{0}/View/Base/{1}.lua", sPath, UINameView);

                using (System.IO.FileStream fs = new System.IO.FileStream(className, System.IO.FileMode.Create))
                {
                    System.IO.StreamWriter sw = new System.IO.StreamWriter(fs);
                    System.Text.StringBuilder sBuilder = new System.Text.StringBuilder();

                    sBuilder.AppendLine("local " + UINameView + " = {}");
                    sBuilder.AppendLine("setmetatable(" + UINameView + ", {__index=UIBase})");
                    sBuilder.AppendLine("");

                    sBuilder.AppendLine(string.Format("function {0}: Create()", UINameView));
                    sBuilder.AppendLine(string.Format("\tself.name = {0}", UIName));
                    sBuilder.AppendLine(string.Format("\tself.path = {0}", moduleName));
                    sBuilder.AppendLine("end");

                    CreateUIScripte(dic, sBuilder, UINameView);

                    sBuilder.AppendLine(string.Format("return {0}", UINameView));

                    sw.Write(sBuilder);
                    sw.Close();
                    fs.Close();
                }

                //生成UI界面逻辑类
                className = string.Format("{0}/View/{1}.lua", sPath, UIName);
                if (!System.IO.File.Exists(className))
                {
                    using (System.IO.FileStream fs = new System.IO.FileStream(className, System.IO.FileMode.Create))
                    {
                        System.IO.StreamWriter sw = new System.IO.StreamWriter(fs);
                        System.Text.StringBuilder sBuilder = new System.Text.StringBuilder();

                        sBuilder.AppendLine(UIName + " = {}");
                        sBuilder.AppendLine(string.Format("local UITable = require \"UI/{1}/View/Base/{2}View\"", moduleName, moduleName, UIName));
                        sBuilder.AppendLine();

                        sBuilder.AppendLine(string.Format("function {0}.Create()", UIName));

                        sBuilder.AppendLine("\tlocal ui = {}");
                        sBuilder.AppendLine("\tsetmetatable(ui, { __index = UITable})");
                        sBuilder.AppendLine("\tui: Create()");
                        sBuilder.AppendLine("\tUI.LoadUI(ui)");
                        sBuilder.AppendLine("\treturn ui");
                        sBuilder.AppendLine("end");

                        sBuilder.AppendLine();

                        sBuilder.AppendLine("function UITable:Awake()");
                        sBuilder.AppendLine();
                        sBuilder.AppendLine("end");

                        sBuilder.AppendLine();
                        sBuilder.AppendLine("function UITable:Start()");
                        sBuilder.AppendLine("\tself:StartInit()");
                        sBuilder.AppendLine();
                        sBuilder.AppendLine("end");

                        sBuilder.AppendLine();
                        sBuilder.AppendLine("function UITable:ButtonClickHandler(btn)");
                        sBuilder.AppendLine();
                        sBuilder.AppendLine("end");

                        sBuilder.AppendLine();
                        sBuilder.AppendLine("function UITable:OnClose()");
                        sBuilder.AppendLine();
                        sBuilder.AppendLine("end");


                        sw.Write(sBuilder);
                        sw.Close();
                        fs.Close();
                    }
                }

                //创建视图控制器
                className = string.Format("{0}/Controller/{1}Ctrl.lua", sPath, moduleName);
                if (!System.IO.File.Exists(className))
                {
                    using (System.IO.FileStream fs = new System.IO.FileStream(className, System.IO.FileMode.Create))
                    {
                        System.IO.StreamWriter sw = new System.IO.StreamWriter(fs);
                        System.Text.StringBuilder sBuilder = new System.Text.StringBuilder();

                        sBuilder.AppendLine(moduleName +"Ctrl = {};");
                        sBuilder.AppendLine(string.Format("CtrlTable = class(\"{0}Ctrl\", CtrlBase);", moduleName));
                        sBuilder.AppendLine();

                        sBuilder.AppendLine("function MailCtrl.OnCreate()");
                        sBuilder.AppendLine("\tlocal ctrl = CtrlTable.New()");
                        sBuilder.AppendLine("\tctrl: OnInit()");
                        sBuilder.AppendLine("\treturn ctrl");
                        sBuilder.AppendLine("end");


                        sBuilder.AppendLine();
                        sBuilder.AppendLine("function CtrlTable:OnInit()");
                        sBuilder.AppendLine("end");

                        sBuilder.AppendLine();
                        sBuilder.AppendLine("function CtrlTable:RegisterListener()");
                        sBuilder.AppendLine("\t--for example");
                        sBuilder.AppendLine(string.Format("\t--self:RegisterNetWorkListener(PROTO_MSG.SC_TEMP, self.CS_Temp);", moduleName));
                        sBuilder.AppendLine("end");

                        sBuilder.AppendLine();
                        sBuilder.AppendLine("function CtrlTable:OnUpdate(dt)");
                        sBuilder.AppendLine("");
                        sBuilder.AppendLine("end");

                        sBuilder.AppendLine();
                        sBuilder.AppendLine("function CtrlTable:OnClear()");
                        sBuilder.AppendLine("end");

                        sw.Write(sBuilder);
                        sw.Close();
                        fs.Close();
                    }
                }

                AssetDatabase.Refresh();

                CreateLuaAllList("");
                Debug.Log("自动生成UI脚本完成Lua完成");
            }
        }
    }


    static private void CreateUIScripte(Dictionary<string, string> dic, StringBuilder sb, string className)
    {
        StringBuilder sb_1 = new StringBuilder();
        StringBuilder sb_2 = new StringBuilder();

        sb_1.AppendLine(string.Format("function {0}:StartInit()", className));
        sb_2.AppendLine(string.Format("function {0}:SetUIComponent(child)", className));

        string temp = "if";
        foreach (KeyValuePair<string, string> item in dic)
        {
           // sb.Append("\tprotected " + item.Value + " " + item.Key + ";");

            //处理点击事件
            if (item.Value == "Button")
            {
                if (item.Key == "btnClose")
                    sb_1.AppendLine("\t" + string.Format(@"self.{0}.onClick:AddListener(function() self:Close() end)", item.Key));
                else
                    sb_1.AppendLine("\t" + string.Format(@"self.{0}.onClick:AddListener(function() self:ButtonClickHandler(self.{0}) end)", item.Key));
            }
            else if (item.Value == "Toggle")
            {
                sb_1.AppendLine("\t" + string.Format(@"self.{0}.onValueChanged:AddListener(function() self:ToggleChangedHandler(self.{0}) end)", item.Key));
            }

            //处理属性初始化
            if (item.Value == "GameObject")
            {
                sb_2.AppendLine("\t" + temp + " child.name == \"" + item.Key + "\" then \n\t\tself." + item.Key + " = child.gameObject;");
                temp = "elseif";
            }
            else
            {
                sb_2.AppendLine("\t" + temp + " child.name == \"" + item.Key + "\" then \n\t\tself." + item.Key + " = child:GetComponent(\"" + item.Value + "\")" );
                temp = "elseif";
            }
        }
        if (dic.Count > 0)
        {
            sb_2.AppendLine("\tend");
        }
        else
        {
            sb_1.AppendLine("");
            sb_2.AppendLine("");
        }

        sb_1.AppendLine("end").AppendLine("");
        sb_2.AppendLine("end").AppendLine("");

        sb.Append(sb_1);
        sb.Append(sb_2);

    }




    static private bool FindUICompent(Transform tf, Dictionary<string, string> dic)
    {
        for (int i = 0; i < tf.childCount; i++)
        {
            Transform child = tf.GetChild(i);
            if (child.tag == "UIComponent")
            {
                for (int j = 0; j < listUICompent.Count; j++)
                {
                    if (child.GetComponent(listUICompent[j]) != null)
                    {
                        if (dic.ContainsKey(child.name) == true)
                        {
                            Debug.LogError("命名重复: " + child.name);
                            return false;
                        }
                        dic.Add(child.name, listUICompent[j].Name);
                        break;
                    }
                }
                if (dic.ContainsKey(child.name) == false)
                {
                    dic.Add(child.name, "GameObject");
                }
            }

            if (child.childCount > 0)
            {
                FindUICompent(child, dic);
            }
        }
        return true;
    }

    public static string GetGameObjectPath(GameObject obj)
    {
        string path = "/" + obj.name;
        while (obj.transform.parent != null)
        {
            obj = obj.transform.parent.gameObject;
            path = "/" + obj.name + path;
        }
        path = path.Substring(1);

        int startIndex = path.IndexOf("/");
        if (startIndex != -1)
        {
            path = path.Substring(startIndex + 1);
        }
        return path;
    }

    /// <summary>
    /// 生成用Lua实现逻辑的配置逻辑;
    /// </summary>
    public static void CreateLuaAllList(string fromPath)
    {
        string sPath = LuaLogicConfig;
        string className = string.Format("{0}/Logic_Config.lua", sPath);
        using (System.IO.FileStream fs = new System.IO.FileStream(className, System.IO.FileMode.Create))
        {
            System.IO.StreamWriter sw = new System.IO.StreamWriter(fs);
            System.Text.StringBuilder sBuilder = new System.Text.StringBuilder();

            sBuilder.AppendLine("--注意：这个文件由工具自动生成,手动修改可能会被覆盖;");
            sBuilder.AppendLine();
            sBuilder.AppendLine();

            sBuilder.AppendLine("------------------------本地配置数据表------------------------;");
            string[] configs = Directory.GetFiles(LuaConfig, "*.lua");
            string fileName = "";
            for (int i = 0; i < configs.Length; ++i)
            {
                fileName = System.IO.Path.GetFileNameWithoutExtension(configs[i]);
                sBuilder.AppendLine(string.Format("require \"Configs/{0}\";", fileName));
            }

            sBuilder.AppendLine("------------------------本地配置数据表------------------------;");
            sBuilder.AppendLine();
            sBuilder.AppendLine();


            string[] dirs = Directory.GetDirectories(LuaDirectory);

            string dirName = "";

            sBuilder.AppendLine("------------------------所有的通信协议文件列表------------------------;");
            for (int i = 0; i < dirs.Length; ++i)
            {
                dirName = System.IO.Path.GetFileNameWithoutExtension(dirs[i]);
                sBuilder.AppendLine(string.Format("require \"UI/{0}/Controller/{1}Ctrl\";", dirName, dirName));
            }
            sBuilder.AppendLine("------------------------所有的通信协议文件列表------------------------;");


            sBuilder.AppendLine();
            sBuilder.AppendLine();
            sBuilder.AppendLine("-----------------------管理模块控制器--------------------------------");
            sBuilder.AppendLine("ModelCtrlArray = {}");
            for (int i = 0; i < dirs.Length; ++i)
            {
                dirName = System.IO.Path.GetFileNameWithoutExtension(dirs[i]);
                sBuilder.AppendLine(string.Format("ModelCtrlArray[{0}] = {1}Ctrl", i, dirName));
            }
            sw.Write(sBuilder);
            sw.Close();
            fs.Close();
        }

        AssetDatabase.Refresh();
    }
}

#endif