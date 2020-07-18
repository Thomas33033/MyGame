using behaviac;
using System.Collections.Generic;

namespace Fight
{
    enum ETargetType
    {
        //敌人
        Enmery = 0,
        TeamMy = 1,
        TeamOther = 2,
        //自己
        Self = 3,
    }

    public class FightEffectActionFindTarget : FightEffectAction
    {
        private int targetType => GetValue(0, 0);

        private int rangeType => GetValue(3, 0);

        public FightEffectActionFindTarget(FightEffectActionInfo info, FightSkillEffectData effectData) : base(info, effectData)
        {
        }

        public override bool Execute(FightEffect fightEffect, FightAttackDataBase something)
        {
            int range = GetValue(1, fightEffect.level);

            int targetNum = GetValue(2, fightEffect.level);
            Debug.Log(range + " " + fightEffect.role.range);
            List<Role> listEnemy = fightEffect.role.scanTargetComp.FindTarget(
                (ESearchTargetType)targetType, 
                rangeType,
                range != 0 ? range : fightEffect.role.range,
                fightEffect.listTargets.Count > 0 ? fightEffect.listTargets[0] : fightEffect.role);


            if (targetNum > 0 && listEnemy.Count > targetNum)
            {
                listEnemy.RemoveRange(targetNum, listEnemy.Count - targetNum);
            }

            fightEffect.ClearTargets();

             fightEffect.AddTargets(listEnemy);

            if (fightEffect.listTargets.Count == 0)
                return false;

            return true;
        }
    }
}