using FightCommom;
using System.Collections.Generic;
using UnityEngine;

namespace Fight {
    public class ScanTargetComponent : BaseComponent
    {
        private Role target;

        public ScanTargetComponent(Role role) : base(role)
        {

        }

        public virtual List<Role> FindTarget(ESearchTargetType targetType, int rangeType, int range, Role rolePosition)
        {
            if (targetType == 0)
            {
                if (target != null)
                {
                    return new List<Role>() { target };
                }
            }

            List<Role> listEnemy = Owner.battleField.GetEnemy(this.Owner, rolePosition.node, targetType, rangeType, range);
            listEnemy.Sort(SortDistanceHandler);

            return listEnemy;
        }


        public  void FindTarget()
        {
            List<Role> listEnemy = Owner.battleField.GetEnemy(Owner.teamId);

            if (listEnemy.Count > 0)
            {
                listEnemy.Sort(SortDistanceHandler);

                for (int j = 0; j < listEnemy.Count; j++)
                {
                    if (listEnemy[j].StatusCheck(RoleStatus.Unselected))
                        continue;

                    if (Owner.range > 1 || listEnemy[j].node.Distance(Owner.node) <= Owner.range)
                    {
                        target = listEnemy[j];
                        break;
                    }

                    List<Node> listHex = this.Owner.battleField.GetAround(listEnemy[j].node, 2);
                    listHex.Sort(this.Owner.SortHexDistanceHandler);

                    bool flag = false;

                    for (int i = 0; i < listHex.Count; i++)
                    {
                        List<Node> paths = this.Owner.battleField.GetMoveNode(Owner.node, listHex[i], true);

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

            this.Owner.target = target;
        }


        
        private int SortDistanceHandler(Role x, Role y)
        {
            bool h1 = x.StatusCheck(RoleStatus.Hide);
            bool h2 = y.StatusCheck(RoleStatus.Hide);
            if (h1 != h2)
                return -h1.CompareTo(h2);

            float d1 = x.node.Distance(Owner.node);
            float d2 = y.node.Distance(Owner.node);
            if (d1 != d2)
            {
                return d1.CompareTo(d2);
            }

            d1 = Mathf.Abs(x.node.pos.z - Owner.node.pos.z);
            d2 = Mathf.Abs(y.node.pos.z - Owner.node.pos.z);

            return d1.CompareTo(d2);

            //if(x.position.r != y.position.r)
            //{
            //    return Mathf.Abs(x.position.q - position.q).CompareTo(Mathf.Abs(y.position.q - position.q));
            //}
        }
    }
}

