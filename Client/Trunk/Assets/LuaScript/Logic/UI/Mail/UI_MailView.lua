--注意：这个文件由工具自动生成,手动修改可能会被覆盖;
local UI_MailView = {}
setmetatable(UI_MailView, {__index=UIBase})

function UI_MailView:Create()
	self.name = "UI_Mail"
	self.path = "SeaAdventure"
end

function UI_MailView:StartInit()
	self.btnClose.onClick:AddListener(function() self:Close() end)
	self.btn_SmallClose.onClick:AddListener(function() self:ButtonClickHandler(self.btn_SmallClose) end)
end

function UI_MailView:SetUIComponent(child)

	if child.name == "btnClose" then
		self.btnClose = child:GetComponent("Button")
	elseif child.name == "txtTitle" then
		self.txtTitle = child:GetComponent("Text")
	elseif child.name == "btn_SmallClose" then
		self.btn_SmallClose = child:GetComponent("Button")
	elseif child.name == "txtNotice" then
		self.txtNotice = child:GetComponent("Text")
	elseif child.name == "Item_drop_1" then
		self.Item_drop_1 = child.gameObject;
	end
end

return UI_MailView;
