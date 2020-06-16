
UI_Login = UIPanelBase:new({
	bagImage_loader = nil
})

function UI_Login:Awake(object)
	UIPanelBase.Awake(self,object)
end

function UI_Login:Start(object)
		
end

function UI_Login:OnEnable(object)
	
end

function UI_Login:OnInit(args)
	self.bagImage_loader = self:GetLoader("bagImage")
	self.bagImage_loader.url = "BigImage/login"	
	local btn_login = self:GetObject("btn_login")
	
	self:AddClickEvent(btn_login,function()
		G_GameDataCtrl:OnReqLogin()
   end)
end


function UI_Login:OnDestroy()
	UIPanelBase.OnDestroy(self)
	self.bagImage_loader = nil
end


