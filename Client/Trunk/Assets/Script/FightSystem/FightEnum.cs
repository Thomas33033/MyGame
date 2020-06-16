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
        Fighter = 0,
        //召唤物
        Summon = 10,
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
    public enum FightDamageType
    {
        Physical = 1,
        Magic = 2,
        Hp = 3,
        Mp = 4,
    }
}