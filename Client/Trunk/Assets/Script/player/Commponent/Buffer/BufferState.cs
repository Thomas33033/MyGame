using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BufferState 
{
    protected BufferStateComponent mgr;
    protected BufferStateData curData;

    public void RefreshData(BufferStateData data)
    {
        this.curData = data;
    }

    // Use this for initialization
    public virtual void OnEnter (BufferStateComponent p_mgr, BufferStateData data)
    {
        this.mgr = p_mgr;
        this.curData = data;
    }
	
	// Update is called once per frame
    public virtual void OnUpdate()
    {
		
	}

    public virtual void OnLeave()
    {

    }
}
