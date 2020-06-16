using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System;
/// <summary>
/// 获取最新版本号
/// 1.对比版本号
/// 2.如果是大版本，则提示下载最新版本
/// 3.如果是小版本，则自动下载资源文件
/// </summary>
public class CheckVersionStep : MonoBehaviour
{

    // Use this for initialization
    private string versionCode = "";
    void Start()
    {
        versionCode = "";
        TimeManager.Instance.StartCoroutine(CheckVersion());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator CheckVersion()
    {
        string versionUrl = config.VersionCodeURL + "?v="+ UnityEngine.Random.Range(10000, 99999);
        //WWW www = new WWW(versionUrl);
        Uri uri = new Uri(versionUrl);
        UnityWebRequest request = new UnityWebRequest();
        request.timeout = 5;
        yield return request.SendWebRequest();
        if (request.isHttpError || request.isNetworkError)             //如果其 请求失败，或是 网络错误
        {
            Debug.LogError(request.error); //打印错误原因
            yield return new WaitForSeconds(1f);
            TimeManager.Instance.StartCoroutine(CheckVersion());
        }
        else //请求成功
        {
            Debug.Log("请求成功");
            string newVersion =  request.downloadHandler.text;
        }

    }
}
