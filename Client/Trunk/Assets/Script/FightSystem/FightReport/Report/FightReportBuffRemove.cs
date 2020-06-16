using UnityEngine;
using UnityEditor;
using Fight;
public class FightReportBuffRemove : FightReport
{
    public int roleId;
    public int buffId;

    public FightReportBuffRemove() { }

    public FightReportBuffRemove(float time, int teamId, int roleId, int buffId) : base(teamId, ReportType.BuffRemove.ToString(), time)
    {
        this.roleId = roleId;
        this.buffId = buffId;
    }
}