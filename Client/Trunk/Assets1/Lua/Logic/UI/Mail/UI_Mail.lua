local UI_Mail = require "Logic/UI/Mail/UI_Mail_Attr";
require "Logic/UI/Mail/UI_MailItem";

--声明和清理成员变量函数，所有的成员变量必须在这里注册和清除;
function UI_Mail:OnMemberVariables()
	--for example
	--self.testMemberVariable = nil;
	self.listView = nil
end

--Awake初始化函数，给按钮添加事件等可以放在这里;
function UI_Mail:Ready()
	self:OnInitEvent();
	self:OnInit()
end

--显示UI函数;
function UI_Mail:OnShow()

end

--隐藏UI函数;
function UI_Mail:OnHide()

end

--销毁清理函数;
function UI_Mail:Clear()
	UI_Mail = nil;
end

--监听事件处理函数(lua内部的);
function UI_Mail:HandleNotification(notification)
end

--感兴趣的消息号;
UI_Mail.mTableNotification = {

};

function UI_Mail:OnInit()
	--G_MailCtrl.mailDataList
	self.listView = ListView.New()
	self.listView:init(UI_MailItem,self.m_mailitem,self.m_MailItemScroller,G_MailCtrl.mailDataList)
end

function UI_Mail:OnInitEvent()
	UIHelper.AddButtonClick(self.m_closeBtn,self.OnCloseBtnClick,self);
	UIHelper.AddButtonClick(self.m_DelReadedBtn,self.OnDelReaderBtnClick,self);
	UIHelper.AddButtonClick(self.m_TestBtn,self.OnOneKeyGetBtnClick,self);
end

function UI_Mail:OnCloseBtnClick()
	LuaUIManager.RemoveUI(self.uiName)
end

function UI_Mail:OnDelReaderBtnClick()
	error("==============")

end

function UI_Mail:OnOneKeyGetBtnClick()
	self:OnInit()
end

return UI_Mail.New();
