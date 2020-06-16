using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Fight
{

    public class StatusComponent
    {
        protected Dictionary<int, int> dicStatus;

        public StatusComponent()
        {
            this.dicStatus = new Dictionary<int, int>();
            for (int i = 0; i < 10; i++)
            {
                dicStatus.Add(i + 1, 0);
            }
        }

        public bool StatusCheck(int status)
        {
            if (dicStatus.ContainsKey(status) == false)
                return false;

            if (status == (int)RoleStatus.Dizz || status == (int)RoleStatus.Silent)
            {
                if (StatusCheck(RoleStatus.ImmuneControl))
                    return false;
            }

            return dicStatus[status] > 0;
        }

        public bool StatusCheck(RoleStatus status)
        {
            return StatusCheck((int)status);
        }

        public void StatusChange(RoleStatus status, int t)
        {
            StatusChange((int)status, t);
        }

        public void StatusChange(int status, int t)
        {
            if (dicStatus.ContainsKey(status) == false)
            {
                dicStatus[status] = 0;
            }
            dicStatus[status] += t;
        }
    }
}