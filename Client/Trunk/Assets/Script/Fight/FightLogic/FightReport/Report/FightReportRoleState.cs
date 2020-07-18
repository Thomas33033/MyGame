using Fight;
public class FightReportRoleState : FightReport
{
    public int state;
    public int stateNum;
    public int roleId;

    public FightReportRoleState()
    {
    }

    public FightReportRoleState(float time, int playerId, int roleId, int state, int stateNum) : base(playerId, ReportType.RoleState.ToString(), time)
    {
        this.roleId = roleId;
        this.state = state;
        this.stateNum = stateNum;
    }
}