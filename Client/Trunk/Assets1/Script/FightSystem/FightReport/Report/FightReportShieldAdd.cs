using Fight;

public class FightReportShieldAdd : FightReport
{
    public string roleId;

    public string shieldId;

    public FightReportShieldAdd() { }

    public FightReportShieldAdd(float time, string playerId, string roleId, string shieldId) : base(playerId, ReportType.ShieldAdd.ToString(), time)
    {
        this.roleId = roleId;
        this.shieldId = shieldId;
    }
}