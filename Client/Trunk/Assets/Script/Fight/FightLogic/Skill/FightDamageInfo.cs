﻿namespace Fight
{
    public class FightEffectInfo : IStaticData
    {
        public int id;

        public int trigger;

        public int cd;

        /// <summary>
        /// 0响应所有触发 1响应同技能触发
        /// </summary>
        public int space;

        public FightEffectActionInfo[] actionInfos;

        public int GetKey()
        {
            return id;
        }
    }

    public class FightSkillEffectData : IStaticData
    {
        public int id;

        public int[] value1;
        public int[] value2;
        public int[] value3;

        public int GetKey()
        {
            return id;
        }
    }

    public class FightEffectActionInfo
    {
        public int id;

        public int step;

        public int[] valuesType;

        public int[] values;

        public string[] keys;
    }

    public class FightDamageInfo : IStaticData
    {
        public int damageId;

        public string Resources;

        public int type;

        public int[] damage;

        public int damageBase;

        public int[] bouns;

        public int GetKey()
        {
            return damageId;
        }
    }

    public class FightEffectInfo_O : IStaticData
    {
        public int EffectID;
        public int EffectLv;
        public int ConditionTarget;
        public int ConditionType;
        public int ConditionPredicate;
        public float ConditionValue;
        public int ActionTarget;
        public int ActionType;
        public int ActionValue;
        public int hurtBaseType;
        public float hurtProportion;
        public int hurtMaxType;
        public float hurtMaxValue;
        public int repairBaseType;
        public float repairProportion;
        public string buffId;

        public string hurtEffectByBuff;
        public string repairEffectByBuff;

        public string SkillHitEffect1;

        public string SkillHitEffect2;

        public int GetKey()
        {
            return EffectID *100+ EffectLv;
        }
    }
}