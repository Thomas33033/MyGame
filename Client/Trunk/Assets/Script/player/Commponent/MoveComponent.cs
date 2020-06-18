using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 处理实体移动
/// </summary>
public class MoveComponent : ComponentBase
{           
    public GamePath path;
    public List<Vector3> wayPoint = new List<Vector3>();
    public int wpCounter = 1;
    public PathSection curPathSection;

    private float rotateSpeed = 1;
    private Vector3 dynamicOffset;
    public int subWPCounter = 0;
    public int currentPathID = 0;
    public List<Vector3> subPath = new List<Vector3>();

    private Vector3 dir;
    private int waveID;

    private bool mArrivedAtTarget = false;
    private Vector3 mTargetPosition;

    public override void OnInit(CharacterBase p_owner)
    {
        base.OnInit(p_owner);
        
    }

    public override void OnUpdate(float dt)
    {
        base.OnUpdate(dt);
       
    }

    //移动到目标位置
    public void MoveToTarget(Vector3 p_pos)
    {
        mTargetPosition = p_pos;
    }

    //巡逻
    public void StartPatrol()
    {
        if (this.Owner.CanMove())
        {
            if (this.wayPoint.Count > 0)
            {
                this.MoveToNext();
            }

            this.MovePathMode();
        }
    }


    void MoveToNext()
    {
        if (wpCounter < wayPoint.Count)
        {
            if (MoveToPoint(wayPoint[wpCounter]))
            {
                wpCounter += 1;
            }
        }
        else ReachDestination();
    }

    private Vector3 position;
    private Vector3 targetPos = new Vector3(999,999,999);
    bool MoveToPoint(Vector3 point)
    {
        this.position = this.Owner.ClientPos;
        if (this.targetPos != point)
        {
            this.dir = (point - this.position + this.dynamicOffset).normalized;
            this.targetPos = point;
        }
       
        if (this.Owner.FlyHeight() > 0) point += new Vector3(0, this.Owner.FlyHeight(), 0);
        float dist = Vector3.Distance(point, this.position);
        if (dist < 1f)
        {
            return true;
        }
        Quaternion wantedRot = Quaternion.LookRotation(point - this.position);
        var trans = this.Owner.Trans;
        
        if (trans != null)
        {
           this.Owner.Rotation =  Quaternion.Slerp(this.Owner.Rotation, wantedRot, rotateSpeed * Time.deltaTime);   
            this.Owner.ClientPos += this.dir * this.Owner.MoveSpeed * Time.deltaTime;

            if(this.Owner.behaviourMgr.CanChangeState(EBehaviourState.Run))
            {
                this.Owner.behaviourMgr.ChangeState<BehaviourState_Run>();
            }
        }
        
        return false;
    }

    /**抵达终点*/
    void ReachDestination()
    {
        this.mArrivedAtTarget = true;
    }

    /// <summary>
    /// 是否抵目标位置
    /// </summary>
    /// <returns></returns>
    public bool ArrivedAtTarget()
    {
        return this.mArrivedAtTarget;
    }



    //--------------------------------------------------------
    public void SetPath(GamePath p)
    {
        this.path = p;
        if (p.dynamicWP > 0)
        {
            float allowance = path.dynamicWP;
            dynamicOffset = new Vector3(Random.Range(-allowance, allowance), 0, Random.Range(-allowance, allowance));
            this.Owner.ClientPos += dynamicOffset;
        }
        else
        {
            dynamicOffset = Vector3.zero;
        }
    }

    public void SetMovePath(List<Vector3> waypoints, int wID)
    {
        waveID = wID;

        wayPoint = waypoints;

        if (this.Owner.FlyHeight() > 0)
            this.Owner.ClientPos += new Vector3(0, this.Owner.FlyHeight(), 0);

        BehaviourComponent bhmgr = this.Owner.GetComponent<BehaviourComponent>();

        if (bhmgr != null)
        {
            bhmgr.SetAnimationSpeed(this.Owner.SpeedFactor());
        }

        float allowance = BuildManager.GetGridSize();
        dynamicOffset = new Vector3(Random.Range(-allowance, allowance), 0, Random.Range(-allowance, allowance));
        this.Owner.ClientPos += dynamicOffset;
    }

    private void GetSubPath()
    {
        subPath = curPathSection.GetSectionPath();
        currentPathID = curPathSection.GetPathID();
        subWPCounter = 0;
    }

    public void SetSubPath(List<Vector3> wp)
    {
        this.wayPoint = wp;
        subWPCounter = 0;
    }

    private void SearchSubPath()
    {
        currentPathID = curPathSection.GetPathID();

        Vector3 pos = this.Owner.ClientPos;

        this.SetSubPath(PathFinder.GetPath(pos, subPath[subPath.Count - 1], curPathSection.platform.GetNodeGraph()));

    }

    void MovePathMode()
    {
        if (curPathSection == null)
        {
            if (path == null) return;
            List<PathSection> PSList = path.GetPath();
            if (wpCounter < PSList.Count)
            {
                curPathSection = PSList[wpCounter];

                GetSubPath();
            }
            else ReachDestination();

        }

        if (curPathSection != null)
        {

            if (currentPathID != curPathSection.GetPathID())
            {
                SearchSubPath();
            }

            if (MoveToPoint(subPath[subWPCounter]))
            {
                if ( subWPCounter == 0) subWPCounter = subPath.Count + 1;

                else subWPCounter += 1;

                if (subWPCounter >= subPath.Count)
                {
                    wpCounter += 1;
                    curPathSection = null;
                }
            }

        }

    }



}
