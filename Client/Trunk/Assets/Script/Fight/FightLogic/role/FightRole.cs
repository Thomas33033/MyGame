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

        private bool bIdle = false;
        private float idleThinkEndTimes;

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

        protected override void FindTarget()
        {
            this.scanTargetComp.FindTarget();
        }

        protected override void Idle(float nowTime)
        {
            if (this.isMoving)
                return;

            if (bIdle == false)
            {
                bIdle = true;
                idleThinkEndTimes = nowTime + RandomTools.Range(2, 6);
                return;
            }

            if (nowTime > idleThinkEndTimes)
            {
                bIdle = false;
                MoveRandom(nowTime);
            }
        }
        
        public override void MoveTarget(float nowTime)
        {
            bIdle = false;

            if (this.isMoving) return;
  
            List<Node> listHex = null;
            listHex = battleField.GetAround(target.node, range, target.nodeSize);
            listHex.Sort(SortHexDistanceHandler);

            for (int i = 0; i < listHex.Count; i++)
            {
                List<Node> paths = battleField.GetMoveNode(node, listHex[i], true);

                if (paths != null && paths.Count > 0)
                {
                    this.moveComponent.SetWayPoints(paths);

                    for (int k = 0; k < paths.Count; k++)
                    {
                         FightSceneRender.Instance.battleFieldRender.platform.SetNodeState(paths[k].Id, ENodeColor.CanBuild);
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

        protected void MoveRandom(float nowTime)
        {
            Node brithNode = this.battleField.dicNodeGraph[this.birthNodeId];
            List<Node> nodeList = this.battleField.GetAround(brithNode, this.nodeSize, 5);

            List<Node> paths = null;
            Node tNode;
            int index = 0;
            while (paths == null && index++ < 50)
            {
                tNode = nodeList[RandomTools.Range(0, nodeList.Count)];
                paths = battleField.GetMoveNode(this.node, tNode, true);

                if (paths != null && paths.Count > 2)
                {
                    this.moveComponent.SetWayPoints(paths);
                }
            }

        }

    }
}