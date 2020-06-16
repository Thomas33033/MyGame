//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;

//public class UI_Main_old : UIPanelBase
//{

//    public float fastForwardSpeed = 3;
//    public bool alwaysEnableNextButton = true;

//    public Text txtGeneral;
//    public Text txtMessage;

//    public Button spawnButton;
//    public Button ffButton;
//    public Button pauseButton;
//    public Button upgradeButton;
//    public Button sellButton;
//    public Button menuButton;
//    public Button restartButton;
//    public Button nextLevelButton;

//    public Image generalBox;
//    public Image towerImage;
//    public Text towerText;

//    public Image SelectedTowerImage;
//    public Text SelectedTowerText;

//    private bool paused = false;
//    private UnitTower currentSelectedTower;
//    private bool buildMenu = false;
//    private Rect UIButtonRect;
//    private Rect towerUIRect;
//    private Rect buildListRect;
//    private Rect generalRect;
//    private bool winLostFlag = false;

    

//    // Use this for initialization
//    public void Awake () {
//        base.Awake();
//        GameTools.AddClickEvent(spawnButton.gameObject, OnSpawnButtonClick);
//        GameTools.AddClickEvent(ffButton.gameObject, OnFastForwardButtonClick);
//        GameTools.AddClickEvent(pauseButton.gameObject, OnPauseButtonClick);
//        GameTools.AddClickEvent(upgradeButton.gameObject, OnUpgradeButtonClick);
//        GameTools.AddClickEvent(sellButton.gameObject, OnSellButtonClick);
//        GameTools.AddClickEvent(menuButton.gameObject, OnMenuButtonClick);
//        GameTools.AddClickEvent(restartButton.gameObject, OnRestartButtonClick);
//        GameTools.AddClickEvent(nextLevelButton.gameObject, OnNextLevelButtonClick);

//        SpawnManager.ClearForSpawningE += ClearForSpawning;
//        SpawnManager.WaveStartSpawnE += UpdateGeneralUIText;

//        GameControl.GameOverE += GameOver;

//        GameControl.ResourceE += UpdateGeneralUIText;
//        GameControl.LifeE += UpdateGeneralUIText;

//        UnitTower.BuildCompleteE += TowerBuildComplete;
//        UnitTower.DestroyE += TowerDestroy;


//        if (!this.enabled)
//        {
//            gameObject.SetActive(false);
//        }	
//    }

//    void ClearForSpawning(bool flag)
//    {
//        spawnButton.enabled = flag;
//    }

//    void UpdateGeneralUIText(int val)
//    {
//        UpdateGeneralUIText();
//    }
//    void UpdateGeneralUIText()
//    {
//        string info = "";

//        info += "Life: " + GameControl.GetPlayerLife() + "\n\n";

//        Resource[] resourceList = GameControl.GetResourceList();
//        foreach (Resource rsc in resourceList)
//        {
//            info += rsc.name + ": " + rsc.value + "\n";
//        }
//        info += "\n";

//        info += "Wave: " + SpawnManager.GetCurrentWave() + "/" + SpawnManager.GetTotalWave();

//        txtGeneral.text = info;
//    }

//    void TowerBuildComplete(UnitTower tower)
//    {
//        if (currentSelectedTower == tower)
//        {
//            if (!currentSelectedTower.IsLevelCapped())
//                upgradeButton.enabled = true;

//            UpdateSelectedTowerText();
//        }
//    }

//    void TowerDestroy(UnitTower tower)
//    {
//        if (currentSelectedTower == tower)
//        {
//            DisableSelectedTowerUI();
//        }
//    }

//    void GameOver(bool flag)
//    {
//        winLostFlag = flag;

//        txtGeneral.text = "";
//        pauseButton.enabled = false;

//        if (winLostFlag)
//        {
//            GameMessage.DisplayMessage("level completed");
//            txtMessage.text = "level completed";
//        }
//        else
//        {
//            GameMessage.DisplayMessage("level failed");
//            txtMessage.text = "level failed";
//        }

//        EnableGeneralMenu(winLostFlag);
//    }

//    void EnableGeneralMenu(bool winLostFlag)
//    {
//        generalBox.enabled = true;
//        menuButton.enabled = true;
//        GameTools.AddClickEvent(menuButton.gameObject, OnMenuButtonClick);

//        restartButton.enabled = true;
//        GameTools.AddClickEvent(menuButton.gameObject, OnRestartButtonClick);

//        if (winLostFlag || alwaysEnableNextButton)
//        {
//            //nextLvlButton.enabled = true;
//            //nextLvlButton.callBackFunc = this.OnNextLvlButton;
//            //StartCoroutine(nextLvlButton.Update());
//        }

//        generalRect = new Rect(Screen.width / 2 - 70, Screen.height / 2 - 75, 140, 150);
//    }

