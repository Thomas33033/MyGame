using System;
using UnityEngine;

namespace Fight
{
    public class FightAttackData : FightAttackDataBase
    {
        public int damageId;

        public int damageType { get => info == null ? 1 : info.type; }

        public int bouns;

        public float damage;

        public int hurt;

        public bool isHit;

        public bool isCrit;

        public float time;

        public bool isDie;

        public FightEffectBox fightEffectBox;

        public FightDamageInfo info;

        public int defensePenetration;

        public int magicDefensePenetration;

        public int defenseDestroy;

        public int magicDefenseDestroy;

        public FightAttackData(Role attacker, Role target, int level, float time, int damageId, DamageSourceType damageSourceType, FightEffectBox fightEffectBox = null) : base(attacker, target, level, damageSourceType)
        {
            this.damageId = damageId;
            this.damageSourceType = damageSourceType;
            this.time = time;
            this.fightEffectBox = fightEffectBox;

            info = StaticData.GetDamageInfo(damageId, level);

#if UNITY_EDITOR
            if (info == null)
            {
                UnityEngine.Debug.LogError("error damageId " + damageId);
            }
#endif
        }

        public void Hit()
        {
            isHit = isHit || CheckHit();
        }

        public void Execute()
        {
            if (info.type == 0)
            {
                if (fightEffectBox != null)
                    fightEffectBox.Trigger(TriggerType.AttackDataExecuted, this, 1);
                return;
            }

            if (isHit)
            {
                this.defenseDestroy += attacker.defenseDestroy;
                this.defensePenetration += attacker.defensePenetration;
                this.magicDefenseDestroy += attacker.magicDefenseDestroy;
                this.magicDefensePenetration += attacker.magicDefensePenetration;

                CreateDamage();

                if (damageType == 3)
                {
                    attacker.TriggerEffect(TriggerType.AttackDataAttackerCureBefore, this);
                    if (fightEffectBox != null)
                        fightEffectBox.Trigger(TriggerType.AttackDataAttackerCureBefore, this, 1);
                    target.TriggerEffect(TriggerType.AttackDataTargetCureBefore, this);
                }
                else if (damageType == 1 || damageType == 2)
                {
                    attacker.TriggerEffect(TriggerType.AttackDataAttackerDamageBefore, this);
                    if (fightEffectBox != null)
                        fightEffectBox.Trigger(TriggerType.AttackDataAttackerDamageBefore, this, 1);
                    target.TriggerEffect(TriggerType.AttackDataTargetDamageBefore, this);
                }

                if (damageType == (int)FightDamageType.Hp)
                {
                    damage = (GetSkillDamageBase() + damage * GetSkillDamageBouns() * (1f + bouns / 100f));
                }
                else if (damageType == (int)FightDamageType.Mp)
                {
                    damage = GetSkillDamageBase() + damage * GetSkillDamageBouns();
                }
                else
                {
                    damage = Math.Max(1, (GetSkillDamageBase() + damage * GetSkillDamageBouns()) * (1f + bouns / 100f) * (1f + attacker.damageBouns - target.damageReduction));
                }

                isCrit = isCrit || CheckCrit();
                if (isCrit)
                {
                    damage *= 1.5f;
                }

                hurt = (int)damage;
                target.AttackDataExecute(this);

                if (damageType == (int)FightDamageType.Hp)
                {
                    if (fightEffectBox != null)
                        fightEffectBox.Trigger(TriggerType.AttackDataAttackerCureHit, this, 1);
                    attacker.TriggerEffect(TriggerType.AttackDataAttackerCureHit, this);
                    target.TriggerEffect(TriggerType.AttackDataTargetCureHit, this);
                }
                else if (damageType != (int)FightDamageType.Mp)
                {
                    if (fightEffectBox != null)
                        fightEffectBox.Trigger(TriggerType.AttackDataAttackerHit, this, 1);
                    attacker.TriggerEffect(TriggerType.AttackDataAttackerHit, this);
                    target.TriggerEffect(TriggerType.AttackDataTargetHit, this);
                }

                if (isDie)
                {
                    if (fightEffectBox != null)
                        fightEffectBox.Trigger(TriggerType.Kill, this, 1);
                    attacker.TriggerEffect(TriggerType.Kill, this);
                }
            }
            else
            {
                attacker.AttackDataMiss(target);
                target.AttackDataDodge(attacker);
            }

            //if (fightEffectBox != null)
            //    fightEffectBox.Trigger(TriggerType.AttackDataExecuted, this, 1);
        }

        private void CreateDamage()
        {
            if (info.damageBase == 1)
            {
                damage = attacker.physicsAttack;
            }
            else if (info.damageBase == 2)
            {
                damage = attacker.hpMax;
            }
            else if (info.damageBase == 3)
            {
                damage = attacker.hp;
            }
            else if (info.damageBase == 4)
            {
                damage = target.hpMax;
            }
            else if (info.damageBase == 5)
            {
                damage = target.hp;
            }
            else if (info.damageBase == 6)
            {
                damage = target.mpMax;
            }
            else
            {
                damage = attacker.physicsAttack;
                target.AttackDataDefense(this);
            }
        }

        private float GetSkillDamageBase()
        {
            if (damageId <= 0)
                return 0f;

            if (info.damage.Length == 0)
                return 0f;

            return info.damage[Mathf.Min(info.damage.Length - 1, level - 1)];
        }

        private float GetSkillDamageBouns()
        {
            if (damageId <= 0)
                return 0f;
            if (info.bouns.Length == 0)
                return 0f;
            return info.bouns[Mathf.Min(info.bouns.Length - 1, level - 1)] / 1000f;
        }

        private bool CheckHit()
        {
            if (info.type == 0)
                return true;

            if (damageSourceType != DamageSourceType.Attack)
                return true;
            int rate = attacker.hit - target.dodge;
            return UnityEngine.Random.Range(0, 100) < rate;
        }

        private bool CheckCrit()
        {
            if (damageSourceType != DamageSourceType.Attack)
                return false;

            int rate = attacker.crit;
            return UnityEngine.Random.Range(0, 100) < rate;
        }
    }
}