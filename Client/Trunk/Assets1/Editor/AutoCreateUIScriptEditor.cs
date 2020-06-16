#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;

/// <summary>
/// 自动生成luaUI脚本
/// </summary>
public class AutoCreateUIScriptEditor : EditorWindow
{
    public static string LuaDirectory = System.Environment.CurrentDirectory + "/Assets/LuaScript/Logic/UI";
    public static string LuaGameDirectory = System.Environment.CurrentDirectory + "/Assets/LuaScript/Logic";
    public static string LuaConfig = System.Environment.CurrentDirectory + "/Assets/LuaScript/Logic/Configs";

    public enum UIType
    {
        GameObject,
        Button,
        Text,
        Image,
        Slider,
        RawImage,
        Toggle,
        InputField,
        Scrollbar,
        GyImage,
        GyText
    }

    public class CSharpInfo
    {
        public string m_attrName;
        public string m_comPath;
        public UIType m_type;

        public CSharpInfo(string attrName, string comPath, UIType type)
        {
            m_attrName = attrName;
            m_comPath = comPath;
            m_type = type;
        }
    }

    public static Dictionary<string, CSharpInfo> s_infos = new Dictionary<string, CSharpInfo>();

    [MenuItem("Assets/[Gatecen] 自动生成UI脚本Lua")]
    public static void CreateScriptLua()
    {
        CreateScript(true);
    }
    public static void CreateScript(bool isLua)
    {
        for (int i = 0; i < Selection.objects.Length; ++i)
        {
            s_infos.Clear();

            GameObject obj = Selection.objects[i] as GameObject;

            GetCSharpInfos(obj);

            if (isLua)//生成Lua部分;
            {
                string moduleName = obj.name.Substring(3, obj.name.Length - 3);
                string sPath = string.Format("{0}/{1}", LuaDirectory, moduleName);
                if (!System.IO.Directory.Exists(sPath))
                {
                    System.IO.Directory.CreateDirectory(sPath);
                }

                //生成属性字段配置
                string UIName = string.Format("UI_{0}", moduleName);
                string className = string.Format("{0}/{1}_Attr.lua", sPath, UIName);
                using (System.IO.FileStream fs = new System.IO.FileStream(className, System.IO.FileMode.Create))
                {
                    System.IO.StreamWriter sw = new System.IO.StreamWriter(fs);
                    System.Text.StringBuilder sBuilder = new System.Text.StringBuilder();

                    sBuilder.AppendLine("--注意：这个文件由工具自动生成,手动修改可能会被覆盖;");
                    sBuilder.AppendLine();
                    //sBuilder.AppendLine(string.Format("local {0} = {1};", onlyName, "{}"));
                    sBuilder.AppendLine(string.Format("local {0} = class(\"{1}\",UIBase);", UIName, UIName, "{}"));
                    sBuilder.AppendLine();


                    sBuilder.AppendLine(string.Format("function {0}:Init()", UIName));
                    sBuilder.AppendLine(string.Format("\tself.uiName = \"{0}\"", UIName));

                    foreach (CSharpInfo info in s_infos.Values)
                    {
                        if (info.m_type == UIType.GameObject)
                        {
                            sBuilder.AppendLine("\tself." + info.m_attrName + " = " + "self.transform:Find(\"" + info.m_comPath + "\").gameObject;");
                        }
                        else
                        {
                            sBuilder.AppendLine("\tself." + info.m_attrName + " = " + "self.transform:Find(\"" + info.m_comPath + "\"):GetComponent(\"" + info.m_type.ToString() + "\");");
                        }
                    }

                    sBuilder.AppendLine("end");

                    sBuilder.AppendLine();

                    sBuilder.AppendLine(string.Format("function {0}:ClearMembers()", UIName));

                    foreach (CSharpInfo info in s_infos.Values)
                    {
                        sBuilder.AppendLine("\tself." + info.m_attrName + " = nil;");
                    }

                    sBuilder.AppendLine("end");

                    sBuilder.AppendLine();
                    sBuilder.AppendLine(string.Format("return {0};", UIName));

                    sw.Write(sBuilder);
                    sw.Close();
                    fs.Close();
                }

                //生成UI界面模板
                className = string.Format("{0}/{1}.lua", sPath, UIName);
                if (!System.IO.File.Exists(className))
                {
                    using (System.IO.FileStream fs = new System.IO.FileStream(className, System.IO.FileMode.Create))
                    {
                        System.IO.StreamWriter sw = new System.IO.StreamWriter(fs);
                        System.Text.StringBuilder sBuilder = new System.Text.StringBuilder();

                        sBuilder.AppendLine(string.Format("local UI_{0} = require \"Logic/UI/{1}/{2}_Attr\";", moduleName, moduleName, UIName));
                        sBuilder.AppendLine();

                        sBuilder.AppendLine();
                        sBuilder.AppendLine("--声明和清理成员变量函数，所有的成员变量必须在这里注册和清除;");
                        sBuilder.AppendLine(string.Format("function {0}:OnMemberVariables()", UIName));
                        sBuilder.AppendLine("\t--for example");
                        sBuilder.AppendLine("\t--self.testMemberVariable = nil;");
                        sBuilder.AppendLine();
                        sBuilder.AppendLine("end");

                        sBuilder.AppendLine();
                        sBuilder.AppendLine("--Awake初始化函数，给按钮添加事件等可以放在这里;");
                        sBuilder.AppendLine(string.Format("function {0}:Ready()", UIName));
                        sBuilder.AppendLine();
                        sBuilder.AppendLine("end");

                        sBuilder.AppendLine();
                        sBuilder.AppendLine("--显示UI函数;");
                        sBuilder.AppendLine(string.Format("function {0}:OnShow()", UIName));
                        sBuilder.AppendLine();
                        sBuilder.AppendLine("end");

                        sBuilder.AppendLine();
                        sBuilder.AppendLine("--隐藏UI函数;");
                        sBuilder.AppendLine(string.Format("function {0}:OnHide()", UIName));
                        sBuilder.AppendLine();
                        sBuilder.AppendLine("end");

                        sBuilder.AppendLine();
                        sBuilder.AppendLine("--销毁清理函数;");
                        sBuilder.AppendLine(string.Format("function {0}:Clear()", UIName));
                        sBuilder.AppendLine(string.Format("\t{0} = nil;", UIName));
                        sBuilder.AppendLine("end");

                        sBuilder.AppendLine();
                        sBuilder.AppendLine("--监听事件处理函数(lua内部的);");
                        sBuilder.AppendLine(string.Format("function {0}:HandleNotification(notification)", UIName));
                        sBuilder.AppendLine("end");

                        sBuilder.AppendLine();
                        sBuilder.AppendLine("--感兴趣的消息号;");
                        sBuilder.AppendLine(UIName + ".mTableNotification = {");
                        sBuilder.AppendLine();
                        sBuilder.AppendLine("};");

                        sBuilder.AppendLine();
                        sBuilder.AppendLine(string.Format("return {0}.New();", UIName));

                        sw.Write(sBuilder);
                        sw.Close();
                        fs.Close();
                    }
                }

                className = string.Format("{0}/{1}Ctrl.lua", sPath, moduleName);
                if (!System.IO.File.Exists(className))
                {
                    using (System.IO.FileStream fs = new System.IO.FileStream(className, System.IO.FileMode.Create))
                    {
                        System.IO.StreamWriter sw = new System.IO.StreamWriter(fs);
                        System.Text.StringBuilder sBuilder = new System.Text.StringBuilder();

                        sBuilder.AppendLine(string.Format("G_{0}Ctrl = nil;", moduleName));
                        sBuilder.AppendLine(string.Format("{0}Ctrl = class(\"{1}Ctrl\", CtrlBase);", moduleName, moduleName));
                        sBuilder.AppendLine();

                        sBuilder.AppendLine();
                        sBuilder.AppendLine("--初始化函数");
                        sBuilder.AppendLine(string.Format("function {0}Ctrl:OnInit()", moduleName));
                        sBuilder.AppendLine(string.Format("\tG_{0}Ctrl = self", moduleName));
                        sBuilder.AppendLine("\tself.m_tempData = {}");
                        sBuilder.AppendLine("end");

                        sBuilder.AppendLine();
                        sBuilder.AppendLine("--监听事件注册函数(来自网络层)");
                        sBuilder.AppendLine(string.Format("function {0}Ctrl:RegisterListener()", moduleName));
                        sBuilder.AppendLine("\t--for example");
                        sBuilder.AppendLine(string.Format("\t--self:RegisterNetWorkListener(PROTO_MSG.SC_TEMP, self.CS_Temp);", moduleName));
                        sBuilder.AppendLine("end");

                        sBuilder.AppendLine();
                        sBuilder.AppendLine("--每帧刷新");
                        sBuilder.AppendLine(string.Format("function {0}Ctrl:OnUpdate(dt)", moduleName));
                        sBuilder.AppendLine("");
                        sBuilder.AppendLine("end");

                        sBuilder.AppendLine();
                        sBuilder.AppendLine("-- 销毁清理函数");
                        sBuilder.AppendLine(string.Format("function {0}Ctrl:OnClear()", moduleName));
                        sBuilder.AppendLine("\tself.m_tempData = nil");
                        sBuilder.AppendLine(string.Format("\tG_{0}Ctrl = nil", moduleName));
                        sBuilder.AppendLine("end");

                        sBuilder.AppendLine();
                        sBuilder.AppendLine(string.Format("function {0}Ctrl.OnCreate()", moduleName));
                        sBuilder.AppendLine(string.Format("\treturn G_{0}Ctrl or {1}Ctrl.New()", moduleName, moduleName));
                        sBuilder.AppendLine("end");

                        sBuilder.AppendLine();

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

    public static void GetCSharpInfos(GameObject obj)
    {
        if (obj.name.StartsWith("m_"))
        {
            CSharpInfo c = new CSharpInfo(obj.name, GetGameObjectPath(obj), UIType.GameObject);

            if (obj.GetComponent<Button>() != null)
            {
                c.m_type = UIType.Button;
            }
            else if (obj.GetComponent<Scrollbar>() != null)
            {
                c.m_type = UIType.Scrollbar;
            }
            else if (obj.GetComponent<InputField>() != null)
            {
                c.m_type = UIType.InputField;
            }
            else if (obj.GetComponent<Text>() != null)
            {
                c.m_type = UIType.Text;
            }
            else if (obj.GetComponent<RawImage>() != null)
            {
                c.m_type = UIType.RawImage;
            }
            else if (obj.GetComponent<Image>() != null)
            {
                c.m_type = UIType.Image;
            }
            else if (obj.GetComponent<Slider>() != null)
            {
                c.m_type = UIType.Slider;
            }
            else if (obj.GetComponent<Toggle>() != null)
            {
                c.m_type = UIType.Toggle;
            }

            if (s_infos.ContainsKey(obj.name))
            {
                Debug.LogError("错误：有相同的命名 " + obj.name);
            }
            s_infos.Add(obj.name, c);
        }

        foreach (Transform t in obj.transform)
        {
            GetCSharpInfos(t.gameObject);
        }
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
        string sPath = LuaGameDirectory;
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
                sBuilder.AppendLine(string.Format("require \"Logic/Configs/{0}\";", fileName));
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
                sBuilder.AppendLine(string.Format("require \"Logic/UI/{0}/{1}Ctrl\";", dirName, dirName));
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