//================================================
// auth：xuetao
// date：2018/5/31 21:17:55
//================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class SkillEffect
{
    public SkillClipData skillData;
    public SkillHolderComponent mgr;

    public virtual void OnEnter(SkillHolderComponent p_mgr, SkillClipData p_data)
    {
        this.mgr = p_mgr;
        this.skillData = p_data;
    }

    public virtual void OnLeave()
    { 
    
    }

}

