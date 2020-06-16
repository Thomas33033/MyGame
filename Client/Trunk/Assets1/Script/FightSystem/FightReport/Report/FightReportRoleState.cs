using Fight;
public class FightReportRoleState : FightReport
{
    public int state;
    public int stateNum;
    public string roleId;

    public FightReportRoleState()
    {
    }

    public FightReportRoleState(float time, string playerId, string roleId, int state, int stateNum) : base(playerId, ReportType.RoleState.ToString(), time)
    {
        this.roleId = roleId;
        this.state = state;
        this.stateNum = stateNum;
    }
}