require "Common/SysTimer"
require "Common/UIHelper"
require "Common/UIBase"
require "Common/ListView"
require "Common/Event"


--负责界面创建、销毁、界面层级管理
--未实现
--1.界面层级管理
--2.界全屏界面隐藏3D场景和其底部UI

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


local arrUI = {}
local tfCanvas = UnityEngine.GameObject.Find("UIRoot").transform
local layerBox = {}
local uiTableBox = {}

for i=0,tfCanvas.childCount-1 do
    table.insert(layerBox,tfCanvas:GetChild(i))
    uiTableBox[i+1] = {}
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
    local obj = LoadTools.LoadUI(uiTable.path,uiTable.name,layerBox[1].gameObject)
    uiTable.gameObject = obj
    uiTable.transform = obj.transform
    UI.FindUIComponent(uiTable,uiTable.transform)

    obj:AddComponent(typeof(LuaUIComponent))
    uiTable.luaUI = obj:GetComponent("LuaUIComponent")

    uiTable.luaUI:SetData(uiTable)
    uiTable.uiId = GetIndex()

    uiTable.layer = 1

    table.insert(uiTableBox[uiTable.layer],uiTable)
    uiTable:Awake()
end

--引导调用
function UI.StartUI(uiTable)
    if uiTable ~= nil then
        uiTable:Start()
        Event.Call(EventType.LoadUI,uiTable.name)
        --GameData.guide:Run("OpenUI_"..uiTable.name)--
    end
end

--
local function DoDestroyUI(uiTable)
    uiTable:OnClose()
    UnityEngine.GameObject.Destroy(uiTable.gameObject)
    uiTable:Clear()
end

function UI.RemoveUI(uiTable)
    --Event.Call(EventType.SceneDestroyUI,uiTable.name)
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

    --此处会删除当前打开的界面
    if uiTable.layer == 1 then
        for i=#uiTableArr,index,-1 do
            local tempUI = uiTableArr[i]
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

function UI.FindUIComponent(ui,transform)
    for i=0,transform.childCount -1 do
        local child = transform:GetChild(i)
        if child.tag == "UIComponent" then
            ui:SetUIComponent(child)
        end
        if child.childCount > 0 then
            UI.FindUIComponent(ui,child)
        end
    end
end

function UI.FindRenderUI(ui,transform)
    for i=0,transform.childCount -1 do
        local child = transform:GetChild(i)
        ui:SetUIComponent(child)
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

--创建列表
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
    UI.OpenUI("UICombatPowerEffect",nDiff)
end

---------------------------------------------------------------------------------------------

--local rootLoading = UnityEngine.GameObject.Find("UIRootCanvas/PanelLoading").transform
--
--local objLoading = LuaTools.LoadUI("Loading","UILoading",rootLoading)
--local objLoadingBg = objLoading.transform:Find("BG").gameObject
--local objLoadingLabel = objLoading.transform:Find("LabelTip").gameObject
--
--local objLoadingAnim_1 = objLoading.transform:Find("Image").gameObject
--local objLoadingAnim_2 = objLoading.transform:Find("Image1").gameObject
--local objLoadingAnim_3 = objLoading.transform:Find("Image2").gameObject
--
--objLoading:SetActive(false)
--objLoadingBg:SetActive(false)
--objLoadingLabel:SetActive(false)

function UI.SetLoading(v,level)
    if IsNil(objLoading) == false then
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