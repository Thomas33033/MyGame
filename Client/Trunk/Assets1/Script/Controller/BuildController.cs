using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildController : MonoBehaviour {

    public Tower[] towers;
    static private float _gridSize = 0;
    public float gridSize = 2;
    public Transform[] platforms;
    private Platform[] buildPlatforms;

    public bool AutoAdjstTextureToGrid = true;

    static public BuildController buildManager;

    static private BuildableInfo currentBuildInfo;

    static private uint towerCount = 0;

    private static GameObject indicator;
    private static GameObject indicator2;

    public static uint PrePlaceTower()
    {
        return towerCount += 1;
    }
    public static uint GetTowerCount()
    {
        return towerCount;
    }

    void Awake()
    {
        buildManager = this;
        towerCount = 0;
        gridSize = Mathf.Clamp(gridSize, 0.5f, 3.0f);
        _gridSize = gridSize;
        InitPlatform();
    }


    // Use this for initialization
    void InitPlatform()
    {


        buildPlatforms = new Platform[platforms.Length];
        int i = 0;
        foreach (Transform basePlane in platforms)
        {
            Platform platform = basePlane.gameObject.GetComponent<Platform>();

            if (platform == null)
            {
                platform = basePlane.gameObject.AddComponent<Platform>();
                platform.buildableType = new _TowerType[6];
                platform.buildableType[0] = _TowerType.TurretTower;
                platform.buildableType[1] = _TowerType.AOETower;
                platform.buildableType[2] = _TowerType.DirectionalAOETower;
                platform.buildableType[3] = _TowerType.SupportTower;
                platform.buildableType[4] = _TowerType.ResourceTower;
                platform.buildableType[5] = _TowerType.Mine;
            }

            buildPlatforms[i] = platform;

            //make sure the plane is perfectly horizontal, rotation around the y-axis is preserved
            basePlane.eulerAngles = new Vector3(0, basePlane.rotation.eulerAngles.y, 0);

            //adjusting the scale
            float scaleX = Mathf.Floor(basePlane.localScale.x * 10 / gridSize) * gridSize * 0.1f;
            float scaleZ = Mathf.Floor(basePlane.localScale.z * 10 / gridSize) * gridSize * 0.1f;

            if (scaleX == 0) scaleX = gridSize * 0.1f;
            if (scaleZ == 0) scaleZ = gridSize * 0.1f;

            basePlane.localScale = new Vector3(scaleX, 1, scaleZ);

            //adjusting the texture
            if (AutoAdjstTextureToGrid)
            {
                Material mat = basePlane.GetComponent<Renderer>().material;

                float x = (basePlane.localScale.x * 10f) / gridSize;
                float z = (basePlane.localScale.z * 10f) / gridSize;

                mat.mainTextureOffset = new Vector2(0.5f, 0.5f);
                mat.mainTextureScale = new Vector2(x, z);
            }
            i++;
        }

    }

    void Start()
    {

    }

    static public void ClearBuildPoint()
    {
        currentBuildInfo = null;
        ClearIndicator();
    }

    static public void ClearIndicator()
    {
       // if (indicator != null) indicator.active = false;
    }

    //called to set indicator to a particular node, set the color as well
    //not iOS performance friendly
    static public void SetIndicator(Vector3 pointer)
    {

        LayerMask mask = 1 << LayerManager.LayerPlatform();
        Ray ray = CameraController.Instance.mainCamera.ScreenPointToRay(pointer);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask))
        {
            for (int i = 0; i < buildManager.buildPlatforms.Length; i++)
            {
                Transform basePlane = buildManager.buildPlatforms[i].thisT;
                if (hit.transform == basePlane)
                {
                    //calculating the build center point base on the input position
                    //check if the row count is odd or even number
                    float remainderX = basePlane.localScale.x * 10 / _gridSize % 2;
                    float remainderZ = basePlane.localScale.z * 10 / _gridSize % 2;

                    //get the rotation offset of the plane
                    Quaternion rot = Quaternion.LookRotation(hit.point - basePlane.position);

                    //get the x and z distance from the centre of the plane in the baseplane orientation
                    //from this point on all x and z will be in reference to the basePlane orientation
                    float dist = Vector3.Distance(hit.point, basePlane.position);
                    float distX = Mathf.Sin((rot.eulerAngles.y - basePlane.rotation.eulerAngles.y) * Mathf.Deg2Rad) * dist;
                    float distZ = Mathf.Cos((rot.eulerAngles.y - basePlane.rotation.eulerAngles.y) * Mathf.Deg2Rad) * dist;

                    //get the sign (1/-1) of the x and y direction
                    float signX = distX / Mathf.Abs(distX);
                    float signZ = distZ / Mathf.Abs(distZ);

                    //calculate the tile number selected in z and z direction
                    float numX = Mathf.Round((distX + (remainderX - 1) * (signX * _gridSize / 2)) / _gridSize);
                    float numZ = Mathf.Round((distZ + (remainderZ - 1) * (signZ * _gridSize / 2)) / _gridSize);

                    //calculate offset in x-axis, 
                    float offsetX = -(remainderX - 1) * signX * _gridSize / 2;
                    float offsetZ = -(remainderZ - 1) * signZ * _gridSize / 2;

                    //get the pos and apply the offset
                    Vector3 p = basePlane.TransformDirection(new Vector3(numX, 0, numZ) * _gridSize);
                    p += basePlane.TransformDirection(new Vector3(offsetX, 0, offsetZ));

                    //set the position;
                    Vector3 pos = p + basePlane.position;
                    //indicator2.SetActive(true);

                    //indicator2.transform.position = pos;
                    //indicator2.transform.rotation = basePlane.rotation;

                    Collider[] cols = Physics.OverlapSphere(pos, _gridSize / 2 * 0.9f, ~mask);
                    if (cols.Length > 0)
                    {
                        indicator2.GetComponent<Renderer>().material.SetColor("_TintColor", Color.red);
                    }
                    else
                    {
                        indicator2.GetComponent<Renderer>().material.SetColor("_TintColor", Color.green);
                    }
                }
            }
        }
        //else indicator2.SetActive(false);
    }

    static public bool CheckBuildPoint(Vector3 pointer)
    {
        BuildableInfo buildableInfo = new BuildableInfo();

        LayerMask mask = 1 << LayerManager.LayerPlatform();
        Ray ray = CameraController.Instance.mainCamera.ScreenPointToRay(pointer);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask))
        {
            for (int i = 0; i < buildManager.buildPlatforms.Length; i++)
            {
                Transform basePlane = buildManager.buildPlatforms[i].thisT;
                if (hit.transform == basePlane)
                {
                    //calculating the build center point base on the input position
                    //check if the row count is odd or even number
                    float remainderX = basePlane.localScale.x * 10 / _gridSize % 2;
                    float remainderZ = basePlane.localScale.z * 10 / _gridSize % 2;

                    //get the rotation offset of the plane
                    Quaternion rot = Quaternion.LookRotation(hit.point - basePlane.position);

                    //get the x and z distance from the centre of the plane in the baseplane orientation
                    //from this point on all x and z will be in reference to the basePlane orientation
                    float dist = Vector3.Distance(hit.point, basePlane.position);
                    float distX = Mathf.Sin((rot.eulerAngles.y - basePlane.rotation.eulerAngles.y) * Mathf.Deg2Rad) * dist;
                    float distZ = Mathf.Cos((rot.eulerAngles.y - basePlane.rotation.eulerAngles.y) * Mathf.Deg2Rad) * dist;

                    //get the sign (1/-1) of the x and y direction
                    float signX = distX / Mathf.Abs(distX);
                    float signZ = distZ / Mathf.Abs(distZ);

                    //calculate the tile number selected in z and z direction
                    float numX = Mathf.Round((distX + (remainderX - 1) * (signX * _gridSize / 2)) / _gridSize);
                    float numZ = Mathf.Round((distZ + (remainderZ - 1) * (signZ * _gridSize / 2)) / _gridSize);

                    //calculate offset in x-axis, 
                    float offsetX = -(remainderX - 1) * signX * _gridSize / 2;
                    float offsetZ = -(remainderZ - 1) * signZ * _gridSize / 2;

                    //get the pos and apply the offset
                    Vector3 p = basePlane.TransformDirection(new Vector3(numX, 0, numZ) * _gridSize);
                    p += basePlane.TransformDirection(new Vector3(offsetX, 0, offsetZ));

                    //set the position;
                    Vector3 pos = p + basePlane.position;

                    //check if the position is blocked, by any other obstabcle other than the baseplane itself
                    Collider[] cols = Physics.OverlapSphere(pos, _gridSize / 2 * 0.9f, ~mask);
                    if (cols.Length > 0)
                    {
                        //Debug.Log("something's in the way "+cols[0]);
                        return false;
                    }
                    else
                    {
                        //confirm that we can build here
                        buildableInfo.buildable = true;
                        buildableInfo.position = pos;
                        buildableInfo.platform = buildManager.buildPlatforms[i];
                    }

                    //check if the platform is walkable, if so, check if building on the point wont block all possible path
                    if (buildManager.buildPlatforms[i].IsWalkable())
                    {
                        //return true is the platform is not block
                        if (buildManager.buildPlatforms[i].CheckForBlock(pos))
                        {
                            return false;
                        }
                    }

                    buildableInfo.buildableType = buildManager.buildPlatforms[i].buildableType;
                    buildableInfo.specialBuildableID = buildManager.buildPlatforms[i].specialBuildableID;

                    break;
                }
            }
        }
        else return false;

        currentBuildInfo = buildableInfo;

        //indicator.SetActive(true);
        //indicator.transform.position = currentBuildInfo.position;
        //indicator.transform.rotation = currentBuildInfo.platform.thisT.rotation;

        return true;
    }

    public static bool CheckBuildPoint(Vector3 pointer, _TowerType type, int specialID)
    {
        if (!CheckBuildPoint(pointer)) return false;

        if (specialID > 0)
        {
            if (currentBuildInfo.specialBuildableID != null && currentBuildInfo.specialBuildableID.Length > 0)
            {
                foreach (int specialBuildableID in currentBuildInfo.specialBuildableID)
                {
                    if (specialBuildableID == specialID)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        else
        {
            if (currentBuildInfo.specialBuildableID != null && currentBuildInfo.specialBuildableID.Length > 0)
            {
                return false;
            }

            foreach (_TowerType buildabletype in currentBuildInfo.buildableType)
            {
                if (type == buildabletype)
                {
                    return true;
                }
            }
        }

        currentBuildInfo.buildable = false;
        return false;
    }

    public static void DragNDropBuilt(Tower tower)
    {
        tower.TowerID = towerCount += 1;
        if (currentBuildInfo.platform != null)
        {
            if (tower.type != _TowerType.Mine)
                currentBuildInfo.platform.Build(currentBuildInfo.position, tower);
        }
        else Debug.Log("null");

        ClearBuildPoint();
    }

    //called by any external component to build tower, uses currentBuildInfo, return false if there isnt one
    public static bool BuildTowerPointNBuild(s_TowerData tower)
    {
        if (currentBuildInfo == null) return false;

        return BuildTowerPointNBuild(tower, currentBuildInfo.position, currentBuildInfo.platform);
    }

    //called by any external component to build tower
    public static bool BuildTowerPointNBuild(s_TowerData sTowerData, Vector3 pos, Platform platform)
    {

        //dont allow building of resource tower before game started
        if (sTowerData.TowerType == _TowerType.ResourceTower && GameControl.gameState == EGameState.Idle)
        {
            GameMessage.DisplayMessage("Cant Build Tower before spawn start");
            return false;
        }

        //check if there are sufficient resource
        if (GameControl.HaveSufficientResource(sTowerData.costRes))
        {
            GameControl.SpendResource(sTowerData.costRes);

            sTowerData.position = pos;
            Tower towerCom = new Tower();
            towerCom.OnInit(sTowerData);

            //register the tower to the platform
            if (platform != null) platform.Build(pos, towerCom);

            //clear the build info and indicator for build manager
            ClearBuildPoint();

            return true;
        }

        GameMessage.DisplayMessage("Insufficient Resource");
        return false;
    }


    private Tower[] sampleTower;
    private int currentSampleID = -1;
    public static void InitiateSampleTower()
    {
        buildManager.sampleTower = new Tower[buildManager.towers.Length];
        for (int i = 0; i < buildManager.towers.Length; i++)
        {
            Tower tower = buildManager.towers[i];
            GameObject towerObj = (GameObject)Instantiate(tower.ModelObj);
            buildManager.sampleTower[i] = new Tower();
            //buildManager.sampleTower[i].OnInit(tower.charactorData);
            towerObj.SetActive(false);
            UnitUtility.SetAdditiveMatColorRecursively(towerObj.transform, Color.green);
        }
    }

    static public void ShowSampleTower(int ID)
    {
        buildManager._ShowSampleTower(ID);
    }
    public void _ShowSampleTower(int ID)
    {
        if (currentSampleID == ID || currentBuildInfo == null) return;

        if (currentSampleID > 0)
        {
            ClearSampleTower();
        }
        currentSampleID = ID;
        sampleTower[ID].Trans.position = currentBuildInfo.position;
        sampleTower[ID].ModelObj.SetActive(true);
        GameControl.ShowIndicator(sampleTower[ID]);
    }

    static public void ClearSampleTower()
    {
        buildManager._ClearSampleTower();
    }
    public void _ClearSampleTower()
    {
        if (currentSampleID < 0) return;

        sampleTower[currentSampleID].ModelObj.SetActive(false);
        GameControl.ClearIndicator();
        currentSampleID = -1;
    }


    static public BuildableInfo GetBuildInfo()
    {
        return currentBuildInfo;
    }

    static public Tower[] GetTowerList()
    {
        return buildManager.towers;
    }

    static public float GetGridSize()
    {
        return _gridSize;
    }

    Vector3 poss;
    //public bool debugSelectPos=true;
    void OnDrawGizmos()
    {

        //if(debugSelectPos) Gizmos.DrawCube(SelectBuildPos(Input.mousePosition), new Vector3(gridSize, 0, gridSize));

    }
}

[System.Serializable]
public class BuildableInfo
{
    public bool buildable = false;
    public Vector3 position = Vector3.zero;
    public Platform platform;
    public _TowerType[] buildableType = null;

    public int[] specialBuildableID;

    //cant build
    public void BuildSpotInto() { }

    //can build anything
    public void BuildSpotInto(Vector3 pos)
    {
        position = pos;
    }

    //can build with restriction to certain tower type
    public void BuildSpotInto(Vector3 pos, _TowerType[] bT)
    {
        position = pos;
        buildableType = bT;
    }
}