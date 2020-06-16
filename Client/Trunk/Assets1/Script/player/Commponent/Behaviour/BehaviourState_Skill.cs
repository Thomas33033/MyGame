using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourState_Skill : BehaviourState 
{
    public Transform[] shootPoint;
    public Transform owerTrans;
    public override void OnEnter(BehaviourComponent mgr)
    {
        base.OnEnter(mgr);
        
    }

    private void UpdateShootPoint()
    {
        GameObject modelObj = this.mgr.Owner.ModelObj;
        if (modelObj != null)
        {
            TurretObject turretObj = modelObj.gameObject.GetComponent<TurretObject>();
            if (turretObj != null)
            {
                if (turretObj.shootPoint != null && turretObj.shootPoint.Length > 0)
                {
                    shootPoint = turretObj.shootPoint;
                    return;
                }
            }
            else
            {
                shootPoint = new Transform[1];
                shootPoint[0] = modelObj.transform;
                return;
            }
        }
        return;
    }

    public void Shoot(SkillClipData skillData)
    {
        if(owerTrans == null)
            owerTrans = this.mgr.Owner.Trans;
        UpdateShootPoint();
        SoundController.Instance.PlaySound(skillData.config.SoundName, this.mgr.Owner.Trans.position);

        string resPath = "Prefabs/ShootObjects/" + skillData.config.ResName;

        var pool = ObjectPoolManager.Instance.CreatePool<ModelPoolObj>(resPath);
        CObjectPool<ModelPoolObj> modelPool = pool;
        ModelPoolObj modelPoolObj = modelPool.GetObject();
        modelPoolObj.itemObj.transform.position = owerTrans.position;
        modelPoolObj.itemObj.transform.rotation = owerTrans.rotation;
        modelPoolObj.itemObj.SetActive(true);
        ShootObject shootObj = modelPoolObj.itemObj.GetComponent<ShootObject>();
        shootObj.poolItem = modelPoolObj;
        shootObj.Shoot(this.mgr.Owner, skillData.target, skillData, shootPoint[0]);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
    }

    public override void OnLeave()
    {
        base.OnLeave();
    }
}
