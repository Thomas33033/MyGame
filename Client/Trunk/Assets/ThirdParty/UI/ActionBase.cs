using UnityEngine;
using System.Collections.Generic;

public class ActionBase
{
    protected GameObject mGameObject;

    public virtual void Update(float elapsed)
    {

    }

    public virtual void Start(GameObject gameObject)
    {
        mGameObject = gameObject;
    }

    public virtual bool IsDone()
    {
        return true;
    }
}

public class ActionSequence : ActionBase
{
    List<ActionBase> mActionList = new List<ActionBase>();
    int mActionIndex;
    ActionBase mCurAction;

    public void ClearAction()
    {
        mCurAction = null;
        mActionList.Clear();
    }

    public void AddAction(ActionBase actionBase)
    {
        mActionList.Add(actionBase);
    }

    public override void Update(float elapsed)
    {
        if (mCurAction == null)
            return;

        mCurAction.Update(elapsed);
        if (mCurAction.IsDone())
        {
            if (mActionIndex == mActionList.Count - 1)
            {
                mCurAction = null;
                return;
            }
            else
            {
                mCurAction = mActionList[++mActionIndex];
                mCurAction.Start(mGameObject);
            }
        }
    }

    public override void Start(GameObject gameObject)
    {
        mGameObject = gameObject;
        if (mActionList.Count > 0)
        {
            mActionIndex = 0;
            mCurAction = mActionList[mActionIndex];
            mCurAction.Start(mGameObject);
        }
    }

    public override bool IsDone()
    {
        return mCurAction == null;
    }
}

public class ActionPlayParticle : ActionBase
{
    ParticleSystem mParticle;
    public ActionPlayParticle(ParticleSystem particle)
    {
        mParticle = particle;
    }
    public override void Start(GameObject gameObject)
    {
        mParticle.Play();
    }
}


public class ActionShowNumber : ActionBase
{

}

public class ActionVisible : ActionBase
{
    bool mVisible;
    public ActionVisible(bool visible)
    {
        mVisible = visible;
    }
    public override void Start(GameObject gameObject)
    {
        if (gameObject.GetComponent<Renderer>() != null)
        {
            gameObject.GetComponent<Renderer>().enabled = mVisible;
        }
    }
}

public class ActionInterval : ActionBase
{
    protected float mDuration;
    protected float mElapsed;

    public ActionInterval(float duration)
    {
        mDuration = duration;
        mElapsed = 0;
    }

    public override bool IsDone()
    {
        return mElapsed >= mDuration;
    }

    public override void Start(GameObject gameObject)
    {
        base.Start(gameObject);
        mElapsed = 0;
    }
}

public class ActionJump : ActionInterval
{
    Vector3 mStartPos;
    Vector3 mTargetPos;
    float mHeight;
    int mTimes;
    int mDirection;

    public ActionJump(float duration, float height, int times)
        :base(duration)
    {
        mHeight = height;
        mTimes = times;
    }

    public override void Update(float elapsed)
    {
        mElapsed += elapsed;
        if (mElapsed > mDuration)
        {
            mElapsed = mDuration;
        }

        int curTimes = (int)(mElapsed * mTimes * 2 / mDuration);
        float curElapsed = mElapsed * mTimes * 2 % mDuration;
        if (curTimes%2 == 0)
        {
            mGameObject.transform.position = Vector3.Lerp(mStartPos, mTargetPos, curElapsed / mDuration);    
        }
        else
        {
            mGameObject.transform.position = Vector3.Lerp(mTargetPos, mStartPos, curElapsed / mDuration);                
        }
    }

    public override void Start(GameObject gameObject)
    {
        base.Start(gameObject);
        mTargetPos = mStartPos = mGameObject.transform.position;
        mTargetPos.y += mHeight;
    }
}

public class ActionMove : ActionInterval 
{
    Vector3 mStartPos;
    Vector3 mTargetPos;

    public ActionMove(Vector3 pos, float duration)
        :base(duration)
    {
        mTargetPos = pos;
    }

    public override void Update(float elapsed)
    {
        mElapsed += elapsed;
        if (mElapsed > mDuration)
        {
            mElapsed = mDuration;
        }

        Vector3 newPos = Vector3.Lerp(mStartPos, mTargetPos, mElapsed / mDuration);
        mGameObject.transform.position = newPos;
    }

    public override void Start(GameObject gameObject)
    {
        base.Start(gameObject);
        mStartPos = mGameObject.transform.position;
    }
}

public class ActionDelay : ActionInterval 
{
    float mTime;

    public ActionDelay(float duration)
        :base(duration)
    {
    }

    public override void Update(float elapsed)
    {
        mElapsed += elapsed;
    }
}


