using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourState_Run : BehaviourState {

    public override void OnEnter(BehaviourComponent mgr)
    {
        base.OnEnter(mgr);
        this.Etype = EBehaviourState.Run;
        this.PlayAnimation();
    }

    //播放跑步动画
    private void PlayAnimation()
    {
        this.mgr.PlayClip("Run");
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
