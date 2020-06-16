using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Fight
{
    public static class StaticData
    {
        public static Dictionary<string, FightSkillInfo> dicSkillInfo;

        public static Dictionary<string, FightBuffInfo> dicBuffInfo;

        public static Dictionary<string, FightEffectInfo> dicEffectInfo;

        public static Dictionary<string, FightSkillEffectData> dicSkillEffectData;

        public static Dictionary<string, FightDamageInfo> dicDamageInfo;

        public static Dictionary<string, FightShieldInfo> dicShieldInfo;

        public static Dictionary<string, FightAuraInfo> dicAuraInfo;

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

        private static Dictionary<string, T> LoadDictionary<T>(string name) where T : IStaticData
        {
            //Debug.Log("LoadDictionary:" + name);
            T[] list = SimpleJson.SimpleJson.DeserializeObject<T[]>(LoadJson(name));

            Dictionary<string, T> result = new Dictionary<string, T>();
            for (int i = 0; i < list.Length; i++)
            {
                if (result.ContainsKey(list[i].GetKey()))
                    Debug.Log(list[i].GetKey());
                result.Add(list[i].GetKey(), list[i]);
            }
            return result;
        }

        private static List<T> LoadList<T>(string name)
        {
            return new List<T>(SimpleJson.SimpleJson.DeserializeObject<T[]>(LoadJson(name)));
            //JsonConvert.DeserializeObject<List<T>>(LoadJson(name));
        }

        private static string LoadJson(string name)
        {
            return ResourcesManager.LoadTextFile(name);
            //return LoadTools.LoadTextAsset("Fight3D", name).text;
        }

        public static FightBuffInfo GetBuffInfo(string buffId, int level)
        {
            if (dicBuffInfo.ContainsKey(buffId))
                return dicBuffInfo[buffId];
            if (dicBuffInfo.ContainsKey(buffId + "_" + level))
                return dicBuffInfo[buffId + "_" + level];
            if (dicBuffInfo.ContainsKey(buffId + "_0"))
                return dicBuffInfo[buffId + "_0"];

            return null;
        }

        public static FightDamageInfo GetDamageInfo(string id, int level)
        {
            if (dicDamageInfo.ContainsKey(id))
                return dicDamageInfo[id];

            return null;
        }

        public static FightAuraInfo GetAruaInfo(string auraId)
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
    string GetKey();
}
