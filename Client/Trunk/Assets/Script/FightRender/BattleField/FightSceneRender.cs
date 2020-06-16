using Fight;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 战斗场景渲染
/// 1.注册战报
/// </summary>
public class FightSceneRender : Singleton<FightSceneRender>
{
    public GameObject rootUI;
    private Dictionary<string, System.Action<FightReport>> dicReportHandler;
    private List<FightReport> _listReport;

    

    public float GetTime()
    {
        return Time.realtimeSinceStartup;
    }

    public FightSceneRender()
    {
        _listReport = new List<FightReport>();

        if (FightScene.Instance.compBehaviour != null)
        {
            FightScene.Instance.compBehaviour.reciveEvent += ReportHandler;
        }

        RegisterFightReport();
    }


    public  void Update()
    {

        FightScene.Instance.Update();
        //base.Update();

        //if (_timeSkillCameraClose > 0 && Time.time > _timeSkillCameraClose)
        //{
        //    _numSkillCamera = 0;
        //    CloseSkillCamera();
        //}

        if (_listReport.Count > 0)
        {
            for (int i = 0; i < _listReport.Count; i++)
            {
                FightReport report = _listReport[i];
              

                if (dicReportHandler.ContainsKey(report.type))
                    dicReportHandler[report.type](report);
            }
            _listReport.Clear();
        }
    }

    //通知UI
    protected void ReportHandler(FightReport report)
    {
        //if (_luaReportFun != null && Array.IndexOf<string>(_arrListeners, report.type) > -1)
        //{
        //    _luaReportFun.call(report);
        //}
        _listReport.Add(report);
    }

    private void RegisterFightReport()
    {
        dicReportHandler = new Dictionary<string, System.Action<FightReport>>();
        
        dicReportHandler.Add(ReportType.RoleAdd.ToString(), DoReport_RoleCreate);

        dicReportHandler.Add(ReportType.TeamReady.ToString(), DoReport_TeamReady);
    }

    private void DoReport_RoleCreate(FightReport v)
    {
        FightReportRoleCreate report = v as FightReportRoleCreate;

    }


    private void DoReport_TeamReady(FightReport v)
    {
        FightReportTeamReady report = v as FightReportTeamReady;
    }


    

   
}
