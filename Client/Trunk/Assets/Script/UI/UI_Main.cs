using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cherry.AssetBundlePacker;

public class UI_Main : UIPanelBase
{
    public GameObject btnPause;
    public GameObject btnSpawn;
    public GameObject towerItemObj;
    public GameObject monsterItemObj;
    
    //private List<UI_DragItem> mlstUIMineNpc = new List<UI_DragItem>();

    //private List<UI_DragItem> mlstUIEnemyNpc = new List<UI_DragItem>();

    void Awake()
    {

    }

	// Use this for initialization
	void Start () {
        //towerItemObj.gameObject.SetActive(false);
        //monsterItemObj.gameObject.SetActive(false);
        ////UITools.AddClickEvent(btnSpawn, OnSpawnButton);
        ////UITools.AddClickEvent(btnPause, OnPauseButton);
        //OnInitMyNpc();
        //OnInitEnemy();
    }

    void OnEnable()
    {
     
    }

    void OnDisable()
    {
      
    }

    void OnInitMyNpc()
    {
        //uint[] towerList = BuildManager.Instance.GetFriendNpc();
        //for (int i = 0; i < towerList.Length; i++)
        //{
        //    GameObject towerObj = GameObject.Instantiate(towerItemObj, towerItemObj.transform.parent);
        //    towerObj.gameObject.SetActive(true);
        //    UI_DragItem uiTower = towerObj.gameObject.GetOrAddComponent<UI_DragItem>();
        //    uiTower.InitData(towerList[i], ETeamType.Friend);
        //    mlstUIMineNpc.Add(uiTower);
        //}
    }

    void OnInitEnemy()
    {
        //uint[] monsterList = BuildManager.Instance.GetEnemyNpc();
        //for (int i = 0; i < monsterList.Length; i++)
        //{
        //    GameObject towerObj = GameObject.Instantiate(monsterItemObj, monsterItemObj.transform.parent);
        //    towerObj.gameObject.SetActive(true);
        //    UI_DragItem uiTower = towerObj.gameObject.GetOrAddComponent<UI_DragItem>();
        //    uiTower.InitData(monsterList[i], ETeamType.Enmery);
        //    mlstUIEnemyNpc.Add(uiTower);
        //}
    }

    // Update is called once per frame
    void Update () {
		
	}

    //void OnSpawnButton()
    //{
    //    if (GameControl.gameState != EGameState.Ended)
    //    {
    //        SpawnManager.Instance.Spawn();
    //    }
    //}

    //void OnPauseButton()
    //{
    //    if (MainGame.Instance.IsPause())
    //    {
    //        btnPause.transform.Find("Text").GetComponent<Text>().text = "暂停";
    //        MainGame.Instance.GameResume();
    //    }
    //    else
    //    {
    //        btnPause.transform.Find("Text").GetComponent<Text>().text = "开始";
    //        MainGame.Instance.GamePause();
    //    }
    //}

}

//public class UI_DragItem : MonoBehaviour
//{
//    public void InitData(uint npcCfgId, ETeamType teamId)
//    {
//        CfgNpcData config = ConfigManager.Instance.GetData<CfgNpcData>((int)npcCfgId);
//        Image image = this.gameObject.FindComponent<Image>("img_icon");
//        Text txt_name = this.gameObject.FindComponent<Text>("txt_name");
//        txt_name.text = config.Name;

//        Sprite sprite = LoadTools.LoadSprite("Atlas/Items", config.HeadIcon);
//        image.sprite = sprite;
//        UITools.AddClickEvent(image.gameObject, () =>
//        {

//        });
//    }
//}

