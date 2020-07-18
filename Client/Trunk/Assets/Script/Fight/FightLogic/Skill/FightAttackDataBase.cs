using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fight
{
    public class FightAttackDataBase
    {
        public Role attacker;

        public Role target;

        public DamageSourceType damageSourceType;

        public int level;

        public FightAttackDataBase(Role attacker, Role target, int level, DamageSourceType damageSourceType)
        {
            this.attacker = attacker;
            this.target = target;
            this.level = level;
            this.damageSourceType = damageSourceType;
        }
    }
}