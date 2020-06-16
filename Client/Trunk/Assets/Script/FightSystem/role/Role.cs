
using System.Collections.Generic;
using UnityEngine;
namespace Fight
{
    public class Role : AttributeBase
    {

        private static int _roleIndex;

        public int id;

        public int uid;

        public int teamId;

        public string tag;

        public RoleType type;

        public BattleField battleField;

        //战斗状态
        public BattleState state;

        //攻击目标
        public Role target;

        public float actionEndTime;

        private float _skillEndTime;

        public float Time;

        public MapGrid position;

        private int _hp;
        public virtual int hp { get => _hp; set => _hp = value; }

        private int _mp;
        public virtual int mp { get => _mp; set => _mp = value; }

        public virtual float lastAttackTime { get => actionEndTime; set => actionEndTime = value; }

       
        public bool autoUseSkill;

        public bool autoUseSkillOnce;

        public bool isPlayer;

        public int site;

        private float _hpInit;

        private int _mpInit;

        public int place;

        public SkillComponent skillComp;
        public TagComponent tagComp;
        public ReportComponent reportComp;
        public FindPathComponent findPathComp;
        public ScanTargetComponent scanTargetComp;
        public BuffComponent buffComp;

        public Role(int teamId, int uid, AttributeData attr,
                float hpInit, int mpInit, FightSkillData[] skills, string tag) : base(attr)
        {
            _roleIndex++;

            _hpInit = hpInit;
            _mpInit = mpInit;

            this.id = _roleIndex;

            this.teamId = teamId;

            this.uid = uid;

            this.attributeBase = attr;

            this.mp = 0;


            reportComp = new ReportComponent(this);
            skillComp = new SkillComponent(this);
            tagComp = new TagComponent(this);
            findPathComp = new FindPathComponent(this);
            scanTargetComp = new ScanTargetComponent(this);
            buffComp = new BuffComponent(this);

            skillComp.SkillCreate(skills);
            tagComp.TagCreate(tag);
        }

        public virtual void Init()
        {
            TriggerEffect(TriggerType.Init);
            UpdateAttribute();
            this.hp = Mathf.CeilToInt(hpMax * _hpInit);
            this.mp = _mpInit;
        }

        public void AttackDataDodge(Role attacker)
        {
        }
            
        //攻击Miss
        public void AttackDataMiss(Role target)
        {
            this.AddReport(new FightReportRoleHurt(Time, teamId, id, hp, 0, false, mp, 1, 1, false));
        }

        //准备战斗
        public virtual void PrepareFight()
        {
            state = BattleState.Fight;
            TriggerEffect(TriggerType.PrepareFight);
        }

        public void SetBattleField(BattleField v)
        {
            battleField = v;
        }

        public BattleField GetBattlefield()
        {
            return battleField;
        }

        //增加魔法值
        public void AddMp(int v)
        {
            if (mp >= mpMax)
                return;
            if (skillComp._skillChannelling != null)
                return;

            mp = Mathf.Min(mp + v, mpMax);
            if (mp < 0)
                mp = 0;

            if (mp >= mpMax)
            {
                TriggerEffect(TriggerType.MpMax);
            }
        }

        public virtual void Attack()
        {
        }

        protected virtual void Idle(float nowTime)
        {

        }


        //处理任务AI
        public virtual void Update(float nowTime)
        {
            //
            skillComp.OnUpdate(nowTime);

            if (StatusCheck(RoleStatus.Dizz)) return;
            

            if (skillComp._skillChannelling != null) return;


            if (actionEndTime != 0 && nowTime < actionEndTime)
            {
                return;
            }

            //目标不存在
            if (target != null && (target.state != BattleState.Fight || target.StatusCheck(RoleStatus.Hide)))
            {
                target = null;
                TriggerEffect(TriggerType.TargetChange);
            }

            if (target == null)
            {
                FindTarget();
            }

            if (StatusCheck(RoleStatus.Silent) == false && CheckSkill())
            {
                if (CastSkill())
                {
                    return;
                }
            }

            if (target == null)
            {
                Idle(nowTime);
            }
            else if (CheckAttackDistance() == false)
            {
                MoveTarget(nowTime);
            }
            else if (nowTime > lastAttackTime)
            {
                Attack();
                lastAttackTime = nowTime + attackCd;
            }
        }

        protected virtual bool CheckSkill()
        {
            return (autoUseSkill || isPlayer == false || autoUseSkillOnce) && mp != 0 && mp >= mpMax && _skillEndTime == 0;
        }


        protected virtual bool CastSkill()
        {
            return false;
        }

