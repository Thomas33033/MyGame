using Fight;

namespace Fight
{
    public class FightReportGameStart : FightReport
    {
        public FightReportGameStart(float time, string playerId) : base(playerId, ReportType.GameStart.ToString(), time)
        {
        }

        public FightReportGameStart() { }
    }
}