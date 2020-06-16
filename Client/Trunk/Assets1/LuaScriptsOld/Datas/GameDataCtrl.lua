



G_GameDataCtrl = nil;
GameDataCtrl = class("GameDataCtrl",DataCtrlBase)


function GameDataCtrl:Awake()
	self.super.Awake(self);
	G_GameDataCtrl = self;
    self:RegisterMessages()
end

function GameDataCtrl:RegisterMessages()
	NetworkManager.RegisterProtoMsg("SC_ResEnterGame", function(msgData) self:SC_ResEnterGame(msgData) end)

end

------------------------------------------------------------
function GameDataCtrl:SC_ResEnterGame(msgData)
	error("进入游戏，初始化游戏大厅")
	G_UIManager:RemoveUI(EGUINameContent.GUI_Login)
	G_UIManager:RemoveUI(EGUINameContent.GUI_SelectRole)
	G_UIManager:RemoveUI(EGUINameContent.GUI_CreateRole)
	G_UIManager:ShowUI(EGUINameContent.GUI_Operation);
end

------------------------------------------------------------
function GameDataCtrl:OnReqLogin()
	NetworkManager.OnReconnection()
end


function GameDataCtrl:OnConnectionOk()
	G_LoginDataCtrl:OnReqLogin()
end

----------------------------------------------------------
function GameDataCtrl:Update()  
	self.super.Update(self)
end

function GameDataCtrl:OnReconnect() 
	self.super.OnReconnect(self)
end

function GameDataCtrl:OnDestory() 
	 self.super.OnDestory(self)
end
