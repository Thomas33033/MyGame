using Fight;

public class FightReportShieldAdd : FightReport
{
    public int roleId;

    public int shieldId;

    public FightReportShieldAdd() { }

    public FightReportShieldAdd(float time, int playerId, int roleId, int shieldId) : base(playerId, ReportType.ShieldAdd.ToString(), time)
    {
        this.roleId = roleId;
        this.shieldId = shieldId;
    }
}