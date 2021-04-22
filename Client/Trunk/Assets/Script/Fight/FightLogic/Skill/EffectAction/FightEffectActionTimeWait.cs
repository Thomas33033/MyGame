using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fight
{
    public class FightEffectActionTimeWait : FightEffectAction
    {
        public float duration { get => GetValue(0, 0) / 1000f; }

        private float _time;

        private FightEffect _fightEffect;

        public FightEffectActionTimeWait(FightEffectActionInfo info, FightSkillEffectData effectData) : base(info, effectData)
        {
        }

        public override bool Execute(FightEffect fightEffect, FightAttackDataBase something)
        {
            _time = fightEffect.role.Time;
            fightEffect.needUpdating = true;
            _fightEffect = fightEffect;
            return true;
        }

        public override void Update(float nowTime)
        {
            if (_time + duration <= nowTime)
            {
                _fightEffect.needUpdating = false;
                FightEffect temp = _fightEffect;
                _fightEffect = null;
                temp.DoNext();
            }
        }
    }
}