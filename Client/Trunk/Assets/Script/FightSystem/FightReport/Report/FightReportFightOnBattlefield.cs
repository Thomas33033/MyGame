using Fight;
public class FightReportRoleAdd : FightReport
{
    public int battlefield;

    public int crewUID;

    public int roleType;

    public int roleId;

    public int hp;

    public int mp;

    public int[] position;

    public FightReportRoleAdd()
    {
    }

    public FightReportRoleAdd(float time, int playerId, int crewUID, int roleType, 
        int roleId, int battlefield, int hp, int mp, int[] position) : 
        base(playerId, ReportType.RoleAdd.ToString(), time)
    {
        this.battlefield = battlefield;
        this.crewUID = crewUID;
        this.roleType = roleType;
        this.roleId = roleId;

        this.hp = hp;
        this.mp = mp;

        this.position = position;
    }
}