public class FireFightInfo : IStaticData
{
    public int FightType;
    public float FightNum;

    public int GetKey()
    {
        return FightType;
    }
}