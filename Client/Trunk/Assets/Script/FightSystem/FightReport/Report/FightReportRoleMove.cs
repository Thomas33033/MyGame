using Fight;

public class FightReportRoleMove : FightReport
{
    public int battlefield;

    public int roleId;

    public int posId;

    public float endTime;

    public FightReportRoleMove() { }

    public FightReportRoleMove(float time, int playerId, int roleId, int battlefield, float endTime, int posId) : base(playerId, ReportType.RoleMove.ToString(), time)
    {
        this.battlefield = battlefield;
        this.roleId = roleId;
        this.posId = posId;
        this.endTime = endTime;
    }
}