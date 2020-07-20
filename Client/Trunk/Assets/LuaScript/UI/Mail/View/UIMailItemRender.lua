local UIMailItemRender = {}
local Render = UI.CreateRenderTable()

function UIMailItemRender.Create(tf)
	local t = { }
	setmetatable(t, Render)
	t: Init(tf)
	return t
end

function Render:Awake()
end

function Render:Start()
end

function Render:SetUIComponent(child)
	if child.name == "mailIconImg" then 
		self.mailIconImg = child:GetComponent("Image")
	elseif child.name == "mailNameText" then 
		self.mailNameText = child:GetComponent("Text")
	elseif child.name == "timeText" then 
		self.timeText = child:GetComponent("Text")
	elseif child.name == "gettedIconImg" then 
		self.gettedIconImg = child:GetComponent("Image")
	end
end

function Render:ButtonClickHandler(btn)
end

function Render:SetVisible(bVisible)
	self.transform.gameObject:SetActive(bVisible)
end

return UIMailItemRender