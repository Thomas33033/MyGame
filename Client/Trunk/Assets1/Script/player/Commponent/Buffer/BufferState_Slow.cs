using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 减速状态或者加速状态 系数大于1为减速，系数小于1为加速
/// </summary>
public class BufferState_Slow : BufferState 
{
    private bool slowRoutine = false;
    private List<Slow> slowEffect = new List<Slow>();
    private float _slowModifier = 1f;
    private float slowModifier
    {
       get{ return _slowModifier;}
       set
       {
            slowModifier = value;
            this.mgr.Owner.SetSpeedFactor(slowModifier);
       }
    }

    public override void OnEnter(BufferStateComponent mgr, BufferStateData data)
    {
        base.OnEnter(mgr,data);
        Debug.Log(this.mgr.Owner.Trans.name + "  被减速 ");
        this.ApplySlow(data.slow);
    }

    public void ApplySlow(Slow slow)
    {
        bool immuned = false;
        if (this.mgr.Owner is Monster) { immuned = ((Monster)this.mgr.Owner).immuneToSlow; }
        else if (this.mgr.Owner is Tower) { immuned = ((Tower)this.mgr.Owner).immuneToSlow; }

        if (!immuned)
        {
            slow.SetTimeEnd(Time.time + slow.duration);
            
            slowEffect.Add(slow);
            if (!slowRoutine)
            {
                slowRoutine = true;
            }
        }
    }


    public override void OnUpdate()
    {
        base.OnUpdate();
        if (slowRoutine)
        {
            if (slowEffect.Count > 0)
            {
                float targetVal = 1.0f;
                for (int i = 0; i < slowEffect.Count; i++)
                {
                    Slow slow = slowEffect[i];

                    //check if the effect has expired
                    if (Time.time >= slow.GetTimeEnd())
                    {
                        slowEffect.RemoveAt(i);
                        i--;
                    }
                    else if (1 - slow.slowFactor < targetVal)
                    {
                        //if the effect is not expire, check the slowFactor
                        //record the val if the slowFactor is slower than the previous entry
                        targetVal = 1 - slow.slowFactor;
                    }
                }
                slowModifier = Mathf.Lerp(slowModifier, targetVal, Time.deltaTime * 10);
            }

            if (slowEffect.Count == 0)
            {
                slowRoutine = false;
                slowModifier = Mathf.Lerp(slowModifier, 1, Time.deltaTime * 10);
                //buffer过期，自动销毁
                this.mgr.RemoveState(this.curData.type);
            }
        }
    }

    public override void OnLeave()
    {
        base.OnLeave();
    }
}
