using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 执行建筑攻击逻辑
/// 负责建筑AI，处理攻击
/// 寻找敌人
/// </summary>
public class RoutineComponent : ComponentBase
{
    Tower tower;
    ScanTargetComponent scaner;

    private float thinkEndTime = 0;
    public Transform[] shootPoint;
    public GameObject shootObject;
    public Transform turretObject;

    public int currentClip = 1;
    private float cooldown = 1;       //冷却时间
    private float reloadDuration = 4; //建造时间
    private float lastReloadTime = 0; //上次建造时间

    private float range;
    private bool stunned
    {
        get { return Owner.buffSateMgr.HasState(StateType.Stunned); }
    }

    public float projectingArc = 10;
    public CfgSkillData skillData = null;
    public override void OnInit(CharacterBase p_owner)
    {
        base.OnInit(p_owner);
        if (p_owner is Tower)
        {
            tower = (Tower)p_owner;
        }

        skillData = p_owner.GetSkillData();
    }

    public override void OnUpdate(float dt)
    {
        base.OnUpdate(dt);

        if (this.skillData == null)
        {
            return;
        }

        if (scaner == null && this.skillData != null)
        {
            scaner = tower.GetComponent<ScanTargetComponent>();
            scaner.SetSearchMaxDistance(this.skillData.Attackrange/2); 
        }
            

        if (scaner == null || tower == null || scaner.Target == null)
            return;

        if (!scaner.Target.IsLive()) return;
        
        if (Time.time <  this.thinkEndTime)
        {
            return;
        }
        thinkEndTime = Time.time + Mathf.Max(0.05f, this.cooldown);
        if (tower.type == _TowerType.AOETower)
        {
            // TurretRoutine();
            AOERoutine();
        }
        else if (tower.type == _TowerType.DirectionalAOETower)
        {
            DirectionalAOERoutine();
        }
        else if (tower.type == _TowerType.SupportTower)
        {
            SupportRoutine();
        }
        else if (tower.type == _TowerType.TurretTower)
        {
            AOERoutine();
        }
    }




    private void TurretRoutine()
    {
        if (scaner == null || tower == null)
            return;

        if (Time.time < thinkEndTime) return;
        range = tower.CurData.baseStat.range;
        if (scaner.Target != null && tower.IsBuilt()
            && Vector3.Distance(tower.Trans.position, scaner.Target.Trans.position) < range
             && !stunned)
        {

            foreach (Transform sp in shootPoint)
            {
                this.RequestAttackTarget(scaner.Target, 0);
            }
        }
    }
    
    void DirectionalAOERoutine()
    {
        if (this.turretObject == null)
            this.turretObject = this.Owner.ModelObj.transform.Find("Turret");

        range = tower.CurData.baseStat.range;
        Vector3 targetPosition = scaner.Target.Trans.position;
        Vector3 curPosition = tower.Trans.position;
        bool inRange = Vector3.Distance(curPosition, targetPosition) < range;
        if (inRange && !stunned && currentClip != 0)
        {
            Vector3 srcPos = tower.Trans.position;
            if (turretObject != null) srcPos = turretObject.position;
            Quaternion wantedRotation = Quaternion.LookRotation(targetPosition - srcPos);

            Collider[] cols = Physics.OverlapSphere(curPosition, range, tower.MaskTarget);
            foreach (Collider col in cols)
            {
                Quaternion tgtRotation = Quaternion.LookRotation(col.transform.position - srcPos);
                if (Quaternion.Angle(wantedRotation, tgtRotation) < projectingArc / 2)
                {
                    CharacterBase entity = EntitesManager.Instance.GetMonster(col.gameObject);
                    if (entity != null)
                    {
                        Monster monster = entity as Monster;
                        this.RequestAttackTarget(monster, 1001);
                        Debug.DrawLine(curPosition, monster.Trans.position, Color.red, 0.25f);
                    }

                }
            }

            currentClip -= 1;

            thinkEndTime = Time.time + Mathf.Max(0.05f, cooldown);
        }
    }

    void AOERoutine()
    {
        Vector3 targetPosition = scaner.Target.Trans.position;
        Vector3 curPosition = tower.Trans.position;
        if (!stunned)
        {
            Collider[] cols = Physics.OverlapSphere(targetPosition, range+10, tower.MaskTarget);
            foreach (Collider col in cols)
            {
                CharacterBase entity = EntitesManager.Instance.GetMonster(col.gameObject);
                if (entity != null)
                {
                    Monster monster = entity as Monster;
                    this.RequestAttackTarget(monster, 1001);
                    Debug.DrawLine(targetPosition, curPosition, Color.red, 0.25f);
                }
            }

            thinkEndTime = Time.time + Mathf.Max(0.05f, cooldown);
        }
    }

    private BuffStat buff;
    private UnitTower[] buffList = new UnitTower[0];
    void SupportRoutine()
    {
        Vector3 targetPosition = scaner.Target.Trans.position;
        Vector3 curPosition = tower.Trans.position;

        buff.buffID = (int)tower.TowerID;
        if (shootObject != null)
        {
            SupportTowerShootRoutine();
        }
        LayerMask maskTower = 1 << LayerManager.LayerTower(); ;
        if (!stunned)
        {
            Collider[] cols = Physics.OverlapSphere(curPosition, range, maskTower);

            if (buffList.Length > cols.Length)
            {
                //Debug.Log("more");
                List<UnitTower> tempBuffList = new List<UnitTower>(buffList.Length);
                tempBuffList.AddRange(buffList);

                for (int i = 0; i < tempBuffList.Count; i++)
                {
                    if (tempBuffList[i] == null)
                    {
                        tempBuffList.RemoveAt(i);
                        i--;
                    }
                }

                buffList = tempBuffList.ToArray();
            }
            else if (buffList.Length < cols.Length)
            {
                //Debug.Log("less");
                buffList = new UnitTower[cols.Length];

                for (int i = 0; i < buffList.Length; i++)
                {
                    buffList[i] = cols[i].gameObject.GetComponent<UnitTower>();

                    buffList[i].Buff(buff);
                }
            }

            thinkEndTime = Time.time + Mathf.Max(0.05f, cooldown);
        }
        //else if (!built)
        //{
        //    UnBuffAll();
        //    while (!built) yield return null;
        //    ReBuffAll();
        //}
    }

    void SupportTowerShootRoutine()
    {
        if (shootObject != null)
        {
            foreach (Transform sp in shootPoint)
            {
                string path = "Prefabs/ShootObjects/" + skillData.ResName;
                var pool = ObjectPoolManager.Instance.CreatePool<ModelPoolObj>(path);
                ModelPoolObj modelPool = pool.GetObject();
                modelPool.itemObj.transform.position = sp.position;
                modelPool.itemObj.transform.localRotation = tower.Trans.rotation;
            }
        }
    }

    public void RequestAttackTarget(CharacterBase target, int skillId)
    {
        SysAction sysAction = new SysAction();
        sysAction.operType = OperateType.attack;
        sysAction.curId = Owner.uid;
        sysAction.targetId = target.uid;
        sysAction.skillId = skillId;
        this.RequestAction(sysAction);
    }


    public void OnDestroy()
    {
        base.OnDestroy();
    }
}
