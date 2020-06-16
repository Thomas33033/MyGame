using UnityEngine;
using System.Collections;

namespace Fight
{
    public class FightRoleSummon : FightRole
    {
        public FightRoleSummon(int teamId, int uid, AttributeData  attr, float hpInit, int mpInit, FightSkillData[] skills, string tag) : base(teamId, uid, attr, hpInit, mpInit, skills, tag)
        {
            type = RoleType.Summon;
        }
    }
}