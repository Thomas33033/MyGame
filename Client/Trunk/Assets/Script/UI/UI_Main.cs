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
    

    private List<UI_DragItem> mlstUITower = new List<UI_DragItem>();

    private List<UI_DragItem> mlstUIMonster = new List<UI_DragItem>();

    private bool enableSpawnButton = true;
    private bool winLostFlag = false;
    private Tower currentSelectedTower;

    void Awake()
    {
    }

	// Use this for initialization
	void Start () {
        towerItemObj.gameObject.SetActive(false);
        monsterItemObj.gameObject.SetActive(false);
        UITools.AddClickEvent(btnSpawn, OnSpawnButton);
        UITools.AddClickEvent(btnPause, OnPauseButton);
        OnInitTower();
        OnInitMonster();
    }

    void OnEnable()
    {
     
    }

    void OnDisable()
    {
      
    }

    void OnInitTower()
    {
        uint[] towerList = BuildManager.Instance.GetTower();
        for (int i = 0; i < towerList.Length; i++)
        {
            GameObject towerObj = GameObject.Instantiate(towerItemObj, towerItemObj.transform.parent);
            towerObj.gameObject.SetActive(true);
            UI_DragItem uiTower = towerObj.gameObject.GetOrAddComponent<UI_DragItem>();
            uiTower.InitData(towerList[i]);
            mlstUITower.Add(uiTower);
        }
    }

    void OnInitMonster()
    {
        uint[] monsterList = BuildManager.Instance.GetMonster();
        for (int i = 0; i < monsterList.Length; i++)
        {
            GameObject towerObj = GameObject.Instantiate(monsterItemObj, monsterItemObj.transform.parent);
            towerObj.gameObject.SetActive(true);
            UI_DragItem uiTower = towerObj.gameObject.GetOrAddComponent<UI_DragItem>();
            uiTower.InitData(monsterList[i]);
            mlstUITower.Add(uiTower);
        }
    }

    // Update is called once per frame
    void Update () {
		
	}

    void OnSpawnButton()
    {
        if (GameControl.gameState != EGameState.Ended)
        {
            SpawnManager.Instance.Spawn();
        }
    }

    void OnPauseButton()
    {
        if (MainGame.Instance.IsPause())
        {
            btnPause.transform.Find("Text").GetComponent<Text>().text = "暂停";
            MainGame.Instance.GameResume();
        }
        else
        {
            btnPause.transform.Find("Text").GetComponent<Text>().text = "开始";
            MainGame.Instance.GamePause();
        }
    }

}

public class UI_DragItem : MonoBehaviour
{
    public void InitData(uint npcCfgId)
    {
        CfgNpcData config = ConfigManager.Instance.GetData<CfgNpcData>((int)npcCfgId);
        Image image = this.gameObject.FindComponent<Image>("img_icon");
        Text txt_name = this.gameObject.FindComponent<Text>("txt_name");
        txt_name.text = config.Name;

        Debug.LogError(config.HeadIcon);
        Sprite sprite = ResourcesManager.LoadSprite(ResPathHelper.UI_ITEM_PATH + config.HeadIcon);
        image.sprite = sprite;
        UITools.AddClickEvent(image.gameObject, () =>
        {
           NpcData _data = new NpcData();
           _data.InitData(npcCfgId);
           BuildManager.BuildTowerDragNDrop(_data);
        });
    }

   
}
