--负责界面创建、销毁、界面层级管理

EUILayerType = {
    -- 主界面层级
    Base = 1,
    -- 二级界面层级
    Common = 2,
    -- 浮动窗口层
    FloatPage = 3,
    -- 进度界面层
    Loading = 4
}


UI = {}

UI.hasNotch = false

local deviceModel = GameConfig.DeviceModel

if deviceModel == "iPhone10,1" or deviceModel == "iPhone10,2" or deviceModel == "iPhone10,4" or deviceModel == "iPhone10,5" then

elseif string.find(deviceModel,"iPhone10") ~= nil
        or string.find(deviceModel,"iPhone11") ~= nil
        or string.find(deviceModel,"OPPO CPH1803") ~= nil
        or string.find(deviceModel,"OPPO PBEM00") ~= nil
        or string.find(deviceModel,"OPPO CPH1823") ~= nil
then

    UI.hasNotch = true
end

local arrUI = {}
local tfCanvas = UnityEngine.GameObject.Find("UIRootCanvas").transform
local layerBox = {}
local uiTableBox = {}

for i=0,tfCanvas.childCount-1 do
    table.insert(layerBox,tfCanvas:GetChild(i))
    uiTableBox[i+1] = {}
end

local arrObjtTest = {}
function UI.UITest( )
    table.insert(arrObjtTest,LuaTools.LoadUI("Fight3D","UIFightSuccuss",layerBox[3]))
end

function UI.UITestClose()
    if #arrObjtTest > 0 then
        UnityEngine.GameObject.Destroy(arrObjtTest[1])
        table.remove(arrObjtTest,1)
    end
end


function UI.Clearn()

    for i,v in ipairs(layerBox) do
        UI.DestroyChildren(v)
    end

    for i,v in ipairs(uiTableBox) do
        uiTableBox[i] = {}
    end

end

function UI.SetEnable(value)
    local canvasGroup = layerBox[1]:GetComponent("CanvasGroup")
    if canvasGroup == nil then
        canvasGroup = layerBox[1].gameObject:AddComponent("UnityEngine.CanvasGroup")
    end
    canvasGroup.blocksRaycasts = value
end

local uiIndex = 0
local function GetIndex()
    uiIndex = uiIndex + 1
    return uiIndex
end

function UI.LoadUI(uiTable)
    local obj = LuaTools.LoadUI(uiTable.path,uiTable.name,layerBox[uiTable.layer])
    uiTable.gameObject = obj
    uiTable.transform = obj.transform
    UI.FindUICompent(uiTable,uiTable.transform)
    uiTable.luaUI =  obj:GetComponent(LuaUIComponent)
    if uiTable.luaUI == nil then
        uiTable.luaUI = obj:AddComponent(LuaUIComponent)
    end
    uiTable.luaUI:SetData(uiTable)
    uiTable.uiId = GetIndex()
    table.insert(uiTableBox[uiTable.layer],uiTable)
    LuaTools.PlaySound("SE_UI_open")

    uiTable:Awake()
end

function UI.StartUI(uiTable)
    if uiTable ~= nil then
        uiTable:Start()
        Event.Call(EventType.LoadUI,uiTable.name)
        GameData.guide:Run("OpenUI_"..uiTable.name)--
    end
end


local function DoDestroyUI(uiTablea)
    uiTablea:OnClose()
    UnityEngine.GameObject.Destroy(uiTablea.gameObject)
    uiTablea:Clear()
end

function UI.RemoveUI(uiTable)
    Event.Call(EventType.SceneDestroyUI,uiTable.name)
    local uiTableArr = uiTableBox[uiTable.layer]
    local index = -1
    if uiTableArr == nil then
        return
    end
    for i,v in ipairs(uiTableArr) do
        if v == uiTable then
            index = i
        end
    end
    if index == -1 then
        return
    end

    if uiTable.layer == 1 then
        for i=#uiTableArr,index,-1 do
            local tempUI = 	uiTableArr[i]
            table.remove(uiTableArr,i)
            tempUI:DoClose()
        end

        index = index - 1

        if uiTableArr[index] ~= nil then
            uiTableArr[index]:OnEnable()
        end
    else
        table.remove(uiTableArr,index)
        uiTable:DoClose()
    end
end

