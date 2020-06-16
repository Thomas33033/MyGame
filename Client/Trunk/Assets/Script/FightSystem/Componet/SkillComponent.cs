using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Experimental.PlayerLoop;

namespace Fight
{
    public class SkillComponent : BaseComponent
    {
        public List<FightSkill> listSkills;
        public FightSkill _skillChannelling;

        //护盾列表
        public List<FightShield> listShield;
        

        public List<FightAura> listAuraMine;
        //光环
        public List<FightAura> listAura;

        public SkillComponent(Role role) : base(role)
        {
            listSkills = new List<FightSkill>();

            listAura = new List<FightAura>();
            listAuraMine = new List<FightAura>();
            
            listShield = new List<FightShield>();
        }

        public override void OnUpdate(float nowTime)
        {
            //刷新buff
            Owner.buffComp.UpdateBuff();
            //刷新技能
            UpdateSkill(nowTime);
            //光环
            UpdateAura(nowTime);
            //刷新护盾
            UpdateShield(nowTime);
        }

        void UpdateAura(float nowTime)
        {
            for (int i = 0; i < listAuraMine.Count; i++)
            {
                listAuraMine[i].Update(nowTime);
            }
        }

        void UpdateShield(float nowTime)
        {
            for (int i = 0; i < listShield.Count; i++)
            {
                if (listShield[i].timeCreate + listShield[i].timeDuration < nowTime)
                {
                    Owner.reportComp.AddReport(new FightReportShieldRemove(Owner.Time,
                        Owner.teamId, Owner.id, listShield[i].info.id));
                    listShield.RemoveAt(i);
                    i--;
                }
            }
        }

        public void SkillCreate(FightSkillData[] skills)
        {
            for (int i = 0; i < skills.Length; i++)
            {
                SkillAdd(skills[i].skillID, skills[i].level);
            }
        }

        public void SkillAdd(int skillId, int level)
        {
            if (StaticData.dicSkillInfo.ContainsKey(skillId) == false)
            {
                return;
            }
            FightSkillInfo skillInfo = StaticData.dicSkillInfo[skillId];
            FightSkill fightSkill = new FightSkill(Owner, skillInfo, level);

            listSkills.Add(fightSkill);
        }

        private void UpdateSkill(float nowTime)
        {
            for (int i = 0; i < listSkills.Count; i++)
            {
                if (listSkills[i].needUpdating)
                {
                    listSkills[i].Update(nowTime);
                }
            }

            if (_skillChannelling != null && _skillChannelling.isChannelling == false)
            {
                Owner.AddReport(new FightReportRoleSkillDone(Owner.Time, Owner.teamId, Owner.id, _skillChannelling.skillId, _skillChannelling.fightEffectBox.GetEffectTargetIds(), Owner.mp));
                Owner.actionEndTime = Owner.Time + _skillChannelling.info.EndTime / 1000f;
                //lastAttackTime = actionEndTime;
                _skillChannelling = null;
            }
        }

        public void Clear()
        {
            for (int i = 0; i < listAuraMine.Count; i++)
            {
                listAuraMine[i].Clear();
                listAuraMine.RemoveAt(i);
                i--;
            }
        }


        internal object GetSkill(int skillId)
        {
            for (int i = 0; i < listSkills.Count; i++)
            {
                if (listSkills[i].skillId == skillId)
                {
                    return listSkills[i];
                }
            }
            return null;
        }

        /// <summary>
        /// 0主动 1被动 2普攻 3火炮
        /// </summary>
        /// <param name="skillType"></param>
        /// <returns></returns>
        public bool CastSkill(int skillType)
        {
            for (int i = 0; i < listSkills.Count; i++)
            {
                if (listSkills[i].type == skillType)
                {
                    if (DoSkill(listSkills[i]) == false)
                        continue;

                    //actionEndTime = Time + listSkills[i].info.SkillTime / 1000f;
                    //lastAttackTime = actionEndTime;

                    return true;
                }
            }

            return false;
        }

        public void CastSkill(int skillId, int level)
        {
            bool flag = false;
            for (int i = 0; i < listSkills.Count; i++)
            {
                if (listSkills[i].skillId == skillId)
                {
                    DoSkill(listSkills[i]);
                    flag = true;
                    break;
                }
            }
            if (flag == false)
            {
                if (StaticData.dicSkillInfo.ContainsKey(skillId) == true)
                {
                    FightSkill fightSkill = new FightSkill(Owner, StaticData.dicSkillInfo[skillId], level);
                    listSkills.Add(fightSkill);
                    DoSkill(fightSkill);
                }
            }
        }

       

