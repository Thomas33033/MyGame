using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEntity
{

    public Wave wave;
    private int waveId;
    private float curTimes;

    List<SubWaveEntity> list;

    public SpawnEntity(Wave p_wave, int p_waveId)
    {
        this.wave = p_wave;
        this.waveId = p_waveId;
        this.list = new List<SubWaveEntity>();
        foreach (SubWave subWave in this.wave.subWaves)
        {
            this.list.Add(new SubWaveEntity(subWave, this.wave, this.waveId));
        }
    }

    public void OnUpdate(float dt)
    {
        foreach (SubWaveEntity subWave in this.list)
        {
            subWave.OnUpdate(dt);
        }

        CheckSpawn(this.waveId);
    }


    void CheckSpawn(int waveID)
    {

        //bool allSpawned = true;
        //foreach (SubWave subWave in this.wave.subWaves)
        //{
        //    if (!subWave.spawned) allSpawned = false;
        //}


        //Debug.Log("wave " + (this.waveId - 1) + " has done spawning");

        //this.wave.spawned = true;

        //if (this.waveId < waves.Length)
        //{
        //    if (spawnMode == _SpawnMode.SkippableContinous || spawnMode == _SpawnMode.SkippableWaveCleared)
        //        canSpawn = true;

        //    if (WaveSpawnedE != null) WaveSpawnedE(waveID);

        //    if (ClearForSpawningE != null) ClearForSpawningE(canSpawn);
        //}

    }
}


public class SubWaveEntity
{
    private SubWave subWave;
    private Wave parentWave;
    private int waveID;

    private float curDelayTimes;
    private float curIntervalTimes;
    private bool playing = false;
    private int spawnCount = 0;

    public SubWaveEntity(SubWave p_subWave, Wave p_wave, int p_waveID)
    {
        this.subWave = p_subWave;
        this.parentWave = p_wave;
        this.waveID = p_waveID;
        this.curDelayTimes = p_subWave.delay;
        this.curIntervalTimes = 0;
        this.playing = true;

    }
    public void OnUpdate(float dt)
    {
        if (this.playing)
            this.SpawnSubwave(dt);
    }

    void SpawnSubwave(float dt)
    {
        this.curDelayTimes += dt;
        if (this.curDelayTimes < subWave.delay)
        {
            return;
        }

        this.curIntervalTimes += dt;
        if (this.curIntervalTimes < subWave.interval)
        {
            return;
        }

        if (this.spawnCount < this.subWave.count)
        {
            SpawnManager.Instance.SpawnFactory(this.subWave, this.waveID);
            parentWave.activeUnitCount += 1;
            this.spawnCount += 1;
            if (this.spawnCount == subWave.count)
            {
                this.playing = false;
            }
        }

        subWave.spawned = true;
    }
}