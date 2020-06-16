using Fight;
public class FightReportCannonEventDone : FightReport
{
    public int roleId;
    public int evtType;

    public FightReportCannonEventDone(float time, int playerId, int roleId, int evtType) : base(playerId, ReportType.CannonEventDone.ToString(), time)
    {
        this.roleId = roleId;
        this.evtType = evtType;
    }
}