using FightCommom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Fight
{
    public class FightRoleBuildings : Role
    {
        public FightRoleBuildings(int teamId, AttributeData attr, float hpInit, int mpInit, FightSkillData[] skills, string tag) : base(teamId, attr, hpInit, mpInit, skills, tag)
        {
            type = RoleType.Buildings;
            this.StatusChange(RoleStatus.Unselected, 1);
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