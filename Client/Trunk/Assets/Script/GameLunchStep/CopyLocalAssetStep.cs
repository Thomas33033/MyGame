using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System;

public class CopyLocalAssetStep : ActionBase
{
    private Action<float, string> progressCallBackEvent;

    private bool bFinished = false;

    public CopyLocalAssetStep(Action<float, string> progressCallBack)
    {
        this.progressCallBackEvent = progressCallBack;
    }

    public override bool IsDone()
    {
        return bFinished;
    }

    public void StartUpdate()
    {
        if (File.Exists(Path.Combine(ResourcesManager.AssetBundlePath, "assetslist.txt")) == false)
        {
           TimeManager.Instance.StartCoroutine(CopyStreaming());
        }
        else
        {
            bFinished = true;
        }
    }


    private IEnumerator CopyStreaming()
    {
        string assetListName = "assetslist.txt";
        yield return new WaitForEndOfFrame();
        string wwwStreamingAssetBundlePath = Path.Combine(ResourcesManager.StreamingAssetsPath, "AssetBundle");
#if UNITY_ANDROID
        UnityWebRequest webRequest = UnityWebRequest.Get(Path.Combine(StreamingAssetsPath, assetListName));

        yield return webRequest.SendWebRequest();
        if (webRequest.isHttpError || webRequest.isNetworkError)
        {
            Debug.Log("(www.error) " + webRequest.error);
            StartDown();
            yield break;
        }

        string txtAssetslist = webRequest.downloadHandler.text;
#else
        string txtAssetslist = File.ReadAllText(Path.Combine(ResourcesManager.StreamingAssetsPath, "AssetBundle", assetListName));
#endif

        string txtCurAssetslist = "0";
        string assetListPath = Path.Combine(ResourcesManager.AssetBundlePath, "assetslist.txt");
        if (File.Exists(assetListPath) == true)
        {
            txtCurAssetslist = File.ReadAllText(assetListPath);
        }

        string[] arrCurAssetslist = txtCurAssetslist.Split('\n');
        string[] arrAssetslist = txtAssetslist.Split('\n');

        if (arrAssetslist.Length > 1 && uint.Parse(arrCurAssetslist[0]) < uint.Parse(arrAssetslist[0]))
        {
            Dictionary<string, string> dicAssetBundleMd5 = new Dictionary<string, string>();
            for (int i = 1; i < arrCurAssetslist.Length; i++)
            {
                if (string.IsNullOrEmpty(arrCurAssetslist[i]) == true)
                    continue;
                string[] arrData = arrCurAssetslist[i].Split(',');
                dicAssetBundleMd5[arrData[1]] = arrData[2];
            }

            for (int i = 1; i < arrAssetslist.Length; i++)
            {
                if (string.IsNullOrEmpty(arrAssetslist[i]) == true)
                    continue;

                string[] arrData = arrAssetslist[i].Split(',');


                progressCallBackEvent((i + 1f) / arrAssetslist.Length, "");


                string fileName = arrData[1];

                if (dicAssetBundleMd5.ContainsKey(fileName) == true && arrData[2] == dicAssetBundleMd5[fileName])
                {
                    continue;
                }

#if UNITY_ANDROID
                webRequest = UnityWebRequest.Get(Path.Combine(wwwStreamingAssetBundlePath, fileName));
                //www = new WWW(Path.Combine(wwwStreamingAssetBundlePath, fileName));
                yield return webRequest.SendWebRequest();
                if (webRequest.isHttpError || webRequest.isNetworkError)
                {
                    i--;
                    continue;
                }
                File.WriteAllBytes(Path.Combine(ResourcesManager.assetBundlePath, fileName), webRequest.downloadHandler.data);
#else
                File.Copy(Path.Combine(wwwStreamingAssetBundlePath, fileName), 
                    Path.Combine(ResourcesManager.AssetBundlePath, fileName), true);
#endif

                yield return new WaitForFixedUpdate();
            }

            File.WriteAllText(Path.Combine(ResourcesManager.AssetBundlePath, "assetslist.txt"), txtAssetslist);
        }
        bFinished = true;
    }
}