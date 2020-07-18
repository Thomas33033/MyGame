using Fight;
public class FightReportRoleCastSkill : FightReport
{
    public int roleId;

    public int skillId;

    public int[] targetIds;

    public int mp;

    public float attackSpeed;

    public FightReportRoleCastSkill() { }

    public FightReportRoleCastSkill(float time, int playerId, int roleId, int skillId, int[] targetIds, int mp, float attackSpeed = 0) : base(playerId, ReportType.RoleCastSkill.ToString(), time)
    {
        this.roleId = roleId;
        this.skillId = skillId;
        this.targetIds = targetIds;
        this.mp = mp;
        this.attackSpeed = attackSpeed;
    }
}