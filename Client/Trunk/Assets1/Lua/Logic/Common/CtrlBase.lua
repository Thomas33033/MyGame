CtrlBase = class("CtrlBase")

function CtrlBase:ctor()
    self:Awake()
end

--需要重写改方法
function CtrlBase:OnInit() end
function CtrlBase:OnClear() end
function CtrlBase:OnUpdate(dt) end

function CtrlBase:Awake()
    self.msgList = {}
    self.mSysTimer = SysTimer.New()
    self:OnInit()
    self:RegisterListener()
end

--注册服务器消息
function CtrlBase:RegisterNetWorkListener(msgName, func)
    Network:AddListener(msgName, func, self);
    LuaTools.TableInsert(self.msgList,msgName)
end

--添加定时器
function CtrlBase:AddTimer(interval, repeatCount, timerFunc)
    self.mSysTimer.AddTimer(interval,repeatCount,timerFunc,self)
end

function CtrlBase:RemoveTimer(timer)
    self.mSysTimer.DeleteTimer(timer)
end

--销毁回调
function CtrlBase:OnDestroy()
    for k,v in pairs(self.msgList) do
        Network:RemoveListener(k,v)
    end
    self.mSysTimer:OnDestroy()
    self.OnClear()
end