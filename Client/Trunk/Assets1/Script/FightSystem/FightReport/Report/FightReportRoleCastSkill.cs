using Fight;
public class FightReportRoleCastSkill : FightReport
{
    public string roleId;

    public string skillId;

    public string[] targetIds;

    public int mp;

    public float attackSpeed;

    public FightReportRoleCastSkill() { }

    public FightReportRoleCastSkill(float time, string playerId, string roleId, string skillId, string[] targetIds, int mp, float attackSpeed = 0) : base(playerId, ReportType.RoleCastSkill.ToString(), time)
    {
        this.roleId = roleId;
        this.skillId = skillId;
        this.targetIds = targetIds;
        this.mp = mp;
        this.attackSpeed = attackSpeed;
    }
}