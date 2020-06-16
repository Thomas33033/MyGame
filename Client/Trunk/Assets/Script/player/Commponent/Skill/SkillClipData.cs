using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 技能数据
/// </summary>
public class SkillClipData 
{
    public CharacterBase target;
    public int skillCofingId;
    public int effectId;
    public int MoveId;
    public CfgSkillData mConfig;

    public CfgSkillData config 
    {
        get 
        {
            if (mConfig == null)
                mConfig = ConfigManager.Instance.GetData<CfgSkillData>(skillCofingId);
            return mConfig;
        }
    }

	
}
