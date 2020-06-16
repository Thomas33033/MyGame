using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 眩晕状态
/// </summary>
public class BufferState_Stun : BufferState 
{
    private float mDuration;  //眩晕持续时间
    private bool mStunned = false;   //眩晕状态


    public override void OnEnter(BufferStateComponent mgr, BufferStateData data)
    {
        base.OnEnter(mgr,data);
        ApplyStun(data.stunDuration);
        Debug.Log(this.mgr.Owner.Trans.name + "  被眩晕 ");
    }

    public void ApplyStun(float stunTimes)
    {
        if (stunTimes > this.mDuration) this.mDuration = stunTimes;
        if (!this.mStunned)
        {
            this.mStunned = true;
            //让玩家眩晕
            this.mgr.Owner.Stunned();
        }
    }


    public override void OnUpdate()
    {
        if (this.mStunned)
        {
            if (this.mDuration > 0)
            {
                this.mDuration -= Time.deltaTime;
            }
            else
            {
                this.mStunned = false;
                //解除眩晕状态
                this.mgr.Owner.Unstunned();
            }
        }
    }

    public override void OnLeave()
    {
        base.OnLeave();
        //解除眩晕状态
        this.mgr.Owner.Stunned();
    }
}
