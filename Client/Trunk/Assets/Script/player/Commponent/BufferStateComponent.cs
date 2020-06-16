using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StateType
{
    Idle = 1,           //待机
    Walk = 2,           //行走
    Run = 3,            //跑步
    Hit = 4,            //攻击
    Death = 5,          //死亡
    Stunned = 6,        //眩晕
    Slow = 7,           //减速
    Birth = 8,          //出生
    DotDamage = 9,      //点伤害
}

public enum SkillType
{ 
    PointAttack = 1,  //点对点攻击，射击、刀砍、
}


public class BufferStateData
{
    public CharacterBase attacker;
    public StateType type;
    public float stunDuration;  //眩晕时间
    public Slow slow;           //减速
    public Dot dot;             //点伤害
}


public class BufferStateComponent : ComponentBase {

    public Dictionary<StateType, BufferState> curBufferMap = new Dictionary<StateType, BufferState>();
    public List<BufferState> curBufferList = new List<BufferState>();
    public void OnInit(CharacterBase p_owner)
    {
        base.OnInit(p_owner);
    }

    public void AddState(BufferStateData data)
    {
        if (!curBufferMap.ContainsKey(data.type))
        {
            BufferState bufferState = BufferFactory.GetState(data.type, data);
            curBufferMap.Add(data.type, bufferState);
            bufferState.OnEnter(this,data);
        }
        else
        {
            curBufferMap[data.type].RefreshData(data);
        }
        ActionState(data);
    }

    private void ActionState(BufferStateData data)
    {
        if (data.type == StateType.Birth)
        {

        }
        else if (data.type == StateType.Death)
        {
            BehaviourComponent comp = this.Owner.GetComponent<BehaviourComponent>();
            comp.ChangeState<BehaviourState_Die>();
        }
        else if (data.type == StateType.Stunned)
        {

        }
        else if (data.type == StateType.Slow)
        {

        }
        else if (data.type == StateType.DotDamage)
        {

        }

    }

    public void RemoveState(StateType type)
    {
        if (type == StateType.Birth)
        {
            //do something......
        }
        else if (type == StateType.Death)
        {

        }
        else if (type == StateType.Stunned)
        {

        }
        else if (type == StateType.Slow)
        {

        }
        else if (type == StateType.DotDamage)
        {

        }
        if (this.curBufferMap.ContainsKey(type))
        {
            this.curBufferList.Remove(this.curBufferMap[type]);
            this.curBufferMap[type].OnLeave();
            this.curBufferMap.Remove(type);
        }
    }

    public bool HasState(StateType type)
    {
        return curBufferMap.ContainsKey(type);
    }


    public override void OnUpdate(float dt)
    {
        base.OnUpdate(dt);
        for (int i = 0; i < curBufferList.Count; i++)
        {
            curBufferList[i].OnUpdate();
        }
    }

    public void OnDestroy()
    {

    }

}


public class BufferFactory
{
    public  delegate BufferState OnStateCall(BufferStateData state);
    public static Dictionary<StateType, OnStateCall> StateMap = new Dictionary<StateType, OnStateCall>();

    public static void RegisterState()
    {
        StateMap.Add(StateType.Death, (state) => { return new BufferState_Death(); });
        StateMap.Add(StateType.Stunned, (state) => { return new BufferState_Stun(); });
        StateMap.Add(StateType.Slow, (state) => { return new BufferState_Slow(); });
        StateMap.Add(StateType.Birth, (state) => { return new BufferState_Birth(); });
        StateMap.Add(StateType.DotDamage, (state) => { return new BufferState_DotDamage(); });
    }

    public static BufferState GetState(StateType type, BufferStateData data)
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
