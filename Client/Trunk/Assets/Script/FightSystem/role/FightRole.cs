using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fight
{
    public class FightRole : Role
    {
        public FightRole(int teamId, int uid, AttributeData attr, float hpInit, int mpInit, FightSkillData[] skills, string tag) : base(teamId, uid, attr, hpInit, mpInit, skills, tag)
        {
            type = RoleType.Fighter;
        }

        protected override void FindTarget()
        {
            List<Role> listEnemy = battleField.GetEnemy(teamId);

            if (listEnemy.Count > 0)
            {
                listEnemy.Sort(SortDistanceHandler);

                for (int j = 0; j < listEnemy.Count; j++)
                {
                    if (listEnemy[j].StatusCheck(RoleStatus.Unselected))
                        continue;

                    if (range > 1 || listEnemy[j].position.Subtract(position).Length() <= range)
                    {
                        target = listEnemy[j];
                        break;
                    }

                    List<MapGrid> listHex = battleField.GetAround(listEnemy[j].position);
                    listHex.Sort(SortHexDistanceHandler);

                    bool flag = false;

                    for (int i = 0; i < listHex.Count; i++)
                    {
                        Stack<MapGrid> paths = battleField.GetMoveHex(position, listHex[i], true);

                        if (paths.Count > 0)
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (flag)
                    {
                        target = listEnemy[j];
                        break;
                    }
                }
            }
        }

        protected int SortDistanceHandler(Role x, Role y)
        {
            bool h1 = x.StatusCheck(RoleStatus.Hide);
            bool h2 = y.StatusCheck(RoleStatus.Hide);

            if (h1 != h2)
                return h1.CompareTo(h2);

            int a1 = position.Distance(x.position);
            int a2 = position.Distance(y.position);

            if (a1 != a2)
            {
                return a1.CompareTo(a2);
            }

            int d1 = Math.Abs(x.position.q - position.q);
            int d2 = Math.Abs(y.position.q - position.q);

            return d1.CompareTo(d2);
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

        protected override void MoveTarget(float nowTime)
        {
            //List<Hex> listHex = battleField.GetAround(target.position, range);
            //listHex.Sort(SortHexDistanceHandler);

            ////bool flag = false;

            //for (int i = 0; i < listHex.Count; i++)
            //{
            //    Stack<Hex> paths = battleField.GetMoveHex(position, listHex[i], true);

            //    if (paths.Count > 0)
            //    {
            //        MoveTo(paths.Peek());
            //        //flag = true;
            //        break;
            //    }
            //}
        }

        protected int SortHexDistanceHandler(MapGrid x, MapGrid y)
        {
            int a1 = position.Distance(x);
            int a2 = position.Distance(y);
            return a1.CompareTo(a2);
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