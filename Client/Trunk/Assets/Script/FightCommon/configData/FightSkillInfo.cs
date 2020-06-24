namespace Fight
{
    public class FightSkillInfo : IStaticData
    {
        public int id;

        /// <summary>
        /// 0 普通攻击 1 物理伤害 2法术伤害 3加血 4加蓝
        /// </summary>
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