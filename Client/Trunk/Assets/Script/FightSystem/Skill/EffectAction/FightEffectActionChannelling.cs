namespace Fight
{
    /// <summary>
    /// 引导施法
    /// </summary>
    public class FightEffectActionChannelling : FightEffectAction
    {
        public float duration => GetValue(0, 0) / 1000f;

        private float _time;

        private FightEffect _fightEffect;

        public FightEffectActionChannelling(FightEffectActionInfo info, FightSkillEffectData effectData) : base(info, effectData)
        {
        }

        public override bool Execute(FightEffect fightEffect, FightAttackDataBase something)
        {
            _time = fightEffect.role.Time;
            fightEffect.isChannelling = true;
            //fightEffect.needUpdating = true;
            _fightEffect = fightEffect;
            return true;
        }

        public override void Update(float nowTime)
        {
            if (_time + duration <= nowTime)
            {
                _fightEffect.isChannelling = false;
                //_fightEffect.needUpdating = false;
                FightEffect temp = _fightEffect;
                _fightEffect = null;
                temp.DoNext();
            }
        }
    }
}