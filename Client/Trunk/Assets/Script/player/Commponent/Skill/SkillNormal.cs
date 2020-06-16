//================================================
// auth：xuetao
// date：2018/5/31 21:18:37
//================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
/// <summary>
/// 普通攻击效果，一对一攻击
/// </summary>
class SkillNormal : SkillEffect
{
    
    public override void OnEnter(SkillHolderComponent mgr, SkillClipData data)
    {
        base.OnEnter(mgr,data);
        this.AttackTarget();
    }

   
    public void AttackTarget()
    {
        //动作和声音绑定在一起，播放动作时并播放音效
        BehaviourComponent bhmgr = this.mgr.Owner.GetComponent<BehaviourComponent>();
        if (bhmgr != null)
        {
            if (!(bhmgr.CurState is BehaviourState_Skill))
                bhmgr.ChangeState<BehaviourState_Skill>();
            BehaviourState_Skill bhs = bhmgr.CurState as BehaviourState_Skill;
            bhs.Shoot(this.skillData);
        }
    }

    public override void OnLeave()
    {
        base.OnLeave();
    }
}

