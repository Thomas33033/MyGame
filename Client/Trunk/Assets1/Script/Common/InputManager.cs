using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;
    private Tower dragTower;
    void Awake()
    {
        Instance = this;
    }

    public void DragNDropTower(Tower tower)
    {
        dragTower = tower;
        StartCoroutine(this.DragNDropRoutine());
    }

    public IEnumerator DragNDropRoutine()
    {
        //UnitUtility.SetMat2AdditiveRecursively(this.dragTower.Trans);
        //UnitUtility.SetAdditiveMatColorRecursively(this.dragTower.Trans, Color.grey);

        //this.dragTower.Trans.GetComponent<Collider>().enabled = false;

        GameControl.DragNDropIndicator(this.dragTower);

        bool buildEnable = false;

        Vector3 lastPos = Vector3.zero;

        while (true)
        {
            //检测建筑是否可以创建
            bool flag = BuildManager.CheckBuildPoint(Input.mousePosition, dragTower.CurData.TowerType, dragTower.specialID);
            
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
                        UnitUtility.SetAdditiveMatColorRecursively(this.dragTower.Trans, Color.green);
                    }
                    else if (!flag && buildEnable)
                    {
                        buildEnable = false;
                        //如果不能创建建筑，将建筑颜色设为红色
                        UnitUtility.SetAdditiveMatColorRecursively(this.dragTower.Trans, Color.red);
                    }
                }
            }

            if (currentBuildInfo == null)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, Mathf.Infinity)) this.dragTower.ClientPos = hit.point;

                else this.dragTower.ClientPos = ray.GetPoint(30);
            }
            else
            {
                this.dragTower.ClientPos = currentBuildInfo.position;
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (flag)
                    DragNDropBuilt();
                else
                    DragNDropCancel();
                break;
            }

            //right-click, cancel
            if (Input.GetMouseButtonDown(1))
            {
                DragNDropCancel();
                break;
            }

            yield return null;
        }
    }


    void DragNDropBuilt()
    {
        //检查资源
        ResItem resItem = this.dragTower.CurData.costRes;
        if (GameControl.HaveSufficientResource(resItem))
        {
            //请求消耗资源
            BagController.Instance.RemoveResource(resItem.itemId, resItem.num);

            UnitUtility.SetMat2DiffuseRecursively(this.dragTower.Trans);

            
            this.dragTower.Trans.GetComponent<Collider>().enabled = true;

            BuildManager.DragNDropBuilt(this.dragTower);

            this.dragTower.InitTower(0);
        }
        else
        {
            GameMessage.DisplayMessage("Insufficient Resource");
            DragNDropCancel();
        }
    }

    void DragNDropCancel()
    {
        BuildManager.ClearBuildPoint();
        //EntitesManager.Instance.RemoveTower(this.dragTower.CurData.uid);
    }
}
