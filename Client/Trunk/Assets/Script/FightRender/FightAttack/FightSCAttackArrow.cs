using UnityEngine;
using System.Collections.Generic;

public class FightSCAttackArrow : FightAttackRender
{
    private Vector3 _startPosition;
    private Vector3 _startContentPosition;

    private Transform targetTrans;

    public override void Update()
    {
        base.Update();

        if (listTargets[0].transform == null)
        {
            Destroy(gameObject);
            return;
        }

        if (objContent.activeSelf)
        {
            objContent.transform.localPosition = Vector3.Lerp(_startContentPosition, new Vector3(), (timePass - timeDelayContent) / (lifeTime - timeDelayContent));
        }

        if (timePass >= timeDelayContent)
        {
            transform.position = Vector3.Lerp(_startPosition, targetTrans.position, (timePass - timeDelayContent) / (lifeTime - timeDelayContent));
        }
    }

    public override void SetData(EffectPoolObj poolObj, RoleRender attacker, List<RoleRender> listTargets, float dieTime = 0, float timeScale = 1f)
    {
        base.SetData(poolObj, attacker, listTargets, dieTime, timeScale);

        Vector3 fromPos = attacker.transform.position + new Vector3(0, 1, 0);
        targetTrans = listTargets[0].hitTrans;

        transform.position = fromPos;
        transform.LookAt(listTargets[0].transform);

        _startPosition = transform.position;
        if (objContent != null)
        {
            _startContentPosition = objContent.transform.localPosition;
        }

        lifeTime = Mathf.Max(0.02f, dieTime - this.timeCreate);
    }
}