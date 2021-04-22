//战斗系统枚举
namespace Fight
{
    public enum FightType{
        ConmmFight = 1,
        FightReport = 2
    }

    public enum RoleStatus
    {
        /// <summary>
        /// 眩晕
        /// </summary>
        Dizz = 1,

        /// <summary>
        /// 沉默
        /// </summary>
        Silent = 2,

        /// <summary>
        /// 不死
        /// </summary>
        Undead = 3,

        /// <summary>
        /// 免疫负面状态
        /// </summary>
        ImmuneDebuff = 4,

        /// <summary>
        /// 无法选中
        /// </summary>
        Unselected = 5,

        /// <summary>
        /// 隐藏
        /// </summary>
        Hide = 6,

        /// <summary>
        /// 免疫控制 免疫1，2
        /// </summary>
        ImmuneControl = 7,
    }

    public enum DamageSourceType
    {
        Attack = 1,
        Cannon = 2,
        Skill = 3,

        /// <summary>
        /// 护盾减伤
        /// </summary>
        Shield = 4, 
        Share = 5,

        Buff = 6,
        Effect = 7,
        Aura = 8,
        Reflex = 9,
        Suck = 10,
        ShipEvent = 11,
    }

    //角色类型
    public enum RoleType
    {
        //战士
        Fighter = 1,
        //建造塔
        BuildTower = 2,
        //普通建筑
        Buildings = 3,
        //英雄
        MainPlayer = 4,
        //召唤物
        Summon = 10,
        //队伍
        Team = 20

    }

    //角色状态类型
    public enum RoleStateType
    {
        SkillCrit = 1,
    }

    //战斗状态
    public enum BattleState
    {
        Fight,
        Die,
    }

    /// <summary>
    /// 战斗伤害类型
    /// </summary>
    public enum EDamageType
    {
        Physical = 1,
        Magic = 2,
        Hp = 3,
        Mp = 4,
    }


    /// <summary>
    /// 1敌人 2队友 3自己 4己方
    /// </summary>
    public enum ESearchTargetType
    {
        Enemy = 1,
        Teammate = 2,
        Self = 3,
        OtherTeam = 4,
    }

}