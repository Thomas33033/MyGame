using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FightCommom;

public enum BuildState
{
    none,
    buildBefor, //等待建造
    building,  //建造中
    buildOver, //建造完成
}

public class Tower : CharacterBase
{
    private TowerMapData mapData;
    public CfgNpcAttrData attrConfig;  

    public uint TowerID
    {
        get { return towerID; }
        set { towerID = value; }
    }



    private enum _UnitSubClass { None, Creep, Tower };
    private _UnitSubClass subClass = _UnitSubClass.None;


    private OccupiedPlatform occupiedPlatform;
    private uint towerID;
    public _TowerType type;

    public int specialID = -1;
    public _TargetMode targetMode = _TargetMode.Hybrid;
    private int maskTarget;

    public BuildState buildState = BuildState.buildBefor;

    public int MaskTarget
    {
        get { return maskTarget; }
    }

    private int[] towerValue = new int[1];

    private NpcData mCurData {
        get { return (NpcData)this.mData; }
    }

    public bool immuneToSlow = false; //减速免疫

    //---------------------------------------------------
    /* 物攻 */
    public override float PAttack() { return 0; }
    /** 魔攻 */
    public override float MAttack() { return 0; }
    /** 物防 */
    public override float PDefence() { return 0; }
    /** 魔防 */
    public override float MDefence() { return 0; }
    /** 暴击 */
    public override float Bang() { return 0; }
    /** 韧性 */
    public override float Toughness() { return 0; }
    /** 命中 */
    public override float Hit() { return 0; }
    /** 闪避 */
    public override float Duck() { return 0; }
    //---------------------------------------------------

    public string Name()
    {
        return this.mCurData.config.Name;
    }


    public void SetBuildState(BuildState p_buildState)
    {
        buildState = p_buildState;
    }

    public bool CanAttack(CharacterBase entity)
    {
        if (!base.CanAttack(entity))
        {
            return false;
        }

        if (this.IsBuilt())
        { 
            return false;
        }

        return true;
    }

    public bool IsBuilt()
    {
        return buildState == BuildState.buildOver;
    }

    public NpcData CurData
    {
        get { return mCurData; }
    }

   

    public float GetSpeed()
    {
        return  this.mapData.moveSpeedFactor;
    }
    public override void CreateBody()
    {
      
    }

    public override void OnInit(CharacterData _data)
    {
        
        base.OnInit(_data);
        this.SearchMode = ESearchTargetType.OnlyMonster;
        this.AttackMode = EAttackMode.Country;
        this.attrConfig = this.CurData.attrConfig;

        maskTarget = LayerManager.layerManager.GetMask(LayerManager.layerManager.layerCreep);
        type = _TowerType.AOETower;
        SetBuildState(BuildState.building);
    }


    protected override void RegisterComponet()
    {
        base.RegisterComponet();

        this.AddComponent<ScanTargetComponent>();
        this.AddComponent<RoutineComponent>();
        this.AddComponent<AudioComponent>();
        this.AddComponent<BufferStateComponent>();
        this.AddComponent<BehaviourComponent>();
        this.AddComponent<SkillHolderComponent>();
        
        if (_TowerType.ResourceTower == type)
        {
            this.AddComponent<ResOutPutComponent>();
        }
    }
    
    //被眩晕
    public bool IsStunned()
    {
        return buffSateMgr.HasState(StateType.Stunned);
    }

    public override void OnUpdate(float dt)
    {
        base.OnUpdate(dt);
    }


    public void SetPlatform(Platform platform, Node node)
    {
        occupiedPlatform = new OccupiedPlatform(platform, node);

        BufferStateData bsData = new BufferStateData();
        bsData.type = StateType.Birth;
        buffSateMgr.AddState(bsData);

        SetBuildState(BuildState.buildOver);
    }

    public void ApplyDamage(float dmg)
    {
        
        Debug.Log("受到伤害:" + dmg * 0.2f + " 剩余血量:" + this.HP);

        this.SetHP(this.HP - dmg * 0.2f);

        base.GetHurt(null,dmg);
        
    }


    public void Stunned()
    {
        base.Stunned();
        behaviourMgr.StopAnimation();
    }

