require "View/UIBase/UIEnum"
require "View/UIBase/UIHelper"

UIPanelBase = New:new( {
	gameObject = nil,
	transform = nil,
	uiPath = nil,
	luaPanelBase = nil,
	uiName = nil,
	
	cMainView = nil,
	-- GComponent
	cUIPanel = nil,
	-- UIPanel
	-- 界面顺序
	order = 1,
	-- 界面层级
	layerType = 1,
	open_time = 1,
} )

UIPanelBase.uiCreateTime = 0
-- 响应打开界面
function UIPanelBase:Awake(object)
	self.gameObject = object
	self.gameObject:SetActive(true)
	self.transform = object.transform
	self.luaPanelBase = self.gameObject:GetComponent('LuaPanelBase')
	self.uiName = self.luaPanelBase:GetUIName()
	self.cMainView = self.luaPanelBase:GetMainView()
	self.cUIPanel = self.luaPanelBase:GetUIPanle()
	UIPanelBase.uiCreateTime = UIPanelBase.uiCreateTime + 1
	self.open_time = UIPanelBase.uiCreateTime
	G_UIManager:AddUI(self.uiName, self)
	self:OnInit()
end

-- 1.销毁资源 2.清空缓存
function UIPanelBase:OnDestroy()
	G_UIManager:RemoveUI(self.uiName)
	self.gameObject = nil
	self.transform = nil
	self.luaPanelBase = nil
	self.uiName = nil
	self.cMainView = nil
	self.cUIPanel = nil
end


function UIPanelBase:OnInit()

end

function UIPanelBase:OnEnable()

end

function UIPanelBase:OnDisable()

end

function UIPanelBase:SetOrder(order)
	if self.luaPanelBase ~= nil then
		self.luaPanelBase:SetOrder(order);
	else
		error("luaPanelBase为空，设置界面顺序失败")
	end
end

-- 关闭界面
function UIPanelBase:OnClose()
	if self.luaPanelBase ~= nil then
		self.luaPanelBase:Dispose();
	end
end


function UIPanelBase:AddClickEvent(Gobj, func)
	self.luaPanelBase:AddClickEvent(Gobj, func)
end

--获取图片组件
function UIPanelBase:GetImage(name)
	return UIHelper.GetImage(self.cMainView,name)
end

--获取图片加载器
function UIPanelBase:GetLoader(name)
	return UIHelper.GetLoader(self.cMainView,name)
end

--获取文本组件GTextField
function UIPanelBase:GetTextField(name)
	return UIHelper.GetTextField(self.cMainView,name)
end

--获取列表组件GList
function UIPanelBase:GetList(name)
	return UIHelper.GetList(self.cMainView,name)
end

--获取按钮组件
function UIPanelBase:GetButton(name)
	return UIHelper.GetButton(self.cMainView,name)
end

function UIPanelBase:GetObject(name)
	return UIHelper.GetObject(self.cMainView,name)
end







