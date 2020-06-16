namespace Fight
{
    public class FightAuraInfo : IStaticData
    {
        public string id;

        public int targetType;

        public string[] effectIds;

        public int zone;

        public int range;

        public int duration;

        public string resource;

        public string attr;

        public int[] attrValue;

        public string GetKey()
        {
            return id;
        }
    }
}