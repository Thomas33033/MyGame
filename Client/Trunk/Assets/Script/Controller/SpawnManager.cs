using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Fight;

/// <summary>
/// 怪物生成管理器
/// </summary>
public class SpawnManager : Singleton<SpawnManager>
{
    public _SpawnMode spawnMode = _SpawnMode.Continous;

    private GamePath defaultPath;

    public Wave[] waves = new Wave[1];
    //可以繁衍
    private bool canSpawn = true;
    private int currentWave = 0;

    private float timeLastSpawn;
    private float waitDuration;

    private SpawnEntity spawnEntity = null;

    private bool bContinousTimedSpawn = false;
    private bool bWaveClearedSpawn = false;


    public void OnInit(GamePath path)
    {
        this.defaultPath = path;
        this.waves = new Wave[1];
        Wave wave = new Wave();
        wave.waveInterval = 5;
        waves[0] = wave; 

        wave.subWaves = new SubWave[1];
        SubWave subWave = new SubWave();

        subWave.monsterConfigId =2001;
        subWave.count = 1;
        subWave.interval = 1;
        subWave.delay = 5;
        subWave.path = null;
        subWave.overrideHP = 0;
        subWave.overrideMoveSpd = 0;
        wave.subWaves[0] = subWave;

    }


    public void OnUpdate(float dt)
    {
        if (GameControl.gameState == EGameState.Ended)
        {
            return;
        }

        if (this.bContinousTimedSpawn)
        {
            this.ContinousTimedSpawn();
        }

        if (this.bWaveClearedSpawn)
        {
            this.WaveClearedSpawn();
        }

        if(this.spawnEntity != null){
            this.spawnEntity.OnUpdate(dt);
        }
        
    }


    public bool Spawn()
    {
        if (GameControl.gameState == EGameState.Ended)
        {
            return false;
        }

        if (canSpawn)
        {
            if (currentWave >= waves.Length)
            {
                Debug.Log("这一波敌人已经生成完毕！");
                return false;
            }
            else
            {
                if (currentWave == 0)
                {
                    GameControl.gameState = EGameState.Started;
                    Debug.Log("游戏开始");
                }
                    
                if (spawnMode == _SpawnMode.Continous )
                {
                     this.bContinousTimedSpawn = true;
                }
            }
        }
        else
        {
            //anything else
            Debug.Log("SpawnManager is not ready for spawning next wave");
            return false;
        }

        return true;
    }

    void CheckIsWaveCleared(int waveID)
    {
        waves[waveID].activeUnitCount -= 1;
        if (waves[waveID].spawned && waves[waveID].activeUnitCount == 0)
        {
            waves[waveID].cleared = true;

            GameControl.GainResource(waves[waveID].resourceGain);

            AudioManager.PlayWaveClearedSound();

            if (currentWave < waves.Length)
            {
                canSpawn = true;
            }
        }
    }


    /// <summary>
    /// 持续生成，根据指定间隔
    /// </summary>
    void ContinousTimedSpawn()
    {
        this.waitDuration = waves[currentWave].waveInterval;
        this.timeLastSpawn = -waitDuration;
        if (currentWave < waves.Length)
        {
            if (Time.time - timeLastSpawn >= waitDuration)
            {
                timeLastSpawn = Time.time;
                waitDuration = waves[currentWave].waveInterval;
                SpawnWave();
            }
        }

        if (currentWave >= waves.Length)
        {
            this.bContinousTimedSpawn = false;
        }
    }

    void WaveClearedSpawn()
    {
        
        if (currentWave < waves.Length)
        {
            if (waves[currentWave - 1].cleared)
            {
                SpawnWave();
            }
        }
    }


    public void SpawnWave()
    {

        GameMessage.DisplayMessage("Incoming wave " + (this.currentWave + 1) + "!");

        AudioManager.PlayNewWaveSound();

        canSpawn = false;

        this.spawnEntity = new SpawnEntity(this.waves[currentWave], this.currentWave);

        currentWave += 1;
    }

    //check if all the spawning in one individual wave is done
    

    public void SpawnFactory(SubWave p_subWave, int p_waveID)
    {
        GamePath tempPath;
        if (p_subWave.path == null) tempPath = defaultPath;
        else tempPath = p_subWave.path;

        Vector3 pos = tempPath.waypoints[0].position;
        Quaternion rot = tempPath.waypoints[0].rotation;

        s_MonsterData _data = new s_MonsterData();
        _data.InitData(p_subWave.monsterConfigId);

        FightHeroData heroData = new FightHeroData();
        heroData.uid = Entity.GetUniqueId();
        heroData.Resource = string.Format("{0}/{1}.prefab", ResPathHelper.UI_NPC_PATH, _data.config.Resname);
        heroData.NodeId = 171;
        heroData.CostNodes = new int[] { 171 };
        heroData.roleId = 10001;
        heroData.Attack = 10;
        heroData.AttackSpeed = 1;
        heroData.BodyCrit = 1;
        heroData.BodyDodge = 1;
        heroData.BodyHit = 100;
        heroData.Defense = 5;
        heroData.HP = 100;
        heroData.MP = 50;
        heroData.CurMp = 0;
        heroData.CurHp = 100;
        heroData.Level = 1;
        heroData.MagicDefense = 5;
        heroData.MaxAnger = 10;
        heroData.MoveSpeed = 10;
        heroData.Range = 2;



        var fightSkillData = new FightSkillData();
        fightSkillData.skillID = 10001;
        fightSkillData.level = 1;
        heroData.SkillData = new FightSkillData[] { fightSkillData };

        FightScene.Instance.CreateRole(heroData,2);


       
        //
        //_data.position = pos;
        //_data.rotation = rot;
        //_data.speedFactor = 1;
        //Monster monster = EntitesManager.Instance.CreateMonster(_data);
        //monster.SetMovePath(tempPath, p_waveID);



    }
   

    

    public void AddActiveUnit(int waveID, int num)
    {
        this.waves[waveID].activeUnitCount += num;
    }

    public float _TimeNextSpawn()
    {
        return timeLastSpawn + waitDuration - Time.time;
    }

    public bool IsClearForSpawning()
    {
        return this.canSpawn;
    }

    public int GetCurrentWave()
    {
        return this.currentWave;
    }

    public int GetTotalWave()
    {
        return this.waves.Length;
    }

    public float TimeNextSpawn()
    {
        return this._TimeNextSpawn();
    }

    public _SpawnMode GetSpawnMode()
    {
        return this.spawnMode;
    }
}

[System.Serializable]
public class SubWave
{
    public GameObject unit;
    public uint monsterConfigId;
    public int count;
    public float interval = 1;
    public float delay;
    public GamePath path;
    public float overrideHP = 0;
    public float overrideMoveSpd = 0;

    [HideInInspector]
    public bool spawned = false;

    //	[HideInInspector] public UnitCreep[] unitList;
}

[System.Serializable]
public class Wave
{
    public SubWave[] subWaves = new SubWave[1];
    public float waveInterval;
    public ResItem resourceGain = new ResItem(Currency.Gold, 100);

    //	[HideInInspector] public List<UnitCreep> activeUnitList=new List<UnitCreep>();

    [HideInInspector]
    public int activeUnitCount = 0;

    [HideInInspector]
    public bool spawned = false;

    [HideInInspector]
    public bool cleared = false; 

}
