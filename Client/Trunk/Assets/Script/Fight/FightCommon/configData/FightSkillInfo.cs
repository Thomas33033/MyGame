namespace Fight
{
    /// <summary>
    /// 战斗技能配置
    /// </summary>
    public class FightSkillInfo : IStaticData
    {
        public int id;

        /// <summary>
        /// 0 主动攻击 
        /// 1 被动攻击 
        /// 2 普攻
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