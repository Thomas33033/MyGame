--- 角色界面逻辑控制器
--- Generated by EmmyLua(https://github.com/EmmyLua)
--- Created by chenhaining.
--- DateTime: 2019/11/14 16:25
---
require "View/UI_Role"

G_RoleDataCtrl = nil
RoleDataCtrl = class("RoleDataCtrl",DataCtrlBase)

function RoleDataCtrl:Awake()
    G_RoleDataCtrl = self
    self:RegisterMessages()
    self.super.Awake(self)
end

function RoleDataCtrl:RegisterMessages()
    --NetworkManager.RegisterProtoMsg("SC_ResAccountLogin", function(msgData) self:SC_ResAccountLogin(msgData) end)
    --NetworkManager.RegisterProtoMsg("CS_CreateRole", function(msgData) self:CS_CreateRole(msgData) end)
end

function RoleDataCtrl:Update()
    self.super.Update(self)
end

function RoleDataCtrl:OnReconnect()
    self.super.OnReconnect(self)
end

function RoleDataCtrl:OnDestory()
    self.super.OnDestory(self)
end