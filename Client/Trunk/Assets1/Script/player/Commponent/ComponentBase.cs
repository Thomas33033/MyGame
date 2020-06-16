using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComponentBase
{
    public CharacterBase Owner;
    public virtual void OnInit(CharacterBase p_owner)
    {
        this.Owner = p_owner;
    }

    public void RequestAction(SysAction action)
    {
        NineScreenMgr.Instance.RequestAction(action);
    }

    public virtual void OnUpdate(float dt)
    { 
    
    }

    public virtual void OnDestroy()
    { 
    
    }
}