function UI.RemoveAll()
    -- print("UI.RemoveAll")
    local uiTableArr = uiTableBox[1]
    for i=#uiTableArr,1,-1 do
        local tempUI = uiTableArr[i]
        if tempUI.lockui ~= true then
            table.remove(uiTableArr,i)
            tempUI:DoClose()
        end
    end

    uiTableArr = uiTableBox[2]
    for i=#uiTableArr,1,-1 do
        print(i,uiTableArr[i].name)
        local tempUI = uiTableArr[i]
        table.remove(uiTableArr,i)
        tempUI:DoClose()
    end
end

function UI.GetUI(name)
    for k,arrUI in pairs(uiTableBox) do
        for i,v in ipairs(arrUI) do
            if v.name == name then
                return v
            end
        end
    end

    return nil
end

function UI.FindUICompent(ui,transform)
    for i=0,transform.childCount -1 do
        local child = transform:GetChild(i)
        if child.tag == "uiCompent" then
            ui:SetUICompent(child)
        end
        if child.childCount > 0 then
            UI.FindUICompent(ui,child)
        end
    end
end

function UI.FindRenderUI(ui,transform)
    for i=0,transform.childCount -1 do
        local child = transform:GetChild(i)
        ui:SetUICompent(child)
        if child.childCount > 0 then
            UI.FindRenderUI(ui,child)
        end
    end
end

function UI.Instantiate(prefab,parent)
    local obj = UnityEngine.GameObject.Instantiate(prefab)
    if parent ~= nil then
        obj.transform:SetParent(parent)
        obj.transform.localPosition = Vector3(0,0,0);
        obj.transform.localScale = Vector3(1,1,1);
    end
    return obj
end

function UI.CreateRenderList(tf,render)
    local arr = {}
    for i=1,tf.childCount do
        local t = {}
        setmetatable(t, render)
        t:Init(tf:GetChild(i-1))
        table.insert(arr,t)
    end
    return arr
end

function UI.CreateRenderTable()
    local Render = {}
    Render.__index = Render

    Render.Awake = function(self)
    end

    Render.Init = function(self,tf)
        self.transform = tf
        self.name = tf.name
        self:Awake()
        UI.FindRenderUI(self,self.transform)
        local dc = self.transform.gameObject:AddComponent("DestroyCallbackComponent")
        dc:SetCall(function ()
            UI.ClearUITable(self)
        end)
    end

    Render.SetUICompent = function(self,child)

    end

    return Render
end

function UI.DropGold(p1,p2)
    LuaTools.DropItem("Orderart","DropItemGold",layerBox[1],p1,p2,0.5,1)
end

function UI.DropItemWorld(p1,p2,name)
    LuaTools.DropItemWorld("Orderart",name,layerBox[1],p1,p2,0.5,1)
end

function UI.DestroyChildren(transform)
    for i=transform.childCount-1,0,-1 do
        if transform:GetChild(i).gameObject ~= nil then
            UnityEngine.GameObject.Destroy(transform:GetChild(i).gameObject)
        end
    end
end

function UI.ShowPowerEffect(nDiff)
    -- local tempEquipUI = UI.GetUI("UIShipEquip")
    -- local tempInstituteUI = UI.GetUI("UIInstitute")
    -- -- print(tempEquipUI ~= nil)
    -- if tempEquipUI ~= nil and tempEquipUI.pageType == "part" then
    -- 	LuaTools.DelayCall(0.71,function ()
    -- 		UI.OpenUI("UICombatPowerEffect",nDiff)
    -- 	end)
    -- elseif tempInstituteUI ~= nil then
    -- 	LuaTools.DelayCall(0.71,function ()
    -- 		UI.OpenUI("UICombatPowerEffect",nDiff)
    -- 	end)
    -- else
    UI.OpenUI("UICombatPowerEffect",nDiff)
    -- end
end

---------------------------------------------------------------------------------------------

local rootLoading = UnityEngine.GameObject.Find("UIRootCanvas/PanelLoading").transform

local objLoading = LuaTools.LoadUI("Loading","UILoading",rootLoading)
local objLoadingBg = objLoading.transform:Find("BG").gameObject
local objLoadingLabel = objLoading.transform:Find("LabelTip").gameObject

local objLoadingAnim_1 = objLoading.transform:Find("Image").gameObject
local objLoadingAnim_2 = objLoading.transform:Find("Image1").gameObject
local objLoadingAnim_3 = objLoading.transform:Find("Image2").gameObject

