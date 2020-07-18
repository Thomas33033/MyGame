using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FightSCAttackGroup : FightAttackRender
{
    public List<FightAttackRender> listFightSCAttacks;
    public bool needSetParent;

    public override void SetData(EffectPoolObj poolObj, RoleRender attacker, List<RoleRender> listTargets, float dieTime = 0, float timeScale = 1)
    {
        base.SetData(poolObj, attacker, listTargets, dieTime, timeScale);
        //int index = 0;
        for (int i = 0; i < listFightSCAttacks.Count; i++)
        {
            //listFightSCAttacks[i].transform.SetParent(listTargets[index].transform);
            //listFightSCAttacks[i].transform.localPosition = new Vector3();

            if (needSetParent)
            {
                listFightSCAttacks[i].transform.SetParent(listTargets[i].transform);
                listFightSCAttacks[i].transform.localPosition = new Vector3();
            }

            listFightSCAttacks[i].SetData(null, attacker, listTargets, dieTime, timeScale);
        }
    }

    protected override void ReturnPool()
    {
        if (poolObj != null)
            poolObj.OnDestroy();
        Destroy(gameObject);
    }
}