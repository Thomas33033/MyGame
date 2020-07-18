using UnityEngine;
using System.Collections;
using Fight;

public class FightReportRoleHpMp : FightReport
{
    public int roleId;
    public int hp;
    public int mp;

    public FightReportRoleHpMp() { }

    public FightReportRoleHpMp(float time, int playerId, int roleId, int hp, int mp) : base(playerId, ReportType.RoleHpMp.ToString(), time)
    {
        this.roleId = roleId;
        this.hp = hp;
        this.mp = mp;
    }
}