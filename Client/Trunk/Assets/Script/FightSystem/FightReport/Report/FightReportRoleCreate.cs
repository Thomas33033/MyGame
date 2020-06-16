using Fight;

public class FightReportRoleCreate : FightReport
{
    public int battlefield;

    public int roleId;

    public FightReportRoleCreate()
    {

    }

    public FightReportRoleCreate(float time, int playerId, int roleId, int battlefield) : 
        base(playerId, ReportType.RoleAdd.ToString(), time)
    {
        this.battlefield = battlefield;
        this.roleId = roleId;
    }
}