using FightCommom;
using System.Collections.Generic;
using UnityEngine;

namespace Fight {
    public class ScanTargetComponent : BaseComponent
    {
        //攻击目标
        public Role target;

        public ScanTargetComponent(Role role) : base(role)
        {

        }

        public virtual List<Role> FindTarget(int targetType, int rangeType, int range, Role rolePosition)
        {
            if (targetType == 0)
            {
                if (target != null)
                {
                    return new List<Role>() { target };
                }
            }

            List<Role> listEnemy = Owner.battleField.GetEnemy(this.Owner, rolePosition.position, targetType, rangeType, range);
            listEnemy.Sort(SortFindEnemy);

            return listEnemy;
        }

        public Node position;
        private int SortFindEnemy(Role x, Role y)
        {
            bool h1 = x.StatusCheck(RoleStatus.Hide);
            bool h2 = y.StatusCheck(RoleStatus.Hide);
            if (h1 != h2)
                return -h1.CompareTo(h2);

            float d1 = x.position.Distance(position);
            float d2 = y.position.Distance(position);
            if (d1 != d2)
            {
                return d1.CompareTo(d2);
            }

            d1 = Mathf.Abs(x.position.pos.z - position.pos.z);
            d2 = Mathf.Abs(y.position.pos.z - position.pos.z);

            return d1.CompareTo(d2);

            //if(x.position.r != y.position.r)
            //{
            //    return Mathf.Abs(x.position.q - position.q).CompareTo(Mathf.Abs(y.position.q - position.q));
            //}
        }
    }
}

