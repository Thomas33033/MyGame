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
        public FightRole(int teamId, AttributeData attr, float hpInit, int mpInit, FightSkillData[] skills, string tag) : base(teamId, attr, hpInit, mpInit, skills, tag)
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

        public override void Update(float nowTime)
        {
            base.Update(nowTime);
        }


        protected bool MoveRandom(float nowTime)
        {
            //int dir = RandomTools.Range(0, MapGrid.directions.Count);
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
        
        public override void MoveTarget(float nowTime)
        {
            if (this.isMoving) return;

            List<Node> listHex = null;
            listHex = battleField.GetAround(target.node,  range, target.nodeSize);
            listHex.Sort(SortHexDistanceHandler);
     
            for (int i = 0; i < listHex.Count; i++)
            {
                List<Node> paths = battleField.GetMoveNode(node, listHex[i], true);

                if (paths != null && paths.Count > 0)
                {
                    this.moveComponent.SetWayPoints(paths);

                    for (int k = 0; k < paths.Count; k++)
                    {
                        FightSceneRender.Instance.battleFieldRender.platform.SetNodeState(paths[k].ID, ENodeColor.CanBuild);
                    }
                    break;
                }
            }
        }

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