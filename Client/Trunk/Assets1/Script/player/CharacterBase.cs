using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CharacterBase : Entity
{

    private Quaternion rotation;
    private Vector3 position;

    protected CharacterData mData;
    private Transform mTrans;

    private Vector3 m_ClientPos;
    private float m_moveSpeed;
    private float m_RotationSpeed;
    private ESearchTargetType m_searchMode;
    private EAttackMode m_attactMode;

    public bool canDelete = false;

    private ComponentMgr mComponentMgr;
    private UIComponent mUIComponent;
    protected BufferStateComponent bufferStateComponent;

    private List<ModelPoolObj> poolList = new List<ModelPoolObj>();
    

    protected virtual void RegisterComponet() {
        this.AddComponent<UIComponent>();
    }


    //---------------------基础属性 开始--------------------
    public Transform Trans { 
        get { return mTrans; } 
         set { mTrans = value; } 
    }

    public GameObject ModelObj
    {
        get {
            if (this.Trans == null)
                return null;
            return this.Trans.gameObject;
        }
    }

    public Quaternion Rotation
    {
        get{ 
            return this.rotation;
        }
        set{
            this.rotation = value;
            if(this.Trans != null)
            {
                this.Trans.rotation = value;
            }
            
        }
       
    }

    public Vector3 ClientPos
    {
        get { return this.m_ClientPos; }
        set { this.SetClientPoint(value); }
    }

    private void SetClientPoint(Vector3 point)
    {
        this.m_ClientPos = point;
        if (this.mTrans != null) {
            this.mTrans.position = point;
        }
    }

    public virtual EEntityType GetEntityType()
    {
        return (EEntityType)this.mData.type;
    }

    public void PushObject(ModelPoolObj poolObj)
    {
        this.poolList.Add(poolObj);
    }

    public CfgSkillData GetSkillData()
    {
        return this.mData.skillData;
    }

    //---------------------基础属性 结束--------------------

    //-----------------------------------------------
    public float MoveSpeed
    {
        get { return m_moveSpeed; }
        set { this.m_moveSpeed = value; }
    }

    public float HP { get { return this.mData.hp; } }

    public float MaxHP { get { return this.mData.maxHp; } }

    public void SetHP(float value)
    {
        this.mData.hp = value;
        if (this.mUIComponent != null)
            this.mUIComponent.RefreshHP();
    }

    public void SetSpeedFactor(float speedFactor)
    {
        this.mData.speedFactor = speedFactor;
        this.m_moveSpeed = speedFactor * this.mData.baseSpeed;
    }

    public float SpeedFactor()
    {
        return this.mData.speedFactor;
    }

    public EAttackMode AttackMode
    {
        get { return this.m_attactMode; }
        set { this.m_attactMode = value; }
    }

    public ESearchTargetType SearchMode
    {
        get { return this.m_searchMode; }
        set { this.m_searchMode = value; }
    }


    public float RotationSpeed
    {
        get { return this.m_RotationSpeed; }
        set { this.m_RotationSpeed = value; }
    }


    public bool CanWork()
    {
        if (this.buffSateMgr.HasState(StateType.Stunned))
            return false;
        if (this.buffSateMgr.HasState(StateType.Death))
            return false;
        return true;
    }

    public bool CanMove()
    {
        if (this.buffSateMgr.HasState(StateType.Stunned))
            return false;
        if (this.buffSateMgr.HasState(StateType.Death))
            return false;
        return true;
    }

    public void OnBirth()
    {

    }

    public virtual Vector3 GetOffset()
    {
        return Vector3.one;
    }

    //---------------------状态---------------------
    /**停止移动*/
    protected void OnStopMove() { }

    /**响应受击*/
    private void OnPlayHit()
    {

    }

    public bool IsLive()
    {
        bool dead = buffSateMgr.HasState(StateType.Death);
        return !dead;
    }

    public virtual void ReachDestination() { }

    /**眩晕*/
    public void Stunned() { }

    /**解除眩晕*/
    public void Unstunned() { }

    //名称
    public string Name() { return ""; }
    //飞行高度
    public float FlyHeight() { return 0; }

    //攻击间隔
    public float GetCooldown() { return 10; }

    //-----------------战斗数值---------------------
    /* 物攻 */
    public virtual float PAttack() { return 0; }
    /** 魔攻 */
    public virtual float MAttack() { return 0; }
    /** 物防 */
    public virtual float PDefence() { return 0; }
    /** 魔防 */
    public virtual float MDefence() { return 0; }
    /** 暴击 */
    public virtual float Bang() { return 0; }
    /** 韧性 */
    public virtual float Toughness() { return 0; }
    /** 命中 */
    public virtual float Hit() { return 0; }
    /** 闪避 */
    public virtual float Duck() { return 0; }
    /** 等级 */
    public int Level() { return 1; }
    //--------------------------------------


    /// <summary>
    /// 是否可以发起攻击
    /// 如果目标不存在，检测自己本身是否可以攻击
    /// 如果目标存在，则检测是否可以攻击该目标
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public bool CanAttack(CharacterBase entity)
    {
        if (entity == null) return false;

        //死亡不能攻击
        if (!this.IsLive()) return false;

        //被眩晕不能攻击
        if (this.buffSateMgr.HasState(StateType.Stunned))
            return false;

        return true;
    }

    /// <summary>
    /// 目标是否在攻击范围内
    /// </summary>
    /// <returns></returns>
    public bool TargetInAttackRange(CharacterBase target)
    {
        if (target == null) return false;

        return MathfHelper.PixelDistance(this.ClientPos, target.ClientPos) > 20;
    }

    /// <summary>
    /// 愤怒值已满
    /// </summary>
    /// <returns></returns>
    public bool IsAngry()
    {
        return false;
    }


    public virtual void CreateBody() {}

    protected void SetBody(GameObject body)
    {
        this.mTrans = body.transform;
        this.mTrans.position = this.mData.position;
        this.mTrans.rotation = this.mData.rotation;
        this.behaviourMgr.RefreshState();
    }

    public virtual void OnInit(CharacterData _data)
    {
        this.mData = _data;
        mComponentMgr = new ComponentMgr(this);
        this.RegisterComponet();

        this.mUIComponent = this.GetComponent<UIComponent>();
        this.bufferStateComponent = this.GetComponent<BufferStateComponent>();

        this.SetHP(_data.hp);

        this.MoveSpeed = _data.baseSpeed * _data.speedFactor;

        this.CreateBody();
    }
    
    public virtual void OnUpdate(float dt)
    {
        base.OnUpdate(dt);
        if (!this.IsLive())
        {
            return;
        }
        mComponentMgr.OnUpdate(dt);
    }

    //----------------------获取组件-----------------------------------
    public BufferStateComponent buffSateMgr
    {
        get { return GetComponent<BufferStateComponent>(); }
    }

    public BehaviourComponent behaviourMgr
    {
        get { return GetComponent<BehaviourComponent>(); }
    }

    public T GetComponent<T>() where T : ComponentBase
    {
        return mComponentMgr.GetComponent<T>();
    }

    public T AddComponent<T>() where T : ComponentBase, new()
    {
        return mComponentMgr.AddComponent<T>();
    }

    //------------------------------------------------------
    /**被攻击受到伤害*/
    public void GetHurt(CharacterBase attacker, float dmg)
    {
        bool dead = buffSateMgr.HasState(StateType.Death);
        if (!dead)
        {
            this.OnPlayHit();
        }
        this.mData.hp -= dmg;

        if (this.mData.hp <= 0 )
        {
            this.mData.hp = 0;
        }
        this.SetHP(this.mData.hp);
    }

    public virtual void OnDie()
    {
    }

    public void OnDelete()
    {
        this.canDelete = true;
        EntitesManager.Instance.RemoveCharactor(this.uid);
    }

    //删除尸体，包括:玩家模型、绑定特效、
    public virtual void OnDestroy()
    {
        Debug.LogError("=======TrueDestroy======== " + this.uid);
        //清除玩家组件
        this.mComponentMgr.OnDispose();

        //释放资源进缓冲池
        for(int i = 0 ,length = this.poolList.Count; i < length; i++)
        {
            this.poolList[i].ReturnToPool();
        }
        this.poolList.Clear();
        this.mComponentMgr = null;
    }

}
