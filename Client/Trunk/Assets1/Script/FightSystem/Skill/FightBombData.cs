﻿using System.Collections.Generic;

namespace Fight
{
    public class FightBombData : FightObject
    {
        public Role role;

        public string damageId;

        public int level;

        public int range;

        public MapGrid position;

        public float delay;

        private float _time;

        public FightBombData(Role role, string damageId, int level, int range, MapGrid position, float delay)
        {
            this.role = role;
            this.damageId = damageId;
            this.level = level;
            this.range = range;
            this.position = position;
            this.delay = delay;
        }

        public override void SetBattleField(BattleField v)
        {
            base.SetBattleField(v);
            _time = Time;
        }

        public override void Update(float nowTime)
        {
            if (nowTime > _time + delay)
            {
                List<Role> listTargets = battleField.GetEnemy(role, 1, range);
                for (int i = 0; i < listTargets.Count; i++)
                {
                    FightAttackData fightAttackData = new FightAttackData(role, listTargets[i], level, Time, damageId, DamageSourceType.Skill, null);
                    fightAttackData.Hit();
                    battleField.AddAttackData(fightAttackData);
                }

                state = BattleState.Die;
            }
        }
    }
}