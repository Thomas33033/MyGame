UI_MailItem = class("UI_MailItem",ListViewItem)

function UI_MailItem:ctor()
    self:OnMemberVariables()
end

function UI_MailItem:OnMemberVariables()
    self.m_roleNum = nil
    self.m_stateImg = nil
    self.m_biaoqian =nil
    self.m_curServerName = nil
    self.data = nil
    self.gameObject = nil
end

function UI_MailItem:Awake(uObj, serverInfo, curSelected)
    self.gameObject = uObj
    self.data = serverInfo

    self.mailIconImg = UIHelper.GetImage(uObj, "mailIconImg")
    self.mailNameText = UIHelper.GetText(uObj, "mailNameText")
    self.timeText = UIHelper.GetText(uObj, "timeText")
    self.gettedIconImg = UIHelper.GetImage(uObj, "gettedIconImg")

    UIHelper.AddButtonClick(uObj, function()
        self:OnIconClickEvent()
    end)
end

function UI_MailItem:SetData(mailData)
    self.mailNameText.text = mailData.name
    error( math.floor(mailData.id % 2))
    if math.floor(mailData.id % 2) == 0 then
        self.gameObject:GetComponent("RectTransform").sizeDelta = Vector2.New(480, 150);
    else
        self.gameObject:GetComponent("RectTransform").sizeDelta = Vector2.New(480, 100);
    end
    --BundleManager:SetAssetBundleSprite( self.m_stateImg, LuaBundlePathString.Atlas_CommonUISpritePath, stateImgName)
end
function UI_MailItem:OnIconClickEvent()
    error("-----------OnIconClickEvent--------")
end

function UI_MailItem:OnDestroy()
    self:OnMemberVariables()
end