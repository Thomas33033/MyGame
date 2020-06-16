using Fight;
public class FightReportRoleDie : FightReport
{
    public int battlefield;

    public int roleId;

    public FightReportRoleDie()
    {
    }

    public FightReportRoleDie(float time, int playerId, int roleId, int battlefield) : base(playerId, ReportType.RoleDie.ToString(), time)
    {
        this.battlefield = battlefield;
        this.roleId = roleId;
    }
}