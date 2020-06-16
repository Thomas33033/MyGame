using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourState_Idle : BehaviourState 
{
    public override void OnEnter(BehaviourComponent mgr)
    {
        base.OnEnter(mgr);
        this.Etype = EBehaviourState.Idle;
        this.PlayAnimation();
    }

    //播放移动动画
    private void PlayAnimation()
    {
        this.mgr.PlayClip("Idle");
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
    }

    public override void OnLeave()
    {
        base.OnLeave();
    }
}
