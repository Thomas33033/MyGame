using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.UIElements;

namespace Fight
{
    /// <summary>
    /// 战斗组合，控制战场开始、战场结束
    /// 负责: 
    ///     1.创建战场
    ///     2.初始化战队，创建角色
    ///     3.管理角色
    ///     
    /// </summary>
    public class BattleComposite
    {
        public FightType fightType;

        public Dictionary<int, FightTeam> dicTeams;

        public Dictionary<int, BattleField> dicBattleField;

        public List<Role> listAllRoles;

        public List<FightReport> listReport;

        public float Time
        {
            get
            {
                return FightScene.Instance.SysTime;
            }
        }

        public bool isFight;

        public HashSet<string> symbols;

        public const float TimeMax = 9000f;

#if UNITY_EDITOR
        public bool isTest;

        public bool win;
#endif

        public BattleComposite()
        {
            dicTeams = new Dictionary<int, FightTeam>();
            dicBattleField = new Dictionary<int, BattleField>();
            listReport = new List<FightReport>();
            listAllRoles = new List<Role>();
            symbols = new HashSet<string>();
        }

        public void Init()
        {
            //for (int i = 0; i < listAllRoles.Count; i++)
            //{
            //    Role fightRole = listAllRoles[i];

            //    FightReportRoleAdd report = new FightReportRoleAdd(0, fightRole.teamId, fightRole.id,
            //        (int)fightRole.type, fightRole.id, fightRole.GetBattlefield().id,
            //        fightRole.hpMax, fightRole.mpMax, fightRole.node.ID, "");

            //    report.hp = fightRole.hpMax;
            //    report.mp = fightRole.mpMax;
            //    listReport.Add(report);

            //}

            foreach (var item in dicTeams)
            {
                listReport.Add(new FightReportTeamReady(0, item.Value.id));
            }
        }

        public bool CheckSymbols(string v)
        {
            return symbols.Contains(v);
        }

        public void AddFightTeam(FightTeam v, int battleFieldId)
        {
            if (dicBattleField.ContainsKey(battleFieldId) == false)
            {
                UnityEngine.Debug.LogError("not find battleField:" + battleFieldId);
            }
            else
            {
                v.SetBattleField(dicBattleField[battleFieldId]);
                dicTeams.Add(v.id, v);
            }
        }

        public void AddBattleField(BattleField v)
        {
            v.composite = this;
            dicBattleField.Add(v.id, v);
        }

        public BattleField GetOtherBattlefield(int myId)
        {
            foreach (var item in dicBattleField)
            {
                if (item.Value.id != myId)
                {
                    return item.Value;
                }
            }
            return null;
        }

        public void Update(float nowTime)
        {
            if (isFight == false)
                return;

            foreach (var item in dicTeams.Values)
            {
                if (isFight == false)
                    break;
                item.Update(nowTime);
            }

            foreach (var item in dicBattleField.Values)
            {
                if (isFight == false)
                    break;

                item.Update(nowTime);
            }

            foreach (var item in dicTeams.Values)
            {
                item.GetReport(ref listReport);
            }

            foreach (var item in dicBattleField.Values)
            {
                item.GetReport(ref listReport);
            }

            //战斗超时，游戏结束，防守方胜利
            if (isFight && nowTime >= TimeMax)
            {
                Debug.LogError("游戏结束：！！！！！！！！！！！！！！！！！！！！！");
                foreach (var item in dicTeams.Values)
                {
                    if (item.isPlayer == false)
                    {
                        GameOver(item.teamId);
                        break;
                    }
                }
            }
        }


        public bool AddRoleOnBattleField(int teamId, int battlefieldId, FightRoleData roleData, int nodeId, bool isPlayer)
        {
            BattleField battleField = dicBattleField[battlefieldId];

            if (!battleField.CanBuild(nodeId))
            {
                return false;
            }

            Role role = null;
            if (roleData.npcType == (int)RoleType.BuildTower)
            {
                role = new FightRoleTower(teamId, GetRoleAttr(roleData), roleData.CurHp, roleData.CurMp, roleData.SkillData, roleData.Tag);
            }
            else if (roleData.npcType == (int)RoleType.Buildings)
            {
                role = new FightRoleBuildings(teamId, GetRoleAttr(roleData), roleData.CurHp, roleData.CurMp, roleData.SkillData, roleData.Tag);
            }
            else if (roleData.npcType == (int)RoleType.Fighter)
            {
                role = new FightRole(teamId, GetRoleAttr(roleData), roleData.CurHp, roleData.CurMp, roleData.SkillData, roleData.Tag);
            }
            else if (roleData.npcType == (int)RoleType.MainPlayer)
            {
                role = new MainPlayer(teamId, GetRoleAttr(roleData), roleData.CurHp, roleData.CurMp, roleData.SkillData, roleData.Tag);
            }
            else
            {
                Debug.LogError("类型错误");
                return false;
            }

            role.isPlayer = isPlayer;
            role.npcId = roleData.npcId;
            role.OnInitPosition(nodeId,roleData.CostNodes, roleData.NodeSize);


            listAllRoles.Add(role);
            battleField.AddRole(role, nodeId);
            role.PrepareFight();

            if (role == null)
            {
                Debug.LogError("role == null");
            }

            if (role.GetBattlefield() == null)
            {
                Debug.LogError("role.GetBattlefield() = null");
            }

            FightReportRoleAdd report = new FightReportRoleAdd(0, role.teamId, role.id,
                   (int)role.type, role.id, role.GetBattlefield().id,
                   role.hpMax, role.mpMax, role.node.Id, roleData.Resource);
            report.hp = role.hpMax;
            report.mp = role.mpMax;
            report.nodeId = role.node.Id;
            report.CostNodes = role.costNodes;
            report.skills = role.skills;

            listReport.Add(report);


            return true;
        }

