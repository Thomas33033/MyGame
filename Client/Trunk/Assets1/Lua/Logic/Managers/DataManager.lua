--游戏角色逻辑控制层管理器
--逻辑层数据创建和销毁
DataManager = {
    ctrlList = {}
}
local this = DataManager

function DataManager.RegisterListener()
    for k, v in pairs(ModelCtrlArray) do
        LuaTools.TableInsert(this.ctrlList,v.OnCreate())
    end
end


--断线重连
function DataManager.OnReconnect()
    for k, v in pairs(this.ctrlList) do
        if v.OnReconnect ~= nil then
            v:OnReconnect();
        end
    end
end

--刷新数据
function DataManager.Update(dt)
    for k, v in ipairs(this.ctrlList) do
        if v.OnUpdate ~= nil then
            v:OnUpdate(dt)
        end
    end
end

--销毁逻辑层，用于切换到登陆界面
function DataManager.OnDestory()
    for k, v in ipairs(this.ctrlList) do
        if v.OnDestroy ~= nil then
            v:OnDestroy(dt)
        end
    end


end




