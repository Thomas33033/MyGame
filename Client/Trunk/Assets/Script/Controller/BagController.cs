//================================================
// auth：xuetao
// date：2018/5/30 12:20:55
//================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class BagItem
{
    public int uid;
    public int baseId;
    public int totalNum;
    public static int index = 0;
    public CfgItemData config;
    public BagItem(int p_baseId, int p_totalNum)
    {
        this.uid = index++;
        this.totalNum = p_totalNum;
        this.baseId = p_baseId;
        this.config = ConfigManager.Instance.GetData<CfgItemData>(p_baseId);
    }
}

public class BagController : Singleton<BagController>
{
    public Dictionary<int, BagItem> mainBagMap = new Dictionary<int, BagItem>();
    public List<BagItem> mainBagList = new List<BagItem>();

    public void OnInit()
    {
        AddResource((int)EItemType.Gold, 500);
        AddResource((int)EItemType.Silver, 500);
    }


    public bool HaveSufficientResource(int baseId, int num)
    {
        if (mainBagMap.ContainsKey(baseId))
        {
            return mainBagMap[baseId].totalNum >= num ? true : false;

        }
        else
        {
            return false;
        }
    }

    public BagItem GetResource(int baseId)
    {
        if (mainBagMap.ContainsKey(baseId))
        {
            return mainBagMap[baseId];
        }
        else
        {
            return new BagItem(baseId,0);
        }
    }

    //增加资源
    public void AddResource(int baseId, int num)
    {
        if (mainBagMap.ContainsKey(baseId))
        {
            mainBagMap[baseId].totalNum += num;
        }
        else
        {
            BagItem bagItem = new BagItem(baseId, num);
            mainBagMap.Add(baseId, bagItem);
            mainBagList.Add(bagItem);
        }
        GameControl.gameControl.RefreshResource();
        DebugMgr.LogError("AddResource:"+baseId + " " + num);
    }


    public bool RemoveResource(int baseId, int num)
    {
        DebugMgr.LogError("RemoveResource:" + baseId + " " + num);
        if (mainBagMap.ContainsKey(baseId))
        {

            int curNum = mainBagMap[baseId].totalNum;
            if (curNum >= num)
            {
                mainBagMap[baseId].totalNum -= num;
                //删除道具数量为0的格子
                if (curNum == num)
                {
                    mainBagList.Remove(mainBagMap[baseId]);
                    mainBagMap.Remove(baseId);
                }
                GameControl.gameControl.RefreshResource();
                return true;
            }
            else
            {
                UnityEngine.Debug.LogError("删除失败: baseId:" + baseId + " 道具数量不够！");
            }
        }
        else
        {
            UnityEngine.Debug.LogError("删除失败: 背包中不存在 baseId:" + baseId );
        }
        return false;
    }
}

