using UnityEngine;
using System.Collections;
using Fight;
using System;
using System.Collections.Generic;

public enum EGameState
{
    Idle, 
    Started, 
    Ended
}

[RequireComponent (typeof (LayerManager))]
public class GameControl : MonoBehaviour {
	
	public delegate void GameOverHandler(bool win); //true if win
	public static event GameOverHandler GameOverE;
	
	public delegate void ResourceHandler(); 
	public static event ResourceHandler ResourceE;
	
	public delegate void LifeHandler(); 
	public static event LifeHandler LifeE;

	static public EGameState gameState=EGameState.Idle;
	
	//public int playerResource=100;
	public int playerLife=10;
	
	public float sellTowerRefundRatio=0.5f;

	[HideInInspector] 
    public LayerManager layerManager;

	private int totalWaveCount;
	private int currentWave=0;
	
	public Transform rangeIndicatorH;
	public Transform rangeIndicatorF;
	
	static public GameControl gameControl;
	
	public float buildingBarWidthModifier=1f;
	public float buildingBarHeightModifier=1f;
	public Vector3 buildingBarPosOffset=new Vector3(0, -0.5f, 0);

    public GamePath defaultPath;

	public Platform platform;

	public bool gameStart = false;


	public static FightCenter fightCenter;
	void Awake()
    {
        gameControl = this;
       
        ConfigManager.Instance.Init();
		GameMessage.Init();
        BagController.Instance.OnInit();

        AudioManager audioManager=(AudioManager)FindObjectOfType(typeof(AudioManager));
		if(audioManager==null){
			AudioManager.Init();
		}

		gameState=EGameState.Idle;
		
		rangeIndicatorH=(Transform)Instantiate(rangeIndicatorH);
		rangeIndicatorH.parent=transform;
		rangeIndicatorF=(Transform)Instantiate(rangeIndicatorF);
		rangeIndicatorF.parent=transform;
		ClearIndicator();
		
		OverlayManager.SetModifier(buildingBarWidthModifier, buildingBarHeightModifier);
		OverlayManager.SetOffset(buildingBarPosOffset);

        SpawnManager.Instance.OnInit(this.defaultPath);

	}

    // Use this for initialization
    void Start ()
    {
       // defaultPath.InitPath();

        totalWaveCount =SpawnManager.Instance.waves.Length;

		Camera mainCam=Camera.main;
		Transform mainCamT=mainCam.transform;
		
		GameObject overlayCamObj=new GameObject();
		overlayCamObj.name="OverlayCamera";
		
		LayerMask layer=1<<LayerManager.LayerOverlay();
		mainCam.cullingMask=mainCam.cullingMask&~layer;
		
		Camera overlayCam=overlayCamObj.AddComponent<Camera>();
		
		overlayCam.clearFlags=CameraClearFlags.Depth;
		overlayCam.depth=mainCam.depth + 1;
		overlayCam.cullingMask=layer;
		overlayCam.fieldOfView=mainCam.fieldOfView;
		
		overlayCamObj.transform.parent=mainCamT;
		overlayCamObj.transform.rotation=mainCamT.rotation;
		overlayCamObj.transform.localPosition=Vector3.zero;
		
		Time.timeScale=1;

		//-------------------------------------------------------------------
		platform.GenerateNode(0);
        platform.SetWalkable(true);

       
        FightData fightdata = new FightData();

        fightdata.enemyBattleData = new FightPlayerData();
		var userData = new FightPlayerInfo();
		userData.userID = 1001;
		userData.nickName = "欢喜冤家";
		userData.userIcon = "Icon_1";
		fightdata.enemyBattleData.userData = userData;
		fightdata.enemyBattleData.heroData = new FightRoleData[] { };
		fightdata.enemyBattleData.teamSkills = new FightSkillData[] { };


		List<SceneEntity> lstScentEntities = StaticData.LoadList<SceneEntity>("FightScene");
		List<FightRoleData> fightRole = new List<FightRoleData>();
		for (int i = 0; i < lstScentEntities.Count; i++)
		{
			NpcData npcData = new NpcData();
			npcData.InitData((uint)lstScentEntities[i].npcId, (ETeamType)lstScentEntities[i].teamId);
			FightRoleData roleData = BuildManager.CreateNpc(npcData);
			roleData.CurHp = lstScentEntities[i].curHp;
			roleData.CurMp = lstScentEntities[i].curMp;
			roleData.NodeId = lstScentEntities[i].npcPos;
			roleData.CostNodes = lstScentEntities[i].nodeCost;
			fightRole.Add(roleData);
		}

		gameStart = true;


		fightdata.selfBattleData = new FightPlayerData();
        userData = new FightPlayerInfo();
        userData.userID = 1002;
        userData.nickName = "欢喜冤家_2";
        userData.userIcon = "Icon_2";
        fightdata.selfBattleData.userData = userData;
        fightdata.selfBattleData.heroData = fightRole.ToArray();
        fightdata.selfBattleData.teamSkills = new FightSkillData[] { };

        fightdata.battleFieldData = new BattleFieldData();
        fightdata.battleFieldData.nodeGraph = platform.GetNodeGraph();
		fightdata.battleFieldData.row = platform.row;
		fightdata.battleFieldData.column = platform.column;


        fightCenter = new FightCenter();
		fightCenter.OnInit(fightdata, platform);

    }
	

