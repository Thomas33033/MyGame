using Fight;
public class FightReportFightOnBattlefield : FightReport
{
    public int battlefield;

    public string crewUID;

    public int roleType;

    public string roleId;

    public int hp;

    public int mp;

    public int[] position;

    public FightReportFightOnBattlefield()
    {
    }

    public FightReportFightOnBattlefield(float time, string playerId, string crewUID, int roleType, string roleId, int battlefield, int hp, int mp, int[] position) : base(playerId, ReportType.FightOnBattlefield.ToString(), time)
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