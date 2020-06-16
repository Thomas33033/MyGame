using Fight;
public class FightReportCannonInfo : FightReport
{
    public int roleId;
    public float lastFireTime;
    public float fireCD;
    public int mp;

    public FightReportCannonInfo() { }

    public FightReportCannonInfo(float time, int playerId, int roleId, float lastFireTime, float fireCD, int mp) : base(playerId, ReportType.CannonInfo.ToString(), time)
    {
        this.roleId = roleId;
        this.mp = mp;
        this.lastFireTime = lastFireTime;
        this.fireCD = fireCD;
    }
}