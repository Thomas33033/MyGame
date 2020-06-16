using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Fight
{
    public class BuffComponent : BaseComponent
    {
        //buff列表
        public List<FightBuff> listBuff;

        public BuffComponent(Role role) : base(role)
        {
            listBuff = new List<FightBuff>();
        }

        public void UpdateBuff()
        {
            float now = Owner.Time;
            for (int i = 0; i < listBuff.Count; i++)
            {
                if (listBuff[i].timeDuration > 0 && listBuff[i].timeCreate + listBuff[i].timeDuration < now)
                {
                    Owner.buffComp.BuffLost(listBuff[i]);
                    listBuff.RemoveAt(i);
                    i--;
                }
            }

            TriggerBuff(TriggerType.Time);
        }

        public void TriggerEffect(TriggerType type, FightAttackDataBase something = null)
        {
            for (int i = 0; i < listBuff.Count; i++)
            {
                listBuff[i].Trigger(type, something);
            }
        }

        public void BuffAdd(FightBuff v)
        {
            if (Owner.StatusCheck(RoleStatus.Silent))
            {
                if (v.attacker.teamId != Owner.teamId)
                {
                    return;
                }
            }

            FightBuff fightBuff = BuffGet(v.buffId);

            if (fightBuff != null)
            {
                if (fightBuff.stackMax < 1)
                {
                    return;
                }

                if (fightBuff.StackBuff(v) == false)
                    return;
            }
            else
            {
                listBuff.Add(v);
                fightBuff = v;
            }

            if (string.IsNullOrEmpty(fightBuff.attr) == false)
            {
                Owner.AttrChange(fightBuff.attr, fightBuff.attrValue);
            }

            //Debug.Log(Time + " BuffAdd " + id + " " + fightBuff.buffId + " " + fightBuff.stack);

            Owner.AddReport(new FightReportBuffAdd(Owner.Time, Owner.teamId, Owner.id, fightBuff.buffId, fightBuff.level, fightBuff.timeEnd, fightBuff.stack));
        }

        public FightBuff BuffGet(int buffId)
        {
            for (int i = 0; i < listBuff.Count; i++)
            {
                if (listBuff[i].buffId == buffId)
                {
                    return listBuff[i];
                }
            }
            return null;
        }

        public void BuffClear()
        {
            for (int i = 0; i < listBuff.Count; i++)
            {
                if (listBuff[i].info.isclear == 0 && listBuff[i].attacker.teamId != Owner.teamId)
                {
                    BuffLost(listBuff[i]);
                    listBuff.RemoveAt(i);
                    i--;
                }
            }
        }

        public void BuffRemove(int buffId)
        {
            for (int i = 0; i < listBuff.Count; i++)
            {
                if (listBuff[i].buffId == buffId)
                {
                    BuffLost(listBuff[i]);
                    listBuff.RemoveAt(i);
                    i--;
                }
            }
        }

        public void BuffLost(FightBuff fightBuff)
        {
            if (string.IsNullOrEmpty(fightBuff.info.attr) == false)
            {
                Owner.AttrChange(fightBuff.attr, -fightBuff.attrValue * Mathf.Max(1, fightBuff.stack));
            }
            Owner.AddReport(new FightReportBuffRemove(Owner.Time, Owner.teamId, Owner.id, fightBuff.info.id));
            //Debug.Log(Time + " BuffLost " + id + " " + fightBuff.buffId);
        }

        public void TriggerBuff(TriggerType type)
        {
            float now = Owner.Time;
            for (int i = 0; i < listBuff.Count; i++)
            {
                if (listBuff[i].CheckTrigger(type))
                {
                    if (listBuff[i].triggerType == (int)TriggerType.Time)
                    {
                        listBuff[i].interval = Mathf.FloorToInt((now - listBuff[i].timeExecute) * 1000);
                    }
                    else
                    {
                        listBuff[i].interval++;
                    }
                    if (listBuff[i].interval >= listBuff[i].intervalExecute)
                    {
                        BuffExecute(listBuff[i]);
                        listBuff[i].timeExecute = now;
                        listBuff[i].interval = 0;
                    }
                }

                if (listBuff[i].countNum > 0 && listBuff[i].countExecute >= listBuff[i].countNum)
                {
                    BuffLost(listBuff[i]);
                    listBuff.RemoveAt(i);
                    i--;
                    continue;
                }
            }
        }

        private void BuffExecute(FightBuff buff)
        {
            if (buff.info.damageId >= 0)
            {
                FightAttackData fightAttackData = new FightAttackData(buff.attacker, buff.target, buff.level, Owner.Time, buff.info.damageId, DamageSourceType.Buff);

                if (buff.stack > 0)
                {
                    fightAttackData.bouns += buff.stack * 100;
                }

                Owner.AddAttackData(fightAttackData);
            }

            buff.Trigger();
        }
    }
}