local UIMainView = {}
setmetatable(UIMainView, {__index=UIBase})

function UIMainView: Create()
	self.name = "UIMain"
	self.path = "Main"
end
function UIMainView:StartInit()
	self.btn_pause.onClick:AddListener(function() self:ButtonClickHandler(self.btn_pause) end)
	self.btn_spawn.onClick:AddListener(function() self:ButtonClickHandler(self.btn_spawn) end)
end

function UIMainView:SetUIComponent(child)
	if child.name == "btn_pause" then 
		self.btn_pause = child:GetComponent("Button")
	elseif child.name == "btn_spawn" then 
		self.btn_spawn = child:GetComponent("Button")
	elseif child.name == "FriendSlotContainer" then 
		self.FriendSlotContainer = child:GetComponent("Image")
	elseif child.name == "EnemySlotContainer" then 
		self.EnemySlotContainer = child:GetComponent("Image")
	end
end

return UIMainView
