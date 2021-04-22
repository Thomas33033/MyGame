using UnityEngine;
using System.Collections.Generic;


public class FightAttackRender : MonoBehaviour
{
    [HideInInspector]
    public RoleRender attacker;

    [HideInInspector]
    public List<RoleRender> listTargets;

    public float lifeTime;
    public GameObject objContent;
    public float timeDelayContent;
    public GameObject objHit;
    public float hitTime = 1f;

    protected float timeDie;
    protected float timeCreate;
    protected float timeLastFrame;
    protected float timeDelta;
    protected float timePass;
    protected float timeScale;

    protected bool isActive;
    protected bool hasHit = true;
    //public List<Animator> listSkeleton;

    protected EffectPoolObj poolObj;

    //缓存初始参数
    private float mLocalTimeDelayContent;

    private Vector3 mContentPosition;


    private GameObject tObjHit;
    private void Awake()
    {
        mLocalTimeDelayContent = timeDelayContent;
        if (this.objContent != null)
        {
            mContentPosition = this.objContent.transform.localPosition;
        }
        OnInitEffect();
    }

    //初始化特效，返回对象池时还原特效。
    protected virtual void OnInitEffect()
    {
        if (objContent != null)
        {
            objContent.transform.localPosition = mContentPosition;
            if (mLocalTimeDelayContent > 0)
            {
                objContent.SetActive(false);
            }
        }

        if (objHit != null)
        {
            objHit.SetActive(false);
        }
    }

    public virtual void Update()
    {
        if (!isActive)
        {
            return;
        }

        float now = FightSceneRender.Instance.GetTime();
        timeDelta = now - timeLastFrame;
        timePass = now - timeCreate;
        timeLastFrame = now;

        if (objContent != null)
        {
            if (objContent.activeSelf == false && now > timeCreate + timeDelayContent)
            {
                objContent.SetActive(true);
            }
        }

        if (lifeTime != 0 && now - timeCreate > lifeTime)
        {
            isActive = false;
            Die();
        }
    }

    public virtual void SetData(EffectPoolObj poolObj, RoleRender attacker, 
        List<RoleRender> listTargets, float dieTime = 0, float timeScale = 1f)
    {
        this.poolObj = poolObj;
        this.attacker = attacker;
        this.listTargets = listTargets;

        this.timeCreate = FightSceneRender.Instance.GetTime();
        this.timeLastFrame = timeCreate;
        this.timeDie = dieTime;
        this.timeDelayContent *= 1f / timeScale;

        if (lifeTime == 0)
        {
            lifeTime = 0.02f;
        }

        this.timeScale = timeScale;

        isActive = true;
    }

    protected virtual void Die()
    {
        if (objHit != null && hasHit && listTargets.Count > 0)
        { 
            tObjHit = GameObject.Instantiate(objHit);
            tObjHit.transform.Reset();
            tObjHit.transform.position = this.transform.position;
            tObjHit.SetActive(true);
            
            if (listTargets.Count > 0)
            {
                transform.position = listTargets[0].transform.position;
            }
            Invoke("ReturnPool", hitTime);
            if (objContent != null)
            {
                objContent.SetActive(false);
            }
        }
        else
        {
            ReturnPool();
        }
    }

    protected virtual void ReturnPool()
    {
        GameObject.Destroy(tObjHit);
        OnInitEffect();
        if (poolObj != null)
        {
            poolObj.ReturnToPool();
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
