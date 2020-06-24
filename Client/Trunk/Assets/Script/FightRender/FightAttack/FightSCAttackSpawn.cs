using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FightSCAttackSpawn : FightAttackRender
{
    public List<FightAttackRender> listFightSCAttacks;

    public override void SetData(EffectPoolObj poolObj, RoleRender attacker, List<RoleRender> listTargets, float dieTime = 0, float timeScale = 1)
    {
        base.SetData(poolObj, attacker, listTargets, dieTime, timeScale);
        int index = 0;
        for (int i = 0; i < listFightSCAttacks.Count; i++)
        {
            if (index < listTargets.Count)
            {
                listFightSCAttacks[i].transform.SetParent(listTargets[index].transform);
                listFightSCAttacks[i].transform.localPosition = new Vector3();
                listFightSCAttacks[i].SetData(null, attacker, new List<RoleRender>() { listTargets[index] }, dieTime, timeScale);
                index++;
            }
            else
            {
                listFightSCAttacks[i].gameObject.SetActive(false);
            }
        }
    }

    protected override void ReturnPool()
    {
        if (poolObj != null)
            poolObj.OnDestroy();
        Destroy(gameObject);
    }
}