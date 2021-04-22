using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FightSCAttackDirection : FightAttackRender
{
    public override void SetData(EffectPoolObj poolObj, RoleRender attacker, List<RoleRender> listTargets, float dieTime = 0, float timeScale = 1)
    {
        base.SetData(poolObj, attacker, listTargets, dieTime, timeScale);

       transform.position = attacker.fromPoint;
       transform.rotation = attacker.transform.rotation;
    }
}