    public void Unstunned()
    {
        base.Unstunned();
        behaviourMgr.PlayAnimation();
    }

    public void InitTower(uint ID)
    {
        towerID = ID;
        this.ClientPos = this.Trans.position;
        //if (targetMode == _TargetMode.Hybrid)
        //{
        //    LayerMask mask1 = 1 << LayerManager.LayerCreep();
        //    LayerMask mask2 = 1 << LayerManager.LayerCreepF();
        //    maskTarget = mask1 | mask2;
        //}
        //else if (targetMode == _TargetMode.Air) maskTarget = 1 << LayerManager.LayerCreepF();
        //else if (targetMode == _TargetMode.Ground) maskTarget = 1 << LayerManager.LayerCreep();

        //if (shootObject != null)
        //{
        //    ObjectPoolManager.New(shootObject, 2);
        //}

        //foreach (TowerStat stat in upgradeStat)
        //{
        //    if (stat.shootObject != null)
        //    {
        //        ObjectPoolManager.New(stat.shootObject, 2);
        //    }
        //}

        //if (type == _TowerType.TurretTower)
        //{
        //    //calculate turret offset if this tower uses a projectile with elevated shoot angle
        //    ShootObject shootObj = shootObject.GetComponent<ShootObject>();
        //    if (shootObj.type == _ShootObjectType.Projectile)
        //    {
        //        turretMaxAngle = shootObj.maxShootRange;
        //        turretMaxRange = shootObj.maxShootRange;
        //    }

        //    StartCoroutine(ScanForTarget());
        //    StartCoroutine(TurretRoutine());
        //}
        //else if (type == _TowerType.DirectionalAOETower)
        //{
        //    StartCoroutine(ScanForTarget());
        //    StartCoroutine(DirectionalAOERoutine());
        //}
        //else if (type == _TowerType.AOETower)
        //{
        //    StartCoroutine(AOERoutine());
        //}
        //else if (type == _TowerType.SupportTower)
        //{
        //    StartCoroutine(SupportRoutine());
        //}
        //else if (type == _TowerType.ResourceTower)
        //{
        //    StartCoroutine(ResourceRoutine());
        //}
        //else if (type == _TowerType.Mine)
        //{
        //    StartCoroutine(MineRoutine());
        //}

        //level = 1;
        //StartCoroutine(Building(baseStat.buildDuration, false));

        //if turret is not animating, then enable turret shoot under all circumstance
        //else turret can only shoot when facing target
        //if (animateTurret == _TurretAni.None) targetInLOS = true;

        //if (buildAnimationBody != null && buildAnimation != null)
        //{
        //    buildAnimationBody.AddClip(buildAnimation, buildAnimation.name);
        //    buildAnimationBody.Play(buildAnimation.name);
        //}
    }

    public bool IsLevelCapped()
    {
        if (CurData.level < 255) return false;
        else return true;
    }

    public ResItem[] GetTowerSellValue()
    {
        List<ResItem> list = new List<ResItem>();
        float ratio = GameControl.GetSellTowerRefundRatio();
        list.Add(new ResItem(100, (int)(10 * ratio)));
        return list.ToArray();
    }

    public void Select()
    {
        //rangeIndicator.renderer.enabled=true;
        Debug.LogError("被选中");
    }

    public void Unselect()
    {
        // rangeIndicator.renderer.enabled = false;
        Debug.LogError("取消选中");
    }


    public override void OnDestroy()
    {
        base.OnDestroy();
    }
}



public class TowerMapData : EntityData
{
    public UnitAttribute HPAttribute;
    public float rotateSpeed = 10;          //旋转速度
    public float moveSpeed;                 //移动速度
    public float currentMoveSpd;            //当前移动速度
    public bool flying = false;             //是否飞行
    public Vector3 dynamicOffset;           //动态偏移
    public float flightHeightOffset = 3f;   //飞行高度偏移
    public float slowModifier = 1.0f;       //减速系数
    private float rotateSpd = 10;           //旋转速度
    public float moveAnimationModifier = 1;
    public bool wpMode = false;
    public float moveSpeedFactor = 1;
}