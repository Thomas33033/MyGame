Home = {}
local UITable = require "Space/Home/Base/HomeView"

function Home.Create()
	local map = {}
	setmetatable(map, { __index = UITable})
	map: Create()
	Space3D.LoadSpace(map)
	return map
end

function SpaceTable:Awake()
	Home.curMap = self
end

function SpaceTable:Start()
	self:StartInit()
end

function SpaceTable:SetData(v)

end

function SpaceTable:ButtonClickHandler(btn)

end

function UITable:OnClose()

end
