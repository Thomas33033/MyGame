using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 负责资源产出，比如植物、兵营、资源建筑等等
/// </summary>
public class ResOutPutComponent : ComponentBase
{
    private int resId;                //资源ID
    private int unitResNum = 1;       //单位每次生产量
    private int cooldown = 1;         //单位生产时间
    private int curNum = 0;       //当前拥有的资源数量
    private int storageMaxNum = 100;  //当前可以存放的资源总数

    private bool canCreate;
    private float thinkTime; 

    public override void OnInit(CharacterBase p_owner)
    {
        base.OnInit(p_owner);
        canCreate = true;
        thinkTime = Time.time + cooldown;
    }

    public void OnUpdate(float dt)
    {
        base.OnUpdate(dt);
        if (!this.Owner.CanWork())
            return;

        if (canCreate && thinkTime < Time.time)
        {
            curNum += unitResNum;
            
            if (curNum >= storageMaxNum)
            {
                //生产完毕，等待收割
                canCreate = false;
            }
            else
            {
                thinkTime = Time.time + Mathf.Max(0.05f, cooldown);
            }
        }
    }

    public void StartWorking()
    {
        canCreate = true;
        thinkTime = Time.time + cooldown;
    }

    //刷新进度
    public void RefreshCreateProgress()
    {
        float totalCreateNum = storageMaxNum / cooldown;
        float hasCreateNum = curNum / cooldown;

        float totalTimes = totalCreateNum * cooldown;
        float curLeftTimes = Mathf.Max(0, Time.time - thinkTime);
        float leftTimes = (totalTimes - hasCreateNum) * cooldown + curLeftTimes;
    }

    //获取资源
    public void GainResource()
    {
        GameControl.GainResource(new ResItem(resId, curNum));
        curNum = 0;
        if (!canCreate)
        {
            StartWorking();
        }
    }
    

}