//    // Update is called once per frame
//    public void Update()
//    {
		
//    }

//    public void OnSpawnButtonClick()
//    {
//        if (GameControl.gameState != _GameState.Ended)
//        {
//            //if(SpawnManager.Spawn()) spawnButton.buttonObj.enabled=false;
//            SpawnManager.Spawn();
//        }
//    }
//    public void OnFastForwardButtonClick()
//    {
//        if (Time.timeScale == 1) Time.timeScale = fastForwardSpeed;
//        else Time.timeScale = 1;
//    }

//    public void OnPauseButtonClick()
//    {
//        TogglePaused();
//    }

//    public void OnUpgradeButtonClick()
//    {
//        if (currentSelectedTower != null)
//        {
//            //Debug.Log("upgrade tower");
//            upgradeButton.enabled = false;
//            currentSelectedTower.Upgrade();
//            UpdateSelectedTowerText();
//        }
//    }

//    public void OnSellButtonClick()
//    {
//        if (currentSelectedTower != null)
//        {
//            currentSelectedTower.Sell();
//            sellButton.enabled = false;
          
//        }
//    }

//    public void OnMenuButtonClick()
//    {

//    }

//    public void OnRestartButtonClick()
//    {

//    }

//    public void OnNextLevelButtonClick()
//    {

//    }


//    void UpdateSelectedTowerText()
//    {
//        if (currentSelectedTower != null)
//        {
//            string towerInfo = "";

//            towerInfo += currentSelectedTower.unitName + "\n";

//            int lvl = currentSelectedTower.GetLevel();
//            towerInfo += "Level: " + lvl.ToString() + "\n\n\n";



//            _TowerType type = currentSelectedTower.type;

//            if (type == _TowerType.ResourceTower)
//            {

//                string val = currentSelectedTower.GetDamage().ToString();
//                string cd = currentSelectedTower.GetCooldown().ToString("f1");

//                string rsc = "Increase rsc by " + val + " for every cd " + cd + "sec\n";
//                rsc = FormatString(rsc);

//                towerInfo += rsc;

//            }
//            else if (type == _TowerType.SupportTower)
//            {

//                BuffStat buffInfo = currentSelectedTower.GetBuff();

//                string buff = "";
//                buff += "Buff damage by " + (buffInfo.damageBuff * 100).ToString("f1") + "%\n";
//                buff += "Buff range by " + (buffInfo.rangeBuff * 100).ToString("f1") + "%\n";
//                buff += "Reduce CD by " + (buffInfo.cooldownBuff * 100).ToString("f1") + "%\n";

//                towerInfo += buff;

//            }
//            else if (type == _TowerType.TurretTower || type == _TowerType.AOETower)
//            {

//                if (currentSelectedTower.GetDamage() > 0)
//                    towerInfo += "Damage: " + currentSelectedTower.GetDamage().ToString("f1") + "\n";
//                if (currentSelectedTower.GetCooldown() > 0)
//                    towerInfo += "Cooldown: " + currentSelectedTower.GetCooldown().ToString("f1") + "sec\n";
//                if (type == _TowerType.TurretTower && currentSelectedTower.GetAoeRadius() > 0)
//                    towerInfo += "AOE Radius: " + currentSelectedTower.GetAoeRadius().ToString("f1") + "\n";
//                if (currentSelectedTower.GetStunDuration() > 0)
//                    towerInfo += "Stun target for " + currentSelectedTower.GetStunDuration().ToString("f1") + "sec\n";

//                Dot dot = currentSelectedTower.GetDot();
//                float totalDot = dot.damage * (dot.duration / dot.interval);
//                if (totalDot > 0)
//                {
//                    string dotInfo = "Cause " + totalDot.ToString("f1") + " damage over the next " + dot.duration + " sec\n";
//                    dotInfo = FormatString(dotInfo);

//                    towerInfo += dotInfo;
//                }

//                Slow slow = currentSelectedTower.GetSlow();
//                if (slow.duration > 0)
//                {
//                    string slowInfo = "Slow target by " + (slow.slowFactor * 100).ToString("f1") + "% for " + slow.duration.ToString("f1") + "sec\n";
//                    slowInfo = FormatString(slowInfo);

//                    towerInfo += slowInfo;
//                }
//            }

//            string desp = FormatString("\n" + currentSelectedTower.GetDescription());

//            towerInfo += desp;


//            towerInfo += "\n\n\n";
//            Resource[] rscList = GameControl.GetResourceList();
//            int count = 0;

//            if (!currentSelectedTower.IsLevelCapped() && currentSelectedTower.IsBuilt())
//            {
//                //~ int cost=currentSelectedTower.GetUpgradeCost();
//                //~ towerInfo+="UpgradeCost: "+cost.ToString()+resourceName+"\n";

