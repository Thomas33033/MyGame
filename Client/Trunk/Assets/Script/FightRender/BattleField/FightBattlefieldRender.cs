using UnityEngine;
using System.Collections.Generic;
using Fight;
using FightCommom;

/// <summary>
/// 客户端战场区域 渲染逻辑
/// </summary>
public class FightBattlefieldRender : MonoBehaviour
{
    public int id;

    public Transform rootBack;

    public Transform rootRole;

    public float sizeX = 1.5f;

    public float sizeY = 1.2f;

    /// <summary>
    /// 行
    /// </summary>
    public int col;

    /// <summary>
    /// 列
    /// </summary>
    public int row;

    private Dictionary<string, FightRoleRender> dic;

    public int rotateNum;

    public GameObject rootObj;

    private void Awake()
    {
        //_layoutHex = new LayoutHex(LayoutHex.flat, new PointHex(sizeY, sizeX), new PointHex());
        //dic = new Dictionary<string, FightSCRoleFighter>();
    }

    private void Start()
    {
        //for (int j = -2; j < 2; j++)
        //{
        //    for (int i = -4; i < 4; i++)
        //    {
        //        DoubledCoord coord = new DoubledCoord(i, j);
        //        Hex hex = coord.RdoubledToCube2();

        //        GameObject obj = LoadTools.LoadGameObject("Fight3D", "NPCCarryBall", rootRole.gameObject);
        //        obj.name = hex.ToString() + " (" + i + "," + j + ")";
        //        obj.transform.localPosition = GetPosition(hex);
        //    }
        //}
    }

    private void Update()
    {
    }


    // zz < zc
    // zf > zc
    // zz > zf

    //public Vector3 GetPosition(int[] arr)
    //{
    //    return GetPosition(new MapGrid(arr[0], arr[1], arr[2]));
    //}

    public Vector3 GetWorldPosition(Node grid)
    {
        return transform.position + grid.pos;
    }

    public void FightOnBattlefield(FightRoleRender roleRender, Node node, bool isMain)
    {
        roleRender.battlefield = this;
        dic.Add(roleRender.roleId, roleRender);

        roleRender.transform.localPosition = node.pos;
        roleRender.gameObject.SetActive(true);
    }

    public void RoleMove(string roleId, float endTime, Node node)
    {
        if (dic.ContainsKey(roleId) == false)
            return;

        Vector3 v3 = node.pos;
        dic[roleId].Move(v3, endTime - FightSceneRender.Instance.GetTime());
    }

    public void RoleJump(string roleId, float endTime, Node node)
    {
        if (dic.ContainsKey(roleId) == false)
            return;

        Vector3 v3 = node.pos;
        dic[roleId].Jump(v3, endTime - FightSceneRender.Instance.GetTime());
    }

    public List<RoleRender> GetRols(string teamId)
    {
        List<RoleRender> list = new List<RoleRender>();

        foreach (var item in dic)
        {
            if (item.Value.teamId == teamId)
            {
                list.Add(item.Value);
            }
        }

        return list;
    }

    public void RoleDie(string roleId)
    {
        if (dic.ContainsKey(roleId))
        {
            dic[roleId].Die();
            dic.Remove(roleId);
        }
    }

}