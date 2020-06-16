//================================================
// auth：xuetao
// date：2018/5/31 21:17:12
//================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class SkillHolderComponent : ComponentBase
{
    public override void OnInit(CharacterBase p_owner)
    {
        base.OnInit(p_owner);
    }

    public override void OnUpdate(float dt)
    {
        base.OnUpdate(dt);
    }

    //释放技能
    public void CastSkill(SkillClipData skillClipData)
    {
        SkillType type = SkillType.PointAttack;
        SkillEffect skillEffect = SkillFactory.GetState(type, skillClipData);
        skillEffect.OnEnter(this, skillClipData);
    }
}

public class SkillFactory
{
    public delegate SkillEffect OnStateCall(SkillClipData state);
    public static Dictionary<SkillType, OnStateCall> StateMap = new Dictionary<SkillType, OnStateCall>();

    public static void RegisterState()
    {
        StateMap.Add(SkillType.PointAttack, (state) => { return new SkillNormal(); });
    }

    public static SkillEffect GetState(SkillType type, SkillClipData data)
    {
        if (StateMap.Count == 0) RegisterState();
        if (StateMap.ContainsKey(type))
        {
            return StateMap[type](data);
        }
        else
        {
            Debug.LogError("not find type :" + type.ToString());
        }
        return null;
    }
}