using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 战斗光环
/// </summary>


namespace Fight
{
    public class FightAura
    {
        public FightAuraInfo info;

        public float timeCreate;

        public float timeDuration => info.duration / 1000f;

        public int zone => info.zone;

        public int range => info.range;

        public int targetType => info.targetType;

        public Role attacker;

        public List<Role> listTarget;

        public int level;

        public FightEffectBox fightEffectBox;

        public Role lodger;

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

        public FightAura(Role attacker, Role lodger, FightAuraInfo info, int level, float timeCreate)
        {
            this.attacker = attacker;
            this.info = info;
            this.level = level;
            this.lodger = lodger;
            this.timeCreate = timeCreate;
            listTarget = new List<Role>();
            fightEffectBox = new FightEffectBox(info.effectIds, level, attacker, null, DamageSourceType.Buff);
        }

        public void Update(float nowTime)
        {
            if (timeDuration > 0 && nowTime > timeCreate + timeDuration)
            {
                lodger.AuraRemove(this);
                return;
            }

            List<Role> listTemp = attacker.scanTargetComp.FindTarget(targetType, 0, range, lodger);

            if (zone != 0)
            {
                for (int i = 0; i < listTemp.Count; i++)
                {
                    bool isMyZone = listTemp[i].isMyZone;
                    if ((zone == 1) != (isMyZone == true))
                    {
                        listTemp.RemoveAt(i);
                        i--;
                    }
                }
            }

            for (int i = 0; i < listTarget.Count; i++)
            {
                if (listTemp.IndexOf(listTarget[i]) == -1)
                {
                    RemoveTarget(listTarget[i]);
                    i--;
                }
            }

            for (int i = 0; i < listTemp.Count; i++)
            {
                if (listTarget.IndexOf(listTemp[i]) == -1)
                {
                    AddTarget(listTemp[i]);
                }
            }
        }

        public void Clear()
        {
            for (int i = 0; i < listTarget.Count; i++)
            {
                RemoveTarget(listTarget[i]);
                i--;
            }
        }

        public void AddTarget(Role v)
        {
            listTarget.Add(v);
            v.AuraEnter(this);

            Trigger(v, TriggerType.AuraAdd);
            if (listTarget.Count == 1)
            {
                Trigger(v, TriggerType.AuraHas);
            }
        }

        public void RemoveTarget(Role v)
        {
            listTarget.Remove(v);
            v.AuraExit(this);

            Trigger(v, TriggerType.AuraRemove);
            if (listTarget.Count == 0)
            {
                Trigger(v, TriggerType.AuraNone);
            }
        }

        public void Trigger(Role target, TriggerType type, FightAttackDataBase something = null)
        {
            if (something == null)
            {
                something = new FightAttackDataBase(attacker, target, level, DamageSourceType.Aura);
            }

            fightEffectBox.Trigger(type, something);
        }
    }
}