objLoading:SetActive(false)
objLoadingBg:SetActive(false)
objLoadingLabel:SetActive(false)

function UI.SetLoading(v,level)
    if Slua.IsNull(objLoading) == false then
        objLoading:SetActive(v)
        objLoadingBg:SetActive(level > 1)
        objLoadingLabel:SetActive(level > 2)
    end
    --objLoadingAnim_1:SetActive(level > 1)
    --objLoadingAnim_2:SetActive(level > 1)
    --objLoadingAnim_3:SetActive(level > 1)
end

local uiHide = true
function UI.UIHide()
    uiHide = not uiHide
    local canvasGroup = tfCanvas:GetComponent("CanvasGroup")
    if canvasGroup == nil then
        canvasGroup = tfCanvas.gameObject:AddComponent("UnityEngine.CanvasGroup")
    end
    if uiHide then
        canvasGroup.alpha = 1
    else
        canvasGroup.alpha = 0
    end
end

function UI.ClearUITable(uiTable)
    for k,v in pairs(uiTable) do
        if type(uiTable[k]) == "table" then
            if uiTable[k].Dispose ~= nil then
                uiTable[k]:Dispose()
            end
        elseif type(uiTable[k]) == "userdata" then
            --uiTable[k]:Unlink()
        end
        uiTable[k] = nil
    end
end

---------------------------------------------------------------------------------------------
require("UI/UIUserResourceRender")
require("UI/UIUserResourceRender")
require("UI/UIRenderShipInfo")
require("UI/UIRenderItemInfo")
require("UI/UIRenderCrewInfo")
require("UI/UIRenderSkillInfo")
require("UI/UIRenderDropItemInfo")
require("UI/UIRenderCaptainSkillInfo")
require("UI/UIRenderBallInfo")
require("UI/UIRenderShipSkillInfo")

require("UI/UIRenderCrewEquipItemInfo")
require("UI/UIPrtyRender")
require("UI/UICurAndNextPrtyRender")
require("UI/UICrewBaseRender")
require("UI/UICrewSkillBaseRender")

require("UI/LuaAntList")

----ui必须在后面
require("UI/Activity/UIActiivtyNotice")
require("UI/Activity/UIActivityMain")
require("UI/Activity/UIActivitySeven7")
require("UI/Activity/UIActivitySeven8")
require("UI/Activity/UIActivitySignIn")
require("UI/Activity/UIActivityPioneerReward")
require("UI/Activity/UIActivityMilitaryRankReward")
require("UI/Activity/UIActivityGrandTotalRechargeReward")
require("UI/Activity_1/UIActivityFirstCharge")
require("UI/Activity/UIActivityAddUpRechargeReward")
require("UI/Activity/UIActivityAddUpExpendReward")
require("UI/Activity/UIActivityCumulativeLoginsReward")

require("UI/Activity/UIActivityLimitedTimeRecruitCrew")
require("UI/Activity/UIActivityGrowthFundReward")

require("UI/ActivityTip_3/ActivityRecruitTipUI")
require("UI/ActivityTip_4/ActivitySevenRewardTipUI")
require("UI/ActivityTip_1/ActivityCollectionTipUI")
require("UI/ActivityTip_2/ActivityLimitedTimeRecruitTipUI")
require("UI/ActivityTip_5/ActivityShellTreasureTipUI")

require("UI/Activity/UIActivityLimitedTimeMain")
require("UI/Activity/UIActivityShellTreasureRankReward")
require("UI/Activity/UIActivityShellTreasureReward")
require("UI/Activity/UIActivityExchange")

require("UI/UIRankPvp/UIRankPvp")
require("UI/UIRankPvp/UIRankLog")
require("UI/UIRankPvp/UIRankRewardHelp")

require("UI/Recruit3D/UIRecruitMain")
require("UI/Recruit3D/UIRecruitHeroInfo")
require("UI/Recruit3D/UIRecruitResult")
require("UI/Recruit3D/UIBeginnerRecruitPool")
require("UI/Recruit3D/UILimitedTimeRecruitRewardPool")

require("UI/GameGenius/UIGameGenius")

require("UI/Ranking/UIRanking")

require("UI/BigTurntable/UIBigTurntable_Lucky")
require("UI/BigTurntable/UIBigTurntable_Super")

