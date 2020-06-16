using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 在可视范围内搜索敌人
/// </summary>
public class ScanTargetComponent : ComponentBase
{
    private CharacterBase mTarget;
    private int mSearchMaxPixelDistance = int.MaxValue;
    public CharacterBase Target
    {
        get { return mTarget; }
    }

    public void SetSearchMaxDistance(int range)
    {
        this.mSearchMaxPixelDistance = range;
    }

    public override void OnInit(CharacterBase p_owner)
    {
        base.OnInit(p_owner);
    }

    public override void OnUpdate(float dt)
    {
        base.OnUpdate(dt);
        //ScanForTarget();
    }

    /**搜索目标*/
    public void ScanForTarget()
    {
        if (!this.Owner.CanAttack(null))
            return;

        if (this.mTarget == null)
        {
            EEntityType type = EEntityType.None;
            bool isOnly = false;
            switch (this.Owner.SearchMode)
            {
                case ESearchTargetType.MonsterFirst:
                    type = EEntityType.Monster;
                    break;
                case ESearchTargetType.NpcFirst:
                    type = EEntityType.Npc;
                    break;
                case ESearchTargetType.TowerFirst:
                    type = EEntityType.Tower;
                    break;
                case ESearchTargetType.OnlyMonster:
                    type = EEntityType.Monster;
                    isOnly = true;
                    break;
                case ESearchTargetType.OnlyNpc:
                    type = EEntityType.Npc;
                    isOnly = true;
                    break;
                case ESearchTargetType.OnlyTower:
                    type = EEntityType.Tower;
                    isOnly = true;
                    break;
                case ESearchTargetType.OnlyPlayer:
                    type = EEntityType.Player;
                    isOnly = true;
                    break;
            }
            if (type != EEntityType.None)
                this.mTarget = this.PrioritySearchTargetList(type, isOnly);
        }
        else
        {
            if (this.Target.IsLive())
            {
                float newdis = MathfHelper.PixelDistance(this.Owner.ClientPos, this.Target.ClientPos);
                Debug.LogError("newdis:" + newdis + "  " + this.mSearchMaxPixelDistance);
                if (this.Target.HP <= 0 || !this.Target.ModelObj || newdis > this.mSearchMaxPixelDistance)
                {
                    this.mTarget = null;
                }
            }
            else
            {
                this.mTarget = null;
            }

        }

    }

    /**取消最大搜索范围 */
    public void CancelMaxPixelDisatance()
    {
        this.mSearchMaxPixelDistance = 99999999;
    }

    public bool CheckArrivedTargetPosition()
    {
        return MathfHelper.PixelDistance(this.Owner.ClientPos, Target.ClientPos) > 20;
    }

    public bool TargetInAttackRange()
    {
        return MathfHelper.PixelDistance(this.Owner.ClientPos, Target.ClientPos) > 20;
    }


    /**按优先级搜索目标*/
    public CharacterBase PrioritySearchTargetList(EEntityType type, bool isOnly)
    {
        var temp = SearchTargetList(EntitesManager.Instance.GetList(type));

        if (!isOnly)
        {
            List<EEntityType> keyList = EntitesManager.Instance.GetTypeList();
            keyList.Remove(type);
            for (int i = 0; i < keyList.Count; i++)
            {
                var t_type = keyList[i];
                temp = SearchTargetList(EntitesManager.Instance.GetList(t_type));
                if (temp != null)
                {
                    break;
                }
            }
        }
        return temp;
    }

    public CharacterBase SearchTargetList(List<CharacterBase> targetList)
    {
        float dis = 99999999;
        
        if (targetList == null)
        {
            return null;
        }

        CharacterBase newTarget = null;
        foreach (var target in targetList)
        {
            if (!target.CanAttack(this.Owner))
                continue;
            float newdis = MathfHelper.PixelDistance(this.Owner.ClientPos, target.ClientPos);
            if (newdis < dis && newdis <= this.mSearchMaxPixelDistance)
            {
                dis = newdis;
                newTarget = target;
            }
        }
        return newTarget;
    }

    public void OnDestroy()
    {
        base.OnDestroy();
    }
}
