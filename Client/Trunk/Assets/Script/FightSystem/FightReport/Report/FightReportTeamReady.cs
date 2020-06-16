using Fight;
public class FightReportTeamReady : FightReport
{
    public FightReportTeamReady() { }

    public FightReportTeamReady(float time, int playerId) : base(playerId, ReportType.TeamReady.ToString(), time)
    {
    }
}