require("UI/Warehouse/UIPropSynthesis")
require("UI/Warehouse/UIEquipLvUp")
require("UI/Warehouse/UIEquipLvUpBase")
require("UI/Warehouse/UIGiftBoxDetails")
require("UI/Warehouse/UIWarehouseNew")
require("UI/Warehouse/UIWarehouseNewSale")
require("UI/Warehouse/UIWarehouseNewAddCapacity")
require("UI/Warehouse/UIPropsDetails")
require("UI/Warehouse/UIPropsDetailsBase")
require("UI/Warehouse/UICrewChipDetails")
require("UI/Warehouse/UICrewPropsDetails")

require("UI/SeaBackpack/UISeaBackpack")

require("UI/Login/UIGameNotice")
require("UI/Login/UILogin")
require("UI/Login/UILoginServerList")
require("UI/Login/UILoginClause")

require("UI/OfflineRewards/UIOfflineRewards")

require("UI/Loading/UIBlackLoading")
require("UI/Loading/UISeaLoading")
require("UI/Loading/UIFightLoading")

require("UI/CrewHouse/UICrewhouse")
require("UI/CrewIllustration/UICrewIllustration")
require("UI/CrewHouse/UICrewIllustrationDetails")
require("UI/CrewHouse/UICrewBatchDel")

require("UI/CrewBringUp/UICrewBringUp")--船员培养界面
require("UI/CrewBringUp/UICrewBringUp_Details")
require("UI/CrewBringUp/UICrewEquipCompared")--船员装备对比界面
require("UI/CrewBringUp/UICrewDetailsIndependent")--船员详细独立界面
require("UI/CrewBringUp/UICrewUpStar")--船员升星界面
require("UI/CrewBringUp/UICrewLevelAndStarUp")--船员升级和升星整合界面
require("UI/CrewBringUp/UICrewUpStarResult")
require("UI/CrewBringUp/UICrewLvUpResult")
require("UI/CrewBringUp/UICrewPrtyShow")
require("UI/CrewBringUp/UICrewSimpleDetails")
require("UI/CrewBringUp/UICrewSimpleGetDetails")
require("UI/CrewBringUp/UICrewSkillDetails")
require("UI/CrewBringUp/UICrewFetterTips")


require("UI/CrewEquipLvUp/UICrewEquipLvUp")
require("UI/CrewEquipLvUp/UICrewEquipFastDecompose")
require("UI/CrewEquipLvUp/UICrewEquipDecomposeTip")
require("UI/CrewEquipLvUp/UICrewEquipDetails_Info")

require("UI/CaptainDailyTask_2/UINewCaptainTask")
require("UI/CaptainDailyTask/CaptainDailyTaskUI")
require("UI/UILvUp/UICaptainLvUp")
require("UI/UILvUp/UICaptainLvUp2")
require("UI/UILvUp/UICaptainLevelupUnlockFun")
require("UI/CaptainDailyTask_3/UIAchievement")
-- require("UI/CaptainDailyTask/DailyTaskRweardUI")
require("UI/CaptainDailyTask_1/CaptainInfoUI")

require("UI/UI_Mail/UIMail")

require("UI/FriendsUI/UIFriend")
require("UI/FriendsUI/UIAddFriend")
require("UI/FriendsUI/UISeeFriend")
require("UI/FriendsUI/UIFriendDetails")
require("UI/FriendsUI/UIFriendDetailsNormal")
require("UI/FriendsUI/UIFriendDetailsEmbed")
require("UI/FriendsUI/UIFriendDetailsCrewConfig")

require("UI/ShipEquip/UIShipPartUpgradeResult")
require("UI/ShipEquip/UIShipyardMain")
require("UI/ShipEquip/UIShipEquip")
require("UI/ShipEquip/UIShipEquip_Skill")
require("UI/ShipEquip/UIShipSkillRefreshConfirm")
-- require("UI/ShipEquip/UIShipEquip_Sailor")
-- require("UI/ShipEquip/UIShipEquip_Ball")
-- require("UI/ShipEquip/UIShipEquip_ConfigCrew")
require("UI/ShipEquip/UIShipEquip_Expand")
-- require("UI/ShipEquip/UIShipBallDesc")

require("UI/Shipyard/UIShipyard")
require("UI/Shipyard/UIShipBuildComplete")
require("UI/UIShipInfo/UIShipInfo")
require("UI/ShipEquip/UIShipDismantleTip")
require("UI/ShipEquip/UIShipDonateTip")
require("UI/Shipyard/UIBuildCampShip")

