using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fight
{
    public class FightTeam : Role
    {
        public FightType fightType;

        public FightTeam(int teamId, FightType fightType, int uid, FightSkillData[] skills) : base(teamId, uid, new AttributeData(), 0, 0, new FightSkillData[0], "")
        {
            this.id = teamId;
            this.fightType = fightType;

            skillComp.SkillCreate(skills);
        }

        public override void SkillAdd(int skillId, int level)
        {
            if (StaticData.dicSkillInfo.ContainsKey(skillId) == false)
            {
                return;
            }
            FightSkillInfo skillInfo = StaticData.dicSkillInfo[skillId];
            if (fightType == 0)
            {
                if (skillInfo.Type != 0 && skillInfo.Type != 1 && skillInfo.Type != 2)
                {
                    return;
                }
            }

            FightSkill fightSkill = new FightSkill(this, skillInfo, level);
            skillComp.listSkills.Add(fightSkill);
        }
    }
}