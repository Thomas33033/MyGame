using System.Collections.Generic;

namespace Fight
{

    public class FightData
    {

        public FightPlayerData selfBattleData;

        public FightPlayerData enemyBattleData;

        public BattleFieldData battleFieldData;
    }

    public class BattleFieldData
    {
        //战场寻路数据
        public Node[] nodeGraph;
    }

    public class FightPlayerData
    {
        public FightPlayerInfo userData;
        public FightHeroData[] heroData;
        public FightSkillData[] teamSkills;
    }

    public class FightPlayerInfo
    {
        public int userID;
        public string nickName;
        public string userIcon;
        public int camp;
    }

    public class FightSkillData
    {
        public int skillID;
        public int level;
    }


    public class FightHeroData
    {
        public int uid;
        public int roleId;
        public int crewID;
        public string Resource;
        public string AttackRes;
        public string Tag;

        public int Position;
        public string CostNodes;
        public int PlatformId;

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

        //死亡掉落
        public string dieDrop;

        
    }
}