require("UI/Map3D/UIMap")

require("UI/UIPVP/UIPvpMain")
require("UI/UIPVP/UIPvpLoading")
require("UI/UIPVP/UIPvpBegin")
require("UI/UIPVP/UIPvpDie")
require("UI/UIPVP/UIPvpNotice")
require("UI/UIPVP/UIPvpSea")
require("UI/UIPVP/UIPvpSettlement3")
require("UI/UIPVP/UIPvpAllPlayerInfo")

require("UI/CaptainSkill/UICaptainSkill")
require("UI/CaptainSkill/UICaptainSkillInfo")
require("UI/CaptainSkill/UICaptainSkillChangeName")

require("UI/Fight3D/UIFight")
require("UI/Fight3D/UIFightFail")
require("UI/Fight3D/UIFightFailNew")
require("UI/Fight3D/UIFightSuccuss")
require("UI/Fight3D/UIFight3DOpen")
require("UI/Fight3D/UIFightReward")
require("UI/Fight3D/UIFightPvpVictory")
require("UI/Fight3D/UIFightPvpDefeat")
require("UI/Fight3D/UIFightBeigin")
require("UI/Fight3D/UIFightDialog")
require("UI/Fight3D/UIFightEnd")
require("UI/Fight3D/UIFightWarning")

require("UI/BuySupply/UIBuySupply")

require("UI/ShipInterior/UICrewDetailsNew")
require("UI/ShipInterior/UICrewSelectedEquip")
require("UI/ShipInterior/UICrewEquipDetails")

require("UI/TreasureBox/UITreasureBox")
require("UI/TreasureBox/UITreasureBoxReward")
require("UI/TreasureBox/UITreasureBoxOneShow")
require("UI/TreasureBox/UITreasureBoxOpen")
require("UI/TreasureBox/UIGetOneTreasureBox")

require("UI/PioneerReward/UIPioneerRewardNew")

require("UI/MessageBox/UIMessageBox")
require("UI/MessageBox/UIMessageBoxGoShop")
require("UI/MessageBox/UIMessageBoxUnlockCrew")
require("UI/MessageBox/UIMessageWithTips")
require("UI/MessageBox/UIError")
require("UI/MessageBox/UIUseGemMessageBox")
require("UI/MessageBox/UIDelCrewMessageBox")
require("UI/MessageBox/UICrewRefreshSkillMessageBox")
require("UI/MessageBox/UIHelp")
require("UI/MessageBox/UISmallTip")

require("UI/RewardTask/UIRewardAnswer")
require("UI/RewardTask/UIRewardGoodsTask")--旧的以后不要啦！
require("UI/RewardTask/UIRewardTask")
require("UI/RewardTask/UIRewardTaskReachedThLlimit")
require("UI/RewardTask/UIRewardTaskRefreshReachedThLlimit")
require("UI/RewardTask/UIRewardTaskUseItemRefreshTip")
require("UI/RewardTask/UIGetCrew")

require("UI/UILvUp/UICampUP")
require("UI/Camp/UIFaction")
require("UI/Camp/UIFactionTec")
require("UI/Camp/UIFactionInfo")
require("UI/SeaZero/UIFactionSelect")

require("UI/ChatUI/UIChat")
require("UI/ChatUI/UIFace")
require("UI/ChatUI/HearsayUI")

