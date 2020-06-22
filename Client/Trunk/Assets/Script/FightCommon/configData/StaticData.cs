using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Fight
{
    public static class StaticData
    {
        public static Dictionary<int, FightSkillInfo> dicSkillInfo;

        public static Dictionary<int, FightBuffInfo> dicBuffInfo;

        public static Dictionary<int, FightEffectInfo> dicEffectInfo;

        public static Dictionary<int, FightSkillEffectData> dicSkillEffectData;

        public static Dictionary<int, FightDamageInfo> dicDamageInfo;

        public static Dictionary<int, FightShieldInfo> dicShieldInfo;

        public static Dictionary<int, FightAuraInfo> dicAuraInfo;

        public static void Init()
        {
            dicSkillInfo = LoadDictionary<FightSkillInfo>("SkillData");
            dicBuffInfo = LoadDictionary<FightBuffInfo>("BuffData");
            dicEffectInfo = LoadDictionary<FightEffectInfo>("FightEffects");
            dicDamageInfo = LoadDictionary<FightDamageInfo>("DamageData");

            dicSkillEffectData = LoadDictionary<FightSkillEffectData>("EffectData");

            dicShieldInfo = LoadDictionary<FightShieldInfo>("ShieldData");
            dicAuraInfo = LoadDictionary<FightAuraInfo>("AuraData");
        }

        private static Dictionary<int, T> LoadDictionary<T>(string name) where T : IStaticData
        {
            T[] list = SimpleJson.SimpleJson.DeserializeObject<T[]>(LoadJson(name));

            Dictionary<int, T> result = new Dictionary<int, T>();
            for (int i = 0; i < list.Length; i++)
            {
                if (result.ContainsKey(list[i].GetKey()))
                    Debug.Log(list[i].GetKey());
                result.Add(list[i].GetKey(), list[i]);
            }
            return result;
        }

        public static List<T> LoadList<T>(string name)
        {
            return new List<T>(SimpleJson.SimpleJson.DeserializeObject<T[]>(LoadJson(name)));
        }

        private static string LoadJson(string name)
        {
            string filePath = "Assets/BundleRes/Config/Fight/" + name + ".json";
            return ResourcesManager.LoadTextFile(filePath);
        }

        public static void SaveData(string name, string jsonData)
        {
            string filePath = Path.Combine(Application.dataPath, "BundleRes/Config/", name);
            File.WriteAllText(filePath, jsonData, System.Text.Encoding.UTF8);
        }

        public static FightBuffInfo GetBuffInfo(int buffId, int level)
        {
            if (dicBuffInfo.ContainsKey(buffId))
                return dicBuffInfo[buffId];
            if (dicBuffInfo.ContainsKey(buffId * 100 + level))
                return dicBuffInfo[buffId * 100 + level];
            if (dicBuffInfo.ContainsKey(buffId * 10))
                return dicBuffInfo[buffId * 10];

            return null;
        }

        public static FightDamageInfo GetDamageInfo(int id, int level)
        {
            if (dicDamageInfo.ContainsKey(id))
                return dicDamageInfo[id];

            return null;
        }

        public static FightAuraInfo GetAruaInfo(int auraId)
        {
            if (dicAuraInfo.ContainsKey(auraId))
                return dicAuraInfo[auraId];
            return null;
        }

        public static int SortFightEffectActionHandler(FightEffectActionInfo a, FightEffectActionInfo b)
        {
            return a.step.CompareTo(b.step);
        }
    }
}

public interface IStaticData
{
    int GetKey();
}
