using Fight;

public class FightReportRoleSkillDone : FightReport
{
    public int roleId;

    public int skillId;

    public int[] targetIds;

    public int mp;

    public FightReportRoleSkillDone() { }

    public FightReportRoleSkillDone(float time, int playerId, int roleId, int skillId, int[] targetIds, int mp) : base(playerId, ReportType.RoleSkillDone.ToString(), time)
    {
        this.roleId = roleId;
        this.skillId = skillId;
        this.targetIds = targetIds;
        this.mp = mp;
    }
}