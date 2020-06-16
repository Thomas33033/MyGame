UI_CreateRole = UIPanelBase:new({
    btn_create = nil,
    btn_back = nil,
    btn_girl = nil,
    btn_boy = nil,
    input_text = nil,
    btn_create = nil,
    bagImage_loader = nil,
})

function UI_CreateRole:Awake(object)
    UIPanelBase.Awake(self, object)
end

function UI_CreateRole:OnInit(args)

    self.input_text = self:GetTextField("input_text")
    self.btn_create = self:GetButton("btn_create")
    self.btn_back = self:GetButton("btn_back")
    self.btn_girl = self:GetButton("btn_girl")
    self.btn_boy = self:GetButton("btn_boy")
    self.bagImage_loader = self:GetLoader("imgbg_loader")
    self.bagImage_loader.url = "BigImage/selectRole"

    self:AddClickEvent(self.btn_create, function()
        G_LoginDataCtrl:OnReqCreateRole("张小凡", EGenders.MALE, 1)
    end)

    self:AddClickEvent(self.btn_back, function()
        self:OnClose();
    end)
end

function UI_CreateRole:Start(object)

end

function UI_CreateRole:OnEnable(object)

end

function UI_CreateRole:OnDestroy()
    UIPanelBase.OnDestroy(self)
    self.btn_create = nil
    self.btn_back = nil
    self.btn_girl = nil
    self.btn_boy = nil
    self.input_text = nil
    self.btn_create = nil
    self.bagImage_loader = nil
end


------------------------------

