using UnityEngine;
using System.Collections;
using Fight;
public class FightReportRoleAuraAdd : FightReport
{
    public string roleId;
    public string auraId;

    public FightReportRoleAuraAdd() { }

    public FightReportRoleAuraAdd(float time,string playerId, string roleId, string auraId) : base(playerId, ReportType.RoleAuraAdd.ToString(), time)
    {
        this.roleId = roleId;
        this.auraId = auraId;
    }
}
