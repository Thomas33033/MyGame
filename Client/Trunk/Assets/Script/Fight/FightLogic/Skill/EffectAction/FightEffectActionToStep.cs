using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fight
{
    public class FightEffectActionToStep : FightEffectAction
    {
        //public int step { get => values[0]; }

        //public int count { get => values[1]; }

        public int num;

        public FightEffectActionToStep(FightEffectActionInfo info, FightSkillEffectData effectData) : base(info, effectData)
        {
        }

        public override void Reset()
        {
            num = 0;
        }

        public override bool Execute(FightEffect fightEffect, FightAttackDataBase something)
        {
            int level = fightEffect.level;
            int step = GetValue(0, level);
            int count = GetValue(1, level);

            if (count == 0 || num < count)
            {
                fightEffect.ToStep(step);
                num++;
            }

            return true;
        }
    }
}