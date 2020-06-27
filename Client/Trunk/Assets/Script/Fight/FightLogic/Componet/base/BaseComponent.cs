﻿using System.Collections;
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
            this.Owner.comps.Add(this);
        }

        //public void RequestAction(SysAction action)
        //{
        //    NineScreenMgr.Instance.RequestAction(action);
        //}

        public virtual void OnUpdate(float nowTime)
        {

        }

        public virtual void OnDestroy()
        {

        }
    }
}