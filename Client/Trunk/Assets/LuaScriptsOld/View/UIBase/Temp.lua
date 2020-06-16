
UI_Temp = UIPanelBase:new({
	bagImage_loader = nil
})

function UI_Temp:Awake(object)
	UIPanelBase.Awake(self,object)
end

function UI_Temp:Start(object)
		
end

function UI_Temp:OnEnable(object)
	
end

function UI_Temp:OnInit(args)
	self.bagImage_loader = self:GetLoader("bagImage")
	self.bagImage_loader.url = "BigImage/login"	
	local btn_login = self:GetObject("btn_login")
	
	self:AddClickEvent(btn_login,function()
		G_GameDataCtrl:OnReqLogin()
   end)
end


function UI_Temp:OnDestroy()
	UIPanelBase.OnDestroy(self)
	self.bagImage_loader = nil
end


