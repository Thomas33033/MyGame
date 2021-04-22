using System.Collections.Generic;

public class FightSCAttackHit : FightAttackRender
{
    public bool needSetParent;

    public override void SetData(EffectPoolObj poolObj, RoleRender attacker, List<RoleRender> listTargets, float dieTime = 0, float timeScale = 1)
    {
        base.SetData(poolObj, attacker, listTargets, dieTime, timeScale);
        if (listTargets.Count > 0)
        {
            transform.position = listTargets[0].transform.position;
            if (needSetParent)
            {
                transform.SetParent(listTargets[0].transform);
            }
        }
       

        if (lifeTime <= 0)
        {
            lifeTime = 0.2f;
        }
    }
}