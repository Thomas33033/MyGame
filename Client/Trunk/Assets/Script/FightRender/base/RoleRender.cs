using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Fight;
using FightCommom;

public class RoleRender :Entity
{
    public string teamId;

    public string roleId;

    public int hp;

    public int hpMax;

    public int mp;

    public int mpMax;

    public Dictionary<int, EffectPoolObj> dicEffectObjects;

    public bool isPlayer;

    public FightRoleUI ui;

    public BattlefieldRender battlefield;

    public GameObject gameObject;

    public Transform transform;

    public Transform hitTrans;

    public RoleRender()  {
        dicEffectObjects = new Dictionary<int, EffectPoolObj>();
    }


    public virtual void Update()
    {
        base.OnUpdate();
    }

    public virtual void SetHpMax(int v)
    {
        hpMax = v;
    }

    public virtual void SetMpMax(int v)
    {
        mpMax = v;
    }

    public virtual void SetHp(int v)
    {
        hp = v;
    }

    public virtual void SetMp(int v)
    {
        mp = v;
    }

    public virtual void StatusSet(int state, bool value)
    {
    }

    public virtual void PlayAnimator(string v)
    {
    }

    public void LoadEffectGameobject(string asset, AsyncPool<EffectPoolObj>.LoadCompleteHandler callback)
    {
        ObjectPoolManager.Instance.LoadObjectAsync<EffectPoolObj>(asset, callback);
    }

    public void EffectAdd(int id, string asset)
    {
        if (string.IsNullOrEmpty(asset) == true || dicEffectObjects.ContainsKey(id) == true)
        {
            return;
        }

        LoadEffectGameobject(asset, (EffectPoolObj poolObj) =>
        {
            if (poolObj.itemObj == null)
                return;
            LoadTools.SetParent(poolObj.itemObj, gameObject);
            dicEffectObjects.Add(id, poolObj);

            FightAttackRender fightAttack = poolObj.itemObj.GetComponent<FightAttackRender>();
            if (fightAttack != null)
            {
                fightAttack.SetData(poolObj, this, new List<RoleRender>() { this });
            }
        });
    }

    public void EffectRemove(int id)
    {
        if (dicEffectObjects.ContainsKey(id) == false)
        {
            return;
        }
        dicEffectObjects[id].ReturnToPool();
        dicEffectObjects.Remove(id);
    }

    public virtual void BuffAdd(FightBuffInfo info)
    {
        EffectAdd(info.id, info.resource);
    }

    public virtual void BuffRemove(FightBuffInfo info)
    {
        EffectRemove(info.id);
    }

    public virtual void ShieldAdd(FightShieldInfo info)
    {
        EffectAdd(info.id, info.resource);
    }

    public virtual void ShieldRemove(FightShieldInfo info)
    {
        EffectRemove(info.id);
    }

    public virtual void AuraAdd(FightAuraInfo info)
    {
        EffectAdd(info.id, info.resource);
    }

    public virtual void AuraRemove(FightAuraInfo info)
    {
        EffectRemove(info.id);
    }

    public virtual void SkillDone(FightSkillInfo skillInfo, List<RoleRender> listTargets)
    {
        if (string.IsNullOrEmpty(skillInfo.Resources) == true)
            return;

        LoadEffectGameobject(skillInfo.Resources, (EffectPoolObj poolObj) =>
        {
            GameObject obj = poolObj.itemObj;

            if (obj != null)
            {
                LoadTools.SetParent(poolObj.itemObj, gameObject);

                FightAttackRender fightSCAttack = obj.GetComponent<FightAttackRender>();
                if (fightSCAttack != null)
                {
                    fightSCAttack.SetData(poolObj, this, listTargets);
                }
            }
        });
    }

    public virtual void SkillCast(FightSkillInfo skillInfo, List<RoleRender> listTargets)
    {
        if (string.IsNullOrEmpty(skillInfo.MagicEffect) == true)
            return;

        LoadEffectGameobject(skillInfo.MagicEffect, (EffectPoolObj poolObj) =>
        {
            GameObject obj = poolObj.itemObj;

            if (obj != null)
            {
                LoadTools.SetParent(poolObj.itemObj, gameObject);
                FightAttackRender fightSCAttack = obj.GetComponent<FightAttackRender>();
                if (fightSCAttack != null)
                {
                    fightSCAttack.SetData(poolObj, this, listTargets);
                }
            }
        });
    }

    public virtual void AttackShow(FightDamageInfo damageInfo, float time, float timeExecute, bool isHit, RoleRender target)
    {
    }

    public virtual void ShowHurt(int hp, bool isHit, int hurt, bool isCrit, int source, int damageType)
    {
    }

    public virtual void Die()
    {
    }

    public void PlayAudioClip(string name)
    {
        //AudioClip audioClip = LoadTools.LoadAudioClip("FightSound", name);
        //if (audioClip != null)
        //{
        //    AudioSource.PlayClipAtPoint(audioClip, new Vector3());
        //}
        //else
        //{
        //    //Debug.LogError("not find clip " + name);
        //}
    }

    //OccupiedPlatform occupiedPlatform;
    //public void SetPlatform(Platform p, Node n)
    //{
    //    occupiedPlatform = new OccupiedPlatform(p, n);
    //}
}



