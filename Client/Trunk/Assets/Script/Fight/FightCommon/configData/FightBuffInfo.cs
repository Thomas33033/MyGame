namespace Fight
{
    public class FightBuffInfo : IStaticData
    {
        public int id;
        public int isclear;
        public int trigger;
        public int interval;
        public int[] duration;
        public int count;
        public int[] effectIds;
        public int damageId;
        public string attr;
        public int[] attrValue;
        public int stack;
        public string resource;

        public int GetKey()
        {
            return id;
        }
    }
}