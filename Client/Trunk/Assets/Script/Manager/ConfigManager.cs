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
        this.configDataMap[typeof(CfgItemData)] = LoadDictionary<CfgItemData>("cfg_item");
        this.configDataMap[typeof(CfgNpcAttrData)] = LoadDictionary<CfgNpcAttrData>("cfg_npc_attr");
        this.configDataMap[typeof(CfgNpcData)] = LoadDictionary<CfgNpcData>("cfg_npc");
        this.configDataMap[typeof(CfgSkillData)] = LoadDictionary<CfgSkillData>("cfg_skill_des");
    }

    private static string LoadJson(string name)
    {
        string filePath = "Assets/BundleRes/Config/data/" + name + ".json";
        return AssetsManager.LoadTextFile(filePath);
    }

    private static Dictionary<int, configBase> LoadDictionary<T>(string name) where T : configBase
    {
        Dictionary<string, T> jsonMap = SimpleJson.SimpleJson.DeserializeObject<Dictionary<string, T>>(LoadJson(name));
        Dictionary<int, configBase> result = new Dictionary<int, configBase>();
        foreach (var v in jsonMap)
        {
            result.Add(int.Parse(v.Key), v.Value);
        }
        return result;
    }
}

public class configBase{}
