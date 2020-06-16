using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Fight
{ 
}
    /// <summary>
    /// 攻击模式
    /// </summary>
    public enum EAttackMode
    {
        Peace = 1,    //和平  
        Country = 2,  //国家
        Camp = 3,     //阵营
        Team = 4,     //队伍模式
    }

    /// <summary>
    /// 攻击搜索目标类型
    /// </summary>
    public enum ESearchTargetType
    { 
        OnlyNpc = 1,
        OnlyPlayer,
        OnlyTower,
        OnlyMonster,
        NpcFirst,
        TowerFirst,
        MonsterFirst,

    }

    /// <summary>
    /// 游戏实体类型
    /// </summary>
    public enum EEntityType
    { 
        None = 0,
        Npc = 1,
        Tower = 2,
        Player = 3,
        Monster = 4,
    }

