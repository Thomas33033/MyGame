using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FightSCAttackLine : FightAttackRender
{
    public float speed;

    private Vector3 vet3;

    private bool bPlay = false;

    public override void Update()
    {
        if (this.bPlay)
        {
            base.Update();
            transform.position += speed * timeDelta * vet3;
        }
    }

    public override void SetData(EffectPoolObj poolObj, RoleRender attacker, List<RoleRender> listTargets, float dieTime = 0, float timeScale = 1)
    {
        base.SetData(poolObj, attacker, listTargets, dieTime, timeScale);

        Vector3 fromPos = attacker.transform.position + new Vector3(0, 1, 0);
        Vector3 toPos = listTargets[0].hitTrans.position;

        transform.position = fromPos;
        
        if (listTargets.Count > 0)
        {
            vet3 = (toPos - fromPos).normalized;
            transform.LookAt(listTargets[0].transform);
        }

        this.bPlay = true;
    }
}