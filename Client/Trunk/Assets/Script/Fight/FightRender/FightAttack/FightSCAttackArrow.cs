using UnityEngine;
using System.Collections.Generic;

public class FightSCAttackArrow : FightAttackRender
{
    private Vector3 _startPosition;
    //目标不存在时，技能向指定方向移动
    private Vector3 _endPoistion;
    private Vector3 _startContentPosition;

    public static GameObject target;

    private Transform targetTrans;

    private Vector3 endPosition;

    private float moveTime;

    private float rate;

    public override void Update()
    {
        base.Update();
        //目标消失，不会自动销毁
        //if (listTargets[0].transform == null)
        //{
        //    Destroy(gameObject);
        //    return;
        //}

        if (objContent.activeSelf)
        {
           // objContent.transform.localPosition = Vector3.Lerp(_startContentPosition, new Vector3(), (timePass - timeDelayContent) / (lifeTime - timeDelayContent));
        }

        if (timePass >= timeDelayContent)
        {
            if (targetTrans == null)
            {
                transform.position = Vector3.Lerp(_startPosition, _endPoistion, (timePass - timeDelayContent) / moveTime);
            }
            else
            {
                endPosition = targetTrans.position - (targetTrans.position -_startPosition).normalized * 1f;
                rate = (timePass - timeDelayContent) / moveTime;
                transform.position = Vector3.Lerp(_startPosition, endPosition, rate);
                ChangeForward(rate);
            }
           
        }
    }

    public override void SetData(EffectPoolObj poolObj, RoleRender attacker, List<RoleRender> listTargets, float dieTime = 0, float timeScale = 1f)
    {
        base.SetData(poolObj, attacker, listTargets, dieTime, timeScale);

        Vector3 fromPos = attacker.fromPoint;

        if (listTargets.Count < 1 || listTargets[0] == null || listTargets[0].transform == null)
        {
            //目标不存在
            _endPoistion = this.attacker.transform.position
                + this.attacker.transform.forward * 5;
            _endPoistion.y = fromPos.y;
            transform.rotation = this.attacker.transform.rotation;
            base.hasHit = false;
        }
        else
        {
            base.hasHit = true;
            targetTrans = listTargets[0].hitTrans;
            this.attacker.transform.LookAt(listTargets[0].transform);
            ChangeForward(1);
        }
        
        transform.position = fromPos;
        _startPosition = transform.position;
        
        if (objContent != null)
        {
            _startContentPosition = objContent.transform.localPosition;
        }

        lifeTime = Mathf.Max(0.02f, dieTime - this.timeCreate);
        moveTime = lifeTime - timeDelayContent;
    }


    void ChangeForward(float times)
    {
        //获得目标点到自身的朝向
        Vector3 finalForward = (targetTrans.position - transform.position).normalized;

        if (finalForward != transform.forward)
        {
            Debug.Log(times);
            //将自身forward朝向慢慢转向最终朝向
            transform.forward = Vector3.Lerp(transform.forward, finalForward, times);
        }
    }
}