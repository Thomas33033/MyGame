using FightCommom;
using System.Collections.Generic;
using UnityEngine;

namespace Fight {
    public class MoveComponent : BaseComponent
    {
        private List<Node> waypoints = new List<Node>();

        public float lastMoveTime; 

        public MoveComponent(Role role) : base(role)
        {
           
        }

        public void SetWayPoints(List<Node> waypoints)
        {
            this.waypoints = waypoints;
            this.lastMoveTime = 0;
            this.Owner.isMoving = true;
        }


        public override void OnUpdate(float nowTime)
        {
            if (!this.Owner.isMoving) return;

            if (this.waypoints.Count > 0)
            {
                if (nowTime > lastMoveTime)
                {
                    this.Owner.isMoving = true;
                    this.Owner.MoveTo(this.waypoints[0]);
                    this.waypoints.RemoveAt(0);
                    Debug.Log("移动中：" + this.waypoints.Count);
                    Debug.LogError("noveTime:" + this.Owner.moveSpeed);
                    lastMoveTime = nowTime + this.Owner.moveSpeed;
                   
                }
            }
            else
            {
                Debug.Log("移动结束：");
                this.Owner.isMoving = false;
            }
        }

    }
}
