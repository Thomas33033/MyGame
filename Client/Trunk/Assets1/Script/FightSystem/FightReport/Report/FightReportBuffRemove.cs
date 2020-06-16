using UnityEngine;
using UnityEditor;
using Fight;
public class FightReportBuffRemove : FightReport
{
    public string roleId;
    public string buffId;

    public FightReportBuffRemove() { }

    public FightReportBuffRemove(float time,string teamId,string roleId, string buffId) : base(teamId, ReportType.BuffRemove.ToString(), time)
    {
        this.roleId = roleId;
        this.buffId = buffId;
    }
}