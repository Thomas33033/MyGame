using Fight;

public class FightReportShieldRemove : FightReport
{
    public int roleId;

    public int shieldId;

    public FightReportShieldRemove() { }

    public FightReportShieldRemove(float time, int playerId, int roleId, int shieldId) : base(playerId, ReportType.ShieldRemove.ToString(), time)
    {
        this.roleId = roleId;
        this.shieldId = shieldId;
    }
}