using Fight;
public class FightReportRoleAttack : FightReport
{
    public int roleId;
    public int targetId;
    public int damageId;
    public float timeExecute;
    public int mp;
    public float attackCd;
    public bool isHit;

    public FightReportRoleAttack() { }

    public FightReportRoleAttack(float time, int playerId, int roleId, int targetId, int damageId, float timeExecute, int mp, float attackCd, bool isHit) : base(playerId, ReportType.RoleAttack.ToString(), time)
    {
        this.roleId = roleId;
        this.targetId = targetId;
        this.damageId = damageId;
        this.timeExecute = timeExecute;
        this.mp = mp;
        this.attackCd = attackCd;
        this.isHit = isHit;
    }
}