        public void UseSkill()
        {
            actionEndTime = Time;
            autoUseSkillOnce = true;
            //if (mp < mpMax)
            //    return;
            //CastSkill(0);
        }

        protected virtual bool DoSkill(FightSkill skillCasting)
        {
            return skillComp.DoSkill(skillCasting);
        }


        public void AuraAdd(FightAura v)
        {
            skillComp.AuraAdd(v);
        }

        public void AuraRemove(int v)
        {
            skillComp.AuraRemove(v);
        }

        public void AuraRemove(FightAura v)
        {
            skillComp.AuraRemove(v);
        }

        public void AuraEnter(FightAura v)
        {
            skillComp.AuraEnter(v);
        }

        public void AuraExit(FightAura v)
        {
            skillComp.AuraExit(v);
        }

        protected virtual bool CheckAttackDistance()
        {
            return position.Distance(target.position) <= range;
        }

        protected virtual void FindTarget()
        {
        }

        protected virtual void MoveTarget(float nowTime)
        {

        }

        protected bool MoveTo(MapGrid grid, bool isToWeapon = false)
        {
            if (battleField.CheckMove(grid, !isToWeapon))
            {
                battleField.RoleMove(this, grid);
                actionEndTime = Time + moveSpeed / 200f;
                AddReport(new FightReportRoleMove(Time, teamId, id, battleField.id, actionEndTime, position.ToArray()));
                return true;
            }
            return false;
        }

        public void JumpTo(MapGrid grid)
        {
            if (battleField.CheckMove(grid) == false)
                return;
            battleField.RoleMove(this, grid);
            actionEndTime = Time + 1.5f;
            AddReport(new FightReportRoleJump(Time, teamId, id, battleField.id, actionEndTime, position.ToArray()));
        }

        #region Damage

        public void AddAttackData(FightAttackData fightAttackData)
        {
            if (fightAttackData.fightEffectBox != null)
                fightAttackData.fightEffectBox.Trigger(TriggerType.AttackDataAttackerHitBefore, fightAttackData, 1);
            TriggerEffect(TriggerType.AttackDataAttackerHitBefore, fightAttackData);

            fightAttackData.Hit();

            if (fightAttackData.time <= Time)
            {
                fightAttackData.Execute();
            }
            else
            {
                battleField.AddAttackData(fightAttackData);
            }

            AddReport(new FightReportRoleAttack(Time, teamId, id, fightAttackData.target.id, fightAttackData.damageId, fightAttackData.time, mp, attackCd, fightAttackData.isHit));
        }

        public virtual void AttackDataDefense(FightAttackData fightAttackData)
        {
            if (fightAttackData.damageType == (int)FightDamageType.Hp)
            {
                //TriggerEffect(TriggerType.CureBefore, fightAttackData);
            }
            else if (fightAttackData.damageType == (int)FightDamageType.Physical)
            {
                //TriggerEffect(TriggerType.DamageBefore, fightAttackData);
                fightAttackData.damage = Mathf.Max(fightAttackData.damage * 0.1f, fightAttackData.damage - 
                    (physicsDefense * (1f - fightAttackData.defenseDestroy / 100f) - fightAttackData.defensePenetration));
            }
            else if (fightAttackData.damageType == (int)FightDamageType.Magic)
            {
                //TriggerEffect(TriggerType.DamageBefore, fightAttackData);
                fightAttackData.damage = Mathf.Max(fightAttackData.damage * 0.1f, fightAttackData.damage - 
                    (magicDefense * (1f - fightAttackData.magicDefenseDestroy / 100f) - fightAttackData.magicDefensePenetration));
            }
        }

        public virtual void AttackDataExecute(FightAttackData fightAttackData)
        {
            if (fightAttackData.damageType == 3)
            {
                //TriggerEffect(TriggerType.CureBefore, fightAttackData);
                DoTreat(fightAttackData);
            }
            else if (fightAttackData.damageType == 4)
            {
                fightAttackData.hurt = Mathf.FloorToInt(fightAttackData.damage);
                AddMp(fightAttackData.hurt);
            }
            else
            {
                //TriggerEffect(TriggerType.DamageBefore, fightAttackData);
                DoDamage(fightAttackData);

                fightAttackData.isDie = state == BattleState.Die;

                if (hpSucking > 0)
                {
                    int suckHp = Mathf.FloorToInt(fightAttackData.hurt * hpSucking / 100f);

                    if (suckHp > 0)
                    {
                        DoCure(suckHp, false, (int)DamageSourceType.Suck, (int)FightDamageType.Hp);
                    }
                }
            }
        }

