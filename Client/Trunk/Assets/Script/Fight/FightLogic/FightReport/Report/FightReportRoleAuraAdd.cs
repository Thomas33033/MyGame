using UnityEngine;
using System.Collections;
using Fight;
public class FightReportRoleAuraAdd : FightReport
{
    public int roleId;
    public int auraId;

    public FightReportRoleAuraAdd() { }

    public FightReportRoleAuraAdd(float time, int playerId, int roleId, int auraId) : base(playerId, ReportType.RoleAuraAdd.ToString(), time)
    {
        this.roleId = roleId;
        this.auraId = auraId;
    }
}
