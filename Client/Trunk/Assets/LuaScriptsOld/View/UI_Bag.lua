require("Datas/PackageDataCtrl")
UI_Bag = UIPanelBase:new({
	bagImage_loader = nil,
	btn_equip = nil,
	btn_pet = nil,
	btn_gem = nil,
	btn_food = nil,
	list_items = nil,
})

function UI_Bag:Awake(object)
	UIPanelBase.Awake(self,object)
end

function UI_Bag:OnDestroy()
	error("---UI_Bag:OnDestroy---")
	UIPanelBase.OnDestroy(self)
	self.btn_equip = nil
	self.btn_pet = nil
	self.btn_gem = nil
	self.btn_food = nil
	self.list_items = nil
end


function UI_Bag:Start(object)
		
end

function UI_Bag:OnEnable(object)
	
end

function UI_Bag:OnInit(args)

	self:AddClick("btn_equip",function()
		self:ShowByType(BagItems.Type.Equip)
   end)

	self:AddClick("btn_food",function()
		self:ShowByType(BagItems.Type.Food)
	end)

	self:AddClick("btn_pet",function()
		self:ShowByType(BagItems.Type.Pet)
	end)

	self:AddClick("btn_gem",function()
		self:ShowByType(BagItemsType.Gem)
	end)

	self.list_items = self.GetList(self, "list_items");
end

function ShowByType(type)
	---@type PackageGroup
	local packageGroup = G_BagDataCtrl:GetOrCreatePackageGroup(type)

end



