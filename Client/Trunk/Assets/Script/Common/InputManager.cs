﻿using Fight;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;
    private GameObject dragTower;
    private ResItem _costRes;
    private FightHeroData fightBuildData;
    List<int> costNodeIDs;

    void Awake()
    {
        Instance = this;
    }

    public void DragNDropTower(GameObject tower, ResItem costRes, FightHeroData fightBuildData)
    {
        _costRes = costRes;
        dragTower = tower;
        this.fightBuildData = fightBuildData;
        StartCoroutine(this.DragNDropRoutine());
    }

    public IEnumerator DragNDropRoutine()
    {
        //UnitUtility.SetMat2AdditiveRecursively(this.dragTower.Trans);
        //UnitUtility.SetAdditiveMatColorRecursively(this.dragTower.Trans, Color.grey);

        this.dragTower.GetComponent<Collider>().enabled = false;

        //GameControl.DragNDropIndicator(this.dragTower);

        bool buildEnable = false;

        Vector3 lastPos = Vector3.zero;

        this.costNodeIDs = new List<int>();

        while (true)
        {
            //检测建筑是否可以创建
            bool flag = BuildManager.CheckBuildPoint(Input.mousePosition, costNodeIDs, _TowerType.AOETower, 0);
            
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
                Debug.LogError("currentBuildInfo.position" + currentBuildInfo.position);
                this.dragTower.transform.position = currentBuildInfo.position;
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
        Debug.Log("DragNDropBuilt");
        //检查资源
        ResItem resItem = _costRes;
        if (GameControl.HaveSufficientResource(resItem))
        {
            Debug.Log("DragNDropBuilt ok");
            //请求消耗资源
            BagController.Instance.RemoveResource(resItem.itemId, resItem.num);

            UnitUtility.SetMat2DiffuseRecursively(this.dragTower.transform);

            this.dragTower.transform.GetComponent<Collider>().enabled = true;

            this.fightBuildData.CostNodes =  string.Join("-",costNodeIDs);
            this.fightBuildData.Position = BuildManager.GetBuildPosition();

            //通知场景创建建筑
            //GameObject.Destroy(this.dragTower);
            //BuildManager.ClearBuildPoint();

            FightScene.Instance.CreateRole(this.fightBuildData);

            //BuildManager.DragNDropBuilt(this.fightBuildData, costNodeIDs);
            //this.dragTower.InitTower(0);

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
        GameObject.Destroy(this.dragTower);
    }
}
