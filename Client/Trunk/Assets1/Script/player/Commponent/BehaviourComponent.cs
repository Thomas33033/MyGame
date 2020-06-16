using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AnimClipData
{
    public string clipName;
    public string soundName;
    public float soundStartTime;
}

public class BehaviourComponent : ComponentBase
{
    public AnimClipData idleClip;
    public AnimClipData RunClip;
    public AnimClipData HitClip;

    public Animator mAnimator;
    public Animator aniBody
    {
        get
        {
            if (mAnimator == null && this.Owner.ModelObj != null)
                mAnimator = this.Owner.ModelObj.GetComponent<Animator>();
            return mAnimator;
        }
    }

    private BehaviourState curState;
    public BehaviourState CurState
    {
        get
        {
            return curState;
        }
    }

    public override void OnInit(CharacterBase p_owner)
    {
        base.OnInit(p_owner);
        this.ChangeState<BehaviourState_Idle>();
    }

    public override void OnUpdate(float dt)
    {
        base.OnUpdate(dt);
    }

    public bool CanChangeState(EBehaviourState state)
    {
        if (this.curState == null)
        {
            return true;
        }

        if (state != this.curState.Etype)
        {
            return true;
        }

        return false;
    }

    public BehaviourState ChangeState<T>() where T : BehaviourState, new()
    {
        if (curState != null)
        {
            curState.OnLeave();
            PoolToos.FreeClass(curState);
        }
        curState = PoolToos.GetClass<T>();
        curState.OnEnter(this);
        return curState;
    }

    public void RefreshState()
    {
        if (this.curState != null)
        {
            this.curState.OnEnter(this);
        }
    }

    public void PlayClip(string clipName)
    {
        if (this.aniBody != null)
        {
            DebugMgr.LogError("clipName:"+clipName);
           aniBody.CrossFade(clipName, 0);
            //this.ForceCrossFade(this.aniBody, clipName, 0);
        }
            
    }

    public void ForceCrossFade(Animator animator, string name, float transitionDuration, int layer = 0, float normalizedTime = float.NegativeInfinity)
    {
        animator.Update(0);

        if (animator.GetNextAnimatorStateInfo(layer).fullPathHash == 0)
        {
            animator.CrossFade(name, transitionDuration, layer, normalizedTime);
        }
        else
        {
            animator.Play(animator.GetNextAnimatorStateInfo(layer).fullPathHash, layer);
            animator.Update(0);
            animator.CrossFade(name, transitionDuration, layer, normalizedTime);
        }
    }


    public void SetAnimationSpeed(float speed)
    {
        //if (aniBody != null && animationMove != null && animationMove.Length > 0)
        //{
        //    foreach (AnimationClip clip in animationMove)
        //    {
        //        aniBody.GetComponent<Animation>()[clip.name].speed = currentMoveSpd * moveAnimationModifier;
        //    }
        //}
    }

    public void StopAnimation()
    {
        Debug.Log("----眩晕----");

        if (aniBody != null)
        {
            //if (animationMove != null && animationMove.Length > 0)
            //{
            //    for (int i = 0; i < animationMove.Length; i++)
            //    {
            //        aniBody.Stop(animationMove[i].name);
            //    }
            //}
        }
    }

    public void PlayAnimation()
    {
        Debug.Log("----解除眩晕----");
        //if (aniBody != null && animationMove != null && animationMove.Length > 0)
        //{
        //    aniBody.Play(animationMove[Random.Range(0, animationMove.Length - 1)].name);
        //}
    }


    public override void OnDestroy()
    {
        base.OnDestroy();
        if (curState != null)
        {
            curState.OnLeave();
            PoolToos.FreeClass(curState);
        }
        
    }
}
