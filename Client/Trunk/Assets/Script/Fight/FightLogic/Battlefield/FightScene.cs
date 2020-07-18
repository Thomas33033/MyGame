using System.Collections.Generic;
using UnityEngine;

namespace Fight
{
    /// <summary>
    /// 场景逻辑类
    /// </summary>
    public class FightScene
    {
        public static FightScene Instance;

        public FightCompositeBehaviour compBehaviour;

        public float SysTime => fightCenter.GetGameTime();

        private FightCenter fightCenter;

        public FightScene(FightCenter fightCenter)
        {
            Instance = this;
            this.fightCenter = fightCenter;
            RandomTools.SetRandomSeed(180);
        }

        public void InitFight(FightType fightType, FightData fightData)
        {
            if (fightType == FightType.ConmmFight)
            {
                StaticData.Init();

                compBehaviour = new FightBattleCompositeBehaviour();

                compBehaviour.InitFight(fightType,fightData);

                compBehaviour.StartBattle();
            }
        }

        public void Update()
        {
            if (compBehaviour != null)
            {
                compBehaviour.Update();
            }

        }

        public void CreateRole(FightRoleData fightBuildData)
        {
            FightBattleCompositeBehaviour compBav = ((FightBattleCompositeBehaviour)compBehaviour);
            int BattleFieldId = 1;
            compBav.RoleAdd(fightBuildData, fightBuildData.teamId, BattleFieldId);
            SaveFightInfo();
        }

        public void DeleteRole()
        {

        }
        
        public void SaveFightInfo()
        {
            List<Role> roles = compBehaviour.composite.listAllRoles;

            List<SceneEntity> lstRoleData = new List<SceneEntity>();
            for (int i = 0; i < roles.Count; i++)
            {
                if (roles[i].teamId != 1)
                {
                    SceneEntity entity = new SceneEntity();
                    entity.npcId = roles[i].npcId;
                    entity.npcPos = roles[i].node.Id;
                    entity.nodeCost = roles[i].costNodes;
                    entity.curHp = roles[i].hp / roles[i].hpMax;
                    entity.curMp = roles[i].mp;
                    entity.level = 1;
                    entity.teamId = roles[i].teamId;
                    lstRoleData.Add(entity);
                }
            }

            string json = SimpleJson.SimpleJson.SerializeObject(lstRoleData);
            StaticData.SaveData("FightScene.json", json);
        }
    }


}