        public bool SummonFighter(int battlefield, Role attacker, float attrScale, int position, FightSkillData[] skills, string npcAsset)
        {
            AttributeData attr = attacker.AttributeData * attrScale;
            attr.moveSpeed = attacker.AttributeData.moveSpeed;
            attr.range = attacker.AttributeData.range;
            attr.hit = attacker.AttributeData.hit;
            attr.attackSpeed = attacker.AttributeData.attackSpeed;

            FightRoleSummon fightRole = new FightRoleSummon(attacker.teamId, attr, 1f, 0, skills, "Summon,1");

            BattleField battleField = dicBattleField[battlefield];
            listAllRoles.Add(fightRole);
            battleField.AddRole(fightRole, position);

            FightReportSummon report = new FightReportSummon(Time, fightRole.teamId, (int)fightRole.type, fightRole.id, 
                fightRole.GetBattlefield().id, fightRole.hpMax, fightRole.mpMax, fightRole.node.Id, npcAsset);

            fightRole.PrepareFight();

            report.hp = fightRole.hpMax;
            report.mp = fightRole.mpMax;
            listReport.Add(report);

            return true;
        }

        private AttributeData GetRoleAttr(FightRoleData info)
        {
            AttributeData attr = new AttributeData();

            attr.anger = info.MaxAnger;
            attr.physicsAttack = info.PhysicalAttack;
            attr.attackSpeed = info.AttackSpeed;

            attr.crit = info.Crit;
            attr.physicsDefense = info.PhysicalDefense;
            attr.magicDefense = info.MagicDefense;

            attr.dodge = info.Dodge;
            attr.hit = info.Hit;
            attr.hp = info.HP;
            attr.range = info.Range;
            attr.moveSpeed = info.MoveSpeed;

            return attr;
        }

        public void GetReport(ref List<FightReport> list)
        {
            list.AddRange(this.listReport);
            listReport.Clear();
        }


        public void PrepareFight()
        {
            foreach (var item in dicTeams.Values)
            {
                item.PrepareFight();
            }

            for (int i = 0; i < listAllRoles.Count; i++)
            {
                Role fightRole = listAllRoles[i];
                fightRole.PrepareFight();
            }
        }

        public void StartBattle()
        {
            isFight = true;

            listReport.Add(new FightReportGameStart(0, 0));

            for (int i = 0; i < listAllRoles.Count; i++)
            {
                Role fightRole = listAllRoles[i];

                if (fightRole.hp < fightRole.hpMax)
                {
                    fightRole.hp = fightRole.hpMax;
                    listReport.Add(new FightReportRoleHpMp(0, fightRole.teamId, fightRole.id, fightRole.hp, fightRole.mp));
                }
            }
        }

        private string GetPlayerRoleHpLeft()
        {
            List<int> listHp = new List<int>();
            for (int i = 0; i < listAllRoles.Count; i++)
            {
                Role fightRole = listAllRoles[i];
                if (fightRole.isPlayer && fightRole.type == RoleType.Fighter)
                {
                    listHp.Add(fightRole.id);
                    listHp.Add(Mathf.FloorToInt(100f * fightRole.hp / fightRole.hpMax));
                    listHp.Add(fightRole.mp);
                }
            }
            return string.Join(",", listHp.ToArray());
        }

        //角色死亡
        public void Die(int teamId, int id)
        {
            int num = 0;
            for (int i = 0; i < listAllRoles.Count; i++)
            {
                Role fightRole = listAllRoles[i];
                if (fightRole.teamId == teamId && fightRole.state != BattleState.Die)
                {
                    num++;
                }
            }
            if (num == 0)
            {
                int winner = -1;
                foreach (var item in dicTeams)
                {
                    if (item.Key != teamId)
                    {
                        winner = item.Key;
                        break;
                    }
                }

                //GameOver(winner);
            }
        }

        private void GameOver(int winner)
        {
            Debug.LogError("GameOver");
            isFight = false;
            listReport.Add(new FightReportGameOver(Time, winner, GetPlayerRoleHpLeft()));

            foreach (var item in dicBattleField)
            {
                item.Value.isEnd = true;
            }
        }

        public Role GetRole(int roleId)
        {
            for (int i = 0; i < listAllRoles.Count; i++)
            {
                if (listAllRoles[i].id == roleId)
                {
                    return listAllRoles[i];
                }
            }
            return null;
        }

        public void UseSkill(int roleId)
        {
            Role fightRole = GetRole(roleId);
            if (fightRole == null)
                return;
            fightRole.UseSkill();
        }

        public void SetAutoSkill(bool v)
        {
            for (int i = 0; i < listAllRoles.Count; i++)
            {
                if (listAllRoles[i].isPlayer == true)
                {
                    listAllRoles[i].autoUseSkill = v;
                }
            }
        }
    }
}