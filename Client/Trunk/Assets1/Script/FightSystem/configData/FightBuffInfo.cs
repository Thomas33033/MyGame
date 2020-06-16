namespace Fight
{
    public class FightBuffInfo : IStaticData
    {
        public string id;
        public int isclear;
        public int trigger;
        public int interval;
        public int[] duration;
        public int count;
        public string[] effectIds;
        public string damageId;
        public string attr;
        public int[] attrValue;
        public int stack;
        public string resource;

        public string GetKey()
        {
            return id;
        }
    }
}