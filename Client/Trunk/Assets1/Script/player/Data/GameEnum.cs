public class Currency
{
   public static int Gold = 1001;
   public static int Silver = 1002;
}



//繁殖模式
public enum _SpawnMode
{
    /// <summary>
    /// 连续
    /// </summary>
    Continous    
}

public enum _TowerType 
{
    TurretTower = 1,         //炮楼
    AOETower = 2,            //范围攻击
    DirectionalAOETower = 3, //方向性的范围攻击
    SupportTower = 4,        //辅助塔
    ResourceTower = 5,       //资源塔
    Mine = 6, 
};
public enum _TargetMode 
{ 
    Hybrid,  //混合的
    Air,     //天空
    Ground   //陆地
};   
public enum _TurretAni 
{ 
    Full, 
    YAxis, 
    None 
}

//操作枚举,用于区分每个角色的行为
public enum OperateType
{ 
    attack,   //攻击
    Harm,      //击中
    Run,      //跑步
}

public enum NpcType
{ 
    None,
    Tower,
    Monster,
}