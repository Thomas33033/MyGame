using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FightSCAttackHitDirection : FightSCAttackHit
{
    public override void SetData(EffectPoolObj poolObj, RoleRender attacker, List<RoleRender> listTargets, float dieTime = 0, float timeScale = 1)
    {
        base.SetData(poolObj, attacker, listTargets, dieTime, timeScale);

        float angle = attacker.transform.position.x < listTargets[0].transform.position.x ? 0 : 180f;

        this.transform.localEulerAngles = new UnityEngine.Vector3(0, angle, 0);
    }
}