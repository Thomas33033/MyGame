using Fight;
public class FightReportSummon : FightReport
{
    public int battlefield;

    public int roleType;

    public string roleId;

    public int hp;

    public int mp;

    public int[] position;

    public string npcAsset;

    public FightReportSummon() { }

    public FightReportSummon(float time, string playerId, int roleType, string roleId, int battlefield, int hp, int mp, int[] position,string npcAsset) : base(playerId, ReportType.Summon.ToString(), time)
    {
        this.battlefield = battlefield;
        this.roleType = roleType;
        this.roleId = roleId;

        this.hp = hp;
        this.mp = mp;

        this.position = position;
        this.npcAsset = npcAsset;
    }
}