        //处理伤害
        public virtual void DoDamage(FightAttackData fightAttackData)
        {
            int hurt = fightAttackData.hurt;
            int shieldHurt = 0;

            skillComp.DoDamage(ref hurt, ref shieldHurt);

            if (shieldHurt > 0)
            {
                reportComp.AddReport(new FightReportRoleHurt(Time, teamId, id, hp, shieldHurt, fightAttackData.isCrit, this.mp, (int)DamageSourceType.Shield, 1));
            }

            fightAttackData.hurt = Mathf.Max(0, hurt);
            if (fightAttackData.hurt > 0)
            {
                TriggerEffect(TriggerType.AttackDataTargetHurtBefore, fightAttackData);
                DoHurt(fightAttackData.hurt, fightAttackData.isCrit, (int)fightAttackData.damageSourceType, fightAttackData.damageType);
                if (hp > 0)
                {
                    TriggerEffect(TriggerType.AttackDataTargetHurtAfter, fightAttackData);
                }
            }
        }

        //处理伤害
        public virtual void DoHurt(int hurt, bool isCrit, int damageSourceType, int damageType)
        {
            if (hurt <= 0)
                return;

            hp -= hurt;

#if UNITY_EDITOR
            if (battleField.composite.isTest && hp < 1)
            {
                hp = 1;
            }
            if (battleField.composite.win && isPlayer == false)
            {
                hp = 0;
            }
#endif

            if (hp <= 0)
            {
                hp = 0;
                TriggerEffect(TriggerType.Dying);
            }

            if (StatusCheck(RoleStatus.Undead) && hp < 1)
            {
                hp = 1;
                Debug.Log("set Undead hp");
            }

            AddMp(Mathf.CeilToInt(150 * hurt / hpMax));

            AddReport(new FightReportRoleHurt(Time, teamId, id, hp, hurt, isCrit, this.mp, damageSourceType, damageType));

            if (hp <= 0 && StatusCheck(RoleStatus.Undead) == false)
            {
                Die();
            }
        }

        //治疗
        public void DoTreat(FightAttackData fightAttackData)
        {
            DoCure(fightAttackData.hurt, fightAttackData.isCrit, (int)fightAttackData.damageSourceType, fightAttackData.damageType);
        }

        //增加治疗战报
        public void DoCure(int hurt, bool isCrit, int damageSourceType, int damageType)
        {
            if (hp >= hpMax || hurt < 0)
                return;
            int hpTemp = hp;
            hp = Mathf.Min(hpMax, hp + hurt);
            AddReport(new FightReportRoleHurt(Time, teamId, id, hp, hp - hpTemp, isCrit, this.mp, damageSourceType, damageType));
        }

        #endregion Damage

        public void AddReport(FightReport fightReport)
        {
            reportComp.AddReport(fightReport);
        }


        /// <summary>
        /// 死亡
        /// </summary>
        public virtual void Die()
        {
            skillComp.Clear();

            state = BattleState.Die;

            AddReport(new FightReportRoleDie(Time, teamId, id, battleField.id));

            battleField.Die(this);

            TriggerEffect(TriggerType.Die);
        }


        public void TriggerEffect(TriggerType type, FightAttackDataBase something = null)
        {
            skillComp.TriggerEffect(type, something);
        }



        public int zone
        {
            get
            {
                if (position.s > 0)
                    return 1;

                if (position.s == 0 && position.r <= 0)
                    return 1;

                return -1;
            }
        }

        public bool isMyZone
        {
            get
            {
                return (site < 16) == (zone == 1);
            }
        }

        public int siteCol
        {
            get
            {
                return site / 4;
            }
        }

        public override void StatusChange(int status, int t)
        {
            base.StatusChange(status, t);

            if (t > 0 && (StatusCheck(RoleStatus.Dizz) || StatusCheck(RoleStatus.Silent)))
            {
                if (skillComp._skillChannelling != null)
                {
                    skillComp._skillChannelling.fightEffectBox.Break();
                    skillComp._skillChannelling = null;
                }

                TriggerEffect(TriggerType.OutControl);
            }

            AddReport(new FightReportRoleState(Time, teamId, id, status, StatusCheck(RoleStatus.Dizz) ? 1 : 0));
        }

        public virtual void SkillAdd(int skillId, int level)
        {
            skillComp.SkillAdd(skillId, level);
        }

        public void GetReport(ref List<FightReport> listReport)
        {
             reportComp.GetReport(ref listReport);
        }
    }
}