using Fight;
public class FightReportRoleDie : FightReport
{
    public int battlefield;

    public string roleId;

    public FightReportRoleDie()
    {
    }

    public FightReportRoleDie(float time, string playerId, string roleId, int battlefield) : base(playerId, ReportType.RoleDie.ToString(), time)
    {
        this.battlefield = battlefield;
        this.roleId = roleId;
    }
}