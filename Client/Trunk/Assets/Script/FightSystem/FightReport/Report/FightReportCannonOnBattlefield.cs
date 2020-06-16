using Fight;

public class FightReportCannonOnBattlefield : FightReport
{
    public int roleUid;
    public int roleId;
    public int mp;

    public FightReportCannonOnBattlefield() { }

    public FightReportCannonOnBattlefield(float time, int playerId, int roleUId, int roleId, int mp) : base(playerId, ReportType.CannonOnBattlefield.ToString(), time)
    {
        this.roleUid = roleUId;
        this.roleId = roleId;
        this.mp = mp;
    }
}