using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System;

public class ConfigManager : Singleton<ConfigManager>
{
    private Dictionary<System.Type, Dictionary<int, configBase>> configDataMap;

    public T GetData<T>(int configId) where T : configBase
    {
        System.Type type = typeof(T);
        if (this.configDataMap[type] == null){
            return null;
        }
        return this.configDataMap[type][configId] as T;
    }

    
    //模拟数据
    public void Init()
    {
        this.configDataMap = new Dictionary<System.Type, Dictionary<int, configBase>>();
        this.configDataMap[typeof(CfgItemData)] = loader<CfgItemData>("cfg_item");
        this.configDataMap[typeof(CfgNpcAttrData)] = loader<CfgNpcAttrData>("cfg_npc_attr");
        this.configDataMap[typeof(CfgNpcData)] = loader<CfgNpcData>("cfg_npc");
        this.configDataMap[typeof(CfgSkillData)] = loader<CfgSkillData>("cfg_skill");
    }


    public object SwitchType(string type, string value)
    {
        if (string.Equals("int", type, StringComparison.CurrentCultureIgnoreCase))
        {
            return int.Parse(value);
        }
        else if (string.Equals("string", type, StringComparison.CurrentCultureIgnoreCase))
        {
            return value;
        }
        else if (string.Equals("bool", type, StringComparison.CurrentCultureIgnoreCase))
        {
            return bool.Parse(value);
        }
        return null;
    }

    public Dictionary<int, configBase> loader<T>(string tableName) where T : configBase, new()
    {
        string path = Application.streamingAssetsPath + "/ConfigData/" + tableName + ".csv";

        Dictionary<int, configBase> map = new Dictionary<int, configBase>();
        FileInfo fileInfo = new FileInfo(path);
        if (!fileInfo.Directory.Exists)
        {
            return map;
        }

        FileStream fs = new FileStream(path, System.IO.FileMode.Open, System.IO.FileAccess.Read);
        StreamReader sr = new StreamReader(fs, Encoding.UTF8);
        string strLine = "";
        int index = 0;
        string[] fieldArray = null;
        string[] typeArray = null;
        string[] valueArray = null;
        while ((strLine = sr.ReadLine()) != null)
        {
            index++;
            if (index == 1)
            {
                fieldArray = strLine.Split('|');
            }
            else if (index == 2)
            {
                typeArray = strLine.Split('|');
            }
            else
            {
                valueArray = strLine.Split('|');
                T t = new T();
                Type type = t.GetType();
                int uid = 0;
                for (int i = 0; i < valueArray.Length; i++)
                {
                    string fieldValue = valueArray[i];
                    string fieldName = fieldArray[i];
                    string temp = fieldName.Substring(0, 1).ToUpper() + fieldName.Substring(1);
                    System.Reflection.PropertyInfo pInfo = type.GetProperty(temp);
                    if (pInfo != null)
                    {
                        try
                        {
                            object v = SwitchType(typeArray[i], fieldValue);
                            if (i == 0) uid = (int)v;
                            pInfo.SetValue(t, v, null);
                        }
                        catch(Exception exp)
                        {
                            Debug.LogError("tableName:"+tableName+"  fieldName:"+fieldName);
                        }
                       
                    }
                }
                if (uid != 0)
                {
                    if (!map.ContainsKey(uid))
                    {
                        map.Add(uid, t);
                    }
                    else
                    {
                        Debug.LogError(uid + "ID重复请检查表格->" + t.GetType());
                    }
                }
            }
        }
        return map;
    }
}

public class configBase{}
