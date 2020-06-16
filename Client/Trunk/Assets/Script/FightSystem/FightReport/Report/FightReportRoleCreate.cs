using Fight;

public class FightReportRoleCreate : FightReport
{
    public int battlefield;

    public int roleId;

    public FightHeroData heroData;

    public FightReportRoleCreate()
    {

    }

    public FightReportRoleCreate(float time, int playerId, FightHeroData heroData, int battlefield) : 
        base(playerId, ReportType.RoleAdd.ToString(), time)
    {
        this.battlefield = battlefield;
        this.roleId = heroData.roleId;
        this.heroData = heroData;
    }
}