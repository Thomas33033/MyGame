using System.Collections.Generic;

namespace Fight
{
    /// <summary>
    /// 
    /// </summary>
    public class FightEffectBox
    {
        public Role attacker;

        public Role lodger;

        public DamageSourceType damageSourceType;

        public List<FightEffect> listEffect;

        public FightEffect mainEffect => listEffect.Count > 0 ? listEffect[0] : null;

        public bool needUpdating => mainEffect == null ? false : mainEffect.needUpdating;

        public bool isChannelling => mainEffect == null ? false : mainEffect.isChannelling;

        public FightEffectBox(int[] effectIds, int level, Role attacker, Role lodger, DamageSourceType damageSourceType)
        {
            this.attacker = attacker;
            this.lodger = lodger;
            this.damageSourceType = damageSourceType;

            listEffect = new List<FightEffect>();
            for (int i = 0; i < effectIds.Length; i++)
            {
                if (StaticData.dicEffectInfo.ContainsKey(effectIds[i]) == false)
                {
                    UnityEngine.Debug.LogError("not find effect " + effectIds[i]);
                    continue;
                }

                FightEffectInfo fightEffectInfo = StaticData.dicEffectInfo[effectIds[i]];
                FightSkillEffectData fightSkillEffectData = StaticData.dicSkillEffectData.ContainsKey(effectIds[i]) ? StaticData.dicSkillEffectData[effectIds[i]] : null;

                listEffect.Add(new FightEffect(this, fightEffectInfo, fightSkillEffectData, level));
            }
        }

        public void Break()
        {
            if (isChannelling)
            {
                mainEffect.Break();
            }
        }

        public FightEffect GetEffect(TriggerType type)
        {
            for (int i = 0; i < listEffect.Count; i++)
            {
                if (listEffect[i].triggerType == (int)type)
                {
                    return listEffect[i];
                }
            }
            return null;
        }

        public int[] GetEffectTargetIds()
        {
            return mainEffect.GetTargetIds();
        }

        public void Update(float nowTime)
        {
            if (mainEffect.needUpdating)
            {
                mainEffect.Update(nowTime);
            }
        }

        public bool Trigger(TriggerType type, FightAttackDataBase something = null, int space = 0)
        {
            if (mainEffect == null)
                return false;

            //if (space == 0 && (mainEffect.triggerSpace != space || mainEffect.triggerType != (int)type))
            //    return false;

            bool result = false;
            for (int i = 0; i < listEffect.Count; i++)
            {
                if (listEffect[i].triggerSpace == space)
                {
                    result = result || listEffect[i].Trigger(type, something);
                }
            }
            return result;
        }

        public void AddAttackData(FightAttackData fightAttackData)
        {
            attacker.AddAttackData(fightAttackData);
        }
    }
}
