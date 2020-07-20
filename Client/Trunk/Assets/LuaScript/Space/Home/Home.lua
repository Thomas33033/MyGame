Home = {}
local SpaceTable = require "Space/Home/Base/HomeView"
require "UI/Main/View/UIMain"

function Home.Create()
	local map = {}
	setmetatable(map, { __index = SpaceTable})
	map: Create()
	Space3D.LoadSpace(map)
	return map
end

function SpaceTable:Awake()
	Home.curMap = self
end

function SpaceTable:Start()
	self:StartInit()
	UIMain.Create()
end

function SpaceTable:SetData(v)

end

function SpaceTable:ButtonClickHandler(btn)

end

function SpaceTable:OnClose()

end
