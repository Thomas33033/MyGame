using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


class SkillManager : Singleton<SkillManager>
{
    public void AttackTarget(CharacterBase attacker,CharacterBase defender , int skillId)
    {
        if (attacker != null)
        {
            SkillHolderComponent skillHolder = attacker.GetComponent<SkillHolderComponent>();
            if (skillHolder != null)
            {
                //计算伤害
                DamageManager.Instance.OnResponseDamage(attacker, defender, skillId);

                //技能表现
                SkillClipData skillClip = new SkillClipData();
                skillClip.skillCofingId = skillId;
                skillClip.target = defender;
                skillHolder.CastSkill(skillClip);
                
               
            }
        
        }
    }
}

