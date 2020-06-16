public class FireFightInfo : IStaticData
{
    public string FightType;
    public float FightNum;

    public string GetKey()
    {
        return FightType;
    }
}