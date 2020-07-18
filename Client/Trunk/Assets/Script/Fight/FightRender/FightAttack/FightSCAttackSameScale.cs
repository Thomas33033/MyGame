using System.Collections.Generic;

public class FightSCAttackSameScale : FightAttackRender
{
    public override void SetData(EffectPoolObj poolObj, RoleRender attacker, List<RoleRender> listTargets, float dieTime = 0, float timeScale = 1)
    {
        base.SetData(poolObj, attacker, listTargets, dieTime, timeScale);
        transform.position = attacker.transform.position;

        //float angle = (attacker as FightSCRoleFighter).skeletonAnimator.transform.localScale.x > 0 ? 1 : -1;

        //this.transform.localEulerAngles = new UnityEngine.Vector3(0, angle, 0);
        //this.transform.localScale = new UnityEngine.Vector3(angle, 1, 1);
    }
}