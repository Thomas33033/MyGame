


G_LoginDataCtrl = nil
LoginDataCtrl = class("LoginDataCtrl", DataCtrlBase)

function LoginDataCtrl:ctor()
	--玩家角色列表
	self.Players = {}
end

function LoginDataCtrl:Awake()
	G_LoginDataCtrl = self
	self:RegisterMessages()
	self.super.Awake(self);
	G_UIManager:ShowUI('UI_Login');
end

function LoginDataCtrl:RegisterMessages()
	NetworkManager.RegisterProtoMsg("SC_ResAccountLogin", function(msgData) self:SC_ResAccountLogin(msgData) end)
	NetworkManager.RegisterProtoMsg("SC_ResCreateRole", function(msgData) self:SC_ResCreateRole(msgData) end)
end

--------------------接收服务器消息----------------------
----登录成功
function LoginDataCtrl:SC_ResAccountLogin(SC_ResAccountLogin_Data)
	local msgData = SC_ResAccountLogin_Data
	print(msgData.Account.LoginName)
	--if msgData.Code == 0 then
		print("登录成功")
		self:OnLoginSuccess(msgData)
	--elseif msgData.Code == 1 then
	--	error("用户名或密码错误")
	--	self:OnLoginFailed()
	--end
end

function LoginDataCtrl:SC_ResCreateRole(msgData)
	print(msgData.BaseInfo.Name)
	
end


--------------------向服务器发送消息----------------------
-- 请求登录服务器
function LoginDataCtrl:OnReqLogin()
	NetworkManager.SendProtoMsg("CS_AccountLogin", {
		LoginName = "ztgame",
		PassWordMD5 = "ztgame@123",
	} )
end

--请求创建角色
function LoginDataCtrl:OnReqCreateRole(name,gender,profession)
	
	local playerBaseInfo = {}
	playerBaseInfo.Gender = gender
	playerBaseInfo.Profession = profession
	playerBaseInfo.Name = name
	local msg = {}
	msg.BaseInfo = playerBaseInfo
	NetworkManager.SendProtoMsg("CS_CreateRole", msg)
end

--请求进入游戏
function LoginDataCtrl:CS_EnterGame(roleId)
	error("请求进入游戏")
	local msgData = {}
	msgData.UUID = roleId
	NetworkManager.SendProtoMsg("CS_EnterGame", msgData)
end
--------------------逻辑相关----------------------
-- 登录成功
function LoginDataCtrl:OnLoginSuccess(SC_ResAccountLogin_Data)
	local msgData = SC_ResAccountLogin_Data

	if msgData.Account ~= nil and msgData.Account.Players ~= nil then
		self.Players = msgData.Account.Players
	end
	
	G_UIManager:ShowUI(EGUINameContent.GUI_SelectRole);
	G_UIManager:RemoveUI(EGUINameContent.GUI_Login);
end

-- 登录失败
function LoginDataCtrl:OnLoginFailed()

end

--显示创建角色UI
function LoginDataCtrl:ShowCreateRoleUI()
	G_UIManager:ShowUI(EGUINameContent.GUI_CreateRole);	
end

function LoginDataCtrl:Update()
	self.super.Update(self)
end

function LoginDataCtrl:OnReconnect()
	self.super.OnReconnect(self)
end

function LoginDataCtrl:OnDestory()
	self.super.OnDestroy(self)
end






