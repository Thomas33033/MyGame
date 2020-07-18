using Fight;
public class FightReportRoleJump : FightReport
{
    public int battlefield;

    public int roleId;

    public int posId;

    public float endTime;

    public FightReportRoleJump() { }

    public FightReportRoleJump(float time, int playerId, int roleId, int battlefield, float endTime, int posId) : base(playerId, ReportType.RoleJump.ToString(), time)
    {
        this.battlefield = battlefield;
        this.roleId = roleId;
        this.endTime = endTime;
        this.posId = posId;
    }
}