require("UI/Guide/UIPlayerName")
require("UI/Guide/UIGuideTalk")
require("UI/Guide/UIGuideBuff")
require("UI/Guide/UIManipulateHints1")
require("UI/Guide/UIManipulateHints2")
require("UI/Guide/UIManipulateHints3")
require("UI/Guide/UIManipulateHints4")
require("UI/Guide/UIManipulateHints5")
require("UI/Guide/UIManipulateHints6")
require("UI/Guide/UIManipulateHints7")
require("UI/Guide/UIManipulateHints8")
require("UI/Guide/UIManipulateHints9")
require("UI/Guide/UIManipulateHints10")
require("UI/Guide/UIManipulateHints11")
require("UI/Guide/UIManipulateHints13")
require("UI/Guide/UIGuideSea")
require("UI/Guide/UIGuideComeOnStage")
require("UI/Guide/UIGuideComeOnStage1")
require("UI/Guide/UIGuideGift")
require("UI/Guide/UIGuideFinger")
require("UI/Guide/UIGuideMessage1")
require("UI/Guide/UIGuideMessage2")
require("UI/Guide/UIGuideMessage3")
require("UI/Guide/UIGuideMessage4")
require("UI/Guide/UIGuideMessage5")
require("UI/Guide/UIGuideMessage6")
require("UI/Guide/UIGuideMessage7")
require("UI/Guide/UIGuideMessage8")
require("UI/Guide/UIGuideMessage80")
require("UI/Guide/UIGuideMessage9")
require("UI/Guide/UIGuideMessage10")
require("UI/Guide/UIGuideMessage11")
require("UI/Guide/UIGuideMessage12")
require("UI/Guide/UIGuideMessage13")
require("UI/Guide/UIGuideMessage17")
require("UI/Guide/UIGuideMessage18")
require("UI/Guide/UIGuideCaptainLevelup")
require("UI/Guide/UISelectedCamp1")
require("UI/Guide/UISelectedCamp2")
require("UI/Guide/UIGuideStationedHarbour")
require("UI/Guide/UIGuideEnterHarbour")
require("UI/Guide/UISelectedCamp")
require("UI/Guide/UISelectedCamp3")
require("UI/Guide/UISelectedCamp4")
require("UI/Guide/UIGuideGetHero")
require("UI/Guide/UIGuideGetHero2")

require("UI/SeaEvent/UISeaEventReward")
require("UI/SeaUI/SeaUI")
require("UI/MessageBox/UIRewardMessage")
require("UI/MessageBox/UIShowReward")
require("UI/SeaUI/UIShipSreck")
require("UI/SeaEvent/UIGetSeaBuff")
require("UI/SeaUI/PlayerShipInfo")
require("UI/SeaUI/UISeaTask")
require("UI/SeaTaskUI/UITaskReward")
require("UI/SeaTaskUI/UITaskFailed")
require("UI/SeaTaskUI/UITaskInfo")
require("UI/SeaUI/UISeaBuff")
require("UI/SeaUI/UIMovie")
require("UI/SeaUI/UISeaType10GameOver")
require("UI/SeaUI/UISeaType10Reward")
require("UI/SeaUI/UIGetSeaItem")

require("UI/ShipDisplay/UIAllShipDisplay")

require("UI/SeaShipState/UISeaShipState")
require("UI/SeaShipState/UISeaShipState_Buff")
-- require("UI/SeaShipState/UISeaShipState_WareHouse")
require("UI/SeaShipState/UISeaShipState_TaskProp")
require("UI/SeaShipState/UISeaWarehouseThrowAwayProp")
-- require("UI/SeaShipState/UISeaGoPort")

require("UI/ShipSinking/ShipSinking")

require("UI/TavernMain/UITavernMain")
require("UI/TavernMain/UILinkGame")
require("UI/TavernMain/UIPicturePuzzleGame")
require("UI/TavernMain/UIBeadGame")
require("UI/TavernMain/UIChipShop")

require("UI/Trade/UITrade")
require("UI/Trade/UITradeShipList")
require("UI/Trade/UITradeChooseGift")
require("UI/Trade/UITradeChooseShip")
require("UI/Trade/UITradeChooseGoods")
require("UI/Trade/UITradeFleetInfo")
require("UI/Trade/UITradeLog")
require("UI/Trade/UITradeHelp")

require("UI/UIShop/UIShop")

require("UI/UIShop/UIShopSingle")
require("UI/UIShop/UIShopConfirm")
require("UI/UIShop/UIShopConfirm2")

require("UI/SeaEvent/UIEvent")
require("UI/SeaEvent/UIEventFail")
require("UI/SeaEvent/UIEventSuccess")
require("UI/SeaUI/UISupplyPointDeplete")
require("UI/Map3D/UISweepReward")
require("UI/SeaUI/UIFindHarbour")
require("UI/SeaUI/UIStationedHarbour")
require("UI/SeaUI/UIEnterHarbour")
require("UI/SeaEvent/UISeaEvent")
require("UI/SeaEvent/UIDiscoverEvent")
require("UI/SeaEvent/UIDiscoverLight")
require("UI/SeaUI/UIWelComeSea")
require("UI/SeaTaskUI/UITradeTask")
require("UI/SeaEvent/UISeachTreasure")
require("UI/SeaEvent/UISeachTreasureEvent")
require("UI/SeaEvent/UISquirrel")
require("UI/SeaEvent/UISquirrelReward")
require("UI/SeaEvent/UIEnterGame")

