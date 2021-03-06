﻿using FightCommom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Fight
{
    public class FightRoleTower : Role
    {
        public FightRoleTower(int teamId, AttributeData attr, float hpInit, int mpInit, FightSkillData[] skills, string tag) : base(teamId, attr, hpInit, mpInit, skills, tag)
        {
            type = RoleType.BuildTower;
        }
        
        public override void SkillAdd(int skillId, int level)
        {
            if (StaticData.dicSkillInfo.ContainsKey(skillId) == false)
            {
                return;
            }
            FightSkillInfo skillInfo = StaticData.dicSkillInfo[skillId];

            if (skillInfo.Type != 0 && skillInfo.Type != 1 && skillInfo.Type != 2)
                return;

            FightSkill fightSkill = new FightSkill(this, skillInfo, level);

            skillComp.listSkills.Add(fightSkill);
        }

        #region Move


        public override void Update(float nowTime)
        {
           
        }

        protected override void Idle(float nowTime)
        {
            
        }

        public override void MoveTarget(float nowTime)
        {
           
        }

        #endregion Move

        protected override bool CastSkill()
        {
            return skillComp.CastSkill(SkillAttackType.Active);
        }

    }
}