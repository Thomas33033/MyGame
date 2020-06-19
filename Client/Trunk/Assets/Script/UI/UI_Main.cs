using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cherry.AssetBundlePacker;

public class UI_Main : UIPanelBase
{

    public GameObject btnPause;
    public GameObject btnSpawn;
    public GameObject towerItem;
    public Text txtGeneral;
    public Button btnUpdate;
    public Text txtSelectedInfo;

    public Animator animator;

    private List<UI_Tower> mlstUITower = new List<UI_Tower>();

    private bool enableSpawnButton = true;
    private bool winLostFlag = false;
    private Tower currentSelectedTower;

    void Awake()
    {
        base.Awake();
    }

	// Use this for initialization
	void Start () {
        towerItem.gameObject.SetActive(false);
        UITools.AddClickEvent(btnSpawn, OnSpawnButton);
        UITools.AddClickEvent(btnPause, OnPauseButton);
        BuildMenuAllTowersFix();
	}

    void OnEnable()
    {
        GameControl.GameOverE += GameOver;
        GameControl.ResourceE += UpdateGeneralUIText;
        GameControl.LifeE += UpdateGeneralUIText;

        //Tower.BuildCompleteE += TowerBuildComplete;
        //Tower.DestroyE += TowerDestroy;
    }

    void OnDisable()
    {
        GameControl.GameOverE -= GameOver;
        GameControl.ResourceE -= UpdateGeneralUIText;
        GameControl.LifeE -= UpdateGeneralUIText;

        //Tower.BuildCompleteE -= TowerBuildComplete;
        //Tower.DestroyE -= TowerDestroy;
    }

    void BuildMenuAllTowersFix()
    {
        uint[] towerList = BuildManager.GetTowerList();

        for (int i = 0; i < towerList.Length; i++)
        {
            GameObject towerObj = GameObject.Instantiate(towerItem,towerItem.transform.parent);
            towerObj.gameObject.SetActive(true);
            UI_Tower uiTower = towerObj.gameObject.GetOrAddComponent<UI_Tower>();
            uiTower.InitData(towerList[i]);
            mlstUITower.Add(uiTower);
        }

    }

	// Update is called once per frame
	void Update () {
		
	}

