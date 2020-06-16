using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fight
{
    public class BaseComponent
    {
        public Role Owner;

        public BaseComponent(Role p_owner)
        {
            this.Owner = p_owner;
        }

        //public void RequestAction(SysAction action)
        //{
        //    NineScreenMgr.Instance.RequestAction(action);
        //}

        public virtual void OnUpdate(float dt)
        {

        }

        public virtual void OnDestroy()
        {

        }
    }
}