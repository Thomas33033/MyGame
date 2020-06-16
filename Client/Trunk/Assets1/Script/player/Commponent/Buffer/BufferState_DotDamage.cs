using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 点伤害
/// </summary>
public class BufferState_DotDamage : BufferState 
{
    private bool mPlay = false;
    private Dot dot;
    private float timeStart;
    private float interval;
    public override void OnEnter(BufferStateComponent mgr, BufferStateData data)
    {
        base.OnEnter(mgr, data);
        dot = data.dot;
        
        ApplayDot();
    }

    public void ApplayDot()
    {
        timeStart = Time.time;
        mPlay = true;
            
        interval = 0;


    }

    public override void OnUpdate()
    {
        if (mPlay)
        {
            if (Time.time - timeStart < dot.duration)
            {
                interval -= Time.deltaTime;
                if (interval < 0)
                {
                    this.mgr.Owner.GetHurt(this.curData.attacker,dot.damage);
                    interval = dot.interval;
                }
            }
            else
            {
                //移除状态
                this.mgr.RemoveState(this.curData.type);
            }
        }
    }

    public override void OnLeave()
    {
        base.OnLeave();
    }
}
