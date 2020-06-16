using UnityEngine;
using System.Collections.Generic;
using Fight;

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

    public Vector3 GetPosition(MapGrid grid)
    {
        //for (int i = 0; i < rotateNum; i++)
        //{
        //    hex = hex.RotateRight();
        //}

        //PointHex p = _layoutHex.HexToPixel(hex);
        return new Vector3((float)grid.x, 0, (float)grid.y);
    }

    // zz < zc
    // zf > zc
    // zz > zf

    public Vector3 GetPosition(int[] arr)
    {
        return GetPosition(new MapGrid(arr[0], arr[1], arr[2]));
    }

    public Vector3 GetWorldPosition(MapGrid grid)
    {
        //for (int i = 0; i < rotateNum; i++)
        //{
        //    hex = hex.RotateRight();
        //}

        //PointHex p = _layoutHex.HexToPixel(hex);
        return transform.position + new Vector3((float)grid.x, 0, (float)grid.y);
    }

    public void FightOnBattlefield(FightRoleRender roleRender, MapGrid position, bool isMain)
    {
        roleRender.battlefield = this;
        dic.Add(roleRender.roleId, roleRender);

        roleRender.transform.localPosition = GetPosition(position);
        roleRender.gameObject.SetActive(true);
    }

    public void RoleMove(string roleId, float endTime, MapGrid position)
    {
        if (dic.ContainsKey(roleId) == false)
            return;

        Vector3 v3 = GetPosition(position);
        dic[roleId].Move(v3, endTime - FightSence.Instance.GetTime());
    }

    public void RoleJump(string roleId, float endTime, MapGrid grid)
    {
        if (dic.ContainsKey(roleId) == false)
            return;

        Vector3 v3 = GetPosition(grid);
        dic[roleId].Jump(v3, endTime - FightSence.Instance.GetTime());
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