namespace Fight
{
    public class FightAuraInfo : IStaticData
    {
        public int id;

        public int targetType;

        public int[] effectIds;

        public int zone;

        public int range;

        public int duration;

        public string resource;

        public string attr;

        public int[] attrValue;

        public int GetKey()
        {
            return id;
        }
    }
}