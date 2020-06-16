using Fight;
public class FightReportRoleJump : FightReport
{
    public int battlefield;

    public string roleId;

    public int[] position;

    public float endTime;

    public FightReportRoleJump() { }

    public FightReportRoleJump(float time, string playerId, string roleId, int battlefield, float endTime, int[] position) : base(playerId, ReportType.RoleJump.ToString(), time)
    {
        this.battlefield = battlefield;
        this.roleId = roleId;
        this.endTime = endTime;
        this.position = position;
    }
}