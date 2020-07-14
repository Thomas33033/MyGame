using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
using System;
using System.IO;

/// <summary>
/// 更新并下载资源
/// </summary>
public class UpdateResStep : ActionBase
{
    private List<AssetBundleInfo> listDown;
    private Dictionary<string, AssetBundleInfo> dicServer;
    private Dictionary<string, AssetBundleInfo> dicLocal;

    private bool isSilent;
    private float allSize;
    private float downSize;

    private string versionCode;

    private Action<float, string> progressCallBackEvent;

    private bool bFinished = false;
    public UpdateResStep(Action<float, string> progressCallBackEvent)
    {
        this.progressCallBackEvent = progressCallBackEvent;
        StartDown();
    }

    public override bool IsDone()
    {
        return bFinished;
    }

    private void StartDown()
    {
        listDown = new List<AssetBundleInfo>();

        if (string.IsNullOrEmpty(GameConfig.AssetIP) == false)
        {
          TimeManager.Instance.StartCoroutine(CheckResources());
        }
        else
        {
            bFinished = true;
        }
    }


    private IEnumerator CheckResources()
    {

        UnityWebRequest webRequest = UnityWebRequest.Get(GameConfig.AssetIP + "/assetslist.txt?v=" 
            + UnityEngine.Random.Range(10000,99999));

        yield return webRequest.SendWebRequest();

        if (webRequest.isHttpError || webRequest.isNetworkError)
        {
            Debug.Log(webRequest.url + "\n" + webRequest.error);
            yield return new WaitForSeconds(5f);
            TimeManager.Instance.StartCoroutine(CheckResources());
        }
        else
        {
            string[] arr = webRequest.downloadHandler.text.Split('\n');

            string[] arr2 = arr[0].Split(',');
            string newVersionCode = arr2[0];
            string oldVersionCode = "0";
            float silentDownNum = 0f;
            if (arr2.Length > 1)
            {
                silentDownNum = float.Parse(arr2[1]);
            }

            dicServer = CreateAssetDictionary(arr);

            if (File.Exists(AssetsManager.AssetBundlePath + "/assetslist.txt") == true)
            {
                oldVersionCode = arr[0];
                dicLocal = CreateAssetDictionary(arr);
                Debug.Log("dicLocal CreateAssetDictionary:" + dicLocal.Count);
            }
            else
            {
                oldVersionCode = "0";
                dicLocal = new Dictionary<string, AssetBundleInfo>();
            }

            yield return new WaitForEndOfFrame();

            if (Tools.CompareVersion(newVersionCode, oldVersionCode))
            {
                foreach (var item in dicServer.Values)
                {
                    if (dicLocal.ContainsKey(item.name) == false)
                    {
                        allSize += item.size;
                        listDown.Add(item);
                    }
                    else if (dicLocal[item.name].md5 != item.md5)
                    {
                        allSize += item.size;
                        listDown.Add(item);
                    }
                    else if (dicLocal[item.name].level != item.level)
                    {
                        dicLocal[item.name].level = item.level;
                    }
                }

                isSilent = allSize <= silentDownNum;

                if (isSilent)
                {
                    TimeManager.Instance.StartCoroutine(DownAssets());
                }
                else
                {
                    //objMessage.SetActive(true);
                    //txtContent.text = Localization.Format("PopupUp", GetSize(allSize));

                    //btnCancel.onClick.AddListener(() =>
                    //{
                    //    SDKFly.SdkTool.LogInfo("RecordUpdateResouceEvent,1");
                    //    Application.Quit();
                    //});
                    //btnConfirm.onClick.AddListener(() =>
                    //{
                    //    objMessage.SetActive(false);
                    TimeManager.Instance.StartCoroutine(DownAssets());
                    //    SDKFly.SdkTool.LogInfo("RecordUpdateResouceEvent,2");
                    //});
                }
            }
            else
            {
                bFinished = true;
            }
        }
    }

    private Dictionary<string, AssetBundleInfo> CreateAssetDictionary(string[] arrServerList)
    {
        Dictionary<string, AssetBundleInfo> result = new Dictionary<string, AssetBundleInfo>();
        for (int i = 1; i < arrServerList.Length; i++)
        {
            if (string.IsNullOrEmpty(arrServerList[i]) == true)
                continue;
            string[] arr = arrServerList[i].Split(',');
            AssetBundleInfo ab = new AssetBundleInfo(int.Parse(arr[0]), arr[1], arr[2], float.Parse(arr[3]), int.Parse(arr[4]));
            result.Add(ab.name, ab);
        }
        return result;
    }

    private IEnumerator DownAssets()
    {
        Debug.Log("DownAssets");

        //if (isSilent)
        //{
        //    txtRes.text = Localization.Get("Init");
        //    progressBar.gameObject.SetActive(true);
        //    txtSpeed.gameObject.SetActive(false);
        //    txtSize.gameObject.SetActive(false);
        //    txtSize2.gameObject.SetActive(false);
        //    progressBar.value = 0f;
        //}
        //else
        //{
        //    txtRes.text = Localization.Get("Loading");
        //    txtSize2.text = "0%";
        //    txtSize.text = string.Format("{0}MB/{1}", (downSize).ToString("0.00"), GetSize(allSize));
        //    txtSpeed.text = "0KB/S";
        //    progressBar.value = 0;
        //    progressBar.gameObject.SetActive(true);
        //}

        float startTime = 0;
        for (int i = 0; i < listDown.Count; i++)
        {
            AssetBundleInfo ab = listDown[i];
            UnityWebRequest webRequest = UnityWebRequest.Get(GameConfig.AssetIP + "/" + ab.name + "_" + ab.md5);
            webRequest.SendWebRequest();
            startTime = Time.time;
            while (webRequest.isDone == false)
            {
                if (isSilent == false)
                {
                    //progressBar.value = (1f * (downSize + webRequest.downloadedBytes)) / allSize;
                    //txtSize.text = string.Format("{0}/{1}", GetSize(downSize + webRequest.downloadedBytes), GetSize(allSize));
                    //txtSpeed.text = string.Format("{0}/S", GetSize(webRequest.downloadedBytes / (Time.time - startTime)));
                    //txtSize2.text = (progressBar.value * 100).ToString("0.00") + "%";
                }
                yield return new WaitForEndOfFrame();
            }

            if (webRequest.isHttpError || webRequest.isNetworkError)
            {
                Debug.Log(ab.name + " " + webRequest.error + " " + webRequest.url);
                yield return new WaitForSeconds(5f);
                i--;
                continue;
            }

            downSize += webRequest.downloadedBytes;

            dicLocal[ab.name] = ab;

        }
        SaveLocal();
        bFinished = true;
    }


    private void SaveLocal()
    {
        string txt = versionCode;

        foreach (var item in dicLocal.Values)
        {
            txt += "\n" + item.type + "," + item.name + "," + item.md5 + "," + item.size + "," + item.level;
        }

        File.WriteAllText(AssetsManager.AssetBundlePath + "/assetslist.txt", txt);
    }
}

public class AssetBundleInfo
{
    public string name;
    public string md5;
    public float size;
    public int type;
    public int level;

    public AssetBundleInfo(int type, string name, string md5, float size, int level)
    {
        this.type = type;
        this.name = name;
        this.size = size;
        this.md5 = md5;
        this.level = level;
    }
}
