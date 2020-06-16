using Fight;

public class FightReportShieldRemove : FightReport
{
    public string roleId;

    public string shieldId;

    public FightReportShieldRemove() { }

    public FightReportShieldRemove(float time, string playerId, string roleId, string shieldId) : base(playerId, ReportType.ShieldRemove.ToString(), time)
    {
        this.roleId = roleId;
        this.shieldId = shieldId;
    }
}