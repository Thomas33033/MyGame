--注意：这个文件由工具自动生成,手动修改可能会被覆盖;

local UI_Mail = class("UI_Mail",UIBase);

function UI_Mail:Init()
	self.uiName = "UI_Mail"
	self.m_BackImg = self.transform:Find("m_BackImg"):GetComponent("Button");
	self.m_closeBtn = self.transform:Find("root/m_closeBtn"):GetComponent("Button");
	self.m_Tips = self.transform:Find("root/m_Tips"):GetComponent("Text");
	self.m_MailItemScroller = self.transform:Find("root/m_MailItemScroller").gameObject;
	self.m_DelReadedBtn = self.transform:Find("root/m_DelReadedBtn"):GetComponent("Button");
	self.m_OneKeyGetBtn = self.transform:Find("root/m_OneKeyGetBtn"):GetComponent("Button");
	self.m_TestBtn = self.transform:Find("root/m_TestBtn"):GetComponent("Button");
end

function UI_Mail:ClearMembers()
	self.m_BackImg = nil;
	self.m_closeBtn = nil;
	self.m_Tips = nil;
	self.m_MailItemScroller = nil;
	self.m_DelReadedBtn = nil;
	self.m_OneKeyGetBtn = nil;
	self.m_TestBtn = nil;
end

return UI_Mail;
