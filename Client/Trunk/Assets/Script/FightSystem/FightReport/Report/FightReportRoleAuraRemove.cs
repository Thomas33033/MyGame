using UnityEngine;
using System.Collections;
using Fight;
public class FightReportRoleAuraRemove : FightReport
{
    public int roleId;
    public int auraId;

    public FightReportRoleAuraRemove() { }

    public FightReportRoleAuraRemove(float time, int playerId, int roleId, int auraId) : base(playerId, ReportType.RoleAuraRemove.ToString(), time)
    {
        this.roleId = roleId;
        this.auraId = auraId;
    }
}
