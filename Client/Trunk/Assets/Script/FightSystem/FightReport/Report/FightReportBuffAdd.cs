using UnityEngine;
using UnityEditor;
using Fight;
public class FightReportBuffAdd : FightReport
{
    public int roleId;
    public int buffId;
    public int level;
    public float endTime;
    public int stack;

    public FightReportBuffAdd() { }

    public FightReportBuffAdd(float time, int teamId, int roleId, int buffId,int level ,float endTime,int stack) : base(teamId, ReportType.BuffAdd.ToString(), time)
    {
        this.roleId = roleId;
        this.buffId = buffId;
        this.level = level;
        this.endTime = endTime;
        this.stack = stack;
    }
}