using UnityEngine;
using System.Collections;
using Fight;
public class FightReportRoleAuraRemove : FightReport
{
    public string roleId;
    public string auraId;

    public FightReportRoleAuraRemove() { }

    public FightReportRoleAuraRemove(float time, string playerId, string roleId, string auraId) : base(playerId, ReportType.RoleAuraRemove.ToString(), time)
    {
        this.roleId = roleId;
        this.auraId = auraId;
    }
}
