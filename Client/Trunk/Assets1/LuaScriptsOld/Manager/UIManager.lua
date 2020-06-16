EUIPageLayerType = {
	-- 主界面层级
	Base = 1,
	-- 二级界面层级
	Common = 2,
	-- 浮动窗口层
	FloatPage = 3,
	-- 进度界面层
	Loading = 4

}

G_UIManager = nil
UIManager = class("UIManager", DataCtrlBase)

-- lua构造方法
function UIManager:ctor(args)
	self.super.ctor(self)
	self.dataList = { }
	self.uiMap = { }
	self.uiGroupMap = { }
	G_UIManager = self;
end

function UIManager:ShowUI(uiName)
	if self.uiMap[uiName] ~= nil then
		local luaPanel = self.uiMap[uiName]
		if luaPanel ~= nil and luaPanel.gameObject ~= nil then
			luaPanel.gameObject:SetActive(true)
			return luaPanel
		end
		self:RemoveUI(uiName)
	end
	require("View/"..uiName)
	-- 创建界面
	local luaPanel = CUIResMgr:CreateUI(uiName)
	return uiPanel
end

function UIManager:AddUI(uiName, luaPanel)
	self.uiMap[uiName] = luaPanel
	local layer = luaPanel.layerType
	if self.uiGroupMap[layer] == nil then
		self.uiGroupMap[layer] = { }
	end
	local uiList = self.uiGroupMap[layer]
	if uiList ~= nil then
		LuaTools.TableInsert(uiList,luaPanel)
	end
	self:sortUI(layer)
end

function UIManager:RemoveUI(uiName)
	if self.uiMap[uiName] ~= nil then
		local luaPanel = self.uiMap[uiName]

		local layer = luaPanel.layerType
		if self.uiGroupMap[layer] then
			local array = self.uiGroupMap[layer]
			if array ~= nil then
				LuaTools.TableDelete(array,luaPanel)
			end
		end
		self.uiMap[uiName] = nil
		error("删除界面:"..uiName)
		if luaPanel ~= nil then luaPanel:OnClose() end

	end
end

function UIManager:sortUI(groupId)
	if self.uiGroupMap == nil then return end

	local uiPageList = self.uiGroupMap[groupId]
	if uiPageList == nil then return end
	-- 界面按由小到大排序
	table.sort(uiPageList, function(a, b)
		--if a.order ~= b.order then return a.order < b.order end
		return a.open_time < b.open_time
	end )
	
	local index = groupId * 1000
	
	for k, v in pairs(uiPageList) do
		index = index + 1
		v:SetOrder(index)
	end

end

function UIManager:OnDestory() self.super.OnDestroy(self) end
