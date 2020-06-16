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

    public List<Animator> listSkeleton;

    protected EffectPoolObj poolObj;

    //缓存初始参数
    private float mLocalTimeDelayContent;

    private Vector3 mContentPosition;

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

        float now = FightSence.Instance.GetTime();
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

        this.timeCreate = FightSence.Instance.GetTime();
        this.timeLastFrame = timeCreate;
        this.timeDie = dieTime;
        this.timeDelayContent *= 1f / timeScale;

        if (lifeTime == 0)
        {
            lifeTime = 0.02f;
        }

        this.timeScale = timeScale;

        //if (timeScale != 1f)
        //{
        //    if (listSkeleton != null && listSkeleton.Count > 0)
        //    {
        //        for (int i = 0; i < listSkeleton.Count; i++)
        //        {
        //            listSkeleton[i].timeScale = timeScale;
        //        }
        //    }
        //}

        isActive = true;
    }

    protected virtual void Die()
    {
        if (objHit != null)
        {
            objHit.SetActive(true);
            if (listTargets.Count > 0)
            {
                transform.position = listTargets[0].transform.position;
            }
            Invoke("ReturnPool", hitTime);
            //Destroy(gameObject, 1f);
            if (objContent != null)
            {
                objContent.SetActive(false);
            }
        }
        else
        {
            ReturnPool();
            //Destroy(gameObject);
        }
    }

    protected virtual void ReturnPool()
    {
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
