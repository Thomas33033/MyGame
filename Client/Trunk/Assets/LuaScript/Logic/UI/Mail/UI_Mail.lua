UI_Mail = {};
local UITable = require("UI/Mail/UI_MailView")
require "UI/Mail/UI_MailItemRender";

function UI_Mail.Create()
	local ui = {}
	setmetatable(ui, {__index=UITable})
	ui:Create()
	ui.layer = 1
	UI.LoadUI(ui)
	return ui
end

function UITable:OnMemberVariables()
	self.listView = nil
end

function UITable:Ready()
	self:OnInitEvent();
	self:OnInit()
end

function UITable:Clear()
	UI_Mail = nil;
end

function UI_Mail:OnInit()
	self.listView = ListView.New()
	self.listView:init(UI_MailItem,self.m_mailitem,self.m_MailItemScroller,G_MailCtrl.mailDataList)
end

function UI_Mail:OnInitEvent()
	UIHelper.AddButtonClick(self.btnClose,self.OnCloseBtnClick,self);
	UIHelper.AddButtonClick(self.btnDelRead,self.OnDelReaderBtnClick,self);
	UIHelper.AddButtonClick(self.btnTest,self.OnOneKeyGetBtnClick,self);
end

function UIBase:ButtonClickHandler(btn)
	if self.btnClose == btn then

	elseif self.btnTest == btn then

	elseif self.btnDelRead == btn then

	end
end


function UI_Mail:OnCloseBtnClick()
	LuaUIManager.RemoveUI(self.uiName)
end

function UI_Mail:OnDelReaderBtnClick()

end

function UI_Mail:OnOneKeyGetBtnClick()
	self:OnInit()
end

return UI_Mail.New();
