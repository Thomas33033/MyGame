UIMain = {}
local UITable = require "UI/Main/View/Base/UIMainView"
local roleItemRender = require "UI/Main/View/UIMainRoleItemRender"

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

	self:OnInitMineNpc()
	self:OnInitEnemyNpc()
end

function UITable:ButtonClickHandler(btn)
	if self.btn_spawn == btn then

	elseif self.btn_pause == btn  then
		error("pause")
	end
end

function UITable:OnClose()

end

function UITable:OnInitMineNpc()
	local lstRoleIds = {}
	table.insert(lstRoleIds,{npcId=1001, teamId=1})
	table.insert(lstRoleIds,{npcId=1002, teamId=1})
	table.insert(lstRoleIds,{npcId=2001, teamId=1})
	table.insert(lstRoleIds,{npcId=2002, teamId=1})
	table.insert(lstRoleIds,{npcId=1005, teamId=1})

	table.insert(lstRoleIds,{npcId=4001, teamId=1})
	table.insert(lstRoleIds,{npcId=4002, teamId=1})
	table.insert(lstRoleIds,{npcId=4003, teamId=1})
	table.insert(lstRoleIds,{npcId=4004, teamId=1})

	self.myRoleList = ListView.New()
	self.myRoleList:init(roleItemRender,self.friendSlotContainer,lstRoleIds)
end

function UITable:OnInitEnemyNpc()
	local lstRoleIds = {}
	table.insert(lstRoleIds,{npcId=1003, teamId=2})
	table.insert(lstRoleIds,{npcId=1004, teamId=2})
	table.insert(lstRoleIds,{npcId=2003, teamId=2})
	table.insert(lstRoleIds,{npcId=2001, teamId=2})

	self.enemyRoleList = ListView.New()
	self.enemyRoleList:init(roleItemRender,self.enemySlotContainer,lstRoleIds)
end

