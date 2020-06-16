
UI_Operation = UIPanelBase:new({
	img_playerIcon = nil
})

function UI_Operation:Awake(object)
	UIPanelBase.Awake(self,object)
end

function UI_Operation:Start(object)
		
end

function UI_Operation:OnEnable(object)
	
end

function UI_Operation:OnInit(args)
	self.img_playerIcon = self:GetLoader("img_playerIcon")
	--self.img_playerIcon.url = "Icon/18"
	self.img_playerIcon.url = "ui://Common/bj3344"
	local btn_login = self:GetObject("btn_bag")
	local btn_role = self:GetObject("btn_role")
	
	self:AddClickEvent(btn_login,function()
		error("打开背包界面")
   end)

	self:AddClickEvent(btn_role,function()
		G_UIManager:ShowUI('UI_Role');
		error("打开角色界面")
	end)


end


function UI_Operation:OnDestroy()
	UIPanelBase.OnDestroy(self)
	self.img_playerIcon = nil
end


