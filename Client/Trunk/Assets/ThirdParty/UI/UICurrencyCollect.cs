using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UICurrencyCollect : MonoBehaviour
{

    [Header("[Count Param]")]
    public int maxIcon; //最大Icon数量
    public int minIcon; //最少Icon数量
    public int maxCurrency; //到达最大Icon数量时候的数量
    [Header("[Boomb Param]")]
    public float range; //炸出范围
    public float timeToBoom;  //飞出时间
    public float timeToStop;  //飞出后停滞的时间
    public float timeToCollect; //飞去固定位置的时间
    [Header("[UnityObj]")]

    public GameObject prefab; //复制的预设

    public Button btnPlay;

    //--------------- status --------------
    public bool isPlaying { get { return ani != null; } }
    //public Sprite sprite
    //{
    //    get { return prefab.sprite; }
    //    set { prefab.sprite = value; }
    //}
    public Vector3 startPos
    {
        get { return _startPos; }
        set { this._startPos = value; }
    }
    //--------------- private -------------
    Coroutine ani;
    Transform parentOfCurrency;
    Vector3 _startPos;


    private void Start()
    {
        _startPos = this.transform.position;
        prefab.gameObject.SetActive(false);

        UITools.AddClickEvent(this.btnPlay.gameObject, this.OnPlatBtnClick);
    }


    private void OnPlatBtnClick()
    {
        BoombToCollectCurrency(10, btnPlay.transform.position);
    }

    //--------------- function ------------
    public void BoombToCollectCurrency(int count, Vector3 to)
    {
        if (isPlaying) return;
        CreateParent();
        int coinCount = count / maxCurrency * maxIcon;
        coinCount = Mathf.Clamp(coinCount, minIcon, maxIcon);
        ani = StartCoroutine(_BoombToCollectCurrency(coinCount, to));
    }

    public void StopAni()
    {
        if (!isPlaying) return;
        StopAllCoroutines();
        ani = null;
        DestroyImmediate(parentOfCurrency.gameObject);
    }

    //----------- boomb ani -----------------

    IEnumerator _BoombToCollectCurrency(int count, Vector3 to)
    {
        List<Transform> currencyObj = new List<Transform>();
        for (int i = 0; i < count; i++)
        {
            currencyObj.Add(CreateCurrencyObj());
        }
        yield return StartCoroutine(ExporeOut(currencyObj));
        yield return new WaitForSeconds(timeToStop);
        yield return StartCoroutine(FlyToPos(currencyObj, to));
        Destroy(parentOfCurrency.gameObject);
        ani = null;
    }

    IEnumerator ExporeOut(List<Transform> currencyObj)
    {
        List<Vector3> toPos = new List<Vector3>();
        List<Vector3> fromPos = new List<Vector3>();
        foreach (Transform obj in currencyObj)
        {
            Vector3 pos = Random.insideUnitCircle;
            pos = pos * range + obj.transform.position;
            toPos.Add(pos);
            fromPos.Add(obj.transform.position);
        }

        float timer = 0;
        while (timer < timeToBoom)
        {
            timer += Time.deltaTime;
            for (int i = 0; i < currencyObj.Count; i++)
            {
                Vector3 from = fromPos[i];
                Vector3 to = toPos[i];
                Transform obj = currencyObj[i];
                Vector3 pos = Vector3.Lerp(currencyObj[i].transform.position, to, timer / timeToBoom);
                obj.transform.position = pos;
            }
            yield return null;
        }
    }
    IEnumerator FlyToPos(List<Transform> currencyObj, Vector3 to)
    {
        Vector3 toPos = to;
        List<Vector3> fromPos = new List<Vector3>();
        foreach (Transform obj in currencyObj)
            fromPos.Add(obj.transform.position);
        float timer = 0;
        while (timer < timeToCollect)
        {
            timer += Time.deltaTime;
            for (int i = 0; i < currencyObj.Count; i++)
            {
                Vector3 from = fromPos[i];
                Transform obj = currencyObj[i];
                Vector3 pos = Vector3.Lerp(from, to, timer / timeToCollect);
                obj.transform.position = pos;
            }
            yield return null;
        }
    }

    Transform CreateCurrencyObj()
    {
        GameObject obj = GameObject.Instantiate(prefab.gameObject);
        obj.SetActive(true);
        obj.transform.SetParent(parentOfCurrency);
        //obj.GetComponent<Image>().sprite = sprite;
        obj.transform.position = parentOfCurrency.transform.position;
        return obj.transform;
    }

    void CreateParent()
    {
        GameObject obj = new GameObject("parentOfCurrency");
        parentOfCurrency = obj.transform;
        //parentOfCurrency.SetParent(this.transform);
        parentOfCurrency.position = startPos;
    }

}
