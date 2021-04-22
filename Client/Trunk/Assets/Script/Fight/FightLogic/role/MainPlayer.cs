using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Fight
{
    public class MainPlayer : Role
    {
        public static MainPlayer instannce;

        public MainPlayer(int teamId, AttributeData attr, float hpInit, int mpInit, FightSkillData[] skills, string tag) 
            : base(teamId, attr, hpInit, mpInit, skills, tag)
        {
            type = RoleType.MainPlayer;
            instannce = this;
        }

        //手动释放主动技能
        public void CastSkill(int index)
        {
            skillComp.CastSkill(index);
        }

        public void MoveTo(int gridId)
        {
            this.moveComponent.MoveByDir(this.battleField.GetNodeById(gridId));
        }

        public void StopMove()
        {
           this.moveComponent.StopMove();
        }

    }
}
