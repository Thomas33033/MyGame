using Fight;
public class FightReportRoleHurt : FightReport
{
    public int roleId;
    public int hp;
    public int hurt;
    public int mp;
    public int source;
    public int damageType;
    public bool isCrit;
    public bool isHit;

    public FightReportRoleHurt() { }

    public FightReportRoleHurt(float time, int playerId, int roleId, int hp, int hurt, bool isCrit, int mp, int source, int damageType, bool isHit = true) : base(playerId, ReportType.RoleHurt.ToString(), time)
    {
        this.roleId = roleId;
        this.hp = hp;
        this.hurt = hurt;
        this.isCrit = isCrit;
        this.mp = mp;
        this.source = source;
        this.damageType = damageType;
        this.isHit = isHit;
    }
}