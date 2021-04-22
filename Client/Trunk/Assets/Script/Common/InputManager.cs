using Fight;
using FightCommom;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;
    private GameObject dragTower;
    private ModelPoolObj poolObj;
    private ResItem costRes;
    private FightRoleData fightBuildData;
    List<int> costNodeIDs;

    private bool bStartTouch = false;

    void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        SelectObj();
    }

    public void DragNDropTower(ModelPoolObj poolObj, ResItem costRes, FightRoleData fightBuildData)
    {
        this.costRes = costRes;
        this.poolObj = poolObj;
        dragTower = poolObj.itemObj;

        this.fightBuildData = fightBuildData;
        
        StartCoroutine(this.DragNDropRoutine());
    }

    public IEnumerator DragNDropRoutine()
    {
        CameraController.Instance.dragEnable = false;

        Collider collider = this.dragTower.GetOrAddComponent<Collider>();

        if (collider != null)
        {
            collider.enabled = false;
        }
       
        bool buildEnable = false;

        Vector3 lastPos = Vector3.zero;

        this.costNodeIDs = new List<int>();

        bool bStartTouch = false;
        bool bStartCreate = false;
        while (true)
        {
            //检测建筑是否可以创建
            bool flag = BuildManager.CheckBuildPoint(Input.mousePosition, costNodeIDs, this.fightBuildData.NodeSize);

            BuildableInfo currentBuildInfo = BuildManager.GetBuildInfo();

            if (currentBuildInfo != null)
            {
                if (currentBuildInfo.position != lastPos)
                {
                    lastPos = currentBuildInfo.position;

                    if (flag && !buildEnable)
                    {
                        buildEnable = true;
                        //可以创建建筑后，将建筑颜色设为green
                        UnitUtility.SetAdditiveMatColorRecursively(this.dragTower.transform, Color.green);
                    }
                    else if (!flag && buildEnable)
                    {
                        buildEnable = false;
                        //如果不能创建建筑，将建筑颜色设为红色
                        UnitUtility.SetAdditiveMatColorRecursively(this.dragTower.transform, Color.red);
                    }
                }
            }

            if (currentBuildInfo == null)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                {
                    this.dragTower.transform.position = hit.point;
                }
                else this.dragTower.transform.position = ray.GetPoint(30);
            }
            else
            {
                this.dragTower.transform.position = currentBuildInfo.position;
               
            }

            //点击
            if (Input.GetMouseButtonDown(0))
            {
                bStartTouch = true;
                
                if (fightBuildData.npcType != (int)RoleType.Buildings)
                {
                    if (flag)
                        DragNDropBuilt(true);
                    else
                        DragNDropCancel();
                    break;
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                bStartTouch = false;
            }

            if (bStartTouch == true)
            {
                if (fightBuildData.NodeSize == (int)RoleType.Buildings)
                {
                    bStartCreate = true;
                    DragNDropBuilt(false);
                }
            }
            else
            {
                if (bStartCreate == true)
                {
                    DragNDropCancel();
                    break;
                }
            }

            //right-click, cancel
            if (Input.GetMouseButtonDown(1))
            {
                DragNDropCancel();
                break;
            }

            yield return null;
        }

        CameraController.Instance.dragEnable = true;
    }

    void DragNDropBuilt(bool isDragCreat)
    {
        if (GameControl.HaveSufficientResource(costRes))
        {
            //请求消耗资源
            BagController.Instance.RemoveResource(costRes.itemId, costRes.num);

            UnitUtility.SetMat2DiffuseRecursively(this.dragTower.transform);

            this.dragTower.GetOrAddComponent<BoxCollider>().enabled = true;

            this.fightBuildData.CostNodes = costNodeIDs.ToArray();

            Node node = BuildManager.GetBuildPosition();
            this.fightBuildData.NodeId = node.Id;

         //   if(!isDragCreat)
            {
                this.poolObj.ReturnToPool();
            }
            

            if (GameControl.fightCenter == null)
            {
                Debug.LogError(GameControl.fightCenter);
            }

            GameControl.fightCenter.CreateRole(this.fightBuildData);
            
        }
        else
        {
            var config = ConfigManager.Instance.GetData<CfgItemData>((int)costRes.itemId);
            GameMessage.DisplayMessage(config.ResName + "不足");
            DragNDropCancel();
        }
    }

    void DragNDropCancel()
    {
        this.poolObj.ReturnToPool();
        BuildManager.ClearBuildPoint();
    }


    //选中对象
    private void SelectObj()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            LayerMask mask = 1 << LayerManager.layerBuilding;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask))
            {
                Debug.LogError(hit.transform.gameObject);
            }
        }
    }
}
