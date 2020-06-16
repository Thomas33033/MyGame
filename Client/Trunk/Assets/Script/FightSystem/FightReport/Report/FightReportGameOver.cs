
using System.Collections.Generic;
using Fight;
public class FightReportGameOver : FightReport
{
    public string Hps;

    public FightReportGameOver() { }

    public FightReportGameOver(float time, int playerId, string roleHps) : base(playerId, ReportType.GameOver.ToString(), time)
    {
        this.Hps = roleHps;
    }
}