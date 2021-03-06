﻿using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using Fight;
using UnityEngine.SceneManagement;

/// <summary>
/// 英雄、怪物、建筑
/// </summary>
public class FightRoleRender : RoleRender
{
    public Animator animator;

    private float _actionTime_attack;
    private float _actionTime_skill;
    private bool isDie;

    private ModelPoolObj npcPoolObj;

    public FightRoleRender() 
    {
        delayCalls = new List<DelayCall>();
    }


    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        for (int i = 0; i < delayCalls.Count; i++)
        {
            if (delayCalls[i].time < Time.time)
            {
                delayCalls[i].action();
                delayCalls.RemoveAt(i);
            }
        }

        float now = FightSceneRender.Instance.GetTime();
        //if (_timeStatusEnd > 0 && now > _timeStatusEnd)
        //{
        //    if (animator.GetCurrentAnimatorStateInfo(0).IsName("dyzz") == true)
        //    {
        //        PlayAnimator("idle");
        //    }

        //    _timeStatusEnd = 0;
        //}
    }

    private void OnEnable()
    {
        PlayAnimator("idle");
        if (ui != null)
        {
            ui.gameObject.SetActive(true);
        }
    }

    private void OnDisable()
    {
        if (ui != null)
        {
            ui.gameObject.SetActive(false);
        }
    }

    public void LoadNpc(string npcAsset, RoleType roleType,Vector3 position)
    {
        this.roleType = roleType;
        var pool = ObjectPoolManager.Instance.CreatePool<ModelPoolObj>(ResPathHelper.UI_NPC_PATH + npcAsset + ".prefab");
        npcPoolObj = pool.GetObject();
        gameObject = npcPoolObj.itemObj;
        transform = gameObject.transform;
        this.transform.position = position;

        animator = this.gameObject.GetComponent<Animator>();
        if (animator == null)
        {
            Transform body = this.gameObject.transform.Find("body");
            if (body != null)
            {
                animator = body.GetComponent<Animator>();
            }
        }

        if (roleType == RoleType.Buildings)
        {
            gameObject.layer = LayerManager.layerBuilding;
        }
        else
        {
            string uiHeadPath = string.Format("{0}/{1}.prefab", ResPathHelper.UI_WINDOW_PATH, "HUD/UI_Head");
            ModelPoolObj modelPoolObj = ObjectPoolManager.Instance.GetPoolObj<ModelPoolObj>(uiHeadPath);
            ui = modelPoolObj.itemObj.GetComponent<FightRoleUI>();
            ui.transform.parent = GameObject.Find("UIRootHp").transform;
            ui.transform.localPosition = new Vector3(0, 1, 0);
            ui.GetComponent<FollowUI3D>().target = transform.Find("head").transform;
            ui.SetData(this.hpMax, this.mpMax);
        }

        
        //animator.runtimeAnimatorController = LoadTools.LoadRoleAnimator("RoleNpc", npcAsset);

        this.hitTrans = transform.Find("hit");
        if (this.hitTrans == null)
        {
            this.hitTrans = this.transform;
        }

        if (animator != null)
        {
            animator.Play("Idle");
        }
    }

    public void Jump(Vector3 v, float time)
    {
        Quaternion wantedRot = Quaternion.LookRotation(v - this.transform.position);
        //transform.rotation = wantedRot;

        transform.DOKill(true);

        float t = time;
        if (t > 0.5f)
            t -= 0.5f;

        transform.DOLocalMoveX(v.x, t).SetEase(Ease.Linear).OnComplete(() =>
        {
            animator.SetBool("move", false);
        });

        transform.DOLocalMoveZ(v.z, t).SetEase(Ease.Linear);

        transform.DOLocalMoveY(2, t / 2f).SetEase(Ease.InOutQuart).OnComplete(() =>
        {
            transform.DOLocalMoveY(0, t / 2f).SetEase(Ease.InOutQuart);
        });
    }


    public void Move(Vector3 v, float time)
    {
        Quaternion wantedRot = Quaternion.LookRotation(v - this.transform.position);
        transform.rotation = wantedRot;

        transform.DOKill(!animator.GetBool("move"));

        transform.DOLocalMove(v, time + 0.1f).SetEase(Ease.Linear).OnComplete(() =>
        {
            animator.SetBool("move", false);
        });
        animator.SetBool("move", true);
    }

    public void StopMove(Vector3 v)
    {
        //如果当前位置，比目标位置大10则强制瞬移到目标点
        transform.DOKill();
        animator.SetBool("move", false);
        float distance = Vector3.Distance(v, this.transform.position);
        if (distance > 5)
        {
            this.transform.position = v;
        }
}

    public override void Die()
    {
        base.Die();
        transform.DOKill(true);
        PlayAnimator("Death");
        isDie = true;
        SkillCastBreak();
        
        this.AddSchedule(2f, this.DoDestroy);
       
        ui.gameObject.SetActive(false);
    }

    private void DoDestroy()
    {
        gameObject.SetActive(false);
    }

    public override void SetHp(int v)
    {
        base.SetHp(v);
        ui.ShowHp(v);
    }

    public override void SetMp(int v)
    {
        base.SetMp(v);
        ui.ShowMp(v);
    }

    private List<DelayCall> delayCalls;

    

    public override void ShowHurt(int hp, bool isHit, int hurt, bool isCrit, int source, int damageType)
    {
        if (damageType == 0)
            return;

        string labelPrefab = "";
        float delayTime = 0f;
        float scale = 0.5f;
        string content = "-" + hurt;
        Ease ease = Ease.OutBack;
        Vector3 startOffset = new Vector3(0, 3, 0);
        Vector3 endOffset = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(1.2f, 1.7f), 0);

        switch (damageType)
        {
            case (int)EDamageType.Hp:

                labelPrefab = "FightDamageRestoreLabel";
                content = "+" + hurt;

                endOffset.x = 0f;
                endOffset.y = UnityEngine.Random.Range(0.7f, 1.2f);

                startOffset.y = 2f;

                ease = Ease.InOutCirc;

                break;

            case (int)EDamageType.Mp:
                labelPrefab = "FightDamageLabel";
                content = "+" + hurt;
                break;

            case (int)EDamageType.Physical:

                labelPrefab = "FightDamageCritLabel";
                if (isCrit)
                {
                    content = "-" + hurt + "#";
                }

                break;

            case (int)EDamageType.Magic:
                labelPrefab = "FightDamageDebuffLabel";
                if (isCrit)
                {
                    content = "-" + hurt + "#";
                }
                break;

            default:
                labelPrefab = "FightDamageLabel";
                break;
        }

        switch (source)
        {
            case (int)DamageSourceType.Skill:
                endOffset.x = 0f;
                ease = Ease.OutCirc;
                break;

            case (int)DamageSourceType.Shield:
                labelPrefab = "FightDamageShieldLabel";
                content = content.Replace("#", "");
                break;
        }

        if (isCrit)
        {
            scale = 0.8f;
        }

        //if (isHit == false)
        //{
        //    labelPrefab = "FightDamageNormalLabel";
        //    content = "Miss";
        //}

        DelayCall delayCall = new DelayCall();
        delayCall.time = Time.time + delayTime;
        delayCall.action = () =>
        {
            var poolObj = ObjectPoolManager.Instance.GetPoolObj<DamagePoolObj>(ResPathHelper.UI_DAMAGE_APTH + labelPrefab + ".prefab");
            GameObject obj = poolObj.itemObj;
            obj.transform.SetParent(FightSceneRender.Instance.rootUI.transform);
            FightDamageLabel fd = obj.GetComponent<FightDamageLabel>();
            //FightDamageLabel fd = LoadTools.LoadGameObject("Fight3D", labelPrefab, Fight3D.instance.rootUI.gameObject)
            fd.transform.localScale = new Vector3(scale, scale, scale);
            fd.transform.position = transform.position + startOffset; // + v3;
            fd.SetPoolObj(poolObj);
            fd.SetText(content, endOffset, ease);
            fd.OnStart();
        };
        delayCalls.Add(delayCall);
    }

    private void ChangeLayer(Transform tf, int v)
    {
        tf.gameObject.layer = v;

        for (int i = 0; i < tf.childCount; i++)
        {
            ChangeLayer(tf.GetChild(i), v);
        }
    }

    public override void SkillDone(FightSkillInfo skillInfo, List<RoleRender> listTargets)
    {
        if (string.IsNullOrEmpty(skillInfo.ResourcesSound) == false)
        {
            PlayAudioClip(skillInfo.ResourcesSound);
        }

        if (string.IsNullOrEmpty(skillInfo.Resources) == false)
        {
            string assetName = skillInfo.Resources;

            //ObjectPoolManager.Instance.LoadObjectAsync<EffectPoolObj>(assetName, (EffectPoolObj poolObj) =>
            //{
            //    GameObject obj = poolObj.itemObj;
            //    LoadTools.SetParent(obj, battlefield.rootRole.gameObject);

            //    if (obj != null)
            //    {
            //        FightAttackRender fightSCAttack = obj.GetComponent<FightAttackRender>();
            //        if (fightSCAttack != null)
            //        {
            //            UpdateSkillEffectFromPoint(skillInfo.id);
            //            fightSCAttack.SetData(poolObj, this, listTargets);
            //        }
            //    }
            //});
        }
    }

    private void SkillCastBreak()
    {
        if (isSkillCasting == false)
            return;

        isSkillCasting = false;

        if (_objCastEffect != null)
        {
           GameObject.Destroy(_objCastEffect);
        }

        //if (isHightShowwing)
        //{
        //    ChangeLayer(transform, LayerMask.NameToLayer("FightObject"));
        //    Fight3D.instance.CloseSkillCamera();
        //    isHightShowwing = false;
        //}
    }

    private bool isSkillCasting;
    //private bool isHightShowwing;

    public override void SkillCast(FightSkillInfo skillInfo, List<RoleRender> listTargets, float dieTime)
    {
        if (string.IsNullOrEmpty(skillInfo.MagicEffectSound) == false)
        {
            PlayAudioClip(skillInfo.MagicEffectSound);
        }

        isSkillCasting = true;

        ShowSkill(skillInfo, listTargets);

        if (string.IsNullOrEmpty(skillInfo.MagicEffect) == false)
        {
            string assetName = skillInfo.MagicEffect;

            assetName = ResPathHelper.UI_EFFECT_APTH + skillInfo.MagicEffect + ".prefab";

            EffectPoolObj poolObj = ObjectPoolManager.Instance.GetPoolObj<EffectPoolObj>(assetName);

            GameObject obj = poolObj.itemObj;
            if (obj != null)
            {
                // LoadTools.SetParent(obj, gameObject);
                ChangeLayer(obj.transform, gameObject.layer);

                FightAttackRender fightSCAttack = obj.GetComponent<FightAttackRender>();
                if (fightSCAttack != null)
                {
                    
                    UpdateSkillEffectFromPoint(skillInfo.id);
                    fightSCAttack.SetData(poolObj, this, listTargets, dieTime);
                }
            }
            else
            {
                Debug.LogError("Not find res " + assetName);
            }

        }

        
    }

    private void ShowSkill(FightSkillInfo skillInfo, List<RoleRender> listTargets)
    {
        transform.DOKill(true);

        if (listTargets.Count > 0 && listTargets[0] != this)
        {
            Quaternion wantedRot = Quaternion.LookRotation(listTargets[0].transform.position - this.transform.position);
            transform.rotation = wantedRot;
        }

        int skillIndex = skillInfo.id % 10;
        if (skillIndex == 0)
        {
            PlayAnimator("attack");
        }
        else
        {
            PlayAnimator("skill_"+ skillIndex);
        }
    }

    private void ResetHightShowwing()
    {
        //ChangeLayer(transform, LayerMask.NameToLayer("FightObject"));
       
    }


    public override void PlayAnimator(string v)
    {
        if (isDie)
            return;
        if(animator != null)
            animator.Play(v);
    }

    private GameObject _objCastEffect;

    //private float _timeStatusEnd;

    internal void StatusShow(int state, int stateNum)
    {
        //_timeStatusEnd = endTime;

        if (state == (int)RoleStatus.Dizz)
        {
            transform.DOKill(false);

            if (stateNum > 0)
            {
                PlayAnimator("dizzy");
            }
            else
            {
                PlayAnimator("idle");
            }

            SkillCastBreak();
        }
    }

    public override void AttackShow(FightDamageInfo damageInfo, float time, float timeExecute, bool isHit, RoleRender target)
    {
        string assetName = ResPathHelper.UI_EFFECT_APTH + damageInfo.Resources + ".prefab";

        //ObjectPoolManager.Instance.LoadObjectAsync<EffectPoolObj>(assetName, (EffectPoolObj poolObj) =>
        //{
        var poolObj = ObjectPoolManager.Instance.GetPoolObj<EffectPoolObj>(assetName);
        GameObject obj = poolObj.itemObj;

        if (obj != null)
        {
            FightAttackRender fightSCAttack = obj.GetComponent<FightAttackRender>();
            if (fightSCAttack != null)
            {
                UpdateSkillEffectFromPoint(0);
                fightSCAttack.SetData(poolObj, this, new List<RoleRender>() { target }, timeExecute);
            }
        }
        //});
    }

    public override void BuffAdd(FightBuffInfo info)
    {
        base.BuffAdd(info);
    }

    /// <summary>
    /// 刷新技能特效起始位置
    /// </summary>
    /// <param name="skillId"></param>
    private void UpdateSkillEffectFromPoint(int skillId)
    {
        Transform trans = transform.Find("skill_" + skillId%10 + "_point");
        if (trans != null)
        {
            this.fromPoint = trans.position;
        }
        else
        {
            this.fromPoint = this.transform.position;
        }
        
    }
}