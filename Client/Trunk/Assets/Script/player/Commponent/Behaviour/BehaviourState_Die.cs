using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourState_Die : BehaviourState
{
    public override void OnEnter(BehaviourComponent mgr)
    {
        base.OnEnter(mgr);
        this.Etype = EBehaviourState.Die;
        PlayAnimation();
    }

    public float GetDuration()
    {
        return 3;
    }

    //播放死亡动画
    private void PlayAnimation()
    {
        Debug.LogError(this.mgr.Owner.Trans.name + "  死亡 ");

        float dieDuration = this.GetDuration();
        this.mgr.PlayClip("Death");
        //清除尸体
        this.mgr.Owner.AddSchedule(dieDuration, () =>
        {
            this.mgr.Owner.OnDelete();
        });
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
