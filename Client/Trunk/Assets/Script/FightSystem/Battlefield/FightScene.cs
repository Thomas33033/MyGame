using UnityEngine;

namespace Fight
{
    /// <summary>
    /// 场景逻辑类
    /// </summary>
    public class FightScene:Singleton<FightScene>
    {
        public FightCompositeBehaviour compBehaviour;

        public void Update()
        {
            if (compBehaviour != null)
            {
                compBehaviour.Update();
            }
        }

        public void InitFight(FightType fightType, FightData fightData)
        {
            if (fightType == FightType.ConmmFight)
            {
                compBehaviour = new FightBattleCompositeBehaviour();

                compBehaviour.InitFight(fightType,fightData);

            }
        }

       

        public void CreateRole(FightHeroData fightBuildData)
        {
            FightBattleCompositeBehaviour compBav = ((FightBattleCompositeBehaviour)compBehaviour);
            int teamId = 1;
            int BattleFieldId = 1;
            compBav.RoleAdd(fightBuildData, teamId, BattleFieldId);
        }

        public void DeleteRole()
        {

        }
    }
}

