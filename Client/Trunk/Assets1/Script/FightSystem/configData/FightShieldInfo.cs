namespace Fight
{
    public class FightShieldInfo : IStaticData
    {
        public string id;
        public int duration;
        public int type;
        public int[] shield;
        public int properties;
        public int[] bouns;
        public string resource;

        public string GetKey()
        {
            return id;
        }
    }
}