require "logic/PB_MANIFEST"
require "logic/PROTO_ENUM"
require "logic/PROTO_MSG"
require "logic/Logic_Config"
require "Common/CtrlBase"

--游戏角色逻辑控制层管理器
--逻辑层数据创建和销毁
CtrlManager = {
    ctrlList = {}
}
local this = CtrlManager

function CtrlManager.RegisterListener()
    for k, v in pairs(ModelCtrlArray) do
        if v ~= nil then
            LuaTools.TableInsert(this.ctrlList,v)
        end
    end
end

--断线重连
function CtrlManager.OnReconnect()
    for k, v in pairs(this.ctrlList) do
        if v.OnReconnect ~= nil then
            v:OnReconnect();
        end
    end
end

--刷新数据
function CtrlManager.Update(dt)
    for k, v in ipairs(this.ctrlList) do
        if v.OnUpdate ~= nil then
            v:OnUpdate(dt)
        end
    end
end

--销毁逻辑层，用于切换到登陆界面
function CtrlManager.OnDestory()
    for k, v in ipairs(this.ctrlList) do
        if v.OnDestroy ~= nil then
            v:OnDestroy(dt)
        end
    end
end




