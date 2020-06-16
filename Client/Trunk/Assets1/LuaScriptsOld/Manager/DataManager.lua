require("Datas/Base/DataCtrlBase")
require("Datas/GameDataCtrl")
require("Datas/LoginDataCtrl")

G_DataManager = nil
--角色逻辑数据控制器，统一创建和卸载
DataManager = class("DataManager", DataCtrlBase)

--lua构造方法
function DataManager:ctor(args)
    self.super.ctor(self)
    self.AccountDataList = {}
    self.roleDataList = {}
    G_DataManager = self;
    self:CreateAccountData()
end

--创建角色相关数据控制类
function DataManager:CreateUserData()
    self:CreateData(MainUserDataCtrl.New())
end
--创建账号相关数据控制类
function DataManager:CreateAccountData()
    self:CreateData(LoginDataCtrl.New(), true)
    self:CreateData(GameDataCtrl.New(), true)
end

function DataManager:CreateData(baseData, bAccount)
    if bAccount == true then
        table.insert(self.AccountDataList, baseData)
    else
        table.insert(self.roleDataList, baseData)
    end
    baseData:Awake()
end


--断线重连
function DataManager:OnReconnect()
    for k, v in ipairs(self.dataList) do
        print(k, v)
        v:OnReconnect();
    end

    for k, v in ipairs(self.AccountDataList) do
        v:OnReconnect(dt)
    end
end

--刷新数据
function DataManager:Update(dt)
    for k, v in ipairs(self.dataList) do
        v:Update(dt)
    end

    for k, v in ipairs(self.AccountDataList) do
        v:Update(dt)
    end
end

function DataManager:OnDestory()
    self.super.OnDestroy(self)
    for k, v in ipairs(self.dataList) do
        v:OnDestroy()
    end

    for k, v in ipairs(self.AccountDataList) do
        v:OnDestroy(dt)
    end
end