    public void RefreshResource()
    {
        if(ResourceE != null)
           ResourceE();
    }


	
	//void TowerDestroy(Tower tower){
	//	if(selectedTower==tower || selectedTower==null || !selectedTower.ModelObj.activeSelf){
	//		ClearSelection();
	//	}
	//}
	
	//IEnumerator _TowerDestroy(UnitTower tower)
	
	public static int GetPlayerLife(){
		return gameControl.playerLife;
	}
	
	// Update is called once per frame
    float deltalTime = 0;
	void Update () {

        this.deltalTime = Time.deltaTime;
        SpawnManager.Instance.OnUpdate(this.deltalTime);
		ObjectPoolManager.Instance.OnUpdate();

		fightCenter.Update();
	}
	
	void WaveStartSpawned(int waveID){
		currentWave+=1;
		
		//if game is not yet started, start it now
		if(gameState==EGameState.Idle) gameState=EGameState.Started;
	}
	
	
	static public NpcData selectedTower;
	
	static public NpcData Select(Vector3 pointer){
		//change this
		int layer=LayerManager.LayerTower();
		
		//LayerMask mask=1<<layer;
		//Ray ray = Camera.main.ScreenPointToRay(pointer);
		//RaycastHit hit;
		//if(!Physics.Raycast(ray, out hit, Mathf.Infinity, mask)){
		//	return null;
		//}
  //      CharacterBase entity = EntitesManager.Instance.GetTower(hit.transform.gameObject);

  //      if (entity != null && entity.GetEntityType() == EEntityType.Tower)
  //      {
  //          selectedTower = entity as Tower;
  //          selectedTower.Select();
  //          gameControl._ShowIndicator(selectedTower);
  //          return selectedTower;
  //      }
		return null;
	}
	
	//public static void ShowIndicator(Tower tower){
	//	gameControl._ShowIndicator(tower);
	//}
	
	//public void _ShowIndicator(Tower tower){
 //       if (tower == null || tower.CurData == null || tower.CurData.skillData == null)
 //       { 
 //           DebugMgr.LogError("数据异常");
 //           return;
 //       }