//                int[] cost = currentSelectedTower.GetCost();

//                //GUI.Label(new Rect(startX, startY, 150, 25), "Upgrade Cost:");
//                towerInfo += "Upgrade Cost:\n ";


//                for (int i = 0; i < cost.Length; i++)
//                {
//                    if (cost[i] > 0)
//                    {
//                        count += 1;
//                        //if(rscList[i].icon!=null){
//                        //	GUI.Label(new Rect(startX+10, startY+count*20, 25, 25), rscList[i].icon);
//                        //	GUI.Label(new Rect(startX+10+25, startY+count*20+3, 150, 25), "- "+cost[i].ToString());
//                        //}
//                        //else{
//                        towerInfo += "- " + cost[i].ToString() + rscList[i].name + "  ";
//                        //}
//                    }
//                }

//                towerInfo += "\n\n";
//            }

//            int[] sellValue = currentSelectedTower.GetTowerSellValue();

//            //GUI.Label(new Rect(startX, startY, 150, 25), "Upgrade Cost:");
//            towerInfo += "Sell Cost:\n ";

//            count = 0;
//            for (int i = 0; i < sellValue.Length; i++)
//            {
//                if (sellValue[i] > 0)
//                {
//                    count += 1;
//                    //if(rscList[i].icon!=null){
//                    //	GUI.Label(new Rect(startX+10, startY+count*20, 25, 25), rscList[i].icon);
//                    //	GUI.Label(new Rect(startX+10+25, startY+count*20+3, 150, 25), "- "+cost[i].ToString());
//                    //}
//                    //else{
//                    towerInfo += "+ " + sellValue[i].ToString() + rscList[i].name + "  ";
//                    //}
//                }
//            }
//            //towerInfo+="SellValue: "+sellValue.ToString()+"\n\n";


//            SelectedTowerText.text = towerInfo;
//        }
//    }




//    void TogglePaused()
//    {
//        paused = !paused;
//        if (paused)
//        {
//            Time.timeScale = 0;
//            txtMessage.text = "Game Paused";
//            spawnButton.enabled = false;
//            ffButton.enabled = false;

//            if (currentSelectedTower != null)
//            {
//                OnUnselectTower();
//            }
//            if (buildMenu)
//            {
//                BuildManager.ClearIndicator();
//                DisableBuildMenu();
//            }

//            EnablePauseMenu();
//        }
//        else
//        {
//            Time.timeScale = 1;
//            txtMessage.text = "";
//            spawnButton.enabled = true;
//            ffButton.enabled = true;

//            DisablePauseMenu();
//        }
//    }

//    void EnablePauseMenu()
//    {
//        generalBox.enabled = true;
//        menuButton.enabled = true;
//        GameTools.AddClickEvent(menuButton.gameObject, OnMenuButtonClick);

//        restartButton.enabled = true;
//        GameTools.AddClickEvent(menuButton.gameObject, OnRestartButtonClick);
//        generalRect = new Rect(Screen.width / 2 - 70, Screen.height / 2 - 75, 140, 150);
//    }

//    void DisablePauseMenu()
//    {
//        generalBox.enabled = false;
//        menuButton.enabled = false;
//        restartButton.enabled = false;
//        //nextLvlButton.buttonObj.enabled=false;

//        generalRect = new Rect(0, 0, 0, 0);
//    }

//    private void DisableBuildMenu()
//    {
//        //foreach (GUIButton button in buildButtonList)
//        //{
//        //    button.buttonObj.enabled = false;
//        //}

//        //buildListRect = new Rect(0, 0, 0, 0);

//        //buildMenu = false;

//    }


//    void OnUnselectTower()
//    {
//        currentSelectedTower = null;
//        DisableSelectedTowerUI();
//        GameControl.ClearSelection();
//    }

//    void DisableSelectedTowerUI()
//    {
//        //towerUIOn=false;

//        SelectedTowerImage.enabled = false;
//        SelectedTowerText.enabled = false;

//        upgradeButton.enabled = false;
//        sellButton.enabled = false;
//        towerUIRect = new Rect(0, 0, 0, 0);
//    }

//    public string FormatString(string s)
//    {
//        char[] delimitor = new char[1] { ' ' };
//        string[] words = s.Split(delimitor); //Split the string into seperate words
//        string result = "";
//        int runningLength = 0;
//        foreach (string word in words)
//        {
//            if (runningLength + word.Length + 1 <= 32)
//            {
//                result += " " + word;
//                runningLength += word.Length + 1;
//            }
//            else
//            {
//                result += "\n" + word;
//                runningLength = word.Length;
//            }
//        }

//        return result.Remove(0, 1); //Remove the first space
//    }
//}
