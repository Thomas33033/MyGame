using FightCommom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Fight
{
    public class FightRole : Role
    {
        public FightRole(int teamId, int uid, AttributeData attr, float hpInit, int mpInit, FightSkillData[] skills, string tag) : base(teamId, uid, attr, hpInit, mpInit, skills, tag)
        {
            type = RoleType.Fighter;
        }

        public override void SkillAdd(int skillId, int level)
        {
            if (StaticData.dicSkillInfo.ContainsKey(skillId) == false)
            {
                return;
            }
            FightSkillInfo skillInfo = StaticData.dicSkillInfo[skillId];

            if (skillInfo.Type != 0 && skillInfo.Type != 1 && skillInfo.Type != 2)
                return;

            FightSkill fightSkill = new FightSkill(this, skillInfo, level);

            skillComp.listSkills.Add(fightSkill);
        }

        #region Move

        protected bool MoveRandom(float nowTime)
        {
            //int dir = UnityEngine.Random.Range(0, MapGrid.directions.Count);
            //List<MapGrid> listHex = new List<MapGrid>();

            //if (teamId == battleField.teamId)
            //{
            //    if (position.q != 0)
            //    {
            //        dir = position.q > 0 ? 3 : 0;
            //    }
            //}
            //else
            //{
            //    if (position.q >= 0)
            //    {
            //        dir = 3;
            //    }
            //}

            //for (int i = 0; i < Hex.directions.Count; i++)
            //{
            //    listHex.Add(position.Add(Hex.directions[(i + dir) % Hex.directions.Count]));
            //}

            //for (int i = 0; i < listHex.Count; i++)
            //{
            //    if (MoveTo(listHex[i]))
            //    {
            //        return true;
            //    }
            //}
            return false;
        }

        protected override void Idle(float nowTime)
        {
            MoveRandom(nowTime);
        }

        private List<Node> waypoints = new List<Node>();

        protected override void MoveTarget(float nowTime)
        {
            if (waypoints != null && waypoints.Count > 0)
            {
                return;
            }
            List<Node> listHex = battleField.GetAround(target.position, range);
            listHex.Sort(SortHexDistanceHandler);
            waypoints = listHex;
            for (int i = 0; i < listHex.Count; i++)
            {
                Debug.Log("GetMoveNode: " + position.ID + " " + listHex[i].ID);

                List<Node> paths = battleField.GetMoveNode(position, listHex[i], true);

                if (paths != null && paths.Count > 0)
                {
                    waypoints = paths;
                    for (int k = 0; k < paths.Count; k++)
                    {
                        FightSceneRender.Instance.battleFieldRender.platform.SetNodeState(paths[k].ID, ENodeColor.CanBuild);
                    }

                    //MoveTo(paths.Peek());
                    break;
                }
            }
        }

        #endregion Move

        public override void Attack()
        {
            TriggerEffect(TriggerType.AttackBefore);

            skillComp.CastSkill(2);

            TriggerEffect(TriggerType.AttackAfter);
            buffComp.TriggerBuff(TriggerType.AttackAfter);
        }

        protected override bool CastSkill()
        {
            return skillComp.CastSkill(0);
        }

    }
}