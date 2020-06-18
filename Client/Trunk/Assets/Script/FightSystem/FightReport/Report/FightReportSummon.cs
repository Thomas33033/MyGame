using Fight;
public class FightReportSummon : FightReport
{
    public int battlefield;

    public int roleType;

    public int roleId;

    public int hp;

    public int mp;

    public int posId;

    public string npcAsset;

    public FightReportSummon() { }

    public FightReportSummon(float time, int playerId, int roleType, int roleId, int battlefield, int hp, 
        int mp, int posId, string npcAsset) : base(playerId, ReportType.Summon.ToString(), time)
    {
        this.battlefield = battlefield;
        this.roleType = roleType;
        this.roleId = roleId;

        this.hp = hp;
        this.mp = mp;

        this.posId = posId;
        this.npcAsset = npcAsset;
    }
}
