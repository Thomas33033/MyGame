using Fight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class FightCenter
{

    FightScene sceneLogic;
    FightSceneRender sceneRender;

    //预定的每帧的时间长度
    public static Fix64 g_fixFrameLen = Fix64.FromRaw(273);

    //游戏的逻辑帧
    public static int g_uGameLogicFrame = 0;

    //是否为回放模式
    public static bool g_bRplayMode = false;

    //随机数对象
    public static SRandom g_srand = new SRandom(1000);

    ////所有操作事件的队列
    //public static List<battleInfo> g_listUserControlEvent = new List<battleInfo>();

    ////所有回放事件的队列
    //public static List<battleInfo> g_listPlaybackEvent = new List<battleInfo>();


    private LockStepLogic lockStepLogic;

    //初始化战斗
    public FightCenter()
    {
        lockStepLogic = new LockStepLogic();
        lockStepLogic.setCallUnit(this);
    }


    public void Update()
    {
        lockStepLogic.updateLogic();
    }


    public void frameLockLogic()
    {
        //如果是回放模式
        //if (GameData.g_bRplayMode)
        //{
        //    //检测回放事件
        //    checkPlayBackEvent(GameData.g_uGameLogicFrame);
        //}

        //recordLastPos();

        sceneLogic.Update();
    }

    public void updateRenderPosition(float interpolation)
    {
        sceneRender.Update(interpolation);
    }


    public void OnInit(FightData fightData, Platform platform)
    {
        sceneLogic = new FightScene(this);

        sceneRender = new FightSceneRender();

        sceneLogic.InitFight(FightType.ConmmFight, fightData);

        sceneRender.InitFight(platform);

        sceneLogic.compBehaviour.reciveEvent += sceneRender.ReportHandler;

        // sceneRender.SetServerTime(fightScene.compBehaviour.GetTime());
    }


    public void CreateRole(FightRoleData fightBuildData)
    {
        sceneLogic.CreateRole(fightBuildData);
    }

    public float GetGameTime()
    {
        return lockStepLogic.GetTime();
    }
}

