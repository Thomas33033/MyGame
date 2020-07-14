UIMain = {}
local UITable = require "UI/Main/View/Base/UIMainView"

function UIMain.Create()
	local ui = {}
	setmetatable(ui, { __index = UITable})
	ui: Create()
	UI.LoadUI(ui)
	return ui
end

function UITable:Awake()

end

function UITable:Start()
self: StartInit()

end
function UITable:ButtonClickHandler(btn)

end
function UITable:OnClose()

end
