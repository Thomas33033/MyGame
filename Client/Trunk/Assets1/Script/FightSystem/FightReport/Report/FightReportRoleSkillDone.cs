using Fight;

public class FightReportRoleSkillDone : FightReport
{
    public string roleId;

    public string skillId;

    public string[] targetIds;

    public int mp;

    public FightReportRoleSkillDone() { }

    public FightReportRoleSkillDone(float time, string playerId, string roleId, string skillId, string[] targetIds, int mp) : base(playerId, ReportType.RoleSkillDone.ToString(), time)
    {
        this.roleId = roleId;
        this.skillId = skillId;
        this.targetIds = targetIds;
        this.mp = mp;
    }
}