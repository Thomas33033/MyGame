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

        public FightTeam(int teamId, FightType fightType, FightSkillData[] skills) : base(teamId, new AttributeData(), 0, 0, new FightSkillData[0], "")
        {
            this.id = teamId;
            this.fightType = fightType;
            this.type = RoleType.Team;
            skillComp.SkillCreate(skills);
        }

        public override void Update(float nowTime)
        {
            //base.Update(nowTime);)
        }
    }
}