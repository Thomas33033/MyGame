---道具显示设置
UI_ItemDefaultDisplayer = class("UI_ItemDefaultDisplayer")

---@param render UI_ItemRender
---@param singleItem SingleBagItem
function UI_ItemDefaultDisplayer:SetRender(render, singleItem)
	print("function UI_ItemDefaultDisplayer.SetDisplayer(render, singleItem)")
	render.icon.url = singleItem.m_Cfg["icon"]
end

---道具消息响应
UI_ItemDefaultHandler = class("UI_ItemDefaultHandler")

function UI_ItemDefaultHandler:OnClicked(render, singleItem)
	print("function UI_ItemDefaultHandler:onClicked(render, singleItem)")
end


---道具格子
G_UI_ItemDefaultDisplayer = UI_ItemDefaultDisplayer.New();
G_UI_ItemDefaultHandler = UI_ItemDefaultHandler.New()
UI_ItemRender = UIPanelBase:new(
		{
			---@type UI_ItemDefaultDisplayer
			m_Displayer = G_UI_ItemDefaultDisplayer,
			---@type  UI_ItemDefaultHandler
			m_EventHandler = G_UI_ItemDefaultHandler,
			icon = nil,
			back = nil,
			m_SingleBagItem = nil
		}
)

function UI_ItemRender:SetDisplayer(displayer)
	if displayer == nil then
		error("UI_ItemRender is setted null displayer")
	end
	self.m_Displayer = displayer;
end

function UI_ItemRender:SetEventHandler(eventHandler)
	if eventHandler == nil then
		error("UI_ItemRender is setted null eventHandler")
  	end
	self.m_EventHandler = eventHandler;
end

---@param singleItem SingleBagItem
function UI_ItemRender:UpdateUI(singleItem)
	this.m_SingleBagItem = singleItem;
	if self.m_Displayer == nil then
		error("UI_ItemRender has no displayer")
		return
	end
	this.m_Displayer:SetRender(self, singleItem)
end


function UI_ItemRender:OnInit(args)
	self.icon = self:GetImage("icon")
end


function UI_Login:OnDestroy()
	UIPanelBase.OnDestroy(self)
	self.icon = nil
	self.back = nil
end



