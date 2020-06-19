using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System;
using System.IO;
/// <summary>
/// 获取最新版本号
/// 1.对比版本号
/// 2.如果是大版本，则提示下载最新版本
/// 3.如果是小版本，则自动下载资源文件
/// </summary>
public class CheckVersionStep : ActionBase
{

    enum EVersionState
    {
        None,
        UpdateRes,
        DownNewApp,
    }



    public Action OnEndEvent;
    private bool bFinished = false;
    private string version = "";

    public CheckVersionStep(Action endEvent)
    {
        this.OnEndEvent = endEvent;

        //获取本地版本号
        version = PlayerPrefs.GetString("VersionCode", GameConfig.Version);

        TimeManager.Instance.StartCoroutine(CheckVersion());
    }

    public override bool IsDone()
    {
        return bFinished;
    }

    private EVersionState CompareVersion(string newVersion, string localVersion)
    {
        string[] newArray = newVersion.Split('.');
        string[] oldArray = localVersion.Split('.');

        if (newArray.Length == 4 && oldArray.Length == 4)
        {
            if (int.Parse(newArray[0]) > int.Parse(oldArray[0])
                || int.Parse(newArray[1]) > int.Parse(oldArray[1]))
            {
                return EVersionState.DownNewApp;
            }
            else
            {
                return EVersionState.UpdateRes;
            }
        }

        return EVersionState.UpdateRes;
    }

    IEnumerator CheckVersion()
    {
        string versionUrl = GameConfig.VersionCodeURL + "?v="+ UnityEngine.Random.Range(10000, 99999);
        UnityWebRequest request = new UnityWebRequest();
        request.uri = new Uri(versionUrl);
        request.timeout = 5;
        yield return request.SendWebRequest();
        if (request.isHttpError || request.isNetworkError)             
        {
            yield return new WaitForSeconds(1f);
            TimeManager.Instance.StartCoroutine(CheckVersion());
        }
        else 
        {
            string newVersion =  request.downloadHandler.text;
            EVersionState state = CompareVersion(newVersion, version);
            if (state == EVersionState.DownNewApp)
            {
                OnEndEvent();
            }
            else {
                bFinished = true;
            }
        }

    }

}
