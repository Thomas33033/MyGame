﻿using Fight;
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
    private FightHeroData fightBuildData;
    List<int> costNodeIDs;

    void Awake()
    {
        Instance = this;
    }

    public void DragNDropTower(ModelPoolObj poolObj, ResItem costRes, FightHeroData fightBuildData)
    {
        this.costRes = costRes;
        this.poolObj = poolObj;
        dragTower = poolObj.itemObj;

        this.fightBuildData = fightBuildData;
        StartCoroutine(this.DragNDropRoutine());
    }

    public IEnumerator DragNDropRoutine()
    {
        this.dragTower.GetComponent<Collider>().enabled = false;

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
        if (GameControl.HaveSufficientResource(costRes))
        {
            //请求消耗资源
            BagController.Instance.RemoveResource(costRes.itemId, costRes.num);

            UnitUtility.SetMat2DiffuseRecursively(this.dragTower.transform);

            this.dragTower.transform.GetComponent<Collider>().enabled = true;

            this.fightBuildData.CostNodes = costNodeIDs.ToArray();
            Node node = BuildManager.GetBuildPosition();
            this.fightBuildData.NodeId = node.ID;
            this.poolObj.ReturnToPool();

            FightScene.Instance.CreateRole(this.fightBuildData, 1);
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
}
