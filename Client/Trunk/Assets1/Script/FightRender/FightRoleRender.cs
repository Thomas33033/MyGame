using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using Fight;
using UnityEngine.SceneManagement;

/// <summary>
/// 英雄、怪物、建筑
/// </summary>
public class FightRoleRender : RoleRender
{
    public Animator skeletonAnimator;

    public Animator animator;

    private float _actionTime_attack;
    private float _actionTime_skill;
    private bool isDie;

    private void Start()
    {
        delayCalls = new List<DelayCall>();
        ui.SetData(hpMax, mpMax, isPlayer);
    }

    // Update is called once per frame
    private void Update()
    {
        ui.transform.position = transform.position;

        for (int i = 0; i < delayCalls.Count; i++)
        {
            if (delayCalls[i].time < Time.time)
            {
                delayCalls[i].action();
                delayCalls.RemoveAt(i);
            }
        }

        float now = FightSence.Instance.GetTime();
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

    public void LoadNpc(string npcAsset)
    {
        GameObject obj = ResourcesManager.LoadAsset<GameObject>(npcAsset);
        if (obj == null)
        {
            npcAsset = "luliya";
            obj = ResourcesManager.LoadAsset<GameObject>(npcAsset);
        }

        //animator.runtimeAnimatorController = LoadTools.LoadRoleAnimator("RoleNpc", npcAsset);

        //MeshRenderer meshRenderer = obj.GetComponent<MeshRenderer>();
        //SkeletonAnimation skeletonAnimation = obj.GetComponent<SkeletonAnimation>();

        //skeletonAnimator.skeletonDataAsset = skeletonAnimation.SkeletonDataAsset;
        //skeletonAnimator.GetComponent<MeshRenderer>().material = meshRenderer.sharedMaterial;

        //ui = LoadTools.LoadGameObject("Fight3D", "FightSCRoleUIInfo", Fight3D.instance.rootUI.gameObject).GetComponent<FightSCRoleUIInfo>();
        //ui.transform.position = transform.position;

    }

    public void Jump(Vector3 v, float time)
    {
        skeletonAnimator.transform.localScale = new Vector3(v.x > transform.localPosition.x ? 1 : -1, 1, 1);

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
        skeletonAnimator.transform.localScale = new Vector3(v.x > transform.localPosition.x ? 1 : -1, 1, 1);

        transform.DOKill(!animator.GetBool("move"));

        transform.DOLocalMove(v, time + 0.1f).SetEase(Ease.Linear).OnComplete(() =>
        {
            animator.SetBool("move", false);
        });
        animator.SetBool("move", true);
    }

    public override void Die()
    {
        base.Die();
        transform.DOKill(true);
        PlayAnimator("die");
        isDie = true;
        SkillCastBreak();
        CancelInvoke();

        Invoke("DoDestroy", 2f);
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
        Vector3 startOffset = new Vector3(0, 1, 0);
        Vector3 endOffset = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(1.2f, 1.7f), 0);

        switch (damageType)
        {
            case (int)FightDamageType.Hp:

                labelPrefab = "FightDamageRestoreLabel";
                content = "+" + hurt;

                endOffset.x = 0f;
                endOffset.y = UnityEngine.Random.Range(0.7f, 1.2f);

                startOffset.y = 2f;

                ease = Ease.InOutCirc;

                break;

            case (int)FightDamageType.Mp:
                labelPrefab = "FightDamageLabel";
                content = "+" + hurt;
                break;

            case (int)FightDamageType.Physical:

                labelPrefab = "FightDamageCritLabel";
                if (isCrit)
                {
                    content = "-" + hurt + "#";
                }

                break;

            case (int)FightDamageType.Magic:
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

        if (isHit == false)
        {
            labelPrefab = "FightDamageNormalLabel";
            content = "Miss";
        }

        DelayCall delayCall = new DelayCall();
        delayCall.time = Time.time + delayTime;
        delayCall.action = () =>
        {
            var poolObj = ObjectPoolManager.Instance.GetPoolObj<DamagePoolObj>(labelPrefab);
            GameObject obj = poolObj.itemObj;
            obj.transform.SetParent(FightSence.Instance.rootUI.transform);
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

            ObjectPoolManager.Instance.LoadObjectAsync<EffectPoolObj>(assetName, (EffectPoolObj poolObj) =>
            {
                GameObject obj = poolObj.itemObj;
                LoadTools.SetParent(obj, battlefield.rootRole.gameObject);

                if (obj != null)
                {
                    FightAttackRender fightSCAttack = obj.GetComponent<FightAttackRender>();
                    if (fightSCAttack != null)
                    {
                        fightSCAttack.SetData(poolObj, this, listTargets);
                    }
                }
            });
        }
    }

    private void SkillCastBreak()
    {
        if (isSkillCasting == false)
            return;

        isSkillCasting = false;

        if (_objCastEffect != null)
        {
            Destroy(_objCastEffect);
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

    public override void SkillCast(FightSkillInfo skillInfo, List<RoleRender> listTargets)
    {
        if (string.IsNullOrEmpty(skillInfo.MagicEffectSound) == false)
        {
            PlayAudioClip(skillInfo.MagicEffectSound);
        }

        isSkillCasting = true;

        if (string.IsNullOrEmpty(skillInfo.MagicEffect) == false)
        {
            string assetName = skillInfo.MagicEffect;

            assetName = skillInfo.MagicEffect;

            ObjectPoolManager.Instance.LoadObjectAsync<EffectPoolObj>(assetName, (EffectPoolObj poolObj) =>
            {
                GameObject obj = poolObj.itemObj;

                if (obj != null)
                {
                    LoadTools.SetParent(obj, gameObject);
                    ChangeLayer(obj.transform, gameObject.layer);

                    FightAttackRender fightSCAttack = obj.GetComponent<FightAttackRender>();
                    if (fightSCAttack != null)
                    {
                        fightSCAttack.SetData(poolObj, this, listTargets);
                    }
                }
            });
        }

        if (skillInfo.Type == 0)
        {
            ShowSkill0(skillInfo, listTargets);
        }
        else if (skillInfo.Type == 2)
        {
            ShowSkill2(skillInfo, listTargets);
        }
    }

    private void ShowSkill0(FightSkillInfo skillInfo, List<RoleRender> listTargets)
    {
        transform.DOKill(true);

        if (listTargets.Count > 0 && listTargets[0] != this)
            skeletonAnimator.transform.localScale = new Vector3(listTargets[0].transform.position.x > transform.position.x ? 1 : -1, 1, 1);

        if (isPlayer)
        {
            //isHightShowwing = true;
            //Fight3D.instance.OpenSkillCamera(Time.time + 1f);
            //ChangeLayer(transform, LayerMask.NameToLayer("FightRoleHight"));
            //Invoke("ResetHightShowwing", 1f);
            //int d = skeletonAnimator.transform.localScale.x > 0 ? 1 : -1;
            //skeletonAnimator.transform.DOScale(new Vector3(d * 1.5f, 1.5f, 1.5f), 0.2f);
        }
        PlayAnimator("castSkill");
    }

    private void ResetHightShowwing()
    {
        ChangeLayer(transform, LayerMask.NameToLayer("FightObject"));
        int d = skeletonAnimator.transform.localScale.x > 0 ? 1 : -1;
        skeletonAnimator.transform.DOScale(new Vector3(d * 1f, 1f, 1f), 0.2f);
    }

    private void ShowSkill2(FightSkillInfo skillInfo, List<RoleRender> listTargets)
    {
        if (listTargets.Count <= 0)
            return;
        
        RoleRender target = listTargets[0];
        
        transform.DOKill(true);
        skeletonAnimator.transform.localScale = new Vector3(target.transform.position.x > transform.position.x ? 1 : -1, 1, 1);
    
        Vector3 vt3 = target.transform.position - transform.position;
        
        int d = 0;
        if (vt3.z > 0.5f)
        {
            d = 1;
        }
        else if (vt3.z < -0.5f)
        {
            d = -1;
        }
        
        animator.SetInteger("direction", d);
        PlayAnimator("attack");
    }

    public override void PlayAnimator(string v)
    {
        if (isDie)
            return;
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
        string assetName = damageInfo.Resources;

        ObjectPoolManager.Instance.LoadObjectAsync<EffectPoolObj>(assetName, (EffectPoolObj poolObj) =>
        {
            GameObject obj = poolObj.itemObj;
            LoadTools.SetParent(obj, battlefield.rootRole.gameObject);

            if (obj != null)
            {
                FightAttackRender fightSCAttack = obj.GetComponent<FightAttackRender>();
                if (fightSCAttack != null)
                {
                    fightSCAttack.SetData(poolObj, this, new List<RoleRender>() { target }, timeExecute);
                }
            }
        });
    }

    public override void BuffAdd(FightBuffInfo info)
    {
        base.BuffAdd(info);
    }

}