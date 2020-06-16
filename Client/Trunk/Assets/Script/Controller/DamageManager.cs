using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class DamageManager : Singleton<DamageManager>
{



    //计算伤害，并通知对方是否收到伤害
    public void OnResponseDamage(CharacterBase attacker, CharacterBase defender, int skillId)
    {
        if(defender.HP < 0)
        {
            return;
        }

        CfgSkillData skillConfig = ConfigManager.Instance.GetData<CfgSkillData>(skillId);
        int skillLevel = skillConfig.Level;
        int[] magicDamagePercent = MathfHelper.StringToInt(skillConfig.Mdamage_percent.Split(','));
        int[] magicDamageValue = MathfHelper.StringToInt(skillConfig.Mdamage_value.Split(','));

        int[] physicalDamagePercent = MathfHelper.StringToInt(skillConfig.Pdamage_value.Split(','));
        int[] physicalDamageValue = MathfHelper.StringToInt(skillConfig.Pdamage_value.Split(','));

        // 物理攻击
        float physicalDamage = attacker.PAttack() - defender.PAttack();
        if (physicalDamage > 0)
        {
            if (physicalDamagePercent.Length >= 2)
                physicalDamage *= (physicalDamagePercent[0] + physicalDamagePercent[1] * skillLevel) / 100;
            else
                physicalDamage = 0;

            if (physicalDamageValue.Length >= 2)
                physicalDamage += physicalDamageValue[0] + physicalDamageValue[1] * skillLevel;
        }
        else{
            physicalDamage = 0;
        }

         // 魔法攻击
        float magicDamage = attacker.MAttack() - defender.MAttack();
        if (magicDamage > 0) {
           
            if (magicDamagePercent.Length >= 2)
                magicDamage *= (magicDamagePercent[0] + magicDamagePercent[1] * skillLevel) / 100;
            else
                magicDamage = 0;

            if (magicDamageValue.Length >= 2)
                magicDamage += magicDamageValue[0] + magicDamageValue[1] * skillLevel;
        }
        else
        {
            magicDamage = 0;
        }

        float damage = physicalDamage + magicDamage ;
        if (damage > 0) defender.GetHurt(attacker, damage);

        //float stunDuration = skillConfig.StunTime;
        //Slow slow = new Slow(skillConfig.Dot);
        //Dot dot = new Dot(skillConfig.Slow);

        //if (stunDuration > 0)
        //{
        //    //眩晕
        //    BufferStateData bsData = new BufferStateData();
        //    bsData.type = StateType.Stunned;
        //    bsData.stunDuration = stunDuration;
        //    bsData.attacker = attacker;
        //    defender.buffSateMgr.AddState(bsData);
        //}
        //if (slow.duration * slow.slowFactor > 0)
        //{   //减速
        //    BufferStateData bsData = new BufferStateData();
        //    bsData.type = StateType.Slow;
        //    bsData.slow = slow;
        //    bsData.attacker = attacker;
        //    defender.buffSateMgr.AddState(bsData);
        //}
        //if (dot.damage * dot.duration * dot.interval > 0)
        //{   //持续伤害
        //    BufferStateData bsData = new BufferStateData();
        //    bsData.type = StateType.DotDamage;
        //    bsData.dot = dot;
        //    bsData.attacker = attacker;
        //    defender.buffSateMgr.AddState(bsData);
        //}
        
    }



}

