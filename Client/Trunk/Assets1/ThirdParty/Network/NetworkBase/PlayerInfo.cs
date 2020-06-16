using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using Net;

public class PlayerInfo : StructData
{
    public string strName = ""; // 角色名字
    public Byte bySex = 0; // 性别 0未定义 1男 2女
    public UInt16 wLevel = 0; // // 当前等级
    public UInt64 qwExp = 0; // // 当前经验
    public UInt32 dwFairyStone = 0; // 灵石
    public UInt32 dwGold = 0; // 金子
    public UInt32 dwCoPatch = 0; // 万能碎片
    public UInt32 dwLeaderShip = 0; // 领导力
    public UInt32 dwPhyPower = 0; // // 体力值
    public UInt32 dwMaxPhyPower = 0; // 体力值上限
    public UInt32 dwMaxFriendNum = 0; // 好友上限
    public UInt32 dwFriendPoint = 0; // 友情点数
    public UInt32 dwHeroBagCount = 0;  // 英雄背包数量
    public UInt32 airLimit = 0;  // 灵气上限
    public UInt32 curAir = 0;   // 当前灵气
    public UInt16 wdMorale;     //士气
    public UInt32 vip;       // VIP等级
    public UInt32 vip_gem;   // VIP充值金额
    public UInt32 battle_coin;  //斗币
    public UInt32 dwBuyHeroBagNum;  //已购买的背包格子
    public UInt32 dwEquipBagNum;    //装备背包数量
    public UInt32 dwBuyEquipBagNum; //购买的装备背包数量
    public UInt32 dwWeekCardDay;    //周卡剩余天数
    public UInt32 dwMonthCardDay;   //月卡剩余天数
    public UInt64 qwMessage;    //消息推送选项
    public Byte byDisturb;      //免打扰模式
    public UInt32 dwRoleId = 0;   //角色id
    public UInt16 wZoneId = 0;      //大区id
    public string strUuid; // 帐号，通行证用户填通行证帐号，非通行证用户填，IPHONE UUID, GlobalVar.MAX_ACCNAMESIZE
    public string strPasswd; /// 非通行证用户填登录密码, GlobalVar.MAX_PASSWORD


    public override OctetsStream WriteData(OctetsStream os)
    {
        return os;
    }
} 
