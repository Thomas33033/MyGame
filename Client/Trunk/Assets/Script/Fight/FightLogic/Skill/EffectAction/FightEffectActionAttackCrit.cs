using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fight
{
    public class FightEffectActionAttackCrit : FightEffectAction
    {
        public FightEffectActionAttackCrit(FightEffectActionInfo info, FightSkillEffectData effectData) : base(info, effectData)
        {
        }

        public override bool Execute(FightEffect fightEffect, FightAttackDataBase something)
        {
            for (int i = 0; i < fightEffect.listAttackData.Count; i++)
            {
                fightEffect.listAttackData[i].isCrit = true;
            }

            return true;
        }
    }
}