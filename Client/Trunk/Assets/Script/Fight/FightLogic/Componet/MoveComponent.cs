using FightCommom;
using System.Collections.Generic;

/**
 * 客户端发送方向给服务器，同时客户端模型
 * 
**/
namespace Fight {
    public class MoveComponent : BaseComponent
    {
        private List<Node> waypoints = new List<Node>();

        public float moveEndTime;

        private int pathIndex;

        private Node nextNode;

        public MoveComponent(Role role) : base(role)
        {
           
        }

        public void MoveByDir(Node node)
        {
            if (node == null)
            {
                return;
            }

            this.waypoints.Clear();
            this.waypoints.Add(node);
            this.SetWayPoints(this.waypoints);
        }

        public void SetWayPoints(List<Node> waypoints)
        {
            this.waypoints = waypoints;
            this.moveEndTime = 0;
            this.pathIndex = -1;
            this.nextNode = this.waypoints[0];
            if (this.Owner.node == nextNode && waypoints.Count > 1)
            {
                this.pathIndex = 0;
                this.nextNode = this.waypoints[1];
            }
            this.Owner.isMoving = true;
        }


        public override void OnUpdate(float nowTime)
        {
            if (!this.Owner.isMoving) return;

            if (pathIndex < this.waypoints.Count)
            {
                if (nowTime > moveEndTime)
                {
                    this.Owner.isMoving = true;

                    pathIndex++;
                    if (pathIndex < this.waypoints.Count)
                    {
                        this.nextNode = this.waypoints[pathIndex];
                    }

                    if (this.Owner.battleField.CheckMove(nextNode))
                    {
                        if (this.Owner.MoveTo(nextNode))
                        {
                            this.Owner.RoleMove(nextNode);
                        }
                        moveEndTime = nowTime + this.Owner.moveSpeed;
                    }
                    else
                    {
                        this.Owner.isMoving = false;
                    }
                }
            }
            else
            {
                this.Owner.isMoving = false;
                this.waypoints.Clear();
                this.nextNode = null;
                DebugMgr.Log("------this.nextNode = null-----");
            }

        }


        public void StopMove()
        {
            this.Owner.isMoving = false;
            this.waypoints.Clear();

            //停止移动
            if (this.moveEndTime - this.Owner.Time > this.Owner.moveSpeed / 2)
            {
                this.Owner.StopMove(this.nextNode);
            }
            else
            {
                this.Owner.StopMove(this.Owner.node);
            }
            this.nextNode = null;
        }
    }
}
