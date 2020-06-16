using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : CharacterBase
{
    public int waveID;                    //波次ID

    public float moveAnimationModifier = 1.0f; //移动动画调节器

    public GameObject scoreEffect;       //得分特效

    public GameObject animationBody;    //动画实体

    public Animation aniBody;           //动画实体

    public bool immuneToSlow = false;   //减速免疫

    public CfgNpcAttrData attrConfig;

    public s_MonsterData ServerData
    {
        get
        {
            return (s_MonsterData)this.mData;
        }
    }

    public string GetResName()
    {
        return ServerData.config.Resname;
    }
    //---------------------------------------------------
    /* 物攻 */
    public override float PAttack() { return this.attrConfig.Pattack; }
    /** 魔攻 */
    public override float MAttack() { return this.attrConfig.Mattack; }
    /** 物防 */
    public override float PDefence() { return this.attrConfig.Pdefence; }
    /** 魔防 */
    public override float MDefence() { return this.attrConfig.Mdefence; }
    /** 暴击 */
    public override float Bang() { return 0; }
    /** 韧性 */
    public override float Toughness() { return 0; }
    /** 命中 */
    public override float Hit() { return 0; }
    /** 闪避 */
    public override float Duck() { return 0; }
    //---------------------------------------------------

    protected override void RegisterComponet()
    {
        base.RegisterComponet();
        this.AddComponent<AudioComponent>();
        this.AddComponent<BufferStateComponent>();
        this.AddComponent<BehaviourComponent>();
        this.AddComponent<MoveComponent>();
    }

    public override void OnInit(CharacterData _data)
    {
        base.OnInit(_data);
        this.attrConfig = this.ServerData.attrConfig;
        this.CreatBody();
    }

    public virtual void CreatBody()
    {
        string path = string.Format("{0}/{1}.prefab", ResPathHelper.UI_NPC_PATH, this.GetResName());
        var pool = ObjectPoolManager.Instance.CreatePool<ModelPoolObj>(path);
        var modelPoolItem = pool.GetObject();
        this.SetBody(modelPoolItem.itemObj);
        this.PushObject(modelPoolItem);

        modelPoolItem.itemObj.layer = LayerManager.LayerCreep();
        //modelPoolItem.itemObj.layer = LayerManager.LayerCreepF();
        this.GetComponent<UIComponent>().CreateHP();
        this.Trans.name = this.GetResName() + "_" + this.uid;
    }

    public void SetMovePath(GamePath p, int wID)
    {
        waveID = wID;

        if (this.FlyHeight() > 0) Trans.position += new Vector3(0, this.FlyHeight(), 0);

        MoveComponent moveComp = this.GetComponent<MoveComponent>();
        if (moveComp != null)
        {
            moveComp.SetPath(p);
        }
        PlayMoveAnimation();
    }

    public void PlayMoveAnimation()
    {
        BehaviourComponent bhmgr = this.GetComponent<BehaviourComponent>();
        bhmgr.ChangeState<BehaviourState_Run>();

        //if (aniBody != null && animationMove != null && animationMove.Length > 0)
        //{
        //    aniBody.Play(animationMove[Random.Range(0, animationMove.Length - 1)].name);
        //}
    }

    public float PlayScore()
    {
        float duration = 0;

        //if (aniBody != null && animationScore != null && animationScore.Length > 0)
        //{
        //    int rand = Random.Range(0, animationDead.Length - 1);
        //    aniBody.CrossFade(animationScore[rand].name);
        //    duration = animationScore[rand].length;
        //}

        //if (audioScore != null)
        //{
        //    AudioManager.PlaySound(audioScore, thisT.position);
        //    duration = Mathf.Max(audioScore.length, duration);
        //}

        return duration;
    }

    public override void ReachDestination()
    {
        Debug.LogError("抵达终止点，开始战斗！！！");
    }

    public void Score()
    {
        //if (scoreEffect != null) ObjectPoolManager.Spawn(scoreEffect, thisT.position, Quaternion.identity);
        float duration = PlayScore();
    }

    public float PlayDead()
    {
        float duration = 0;

        if (aniBody != null)
        {
            aniBody.Stop();
        }

        //if (aniBody != null && animationDead != null && animationDead.Length > 0)
        //{
        //    int rand = Random.Range(0, animationDead.Length - 1);
        //    aniBody.CrossFade(animationDead[rand].name);
        //    duration = animationDead[rand].length;
        //}

        //if (audioDead != null)
        //{
        //    AudioManager.PlaySound(audioDead, thisT.position);
        //    duration = Mathf.Max(audioDead.length, duration);
        //}

        return duration;
    }

    public override void OnDie()
    {

        base.OnDie();

        BufferStateData bsData = new BufferStateData();
        bsData.type = StateType.Death;
        bsData.stunDuration = 3;  //默认死亡动画3秒，3秒后自动清除尸体
        this.buffSateMgr.AddState(bsData);

        //获得金币
        //GameControl.GainResource(this.BaseData.dropRes);
        //添加死亡状态


        //uint spawnNumber = ServerData.deadSpawnNum;
        //if(spawnNumber > 0)
        //{
        //    SpawnManager.AddActiveUnit(waveID, (int)spawnNumber);

        //    for (int i = 0; i < spawnNumber; i++)
        //    {
        //        //generate a small offset position within the grid size so not all creep spawned on top of each other and not too far apart
        //        float allowance = BuildManager.GetGridSize() / 2;

        //        float x = Random.Range(-allowance, allowance);
        //        float y = Random.Range(-allowance, allowance);

        //        Vector3 pos = Trans.position + new Vector3(x, 0, y);

        //        s_MonsterData _data = new s_MonsterData();
        //        _data.InitData(ServerData.deadSpawnId);
        //        _data.position = pos;
        //        _data.rotation = Trans.rotation;
        //        Monster monster = EntitesManager.Instance.CreateMonster(_data);
        //        monster.Init(path, waveID);
        //        monster.ModelObj.transform.localScale = Vector3.one * 0.5f;
        //        monster.AddSchedule(0.1f, () =>
        //        {
        //            monster.ResumeParentPath(wpMode, wp, wpCounter, currentPS, subPath, currentPathID, subWPCounter);
        //        });

        //    }
        //}
    }

    public void ResumeParentPath(bool wpM, List<Vector3> w, int wpC, PathSection cPS, List<Vector3> sP, int pID, int subWPC)
    {


    }

    public void PlayHit()
    {
        //Debug.Log("Play hit");
        //if (aniBody != null && animationHit != null && animationHit.Length > 0)
        //{
        //    aniBody.CrossFade(animationHit[Random.Range(0, animationHit.Length - 1)].name);
        //}

        //if (audioHit != null) AudioManager.PlaySound(audioHit, thisT.position);
    }

    public override void OnUpdate(float dt)
    {
        base.OnUpdate(dt);
    }


    public void OnDestroy()
    {
        base.OnDestroy();
    }
}
