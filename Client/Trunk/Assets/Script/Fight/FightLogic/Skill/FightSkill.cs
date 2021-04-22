using System;

namespace Fight
{

    public enum SkillAttackType
    {
        /// <summary>
        /// 主动技能
        /// </summary>
        Active = 0,
        /// <summary>
        /// 被动技能
        /// </summary>
        Passive = 1,
        /// <summary>
        /// 普攻
        /// </summary>
        Normal = 2,
    }

    /// <summary>
    /// 战斗技能，实例化单个技能
    /// </summary>
    public class FightSkill
    {
        public int skillId { get => info.id; }

        public int type { get => info.Type; }

        public int level;

        public float lastTime;

        public float cd;

        public Role role;

        public FightSkillInfo info;

        public FightEffectBox fightEffectBox;

        public bool isChannelling { get => fightEffectBox.isChannelling; }
        public bool needUpdating { get => fightEffectBox.needUpdating; }

        public FightSkill(Role role, FightSkillInfo info, int level)
        {
            this.role = role;
            this.info = info;
            this.level = level;

            DamageSourceType damageSourceType = DamageSourceType.Skill;

            if (info.Type == 2)
            {
                damageSourceType = DamageSourceType.Attack;
            }

            fightEffectBox = new FightEffectBox(info.effectIds, level, role, role, damageSourceType);

#if UNITY_EDITOR

            if (fightEffectBox.listEffect.Count == 0)
            {
                UnityEngine.Debug.LogError("no action " + info.id);
            }

#endif
        }

        public bool Trigger(TriggerType type, FightAttackDataBase something = null, int space = 0)
        {
            return fightEffectBox.Trigger(type, something, space);
        }

        internal void Update(float nowTime)
        {
            fightEffectBox.Update(nowTime);
        }
    }
}