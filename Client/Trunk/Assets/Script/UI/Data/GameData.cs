using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FightCommom;



public enum ETeamType
{ 
    Friend = 1,
    Enmery = 2,
}

public enum _TowerType
{
    TurretTower,
    AOETower,
    DirectionalAOETower,
    SupportTower,
    ResourceTower,
    Mine,
}


public enum EItemType { 
    Silver = 1,
    Gold = 2,
}

public class ResItem
{
    public int itemId = 0;
    public int num = 0;

    public ResItem(int p_itemId, int p_num)
    {
        this.itemId = p_itemId;
        this.num = p_num;
    }

    public ResItem(string str)
    {
        string[] array = str.Split('-');
        if (array.Length == 2)
        {
            itemId = int.Parse(array[0]);
            num = int.Parse(array[1]);
        }
    }
}

public class NpcData
{
    public ResItem costRes;     //建造话费的资源
    public CfgNpcData config;
    public CfgNpcAttrData attrConfig;
    public ETeamType teamId;

    public void InitData(int configId, ETeamType teamId)
    {
        this.config = ConfigManager.Instance.GetData<CfgNpcData>(configId);
        this.attrConfig = ConfigManager.Instance.GetData<CfgNpcAttrData>(this.config.AttrId + this.config.Level - 1);
        costRes = new ResItem(this.config.BuildCost);
        this.teamId = teamId;
    }
}

