//战斗系统枚举
namespace Fight
{

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