        //过滤护盾伤害
        public void DoDamage(ref int hurt, ref int shieldHurt)
        {
            //处理盾牌伤害
            for (int i = 0; i < listShield.Count; i++)
            {
                int tempHurt = Mathf.Min(hurt, listShield[i].sheild);

                listShield[i].sheild -= tempHurt;
                hurt -= tempHurt;
                shieldHurt += tempHurt;

                if (listShield[i].sheild <= 0)
                {
                    Owner.AddReport(new FightReportShieldRemove(Owner.Time, Owner.teamId, 
                        Owner.id, listShield[i].info.id));
                    listShield.RemoveAt(i);
                    i--;
                }

                if (hurt <= 0)
                {
                    break;
                }
            }
        }


        public void TriggerEffect(TriggerType type, FightAttackDataBase something = null)
        {
            for (int i = 0; i < listSkills.Count; i++)
            {
                if (listSkills[i].Trigger(type, something))
                {
                    int[] targetIds = listSkills[i].fightEffectBox.GetEffectTargetIds();
                    Owner.AddReport(new FightReportRoleSkillDone(Owner.Time, Owner.teamId, Owner.id, listSkills[i].skillId, targetIds, Owner.mp));
                }
            }

            Owner.buffComp.TriggerEffect(type, something);

            for (int i = 0; i < listAura.Count; i++)
            {
                listAura[i].Trigger(Owner, type, something);
            }
        }

        public void ShieldAdd(FightShield fightShield)
        {
            for (int i = 0; i < listShield.Count; i++)
            {
                if (listShield[i].info.id == fightShield.info.id)
                {
                    return;
                }
            }

            listShield.Add(fightShield);
            Owner.AddReport(new FightReportShieldAdd(Owner.Time, Owner.teamId, Owner.id, fightShield.info.id));
        }

        public FightShield ShieldCreate(Role target, FightShieldInfo fightShieldInfo, int level)
        {
            FightShield fightShield = new FightShield(Owner.Time, Owner, target, fightShieldInfo, level);

            TriggerEffect(TriggerType.ShieldCreated, fightShield);

            return fightShield;
        }

        public void AuraAdd(FightAura v)
        {
            listAuraMine.Add(v);
            Owner.AddReport(new FightReportRoleAuraAdd(Owner.Time, Owner.teamId, Owner.id, v.info.id));

            v.Trigger(Owner, TriggerType.AuraCreate);
            v.Trigger(Owner, TriggerType.AuraNone);
        }

        public void AuraRemove(int v)
        {
            for (int i = 0; i < listAuraMine.Count; i++)
            {
                if (listAuraMine[i].info.id == v)
                {
                    AuraRemove(listAuraMine[i]);
                    break;
                }
            }
        }

        public void AuraRemove(FightAura v)
        {
            v.Clear();
            listAuraMine.Remove(v);
            Owner.AddReport(new FightReportRoleAuraRemove(Owner.Time, Owner.teamId, Owner.id, v.info.id));

            v.Trigger(Owner, TriggerType.AuraEnd);
        }

        public void AuraEnter(FightAura v)
        {
            listAura.Add(v);

            if (string.IsNullOrEmpty(v.info.attr) == false)
            {
                Owner.AttrChange(v.attr, v.attrValue);
            }
            TriggerEffect(TriggerType.AuraEnter);
        }

        public void AuraExit(FightAura v)
        {
            listAura.Remove(v);

            if (string.IsNullOrEmpty(v.info.attr) == false)
            {
                Owner.AttrChange(v.info.attr, -v.attrValue);
            }
            TriggerEffect(TriggerType.AuraExit);
        }

        public virtual bool DoSkill(FightSkill skillCasting)
        {
            if (skillCasting.Trigger(TriggerType.None) == false)
            {
                return false;
            }

            int[] targetIds = skillCasting.fightEffectBox.GetEffectTargetIds();

            if (skillCasting.info.Type == 0 || skillCasting.info.Type == 3)
            {
                Owner.mp = 0;
                Owner.autoUseSkillOnce = false;
                TriggerEffect(TriggerType.Skill);
            }
            else if (skillCasting.info.Type == 2)
            {
                Owner.AddMp(12);
            }

            Owner.AddReport(new FightReportRoleCastSkill(Owner.Time, Owner.teamId, Owner.id, 
                skillCasting.skillId, targetIds, Owner.mp, Owner.attackCd));

            if (skillCasting.isChannelling)
            {
                _skillChannelling = skillCasting;
            }
            else
            {
                Owner.AddReport(new FightReportRoleSkillDone(Owner.Time, Owner.teamId, Owner.id, skillCasting.skillId, targetIds, Owner.mp));
            }

            Owner.AddReport(new FightReportRoleHpMp(Owner.Time, Owner.teamId, Owner.id, Owner.hp, Owner.mp));

            Owner.actionEndTime = Owner.Time + skillCasting.info.EndTime / 1000f;
            //lastAttackTime = actionEndTime;

            return true;
        }

    }
}