local UIMailView = {}
setmetatable(UIMailView, {__index=UIBase})

function UIMailView: Create()
	self.name = UIMail
	self.path = Mail
end
function UIMailView:StartInit()
	self.BtnDelReaded.onClick:AddListener(function() self:ButtonClickHandler(self.BtnDelReaded) end)
	self.BtnOneKeyGet.onClick:AddListener(function() self:ButtonClickHandler(self.BtnOneKeyGet) end)
end

function UIMailView:SetUIComponent(child)
	if child.name == "MailItemScroller" then 
		self.MailItemScroller = child:GetComponent("Image")
	elseif child.name == "BtnDelReaded" then 
		self.BtnDelReaded = child:GetComponent("Button")
	elseif child.name == "BtnOneKeyGet" then 
		self.BtnOneKeyGet = child:GetComponent("Button")
	end
end

return UIMailView
