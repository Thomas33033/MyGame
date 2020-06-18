namespace Fight
{
    public class FightSkillInfo : IStaticData
    {
        public int id;

        public int Type;

        public int EndTime;

        //public int SkillTime;

        public string Resources;

        public string ResourcesSound;

        public string MagicEffect;

        public string MagicEffectSound;

        public int[] effectIds;

        public string SkillAct;

        public int GetKey()
        {
            return id;
        }
    }
}