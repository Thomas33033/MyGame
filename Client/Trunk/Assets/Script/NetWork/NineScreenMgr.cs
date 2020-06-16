using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//系统动作消息类
/// <summary>
/// 包含玩家移动、攻击、等所有操作
/// </summary>
public class SysAction
{
    public static int uid;
    public int index;
    public int skillId = 0;
    public OperateType operType;

    public int curId;
    public NpcType curType;
    public int targetId;
    public NpcType targetType;

    public SysAction()
    {
        index = uid++;
    }

    public SysAction(CharacterBase cur,CharacterBase target,OperateType type)
    {
        index = uid++;
        curId = cur.uid;
        targetId = target.uid;
        operType = type;
    }
}

public class NineScreenMgr : Singleton<NineScreenMgr>
{
    //单帧执行的动作的数量
    public int ActionNumInFrame = 10;
    public Queue<SysAction> ActionList = new Queue<SysAction>();

    public void Init()
    {
    }

    public void OnUpdate(float dt)
    {
        for (int i = 0; i < ActionNumInFrame; i++)
        {
            if (ActionList.Count <= 0)
            {
                break;
            }
           DoAction(ActionList.Dequeue());
        }
    }

    public void DoAction(SysAction action)
    {
       CharacterBase attacker = EntitesManager.Instance.GetEntity(action.curId);
       CharacterBase defender = EntitesManager.Instance.GetEntity(action.targetId);


       if (action.operType == OperateType.attack)  //处理技能消息
       {
           SkillManager.Instance.AttackTarget(attacker, defender, action.skillId);
       }
       else if (action.operType == OperateType.Run)
       {
           Debug.Log("Run");
       }

                
       //}

    }

    public void RequestAction(SysAction action)
    {
        ActionList.Enqueue(action);
    }
}
