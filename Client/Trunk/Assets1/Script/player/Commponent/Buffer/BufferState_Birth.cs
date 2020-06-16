using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 出生状态
/// 播放出生特效、出生音效、出生动作等等
/// </summary>
public class BufferState_Birth : BufferState 
{
    GameObject birthEffectObj = null;
    public override void OnEnter(BufferStateComponent mgr, BufferStateData data)
    {
        base.OnEnter(mgr,data);
        Debug.LogError("进入出生状态");
        //SetBuildState(BuildState.buildOver);
        AssetManager.Instance.LoadAssets(AssetsType.Effect, "SO(Effect_AOEWave)", (obj) =>
        {
            birthEffectObj = GameObject.Instantiate(obj,this.mgr.Owner.ModelObj.transform);
            birthEffectObj.transform.localScale = Vector3.one;
            birthEffectObj.transform.localPosition = new Vector3(0, 0.5f, 0);
            birthEffectObj.transform.localRotation = new Quaternion(0, 0, 0, 0);
            
        });

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
