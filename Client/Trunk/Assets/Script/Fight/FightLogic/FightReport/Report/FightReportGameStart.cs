using Fight;

namespace Fight
{
    public class FightReportGameStart : FightReport
    {
        public FightReportGameStart(float time, int playerId) : base(playerId, ReportType.GameStart.ToString(), time)
        {
        }

        public FightReportGameStart() { }
    }
}