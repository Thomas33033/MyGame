using UnityEngine;

namespace Fight
{
    /// <summary>
    /// 护盾
    /// </summary>
    public class FightShield : FightAttackDataBase
    {
        public FightShieldInfo info;

        public float timeCreate;

        public int sheild;

        public float timeDuration { get => info.duration / 1000f; }

        public FightShield(float time, Role role, Role target, FightShieldInfo info, int level) : base(role, target, level, DamageSourceType.Shield)
        {
            this.timeCreate = time;
            this.info = info;

            if (info.shield.Length > 0)
            {
                sheild = info.shield[Mathf.Min(info.shield.Length - 1, level - 1)];
            }

            int bouns = 0;
            if (info.bouns.Length > 0)
            {
                bouns = info.bouns[Mathf.Min(info.bouns.Length - 1, level - 1)];
            }

            switch (info.properties)
            {
                case 0:
                    break;

                case 1:
                    sheild += Mathf.FloorToInt(role.physicsDefense * bouns / 100f);
                    break;

                case 2:
                    sheild += Mathf.FloorToInt(role.physicsAttack * bouns / 100f);
                    break;

                case 3:
                    sheild += Mathf.FloorToInt(role.hpMax * bouns / 100f);
                    break;

                case 4:
                    sheild += Mathf.FloorToInt(role.hp * bouns / 100f);
                    break;

                case 5:
                    sheild += Mathf.FloorToInt(target.hpMax * bouns / 100f);
                    break;

                case 6:
                    sheild += Mathf.FloorToInt(target.hp * bouns / 100f);
                    break;
            }
        }
    }
}