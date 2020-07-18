using UnityEngine;
using System.Collections;

namespace Fight
{
    public class FightRoleSummon : FightRole
    {
        public FightRoleSummon(int teamId, AttributeData  attr, float hpInit, int mpInit, FightSkillData[] skills, string tag) : base(teamId, attr, hpInit, mpInit, skills, tag)
        {
            type = RoleType.Summon;
        }
    }
}