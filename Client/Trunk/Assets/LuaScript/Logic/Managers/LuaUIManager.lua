

LuaUIManager = {
	uiMap = {},
	uiGroupMap = {}
}
local this = LuaUIManager

--创建界面，并实例化lua对象
function LuaUIManager.ShowUI(uiName)
	if this.uiMap[uiName] ~= nil then
		local luaPanel = this.uiMap[uiName]
		if luaPanel ~= nil and luaPanel.gameObject ~= nil then
			luaPanel.gameObject:SetActive(true)
			return luaPanel
		end
		this.RemoveUI(uiName)
	end

	-- 创建界面
	local uiObj = UIManager.Instance:CreateUI(uiName)
	local modelName = string.gsub(uiName,"UI_","")
	local luaPanel = require("Logic/UI/"..modelName.."/"..uiName)
	luaPanel:Awake(uiObj)
	this.AddUI(uiName,luaPanel)
	return uiPanel
end

--将界面添加到界面管理器中
function LuaUIManager.AddUI(uiName, luaPanel)
	this.uiMap[uiName] = luaPanel
	local layer = 2000 --luaPanel.layerType
	if this.uiGroupMap[layer] == nil then
		this.uiGroupMap[layer] = { }
	end
	local uiList = this.uiGroupMap[layer]
	if uiList ~= nil then
		LuaTools.TableInsert(uiList,luaPanel)
	end
	this:sortUI(layer)
end

--删除界面，并销毁lua对象
function LuaUIManager.RemoveUI(uiName)
	if this.uiMap[uiName] ~= nil then
		local luaPanel = this.uiMap[uiName]

		local layer = luaPanel.layerType
		if this.uiGroupMap[layer] then
			local array = this.uiGroupMap[layer]
			if array ~= nil then
				LuaTools.TableDelete(array,luaPanel)
			end
		end
		this.uiMap[uiName] = nil
		error("删除界面:"..uiName)
		if luaPanel ~= nil then
			UIManager.Instance:DeleteUI(luaPanel.gameObject)
			luaPanel:OnDestroy()
		end
	end
end

--界面排序
function LuaUIManager.sortUI(groupId)
	if this.uiGroupMap == nil then return end

	local uiPageList = this.uiGroupMap[groupId]
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

function LuaUIManager.ShowBox(content, okCallback, cancelCallback, okBtnText, cancelBtnText, isOneBtn,isShowCheck,CheckText)
	LuaUIManager:ShowUI("BoxWnd");
	local notification = {};
	notification.content = content;
	notification.okCallback = okCallback;
	notification.cancelCallback = cancelCallback;
	notification.okBtnText = okBtnText;
	notification.cancelBtnText = cancelBtnText;
	notification.isOneBtn = isOneBtn;
	notification.isShowCheck = isShowCheck;
	notification.CheckText = CheckText;
	GyNotify:SendNotification(CLIENT_NOTIFI_ID.SHOW_BOX, notification);
end


function LuaUIManager.OnDestory()  end
