using Fight;
using LuaInterface;
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

    public Dictionary<int, FightRoleRender> dicFightRole;

    public BattlefieldRender battleFieldRender;
  
    private float serverTime;
    private float _reciveServerTime;

    private FightSceneRender() { }

    public override void OnCreate()
    {
        rootUI = GameObject.Find("UIRootDamage");
        _listReport = new List<FightReport>();
        dicFightRole = new Dictionary<int, FightRoleRender>();
    }

    public void SetServerTime(float time)
    {
        _reciveServerTime = Time.realtimeSinceStartup;
        this.serverTime = time;
    }

    public float GetTime()
    {
        return serverTime + Time.realtimeSinceStartup - _reciveServerTime;
    }


    public void InitFight(Platform platform, FightData fightData)
    {
        if (FightScene.Instance.compBehaviour != null)
        {
            FightScene.Instance.compBehaviour.reciveEvent += ReportHandler;
        }

        RegisterFightReport();

        battleFieldRender = new BattlefieldRender(this,platform);
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


        foreach (var v in dicFightRole)
        {
            v.Value.Update();
        }
    }

    //通知UI
    protected void ReportHandler(FightReport report)
    {
        Debug.Log("收到战报 " + report.type);
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

        dicReportHandler.Add(ReportType.RoleMove.ToString(), DoReport_RoleMove);

        dicReportHandler.Add(ReportType.RoleDie.ToString(), DoReport_RoleDie);

        dicReportHandler.Add(ReportType.RoleHurt.ToString(), DoReport_RoleHurt);

        dicReportHandler.Add(ReportType.RoleHpMp.ToString(), DoReport_RoleHpMp);

        dicReportHandler.Add(ReportType.RoleCastSkill.ToString(), DoReport_RoleCastSkill);

        dicReportHandler.Add(ReportType.RoleSkillDone.ToString(), DoReport_RoleSkillDone);

        dicReportHandler.Add(ReportType.RoleAttack.ToString(), DoReport_RoleAttack);
    }

   
    private void DoReport_RoleCreate(FightReport v)
    {
        FightReportRoleCreate report = v as FightReportRoleCreate;
        FightHeroData roleData = report.heroData;

        FightRoleRender roleRender = new FightRoleRender();
        roleRender.SetHpMax(roleData.HP);
        roleRender.SetMpMax(roleData.MP);
        roleRender.LoadNpc(roleData.Resource, battleFieldRender.GetWorldPosition(roleData.NodeId));
        
        
        dicFightRole[report.roleId] = roleRender;

        for (int i = 0; i < roleData.CostNodes.Length; i++)
        {
            battleFieldRender.platform.SetNodeWalkState(roleData.CostNodes[i], false);
        }
        

        string asset = "";
        for (int i = 0; i < roleData.SkillData.Length; i++)
        {
            if (StaticData.dicSkillInfo.ContainsKey(roleData.SkillData[i].skillID) == false)
                continue;

            FightSkillInfo skillInfo = StaticData.dicSkillInfo[roleData.SkillData[i].skillID];
            //预加载特效资源
            //if (string.IsNullOrEmpty(skillInfo.Resources.Trim()) == false)
            //{
            //    asset = ResPathHelper.UI_EFFECT_APTH + skillInfo.Resources + ".prefab";
            //    ObjectPoolManager.Instance.CreateAsyncPool<EffectPoolObj>(asset);
            //}

            //if (string.IsNullOrEmpty(skillInfo.MagicEffect.Trim()) == false)
            //{
            //    asset = ResPathHelper.UI_EFFECT_APTH + skillInfo.MagicEffect + ".prefab";
            //    ObjectPoolManager.Instance.CreateAsyncPool<EffectPoolObj>(asset);
            //}
        }
    }


    private void DoReport_TeamReady(FightReport v)
    {
        FightReportTeamReady report = v as FightReportTeamReady;
    }


    private void DoReport_RoleMove(FightReport v)
    {
        FightReportRoleMove report = v as FightReportRoleMove;

        if (dicFightRole.ContainsKey(report.roleId))
        {
            Vector3 pos = battleFieldRender.GetWorldPosition(report.posId);
            dicFightRole[report.roleId].Move(pos,    report.endTime - report.time);
        }
                
    }


    private void DoReport_RoleAttack(FightReport v)
    {
        Debug.Log("DoReport_RoleAttack");
        FightReportRoleAttack report = v as FightReportRoleAttack;
        FightDamageInfo fightDamageInfo = StaticData.GetDamageInfo(report.damageId, 1);
        if (fightDamageInfo == null || string.IsNullOrEmpty(fightDamageInfo.Resources) == true)
        {
            return;
        }

        if (dicFightRole.ContainsKey(report.roleId))
        {
          
            dicFightRole[report.roleId].AttackShow(fightDamageInfo, report.time, 
                report.timeExecute, report.isHit, dicFightRole[report.targetId]);
        }
    }


    private void DoReport_RoleDie(FightReport v)
    {
        FightReportRoleDie report = v as FightReportRoleDie;

        if (dicFightRole.ContainsKey(report.roleId))
        {
            dicFightRole[report.roleId].Die();
        }
    }


    private void DoReport_RoleHurt(FightReport v)
    {
        FightReportRoleHurt report = v as FightReportRoleHurt;

        if (dicFightRole.ContainsKey(report.roleId) == false)
            return;

        dicFightRole[report.roleId].ShowHurt(report.hp, report.isHit, report.hurt, report.isCrit, report.source, report.damageType);
        dicFightRole[report.roleId].SetHp(report.hp);
        dicFightRole[report.roleId].SetMp(report.mp);
    }

    private void DoReport_RoleHpMp(FightReport v)
    {
        FightReportRoleHpMp report = v as FightReportRoleHpMp;
        Debug.Log(report.hp + " " + report.mp);
        dicFightRole[report.roleId].SetHp(report.hp);
        dicFightRole[report.roleId].SetMp(report.mp);
    }

    //播放施法效果
    private void DoReport_RoleCastSkill(FightReport v)
    {
        FightReportRoleCastSkill report = v as FightReportRoleCastSkill;
        FightSkillInfo fightSkillInfo = StaticData.dicSkillInfo[report.skillId];

        if (dicFightRole.ContainsKey(report.roleId) == false || StaticData.dicSkillInfo.ContainsKey(report.skillId) == false)
            return;

        List<RoleRender> listTargets = new List<RoleRender>();
        for (int i = 0; i < report.targetIds.Length; i++)
        {
            listTargets.Add(dicFightRole[report.targetIds[i]]);
        }
        dicFightRole[report.roleId].SkillCast(fightSkillInfo, listTargets);
        dicFightRole[report.roleId].SetMp(report.mp);
    }

    //播放技能效果
    private void DoReport_RoleSkillDone(FightReport v)
    {
        FightReportRoleSkillDone report = v as FightReportRoleSkillDone;
        FightSkillInfo fightSkillInfo = StaticData.dicSkillInfo[report.skillId];

        if (dicFightRole.ContainsKey(report.roleId) == false || StaticData.dicSkillInfo.ContainsKey(report.skillId) == false)
            return;

        List<RoleRender> listTargets = new List<RoleRender>();
        for (int i = 0; i < report.targetIds.Length; i++)
        {
            listTargets.Add(dicFightRole[report.targetIds[i]]);
        }
        dicFightRole[report.roleId].SkillDone(fightSkillInfo, listTargets);
        dicFightRole[report.roleId].SetMp(report.mp);
    }

}
