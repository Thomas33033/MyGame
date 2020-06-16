
namespace Fight
{
    public class FightReport
    {
        public int id;

        public string type;

        public int playerId;

        public float time;

        public FightReport()
        {
        }

        public FightReport(int playerId, string type, float time)
        {
            this.playerId = playerId;
            this.type = type;
            this.id = GetIndex();
            this.time = time;
        }

        static private int _index;

        static private int GetIndex()
        {
            return _index++;
        }
    }


}
public enum ReportType
{

    RoleAdd,
    RoleMove,
    RoleDie,
    RoleAttack,
    RoleHurt,
    RoleCastSkill,
    RoleState,
    RoleSkillDone,
    RoleJump,
    RoleHpMp,

    TeamRoles,
    TeamPoint,
    TeamReady,

    CannonOnBattlefield,
    CannonInfo,
    CannonEvent,
    CannonEventDone,

    ShipHurt,
    ShipEvent,
    ShipEventAttack,
    ShipEventDone,
    ShipEventRescue,

    GameOver,
    GameStart,

    EffectTrigger,

    RoleAuraAdd,
    RoleAuraRemove,

    BuffAdd,
    BuffRemove,

    ShieldAdd,
    ShieldRemove,

    Summon,
}