--界面基类
UIBase = class("UIBase")


--------------需要子类实现的方法---------------------
--子类实现 初始化界面成员变量
function UIBase:Init() end
--子类实现 初始化界面逻辑
function UIBase:Ready() end
--子类实现 
function UIBase:Update(dt) end
--子类实现 
function UIBase:OnMemberVariables() end
--子类实现 监听事件处理函数(lua内部的);
function UIBase:HandleNotification(notification) end

function UIBase:Awake(p_gameObject)
    self.gameObject = p_gameObject;
    self.transform = p_gameObject.transform;
	self.mSysTimer = SysTimer.New()
	self.mTableNotification = self.mTableNotification or {}
	self:OnMemberVariables()
    self:Init();
    self:RegisterNotify();
	self:Ready();
	--if self.Update then
	--	self.mUpdateFunc = function() self:Update(Time.deltaTime) end
	--	TimeManager:AddToUpdate( self.mUpdateFunc );
	--end

	--if self.uiName ~= nil then
	--	local soundId = LogicTools.GetUIPageSoundId(self.uiName.."Wnd")
	--	if soundId ~= 0 then
	--		LogicTools.PlayAudio(soundId)
	--	end
	--end
end

--显示UI函数(表示此UI已经打开且是隐藏的状态),(切勿删除和修改);
--注意：在lua中不要主动调用,用UIManager.OpenWndByLua;
function UIBase:Show()
	self:RegisterNotify();
	self:OnShow();
end

--隐藏UI函数,(切勿删除和修改);
--注意：在lua中不要主动调用,用UIManager.HideWndByLua;
function UIBase:Hide()
	self:UnRegisterNotify();
	self:OnHide();
end



--界面销毁
function UIBase:OnDestroy()

	--if self.Update then
	--	TimeManager:RemoveFromUpdate( self.mUpdateFunc );
	--	self.mUpdateFunc = nil;
	--end
    self:UnRegisterNotify();
	self:Clear();
	self:OnMemberVariables ()
	self.mSysTimer:OnDestroy()
	self.gameObject = nil;
	self.transform = nil;
	self.mTableNotification = nil
end

---------------------日常逻辑 ----------------------------
--监听事件注册函数;
function UIBase:RegisterNotify()
	for i,k in pairs(self.mTableNotification) do
		GyNotify:RegisterNotify(k, self.HandleNotification, self);
	end
end

--解除监听事件函数;
function UIBase:UnRegisterNotify()
	for _,v in pairs(self.mTableNotification) do
		GyNotify:UnRegisterNotify(v, self);
	end
end
