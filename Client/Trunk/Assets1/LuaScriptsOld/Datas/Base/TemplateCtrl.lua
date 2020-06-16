--模板控制器，复制后替换Temp字符串
G_Temp = nil
Temp = class("Temp", DataCtrlBase)

function Temp:Awake()
	G_Temp = self
	self:RegisterMessages()
	self.super.Awake(self);
end

function Temp:RegisterMessages()
	NetworkManager.RegisterProtoMsg("SC_ResEnterGame", function(msgData) self:SC_ResEnterGame(msgData) end)
end

--------------------接收服务器消息----------------------
function Temp: SC_ResEnterGame(SC_ResEnterGameData)
	local msgdata = SC_ResEnterGameData
end
--------------------向服务器发送消息----------------------


--------------------逻辑相关----------------------
function Temp:Update()
	self.super.Update(self)
end

function Temp:OnReconnect()
	self.super.OnReconnect(self)
end

function Temp:OnDestory()
	self.super.OnDestroy(self)
end