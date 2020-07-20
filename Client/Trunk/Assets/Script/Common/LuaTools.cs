
public class LuaTools
{
    static public  void DragNpc(int cfgId, int teamId)
    {
        NpcData _data = new NpcData();
        _data.InitData(cfgId, (ETeamType)teamId);
        BuildManager.BuildTowerDragNDrop(_data);
    }

  
}
