using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterData
{
    public uint uid;
    public EEntityType type;
    public uint baseId;
    public byte level;
    public float hp;
    public float maxHp;
    /**移动速率*/
    public float speedFactor;
    /**基础移动速度*/
    public float baseSpeed;
    /**技能配置*/
    public CfgSkillData skillData;

    public Vector3 position = Vector3.zero;
    public Quaternion rotation = new Quaternion(0, 0, 0, 0);
    public ResItem dropRes = new ResItem(Currency.Gold,10);  //死亡掉落的资源
}

/// <summary>
/// 服务器怪物数据
/// </summary>
public class s_MonsterData : CharacterData
{
    public bool flying;
    public float flightYOffset;
    public uint itemId;
    public uint itemNum;
    public uint deadSpawnId;
    public uint deadSpawnNum;

    public CfgNpcData config;
    public CfgNpcAttrData attrConfig;

    public void InitData(uint configId)
    {
        this.type = EEntityType.Monster;
        this.config = ConfigManager.Instance.GetData<CfgNpcData>((int)configId);
        this.attrConfig = ConfigManager.Instance.GetData<CfgNpcAttrData>(this.config.Statstype*1000+this.config.Level);
        this.baseId = (uint)config.UId;
        this.level = (byte)config.Level;
        this.hp = config.Hp;
        this.maxHp = config.Hp;
        this.flightYOffset = attrConfig.Flyheight;
        this.baseSpeed = attrConfig.Movespeed;
        this.flying = this.flightYOffset == 0 ? true : false;
        string[] array = this.config.DieDrop.Split('-');
        if (array.Length > 1)
        {
            this.deadSpawnId = uint.Parse(array[0]);
            this.deadSpawnNum = uint.Parse(array[1]);
        }
   }
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

public class s_TowerData : CharacterData
{
    public _TowerType TowerType = _TowerType.AOETower;
    public TowerStat baseStat;
    public float resOutputNum;  //资源产量
    public float itemId;        //资源ID
    public float costItem;      //建造需要的资源ID
    public ResItem outputRes;   //生产的资源
    public ResItem costRes;     //建造话费的资源

    public CfgNpcData config;
    public CfgNpcAttrData attrConfig;

    
    public void InitData(uint configId)
    {
        this.type = EEntityType.Tower;
        this.config = ConfigManager.Instance.GetData<CfgNpcData>((int)configId);
        this.attrConfig = ConfigManager.Instance.GetData<CfgNpcAttrData>(this.config.Statstype*1000+this.config.Level);

       
        this.baseId = (uint)config.UId;
        this.level = (byte)config.Level;
        this.hp = config.Hp;

        int curSkillId = this.config.SkillId * 1000 + this.level;
        this.skillData = ConfigManager.Instance.GetData<CfgSkillData>(curSkillId);

        this.baseStat = new TowerStat();
        this.baseStat.cooldown = 10;//config.Cooldown;

        this.baseStat.reloadDuration = 10;//config.BuildTime;
        this.baseStat.range = 10;//config.Range;
        this.baseStat.minRange = 5;//config.Range;
        this.baseStat.mineOneOff = false;
        this.baseStat.currentClip = 1;
        this.baseStat.lastReloadTime = 0;
        this.baseStat.projectingArc = 10;//config.Arc;

        //outputRes = new ResItem(config.ResOutput);
        costRes = new ResItem(this.config.Buildcost);
    }
}


[System.Serializable]
public class TowerStat
{
    public int cost = 10;
    public int[] costs = new int[1];
    public float range;
    public float minRange;
    public float cooldown = 1;
    public float reloadDuration = 1;
    public bool mineOneOff;
    public int currentClip = 1;
    public float lastReloadTime;
    public float projectingArc = 10;

    public BuffStat buff;
    public int[] incomes = new int[1];
    public float buildDuration = 1;

    public Transform turretObject;
    public Transform baseObject;

    public TowerStat Clone()
    {
        TowerStat clone = new TowerStat();
        clone.cost = cost;
        //clone.costs=costs;

        clone.costs = new int[costs.Length];
        for (int i = 0; i < costs.Length; i++)
        {
            clone.costs[i] = costs[i];
        }
        clone.cooldown = cooldown;
        clone.reloadDuration = reloadDuration;
        //clone.incomes=incomes;
        clone.incomes = new int[incomes.Length];
        for (int i = 0; i < incomes.Length; i++)
        {
            clone.incomes[i] = incomes[i];
        }

        clone.buildDuration = buildDuration;

        return clone;
    }

}

//点伤害
[System.Serializable]
public class Dot
{
    public float damage = 0;
    public float duration = 0;
    public float interval = 0;

    public Dot() { }
    public Dot(string str)
    {
        string[] array = str.Split('-');
        if (array.Length == 3)
        {
            damage = float.Parse(array[0]);
            interval = float.Parse(array[1]);
            duration = float.Parse(array[2]);
        }
       
    }

    public Dot Clone()
    {
        Dot clone = new Dot();
        clone.damage = damage;
        clone.duration = duration;
        clone.interval = interval;

        return clone;
    }
}

[System.Serializable]
public class Slow
{
    public float duration = 0;
    public float slowFactor = 0;
    private float timeEnd;
    public Slow() { }

    public Slow(string str)
    {
        string[] array = str.Split('-');
        if (array.Length == 2)
        {
            slowFactor = float.Parse(array[0]);
            duration = float.Parse(array[1]);
        }
        
    }

    public float GetTimeEnd()
    {
        return timeEnd;
    }
    public void SetTimeEnd(float val)
    {
        timeEnd = val;
    }

    public Slow Clone()
    {
        Slow clone = new Slow();
        clone.slowFactor = slowFactor;
        clone.duration = duration;
        clone.timeEnd = timeEnd;

        return clone;
    }
}

[System.Serializable]
public class BuffStat
{
    //buff doesnt stack, higher level override lowerlevel buff
    [HideInInspector]
    public int buffID = 0;
    public float damageBuff = 0.1f;
    public float cooldownBuff = 0.1f;
    public float rangeBuff = 0.1f;

    public BuffStat Clone()
    {
        BuffStat clone = new BuffStat();
        clone.buffID = buffID;
        clone.damageBuff = damageBuff;
        clone.cooldownBuff = cooldownBuff;
        clone.rangeBuff = rangeBuff;

        return clone;
    }
}

public class Buffed
{
    //	private List<BuffStat> buffList=new List<BuffList>();
    //	
    //	
    //	public void AddBuff(int ID, BuffStat){
    //		
    //	}
}

public class OccupiedPlatform
{
    public Platform platform;
    public Node node;

    public OccupiedPlatform(Platform p, Node n)
    {
        platform = p;
        node = n;
    }
}
