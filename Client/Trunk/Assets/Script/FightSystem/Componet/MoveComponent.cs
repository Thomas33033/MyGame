using FightCommom;
using System.Collections.Generic;
using UnityEngine;

namespace Fight {
    public class MoveComponent : BaseComponent
    {
        private List<Node> waypoints = new List<Node>();

        public float lastMoveTime;

        private int pathIndex;

        private Node nextNode;

        public MoveComponent(Role role) : base(role)
        {
           
        }

        public void SetWayPoints(List<Node> waypoints)
        {
            this.waypoints = waypoints;
            this.lastMoveTime = 0;
            this.pathIndex = 0;
            this.nextNode = waypoints[pathIndex];

            if (this.Owner.node == nextNode)
            {
                this.pathIndex = 1;
                this.nextNode = waypoints[pathIndex];
            }

            this.Owner.isMoving = true;
        }


        public override void OnUpdate(float nowTime)
        {
            if (!this.Owner.isMoving) return;

            if (pathIndex < this.waypoints.Count)
            {
                if (nowTime > lastMoveTime)
                {
                    this.Owner.isMoving = true;

                    if (this.Owner.battleField.CheckMove(nextNode))
                    {
                        if (this.Owner.MoveTo(nextNode))
                        {
                            this.Owner.RoleMove(nextNode);
                        }

                        pathIndex++;
                        
                        this.nextNode = this.waypoints[pathIndex];
                        Debug.Log("this.Owner.moveSpeed:" + this.Owner.moveSpeed);
                        lastMoveTime = nowTime + this.Owner.moveSpeed;
                    }
                    else
                    {
                        this.Owner.isMoving = false;
                    }
                }
            }
            else
            {
                Debug.Log("移动结束：");
                this.Owner.isMoving = false;
                this.waypoints.Clear();
                this.nextNode = null;
            }
        }

    }
}
