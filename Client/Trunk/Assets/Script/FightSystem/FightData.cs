using System.Collections.Generic;
using FightCommom;

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
        public int row;
        public int column;
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
        public int npcId;
        public int npcType;
        public string Resource;
        public string AttackRes;
        public string Tag;
        public int teamId;
        public int NodeSize;


        public int NodeId;
        public string Position;
        public int[] CostNodes;
        public int PlatformId;

        public int PhysicalAttack;
        public int MagicAttack;
        public int PhysicalDefense;
        public int MagicDefense;

        public int AttackSpeed;
        public int Crit;
        public int Dodge;
        public int Hit;
        
        public int HP;
        public int MP;
        public int CurMp;
        public float CurHp;
        public int Level;
        
        public int MaxAnger;
        public int MoveSpeed;
        public int Range;
        
        public int Star;
        public FightSkillData[] SkillData = new FightSkillData[] { };

        //死亡掉落
        public string dieDrop;

        
    }
}