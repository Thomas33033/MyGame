﻿
using UnityEngine;

namespace Fight
{
    public class FightEffectActionAttack : FightEffectAction
    {
        public FightEffectActionAttack(FightEffectActionInfo info, FightSkillEffectData effectData) : base(info, effectData)
        {
        }

        public int damageId => keys[0];

        public float timeDelay => GetValue(0, 0) / 1000f;

        public float timeSpeed => GetValue(1, 0) / 1000f;

        public override void Update(float nowTime)
        {

        }


        public override bool Execute(FightEffect fightEffect, FightAttackDataBase something)
        {
            float now = fightEffect.role.Time;
            float timeDelay = this.timeDelay;
            float timeSpeed = this.timeSpeed;

            DamageSourceType damageSourceType = fightEffect.box.damageSourceType;

            if (fightEffect.listTargets.Count > 0)
            {
                for (int i = 0; i < fightEffect.listTargets.Count; i++)
                {
                    if (fightEffect.listTargets[i].state != BattleState.Fight)
                        continue;

                    float time = now + timeDelay;
                    if (timeSpeed > 0)
                    {
                        int d = (int)Vector3.Distance(fightEffect.listTargets[i].node.pos, fightEffect.role.node.pos);
                        time += timeSpeed * d;
                    }
                    FightAttackData fightAttackData = new FightAttackData(fightEffect.role, fightEffect.listTargets[i], fightEffect.level, time, damageId, damageSourceType, fightEffect.box);

                    fightEffect.AddAttackData(fightAttackData);
                }
            }
            else
            {
                float time = now + timeDelay;
                time += timeSpeed * fightEffect.role.range+1;

                fightEffect.box.attacker.AddReport(new FightReportRoleAttack(now,
                    fightEffect.role.teamId, fightEffect.role.id,
                    -1,
                    damageId,
                    time,
                    0,0, false));
            }

            

            return true;
        }
    }
}