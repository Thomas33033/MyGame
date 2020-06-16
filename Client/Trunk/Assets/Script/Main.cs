using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cherry.AssetBundlePacker;
using System;
using System.Text;

public class Main : MonoBehaviour {

    bool m_isReady = false;

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
        //初始化组件
        TimeManager.CreateSingleton();

        //检测版本

        //更新资源

        //初始化公共系统模块
        AssetBundleManager.CreateSingleton();

        LuaManager.CreateSingleton();
        //进入登陆模块

    }
	
	// Update is called once per frame
	void Update () {
        if (!m_isReady && AssetBundleManager.Instance.IsReady == true)
        {
            m_isReady = true;

            UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
            UI_Loading.instance.ShowProgress(() =>
            {
                GameObject.Destroy(UI_Loading.instance.gameObject);
            });
        }
    }


    void CheckVersion()
    {

    }
}
