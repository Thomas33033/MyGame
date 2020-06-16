﻿using Fight;

public class FightReportRoleMove : FightReport
{
    public int battlefield;

    public string roleId;

    public int[] position;

    public float endTime;

    public FightReportRoleMove() { }

    public FightReportRoleMove(float time, string playerId, string roleId, int battlefield, float endTime, int[] position) : base(playerId, ReportType.RoleMove.ToString(), time)
    {
        this.battlefield = battlefield;
        this.roleId = roleId;
        this.position = position;
        this.endTime = endTime;
    }
}