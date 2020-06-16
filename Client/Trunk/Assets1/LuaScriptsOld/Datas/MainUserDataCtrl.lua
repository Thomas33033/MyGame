G_MainUserDataCtrl = nil
MainUserDataCtrl = class("MainUserDataCtrl", DataCtrlBase)

function MainUserDataCtrl:Awake()
	G_MainUserDataCtrl = self
	self:RegisterMessages()
	self.super.Awake(self);
end

function MainUserDataCtrl:RegisterMessages()
	NetworkManager.RegisterProtoMsg("SC_ResEnterGame", function(msgData) self:SC_ResEnterGame(msgData) end)
end

--------------------接收服务器消息----------------------
function MainUserDataCtrl: SC_ResEnterGame(SC_ResEnterGameData)
	local msgdata = SC_ResEnterGameData
end
--------------------向服务器发送消息----------------------


--------------------逻辑相关----------------------
function MainUserDataCtrl:Update()
	self.super.Update(self)
end

function MainUserDataCtrl:OnReconnect()
	self.super.OnReconnect(self)
end

function MainUserDataCtrl:OnDestory()
	self.super.OnDestroy(self)
end