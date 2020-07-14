local HomeView = { }
setmetatable(HomeView, { __index = SpaceBase})
function HomeView:Create()
	self.name = "Home"
	self.path = "Home"
end

function HomeView:StartInit()

end

function HomeView:SetUIComponent(child)

end

return HomeView
