using Fight;
public class FightReportCannonEventDone : FightReport
{
    public string roleId;
    public int evtType;

    public FightReportCannonEventDone(float time, string playerId, string roleId, int evtType) : base(playerId, ReportType.CannonEventDone.ToString(), time)
    {
        this.roleId = roleId;
        this.evtType = evtType;
    }
}