using Fight;

public class FightReportCannonOnBattlefield : FightReport
{
    public string roleUid;
    public string roleId;
    public int mp;

    public FightReportCannonOnBattlefield() { }

    public FightReportCannonOnBattlefield(float time, string playerId, string roleUId, string roleId, int mp) : base(playerId, ReportType.CannonOnBattlefield.ToString(), time)
    {
        this.roleUid = roleUId;
        this.roleId = roleId;
        this.mp = mp;
    }
}