namespace Fight
{
    public class FightSkillInfo : IStaticData
    {
        public string id;

        public int Type;

        public int EndTime;

        //public int SkillTime;

        public string Resources;

        public string ResourcesSound;

        public string MagicEffect;

        public string MagicEffectSound;

        public string[] effectIds;

        public string SkillAct;

        public string GetKey()
        {
            return id;
        }
    }
}