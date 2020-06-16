UI_SelectRole = UIPanelBase:new({
    bagImage_loader = nil,
    txt_name = nil,
    btn_ok = nil,
    list_role = nil,
    selectedPlayer = nil
})

function UI_SelectRole:Awake(object)
    UIPanelBase.Awake(self, object)
end

function UI_SelectRole:OnInit(args)
    self.bagImage_loader = self:GetLoader("imgbg_loader")
    self.txt_name = self:GetTextField("txt_name")
    self.btn_ok = self:GetButton("btn_ok")
    self.list_role = self:GetList("list_role")
    self.bagImage_loader.url = "BigImage/selectRole"

    self:AddClickEvent(self.btn_ok, function()

       G_LoginDataCtrl:CS_EnterGame( self.selectedPlayer.UUID);
    end)

    UIHelper.SetListItemRender(self.list_role, function(number, obj)
        self:RenderListItem(number, obj)
    end)
    self.list_role.numItems = 2

    if #G_LoginDataCtrl.Players > 0 then
        self.selectedPlayer = G_LoginDataCtrl.Players[1]
        self.txt_name.text = self.selectedPlayer.Name
        self.btn_ok.visible = true

    else
        self.txt_name.text = "没有角色"
        self.btn_ok.visible = false
    end

end

function UI_SelectRole:RenderListItem(index, obj)
    local imgLock = UIHelper.GetImage(obj, "img_lock")
    local imgAdd = UIHelper.GetObject(obj, "img_add")
    local btn_bg = UIHelper.GetObject(obj, "btn_bg")
    local img_icon_loader = UIHelper.GetLoader(obj, "img_icon")

    imgLock.visible = false
    self:AddClickEvent(btn_bg, function()
        G_LoginDataCtrl:ShowCreateRoleUI()
    end)

    if G_LoginDataCtrl.Players[index + 1] ~= nil then
        imgAdd.visible = false
        img_icon_loader.visible = true
        img_icon_loader.url = "Icon/19"

    else
        imgAdd.visible = true
        img_icon_loader.visible = false
    end
    --
    -- obj
    -- img_add
    --    MailItem item = (MailItem)obj;
    --    item.setFetched(index % 3 == 0);
    --    item.setRead(index % 2 == 0);
    --    item.setTime("5 Nov 2015 16:24:33");
    --    item.title = index + " Mail title here";
end

function UI_SelectRole:Start(object)
end

function UI_SelectRole:OnEnable(object)
end

function UI_SelectRole:OnDestroy()
    UIPanelBase.OnDestroy(self)
    self.bagImage_loader = nil
end

------------------------------
-- UI_SelectRoleItem = 
