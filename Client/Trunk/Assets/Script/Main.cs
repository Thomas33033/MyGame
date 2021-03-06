﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cherry.AssetBundlePacker;
using System;
using System.Text;
using Fight;

public class Main : MonoBehaviour {

    bool m_isReady = false;
    ActionSequence actionSequence = new ActionSequence();
    private void Awake()
    {
        GameObject.DontDestroyOnLoad(this.gameObject);
#if UNITY_STANDALONE
        Screen.SetResolution(1024, 640, false);
        Screen.fullScreen = false;
#endif
    }

    // Use this for initialization
    void Start () {
        //actionSequence.AddAction(new CopyLocalAssetStep(RefreshProgress));
        //actionSequence.AddAction(new CheckVersionStep(OnNewVersion));
        //actionSequence.AddAction(new UpdateResStep(RefreshProgress));
        //actionSequence.finishedOverEvent = OnEnterGame;
        //actionSequence.Start(this.gameObject);
        
        OnEnterGame();
        
        
        Debug.LogError( Fix64.FromRaw(300));
        m_Velocity = 100;
    }


    void OnNewVersion()
    { 
        //提示下载最新版本
    }

    void RefreshProgress(float progress,string tips)
    {
        UI_Loading.instance.SetProgress(progress);
    }

    void OnEnterGame()
    {
        actionSequence.ClearAction();
        actionSequence = null;

        TimeManager.CreateSingleton();
        AssetBundleManager.CreateSingleton(); 
        m_isReady = true;
    }

    float m_Velocity;
    // Update is called once per frame
    void Update () {

       // if (actionSequence != null) actionSequence.Update(Time.deltaTime);

        if (m_isReady)
        {
            m_isReady = false;
            LuaManager.CreateSingleton();

            UnityEngine.SceneManagement.SceneManager.LoadScene("Home");

            UI_Loading.instance.ShowProgress(() =>
            {

                GameObject.Destroy(UI_Loading.instance.gameObject);
            });
        }
    }

}