require("UI/Instance/UIInstance")
require("UI/Instance/UIInstanceSelDifficult")
require("UI/Instance/UIChallengeVictory")

require("UI/MapUI/UIWorldInstance")

require("UI/MainUI/UIFishFruit")
require("UI/MainUI/UIChangeName")
require("UI/MainUI/UIExchangeCode")
require("UI/MainUI/UILanguage")
require("UI/MainUI/UICombatPowerEffect")
require("UI/MainUI/UIPartLvUpEffect")
require("UI/MainUI/UIMain")
require("UI/MainUI/UIGameConfig")
require("UI/MainUI/UITips")
require("UI/MainUI/UIRewardTips")

require("UI/Institute/UIInstitute")
require("UI/Institute/UIInstituteConfirm")

require("UI/MessageBox/UIMessageNotice")

require("UI/ShipDress/UIShipDress")
require("UI/ShipDress/UIShipDeckSkinMain")

require("UI/UICollection/UICollection")
require("UI/FriendsUI/UIFriendDetailsCrewInfo")

require("UI/Activity/UIShipDisplay")

require("UI/UIEquipUp/UIEquipUp")
require("UI/UIEquipUp/UICrewEquipDismantleTip")
require("UI/SeaUI/UIWorldInstanceReward")

require("UI/Loading/UISeaToFightLoading")
require("UI/SeaEvent/UIFishing")
require("UI/UIAppointCrew/UIAppointCrew")

require("UI/UIAppointCrew/UIFetterTips")
require("UI/UIAppointCrew/UICultivateCrew")
require("UI/Camp/UIFactionFirstReward")
require("UI/Camp/UIFactionTips")
require("UI/CaptainDailyTask/UITurnOverCampThing")
require("UI/CaptainDailyTask/UICampTaskRewardPreview")
require("UI/CaptainDailyTask/UICampTaskContributionPreview")

require("UI/LostSea/UILostSeaHard")
require("UI/LostSea/UILostSeaMain")
require("UI/LostSea/UILostSeaMeetEnemy")
require("UI/LostSea/UILostSeaRandomEvent")
require("UI/LostSea/UILostSeaBag")
require("UI/Warehouse/UICrewLostSeaBuffDetails")
require("UI/UIAppointCrew/UILostSeaAppointCrew")
require("UI/LostSea/UILostSeaBuffRemove")
require("UI/LostSea/UILostSeaMessageBox")
require("UI/Instance/UIInstanceReward")
require("UI/LostSea/UILostSeaMessageBoxTwo")
require("UI/Loading/UILostSeaNextFightLoading")
require("UI/LostSea/UILostSeaStageState")
require("UI/LostSea/UILostSeaEnd")
require("UI/LostSea/UILostSeaFightAward")
require("UI/LostSea/UILostSeaFightAwardBox")
require("UI/LostSea/UILostSeaBuffLevelUp")
require("UI/CrewTrial/UICrewTrialChapter")
require("UI/CrewTrial/UICrewTrailInfo")
require("UI/Adventure/UIAdventure")

require("UI/Activity/UIActivityMonthCard")
require("UI/Activity_1/UIActivityTimeGift")

require("UI/MessageBox/UISmallTipWithPos")
require("UI/UIAppointCrew/UIAppointFetterHelp")
require("UI/SeaEvent/UISeaEventAdventure")


function UI.OpenUI(name,...)

    local uiTable = nil
    if _G[name] ~= nil then
        uiTable = _G[name].Create()
    else
        return nil
    end

    if select("#", ...) > 0 then
        uiTable:SetData(...)
    end
    uiTableArr = uiTableBox[uiTable.layer]

    if #uiTableArr > 1 then
        uiTableArr[#uiTableArr - 1]:OnCover()
    end

    return uiTable
end

function UI.OpenOrGetUI(name,...)
    local uiTable = UI.GetUI(name)
    if _G[name] ~= nil and uiTable == nil  then
        uiTable = _G[name].Create()
    end

    if select("#", ...) > 0 then
        uiTable:SetData(...)
    end
    uiTableArr = uiTableBox[uiTable.layer]

    if #uiTableArr > 1 then
        uiTableArr[#uiTableArr - 1]:OnCover()
    end

    return uiTable
end