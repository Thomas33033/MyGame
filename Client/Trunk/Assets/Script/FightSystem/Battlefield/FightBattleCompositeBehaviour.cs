using UnityEngine;
using UnityEditor;

namespace Fight 
{
    /// <summary>
    /// 塔防战场组合
    /// </summary>
    public class FightBattleCompositeBehaviour : FightCompositeBehaviour
    {
        public BattleComposite composite;

        public override bool isFight => composite.isFight;

        public override float GetTime()
        {
            if (composite == null)
                return 0f;

            return composite.Time;
        }

        public override void InitFight(FightType fightType, FightData fightData)
        {
            base.InitFight(fightType, fightData);

            composite = new BattleComposite();
            composite.fightType = fightType;

            BattleField battleField = new BattleField(1, "BattleField");
            composite.AddBattleField(battleField);

            InitPlayer(fightType, _fightData.selfBattleData, battleField);
            InitEnemy(fightType, _fightData.enemyBattleData, battleField);

            composite.Init();
        }

        private void InitPlayer(FightType fightType, FightPlayerData fightPlayerData, BattleField battleField)
        {
            FightTeam fightTeam = new FightTeam(fightPlayerData.userData.userID, fightType, fightPlayerData.userData.userID, fightPlayerData.teamSkills);
            fightTeam.isPlayer = true;
            composite.AddFightTeam(fightTeam, battleField.id);

            for (int i = 0; i < fightPlayerData.heroData.Length; i++)
            {
                int p = fightPlayerData.heroData[i].Position;

                composite.AddRoleOnBattleField(fightTeam.id, battleField.id, fightPlayerData.heroData[i], p, true);
            }
        }

        private void InitEnemy(FightType fightType, FightPlayerData fightPlayerData, BattleField battleField)
        {
            FightTeam fightTeam = new FightTeam(fightPlayerData.userData.userID, fightType, fightPlayerData.userData.userID, fightPlayerData.teamSkills);

            composite.AddFightTeam(fightTeam, battleField.id);

            for (int i = 0; i < fightPlayerData.heroData.Length; i++)
            {
                int p = fightPlayerData.heroData[i].Position;

                p = 28 + p % 4 - p / 4 * 4;

                composite.AddRoleOnBattleField(fightTeam.id, battleField.id, fightPlayerData.heroData[i], p, false);
            }
        }

        public override void SetAutoSkill(bool v)
        {
            composite.SetAutoSkill(v);
        }

        public override void StartBattle()
        {
            composite.StartBattle();

            _startCompositeTime = Time.time;
        }

        public override void PrepareFight()
        {
            composite.PrepareFight();
        }

        private float _lastCompositeTime;
        private float _startCompositeTime;

        private void FixedUpdate()
        {
            if (composite == null)
                return;

#if UNITY_EDITOR
            composite.isTest = true;
            composite.win = false;
#endif

            if (composite.isFight)
            {
                composite.Update(Time.deltaTime);
            }

            composite.GetReport(ref _listReport);
        }

        public override void UseSkill(int roleId)
        {
            composite.UseSkill(roleId);
        }


        public override void PauseFight()
        {
            composite.isFight = false;
        }

        public override void PlayFight()
        {
            composite.isFight = true;
        }

        public override void SymbolsAdd(string v)
        {
            composite.symbols.Add(v);
        }

        public void RoleAdd(FightHeroData heroData,int teamId, int battleFieldId)
        {
            composite.AddRoleOnBattleField(teamId, battleFieldId, heroData, heroData.Position, false);
        }
    }
}

