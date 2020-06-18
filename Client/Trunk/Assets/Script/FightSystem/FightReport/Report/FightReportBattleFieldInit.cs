using UnityEngine;
using UnityEditor;
using Fight;

public class FightReportBattleFieldInit : FightReport
{
    public int battlefield;
    
    public FightReportBattleFieldInit() { }

    public FightReportBattleFieldInit(float time, int teamId, float endTime, int stack) : base(teamId, ReportType.BattleFieldInit.ToString(), time)
    {

    }
}