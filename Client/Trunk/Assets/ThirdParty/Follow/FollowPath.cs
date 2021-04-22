using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPath : MonoBehaviour
{

    public int CurrentWayPointID = 0;
    public float Speed;//移动速度
    public float reachDistance = 0.01f;//里路径点的最大范围

    public List<Vector3> m_listPos = new List<Vector3>();
    public bool m_isCanFollow = false;

    public bool m_isLeader = false;

    public Transform m_followTrans;
    public Transform m_leaderTrans;
    void Start()
    {

    }

    public void SetLeader(List<Vector3> listPos)
    {
        m_isLeader = true;
        m_listPos = listPos;
        m_isCanFollow = true;
        CurrentWayPointID = 0;

    }

    public void SetFollow(Transform trans)
    {
        m_isLeader = false;
        m_followTrans = trans;

        m_isCanFollow = true;
        CurrentWayPointID = 0;

    }

    void Update()
    {
        if (!m_isCanFollow)
            return;

        if (m_isLeader == true)
        {
            if (CurrentWayPointID >= m_listPos.Count)
            {
                return;
            }
            float distance = Vector3.Distance(m_listPos[CurrentWayPointID], transform.position);
            transform.position = Vector3.MoveTowards(transform.position, m_listPos[CurrentWayPointID], Time.deltaTime * Speed);

            Vector3 LookDirection = m_listPos[CurrentWayPointID] - transform.position;
            Quaternion targetlook = Quaternion.LookRotation(LookDirection);

            if (distance <= reachDistance)
            {
                CurrentWayPointID++;
                if (CurrentWayPointID < m_listPos.Count)
                {
                    playerRotate(m_listPos[CurrentWayPointID], gameObject/*transform.Find("Tank").gameObject*/);
                }

            }
        }

        else if (m_isLeader == false)
        {
            float distance = Vector3.Distance(m_followTrans.position, transform.position);

            if (distance > 3)
            {
                transform.position = Vector3.MoveTowards(transform.position, m_followTrans.position, Time.deltaTime * Speed * 1.25f);
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, m_followTrans.position, Time.deltaTime * Speed);
            }

            //Vector3 LookDirection = m_listPos[CurrentWayPointID] - transform.position;
            //Quaternion targetlook = Quaternion.LookRotation(LookDirection);
            //playerRotate(m_followTrans.position, gameObject/*transform.Find("Tank").gameObject*/);
            transform.LookAt(m_followTrans);
        }
    }


    void playerRotate(Vector3 target, GameObject rot)
    {
        //向量的叉乘
        if (Vector3.Cross(rot.transform.forward, target - this.transform.position).y > 0)
        {
            //计算角度
            rot.transform.Rotate(Vector3.up * Vector3.Angle(rot.transform.forward, target - this.transform.position));
        }
        else
        {
            rot.transform.Rotate(Vector3.down * Vector3.Angle(rot.transform.forward, target - this.transform.position));
        }
    }

}