    void OnSpawnButton()
    {
        //if (GameControl.gameState != EGameState.Ended)
        //{
        //    SpawnManager.Instance.Spawn();
        //}
        LuaManager.Instance.CallFunction("Main.Start");
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

    void ClearForSpawning(bool flag)
    {
        enableSpawnButton = flag;
        btnSpawn.GetComponent<Button>().enabled = flag;
    }
    void GameOver(bool flag)
    {
        winLostFlag = flag;
        txtGeneral.text = "";
        btnPause.GetComponent<Button>().enabled = false;

        if (winLostFlag)
        {
            GameMessage.DisplayMessage("level completed");
        }
        else
        {
            GameMessage.DisplayMessage("level failed");
        }

        EnableGeneralMenu(winLostFlag);
    }

    void EnableGeneralMenu(bool winLostFlag)
    {
        //generalBox.enabled = true;
        //menuButton.buttonObj.enabled = true;
        //menuButton.callBackFunc = this.OnMenuButton;
        //StartCoroutine(menuButton.Update());

        //restartButton.buttonObj.enabled = true;
        //restartButton.callBackFunc = this.OnRestartButton;
        //StartCoroutine(restartButton.Update());

        //if (winLostFlag || alwaysEnableNextButton)
        //{
        //    nextLvlButton.buttonObj.enabled = true;
        //    nextLvlButton.callBackFunc = this.OnNextLvlButton;
        //    StartCoroutine(nextLvlButton.Update());
        //}

        //generalRect = new Rect(Screen.width / 2 - 70, Screen.height / 2 - 75, 140, 150);
    }

    void TowerBuildComplete(Tower tower)
    {
        if (currentSelectedTower == tower)
        {
            if (!currentSelectedTower.IsLevelCapped())
                btnUpdate.enabled = true;

            UpdateSelectedTowerText();
        }
    }

    void UpdateSelectedTowerText()
    {
        //if (currentSelectedTower != null)
        //{
        //    string towerInfo = "";

        //    towerInfo += currentSelectedTower.config.Name + "\n";
        //    uint lvl = currentSelectedTower.CurData.level;
        //    towerInfo += "等级: " + lvl.ToString() + "\n\n\n";

        //    _TowerType type = currentSelectedTower.type;

        //    if (type == _TowerType.ResourceTower)
        //    {
        //        string val = currentSelectedTower.CurData.resOutputNum.ToString();
        //        string cd = currentSelectedTower.CurData.baseStat.cooldown.ToString("f1");
        //        string rsc = "增加资源" + val + " 每次CD时间为" + cd + "秒\n";
        //        rsc = FormatString(rsc);
        //        towerInfo += rsc;
        //    }
        //    else if (type == _TowerType.SupportTower)
        //    {
        //        BuffStat buffInfo = currentSelectedTower.CurData.baseStat.buff;
        //        string buff = "";
        //        buff += "Buff伤害: " + (buffInfo.damageBuff * 100).ToString("f1") + "%\n";
        //        buff += "Buff范围: " + (buffInfo.rangeBuff * 100).ToString("f1") + "%\n";
        //        buff += "减少CD: " + (buffInfo.cooldownBuff * 100).ToString("f1") + "%\n";
        //        towerInfo += buff;
        //    }
        //    else if (type == _TowerType.TurretTower || type == _TowerType.AOETower)
        //    {
        //        float cooldown = currentSelectedTower.CurData.baseStat.cooldown;
        //        CfgSkillData skillCfg = currentSelectedTower.GetComponent<RoutineComponent>().skillData;
        //        float damage = skillCfg.Mdamage_value;
        //        float aoeRadius = skillCfg.AoeRadius;
        //        float stunDuration = skillCfg.StunTime;
        //        Dot dot = new Dot(skillCfg.Dot);
        //        Slow slow = new Slow(skillCfg.Slow);

        //        if (damage > 0)
        //            towerInfo += "伤害: " + damage.ToString("f1") + "\n";
        //        if (cooldown > 0)
        //            towerInfo += "冷却: " + cooldown.ToString("f1") + "sec\n";
        //        if (type == _TowerType.TurretTower && aoeRadius > 0)
        //            towerInfo += "AOE 半径: " + aoeRadius.ToString("f1") + "\n";
        //        if (stunDuration > 0)
        //            towerInfo += "眩晕时间: " + stunDuration.ToString("f1") + "秒\n";

        //        float totalDot = dot.damage * (dot.duration / dot.interval);
        //        if (totalDot > 0)
        //        {
        //            string dotInfo = "受到 " + totalDot.ToString("f1") + " 伤害 在之后的" + dot.duration + " 秒内\n";
        //            dotInfo = FormatString(dotInfo);
        //            towerInfo += dotInfo;
        //        }

        //        if (slow.duration > 0)
        //        {
        //            string slowInfo = "减速： " + (slow.slowFactor * 100).ToString("f1") + "%  持续时间: " + slow.duration.ToString("f1") + "秒\n";
        //            slowInfo = FormatString(slowInfo);
        //            towerInfo += slowInfo;
        //        }
        //    }

        //    string desp = FormatString("\n" + currentSelectedTower.config.Description);

        //    towerInfo += desp;

        //    towerInfo += "\n\n\n";

        //    int count = 0;

        //    if (!currentSelectedTower.IsLevelCapped() && currentSelectedTower.IsBuilt())
        //    {
        //        ResItem cost = currentSelectedTower.CurData.costRes;
        //        towerInfo += "升级 花费:\n ";
        //        ItemData itemConfigData = ConfigManager.Instance.GetResourceConfig(cost.itemId);
        //        count += 1;
        //        towerInfo += "- " + itemConfigData.ResName + "  ";
        //        towerInfo += "\n";
        //    }

        //    ResItem[] sellitems = currentSelectedTower.GetTowerSellValue();
        //    towerInfo += "Sell Cost:\n ";
        //    count = 0;
        //    for (int i = 0; i < sellitems.Length; i++)
        //    {
        //        count += 1;
        //        ItemData itemConfigData = ConfigManager.Instance.GetResourceConfig(sellitems[i].itemId);
        //        towerInfo += "+ " + itemConfigData.ResName;
        //    }
            
        //    txtSelectedInfo.text = towerInfo;
        //}
    }
    public string FormatString(string s)
    {
        //char[] delimitor = new char[1] { ' ' };
        //string[] words = s.Split(delimitor); //Split the string into seperate words
        //string result = "";
        //int runningLength = 0;
        //foreach (string word in words)
        //{
        //    if (runningLength + word.Length + 1 <= 32)
        //    {
        //        result += " " + word;
        //        runningLength += word.Length + 1;
        //    }
        //    else
        //    {
        //        result += "\n" + word;
        //        runningLength = word.Length;
        //    }
        //}

        //return result.Remove(0, 1); //Remove the first space
        return "1";
    }
    void TowerDestroy(Tower tower)
    {
        if (currentSelectedTower == tower) currentSelectedTower = null;
    }

    void UpdateGeneralUIText(int val)
    {
        UpdateGeneralUIText();
    }

    void UpdateGeneralUIText()
    {
        string info = "";
        info += "生命值: " + GameControl.GetPlayerLife() + "\n";

        BagItem goldItem = BagController.Instance.GetResource(Currency.Gold);
        BagItem silverItem = BagController.Instance.GetResource(Currency.Silver);

        info += goldItem.config.ResName + ": " + goldItem.totalNum + "\n";
        info += silverItem.config.ResName + ": " + silverItem.totalNum + "\n";

        info += "Wave: " + SpawnManager.Instance.GetCurrentWave() + "/" + SpawnManager.Instance.GetTotalWave();
        Debug.Log(info);
        txtGeneral.text = info;
    }
}

public class UI_Tower : MonoBehaviour
{
    public void InitData(uint p_configId)
    {
        CfgNpcData config = ConfigManager.Instance.GetData<CfgNpcData>((int)p_configId);
        Image image = this.gameObject.FindComponent<Image>("img_icon");
        Text txt_name = this.gameObject.FindComponent<Text>("txt_name");
        txt_name.text = config.Name;
        string path = ResPathHelper.UI_ITEM_PATH + string.Format( "{0}.psd", config.Headicon);

        Texture t = ResourcesManager.LoadAsset<Texture>(path);
        Texture2D icon = (Texture2D)t;
        image.sprite = Sprite.Create(icon, new Rect(0, 0, icon.width, icon.height), new Vector2(0.5f, 0.5f));
        UITools.AddClickEvent(image.gameObject, () =>
        {
           s_TowerData _data = new s_TowerData();
           _data.InitData(p_configId);
           BuildManager.BuildTowerDragNDrop(_data);
        });
    }

   
}
