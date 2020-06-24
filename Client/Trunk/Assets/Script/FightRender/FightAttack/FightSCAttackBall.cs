using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FightSCAttackBall : FightAttackRender
{
    private Vector3 p1;
    private Vector3 p2;

    public bool isHit;

    public GameObject objMiss;

    protected override void OnInitEffect()
    {
        base.OnInitEffect();
        if (objMiss != null)
            objMiss.SetActive(false);
    }

    public override void Update()
    {
        base.Update();

        float h = 4f * lifeTime / 2f;
        float _a = -4 * h / (lifeTime * lifeTime);
        float _b = 4 * h / lifeTime;

        transform.position = Vector3.Lerp(p1, p2, (timePass) / lifeTime) + new Vector3(0f, _a * timePass * timePass + _b * timePass, 0f);
    }

    public void SetData(EffectPoolObj poolObj, RoleRender attacker, RoleRender target, bool isHit, float dieTime = 0, float timeScale = 1)
    {
        this.isHit = isHit;
        base.SetData(poolObj, attacker, new List<RoleRender>() { target }, dieTime, timeScale);
        //p1 = transform.position;
        //p2 = listTargets[0].GetHitPosition(isHit);
        //lifeTime = Mathf.Max(0.02f, dieTime - this.timeCreate);
    }

    protected override void Die()
    {
        if (objContent != null)
        {
            objContent.SetActive(false);
        }

        if (listTargets.Count > 0)
        {
            transform.position = listTargets[0].transform.position;

           // if (listTargets[0] is FightSCShip)
           //     (listTargets[0] as FightSCShip).Hurt(p2, isHit);
        }

        if (isHit)
        {
            if (objHit != null)
                objHit.SetActive(true);
        }
        else
        {
            if (objMiss != null)
                objMiss.SetActive(true);
        }
        //Destroy(gameObject, hitTime);

        Invoke("ReturnPool", hitTime);
    }
}