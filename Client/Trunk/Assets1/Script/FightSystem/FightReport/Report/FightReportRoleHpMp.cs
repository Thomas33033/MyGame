using UnityEngine;
using System.Collections;
using Fight;

public class FightReportRoleHpMp : FightReport
{
    public string roleId;
    public int hp;
    public int mp;

    public FightReportRoleHpMp() { }

    public FightReportRoleHpMp(float time, string playerId, string roleId, int hp, int mp) : base(playerId, ReportType.RoleHpMp.ToString(), time)
    {
        this.roleId = roleId;
        this.hp = hp;
        this.mp = mp;
    }
}