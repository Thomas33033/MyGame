using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SanJiao : MonoBehaviour
{

    public GameObject m_tmp;
    public List<Transform> m_listPos;
    public Transform m_pointPar;
    public int m_tankCnt;
    public Dictionary<int, List<Vector3>> m_dicPos = new Dictionary<int, List<Vector3>>();
    public List<Transform> m_listTank;
    // Use this for initialization
    void Start()
    {
        m_dicPos[0] = new List<Vector3>();
        for (int i = 0; i < m_pointPar.childCount; i++)
        {
            m_listPos.Add(m_pointPar.GetChild(i));


        }

        for (int i = 0; i < m_listPos.Count; i++)
        {
            m_dicPos[0].Add(m_listPos[i].transform.position);


            GameObject objPos = Instantiate(m_tmp);
            objPos.gameObject.SetActive(true);
            objPos.transform.position = m_listPos[i].transform.position;

            if (i < m_listPos.Count - 1)
            {

                Vector3 dir = m_listPos[i + 1].transform.position - m_listPos[i].transform.position;
                objPos.transform.rotation = Quaternion.LookRotation(dir.normalized);
            }

            if (i == m_listPos.Count - 1)
            {
                Vector3 dir = m_listPos[i].transform.position - m_listPos[i - 1].transform.position;
                objPos.transform.rotation = Quaternion.LookRotation(dir.normalized);
            }

            if (m_tankCnt > 1)
            {
                Sector(objPos.transform, objPos.transform.position, 60, 5, m_tankCnt - 1);
            }
        }

        CreateTank();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void CreateTank()
    {
        Transform par = null;
        Quaternion qua = Quaternion.identity;
        for (int i = 0; i < m_tankCnt; i++)
        {
            GameObject tank;
            //m_listTank.Add(tank.transform);

            if (i == 0)
            {
                tank = Instantiate(m_tmp);
                tank.transform.position = m_dicPos[i][0];
                tank.transform.GetComponent<FollowPath>().SetLeader(m_dicPos[i]);

                Vector3 dir = m_dicPos[i][1] - m_dicPos[i][0];
                tank.transform.rotation = Quaternion.LookRotation(dir.normalized); // 一开始就要确定leader 的方向
                qua = tank.transform.rotation;
                par = tank.transform;
            }
            else
            {
                GameObject follow = new GameObject();
                follow.transform.position = m_dicPos[i][0];
                follow.transform.SetParent(par, true);

                tank = Instantiate(m_tmp);
                tank.transform.position = m_dicPos[i][0];
                tank.transform.GetComponent<FollowPath>().SetFollow(follow.transform);
                tank.transform.rotation = qua;
                //tank.transform.SetParent(par, false);
            }
        }
    }


    public void Sector(Transform t, Vector3 center, float angle, float radius, int cnt)
    {
        Vector3 forward = -t.forward;

        int sign = -1;
        for (int i = 0; i < cnt; i++)
        {

            if (i % 2 == 0)
            {
                sign = -1;
            }
            else
            {
                sign = 1;
            }

            Vector3 pos = Quaternion.Euler(0f, sign * angle / 2, 0f) * forward * radius * (1 + i / 2) + center;

            //GameObject obj = Instantiate(m_tmp);
            //obj.transform.position = pos;

            if (m_dicPos.ContainsKey(i + 1) == false)
            {
                m_dicPos[1 + i] = new List<Vector3>();
            }
            m_dicPos[1 + i].Add(pos);
        }



    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;

        for (int i = 0; i < m_pointPar.transform.childCount; i++)
        {
            Vector3 position = m_pointPar.transform.GetChild(i).position;
            if (i > 0)
            {
                Vector3 previous = m_pointPar.transform.GetChild(i - 1).position;
                Gizmos.DrawLine(previous, position);
                Gizmos.DrawWireSphere(position, 0.3f);
            }
        }

    }

}

