using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fight
{
    public class FightObject
    {
        public BattleState state;

        protected BattleField battleField;

        public float Time { get => battleField.composite.Time; }

        public virtual void SetBattleField(BattleField v)
        {
            battleField = v;
            state = BattleState.Fight;
        }

        public virtual void Update(float nowTime)
        {
        }
    }
}