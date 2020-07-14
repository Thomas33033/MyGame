SpaceBase = {}
setmetatable(SpaceBase, {__index=UIBase})

function SpaceBase:Close()	
	Space3D.RemoveSpace(self)
end

