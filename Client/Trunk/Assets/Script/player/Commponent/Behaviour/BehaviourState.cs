using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum EBehaviourState
{
    Idle = 1,
    Run = 2,
    Die = 9
}

public class BehaviourState
{
    public EBehaviourState Etype;
    protected BehaviourComponent mgr;
    
    public virtual void OnEnter (BehaviourComponent mgr) 
    {
        this.mgr = mgr;
	}


    public virtual void OnUpdate() 
    {
		
	}

    public virtual void OnLeave()
    { 
    
    }

    public virtual void OnReset()
    { 
    
    }
}
