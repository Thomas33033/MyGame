using System.Collections.Generic;

namespace Fight
{

    public class FightData
    {
        public FightPlayerData selfBattleData;
        public FightPlayerData enemyBattleData;
    }

    public class FightPlayerData
    {
        public FightPlayerInfo userData;
        public FightShipData shipData;
        public FightSeamanData[] seamanData;
        public FightSkillData[] teamSkills;
    }

    public class FightPlayerInfo
    {
        public string userID;
        public string nickName;
        public string userIcon;
        public int camp;
    }

    public class FightSkillData
    {
        public string skillID;
        public int level;
    }

    public class FightShipData
    {
        public string ShipID;
        public string Resources;
        public int ResourcesType;
        public float CurHp;
        public int HP;
        public int Armor;
        public int Dodge;
        public int DamageReduction;
        public int FireDefense;
        public int LeakingDefense;
        public int WeaponDefense;

        public FightSkillData[] SkillData;
    }

    public class FightSeamanData
    {
        public string uid;
        public string roleId;
        public string crewID;
        public string Resource;
        public string AttackRes;
        public string Tag;
        public int Position;

        public int Attack;
        public int AttackSpeed;
        public int BodyCrit;
        public int BodyDodge;
        public int BodyHit;
        public int Defense;
        public int GunAttack;
        public int GunCrit;
        public int GunHit;
        public int HP;
        public int CurMp;
        public float CurHp;
        public int Level;
        public int MagicDefense;
        public int MaxAnger;
        public int MoveSpeed;
        public int Range;
        public int ReloadSpeed;
        public int Star;
        public FightSkillData[] SkillData;
    }
}