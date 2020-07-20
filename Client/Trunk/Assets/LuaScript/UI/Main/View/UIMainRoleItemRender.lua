local UIMainRoleItemRender = {}
local Render = require "Common/ItemRender"

function UIMainRoleItemRender.Create(tf)
	local t = { }
	setmetatable(t, { __index = Render})
	t:Init(tf)
	return t
end

function Render:Awake()

end

function Render:StartInit()
	self.btnClick = self.gameObject:GetComponent("Button")
	self.btnClick.onClick:AddListener(function() self:ButtonClickHandler(self.btnClick) end)
end

function Render:SetUIComponent(child)
	if child.name == "img_icon" then 
		self.img_icon = child:GetComponent("Image")
	elseif child.name == "txt_name" then 
		self.txt_name = child:GetComponent("Text")
	end
end

function Render:SetData(data)
	self.npcId = data.npcId
	self.teamId = data.teamId
	local cfgNpc = cfg_npc[self.npcId]

	self.img_icon.sprite = self:LoadSprite("Atlas/Items",cfgNpc.HeadIcon)
	self.txt_name.text = cfgNpc.Name
end

function Render:ButtonClickHandler(btn)
	error("==ButtonClickHandler==")
	if btn == self.btnClick then
		error(self.npcId,self.teamId)
		LuaTools.DragNpc(self.npcId,self.teamId)
	end
end

function Render:SetVisible(bVisible)
	self.transform.gameObject:SetActive(bVisible)
end

return UIMainRoleItemRender


