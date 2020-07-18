using Fight;
public class FightReportCannonEvent : FightReport
{
    public int battlefieldId;

    public int roleId;

    public int eventType;

    public float endTime;

    public FightReportCannonEvent() { }

    public FightReportCannonEvent(float time, int battlefieldId, int playerId, int roleId, int eventType, float endTime) : base(playerId, ReportType.CannonEvent.ToString(), time)
    {
        this.roleId = roleId;
        this.battlefieldId = battlefieldId;
        this.eventType = eventType;
        this.endTime = endTime;
    }
}