 //       //show range indicator on the tower
 //       //for support tower, show friendly range indicator
 //       if (tower.type == _TowerType.SupportTower)
 //       {
 //           //Debug.Log(tower.type);
 //           float range = tower.CurData.attrConfig.Range;
 //           if (rangeIndicatorF != null)
 //           {
 //               rangeIndicatorF.position = tower.Trans.position + Vector3.up*0.5f;
 //               rangeIndicatorF.localScale = new Vector3(range / 10, 1, range / 10);
 //               rangeIndicatorF.GetComponent<Renderer>().enabled = true;
 //           }
 //           if (rangeIndicatorH != null) rangeIndicatorH.GetComponent<Renderer>().enabled = false;
 //       }
 //       //for support tower, show hostile range indicator
 //       else if (tower.type != _TowerType.ResourceTower)
 //       {
 //           float range = tower.CurData.baseStat.range;
 //           if (rangeIndicatorH != null)
 //           {
 //               rangeIndicatorH.position = tower.Trans.position + Vector3.up;
 //               rangeIndicatorH.localScale = new Vector3(range / 10, 1, range / 10);
 //               rangeIndicatorH.GetComponent<Renderer>().enabled = true;
 //           }
 //           if (rangeIndicatorF != null) rangeIndicatorF.GetComponent<Renderer>().enabled = false;
 //       }
 //       else if (tower.type != _TowerType.AOETower)
 //       {
 //           float range = tower.CurData.baseStat.range;
 //           if(rangeIndicatorH != null)
 //           {
 //               rangeIndicatorH.position = tower.Trans.position + Vector3.up;
 //               rangeIndicatorH.localScale = new Vector3( range / 10, 1, range / 10);
 //               rangeIndicatorH.GetComponent<Renderer>().enabled = true;
 //           }
 //           if (rangeIndicatorF != null) rangeIndicatorF.GetComponent<Renderer>().enabled = false;
 //       }
	//}
	
	//public static void DragNDropIndicator(Tower tower)
 //   {
	//	if(tower.type!=_TowerType.ResourceTower)
 //       {
 //           if (gameControl == null) Debug.LogError("gameControl == null");
 //           if (tower == null) Debug.LogError("tower == null");
 //           gameControl._ShowIndicator(tower);
	//		gameControl.StartCoroutine(gameControl._DragNDropIndicator(tower));
	//	}
	//}
	//IEnumerator _DragNDropIndicator(Tower tower){
 //       while (tower.ModelObj != null && tower.TowerID == 0)
 //       {
	//		if(tower.type==_TowerType.SupportTower){
 //               if (rangeIndicatorF != null) rangeIndicatorF.position = tower.Trans.position + Vector3.up;
	//		}
	//		else{
 //               if (rangeIndicatorH != null) rangeIndicatorH.position = tower.Trans.position + Vector3.up;
	//		}
				
	//		yield return null;
	//	}
	//	ClearIndicator();
	//}
	
	public static void ClearIndicator(){
		gameControl._ClearIndicator();
	}
	
	public void _ClearIndicator(){
		if(rangeIndicatorH!=null) rangeIndicatorH.GetComponent<Renderer>().enabled=false;
		if(rangeIndicatorF!=null) rangeIndicatorF.GetComponent<Renderer>().enabled=false;
	}
	
	static public void ClearSelection(){
		selectedTower=null;
		gameControl._ClearIndicator();
	}
	

	static public void GainResource(ResItem item)
    {
        BagController.Instance.AddResource(item.itemId, item.num);
    }
	
	static public void SpendResource(ResItem item)
    {
        BagController.Instance.RemoveResource(item.itemId,item.num);
    }
	
	
	static public int GetResourceVal(int itemBaseId)
    {
        return BagController.Instance.GetResource(itemBaseId).totalNum;
    }

	static public bool HaveSufficientResource(ResItem resItem)
    {
        return BagController.Instance.HaveSufficientResource(resItem.itemId,resItem.num);
    }
	
	
	static public float GetSellTowerRefundRatio(){
		return gameControl.sellTowerRefundRatio;
	}
	
	static public void GradualPause(){
		
	}
	
	static public void GradualResume(){
		
	}
}
