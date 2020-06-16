using System;
using UnityEngine;
namespace Fight
{
    /// <summary>
    /// 战斗buff
    /// </summary>

    public class FightBuff : FightAttackDataBase
    {
        public FightBuffInfo info;

        public string buffId { get => info.id; }

        public int triggerType { get => info.trigger; }

        public float timeCreate;

        public float timeExecute;

        public float timeDuration
        {
            get
            {
                if (info.duration.Length == 0)
                    return 0f;

                return info.duration[Mathf.Min(info.duration.Length - 1, level - 1)] / 1000f;
            }
        }

        public float timeEnd => timeCreate + timeDuration;

        public int interval;

        public int intervalExecute { get => info.interval; }

        public int countNum { get => info.count; }

        public int countExecute;

        public int stack;

        public int stackMax { get => info.stack; }

        public FightEffectBox fightEffectBox;

        public string attr => info.attr;

        public int attrValue
        {
            get
            {
                if (info.attrValue.Length == 0)
                    return 0;

                return info.attrValue[Mathf.Min(info.attrValue.Length - 1, level - 1)];
            }
        }

        public FightBuff(Role attacker, Role target, int level, FightBuffInfo info, float timeCreate) : base(attacker, target, level, DamageSourceType.Buff)
        {
            this.info = info;
            this.timeCreate = timeCreate;

            fightEffectBox = new FightEffectBox(info.effectIds, level, attacker, target, DamageSourceType.Buff);
            stack = 1;
        }

        public bool CheckTrigger(TriggerType type)
        {
            if ((int)type != triggerType)
                return false;
            return true;
        }

        public void Trigger()
        {
            fightEffectBox.Trigger(TriggerType.Buff, null, 1);
            countExecute++;
        }

        public void Trigger(TriggerType type, FightAttackDataBase something = null)
        {
            if (fightEffectBox.Trigger(type, something))
            {
                countExecute++;
            }
        }

        public bool StackBuff(FightBuff v)
        {
            timeCreate = v.timeCreate;

            if (stack >= stackMax)
                return false;

            stack++;
            countExecute = 0;

            if (stack >= stackMax)
            {
                fightEffectBox.Trigger(TriggerType.BuffStack, this, 1);
            }
            return true;
        }
    }
}